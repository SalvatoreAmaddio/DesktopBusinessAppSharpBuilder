﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FrontEnd.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.10.0.0")]
    public sealed partial class FrontEndSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static FrontEndSettings defaultInstance = ((FrontEndSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new FrontEndSettings())));
        
        public static FrontEndSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("salvatoreamaddio94@gmail.com")]
        public string EmailUserName {
            get {
                return ((string)(this["EmailUserName"]));
            }
            set {
                this["EmailUserName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string ReportDefaultDirectory {
            get {
                return ((string)(this["ReportDefaultDirectory"]));
            }
            set {
                this["ReportDefaultDirectory"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool FirstTimeLogin {
            get {
                return ((bool)(this["FirstTimeLogin"]));
            }
            set {
                this["FirstTimeLogin"] = value;
            }
        }
    }
}
