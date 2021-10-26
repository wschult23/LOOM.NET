// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Loom.CodeBuilder.Gripper
{
    public class AssemblyInfo
    {
        public string Name;
        public string Filename;
        public Assembly Assembly;

        public AssemblyInfo(string name, string filename, Assembly assembly)
        {
            this.Name = name;
            this.Filename = filename;
            this.Assembly = assembly;
        }
    }
}
