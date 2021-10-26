// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------

namespace Loom.Compiler.Properties {
    using System;
    
    
    /// <summary>
    ///   Eine stark typisierte Ressourcenklasse zum Suchen von lokalisierten Zeichenfolgen usw.
    /// </summary>
    // Diese Klasse wurde von der StronglyTypedResourceBuilder automatisch generiert
    // -Klasse über ein Tool wie ResGen oder Visual Studio automatisch generiert.
    // Um einen Member hinzuzufügen oder zu entfernen, bearbeiten Sie die .ResX-Datei und führen dann ResGen
    // mit der /str-Option erneut aus, oder Sie erstellen Ihr VS-Projekt neu.
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
        ///   Gibt die zwischengespeicherte ResourceManager-Instanz zurück, die von dieser Klasse verwendet wird.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Loom.Compiler.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Überschreibt die CurrentUICulture-Eigenschaft des aktuellen Threads für alle
        ///   Ressourcenzuordnungen, die diese stark typisierte Ressourcenklasse verwenden.
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
        ///   Sucht eine lokalisierte Zeichenfolge, die The file &apos;{0}&apos; does not exist. ähnelt.
        /// </summary>
        internal static string ERR_4000 {
            get {
                return ResourceManager.GetString("ERR_4000", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Can&apos;t generate code for assembly &apos;{0}&apos;. ähnelt.
        /// </summary>
        internal static string ERR_4001 {
            get {
                return ResourceManager.GetString("ERR_4001", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die An unexpected error occours while loading &quot;{0}&quot;: {1} ähnelt.
        /// </summary>
        internal static string ERR_4002 {
            get {
                return ResourceManager.GetString("ERR_4002", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Unable to resolve type &apos;{0}&apos;. ähnelt.
        /// </summary>
        internal static string ERR_9200 {
            get {
                return ResourceManager.GetString("ERR_9200", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Ambiguous argument &apos;{0}&apos;.  The value was &apos;{1}&apos;. ähnelt.
        /// </summary>
        internal static string ERR_ArgumentAmbiguous {
            get {
                return ResourceManager.GetString("ERR_ArgumentAmbiguous", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Invalid argument &apos;{0}&apos; ähnelt.
        /// </summary>
        internal static string ERR_ArgumentInvalid {
            get {
                return ResourceManager.GetString("ERR_ArgumentInvalid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die No assembly filename set. ähnelt.
        /// </summary>
        internal static string ERR_FilenameNotSet {
            get {
                return ResourceManager.GetString("ERR_FilenameNotSet", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Invalid aspect configuration file: &apos;{0}&apos;. ähnelt.
        /// </summary>
        internal static string ERR_InvalidConfig {
            get {
                return ResourceManager.GetString("ERR_InvalidConfig", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Could not load the PDB file for &apos;{0}&apos; . Proceeding anyway. ähnelt.
        /// </summary>
        internal static string WRN_4501 {
            get {
                return ResourceManager.GetString("WRN_4501", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Skipping assembly &apos;{0}&apos;: Assuming that the assembly does not belong to the main assembly (it is not located in the same path). ähnelt.
        /// </summary>
        internal static string WRN_4502 {
            get {
                return ResourceManager.GetString("WRN_4502", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Skipping &apos;{0}&apos;: The assembly is up to date. ähnelt.
        /// </summary>
        internal static string WRN_4503 {
            get {
                return ResourceManager.GetString("WRN_4503", resourceCulture);
            }
        }
    }
}
