//-----------------------------------------------------------------------
// <copyright file="Installer.cs" company="JHOB Technologies, LLC">
//     Copyright © JHOB Technologies, LLC. All rights reserved.
// </copyright>
// <license>Microsoft Public License</license>
// <author>Brian Schloz</author>
//-----------------------------------------------------------------------
namespace SQLInstaller.Core
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Reflection;
	using System.Security.Principal;
	using System.Threading;

	/// <summary>
	/// Install class.
	/// </summary>
	public sealed class Installer : IDisposable
	{
        /// <summary>
        /// Indicates whether or not the class has been disposed.
        /// </summary>
		private bool isDisposed;
        
        /// <summary>
        /// The message queue for writing console messages from the main calling thread.
        /// </summary>
		private Queue<Progress> messages;

        /// <summary>
        /// The parameters used for this install.
        /// </summary>
		private Parameters parameters;

        /// <summary>
        /// The client used for this install.
        /// </summary>
		private BaseClient client;

        /// <summary>
        /// Initializes a new instance of the Installer class.
        /// </summary>
        /// <param name="parameters">The parameters to use.</param>
		public Installer(Parameters parameters)
		{
			this.Upgrade = Constants.RTM;
			this.messages = new Queue<Progress>();
			this.parameters = parameters;

			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(this.CurrentDomain_AssemblyResolve);
		}

        /// <summary>
        /// Gets a value indicating whether the database exists
        /// </summary>
        public bool Exists { get; private set; }

        /// <summary>
        /// Gets the version.
        /// </summary>
		public string Version { get; private set; }

        /// <summary>
        /// Gets the upgrade version.
        /// </summary>
		public string Upgrade { get; private set; }

        /// <summary>
        /// Gets the user who ran the last install/upgrad.
        /// </summary>
		public string UpgradeBy { get; private set; }

        /// <summary>
        /// Gets the count of errors that occurred during the install/upgrade.
        /// </summary>
		public int Errors { get; private set; }

        /// <summary>
        /// Gets the total number of scripts found.
        /// </summary>
		public int ScriptsTotal { get; private set; }

        /// <summary>
        /// Gets the total number of scripts that were executed.
        /// </summary>
		public int ScriptsRun { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the database is current.
        /// </summary>
		public bool IsCurrent
		{
			get
			{
                if (string.Compare(this.Version, Constants.RTM, true) == 0)
                {
                    return string.Compare(this.Upgrade, Constants.RTM, true) == 0;
                }
                else
                {
                    return string.Compare(this.Version, this.Upgrade, true) >= 0;
                }
			}
		}

        /// <summary>
        /// Gets a value indicating whether this is a clean install.
        /// </summary>
        public bool CleanInstall
        {
            get { return !this.Exists || this.parameters.Options.HasFlag(Options.Drop); }
        }

        /// <summary>
        /// Method called when an attempt is made to resolve an assembly reference.
        /// </summary>
        /// <param name="sender">The caller of this method.</param>
        /// <param name="args">The method arguments</param>
        /// <returns>A matching assembly.</returns>
		public Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
            if (args.Name.Equals(Constants.BatchParser1 + Constants.BatchParserVer + Constants.BatchParser2, StringComparison.InvariantCultureIgnoreCase))
            {
                // Workaround for locating a valid SMO assembly (since they are not distributed with the installer). 
                Assembly parser = null;
                for (int i = Constants.BatchParserVer + 1; parser == null && (i < Constants.BatchParserVer + 10); i++)
                {
                    try
                    {
                        parser = Assembly.Load(Constants.BatchParser1 + i + Constants.BatchParser2);
                    }
                    catch
                    {
                    }
                }

                return parser;
            }
            else
            {
                // Fix: do not throw exception if attempting to locate satellite resource assembly.
                if (!args.Name.StartsWith(string.Concat(Assembly.GetExecutingAssembly().GetName().Name, Constants.ResourcesExt), StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new FileLoadException(string.Format(Resources.ErrorAssembly, args.Name, Constants.CrLf));
                }

                return null;
            }
		}

        /// <summary>
        /// Prepares the installation/upgrade by selecting which files to run and upgrade path to take.
        /// </summary>
		public void Prepare()
        {
            if (this.parameters.Provider == null
                || string.IsNullOrEmpty(this.parameters.Provider.Name)
                || string.IsNullOrEmpty(this.parameters.ConnectionString)
                || string.IsNullOrEmpty(this.parameters.Database))
            {
                throw new ArgumentException(Resources.WarnMissingReq);
            }

			if (this.parameters.FileTypes.Count == 0)
			{
				this.parameters.FileTypes.Add(new FileType() { Name = "PreInstall", HaltOnError = true });
				this.parameters.FileTypes.Add(new FileType() { Name = "Table" });
				this.parameters.FileTypes.Add(new FileType() { Name = "UserDefinedFunction" });
				this.parameters.FileTypes.Add(new FileType() { Name = "View" });
				this.parameters.FileTypes.Add(new FileType() { Name = "StoredProcedure" });
				this.parameters.FileTypes.Add(new FileType() { Name = "Trigger" });
				this.parameters.FileTypes.Add(new FileType() { Name = "PostInstall" });
				this.parameters.FileTypes.Add(new FileType() { Name = "ForeignKey" });
			}

			if (string.IsNullOrEmpty(this.parameters.InstallPath))
			{
				this.parameters.InstallPath = Constants.DefaultInstallPath;
			}

			if (string.IsNullOrEmpty(this.parameters.UpgradePath))
			{
				this.parameters.UpgradePath = Constants.DefaultUpgradePath;
			}

			if (string.IsNullOrEmpty(this.parameters.ScriptExtension))
			{
				this.parameters.ScriptExtension = Constants.DefaultScriptExtension;
			}
			else if (!this.parameters.ScriptExtension.StartsWith(Constants.Dot))
			{
				this.parameters.ScriptExtension = Constants.Dot + this.parameters.ScriptExtension;
			}

			this.client = BaseClient.Create(this.parameters);
			this.Exists = this.client.CheckExists();

            if (!Directory.Exists(this.parameters.ScriptPath))
            {
                throw new ArgumentException(Resources.WarnMissingScriptDir + this.parameters.ScriptPath);
            }

            DirectoryInfo installScripts = new DirectoryInfo(Path.Combine(this.parameters.ScriptPath, this.parameters.InstallPath));
            DirectoryInfo upgradeScripts = new DirectoryInfo(Path.Combine(this.parameters.ScriptPath, this.parameters.UpgradePath));

            if (this.Exists && !this.CleanInstall)
            {
                string version = this.client.GetVersion();

                if (!string.IsNullOrWhiteSpace(version))
                {
                    string[] existingVersion = version.Split(new char[] { Constants.SplitChar }, StringSplitOptions.RemoveEmptyEntries);

                    if (existingVersion.Length == 2)
                    {
                        this.Version = existingVersion[0];
                        this.UpgradeBy = existingVersion[1];
                    }
                }
            }

            if (upgradeScripts.Exists)
            {
                DirectoryInfo[] candidates = upgradeScripts.GetDirectories();
                if (candidates.Length > 0)
                {
                    bool retry = this.parameters.Options.HasFlag(Options.Retry);
                    bool isRtm = string.Compare(this.Version, Constants.RTM, StringComparison.OrdinalIgnoreCase) == 0;

                    Array.Sort(candidates, new DirInfoSorter());
                    this.Upgrade = candidates[candidates.Length - 1].Name;

                    foreach (DirectoryInfo di in candidates)
                    {
                        if (string.Compare(di.Name, Constants.RTM, true) == 0)
                        {
                            throw new ArgumentException(Resources.InvalidReserved + Constants.RTM);
                        }

                        int comp = string.Compare(this.Version, di.Name, true);
                        if (isRtm || this.CleanInstall || (!retry && comp < 0) || (retry && comp <= 0))
                        {
                            this.ScriptsTotal += this.GetCandidateCount(di);
                        }
                    }
                }
            }

            if (this.CleanInstall && installScripts.Exists)
            {
                this.ScriptsTotal = this.GetCandidateCount(installScripts);
            }
        }

        /// <summary>
        /// Executes the installation/upgrade process.
        /// </summary>
		public void Create()
		{
			string errorMessage = string.Empty;

			try
			{
				if (this.Exists && this.CleanInstall)
				{
					this.SetProgress(StatusMessage.Start, Resources.StatusDroppingDatabase + this.parameters.Database);
					this.client.DropDatabase();
					this.SetProgress(StatusMessage.Complete, Resources.StatusDone);
                    if ((this.parameters.Options & Options.Verbose) == Options.Verbose)
                    {
                        SetProgress(StatusMessage.Progress, string.Empty, 50);
                    }

					this.Exists = false;
				}

				DirectoryInfo installScripts = new DirectoryInfo(Path.Combine(this.parameters.ScriptPath, this.parameters.InstallPath));
				DirectoryInfo upgradeScripts = new DirectoryInfo(Path.Combine(this.parameters.ScriptPath, this.parameters.UpgradePath));

				if (this.CleanInstall && (this.parameters.Options.HasFlag(Options.Create) || this.parameters.Options.HasFlag(Options.Drop)))
				{
					this.SetProgress(StatusMessage.Start, Resources.StatusCreatingDatabase + this.parameters.Database);
					this.client.CreateDatabase();

                    if (this.parameters.Options.HasFlag(Options.Verbose))
                    {
                        SetProgress(StatusMessage.Progress, string.Empty, 100);
                    }

					this.SetProgress(StatusMessage.Complete, Resources.StatusDone);
                }

                if (this.CleanInstall && installScripts.Exists)
                {
                    this.SetProgress(StatusMessage.Start, Resources.StatusInstallingDatabase + this.parameters.Database);

                    foreach (FileType fileType in this.parameters.FileTypes)
                    {
                        if (!fileType.IsDisabled)
                        {
                            if (!string.IsNullOrEmpty(fileType.Description))
                            {
                                this.SetProgress(StatusMessage.Detail, fileType.Description);
                            }

                            string searchPattern = Constants.Asterisk + Constants.Dot + fileType.Name + this.parameters.ScriptExtension;
                            this.ExecuteScripts(installScripts.GetFiles(searchPattern, SearchOption.AllDirectories), fileType.HaltOnError, fileType.IsGlobal);
                        }
                    }

                    this.SetProgress(StatusMessage.Complete, Resources.StatusDone);
                    if (this.ScriptsRun == 0)
                    {
                        this.SetProgress(StatusMessage.Detail, string.Format(Resources.WarningGeneric, Resources.WarnNoScripts));
                        this.SetProgress(StatusMessage.Complete);
                    }

    				this.client.SetVersion(this.Upgrade, WindowsIdentity.GetCurrent().Name.Replace(Constants.BackSlash, Constants.ForwardSlash) + Resources.StatusOnSeparator + DateTime.Now);
                }
				else if (upgradeScripts.Exists)
				{
                    if (this.CleanInstall)
                    {
                        this.SetProgress(StatusMessage.Start, Resources.StatusInstallingDatabase + this.parameters.Database);
                    }

					DirectoryInfo[] candidates = new DirectoryInfo[] { };

                    candidates = upgradeScripts.GetDirectories();
                    if (candidates.Length > 0)
                    {
                        if (this.ScriptsTotal == 0)
                        {
                            this.SetProgress(StatusMessage.Detail, string.Format(Resources.WarningGeneric, Resources.WarnNoNewScripts));
                            this.SetProgress(StatusMessage.Complete);
                        }

                        Array.Sort(candidates, new DirInfoSorter());
                    }
                    else
                    {
                        this.SetProgress(StatusMessage.Detail, string.Format(Resources.WarningGeneric, Resources.WarnMissingVersions));
                        this.SetProgress(StatusMessage.Complete);
                    }

                    bool retry = (this.parameters.Options & Options.Retry) == Options.Retry;
                    bool isRtm = string.Compare(this.Version, Constants.RTM, true) == 0;

                    foreach (DirectoryInfo upgradeDir in candidates)
					{
						int comp = string.Compare(this.Version, upgradeDir.Name, StringComparison.OrdinalIgnoreCase);
						if ((!retry && comp < 0) || (retry && comp <= 0) || isRtm)
						{
                            this.SetProgress(StatusMessage.Start, Resources.StatusUpgradingDatabase + upgradeDir.Name);

                            foreach (FileType fileType in this.parameters.FileTypes)
							{
								if (!fileType.IsDisabled)
								{
									if (!string.IsNullOrEmpty(fileType.Description))
									{
										this.SetProgress(StatusMessage.Detail, fileType.Description);
									}

                                    string searchPattern = Constants.Asterisk + Constants.Dot + fileType.Name + this.parameters.ScriptExtension;
                                    this.ExecuteScripts(upgradeDir.GetFiles(searchPattern, SearchOption.AllDirectories), fileType.HaltOnError, fileType.IsGlobal);
								}
							}

							this.SetProgress(StatusMessage.Complete);

							this.client.SetVersion(upgradeDir.Name, WindowsIdentity.GetCurrent().Name.Replace(Constants.BackSlash, Constants.ForwardSlash) + Resources.StatusOnSeparator + DateTime.Now);
						}
					}
				}
                else
                {
                    this.SetProgress(StatusMessage.Detail, string.Format(Resources.WarningGeneric, Resources.WarnMissingUpgrade));
                    this.SetProgress(StatusMessage.Complete);
                }
            }
			catch (Exception ex)
			{
				this.Errors++;
				this.SetProgress(StatusMessage.Complete);
				errorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
				this.SetProgress(StatusMessage.Detail, string.Format(Resources.ErrorGeneric, errorMessage));
			}
			finally
			{
				this.SetProgress(StatusMessage.Exit, errorMessage, this.Errors);
			}
		}

        /// <summary>
        /// Returns the latest progress.
        /// </summary>
        /// <param name="isRunning">Indicates whether or not we are still in the middle of an install/upgrade.</param>
        /// <returns>The latest progress.</returns>
		public Progress GetProgress(bool isRunning)
		{
			Progress prog = null;
			lock (this.messages)
			{
                if (this.messages.Count == 0)
                {
                    Monitor.Wait(this.messages, 30000);
                }

                if (this.messages.Count > 0)
                {
                    prog = this.messages.Dequeue();
                }
			}

            if (prog == null)
            {
                prog = new Progress(isRunning ? StatusMessage.Running : StatusMessage.Exit, 0);
            }

			return prog;
		}

		#region IDisposable Members

        /// <summary>
        /// Disposes the object.
        /// </summary>
		public void Dispose()
		{
			if (!this.isDisposed)
			{
				GC.SuppressFinalize(this);
				this.isDisposed = true;
			}
		}

		#endregion

        /// <summary>
        /// Helper method to execute a set of database scripts.
        /// </summary>
        /// <param name="files">An array of files to execute.</param>
        /// <param name="throwOnError">Indicates whether or not to throw an exception or just log and continue.</param>
        /// <param name="isGlobal">Indicates whether or not the script is local to the database or global to the database engine.</param>
		private void ExecuteScripts(FileInfo[] files, bool throwOnError, bool isGlobal)
		{
			Array.Sort(files, new FileInfoSorter());

			foreach (FileInfo pre in files)
			{
				StreamReader sr = null;
				try
				{
                    if ((this.parameters.Options & Options.Verbose) == Options.Verbose)
                    {
                        this.SetProgress(StatusMessage.Detail, Resources.StatusExecutingScript + pre.Name);
                    }

					sr = new StreamReader(pre.FullName);
					string script = sr.ReadToEnd();
					sr.Close();
					this.client.Execute(script, !isGlobal);
				}
				catch (Exception ex)
				{
					this.Errors++;
                    this.SetProgress(StatusMessage.Detail, string.Format(Resources.ErrorFile, pre.FullName, (ex.InnerException != null ? ex.InnerException.Message : ex.Message)));
                    if (throwOnError)
                    {
                        throw;
                    }
				}
				finally
				{
					this.ScriptsRun++;
                    if ((this.parameters.Options & Options.Verbose) == Options.Verbose && this.ScriptsTotal > 0)
                    {
                        this.SetProgress(StatusMessage.Progress, string.Empty, Convert.ToInt32(decimal.Divide((decimal)this.ScriptsRun, (decimal)this.ScriptsTotal) * 100));
                    }

                    if (sr != null)
                    {
                        sr.Close();
                    }
				}
			}
		}

        /// <summary>
        /// Method to alter the progress of the install/upgrade.
        /// </summary>
        /// <param name="status">The status information.</param>
        private void SetProgress(StatusMessage status)            
        {
            this.SetProgress(status, string.Empty, 0);
        }

        /// <summary>
        /// Method to alter the progress of the install/upgrade.
        /// </summary>
        /// <param name="status">The status information.</param>
        /// <param name="message">A message to go along with the status.</param>
		private void SetProgress(StatusMessage status, string message)
		{
            this.SetProgress(status, message, 0);
		}

        /// <summary>
        /// Method to alter the progress of the install/upgrade.
        /// </summary>
        /// <param name="status">The status information.</param>
        /// <param name="message">A message to go along with the status.</param>
        /// <param name="percent">The percent complete.</param>
        private void SetProgress(StatusMessage status, string message, int percent)
		{
            lock (this.messages)
			{
                this.messages.Enqueue(new Progress(status, percent, message));
                Monitor.Pulse(this.messages);
			}
		}

        /// <summary>
        /// Gets the count of candidate files from a directory.
        /// </summary>
        /// <param name="di">The directory from which to select candidate files.</param>
        /// <returns>The count of candidate files found.</returns>
		private int GetCandidateCount(DirectoryInfo di)
		{
			int count = 0;

			foreach (FileType fileType in this.parameters.FileTypes)
			{
				string searchPattern = Constants.Asterisk + Constants.Dot + fileType.Name + this.parameters.ScriptExtension;
				count += di.GetFiles(searchPattern, SearchOption.AllDirectories).Length;
			}

			return count;
		}
	}
}