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
using System.IO;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using Loom.CodeBuilder.Gripper;
using Loom.Configuration;
using Loom.WeaverMessages;
using Loom.Common;

namespace Loom.Compiler
{
    public class PERewriter:IDisposable
    {
        SymbolDictionary symbols;
        Dictionary<string, IModule> processedassemblies;
        List<IAssemblyReference> skippedloomassemblies;
        IMetadataHost host;
        AssemblyLoader assemblyloader;
        Weaver weaver;

        public Weaver Weaver
        {
            get { return weaver; }
        }

        string filename;
        string workingdir;

        public PERewriter(IEnumerable<string> pathlist,string filename, LoomConfig config)
        {
            this.workingdir=Path.GetDirectoryName(filename);
            this.filename = Path.GetFileName(filename);

            assemblyloader = new AssemblyLoader();
            assemblyloader.AddFolderRecursive(workingdir);
            assemblyloader.AddFolders(pathlist);

            symbols = new SymbolDictionary();
            processedassemblies = new Dictionary<string, IModule>();
            skippedloomassemblies = new List<IAssemblyReference>();
            host = new PeReader.DefaultHost();
            weaver = Weaver.Init(symbols, workingdir, config, assemblyloader);
        }

        private void CreateModule(string assemblyname)
        {
            try
            {
                if (assemblyname == "CommonLanguageRuntimeLibrary")
                {
                    assemblyname = "mscorlib";
                }
                var fi = assemblyloader.GetLocation(assemblyname);

                if (fi == null)
                {
                    throw new AspectWeaverException(4000, Properties.Resources.ERR_4000, assemblyname);
                }
                if (processedassemblies.ContainsKey(assemblyloader.GetAssemblyName(assemblyname)))
                {
                    return;
                }


                IModule/*?*/ module = host.LoadUnitFrom(fi) as IModule;

                if (module is Dummy)
                {
                    System.Diagnostics.Debug.Fail("has to exist");
                    processedassemblies.Add(assemblyname, module);
                    return;
                }

                if (IsSystemAssembly(module.Name.Value))
                {
                    processedassemblies.Add(module.Name.Value, module);

                    return;
                }


                // Schauen, ob die Assembly schon verwoben wurde
                // Das ist der Fall, wenn es eine Referenzierte Assembly gibt, die eine Pendant mit Glue-Code enthült
                // Gibt es für die aktuelle Assembly eine referenzierte Glue-Code Assembly, so wird diese für die spätere ermittlung der Metadaten benötigt
                var loomassembly = module.AssemblyReferences.Where(refass => refass.Name.Value.StartsWith("Loom.") && refass.Name.Value.Substring(5) == module.Name.Value).FirstOrDefault();

                bool bSkipassembly = false;
                if (loomassembly != null)
                {
                    skippedloomassemblies.Add(loomassembly);
                    bSkipassembly = true;
                }
                else
                {
                    foreach (var refass in module.AssemblyReferences.Where(refass => refass.Name.Value.StartsWith("Loom.")))
                    {
                        if (module.AssemblyReferences.Where(cmpass => cmpass.Name.Value == refass.Name.Value.Substring(5)).FirstOrDefault() != null)
                        {
                            bSkipassembly = true;
                            break;
                        }
                    }
                }

                if (bSkipassembly)
                {
                    AspectWeaver.WriteWeaverMessage(Loom.WeaverMessages.MessageType.Warning, 4503, Properties.Resources.WRN_4503, module.Name.Value);
                }
                else
                {
                    Console.WriteLine(module.Name);
                    module = new MetadataDeepCopier(host).Copy(module);

                    processedassemblies.Add(module.Name.Value, module);

                    foreach (var refass in module.AssemblyReferences)
                    {
                        if (refass.Name.Value == "Loom" || refass.Name.Value.StartsWith("Loom."))
                        {
                            continue;
                        }

                        CreateModule(refass.ResolvedAssembly.ModuleName.Value);
                    }
                }
            }
            catch (AspectWeaverException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new AspectWeaverException(4002, Properties.Resources.ERR_4002, assemblyname, e.Message);
            }
        }

