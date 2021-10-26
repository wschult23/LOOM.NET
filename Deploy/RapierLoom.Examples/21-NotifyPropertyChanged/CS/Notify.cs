// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Loom;
using Loom.JoinPoints;
using System.ComponentModel;
using Loom.AspectProperties;
using System.Threading;

namespace NotifyPropertyChanged
{
    
    /// <summary>
    /// Implements the <see cref="INotifyPropertyChanged"/> and Raises the event whenever the set Method of a annotated property was called.
    /// </summary>
    /// <remarks>
    /// It is important to declare this aspect as "Per Instance". Otherwise we would get an aspect instance for each annotated property 
    /// and this would lead into an ambiguous introduction of the <see cref="PropertyChanged"/> event
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property)]
    [CreateAspect(Per.Instance)] 
    public class NotifyPropertyChanged : AspectAttribute
    {

        [Access(Advice.After), IncludeAll]
        public void PropertyChangedAdvice<T>([JPContext] Context ctx, T obj)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(ctx.Instance, new PropertyChangedEventArgs(ctx.CurrentMethod.Name.Substring(4)));
            }
        }

        [Introduce(typeof(INotifyPropertyChanged))]
        public event PropertyChangedEventHandler  PropertyChanged;
    }

}
