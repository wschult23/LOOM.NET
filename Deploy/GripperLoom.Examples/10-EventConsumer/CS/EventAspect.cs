// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Loom;
using Loom.JoinPoints;
using Loom.AspectProperties;

namespace EventConsumerExample
{
    [CreateAspect(Per.AspectClass)]
    public abstract class Event : AspectAttribute
    {
        static Dictionary<Type, Event> c_events = new Dictionary<Type, Event>();


        // stores all bound method context objects
        private List<Context> boundMethods = new List<Context>();

        [Call(Advice.Initialize)]  // indicates, that this aspect method is called at object creation (Weaver.Create())
        [IncludeAll]  // includes every method of the target class
        public void InitializeEvent([JPContext] Context ctx, Event eventargs)
        {
            if (!c_events.ContainsKey(GetType()))
            {
                c_events.Add(GetType(), this);
            }

            // stores the context of the initialized target method
            boundMethods.Add(ctx);
        }

        /// <summary>
        /// Check if the method declaration was correct
        /// </summary>
        /// <param name="arg"></param>
        [Error("Invalid parameter declaration: The method must have one parameter of type Event"), IncludeNonInterwoven]
        public void Error(params object[] arg)
        {
        }

        /// <summary>
        /// Uregister the Event if the object is disposed
        /// </summary>
        /// <param name="dispctx"></param>
        [Introduce(typeof(IDisposable), ExistingInterfaces.Advice)]
        public void Dispose([JPContext] Context dispctx)
        {
            boundMethods.RemoveAll(ctx => ctx.Instance == dispctx.Instance);
            if (dispctx.HasJoinPoint())
            {
                dispctx.Call();
            }
        }

        public void Fire()
        {
            var evt = c_events[GetType()];
            if (evt == null) return; // No registered target
            foreach (Context ctx in evt.boundMethods)
            {
                // calls the originally stored context object
                ctx.Call(this);
            }
        }
    }

    public class CustomEventOne : Event { };

    public class CustomEventTwo : Event { };
}