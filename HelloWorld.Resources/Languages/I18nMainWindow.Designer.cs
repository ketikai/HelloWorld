
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// 
// This template generates PublicResXFileCodeGenerator compatible code plus some
// useful extensions. 
// 
// The original version provided by ResXResourceManager is restricted to resource key names
// that are valid c# identifiers to keep this template simple (KISS!).
// 
// Us it as it is or as a scaffold to generate the code you need.
//
// As long as you have ResXResourceManager running in the background, the generated code 
// will be kept up to date.
//  
//------------------------------------------------------------------------------

namespace HelloWorld.Resources.Languages {

    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by a text template.
    // To add or remove a member, edit your .ResX file.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ResXResourceManager", "1.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class I18nMainWindow : global::Localization.ICultural, global::System.ComponentModel.INotifyPropertyChanged {

        private static readonly global::System.Lazy<I18nMainWindow> LazyInstance = new global::System.Lazy<I18nMainWindow>(() => new I18nMainWindow());

        /// <summary>
        ///   Instance of this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static I18nMainWindow Instance => LazyInstance.Value;
        
        #nullable enable
        public event global::System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;

        private void OnResourceCultureChanged()
        {
            this.PropertyChanged?.Invoke(this, new global::System.ComponentModel.PropertyChangedEventArgs(null));
        }
        
        /// <summary>
        ///   Returns ResourceManager instance.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public global::System.Resources.ResourceManager ResourceManager { get; private set; }
        
        #nullable enable
        private global::System.Globalization.CultureInfo? ResourceCulture;

        /// <summary>
        ///   Returns Culture instance or null.
        /// </summary>
        #nullable enable
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public global::System.Globalization.CultureInfo? Culture
        {
            get
            {
                return this.ResourceCulture;
            }
            set
            {
                if (this.ResourceCulture == value)
                {
                    return;
                }
                this.ResourceCulture = value;
                this.OnResourceCultureChanged();
            }
        }
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private I18nMainWindow() {
            //this.ResourceManager = new global::System.Resources.ResourceManager("HelloWorld.Resources.Languages.I18nMainWindow", typeof(I18nMainWindow).Assembly);
            this.ResourceManager = new global::Localization.ResourceManagerX("HelloWorld.Resources.resources.dll", "HelloWorld.Resources.Languages.I18nMainWindow", typeof(I18nMainWindow));
            ((global::Localization.ICulturalManager) global::System.Windows.Application.Current).AddCultural(this);
        }

        public string this[string key]
        {
            get
            {
                return this.ResourceManager.GetString(key, this.Culture) ?? string.Empty;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to "Auto-Assign"
        /// </summary>
        public string Auto_Assign
        {
            get
            {
                return this.ResourceManager.GetString("Auto-Assign", this.Culture) ?? string.Empty;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to "Auto-Gen"
        /// </summary>
        public string Auto_Gen
        {
            get
            {
                return this.ResourceManager.GetString("Auto-Gen", this.Culture) ?? string.Empty;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to "Connect"
        /// </summary>
        public string Connect
        {
            get
            {
                return this.ResourceManager.GetString("Connect", this.Culture) ?? string.Empty;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to "Disconnect"
        /// </summary>
        public string Disconnect
        {
            get
            {
                return this.ResourceManager.GetString("Disconnect", this.Culture) ?? string.Empty;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to "General Settings"
        /// </summary>
        public string General_Settings
        {
            get
            {
                return this.ResourceManager.GetString("General Settings", this.Culture) ?? string.Empty;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to "Hostname"
        /// </summary>
        public string Hostname
        {
            get
            {
                return this.ResourceManager.GetString("Hostname", this.Culture) ?? string.Empty;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to "Information..."
        /// </summary>
        public string Information___
        {
            get
            {
                return this.ResourceManager.GetString("Information...", this.Culture) ?? string.Empty;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to "Server List"
        /// </summary>
        public string Server_List
        {
            get
            {
                return this.ResourceManager.GetString("Server List", this.Culture) ?? string.Empty;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to "Virtual Network Address"
        /// </summary>
        public string Virtual_Network_Address
        {
            get
            {
                return this.ResourceManager.GetString("Virtual Network Address", this.Culture) ?? string.Empty;
            }
        }
    }

}
