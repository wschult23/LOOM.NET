// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections;
using System.Text;

using Loom;
using System.Collections.Generic;


namespace EventPublisher
{
    /// <summary>
    /// This program shows how to use an aspect to expose an event that can be triggered by a target class.
    /// </summary>
    public class Program
    {
        static void Main(string[] args)
        {
            EventPublisher eventPublisher = new EventPublisher();
            // register to the ItemAdded event
            eventPublisher.ItemAdded += new ItemAdded(HelloLoom_ItemAdded);
                        
            // create new generic integer list and interweave the eventPublisher
            ICollection<int> list = Weaver.Create<List<int>>(eventPublisher);
            // ItemAdded event is fired
            list.Add(1);
            
            // create new non-generic list and interweave the eventPublisher
            IList array = Weaver.Create<ArrayList>(eventPublisher);
            // ItemAdded event is fired
            array.Add("foo");

            // no need for explicit interweaving with EventPublisher aspect
            // EventPublisher attribute is used to mark target class
            MyClass myclass = Weaver.Create<MyClass>();
            // register itemAdded event
            ((IEventControl)myclass).ItemAdded += new ItemAdded(HelloLoom_ItemAdded);
            // ItemAdded event is fired
            myclass.Add(1.1);

            Console.ReadKey();
        }

        // event handler
        static void HelloLoom_ItemAdded(object sender, object item)
        {
            Console.WriteLine("\"{0}\" added to {1}.", item, sender);
        }

    }
}
