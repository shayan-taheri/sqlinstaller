//-----------------------------------------------------------------------
// <copyright file="DirInfoSorter.cs" company="JHOB Technologies, LLC">
//     Copyright © JHOB Technologies, LLC. All rights reserved.
// </copyright>
// <license>Microsoft Public License</license>
// <author>Brian Schloz</author>
//-----------------------------------------------------------------------
namespace SQLInstaller.Core
{
	using System.Collections;
	using System.IO;

	/// <summary>
	/// Basic comparer for sorting directories.
	/// </summary>
	public class DirInfoSorter : IComparer
	{
		#region IComparer Members

        /// <summary>
        /// Compares two objects.
        /// </summary>
        /// <param name="x">First object to compare.</param>
        /// <param name="y">Second object to compare.</param>
        /// <returns>The relative position in sort order of the two objects.</returns>
		public int Compare(object x, object y)
		{
            return FileInfoSorter.NaturalCompare(((DirectoryInfo)x).Name.ToLowerInvariant(), ((DirectoryInfo)y).Name.ToLowerInvariant());
		}

		#endregion
	}
}