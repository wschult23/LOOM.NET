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

namespace Authentication
{
    public class MyClass
    {
        private List<object> list = new List<object>();

        public virtual void PrintList()
        {
            foreach (object o in list)
                Console.WriteLine(o);
        }

        [AuthenticationAspect]
        public virtual void Add(object newItem)
        {
            list.Add(newItem);
            Console.WriteLine("Added {0} to list. Countet {1} items in list.", newItem, list.Count);
        }

        [AuthenticationAspect]
        public virtual void Delete(int index)
        {
            if (index < list.Count)
            {
                list.RemoveAt(index);
                Console.WriteLine("Removed item at position {0}.", index);
            }
        }
        

    }
}
