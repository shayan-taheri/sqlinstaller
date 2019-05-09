//-----------------------------------------------------------------------
// <copyright file="BaseClient.cs" company="JHOB Technologies, LLC">
//     Copyright © JHOB Technologies, LLC. All rights reserved.
// </copyright>
// <license>GNU General Public License v3.0</license>
// <author>Brian Schloz</author>
//-----------------------------------------------------------------------
namespace JobTech.SqlInstaller.Core
{
    using FirebirdSql.Data.FirebirdClient;
    using MySql.Data.MySqlClient;
    using Npgsql;
    using Oracle.ManagedDataAccess.Client;
    using System;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Data.SQLite;
    using System.IO;
    using System.Reflection;
    using Teradata.Client.Provider;

    /// <summary>
    /// Base client class.
    /// </summary>
    public class BaseClient
    {
        /// <summary>
        /// Initializes a new instance of the BaseClient class.
        /// </summary>
        protected BaseClient()
        {
        }

        /// <summary>
        /// Gets the data provider.
        /// </summary>
        public Provider Provider { get; private set; }

        /// <summary>
        /// Gets the provider factory.
        /// </summary>
        public DbProviderFactory DbProviderFactory { get; private set; }

        /// <summary>
        /// Gets the provider specific database connection string.
        /// </summary>
        public string ConnectionString { get; protected set; }

        /// <summary>
        /// Gets the database name.
        /// </summary>
        public string Database { get; private set; }

        /// <summary>
        /// Factory method for creating the installer client.
        /// </summary>
        /// <param name="parameters">The parameters to use during the install.</param>
        /// <returns>The client to use for install</returns>
        public static BaseClient Create(Parameters parameters)
        {
            BaseClient client = null;

            switch (parameters.Provider.Name.ToLowerInvariant())
            {
                case Constants.SqlServer:
                    client = new BaseClient() { DbProviderFactory = SqlClientFactory.Instance };
                    break;
                case Constants.MySql:
                    client = new BaseClient() { DbProviderFactory = MySqlClientFactory.Instance};
                    break;
                case Constants.PostGres:
                    client = new BaseClient() { DbProviderFactory = NpgsqlFactory.Instance };
                    break;
                case Constants.Oracle:
                    client = new CommercialClient(Constants.OracleAlterSession) { DbProviderFactory = OracleClientFactory.Instance };
                    break;
                case Constants.Firebird:
                    client = new FirebirdClient() { DbProviderFactory = FirebirdClientFactory.Instance };
                    break;
                case Constants.Teradata:
                    client = new CommercialClient(Constants.TeradataAlterSession) { DbProviderFactory = TdFactory.Instance };
                    break;
                case Constants.Azure:
                    client = new AzureClient() { DbProviderFactory = SqlClientFactory.Instance };
                    break;
                case Constants.SQLite:
                    client = new SQLiteClient() { DbProviderFactory = SQLiteFactory.Instance };
                    break;
                default:
                    break;
            }

            if (client == null)
            {
                throw new ArgumentException(Resources.ErrorUnknownProvider + parameters.Provider.Name);
            }

            string provConfig = Resources.ProviderFactory;
            string provConfigPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Constants.ProviderFactory + Constants.XmlExt);

            if (File.Exists(provConfigPath))
            {
                using (StreamReader r = new StreamReader(provConfigPath))
                {
                    provConfig = r.ReadToEnd();
                }
            }

            ProviderFactory providerFactory = ProviderFactory.Load(provConfig);
            if (!providerFactory.Providers.Contains(parameters.Provider.Name))
            {
                throw new ArgumentException(Resources.ErrorUnknownProvider + parameters.Provider.Name);
            }

            client.Provider = providerFactory.Providers[parameters.Provider.Name];

            if (!string.IsNullOrEmpty(parameters.Provider.InvariantName))
            {
                client.Provider.InvariantName = parameters.Provider.InvariantName;
            }

            foreach (Script s in parameters.Provider.Scripts)
            {
                client.Provider.Scripts[s.Type].CommandText = s.CommandText;
            }

            client.ConnectionString = parameters.ConnectionString;
            client.Database = parameters.Database;

            return client;
        }

