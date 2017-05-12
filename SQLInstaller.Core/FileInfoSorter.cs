//-----------------------------------------------------------------------
// <copyright file="FileInfoSorter.cs" company="JHOB Technologies, LLC">
//     Copyright © JHOB Technologies, LLC. All rights reserved.
// </copyright>
// <license>Microsoft Public License</license>
// <author>Brian Schloz</author>
//-----------------------------------------------------------------------
namespace SQLInstaller.Core
{
	using System;
	using System.Collections;
	using System.IO;

	/// <summary>
	/// Basic comparison for FileInfo
	/// </summary>
	public class FileInfoSorter : IComparer
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
			return FileInfoSorter.NaturalCompare(((FileInfo)x).FullName.ToLowerInvariant(), ((FileInfo)y).FullName.ToLowerInvariant());
		}

        /// <summary>
        /// Compares two string using natural sort order
        /// </summary>
        /// <param name="x">First string to compare.</param>
        /// <param name="y">Second string to compare.</param>
        /// <returns>The relative position in sort order of the two strings.</returns>
        public static int NaturalCompare(string x, string y)
        {
            if (x == null && y == null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;

            int lx = x.Length, ly = y.Length;

            for (int mx = 0, my = 0; mx < lx && my < ly; mx++, my++)
            {
                if (char.IsDigit(x[mx]) && char.IsDigit(y[my]))
                {
                    long vx = 0, vy = 0;

                    for (; mx < lx && char.IsDigit(x[mx]); mx++)
                        vx = vx * 10 + x[mx] - '0';

                    for (; my < ly && char.IsDigit(y[my]); my++)
                        vy = vy * 10 + y[my] - '0';

                    if (vx != vy)
                        return vx > vy ? 1 : -1;
                }

                if (mx < lx && my < ly && x[mx] != y[my])
                    return x[mx] > y[my] ? 1 : -1;
            }

            return lx - ly;
        }

		#endregion
	}
}