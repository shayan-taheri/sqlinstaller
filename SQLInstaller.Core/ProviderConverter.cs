//-----------------------------------------------------------------------
// <copyright file="ProviderConverter.cs" company="JHOB Technologies, LLC">
//     Copyright © JHOB Technologies, LLC. All rights reserved.
// </copyright>
// <license>Microsoft Public License</license>
// <author>Brian Schloz</author>
//-----------------------------------------------------------------------
namespace SQLInstaller.Core
{
	using System;
	using System.ComponentModel;
	using System.Globalization;

	/// <summary>
	/// Type converter for provider passed as an argument/string.
	/// </summary>
	public sealed class ProviderConverter : TypeConverter
	{
        /// <summary>
        /// Method to determine if conversion is possible.
        /// </summary>
        /// <param name="context">The converter context.</param>
        /// <param name="sourceType">The type from which to convert.</param>
        /// <returns>A value indicating whether conversion is possible.</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType.Equals(typeof(string));
        }

        /// <summary>
        /// Method to convert from one type to another.
        /// </summary>
        /// <param name="context">The converter context.</param>
        /// <param name="culture">The target culture.</param>
        /// <param name="value">The value from which to convert.</param>
        /// <returns>The converted value.</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            Provider prov = new Provider();
            prov.Name = value as string;

            return prov;
        }
	}
}