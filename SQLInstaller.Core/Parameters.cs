//-----------------------------------------------------------------------
// <copyright file="Parameters.cs" company="JHOB Technologies, LLC">
//     Copyright © JHOB Technologies, LLC. All rights reserved.
// </copyright>
// <license>Microsoft Public License</license>
// <author>Brian Schloz</author>
//-----------------------------------------------------------------------
namespace SQLInstaller.Core
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.IO;
	using System.IO.IsolatedStorage;
	using System.Security;
	using System.Security.Cryptography;
	using System.Security.Principal;
	using System.Text;
	using System.Xml;
	using System.Xml.Serialization;

	/// <summary>
	/// Parameters class.
	/// </summary>
	[Serializable]
	public sealed class Parameters : IDisposable
	{
		#region Fields

        /// <summary>
        /// A value indicating whether the object has been disposed.
        /// </summary>
		private bool isDisposed;

        /// <summary>
        /// The RSA service provider.
        /// </summary>
		[NonSerializedAttribute]
		private RSACryptoServiceProvider rsa;

		#endregion

		#region Constructors

        /// <summary>
        /// Initializes a new instance of the Parameters class.
        /// </summary>
        /// <param name="configPath">The path to the configuration file.</param>
        /// <param name="database">The database name.</param>
        public Parameters(string configPath)
            : this()
        {
            this.ConfigPath = configPath;
        }

        /// <summary>
        /// Initializes a new instance of the Parameters class.
        /// </summary>
		public Parameters()
		{
            this.ConnectionString = Constants.DefaultConnString;
            this.Options = this.Options.Add(Options.Create | Options.Verbose);
            this.Provider = new Provider();
            this.FileTypes = new List<FileType>();

            CspParameters csp = new CspParameters();
            csp.Flags = CspProviderFlags.UseMachineKeyStore;
            csp.KeyContainerName = Path.GetFileNameWithoutExtension(Constants.DefaultConfigFile) + WindowsIdentity.GetCurrent().Name;
            this.rsa = new RSACryptoServiceProvider(csp);

            TypeDescriptor.AddAttributes(typeof(Provider), new TypeConverterAttribute(typeof(ProviderConverter)));
            TypeDescriptor.AddAttributes(typeof(List<FileType>), new TypeConverterAttribute(typeof(FileTypeConverter)));
        }

		#endregion

		#region Properties

        /// <summary>
        /// Gets the list of file types.
        /// </summary>
		public List<FileType> FileTypes { get; set; }

        /// <summary>
        /// Gets a value indicating whether to serialize.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlIgnore]
        public bool FileTypesSpecified
        {
            get { return this.FileTypes.Count > 0; }
        }

        /// <summary>
        /// Gets or sets the configuration path.
        /// </summary>
		[XmlIgnore]
		public string ConfigPath { get; set; }

        /// <summary>
        /// Gets or sets the database name.
        /// </summary>
		[XmlElement]
        [Required(ErrorMessageResourceName = "ErrorArgument", ErrorMessageResourceType = typeof(Resources))]
        public string Database { get; set; }

        /// <summary>
        /// Gets or sets the database provider.
        /// </summary>
		public Provider Provider { get; set; }
        
        /// <summary>
        /// Gets a value indicating whether to serialize.
        /// </summary>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[XmlIgnore]
		public bool ProviderSpecified
		{
			get { return !string.IsNullOrEmpty(this.Provider.Name); }
		}

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
		[XmlElement]
		public string ConnectionString { get; set; }

        /// <summary>
        /// Gets the script path.
        /// </summary>
		[XmlIgnore]
		public string ScriptPath
		{
			get 
			{ 
				string scriptPath = Path.GetDirectoryName(this.ConfigPath);
                if (string.IsNullOrEmpty(scriptPath))
                {
                    scriptPath = Constants.CurrentDir;
                }

				return scriptPath;
			}
		}

        /// <summary>
        /// Gets or sets the options.
        /// </summary>
		[XmlAttribute]
		public Options Options { get; set; }

        /// <summary>
        /// Gets or sets the script extension.
        /// </summary>
		[XmlAttribute]
		public string ScriptExtension { get; set; }

        /// <summary>
        /// Gets or sets the install path.
        /// </summary>
		[XmlAttribute]
		public string InstallPath { get; set; }

        /// <summary>
        /// Gets or sets the upgrade path.
        /// </summary>
		[XmlAttribute]
		public string UpgradePath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the connection string has been encrypted.
        /// </summary>
		[XmlAttribute]
		public bool IsProtected { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to prompt the user for upgrade.
        /// </summary>
		[XmlAttribute]
		public bool NoPrompt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to output the configuration xml.
        /// </summary>
        [XmlIgnore]
        public bool WriteConfig { get; set; }

        /// <summary>
        /// Gets a value indicating whether to serialize.
        /// </summary>
        [Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[XmlIgnore]
		public bool NoPromptSpecified
		{
			get { return this.NoPrompt; }
		}

		#endregion

		#region Public Methods

        /// <summary>
        /// Method to load the configuration from the path.
        /// </summary>
        /// <param name="configPath">The path to use to load the configuration.</param>
        /// <returns>The parameters object.</returns>
		public static Parameters Load(string configPath)
		{
			Parameters p = null;

			XmlSerializer s = new XmlSerializer(typeof(Parameters));
			using (StreamReader r = new StreamReader(configPath))
			{
				p = s.Deserialize(r) as Parameters;
			}

			p.ConfigPath = configPath;

            if (!p.IsProtected)
            {
                p.ProtectConnectionString();
            }
            else
            {
                p.RevealConnectionString();
            }

			return p;
		}

        /// <summary>
        /// Writes the configuration to a file.
        /// </summary>
		public void Write()
		{
			StreamWriter sw = null;

			try
			{
				sw = new StreamWriter(this.ConfigPath);
				this.WriteXml(sw.BaseStream);
			}
			finally
			{
                if (sw != null)
                {
                    sw.Close();
                }
			}
		}

        /// <summary>
        /// Method to encrypt the database connection string.
        /// </summary>
        public void ProtectConnectionString()
        {
            string saveString = this.ConnectionString;

            RijndaelManaged aes = this.CreateCipher();
            ICryptoTransform transform = aes.CreateEncryptor();
            byte[] connectionStringBytes = Encoding.UTF8.GetBytes(this.ConnectionString);
            byte[] cipherText = transform.TransformFinalBlock(connectionStringBytes, 0, connectionStringBytes.Length);
            this.ConnectionString = Convert.ToBase64String(cipherText);

            this.IsProtected = true;
            this.Write();
            this.ConnectionString = saveString;
        }

		#endregion

		#region IDisposable Members

        /// <summary>
        /// Method to dispose the object.
        /// </summary>
		public void Dispose()
		{
            if (!this.isDisposed)
			{
                this.rsa.Clear();
				GC.SuppressFinalize(this);
                this.isDisposed = true;
			}
		}

		#endregion

		#region Private Methods

        /// <summary>
        /// Method to write the XML to a stream.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
		private void WriteXml(Stream stream)
		{
			XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
			ns.Add(string.Empty, string.Empty);
			XmlSerializer s = new XmlSerializer(this.GetType());
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.OmitXmlDeclaration = true;
			settings.Encoding = Encoding.UTF8;
			settings.IndentChars = Constants.Tab;
			settings.Indent = true;
			XmlWriter w = XmlWriter.Create(stream, settings);
			s.Serialize(w, this, ns);
		}
        
        /// <summary>
        /// Method to unencrypt the connection string.
        /// </summary>
		private void RevealConnectionString()
		{
			RijndaelManaged aes = this.CreateCipher();

			try
			{
				ICryptoTransform transform = aes.CreateDecryptor();
				byte[] connectionStringBytes = Convert.FromBase64String(this.ConnectionString);
				byte[] plainText = transform.TransformFinalBlock(connectionStringBytes, 0, connectionStringBytes.Length);
				this.ConnectionString = Encoding.UTF8.GetString(plainText);
			}
			catch (FormatException) 
            { 
            }
			catch (CryptographicException) 
            { 
            }

			this.IsProtected = false;
		}

        /// <summary>
        /// Method to create the encryption cipher.
        /// </summary>
        /// <returns>The encryption cypher.</returns>
		private RijndaelManaged CreateCipher()
		{
			RijndaelManaged aes = new RijndaelManaged();
			IsolatedStorageFile isf = null;
			try
			{
				isf = IsolatedStorageFile.GetUserStoreForAssembly();
			}
			catch (SecurityException)
			{
				isf = IsolatedStorageFile.GetMachineStoreForAssembly();
			}

            FileMode fm = FileMode.Open;
            FileAccess fa = FileAccess.Read;

            if (!isf.FileExists(Constants.CipherFile))
            {
                fm = FileMode.CreateNew;
                fa = FileAccess.ReadWrite;
            }
       
			using (IsolatedStorageFileStream ifs = new IsolatedStorageFileStream(Constants.CipherFile, fm, fa, isf))
			{
				string cipherData = string.Empty;
				if (fm == FileMode.Open)
				{
					using (StreamReader sr = new StreamReader(ifs))
					{
						cipherData = sr.ReadLine();
						string[] cipherInit = cipherData.Split(new char[] { Constants.Pipe });
                        if (cipherInit.Length == 2)
                        {
                            aes.IV = this.rsa.Decrypt(Convert.FromBase64String(cipherInit[0]), true);
                            aes.Key = this.rsa.Decrypt(Convert.FromBase64String(cipherInit[1]), true);
                        }
                        else
                        {
                            throw new ArgumentException(Resources.ErrorInvalidCipherData);
                        }

						sr.Close();
					}
				}
				else
				{
                    cipherData = Convert.ToBase64String(this.rsa.Encrypt(aes.IV, true))
                        + Constants.Pipe + Convert.ToBase64String(this.rsa.Encrypt(aes.Key, true));

					using (StreamWriter sw = new StreamWriter(ifs))
					{
						sw.WriteLine(cipherData);
						sw.Flush();
						sw.Close();
					}
				}
			}

			return aes;
		}

		#endregion
	}
}