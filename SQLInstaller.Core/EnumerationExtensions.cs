//-----------------------------------------------------------------------
// <copyright file="EnumerationExtensions.cs" company="JHOB Technologies, LLC">
//     Copyright © JHOB Technologies, LLC. All rights reserved.
// </copyright>
// <license>Microsoft Public License</license>
// <author>Brian Schloz</author>
//-----------------------------------------------------------------------
namespace SQLInstaller.Core
{
	using System;

	/// <summary>
	/// Extensions to Enum class providing some helper methods.
	/// </summary>
	public static class EnumerationExtensions
	{
        /// <summary>
        /// Method to determine whether or not an object has a given enum defined.
        /// </summary>
        /// <typeparam name="T">The enum to use.</typeparam>
        /// <param name="type">The enum instance containing the values to find.</param>
        /// <param name="value">The value to find.</param>
        /// <returns>A value indicating whether or not the object has a given enum defined.</returns>
		public static bool Has<T>(this Enum type, T value)
		{
			try
			{
				return ((int)(object)type & (int)(object)value) == (int)(object)value;
			}
			catch
			{
				return false;
			}
		}

        /// <summary>
        /// Method to determine whether or not an object is equal to the given enum.
        /// </summary>
        /// <typeparam name="T">The enum to use.</typeparam>
        /// <param name="type">The enum instance containing the values to compare.</param>
        /// <param name="value">The value to find.</param>
        /// <returns>A value indicating whether or not the object is equal to the given enum.</returns>
        public static bool Is<T>(this Enum type, T value)
		{
			try
			{
				return (int)(object)type == (int)(object)value;
			}
			catch
			{
				return false;
			}
		}

        /// <summary>
        /// Method to add a value to a given enum.
        /// </summary>
        /// <typeparam name="T">The enum to use.</typeparam>
        /// <param name="type">The enum instance containing the values to add.</param>
        /// <param name="value">The value to add.</param>
        /// <returns>The new value.</returns>
        public static T Add<T>(this Enum type, T value)
		{
			try
			{
				return (T)(object)((int)(object)type | (int)(object)value);
			}
			catch (Exception ex)
			{
				throw new ArgumentException(string.Format(Resources.ErrorEnumAppend, typeof(T).Name), ex);
			}
		}

        /// <summary>
        /// Method to remove a value from a given enum.
        /// </summary>
        /// <typeparam name="T">The enum to use.</typeparam>
        /// <param name="type">The enum instance containing the values to remove.</param>
        /// <param name="value">The value to remove.</param>
        /// <returns>The new value.</returns>
        public static T Remove<T>(this Enum type, T value)
		{
			try
			{
				return (T)(object)((int)(object)type & ~(int)(object)value);
			}
			catch (Exception ex)
			{
				throw new ArgumentException(string.Format(Resources.ErrorEnumRemove, typeof(T).Name), ex);
			}
		}

        /// <summary>
        /// Method to set a value from a given enum if a flag is set.
        /// </summary>
        /// <typeparam name="T">The enum to use.</typeparam>
        /// <param name="type">The enum instance containing the values to remove.</param>
        /// <param name="value">The value to remove.</param>
        /// <param name="doSet">A value indicating whether or not to set the enum.</param>
        /// <returns>The new value.</returns>
        public static T SetIf<T>(this Enum type, T value, bool doSet)
		{
            if (doSet)
            {
                return Add(type, value);
            }
            else
            {
                return Remove(type, value);
            }
		}
	}
}