        /// <summary>
        /// Method to check if database already exists (e.g. for upgrade).
        /// </summary>
        /// <returns>A value indicating if the database exists.</returns>
        public virtual bool CheckExists()
        {
            if (!this.Provider.Scripts.Contains(ScriptType.Exists))
            {
                throw new ArgumentException(Resources.ErrorMissingStatement + ScriptType.Exists.ToString());
            }

            string commandText = string.Format(this.Provider.Scripts[ScriptType.Exists].CommandText, this.Database);

            return Convert.ToInt32(this.ExecuteScalar(commandText, false)) > 0;
        }

        /// <summary>
        /// Method to get the current database version.
        /// </summary>
        /// <returns>The database version.</returns>
        public virtual string GetVersion()
        {
            string version = string.Empty;
            if (!this.Provider.Scripts.Contains(ScriptType.GetVersion))
            {
                throw new ArgumentException(Resources.ErrorMissingStatement + ScriptType.GetVersion.ToString());
            }

            string commandText = string.Format(this.Provider.Scripts[ScriptType.GetVersion].CommandText, this.Database);

            try
            {
                object scalar = this.ExecuteScalar(commandText, true);
                if (scalar != null)
                {
                    version = scalar.ToString();
                }
            }
            catch
            {
            }

            return version;
        }

        /// <summary>
        /// Method to set the database version.
        /// </summary>
        /// <param name="version">An invariant string indicating the database version.</param>
        /// <param name="details">A string containing the user/date the install/upgrade took place.</param>
        public virtual void SetVersion(string version, string details)
        {
            if (!this.Provider.Scripts.Contains(ScriptType.SetVersion))
            {
                throw new ArgumentException(Resources.ErrorMissingStatement + ScriptType.SetVersion.ToString());
            }

            string commandText = string.Format(this.Provider.Scripts[ScriptType.SetVersion].CommandText, this.Database, version, details);

            this.Execute(commandText, true);
        }

        /// <summary>
        /// Method to drop an existing database. 
        /// </summary>
        public virtual void DropDatabase()
        {
            if (this.Provider.Scripts.Contains(ScriptType.Drop))
            {
                string commandText = string.Format(this.Provider.Scripts[ScriptType.Drop].CommandText, this.Database);
                this.Execute(commandText, false);
            }
        }

        /// <summary>
        /// Method to create a new database.
        /// </summary>
        public virtual void CreateDatabase()
        {
            if (this.Provider.Scripts.Contains(ScriptType.Create))
            {
                string commandText = string.Format(this.Provider.Scripts[ScriptType.Create].CommandText, this.Database);
                this.Execute(commandText, false);
            }
        }

        /// <summary>
        /// Method to execute a SQL script using the underlying data provider.
        /// </summary>
        /// <param name="script">The text of the script to execute.</param>
        public void Execute(string script)
        {
            this.Execute(script, true);
        }

        /// <summary>
        /// Method to execute a SQL script using the underlying data provider.
        /// </summary>
        /// <param name="script">The text of the script to execute.</param>
        /// <param name="changeDatabase">Indicates whether or not to change to the new database prior to executing the script.</param>
        public virtual void Execute(string script, bool changeDatabase)
        {
            using (DbConnection connection = this.DbProviderFactory.CreateConnection())
            {
                connection.ConnectionString = this.ConnectionString;
                connection.Open();
                if (changeDatabase)
                {
                    connection.ChangeDatabase(this.Database);
                }

                DbCommand cmd = this.DbProviderFactory.CreateCommand();
                cmd.Connection = connection;
                cmd.CommandTimeout = 0;
                cmd.CommandText = script;
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Method to execute a database script returning a scalar value.
        /// </summary>
        /// <param name="script">The text of the script to execute.</param>
        /// <param name="changeDatabase">Indicates whether or not to change to the new database prior to executing the script.</param>
        /// <returns>The scalar value returned from the database script.</returns>
        public virtual object ExecuteScalar(string script, bool changeDatabase)
        {
            object scalar = 0;

            using (DbConnection connection = this.DbProviderFactory.CreateConnection())
            {
                connection.ConnectionString = this.ConnectionString;
                connection.Open();
                if (changeDatabase)
                {
                    connection.ChangeDatabase(this.Database);
                }

                DbCommand cmd = this.DbProviderFactory.CreateCommand();
                cmd.Connection = connection;
                cmd.CommandText = script;
                scalar = cmd.ExecuteScalar();
            }

            return scalar;
        }
    }
}