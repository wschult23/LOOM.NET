// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Observer
{
    /// <summary>
    /// To observe a target class, an observer has to implement the IObserver interface.
    /// Use the IObserver.Notify() method to handle the notify event.
    /// </summary>
    class Observer : IObserver
    {
        string name;

        public Observer(string Name)
        {
            this.name = Name;
        }

        string IObserver.Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Is invoked by the observed object, after a method annotated with the ChangesState attribute was called.
        /// </summary>
        /// <param name="ObservedSubject">the observed subject</param>
        void IObserver.Notify(ISubject ObservedSubject)
        {
            Console.WriteLine(" {0} was notified for {1}. State changed to {2}.", 
                this.name, ObservedSubject, ((TargetClass)ObservedSubject).State);
        }
    }

}
