//-----------------------------------------------------------------------
// <copyright file="InstallDbTask.cs" company="JHOB Technologies, LLC">
//     Copyright © JHOB Technologies, LLC. All rights reserved.
// </copyright>
// <license>Microsoft Public License</license>
// <author>Brian Schloz</author>
//-----------------------------------------------------------------------
namespace SQLInstaller.Core
{
    using System;
    using System.Resources;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// install Db Task.
    /// </summary>
    public class InstallDbTask : Task
    {
        public InstallDbTask()
        {
            this.XmlFile = Constants.SQLInstallerXml;
        }

        public string XmlFile { get; set; }

        public override bool Execute()
        {
            string[] args = new string[] { this.XmlFile };
            this.Log.LogMessage(MessageImportance.High, "Starting process...");
            var task = Manager.Run(args);
            task.Wait();
            this.Log.LogMessage(MessageImportance.High, "Finishing process...");

            return task.Result == 0;
        }

    }
}