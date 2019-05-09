//-----------------------------------------------------------------------
// <copyright file="ScriptType.cs" company="JHOB Technologies, LLC">
//     Copyright � JHOB Technologies, LLC. All rights reserved.
// </copyright>
// <license>GNU General Public License v3.0</license>
// <author>Brian Schloz</author>
//-----------------------------------------------------------------------
namespace JobTech.SqlInstaller.Core
{
    /// <summary>
    /// Script types.
    /// </summary>
    public enum ScriptType
    {
        /// <summary>
        /// Script to check if database exists.
        /// </summary>
        Exists,

        /// <summary>
        /// Script to drop database.
        /// </summary>
        Drop,

        /// <summary>
        /// Script to create database.
        /// </summary>
        Create,

        /// <summary>
        /// Script to get database version.
        /// </summary>
        GetVersion,

        /// <summary>
        /// Script to set database version.
        /// </summary>
        SetVersion,
    }
}