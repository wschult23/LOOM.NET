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

namespace EventPublisher
{
    public delegate void ItemAdded(object sender, object item);
    
    /// <summary>
    /// 
    /// </summary>
    public interface IEventControl
    {
        event ItemAdded ItemAdded;
    }

    /// <summary>
    /// EventPublisher aspect class
    /// </summary>
    public class EventPublisher : AspectAttribute
    {
        [Introduce(typeof(IEventControl))] // introduces the IEventControl interface to the target class using the declared ItemAdded event
        public event ItemAdded ItemAdded;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="R">generic return type parameter used as wildcard</typeparam>
        /// <typeparam name="T">generic type parameter used as wildcard</typeparam>
        /// <param name="target">object representing the target object</param>
        /// <param name="item">type parameter of any type</param>
        /// <returns>wildcard - any type is matched except void</returns>
        [Call(Advice.Before)]
        [Include("Add")]
        public void Add<T>([JPTarget] object target, T item)
        {
            // triggers the ItemAdded event
            ItemAdded(target, item);
        }
    }
}
