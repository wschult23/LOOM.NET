// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Reflection;
using System.Linq;

using Loom;
using Loom.JoinPoints;
using Loom.Runtime;
using Loom.Common;

namespace Loom.Runtime.Serialization
{
    /// <summary>
    /// This class is used for internal purposes
    /// </summary>
    [Serializable]
    public sealed class SerializationAspect : AspectAttribute, IObjectReference, ISerializable
    {
        /// <summary>
        /// the jpinterwoven object
        /// </summary>
        private object theObject;

        /// <summary>
        /// Liefert alle dynamischen Aspecte welche serialisiert werden müssen
        /// </summary>
        /// <returns></returns>
        private Aspect[] GetDynamicAspects(Type targettype, object instance)
        {
            // statischen Aspekte aus der Liste löschen
            Aspect[] statics = Weaver.GetClassObjectSafe(targettype).GetAspects(true);
            Aspect[] asparr = Weaver.GetAspects(instance);
            for (int iPos = 0; iPos < statics.Length; iPos++)
            {
                if (statics[iPos] != null)
                {
                    asparr[iPos] = null;
                }
            }
            return asparr;
        }

        #region IObjectReference Members

        /// <summary>
        /// returns the real object
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Object GetRealObject(StreamingContext context)
        {
            return theObject;
        }
        #endregion

        /// <summary>
        /// Internal use. 
        /// </summary>
        public static SerializationAspect c_BaseObject = new SerializationAspect();

        private SerializationAspect()
        {
        }

        /// <summary>
        /// Deserialisation ctor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public SerializationAspect(SerializationInfo info, StreamingContext context)
        {
            // get original Type
            Type type = (Type)info.GetValue(WeavingCodeNames.c_serialize_basetype, typeof(Type));
            var clobj = Weaver.GetClassObjectSafe(type);
            // get the list of aspects
            Aspect[] aspects = (Aspect[])info.GetValue(WeavingCodeNames.c_serialize_aspects, typeof(Aspect[]));
            Aspect[] statics = clobj.GetAspects(true);
            int iPos;
            for (iPos = 0; iPos < statics.Length; iPos++)
            {
                if (aspects[iPos] == null)
                {
                    System.Diagnostics.Debug.Assert(statics[iPos] != null);
                    aspects[iPos] = statics[iPos];
                }
                else
                {
                    System.Diagnostics.Debug.Assert(statics[iPos] == null);
                }
            }
            // additional dynamic aspects?
            if(iPos<aspects.Length)
            {
                Aspect[] dynamicaspects = new Aspect[aspects.Length - iPos];
                for (; iPos < aspects.Length; iPos++)
                {
                    dynamicaspects[iPos - statics.Length] = aspects[iPos];
                }
                clobj = Weaver.GetClassObject(type, dynamicaspects);
            }

            // create instance and deserialize all member
            bool bAttributed = info.GetBoolean(WeavingCodeNames.c_serialize_attributed);

            if (bAttributed)
            {
                theObject = clobj.CreateInstanceInternal(aspects, Loom.Common.SimpleObjects.c_emptyobjectarray);
                foreach (FieldInfo fi in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    if (!fi.IsNotSerialized)
                    {
                        fi.SetValue(theObject, info.GetValue(fi.Name, fi.FieldType));
                    }
                }
            }
            else
            {
                theObject = clobj.CreateInstanceInternal(aspects, new object[] { info, context });
            }
        }

        /// <summary>
        /// Internal use.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="instance"></param>
        /// <param name="targetclass"></param>
        /// <param name="context"></param>
        [Introduce(typeof(ISerializable))]
        public void GetObjectData([JPTargetClass] Type targetclass, [JPTarget] object instance, SerializationInfo info, StreamingContext context)
        {
            // Around of serializing this object, 
            // serialize the aspect instead.
            info.SetType(GetType());
            info.AddValue(WeavingCodeNames.c_serialize_basetype, targetclass, typeof(Type));
            info.AddValue(WeavingCodeNames.c_serialize_aspects, GetDynamicAspects(targetclass, instance), typeof(Aspect[]));
            info.AddValue(WeavingCodeNames.c_serialize_attributed, true);
            // serialize base targettype
            foreach (FieldInfo fi in targetclass.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (!fi.IsNotSerialized)
                {
                    info.AddValue(fi.Name, fi.GetValue(instance), fi.FieldType);
                }
            }
        }

        /// <summary>
        /// Internal use.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="targetclass"></param>
        /// <param name="info"></param>
        /// <param name="context"></param>
        [Call(Advice.Before), Include(typeof(ISerializable))]
        public void GetObjectData([JPTarget] object instance, [JPTargetClass] Type targetclass, SerializationInfo info, StreamingContext context)
        {
            // Around of serializing this object, 
            // serialize the aspect instead.
            info.SetType(GetType());
            info.AddValue(WeavingCodeNames.c_serialize_basetype, targetclass, typeof(Type));
            info.AddValue(WeavingCodeNames.c_serialize_aspects, GetDynamicAspects(targetclass, instance), typeof(Aspect[]));
            info.AddValue(WeavingCodeNames.c_serialize_attributed, false);
        }


        #region ISerializable Members

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
        }

        #endregion
    }


}