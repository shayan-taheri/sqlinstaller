//-----------------------------------------------------------------------
// <copyright file="Constants.cs" company="JHOB Technologies, LLC">
//     Copyright © JHOB Technologies, LLC. All rights reserved.
// </copyright>
// <license>GNU General Public License v3.0</license>
// <author>Brian Schloz</author>
//-----------------------------------------------------------------------
namespace JobTech.SqlInstaller
{
    /// <summary>
    /// Constants class.
    /// </summary>
    internal sealed class Constants
    {
        /// <summary>
        /// The minimum amount to spin.
        /// </summary>
        public const double MinSpinTimeout = 250;

        /// <summary>
        /// A carriage return.
        /// </summary>
        public const string CarriageReturn = "\r";

        /// <summary>
        /// The wait text.
        /// </summary>
        public const string Wait = "...";

        /// <summary>
        /// The default xml configuration file.
        /// </summary>
        public const string SQLInstallerXml = "SQLInstaller.xml";

        /// <summary>
        /// The default database name.
        /// </summary>
        public const string SingleWhitespace = " ";

        /// <summary>
        /// The OptIn application setting.
        /// </summary>
        public const string OptIn = "OptIn";

        public const string RTM = "RTM";
        public const string SqlServer = "sqlserver";
        public const string MySql = "mysql";
        public const string SQLite = "sqlite";
        public const string Oracle = "oracle";
        public const string PostGres = "postgres";
        public const string Firebird = "firebird";
        public const string Teradata = "teradata";
        public const string Azure = "azure";
        public const string CreateDatabase = "CreateDatabase";
        public const string DropDatabase = "DropDatabase";
        public const string DataSource = "Data Source";

        public const string DefaultConfigFile = @".\SQLInstaller.xml";
        public const string DefaultProvider = "SqlServer";
        public const string DefaultConnString = "Data Source=localhost;Integrated Security=SSPI;";
        public const string CipherFile = "SQLInstaller.aes";
        public const string ProviderFactory = "ProviderFactory";

        public const char Pipe = '|';
        public const char SplitChar = ';';
        public const char NewLine = '\n';
        public const char Space = ' ';
        public const char BackSlash = '\\';
        public const char ForwardSlash = '/';
        public const string CrLf = "\r\n";
        public const string Tab = "\t";
        public const string CurrentDir = @".\";
        public const string XmlExt = ".xml";
        public const string OpenBracket = "[";
        public const string Dot = ".";
        public const string Asterisk = "*";
        public const string CloseBracketHyphen = "] - ";
        public const string ResourcesExt = ".resources";

        public const string DefaultInstallPath = "Install";
        public const string DefaultUpgradePath = "Upgrade";
        public const string DefaultScriptExtension = ".sql";

        public const string Begin = "BEGIN";
        public const string OracleAlterSession = "ALTER SESSION SET CURRENT_SCHEMA=";

        public const string DB2AlterSession = "SET SCHEMA ";
        public const string TeradataAlterSession = "DATABASE ";

        public const string FbScript = "FirebirdSql.Data.Isql.FbScript";
        public const string FbBatchExecution = "FirebirdSql.Data.Isql.FbBatchExecution";
        public const string Parse = "Parse";
        public const string Execute = "Execute";

        /// <summary>
        /// Prevents a default instance of the Constants class from being created.
        /// </summary>
        private Constants()
        {
        }
    }
}