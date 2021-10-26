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

namespace Observer
{
    /// <summary>
    /// This example demonstrates how to implement an Observer pattern using the Introduce and the 
    /// IncludeAnnotated attribute. Several observers register to an observable subject holding a 
    /// state which should be observed. Using an user-defined attribute to mark state changing methods 
    /// inside the observed object, all registered observers are notified on a state change.
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            Run();
            GC.Collect();
            Console.Read();
        }

        public static void Run()
        {
            // create an target class through the weaver
            TargetClass target = Weaver.Create<TargetClass>();

            // create two observers
            Observer observer1 = new Observer("oberserver1");
            Observer observer2 = new Observer("oberserver2");

            Console.WriteLine("Registering observers on target class:");
            
            // cast to the ISubject interface to register the observers
            ISubject subject = (ISubject)target;
            subject.Register(observer1);
            subject.Register(observer2);
            
            // invoke some methods - notifies both observers
            target.Method1(); // changes state
            target.Method2(); // does not change state

            // unregister observer1
            subject.Unregister(observer1);

            // notifies only observer2 - observer1 was unregistered
            target.Method3();
        } // observer2 is unregistered after leaving the Run method

    }
}
