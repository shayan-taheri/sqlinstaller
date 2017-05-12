//-----------------------------------------------------------------------
// <copyright file="Provider.cs" company="JHOB Technologies, LLC">
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
	/// Provider factory.
	/// </summary>
	[Serializable]
	public sealed class Provider
	{
        /// <summary>
        /// The collection of scripts for this provider.
        /// </summary>
		private ScriptCollection scripts;

        /// <summary>
        /// Initializes a new instance of the Provider class.
        /// </summary>
		public Provider()
		{
			this.scripts = new ScriptCollection();
			this.Name = Constants.DefaultProvider;
		}

        /// <summary>
        /// Gets or sets the provider name.
        /// </summary>
		[XmlAttribute]
		public string Name { get; set; }

        /// <summary>
        /// Gets or sets the provider invariant name.
        /// </summary>
		[XmlAttribute]
		public string InvariantName { get; set; }

        /// <summary>
        /// Gets a value indicating whether serializaion.
        /// </summary>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[XmlIgnore]
		public bool ScriptsSpecified
		{
			get { return this.Scripts.Count > 0; }
		}

        /// <summary>
        /// Gets the collection of scripts for this provider.
        /// </summary>
		public ScriptCollection Scripts
		{
			get { return this.scripts; }
		}
	}
}