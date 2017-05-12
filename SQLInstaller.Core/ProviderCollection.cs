//-----------------------------------------------------------------------
// <copyright file="ProviderCollection.cs" company="JHOB Technologies, LLC">
//     Copyright © JHOB Technologies, LLC. All rights reserved.
// </copyright>
// <license>Microsoft Public License</license>
// <author>Brian Schloz</author>
//-----------------------------------------------------------------------
namespace SQLInstaller.Core
{
	using System;
	using System.Collections.ObjectModel;
	using System.ComponentModel;

    /// <summary>
    /// A collection of providers.
    /// </summary>
	[Serializable]
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public sealed class ProviderCollection : KeyedCollection<string, Provider>
	{
		#region Constructors

        /// <summary>
        /// Initializes a new instance of the ProviderCollection class.
        /// </summary>
		public ProviderCollection()
			: base(StringComparer.OrdinalIgnoreCase)
		{
		}

		#endregion

		#region Methods

        /// <summary>
        /// Method to get the key for an item in the colleciton.
        /// </summary>
        /// <param name="item">The item for which to retrieve the key.</param>
        /// <returns>The key for the item.</returns>
		protected override string GetKeyForItem(Provider item)
		{
			return item.Name;
		}

		#endregion
	}
}