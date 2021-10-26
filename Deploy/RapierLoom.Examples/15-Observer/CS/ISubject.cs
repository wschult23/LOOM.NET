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
    /// Interface introduced to the observed object by the aspect weaver using the Introduce attribute.
    /// </summary>
    public interface ISubject
    {
        void Register(IObserver Observer); // registers an observer; notifies him
        void Unregister(IObserver Observer); // unregisters an observer
    }
}
