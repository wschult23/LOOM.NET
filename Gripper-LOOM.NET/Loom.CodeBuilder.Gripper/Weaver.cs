// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;

using Loom;
using Loom.CodeBuilder.Gripper;
using Loom.JoinPoints.Implementation;
using System.IO;
using System.Security.Policy;
using Loom.Configuration;
using Loom.Common;

namespace Loom.CodeBuilder.Gripper
{

    public class Weaver : MarshalByRefObject
    {
        static AppDomain app;
        static AspectWeaver.IAspectWeaverEvent c_messages;
        private SymbolDictionary dict;
        AssemblyLoader assemblyloader;
        private string workingdir;


        /// <summary>
        /// Wird benötigt für den App-Domain-Übergang der Rücknachrichten benötigt
        /// </summary>
        private class MessageHandler : MarshalByRefObject, AspectWeaver.IAspectWeaverEvent
        {
            public event WeaverMessages.WeaverMessageEventHandler WeaverMessages;

            public void Fire(WeaverMessages.WeaverMessageEventArgs arg)
            {
                System.Diagnostics.Debug.Assert(WeaverMessages != null);
                WeaverMessages(this, arg);
            }

            public bool HasTarget
            {
                get { return WeaverMessages!=null; }
            }
        }

      
        public Weaver()
        {
        }



        private void InitInstance(AspectWeaver.IAspectWeaverEvent messages, SymbolDictionary dict, string directory, LoomConfig config, AssemblyLoader assemblyloader)
        {
            AspectWeaver.EventHandler = messages;
            JoinPointCollection.Initialize(new Loom.CodeBuilder.DynamicProxy.JoinPoints.ProxyAspectCoverageInfoFactory());
            if (config != null)
            {
                JoinPointCollection.ApplyConfiguration(config);
            }
            this.assemblyloader = assemblyloader;
            this.dict = dict;
            this.workingdir = directory;
        }



        public static Weaver Init(SymbolDictionary dict, string directory, LoomConfig config, AssemblyLoader assemblyloader)
        {
            c_messages = new MessageHandler();
            AspectWeaver.EventHandler = c_messages;
            app = AppDomain.CreateDomain("Weave",
                                        AppDomain.CurrentDomain.Evidence,
                                        directory,
                                        null, true);

            var name=typeof(Weaver).Assembly.CodeBase;
            var weaver=(Weaver)app.CreateInstanceFromAndUnwrap(name,typeof(Weaver).FullName);
            weaver.InitInstance(c_messages, dict, directory, config, assemblyloader);
            return weaver;
        }

       

        public static void Unload()
        {
            AppDomain.Unload(app);
        }



        public string InterweaveAssembly(string filename, string assemblysymbol)
        {
            return ParseAndInterweaveAssembly(GetAssembly(filename, assemblysymbol));
        }

        private Assembly GetAssembly(string filename, string assemblysymbol)
        {
            if (String.IsNullOrEmpty(filename))
            {
                var assemblyInfo = assemblyloader.LoadAssembly(assemblysymbol);
                return assemblyInfo.Assembly;
            }
            else
            {
                return Assembly.LoadFile(filename);
            }
        }


        private string ParseAndInterweaveAssembly(Assembly assembly)
        {
            AssemblyName an = assembly.GetName();
            var modulename = string.Format("Loom.{0}.dll", an.Name);
            var assemblyname = an.Name;
            an.Name = "Loom." + assemblyname;

            var targetdir = Path.GetDirectoryName(assembly.Location);
            if (!targetdir.StartsWith(workingdir))
            {
                targetdir = workingdir;
            }

            an.SetPublicKeyToken(null);
            an.SetPublicKey(null);
            AssemblyBuilder assemblybuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(an, AssemblyBuilderAccess.Save, targetdir);
            ModuleBuilder theBuilder = assemblybuilder.DefineDynamicModule(an.Name,modulename, true);

            bool bInterwoven = false;
            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsInterface || type.IsNotPublic || type.IsSealed || type.IsAbstract) continue;
                GLProxyTypeBuilder typebuilder = new GLProxyTypeBuilder(theBuilder, type);
                if (typebuilder.Interweave())
                {
                    dict.AddSymbol(assemblyname, type.FullName, true);
                    bInterwoven = true;
                }
                else
                {
                    dict.AddSymbol(assemblyname, type.FullName, false);
                }
            }

            // Post-Check if we are running against the right Version of LOOM.dll
            var loomAssem=AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName.StartsWith("Loom,"));
            if (loomAssem.Count() > 1)
            {
                var refloom = loomAssem.First().GetName().Version;
                var cmpLoom = loomAssem.Last().GetName().Version;
                throw new AspectWeaverException(3003, Properties.Resources.ERR_3003, refloom.ToString(), cmpLoom.ToString());
            }

            string filename = targetdir + Path.DirectorySeparatorChar + modulename;
            try
            {
                File.Delete(filename);
            }
            catch { }

            if (!bInterwoven)
            {
                return null;
            }
            
            assemblybuilder.Save(modulename);

            return filename;
        }

        public static event WeaverMessages.WeaverMessageEventHandler WeaverMessages
        {
            add
            {
                c_messages.WeaverMessages += value;
            }
            remove
            {
                c_messages.WeaverMessages += value;
            }
        }
    }
}
