//-----------------------------------------------------------------------
// <copyright file="FirebirdClient.cs" company="JHOB Technologies, LLC">
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
	/// Firebird client class.
	/// </summary>
	public sealed class FirebirdClient : BaseClient
	{
        /// <summary>
        /// Initializes a new instance of the FirebirdClient class.
        /// </summary>
		public FirebirdClient()
		{
		}

        /// <summary>
        /// Method to check if database already exists (e.g. for upgrade).
        /// </summary>
        /// <returns>A value indicating if the database exists.</returns>
        public override bool CheckExists()
		{
			bool exists = false;

			try
			{
				using (DbConnection connection = this.DbProviderFactory.CreateConnection())
				{
					connection.ConnectionString = this.ConnectionString;
					connection.Open();
					exists = true;
				}
			}
			catch (Exception) 
            { 
            }

			return exists;
		}

        /// <summary>
        /// Method to create a new database.
        /// </summary>
        public override void CreateDatabase()
		{
			this.ExecuteConnectionMethod(Constants.CreateDatabase);
		}

        /// <summary>
        /// Method to drop an existing database.
        /// </summary>
        public override void DropDatabase()
		{
			this.ExecuteConnectionMethod(Constants.DropDatabase);
		}

		/// <devdoc>
		/// The Firebird .NET provider gives you some helper functions when dealing
		/// with batch scripts. We use reflection here to avoid having to reference
		/// the assemblies in the project. However, it is possible that future
		/// releases of the Firebird .NET data provider would break this code 
		/// (e.g. if the class names are changed).
		/// </devdoc>
        /// <summary>
        /// Method to execute a SQL script using the underlying data provider.
        /// </summary>
        /// <param name="script">The text of the script to execute.</param>
        /// <param name="changeDatabase">Indicates whether or not to change to the new database prior to executing the script.</param>
        public override void Execute(string script, bool changeDatabase)
		{
			using (DbConnection connection = this.DbProviderFactory.CreateConnection())
			{
				connection.ConnectionString = this.ConnectionString;
				connection.Open();

				Assembly assembly = Assembly.GetAssembly(this.DbProviderFactory.GetType());
				object parser = assembly.CreateInstance(Constants.FbScript, true, BindingFlags.CreateInstance, null, new object[] { new StringReader(script) }, null, null);

				MethodInfo methodInfo = parser.GetType().GetMethod(Constants.Parse, Type.EmptyTypes);
				methodInfo.Invoke(parser, null);

				object batch = assembly.CreateInstance(Constants.FbBatchExecution, true, BindingFlags.CreateInstance, null, new object[] { connection, parser }, null, null);

				methodInfo = batch.GetType().GetMethod(Constants.Execute, Type.EmptyTypes);
				methodInfo.Invoke(batch, null);
			}
		}

        /// <summary>
        /// Executes a special method for connecting to the database.
        /// </summary>
        /// <param name="commandText">The text of the command to execute.</param>
		private void ExecuteConnectionMethod(string commandText)
		{
			Type connectionType = this.DbProviderFactory.CreateConnection().GetType();
			MethodInfo methodInfo = connectionType.GetMethod(commandText, new Type[] { typeof(string) });
            if (methodInfo == null)
            {
                throw new ApplicationException(Resources.ErrorConnectionMethod + commandText);
            }

			methodInfo.Invoke(null, new object[] { this.ConnectionString });
		}
	}
}