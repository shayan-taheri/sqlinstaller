//-----------------------------------------------------------------------
// <copyright file="ScriptCollection.cs" company="JHOB Technologies, LLC">
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
    /// A collection of scripts.
    /// </summary>
	[Serializable]
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public sealed class ScriptCollection : KeyedCollection<ScriptType, Script>
	{
		#region Methods

        /// <summary>
        /// Method to get the key for an item in the colleciton.
        /// </summary>
        /// <param name="item">The item for which to retrieve the key.</param>
        /// <returns>The key for the item.</returns>
        protected override ScriptType GetKeyForItem(Script item)
		{
			return item.Type;
		}

		#endregion
	}
}