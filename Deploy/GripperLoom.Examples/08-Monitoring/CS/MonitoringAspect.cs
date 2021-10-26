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

namespace Monitoring
{
    /// <summary>
    /// This aspect shows the usage of the aspect context.
    /// </summary>
    public class MonitoringAspect : AspectAttribute
    {
        /// <summary>
        /// This aspect method is matched every target method.
        /// It starts monitoring the methods time of execution.
        /// </summary>
        /// <param name="context">represents the current call context</param>
        /// <param name="args">used as wildcard</param>
        [IncludeAll]
        [Call(Advice.Before)]
        public void Start_Monitor([JPContext] Context context, params object[] args)
        {
            // using the Tag property of the context, you can store additional information for the current context
            context.Tag = DateTime.Now;
        }

        /// <summary>
        /// This aspect method is matched also every target method.
        /// It prints the method name of the current context and the excecution time.
        /// </summary>
        /// <param name="context">represents the current call context, same as above</param>
        /// <param name="args">used as wildcard</param>
        [IncludeAll]
        [Call(Advice.After)]
        public void Stop_Monitor([JPContext] Context context, params object[] args)
        {
            // The Tag property contains the DateTime object inserted through the Start_Monitor aspect
            TimeSpan executiontime = DateTime.Now - (DateTime)context.Tag;
            Console.WriteLine("{0} done in {1}ms.", context.CurrentMethod.Name, executiontime.Milliseconds);
        }
    }

}
