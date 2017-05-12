//-----------------------------------------------------------------------
// <copyright file="StatusMessage.cs" company="JHOB Technologies, LLC">
//     Copyright © JHOB Technologies, LLC. All rights reserved.
// </copyright>
// <license>Microsoft Public License</license>
// <author>Brian Schloz</author>
//-----------------------------------------------------------------------
namespace SQLInstaller.Core
{
	/// <summary>
	/// Status message enumeration.
	/// </summary>
	public enum StatusMessage
	{
		/// <summary>
		/// Start of installation.
		/// </summary>
		Start,

        /// <summary>
        /// Output detailed status message.
        /// </summary>
        Detail,

		/// <summary>
		/// Provide progress indicator (e.g. percentage complete)
		/// </summary>
		Progress,

		/// <summary>
		/// Installation is running.
		/// </summary>
		Running,

		/// <summary>
		/// Installation has completed.
		/// </summary>
		Complete,

		/// <summary>
		/// Installation is exiting.
		/// </summary>
		Exit,
	}
}