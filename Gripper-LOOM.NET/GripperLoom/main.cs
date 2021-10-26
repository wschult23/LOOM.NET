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
using System.Linq;
using System.Xml.Serialization;
using Loom.Configuration;
using System.Xml;
using System.Collections;
using Microsoft.Win32;


namespace Loom.Compiler
{
    public class Gripper
    {
        const string aspectconfig = "-Aspects:";
        const string libpath = "-LibPath:";
        const string ignorecli = "-IgnoreCLIPath";
        const string showweavinginfo = "-ShowWeavingInfo";

        static bool bShowWeavingInfo = false;

        private static string GetOption(string option, string arg)
        {
            if (arg.ToUpper().StartsWith(option.ToUpper()))
            {
                return arg.Substring(option.Length);
            }
            return null;
        }

        private static bool CheckOption(ref string val,string option, string arg)
        {
            string optval = GetOption(option, arg);
            if (optval != null)
            {
                if (val != null)
                {
                    throw new ArgumentException(string.Format(Properties.Resources.ERR_ArgumentAmbiguous, arg, optval));
                }
                val = optval;
                return true;
            }
            return false;
        }

        private static bool CheckOption(IList<string> val, string option, string arg)
        {
            string optval = GetOption(option, arg);
            if (optval != null)
            {
                val.Add(optval);
                return true;
            }
            return false;
        }

        private static bool CheckOption(ref bool flag, string option, string arg)
        {
            string optval = GetOption(option, arg);
            if (optval != null)
            {
                if (optval!=string.Empty)
                {
                    throw new ArgumentException(string.Format(Properties.Resources.ERR_ArgumentInvalid, arg));
                }
                flag = true;
                return true;
            }
            return false;
        }

        public static int Main(string[] argv)
        {
            Console.WriteLine("\nGripper-Loom.NET aspect weaving post-compiler");
            var a = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), true).First();
            var val=typeof(AssemblyCopyrightAttribute).GetProperty("Copyright").GetValue(a, null);
            Console.WriteLine(val);
            Console.WriteLine();

            if (argv.Length == 0 || argv[0]=="-?")
            {
                Console.WriteLine("usage: {0} <executing assembly filename> [{1}<aspect config file>] [{2}] [{3}<additional library path>]*", Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location),aspectconfig, ignorecli,libpath);
                Console.WriteLine();
                return 1;
            }


            try
            {
                // Parameter auswerten
                string filename = null;
                string loomconfigfile = null;
                bool bIgnoreCli = false;
                List<string> pathlist = new List<string>();
                foreach (var arg in argv)
                {
                    if (arg.StartsWith("-"))
                    {
                        if (!(CheckOption(ref loomconfigfile, aspectconfig, arg)
                            || CheckOption(pathlist, libpath, arg)
                            || CheckOption(ref bIgnoreCli, ignorecli, arg)
                            || CheckOption(ref bShowWeavingInfo, showweavinginfo, arg)))
                        {
                            throw new Exception(string.Format(Properties.Resources.ERR_ArgumentInvalid, arg));
                        }
                    }
                    else
                    {
                        if (filename != null)
                        {
                            throw new Exception(string.Format(Properties.Resources.ERR_ArgumentInvalid, arg));
                        }
                        filename = arg;
                    }
                }
                if (filename == null)
                {
                    throw new Exception(Properties.Resources.ERR_FilenameNotSet);
                }

                // Aspektkonfiguration lesen
                LoomConfig config = null;
                if (loomconfigfile != null)
                {
                    var xml = new XmlSerializer(typeof(LoomConfig));

                    try
                    {
                        using (var reader = XmlReader.Create(new StreamReader(loomconfigfile)))
                        {
                            config = (LoomConfig)xml.Deserialize(reader);
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception(string.Format(Properties.Resources.ERR_InvalidConfig, e.Message), e);
                    }
                }

                // Libpfade
                if (!bIgnoreCli)
                {
                    var s = Environment.GetEnvironmentVariable("FrameworkDir");
                    if (s != null)
                    {
                        s += Path.DirectorySeparatorChar;
                        s += Environment.GetEnvironmentVariable("FrameworkVersion");
                        pathlist.Add(s);
                    }
                    else
                    {
                        var reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\.NETFramework");
                        var dir = string.Format("{0}v{1}.{2}.{3}", reg.GetValue("InstallRoot").ToString(), Environment.Version.Major, Environment.Version.Minor, Environment.Version.Build);
                        pathlist.Add(dir);
                    }
                }


                // Start Compile
                var pe = new PERewriter(pathlist, filename, config);
                pe.WeaverMessages += new WeaverMessages.WeaverMessageEventHandler(AspectWeaver_WeaverMessages);
                pe.Rewrite();
                Console.WriteLine();
            }
            catch (AspectWeaverException)
            {
                Console.WriteLine("Build failed.");
                return 2;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Build failed.");
                return 3;
            }

            return 0;
        }

        static void AspectWeaver_WeaverMessages(object sender, WeaverMessages.WeaverMessageEventArgs args)
        {
            if (!bShowWeavingInfo && args.Type == WeaverMessages.MessageType.Info)
            {
                return;
            }
            Console.Write(args.Type.ToString());
            Console.Write("({0:D4})",args.Code);
            Console.Write(": ");
            Console.WriteLine(args.Message);
        }
    }
}
