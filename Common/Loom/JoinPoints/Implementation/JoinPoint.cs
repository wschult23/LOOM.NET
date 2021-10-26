// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Loom.JoinPoints.Implementation
{
    internal interface IJoinPointInfo
    {
        JoinPoint JoinPoint { get; }
    }

    internal abstract class JoinPoint
    {
        protected Type declaringtype;

        public Type DeclaringType
        {
            get
            {
                return declaringtype;
            }
        }

        public abstract MemberInfo DeclaringMember { get; }

        protected JoinPoint(Type declaringtype, AspectCoverageInfo[] aspects)
        {
            this.declaringtype = declaringtype;
            this.aspects = aspects;
        }

        /// <summary>
        /// Die annotierten Aspekte
        /// </summary>
        private AspectCoverageInfo[] aspects;
        /// <summary>
        /// The Aspekts
        /// </summary>
        internal AspectCoverageInfo[] Aspects
        {
            get { return aspects; }
        }

        /// <summary>
        /// Erstellt eine kopie des JoinPoints für einen abgeleiteten Typen
        /// </summary>
        /// <param name="declaringtype"></param>
        /// <returns></returns>
        internal JoinPoint Clone(Type declaringtype)
        {
            JoinPoint jp = (JoinPoint)MemberwiseClone();
            jp.declaringtype = declaringtype;
            return jp;
        }
    }

    /// <summary>
    /// Ein JoinPoint-Schatten, der sich auf eine Methoden/Construktor Aufruf bezieht
    /// Wurde die Methode überschrieben (override oder durch Neuimplementation des Interfaces),
    /// sind die JoinPoints gleich
    /// </summary>
    internal abstract class MethodBaseJoinPoint : JoinPoint
    {
        protected MethodBaseJoinPoint(Type declaringtype, AspectCoverageInfo[] aspects)
            :
            base(declaringtype, aspects)
        {
        }

        /// <summary>
        /// Die Methode, wie sie für das JoinpointMatching verwendet wird
        /// Es ist die Implementation der Methode
        /// </summary>
        public abstract MethodBase Method
        {
            get;
        }

        public override MemberInfo DeclaringMember
        {
            get { return Method; }
        }
    }

    internal abstract class MethodInfoJoinPoint : MethodBaseJoinPoint
    {
        /// <summary>
        /// Die Implementation
        /// </summary>
        protected MethodInfo miimplementation;
        /// <summary>
        /// Das Interface, wenn vorhanden
        /// </summary>
        protected MethodInfo miinterface;

        public MethodInfoJoinPoint(Type declaringtype, MethodInfo miimplementation, MethodInfo miinterface, AspectCoverageInfo[] aspects) :
            base(declaringtype, aspects)
        {
            this.miimplementation = miimplementation;
            this.miinterface = miinterface;
        }

        public MethodInfo MethodImplementation
        {
            get
            {
                return miimplementation;
            }
        }

        public MethodInfo MethodInterface
        {
            get
            {
                return miinterface;
            }
        }

        public override MethodBase Method
        {
            get { return miimplementation; }
        }

        public override string ToString()
        {
            return Common.Convert.ToString(miimplementation);
        }
    }

    internal abstract class MethodJoinPoint : MethodInfoJoinPoint
    {
        public MethodJoinPoint(Type declaringtype, MethodInfo miimplementation, MethodInfo miinterface, AspectCoverageInfo[] aspects)
            :
            base(declaringtype, miimplementation, miinterface, aspects)
        {
        }
    }

    internal abstract class ConstructorJoinPoint : MethodBaseJoinPoint
    {
        protected ConstructorInfo ciconstructor;

        public ConstructorJoinPoint(Type declaringtype, ConstructorInfo ciconstructor, AspectCoverageInfo[] aspects) :
            base(declaringtype, aspects)
        {
            this.ciconstructor = ciconstructor;
        }

        public ConstructorInfo ConstructorImplementation
        {
            get
            {
                return ciconstructor;
            }
        }

        public override MethodBase Method
        {
            get { return ciconstructor; }
        }

        public override string ToString()
        {
            return Common.Convert.ToString(ciconstructor);
        }
    }

    internal abstract class PropertyJoinPoint : MethodInfoJoinPoint
    {
        protected PropertyInfo pipropertyinfo;

        public PropertyJoinPoint(Type declaringtype, PropertyInfo pipropertyinfo, MethodInfo miimplementation, MethodInfo miinterface, AspectCoverageInfo[] aspects)
            :
            base(declaringtype, miimplementation, miinterface, aspects)
        {
            this.pipropertyinfo = pipropertyinfo;
        }

        public PropertyInfo PropertyInfo
        {
            get
            {
                return pipropertyinfo;
            }
        }

        public override MemberInfo DeclaringMember
        {
            get
            {
                return pipropertyinfo;
            }
        }

        public override string ToString()
        {
            return Common.Convert.ToString(pipropertyinfo);
        }
    }

    internal abstract class EventJoinPoint : MethodInfoJoinPoint
    {
        protected EventInfo eieventinfo;

        public EventJoinPoint(Type declaringtype, EventInfo eieventinfo, MethodInfo miimplementation, MethodInfo miinterface, AspectCoverageInfo[] aspects)
            :
            base(declaringtype, miimplementation, miinterface, aspects)
        {
            this.eieventinfo = eieventinfo;
        }

        public EventInfo EventInfo
        {
            get
            {
                return eieventinfo;
            }
        }

        public override MemberInfo DeclaringMember
        {
            get
            {
                return eieventinfo;
            }
        }

        public override string ToString()
        {
            return Common.Convert.ToString(eieventinfo);
        }
    }

    internal abstract class DestroyMethodJoinPoint : MethodInfoJoinPoint
    {
        public DestroyMethodJoinPoint(Type declaringtype, MethodInfo miimplementation, AspectCoverageInfo[] aspects) :
            base(declaringtype, miimplementation, null, aspects)
        {
        }

        public override MethodBase Method
        {
            get { return miimplementation; }
        }

        public override string ToString()
        {
            return Common.Convert.ToString(miimplementation);
        }
    }
}
