//-----------------------------------------------------------------------
// <copyright file="Script.cs" company="JHOB Technologies, LLC">
//     Copyright � JHOB Technologies, LLC. All rights reserved.
// </copyright>
// <license>GNU General Public License v3.0</license>
// <author>Brian Schloz</author>
//-----------------------------------------------------------------------
namespace JobTech.SqlInstaller.Core
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// SQL Scripts for command methods.
    /// </summary>
    [Serializable]
    public sealed class Script
    {
        /// <summary>
        /// Gets or sets the script type.
        /// </summary>
        [XmlAttribute]
        public ScriptType Type { get; set; }

        /// <summary>
        /// Gets or sets the command text.
        /// </summary>
        [XmlText]
        public string CommandText { get; set; }
    }
}