// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using Loom.JoinPoints.Implementation;

namespace Loom.Runtime
{
    /// <summary>
    /// This class supports the LOOM.NET infrastructure and is not intended to be used directly from your code. 
    /// </summary>
    public abstract class ClassObject
    {
        /// <summary>
        /// 2nd level cache for all dynamic interwoven types
        /// </summary>
        private Dictionary<Type, ClassObject> dynamicWovenClassObj;

        /// <summary>
        /// This method supports the LOOM.NET infrastructure and is not intended to be used directly from your code. 
        /// </summary>
        protected ClassObject()
        {
        }

        /// <summary>
        /// Fügt ein Classobjekt für einen dynamischen Aspekt hinzu
        /// </summary>
        /// <param name="aspecttype"></param>
        /// <param name="obj"></param>
        internal void AddClassObject(Type aspecttype, ClassObject obj)
        {
            if (dynamicWovenClassObj == null)
            {
                dynamicWovenClassObj = new Dictionary<Type, ClassObject>();
            }
            dynamicWovenClassObj.Add(aspecttype, obj);
        }

        /// <summary>
        /// Ruft ein Classobjekt für einen dynamischen Aspekt ab
        /// </summary>
        /// <param name="aspecttype"></param>
        /// <param name="obj"></param>
        internal ClassObject GetClassObject(Type aspecttype)
        {
            if (dynamicWovenClassObj == null)
            {
                return null;
            }
            ClassObject obj;
            if (dynamicWovenClassObj.TryGetValue(aspecttype, out obj))
            {
                return obj;
            }
            return null;
        }

        /// <summary>
        /// This method supports the LOOM.NET infrastructure and is not intended to be used directly from your code. 
        /// </summary>
        public virtual Aspect[] GetAspects(bool exludeNonStatics) { return null; }

        /// <summary>
        /// This method is for internal purposes only! Do never call this method directly from your code!
        /// </summary>
        public object CreateInstance(object[] args, Aspect[] dynamicaspects)
        {
            // Dynamische und Statische Aspekte zusammenkopieren
            Aspect[] aspects = GetAspects(false);
            Aspect[] allaspects;
            if (dynamicaspects != null)
            {
                if (aspects == null)
                {
                    allaspects = dynamicaspects;
                }
                else
                {
                    allaspects = new Aspect[dynamicaspects.Length + aspects.Length];
                    aspects.CopyTo(allaspects, 0);
                    dynamicaspects.CopyTo(allaspects, aspects.Length);
                }
            }
            else
            {
                allaspects = aspects;
            }
            return CreateInstanceInternal(allaspects, args == null ? Loom.Common.SimpleObjects.c_emptyobjectarray : args);
        }

        /// <summary>
        /// This property is for internal purposes only! Do never access this property directly from your code!
        /// </summary>
        public virtual Type Type { get { throw new NotImplementedException(); } }

        /// <summary>
        /// This property is for internal purposes only! Do never access this property directly from your code!
        /// </summary>
        public virtual Type TargetClass { get { throw new NotImplementedException(); } }

        /// <summary>
        /// Erzeugt Eine neue Instanz
        /// </summary>
        /// <param name="aspectargs"></param>
        /// <param name="ctorargs"></param>
        /// <returns></returns>
        public virtual object CreateInstanceInternal(Aspect[] aspectargs, object[] ctorargs) { return null; }
    }


    /// <summary>
    /// Wird verwendet, wenn beim verweben eine Exception geworfen wurde
    /// </summary>
    internal class ClassObjectException : ClassObject
    {
        Type type;
        Exception e;

        public ClassObjectException(Type type, Exception e)
        {
            this.type = type;
            this.e = e;
        }

        public override Type Type
        {
            get { return type; }
        }

        public override Aspect[] GetAspects(bool excludeNonStatics)
        {
            return null;
        }

        public override object CreateInstanceInternal(Aspect[] aspectargs, object[] ctorargs)
        {
            throw e;
        }
    }

}
