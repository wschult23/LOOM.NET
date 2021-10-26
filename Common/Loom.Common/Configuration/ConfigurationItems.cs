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
using System.Xml.Serialization;

namespace Loom.Configuration
{
    [XmlRoot("loom")]
    [Serializable]
    public class LoomConfig
    {
        [XmlElement("assembly")]
        public AssemblyConfig[] assemblies;
    }

    [Serializable]
    public class AspectConfig
    {
        [XmlAttribute("type")]
        public string type;

        [XmlAttribute("assembly")]
        public string assembly;

        [XmlElement("parameter")]
        public ParameterWithValueConfig[] parameter;
    }
    
    [Serializable]
    public class ReflectionObjectConfig
    {
        [XmlAttribute("name")]
        public string name;

        [XmlElement("apply-aspect")]
        public AspectConfig[] aspects;
    }

    [Serializable]
    public class AssemblyConfig : ReflectionObjectConfig
    {
        [XmlElement("type")]
        public TargetTypeConfig[] types;
    }

    [Serializable]
    public class TargetTypeConfig : ReflectionObjectConfig
    {
        [XmlElement("method", typeof(MethodConfig))]
        [XmlElement("constructor", typeof(ConstructorConfig))]
        [XmlElement("property", typeof(PropertyConfig))]
        [XmlElement("event", typeof(EventConfig))]
        public MemberConfig[] member;
    }

    [Serializable]
    public class MemberConfig : ReflectionObjectConfig
    {
    }

    [Serializable]
    public class MethodConfig : MemberConfig
    {
        [XmlElement("parameter")]
        public ParameterConfig[] parameter;
    }

    [Serializable]
    public class AccessorConfig : MemberConfig
    {
    }

    [Serializable]
    public class EventConfig : AccessorConfig
    {
    }

    [Serializable]
    public class PropertyConfig : AccessorConfig
    {
    }

    [Serializable]
    public class ConstructorConfig : MethodConfig
    {
    }

    [Serializable]
    public class ParameterConfig
    {
        [XmlAttribute("type")]
        public string type;
    }

    [Serializable]
    public class ParameterWithValueConfig : ParameterConfig
    {
        [XmlAttribute("assembly")]
        public string assembly;
        [XmlText]
        public string value;
        [XmlElement("element")]
        public ParameterWithValueConfig[] arrayelements;
    }
}
