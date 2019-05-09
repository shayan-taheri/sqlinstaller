//-----------------------------------------------------------------------
// <copyright file="Constants.cs" company="JHOB Technologies, LLC">
//     Copyright � JHOB Technologies, LLC. All rights reserved.
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
        /// Prevents a default instance of the Constants class from being created.
        /// </summary>
        private Constants()
        {
        }
    }
}