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

namespace CrossAppDomainCallAspect
{
    [Serializable]
    public class MyClass
    {
        private int i;
        
        // mark this method as CrossDomain method using the CrossDomainAttribute aspect
        // the CrossDomainAttribute aspect uses a parameter to specifiy the name of the new AppDomain
        [CrossDomain("PrivateAppDomain")]  
        public virtual int CrossDomainCall(string s)
        {
            Console.WriteLine("CrossDomainCall(\"{0}\") called in application domain \"{1}\"", s, AppDomain.CurrentDomain.FriendlyName);
            return i;
        }

        public virtual void SetValue(int i)
        {
            Console.WriteLine("SetValue({0}) called in application domain \"{1}\"", i, AppDomain.CurrentDomain.FriendlyName);
            this.i = i;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // create your object 
            MyClass mc = Weaver.Create<MyClass>();

            // a simple method call in the current AppDomain
            mc.SetValue(42);

            System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Lowest;
            // a method call in another AppDomain called foo 
            int i=mc.CrossDomainCall("foo");

            Console.WriteLine("CrossDomainCall returns {0} in application domain \"{1}\"", i, AppDomain.CurrentDomain.FriendlyName);
            Console.ReadKey();
        }
    }
}
