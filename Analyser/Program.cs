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
using System.Linq;

using Loom.Analyzer;
using System.IO;
using Loom;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("\nLoom.NET weaving plan analyzer");
            Console.WriteLine("(C) 2001-2012 by Wolfgang Schult, All rights reserved");
            Console.WriteLine();
            if (args.Length != 1)
            {
                Console.WriteLine("usage: {0} <assembly filename>", Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location));
                Console.WriteLine();
                Console.ReadKey();
                return;
            }

            Analyzer weaver = new Analyzer();
            AspectWeaver.WeaverMessages += new Loom.WeaverMessages.WeaverMessageEventHandler(Weaver_WeaverMessages);
            try
            {
                Assembly ass = Assembly.LoadFrom(args[0]);
                foreach (Type t in ass.GetTypes().Where(t=>t.IsClass && t.IsPublic && !t.IsSealed))
                {
                    weaver.InterweaveType(t);
                }
                Console.WriteLine("weaving succeeded.");
            }
            catch (Exception e)
            {
                Console.WriteLine("weaving failed.\n");
                while (e != null)
                {
                    Console.WriteLine(e.Message);
                    e = e.InnerException;
                }
            }
        }

        static void Weaver_WeaverMessages(object sender, Loom.WeaverMessages.WeaverMessageEventArgs args)
        {
            Console.WriteLine(args.Message);
        }
    }
}
