//-----------------------------------------------------------------------
// <copyright file="AzureClient.cs" company="JHOB Technologies, LLC">
//     Copyright © JHOB Technologies, LLC. All rights reserved.
// </copyright>
// <license>Microsoft Public License</license>
// <author>Brian Schloz</author>
//-----------------------------------------------------------------------
namespace SQLInstaller.Core
{
	using System.Data.SqlClient;

	using Microsoft.SqlServer.Management.Common;
	using Microsoft.SqlServer.Management.Smo;

	/// <summary>
	/// Microsoft SQL Server client class. Use SMO for user scripts.
	/// </summary>
	public sealed class AzureClient : BaseClient
	{
        /// <summary>
        /// Initializes a new instance of the AzureClient class.
        /// </summary>
		public AzureClient()
		{
		}

        /// <summary>
        /// Method to execute a SQL script using the underlying data provider.
        /// </summary>
        /// <param name="script">The text of the script to execute.</param>
        /// <param name="changeDatabase">Indicates whether or not to change to the new database prior to executing the script.</param>
        public override void Execute(string script, bool changeDatabase)
		{
            if (changeDatabase)
            {
                var csb = new SqlConnectionStringBuilder(this.ConnectionString);
                csb.InitialCatalog = this.Database;
                this.ConnectionString = csb.ConnectionString;
            }

            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
			{
				conn.Open();

				Server server = new Server(new ServerConnection(conn));
				server.ConnectionContext.StatementTimeout = 0;
				server.ConnectionContext.ExecuteNonQuery(script);
			}
		}
	}
}