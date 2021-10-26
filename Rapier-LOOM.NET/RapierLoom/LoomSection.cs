// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Xml.Serialization;

namespace Loom.Configuration
{
    /// <summary>
    /// Loom configuration section fo internal purposes only.
    /// </summary>
    public sealed class LoomSection : ConfigurationSection
    {
        /// <summary>
        /// Internal use.
        /// </summary>
        public LoomSection()
        {
        }

        /// <summary>
        /// Internal use.
        /// </summary>
        [ConfigurationProperty("name",
            DefaultValue = "loom",
            IsRequired = true)]
        public string Name
        {
            get
            {
                return (string)this["name"];
            }
            set
            {
                this["name"] = value;
            }
        }


        private LoomConfig config;

        /// <summary>
        /// Internal use.
        /// </summary>
        public LoomConfig Config
        {
            get
            {
                return config;
            }
        }

        /// <summary>
        /// Internal use.
        /// </summary>
        /// <param name="reader"></param>
        protected override void DeserializeSection(System.Xml.XmlReader reader)
        {
            var xml = new XmlSerializer(typeof(LoomConfig));

            config = (LoomConfig)xml.Deserialize(reader);
        }
    }
}