        public void Rewrite()
        {
            Console.WriteLine("Processing assembly:");
            CreateModule(filename);

            Console.WriteLine("\nGenerate glue code for type:");
            try
            {
                foreach (var assref in skippedloomassemblies)
                {
                    var weavingmodule = host.LoadUnitFrom(assref.ResolvedAssembly.Location) as IModule;
                    foreach (var tp in weavingmodule.GetAllTypes())
                    {
                        var nsp = tp as INamespaceTypeDefinition;
                        if (nsp!=null && nsp.Container.Name.Value!=WeavingCodeNames.c_LoomNamespace)
                        {
                            if (tp.Name.Value.EndsWith(WeavingCodeNames.c_classObjectExtension))
                            {
                                symbols.AddSymbol(weavingmodule.Name.Value, tp.Name.Value.Substring(0, tp.Name.Value.Length - WeavingCodeNames.c_classObjectExtension.Length), true);
                            }
                        }
                    }
                    symbols.AddModule(assref.ContainingAssembly.Name.Value, weavingmodule);
                }
                foreach (var kv in processedassemblies)
                {
                    var module = kv.Value;
                    var assemblyname = kv.Key; 

                    var modulename = weaver.InterweaveAssembly(module.Location,assemblyname);
                    if (modulename != null)
                    {
                        var weavingmodule = host.LoadUnitFrom(modulename) as IModule;
                        symbols.AddModule(assemblyname, weavingmodule);
                    }
                }
            }
            finally
            {
                Weaver.Unload();
            }

            Console.WriteLine("\nRewriting assembly:");
            foreach (var kv in processedassemblies)
            {
                IModule module = kv.Value;
                FileInfo fi = new FileInfo(module.Location);

                // Systemasseblies nicht.
                if (module is Dummy || IsSystemAssembly(module.Name.Value))
                {
                    continue;
                }
                if (!fi.DirectoryName.StartsWith(workingdir))
                {
                    AspectWeaver.WriteWeaverMessage(Loom.WeaverMessages.MessageType.Warning, 4502,Properties.Resources.WRN_4502, module.Name.Value);
                    continue;
                }

                string newName = string.Format("{0}{1}tmp.{2}", fi.DirectoryName, Path.DirectorySeparatorChar, fi.Name);

                Console.WriteLine(fi.Name);

                PdbReader/*?*/ pdbReader = null;
                string pdbFile = Path.ChangeExtension(module.Location, "pdb");
                if (File.Exists(pdbFile))
                {
                    using (var pdbStream = File.OpenRead(pdbFile))
                    {
                        pdbReader = new PdbReader(pdbStream, host);
                    }
                }
                else
                {
                    AspectWeaver.WriteWeaverMessage(Loom.WeaverMessages.MessageType.Warning, 4501, Properties.Resources.WRN_4501, module.Name.Value);
                }


                using (pdbReader)
                {
                    var localScopeProvider = pdbReader == null ? null : new ILGenerator.LocalScopeProvider(pdbReader);

                    ILMutator mutator = new ILMutator(symbols, host, pdbReader);
                    var newmodule = mutator.Rewrite(module);
                    using (var peStream = File.Create(newName))
                    {
                        using (var pdbWriter = new PdbWriter(Path.ChangeExtension(fi.FullName, ".pdb"), pdbReader))
                        {
                            PeWriter.WritePeToStream(newmodule, host, peStream, pdbReader, localScopeProvider, pdbWriter);
                        }
                    }
                }

                File.Replace(newName, fi.FullName, null);
            }

        }

        private bool IsSystemAssembly(string assemblyname)
        {
            return (assemblyname == "mscorlib" || assemblyname.StartsWith("System"));
        }

        public void Dispose()
        {
            var disp=host as IDisposable;
            if (disp != null)
            {
                disp.Dispose();
            }
        }

        public event WeaverMessages.WeaverMessageEventHandler WeaverMessages
        {
            add
            {
                Weaver.WeaverMessages += value;
            }
            remove
            {
                Weaver.WeaverMessages -= value;
            }
        }
    }
}
