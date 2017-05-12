//-----------------------------------------------------------------------
// <copyright file="FileType.cs" company="JHOB Technologies, LLC">
//     Copyright © JHOB Technologies, LLC. All rights reserved.
// </copyright>
// <license>Microsoft Public License</license>
// <author>Brian Schloz</author>
//-----------------------------------------------------------------------
namespace SQLInstaller.Core
{
	using System;
	using System.ComponentModel;
	using System.Xml.Serialization;

	/// <summary>
	/// FileType class.
	/// </summary>
	[Serializable]
	public sealed class FileType
	{
        /// <summary>
        /// Initializes a new instance of the FileType class.
        /// </summary>
		public FileType()
		{
		}

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
		[XmlAttribute]
		public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
		[XmlAttribute]
		public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the scripts belonging to the file type should be executed in the global database context.
        /// </summary>
		[XmlAttribute]
		public bool IsGlobal { get; set; }

        /// <summary>
        /// Gets a value indicating whether the corresponding property should be serialized.
        /// </summary>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[XmlIgnore]
		public bool IsGlobalSpecified
		{
			get { return this.IsGlobal; }
		}

        /// <summary>
        /// Gets or sets a value indicating whether or not scripts of this file type should halt the install/upgrade if an error occurs.
        /// </summary>
		[XmlAttribute]
		public bool HaltOnError { get; set; }

        /// <summary>
        /// Gets a value indicating whether the corresponding property should be serialized.
        /// </summary>
        [Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[XmlIgnore]
		public bool HaltOnErrorSpecified
		{
			get { return this.HaltOnError; }
		}

        /// <summary>
        /// Gets or sets a value indicating whether or not scripts of this file type should be disabled.
        /// </summary>
        [XmlAttribute]
		public bool IsDisabled { get; set; }

        /// <summary>
        /// Gets a value indicating whether the corresponding property should be serialized.
        /// </summary>
        [Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[XmlIgnore]
		public bool IsDisabledSpecified
		{
			get { return this.IsDisabled; }
		}
	}
}