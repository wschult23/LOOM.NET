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
    /// Applying the Observable attribute to this class, enables it to be observable by 
    /// other objects implementing the IObserver interface.
    /// </summary>
    [Observable]
    public class TargetClass
    {
        public int State = 0; // point of interest - state of the target class changes during runtime
                              // use the ChangesState attribute to inform observers in case of an change
        
        [ChangesState] // this method changes the state of the target class
        public virtual void Method1()
        {
            Console.WriteLine("Method1 is called.");
            State++; // state changed here
        }

        public virtual void Method2()
        {
            Console.WriteLine("Method2 is called.");
        }

        [ChangesState]
        public virtual void Method3()
        {
            Console.WriteLine("Method3 is called.");
            State--; // state changed here again
        }
    }
}
