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
        class MyList : List<double> { } 

        static void Main(string[] args)
        {
            // EventPublisher attribute is used to mark target class
            IList<double> mylist = new MyList();
            // register itemAdded event
            ((IEventControl)mylist).ItemAdded += new ItemAdded(HelloLoom_ItemAdded);
            // ItemAdded event is fired
            mylist.Add(1.1);

            Console.ReadKey();
        }

        // event handler
        static void HelloLoom_ItemAdded(object sender, object item)
        {
            Console.WriteLine("\"{0}\" added to {1}.", item, sender);
        }

    }
}
