// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Linq;

namespace Loom.CodeBuilder.Gripper
{
    [Serializable]
    public class AssemblyLoader
    {
        private List<string> folders=new List<string>();

        public AssemblyLoader()
        {
        }

        public void AddFolders(IEnumerable<string> folders)
        {
            this.folders.AddRange(folders);
        }

        public void AddFolderRecursive(string folder)
        {
            folders.Add(folder);
            AddSubFolders(folder);
        }

        private void AddSubFolders(string folder)
        {
            foreach (var s in Directory.EnumerateDirectories(folder))
            {
                folders.Add(s);     
                AddSubFolders(s);
            }
        }

        public AssemblyInfo LoadAssembly(string assemblyName)
        {
            var assembly=AppDomain.CurrentDomain.GetAssemblies().Where(ass => MakeSimpleName(ass.FullName) == assemblyName).FirstOrDefault();
            if (assembly != null)
            {
                return new AssemblyInfo(assemblyName, assembly.Location, assembly);
            }

            var file = GetLocation(assemblyName);
            if (file == null)
            {
                throw new AspectWeaverException(3002, Properties.Resources.ERR_3002, assemblyName);
            }

            try
            {
                return new AssemblyInfo(
                    assemblyName,
                    file,
                    Assembly.LoadFrom(file));
            }
            catch (Exception e)
            {
                throw new AspectWeaverException(3001, Properties.Resources.ERR_3001, file, e.Message);
            }
        }

        public string GetLocation(string assemblyName)
        {
            FileInfo fi=new FileInfo(assemblyName);
            if (fi.Exists)
            {
                return fi.FullName;
            }

            foreach (string folder in folders)
            {
                string filename = string.Format("{0}{1}{2}", folder, Path.DirectorySeparatorChar, MakeSimpleName(assemblyName));
                fi = new FileInfo(filename + ".dll");
                if (fi.Exists)
                {
                    return fi.FullName;
                }
                fi = new FileInfo(filename + ".exe");
                if (fi.Exists)
                {
                    return fi.FullName;
                }
                filename = string.Format("{0}{1}{2}", folder, Path.DirectorySeparatorChar, assemblyName);
                fi = new FileInfo(filename);
                if (fi.Exists)
                {
                    return fi.FullName;
                }
            }

            return null;
        }

        public string GetAssemblyName(string filename)
        {
            return Path.GetFileNameWithoutExtension(filename);
        }

        private string MakeSimpleName(string assemblyName)
        {
            return (assemblyName.Contains(",") ? assemblyName.Remove(assemblyName.IndexOf(',')) : assemblyName);
        }

        private Assembly getAssembly(string filename, bool reflectionOnly)
        {
            if (reflectionOnly)
                return Assembly.ReflectionOnlyLoadFrom(filename);
            else
                return Assembly.LoadFrom(filename);
        }
    }
}
