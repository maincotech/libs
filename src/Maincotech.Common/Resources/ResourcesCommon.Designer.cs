﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Maincotech.Common.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class ResourcesCommon {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ResourcesCommon() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Maincotech.Common.Resources.ResourcesCommon", typeof(ResourcesCommon).Assembly);
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
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to disable module {0}..
        /// </summary>
        public static string ModuleManager_Disable_Failed {
            get {
                return ResourceManager.GetString("ModuleManager_Disable_Failed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Module {0} has been disabled successfully..
        /// </summary>
        public static string ModuleManager_Disable_Succeeded {
            get {
                return ResourceManager.GetString("ModuleManager_Disable_Succeeded", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to enable module {0}..
        /// </summary>
        public static string ModuleManager_Enable_Failed {
            get {
                return ResourceManager.GetString("ModuleManager_Enable_Failed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Module {0} has been enabled successfully..
        /// </summary>
        public static string ModuleManager_Enable_Succeeded {
            get {
                return ResourceManager.GetString("ModuleManager_Enable_Succeeded", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to install module {0}..
        /// </summary>
        public static string ModuleManager_Install_Failed {
            get {
                return ResourceManager.GetString("ModuleManager_Install_Failed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Module({0}) has been installed successfully..
        /// </summary>
        public static string ModuleManager_Install_Succeeded {
            get {
                return ResourceManager.GetString("ModuleManager_Install_Succeeded", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Module({0}) has been not found in configuration file..
        /// </summary>
        public static string ModuleManager_ModuleNotFoundInConfig {
            get {
                return ResourceManager.GetString("ModuleManager_ModuleNotFoundInConfig", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Module({0}) has been not found in runtime..
        /// </summary>
        public static string ModuleManager_ModuleNotFoundInRuntime {
            get {
                return ResourceManager.GetString("ModuleManager_ModuleNotFoundInRuntime", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to uninstall module {0}..
        /// </summary>
        public static string ModuleManager_Uninstall_Failed {
            get {
                return ResourceManager.GetString("ModuleManager_Uninstall_Failed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Module({0}) has been uninstalled successfully..
        /// </summary>
        public static string ModuleManager_Uninstall_Succeeded {
            get {
                return ResourceManager.GetString("ModuleManager_Uninstall_Succeeded", resourceCulture);
            }
        }
    }
}
