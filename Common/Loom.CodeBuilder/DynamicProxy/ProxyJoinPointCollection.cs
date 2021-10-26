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
using Loom.JoinPoints.Implementation;
using System.Reflection;

namespace Loom.CodeBuilder.DynamicProxy
{
    class ProxyJoinPointCollection:JoinPointCollection
    {
        /// <summary>
        /// Die Implementation von __GetAspects(ArrayList,Type) (wenn verwoben)
        /// </summary>
        private MethodInfo miGetAspectsMethodImpl;
        /// <summary>
        /// Die Methode, die GetAspects implementiert
        /// </summary>
        public MethodInfo ImplOfGetAspectsMethod
        {
            get { return miGetAspectsMethodImpl; }
        }

        /// <summary>
        /// Der Index des neu hinzugefügten Aspextes
        /// </summary>
        private int aspectindex;
        /// <summary>
        /// Der Index des neu hinzugefügten Aspextes
        /// </summary>
        public int AspectIndex
        {
            get
            {
                return aspectindex;
            }
        }

        internal ProxyJoinPointCollection(JoinPointCollection basecollection, Type type, ConstructorJoinPoint[] ctorjoinpoints, MethodInfoJoinPoint[] vtbljoinpoints, InterfaceJoinPoints[] interfacejoinpoints, MethodInfo miGetAspectsMethodImpl, int aspectindex)
            :
            base(basecollection,type,ctorjoinpoints,vtbljoinpoints,interfacejoinpoints)
        {
            this.miGetAspectsMethodImpl = miGetAspectsMethodImpl;
            this.aspectindex = aspectindex;
        }
    }
}
