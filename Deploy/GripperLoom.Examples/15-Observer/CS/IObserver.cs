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
    /// Defines an interface to be implemented by the observer, to get notified by an observable object.
    /// </summary>
    public interface IObserver
    {
        string Name { get; }
        void Notify(ISubject observedSubject);
    }
}
