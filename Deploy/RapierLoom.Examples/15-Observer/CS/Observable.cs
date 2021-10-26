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
using Loom.JoinPoints;

namespace Observer
{
    /// <summary>
    /// User-defined attribute to mark methods inside an observable class, which should be observed.
    /// In this example, these annotated methods are changing the state of the observable object.
    /// </summary>
    public class ChangesState : Attribute { }
    
    /// <summary>
    /// This attribute can be used to annotate some class, which should be observed by one or more observers.
    /// Use the ChangesState attribute to mark relevant state changing methods inside the class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class Observable : AspectAttribute
    {
        List<IObserver> observers = new List<IObserver>();

        [IncludeAnnotated(typeof(ChangesState))] // Indicates, that only target methods annotated with an ChangesState attribute should be considered
        [Call(Advice.After)]  // the notification happens after the target method execution
        public void Notify([JPContext] Context ctx, params object[] args)
        {
            Console.WriteLine("Notify all registered observers:");
            foreach (IObserver observer in this.observers)
            {
                Console.WriteLine(" Notify {0}.", observer.Name);
                observer.Notify((ISubject)ctx.Instance); // notifies an observer handing-over the target class instance to access the changed target class state
            }
        }

        [Introduce(typeof(ISubject), ExistingInterfaces.Advice)] // Introduces the ISubject interface and implements the Register method
        public void Register(IObserver Observer)
        {
            if (!this.observers.Contains(Observer))
            {
                this.observers.Add(Observer);
                Console.WriteLine(" {0} was registered.", Observer.Name);
            }
        }

        [Introduce(typeof(ISubject), ExistingInterfaces.Advice)] // Introduces the ISubject interface and implements the Unregister method
        public void Unregister(IObserver Observer)
        {
            if (this.observers.Contains(Observer))
            {
                this.observers.Remove(Observer);
                Console.WriteLine(" {0} was unregistered.", Observer.Name);
            }
        }

        [Finalize(Advice.Before)] // interweaved with the target class destructor to unregister all observers before the target class destructor runs
        public void FinalizeObservers()
        {
            Console.WriteLine("Observable object is shut down. Unregistering running observers.");
            this.observers.ForEach(Unregister);
        }
        
    }
}
