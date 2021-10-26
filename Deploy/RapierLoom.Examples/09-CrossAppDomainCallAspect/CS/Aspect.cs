// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Runtime.Remoting;

using Loom;
using Loom.JoinPoints;

namespace CrossAppDomainCallAspect
{
    /// <summary>
    /// represents the remote object invoking the context object in another AppDomain
    /// </summary>
    public class CrossDomainCall : MarshalByRefObject
    {
        public object DoCallback(Context ctx, object[] args)
        {
            return ctx.Invoke(args);
        }
    }

    [Serializable]
    public class CrossDomainAttribute : AspectAttribute
    {
        CrossDomainCall appdomaincall;
        
        // holds the domain names and remote object references in a dictionary, for faster lookup
        static Dictionary<string, CrossDomainCall> domains = new Dictionary<string, CrossDomainCall>();

        /// <summary>
        /// Executes the marked target method in another AppDomain.
        /// </summary>
        /// <param name="domainname">the name of the AppDomain the method should be executed</param>
        public CrossDomainAttribute(string domainname)
        {
            if (AppDomain.CurrentDomain.FriendlyName != domainname)
            {
                lock (domains)
                {
                    // does the domain already exist?
                    if (!domains.TryGetValue(domainname, out appdomaincall))
                    {
                        // if AppDomain doesn't exists yet, create a new one
                        AppDomain appdomain = AppDomain.CreateDomain(domainname);
                        // create a new remote object reference
                        appdomaincall = (CrossDomainCall)appdomain.CreateInstanceAndUnwrap(typeof(CrossDomainCall).Assembly.FullName, typeof(CrossDomainCall).FullName);
                        // and add the domainname and the created object reference to the dictionary
                        domains.Add(domainname, appdomaincall);
                    }
                }
            }
        }

        [IncludeAll]            // includes all target methods
        [Call(Advice.Around)]   // this aspect method is called instead of the target methods
        public T DoCall<T>([JPContext] Context ctx, params object[] args)  // wildcards indicating this aspect method matches all target methods
        {
            // do a remote call if possible
            if (appdomaincall != null)
            {
                return (T)appdomaincall.DoCallback(ctx, args);
            }
            else // alternatively invoke in same AppDomain
            {
                return (T)ctx.Invoke(args);
            }
        }
    }
}
