﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SQLInstaller.Console {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("SQLInstaller.Console.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to  to Version .
        /// </summary>
        internal static string AskToVersion {
            get {
                return ResourceManager.GetString("AskToVersion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Upgrade .
        /// </summary>
        internal static string AskUpgrade {
            get {
                return ResourceManager.GetString("AskUpgrade", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to  (Y/N)? .
        /// </summary>
        internal static string AskYesNo {
            get {
                return ResourceManager.GetString("AskYesNo", resourceCulture);
            }
        }
        
        internal static System.Drawing.Icon data_replace {
            get {
                object obj = ResourceManager.GetObject("data_replace", resourceCulture);
                return ((System.Drawing.Icon)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to sqlinstaller : error : {0}.
        /// </summary>
        internal static string ErrorGeneric {
            get {
                return ResourceManager.GetString("ErrorGeneric", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to . Creating a new template. Please edit as appropriate. Exiting..
        /// </summary>
        internal static string ExitingWithNewTemplate {
            get {
                return ResourceManager.GetString("ExitingWithNewTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {\rtf1\ansi\ansicpg1252\deff0\deflang1033{\fonttbl{\f0\froman\fcharset0 Times New Roman;}{\f1\fswiss\fcharset0 Arial;}}
        ///{\*\generator Msftedit 5.41.21.2500;}\viewkind4\uc1\pard\sb100\sa100\f0\fs24 Microsoft Public License (Ms-PL)\line\line This license governs use of the accompanying software. If you use the software, you accept this license. If you do not accept the license, do not use the software.\line\line 1. Definitions\line\line The terms &quot;reproduce,&quot; &quot;reproduction,&quot; &quot;derivative works,&quot; and &quot;distribu [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string License {
            get {
                return ResourceManager.GetString("License", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Missing parameters xml file:.
        /// </summary>
        internal static string MissingParmFile {
            get {
                return ResourceManager.GetString("MissingParmFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to  has already been upgraded to .
        /// </summary>
        internal static string StatusAlreadyUpgraded {
            get {
                return ResourceManager.GetString("StatusAlreadyUpgraded", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to  by .
        /// </summary>
        internal static string StatusBy {
            get {
                return ResourceManager.GetString("StatusBy", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Completed with .
        /// </summary>
        internal static string StatusCompletedWith {
            get {
                return ResourceManager.GetString("StatusCompletedWith", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Connecting to data source....
        /// </summary>
        internal static string StatusConnecting {
            get {
                return ResourceManager.GetString("StatusConnecting", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Done..
        /// </summary>
        internal static string StatusDone {
            get {
                return ResourceManager.GetString("StatusDone", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to  error(s)..
        /// </summary>
        internal static string StatusErrorCount {
            get {
                return ResourceManager.GetString("StatusErrorCount", resourceCulture);
            }
        }
    }
}