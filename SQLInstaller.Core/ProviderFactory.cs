//-----------------------------------------------------------------------
// <copyright file="ProviderFactory.cs" company="JHOB Technologies, LLC">
//     Copyright © JHOB Technologies, LLC. All rights reserved.
// </copyright>
// <license>Microsoft Public License</license>
// <author>Brian Schloz</author>
//-----------------------------------------------------------------------
namespace SQLInstaller.Core
{
	using System;
	using System.IO;
	using System.Xml.Serialization;

	/// <summary>
	/// Provider factory class.
	/// </summary>
	[Serializable]
	[XmlRoot(Constants.ProviderFactory, Namespace = Constants.ProviderFactory)]
	public sealed class ProviderFactory
	{
        /// <summary>
        /// A collection of providers.
        /// </summary>
		private ProviderCollection providerCollection;

        /// <summary>
        /// Prevents a default instance of the ProviderFactory class from being created.
        /// </summary>
		private ProviderFactory()
		{
			this.providerCollection = new ProviderCollection();
		}

        /// <summary>
        /// Gets the provider collection.
        /// </summary>
		public ProviderCollection Providers
		{
			get { return this.providerCollection; }
		}

        /// <summary>
        /// Method to create the provider factory from the factory configuration.
        /// </summary>
        /// <param name="factoryConfig">The factory configuration.</param>
        /// <returns>A provider factory.</returns>
		public static ProviderFactory Load(string factoryConfig)
		{
			ProviderFactory factory = null;

			using (StringReader r = new StringReader(factoryConfig))
			{
				XmlRootAttribute xra = new XmlRootAttribute(Constants.ProviderFactory);
				XmlSerializer s = new XmlSerializer(typeof(ProviderFactory), null, new Type[] { }, xra, Constants.ProviderFactory);
				factory = s.Deserialize(r) as ProviderFactory;
			}

			return factory;
		}
	}
}