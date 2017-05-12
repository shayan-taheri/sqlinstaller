//-----------------------------------------------------------------------
// <copyright file="Options.cs" company="JHOB Technologies, LLC">
//     Copyright © JHOB Technologies, LLC. All rights reserved.
// </copyright>
// <license>Microsoft Public License</license>
// <author>Brian Schloz</author>
//-----------------------------------------------------------------------
namespace SQLInstaller.Core
{
	using System;

	/// <summary>
	/// Options enumeration.
	/// </summary>
	[Flags]
	public enum Options
	{
		/// <summary>
		/// No options.
		/// </summary>
		None = 0x00,

		/// <summary>
		/// Create database (if it does not exist)
		/// </summary>
		Create = 0x01,

		/// <summary>
		/// Drop database (if it exists)
		/// </summary>
		Drop = 0x02,

		/// <summary>
		/// Retry the last upgrade/install operation.
		/// </summary>
		Retry = 0x04,

		/// <summary>
		/// Output all status messages.
		/// </summary>
		Verbose = 0x08,

        /// <summary>
        /// Sets the process exit code to non-zero for all errors including SQL.
        /// </summary>
        ExitCode = 0x10,
    }
}