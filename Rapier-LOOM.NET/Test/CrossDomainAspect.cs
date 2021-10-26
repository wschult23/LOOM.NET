// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using Loom;
using Loom.JoinPoints;
using System.Runtime.Remoting;

namespace Test
{
    public class CrossDomainCall : MarshalByRefObject
    {
        public object DoCallback(Context ctx, object[] args)
        {
            return ctx.Invoke(args);
        }
    }

    [Serializable]
    public class CrossDomainAttribute:AspectAttribute
    {
        CrossDomainCall appdomaincall;
        static Dictionary<string, CrossDomainCall> domains = new Dictionary<string, CrossDomainCall>();
        
        public CrossDomainAttribute(string domainname, ref Delegate del)
        {
            if (AppDomain.CurrentDomain.FriendlyName != domainname)
            {
                lock (domains)
                {
                    if (!domains.TryGetValue(domainname, out appdomaincall))
                    {
                        AppDomain appdomain = AppDomain.CreateDomain(domainname);
                        appdomaincall = (CrossDomainCall)appdomain.CreateInstanceAndUnwrap(typeof(CrossDomainCall).Assembly.FullName, typeof(CrossDomainCall).FullName);
                        domains.Add(domainname, appdomaincall);
                    }
                }
            }
        }

        [IncludeAll]
        [Call(Advice.Around)]
        public T DoCall<T>([JPContext] Context ctx, params object[] args)
        {
            if (appdomaincall != null)
            {
                return (T)appdomaincall.DoCallback(ctx, args);
            }
            else
            {
                return (T)ctx.Invoke(args);
            }
        }
    }
}
