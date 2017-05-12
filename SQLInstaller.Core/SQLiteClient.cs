//-----------------------------------------------------------------------
// <copyright file="SQLiteClient.cs" company="JHOB Technologies, LLC">
//     Copyright © JHOB Technologies, LLC. All rights reserved.
// </copyright>
// <license>Microsoft Public License</license>
// <author>Brian Schloz</author>
//-----------------------------------------------------------------------
namespace SQLInstaller.Core
{
	using System;
	using System.Data.Common;
	using System.IO;
	using System.Reflection;

	/// <summary>
	/// SQLite client class.
	/// </summary>
	public sealed class SQLiteClient : BaseClient
	{
        /// <summary>
        /// Initializes a new instance of the SQLiteClient class.
        /// </summary>
		public SQLiteClient()
		{
		}

        /// <summary>
        /// Method to check if database already exists (e.g. for upgrade).
        /// </summary>
        /// <returns>A value indicating if the database exists.</returns>
        public override bool CheckExists()
		{
			return File.Exists(this.GetFilePath());
		}

        /// <summary>
        /// Method to create a new database.
        /// </summary>
        public override void CreateDatabase()
		{
			return;
		}

        /// <summary>
        /// Method to execute a SQL script using the underlying data provider.
        /// </summary>
        /// <param name="script">The text of the script to execute.</param>
        /// <param name="changeDatabase">Indicates whether or not to change to the new database prior to executing the script.</param>
        public override void Execute(string script, bool changeDatabase)
		{
			base.Execute(script, false);
		}

        /// <summary>
        /// Method to execute a database script returning a scalar value.
        /// </summary>
        /// <param name="script">The text of the script to execute.</param>
        /// <param name="changeDatabase">Indicates whether or not to change to the new database prior to executing the script.</param>
        /// <returns>The scalar value returned from the database script.</returns>
        public override object ExecuteScalar(string script, bool changeDatabase)
		{
			return base.ExecuteScalar(script, false);
		}

        /// <summary>
        /// Method to drop an existing database. 
        /// </summary>
        public override void DropDatabase()
		{
			string filePath = this.GetFilePath();

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
		}

        /// <summary>
        /// Method to get the file path from the connection string.
        /// </summary>
        /// <returns>The file path.</returns>
		private string GetFilePath()
		{
			DbConnectionStringBuilder csb = new DbConnectionStringBuilder();
			csb.ConnectionString = this.ConnectionString;
			object dataSource;

            if (csb.TryGetValue(Constants.DataSource, out dataSource))
            {
                return dataSource as string;
            }
            else
            {
                throw new ArgumentException(Resources.ErrorMissingDataSource);
            }
		}
	}
}