// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using Loom.WeaverMessages;
using Loom.JoinPoints;
using Loom.JoinPoints.Implementation;
using Loom.CodeBuilder;
using Loom.AspectProperties;

namespace Loom.AspectModel
{

    /// <summary>
    /// An aspect class encapsulates the metadata for a specific aspect that will be interwoven to a target class
    /// </summary>
    internal class AspectClass
    {
        /// <summary>
        /// the introductions defined for the represented aspect
        /// </summary>
        private Dictionary<Type, AspectMemberCollection> introductions;
        /// <summary>
        /// the members defined for the represented aspect
        /// </summary>
        private AspectMemberCollection advices;
        /// <summary>
        /// the CLR type of the aspect
        /// </summary>
        private Type aspecttype;
        /// <summary>
        /// The BaseType
        /// </summary>
        private AspectClass baseaspect;
        /// <summary>
        /// Inherit this Aspect
        /// </summary>
        public readonly bool Inherited;
        /// <summary>
        /// This aspect is virtual
        /// </summary>
        public readonly bool IsVirtual=false;
        /// <summary>
        /// Aspect Instantiation
        /// </summary>
        public readonly Per AspectInstantiation=Per.InstanceAndAnnotation;

        /// <summary>
        /// instantiates a new metadata aspect class
        /// </summary>
        /// <param name="aspecttype">aspect type to create metadata</param>
        /// <param name="baseaspect">base type metadata</param>
        public AspectClass(Type aspecttype, AspectClass baseaspect)
        {
            this.introductions = new Dictionary<Type, AspectMemberCollection>();
            this.advices = new MultiItemAspectMemberCollection();
            this.aspecttype = aspecttype;
            this.baseaspect=baseaspect;

            // check for attributes
            object[] attributes = aspecttype.GetCustomAttributes(true);

            // an InstanceCreation attribute exists
            foreach (object attr in attributes)
            {
                AttributeUsageAttribute attrusage = attr as AttributeUsageAttribute;
                if (attrusage != null)
                {
                    Inherited = attrusage.Inherited;
                    continue;
                }
                CreateAspectAttribute create = attr as CreateAspectAttribute;
                if (create != null)
                {
                    this.AspectInstantiation = create.creationType;
                    continue;
                }
            }
            switch (this.AspectInstantiation)
            {
                case Per.Instance:
                case Per.Class:
                case Per.AspectClass:
                    this.IsVirtual = true;
                    break;
                case Per.InstanceAndAnnotation:
                case Per.ClassAndAnnotation:
                case Per.Annotation:
                    this.IsVirtual = false;
                    break;
                default:
                    System.Diagnostics.Debug.Fail("invalid enum");
                    break;
            }

            // encount all methods to be interwoven
            foreach (MethodInfo mi in aspecttype.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
            {
                DefineWeavingPoints(mi);
            }
            // properties
            foreach (PropertyInfo pi in aspecttype.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
            {
                DefineWeavingPoints(pi);
            }
            // events
            foreach (EventInfo ev in aspecttype.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
            {
                DefineWeavingPoints(ev);
            }

            if (baseaspect != null)
            {
                var miexclude = new HashSet<MethodInfo>();

                foreach (AspectMember am in Methods)
                {
                    var at=am.Method.GetMethodImplementationFlags();
                    miexclude.Add(am.Method.GetBaseDefinition());
                }

                baseaspect.AddAdvices(this, miexclude);
                baseaspect.AddIntroductions(this, miexclude);
            }
        }


        public IEnumerable<AspectMember> Methods
        {
            get
            {
                foreach (AspectMember am in advices)
                {
                    yield return am;
                }
                foreach (AspectMemberCollection amc in introductions.Values)
                {
                    foreach (AspectMember am in amc)
                    {
                        yield return am;
                    }
                }
            }
        }


        void AddAdvices(AspectClass child, HashSet<MethodInfo> miexlude)
        {
            foreach (AspectMember am in advices)
            {
                if (!miexlude.Contains(am.Method.GetBaseDefinition()))
                {
                    child.AddAdvice(am);
                }
            }
            if (baseaspect != null)
            {
                baseaspect.AddAdvices(child, miexlude);
            }
        }

        void AddIntroductions(AspectClass child, HashSet<MethodInfo> miexlude)
        {
            foreach (AspectMemberCollection amc in introductions.Values)
            {
                foreach (AspectMember am in amc)
                {
                    if (!miexlude.Contains(am.Method))
                    {
                        child.AddIntroduction((IntroduceAspectMethodBase)am);
                    }
                }
            }

            if (baseaspect != null)
            {
                baseaspect.AddIntroductions(child, miexlude);
            }
        }

        /// <summary>
        /// lookup joinpoint methods
        /// </summary>
        /// <param name="mi"></param>
        private void DefineWeavingPoints(MemberInfo mi)
        {
            object[] objarr = mi.GetCustomAttributes(typeof(AspectMethodAttribute), false);
            foreach (AspectMethodAttribute ama in objarr)
            {
                ama.DefineWeavingPoints(this, mi);
            }
        }

        /// <summary>
        /// the CLR typee of the aspect
        /// </summary>
        public Type AspectType
        {
            get { return aspecttype; }
        }

        /// <summary>
        /// add an introduction
        /// </summary>
        /// <param name="amb"></param>
        public void AddIntroduction(IntroduceAspectMethodBase amb)
        {
            if (!amb.Method.IsPublic)
            {
                AspectWeaver.WriteWeaverMessage(MessageType.Warning, 1,  Loom.Resources.Warnings.WARN_0201, Common.Convert.ToString(amb.Method));
                return;
            }

            AspectMemberCollection list;
            if (!introductions.TryGetValue(amb.Interface, out list))
            {
                list = new MultiItemAspectMemberCollection();
                introductions.Add(amb.Interface, list);
            }
            list.Add(amb);

            AddBaseInterfaces(amb.Interface);
        }

        /// <summary>
        /// Ensures that an introduction implements the underlying interface
        /// </summary>
        /// <param name="ifc">the interface</param>
        private void AddBaseInterfaces(Type ifc)
        {
            foreach (Type baseifc in ifc.GetInterfaces())
            {
                if (!introductions.ContainsKey(baseifc))
                {
                    introductions.Add(baseifc, new MultiItemAspectMemberCollection());
                }
                AddBaseInterfaces(baseifc);
            }
        }


        /// <summary>
        /// adds the advices
        /// </summary>
        /// <param name="am">the advice method</param>
        public void AddAdvice(AspectMember am)
        {
            if (!am.Method.IsPublic)
            {
                AspectWeaver.WriteWeaverMessage(MessageType.Warning, 1, Loom.Resources.Warnings.WARN_0201, Common.Convert.ToString(am.Method));
                return;
            }

            advices.Add(am);
        }

        /// <summary>
        /// Gets matching methodes for a joinpoint
        /// </summary>
        /// <param name="jp">the joinpoint</param>
        /// <returns>null: no match, a collection of advice methods otherwise</returns>
        public AspectMemberCollection GetAdvices(JoinPoint jp)
        {
            MultiItemAspectMemberCollection matches = new MultiItemAspectMemberCollection();
            MultiItemAspectMemberCollection secondpass = new MultiItemAspectMemberCollection();
            int pass = 1;
            // check introductions first
            MethodInfoJoinPoint jpt = jp as MethodInfoJoinPoint;
            if (jpt != null && jpt.MethodInterface != null)
            {
                AspectMemberCollection itlist;
                if (introductions.TryGetValue(jpt.MethodInterface.DeclaringType, out itlist))
                {
                    secondpass.AddRange(AddMatches(itlist, jp, pass, matches));
                }
            }

            // now the advices
            secondpass.AddRange(AddMatches(advices, jp, pass, matches));

            while (secondpass != null && secondpass.Count != 0)
            {
                pass++;
                int countbefore = secondpass.Count;
                secondpass = AddMatches(secondpass, jp, pass, matches);
                if (secondpass != null && secondpass.Count == countbefore)
                {
                    InvalidMatch(jp, secondpass);
                }
            }

            if (matches.Count > 0)
            {
#if DEBUG
                AspectWeaver.WriteWeaverMessage(MessageType.Info, 500, Resources.Info.INF_0500 , jp.ToString(), matches.ToString());
#endif
                return matches;
            }

            return null;
        }

        /// <summary>
        /// generates a error message for invalid matches
        /// </summary>
        /// <param name="jp">the joinpoint</param>
        /// <param name="secondpass">a collection of invalid matches</param>
        private void InvalidMatch(JoinPoint jp, AspectMemberCollection secondpass)
        {
            StringBuilder sb = new StringBuilder();
            foreach (AspectMember am in secondpass)
            {
                sb.Append(am.ToString());
                sb.Append('\n');
            }
            throw new AspectWeaverException(13, Resources.Errors.ERR_0013, jp.ToString(), this.ToString(), sb.ToString());
        }

        /// <summary>
        /// adds poential candidates to the list of matches
        /// </summary>
        /// <param name="source">match candidates</param>
        /// <param name="jp">the joinpoint</param>
        /// <param name="pass">number of attempts</param>
        /// <param name="matches">collection of matches</param>
        /// <returns></returns>
        private static MultiItemAspectMemberCollection AddMatches(AspectMemberCollection source, JoinPoint jp, int pass, AspectMemberCollection matches)
        {
            MultiItemAspectMemberCollection secondpass = null;
            foreach (AspectMember am in source)
            {
                switch (am.IsMatch(jp, pass, matches))
                {
                    case AspectMemberMatchResult.Match:
                        matches.Add(am);
                        break;
                    case AspectMemberMatchResult.NotYet:
                        if (secondpass == null) secondpass = new MultiItemAspectMemberCollection();
                        secondpass.Add(am);
                        break;
                }
            }
            return secondpass;
        }

        /// <summary>
        /// gets the list of interfaces for the introductions
        /// </summary>
        /// <returns></returns>
        public ICollection<Type> GetIntroducedInterfaces()
        {
            return introductions.Keys;
        }

        /// <summary>
        /// Gets the aspect method (introduction) for a given joinpoint
        /// </summary>
        /// <param name="jp">the JoinPoint</param>
        /// <returns>a lis of aspect methods</returns>
        public AspectMemberCollection GetIntroduction(MethodInfoJoinPoint jp)
        {
            SingleItemAspectMemberCollection res = new SingleItemAspectMemberCollection();
            AspectMemberCollection source = introductions[jp.MethodInterface.DeclaringType];
            System.Diagnostics.Debug.Assert(source != null, "Invalid Parameter"); // das kann nie Passieren, wenn über GetIntroducedInterfaces enumeriert wird

            int pass = 1;

            try
            {
                for (; ; )
                {
                    int countbefore = source.Count;
                    source = AddMatches(source, jp, pass, res);
                    if ((source == null) || (source.Count == 0)) break;
                    if (countbefore == source.Count)
                    {
                        InvalidMatch(jp, source);
                    }
                    pass++;
                }
            }
            catch (SingleItemAspectMemberCollection.InvalidAddOperationException ex)
            {
                throw new AspectWeaverException(1, Loom.Resources.Errors.ERR_0001, ex.Item.ToString(), jp.ToString());
            }


            if (res.Item == null)
            {
                throw new AspectWeaverException(9, Loom.Resources.Errors.ERR_0009, aspecttype.FullName, jp.Method.DeclaringType.FullName, jp.ToString());
            }
            return res;
        }

        /// <summary>
        /// Returns a string representation of this object
        /// </summary>
        /// <returns>a string description</returns>
        public override string ToString()
        {
            return Common.Convert.ToString(aspecttype);
        }
    }
}
