// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Loom;

namespace VariablesExample
{
    [Aspect1]
    [Aspect2]
    public class TargetClass
    {
        protected int visible;

        public virtual void Test()
        {
            Console.WriteLine("TargetClass: visible={0}", this.visible);  // visible=2, incremented by aspect1 and aspect2
        }
    }

    /// <summary>
    /// This example shows how to use join-point variables to share data between aspects of different 
    /// aspect classes and their target classes. Different scopes are used to show the difference between 
    /// a private and a public/protected join-point variable.
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            // interweave the target class with two different aspects of two different aspect classes
            TargetClass target = Weaver.Create<TargetClass>();
            
            // calling the test method
            target.Test();

            Console.Read();
        }
    }
}
