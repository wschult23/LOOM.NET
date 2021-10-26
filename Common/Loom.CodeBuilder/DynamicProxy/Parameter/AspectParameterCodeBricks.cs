// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

using Loom.Common;
using Loom.CodeBuilder.DynamicProxy;
using Loom.CodeBuilder.DynamicProxy.CodeBricks;
using Loom.CodeBuilder.DynamicProxy.DataSlots;


namespace Loom.CodeBuilder.DynamicProxy.Parameter
{
    /// <summary>
    /// Aspectparameter für den Typ der Targetclass
    /// </summary>
    internal class TargetClassTypeAspectParameterCodeBrick : SingletonParameterCodeBrick<TargetClassTypeAspectParameterCodeBrick>
    {
        public override void EmitPrepare(ProxyMemberBuilder meb)
        {
            ILGenerator ilgen = meb.ILGenerator;
            ilgen.Emit(OpCodes.Ldtoken, meb.DeclaringBuilder.TargetClass);
            ilgen.Emit(OpCodes.Call, ReflectionObjects.c_mi_Type_GetTypeFromHandle);
        }
    }

    /// <summary>
    /// Aspectparameter für den Verwebungstyp
    /// </summary>
    internal class DynamicTypeAspectParameterCodeBrick : SingletonParameterCodeBrick<DynamicTypeAspectParameterCodeBrick>
    {
        public override void EmitPrepare(ProxyMemberBuilder meb)
        {
            ILGenerator ilgen = meb.ILGenerator;
            ilgen.Emit(OpCodes.Ldarg_0);
            ilgen.Emit(OpCodes.Callvirt, ReflectionObjects.c_mi_Object_GetType);
        }
    }

    /// <summary>
    /// Aspectparameter für den Verwebungstyp
    /// </summary>
    internal class ActivatorTypeAspectParameterCodeBrick : SingletonParameterCodeBrick<ActivatorTypeAspectParameterCodeBrick>
    {
        public override void EmitPrepare(ProxyMemberBuilder meb)
        {
            ILGenerator ilgen = meb.ILGenerator;
            ilgen.Emit(OpCodes.Ldarg_0);
            ilgen.Emit(OpCodes.Callvirt, ReflectionObjects.c_mi_ClassObject_getType);
        }
    }

    /// <summary>
    /// Aspectparameter für die Targetclass
    /// </summary>
    internal class TargetAspectParameterCodeBrick : SingletonParameterCodeBrick<TargetAspectParameterCodeBrick>
    {
        public override void EmitPrepare(ProxyMemberBuilder meb)
        {
            IILGenerator ilgen = (IILGenerator)meb;
            ilgen.ILGenerator.Emit(OpCodes.Ldarg_0);
        }
    }

    /// <summary>
    /// Parameter, die NULL sind
    /// </summary>
    internal class NullParameterCodeBrick : SingletonParameterCodeBrick<NullParameterCodeBrick>
    {
        public override void EmitPrepare(ProxyMemberBuilder meb)
        {
            IILGenerator ilgen = (IILGenerator)meb;
            ilgen.ILGenerator.Emit(OpCodes.Ldnull);
        }
    }

    /// <summary>
    /// Value Parameter, die 0 sind oder Generische Typen
    /// </summary>
    internal class DefaultValueCodeBrick : ParameterCodeBrick, IGenericParameterCodeBrick
    {
        /// <summary>
        /// Der Typ in den umgewandelt werden muss
        /// </summary>
        private Type targettype;
        /// <summary>
        /// Der Typ, des Parameters vorliegt
        /// </summary>
        private Type sourcetype;

        /// <summary>
        /// Konstruktor für Value Generics
        /// </summary>
        /// <param name="sourcetype">Der Typ, der auf dem Argumentenstack liegt oder liegen würde, also der Typ des Parameters der Zielmethode</param>
        /// <param name="targettype">Der Typ, der auf den Stack geladen werden soll</param>
        public DefaultValueCodeBrick(Type sourcetype, Type targettype)
        {
            this.targettype = targettype;
            this.sourcetype = sourcetype;
        }


        public override void EmitPrepare(ProxyMemberBuilder meb)
        {
            ILGenerator ilgen = ((IILGenerator)meb).ILGenerator;
            // Wenn der Parameter der Aspektmethode (targettype) generisch ist, ist er an sourcetype gebunden und
            // es muss ein Objekt diesen Typs auf den Stack geladen werden
            // andernfalls muss ein defaultobjekt des Parametertyps der Aspektmethode erzeugt werden
            Type type = targettype.IsGenericParameter ? sourcetype : targettype;

            if (type.IsGenericParameter)
            {
                ilgen.Emit(OpCodes.Ldnull);
            }
            else
            {
                CommonCode.EmitInitValueType(ilgen, type);
            }
        }



        /// <summary>
        /// Bindet die generischen Parameter des Ziels an die Typen des Aufrufers
        /// </summary>
        /// <param name="generics"></param>
        public void AddGenericParameter(GenericDictionary generics)
        {
            if (targettype.IsGenericParameter)
            {
                if (!generics.ContainsKey(targettype))
                {
                    generics.Add(targettype, sourcetype);
                }
            }
        }

    }

    /// <summary>
    /// Ref-Parameter, die NULL sind
    /// </summary>
    internal class NullRefParamCodeBrick : ParameterCodeBrick, IGenericParameterCodeBrick
    {
        /// <summary>
        /// Der Typ in den umgewandelt werden muss
        /// </summary>
        private Type targettype;
        /// <summary>
        /// Der Typ, des Parameters vorliegt
        /// </summary>
        private Type sourcetype;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourcetype">Der Typ, der auf dem Argumentenstack liegt oder liegen würde, also der Typ des Parameters der Zielmethode</param>
        /// <param name="targettype">Der Typ, der auf den Stack geladen werden soll</param>
        public NullRefParamCodeBrick(Type sourcetype, Type targettype)
        {
            this.targettype = targettype;
            this.sourcetype = sourcetype;

        }

        /// <summary>
        /// Es muss lediglich eine lokale Variable angelegt werden, da der IL-Code Initlocals gesetzt hat
        /// </summary>
        /// <param name="meb"></param>
        public override void EmitPrepare(ProxyMemberBuilder meb)
        {
            ILGenerator ilgen = ((IILGenerator)meb).ILGenerator;
            LocalBuilder loc = ilgen.DeclareLocal(targettype);
            ilgen.Emit(OpCodes.Ldloca, loc);
        }

        #region IGenericParameterCodeBrick Members

        public void AddGenericParameter(GenericDictionary generics)
        {
            if (targettype.IsGenericParameter)
            {
                if (!generics.ContainsKey(targettype))
                {
                    generics.Add(targettype, sourcetype);
                }
            }
        }
        #endregion
    }


    /// <summary>
    /// übergabe einer lokalen Variable an eine Aspektmethode
    /// </summary>
    internal class DataSlotParameterCodeBrick : ParameterCodeBrick, IGenericParameterCodeBrick
    {
        protected DataSlot lv;
        protected ParameterInfo target;

        /// <summary>
        /// Läd eine lokale Variable auf den Parameter-Stack und konvertiert ihn ggf.
        /// </summary>
        /// <param name="lv">Datenstruktur welche die lokale Variable enthält</param>
        /// <param name="target">Der Zielparameter</param>
        public DataSlotParameterCodeBrick(DataSlot lv, ParameterInfo target)
        {
            System.Diagnostics.Debug.Assert(lv != null);
            System.Diagnostics.Debug.Assert(!target.ParameterType.IsByRef); // Hier muss der DataSlotRefParameterCodebrick genommen werden!
            this.lv = lv;
            this.target = target;
        }

        public override void EmitPrepare(ProxyMemberBuilder meb)
        {
            // Wenn der Parameter der Aspektmethode generisch ist, dann ermitteln wir den tatsächlichen Typen aus dem Dataslot (s.u.)
            Type targettype = target.ParameterType;
            if (targettype.IsGenericParameter) targettype = lv.DataSlotType;
            lv.EmitLoadValue(meb, targettype);
        }

        #region IGenericParameterCodeBrick Members

        public void AddGenericParameter(GenericDictionary generics)
        {
            if (!target.ParameterType.IsGenericParameter) return;

            if (!generics.ContainsKey(target.ParameterType))
            {
                generics.Add(target.ParameterType, lv.DataSlotType);
            }

        }

        #endregion

    }

    /// <summary>
    /// übergabe einer lokalen Variable an eine Aspektmerthode
    /// </summary>
    internal class DataSlotRefParameterCodeBrick : ParameterCodeBrick
    {
        DataSlot lv;
        ParameterInfo target;

        /// <summary>
        /// Läd eine lokale Variable auf den Parameter-Stack und konvertiert ihn ggf.
        /// </summary>
        /// <param name="lv">Datenstruktur welche die lokale Variable enthält</param>
        /// <param name="target">Der Zielparameter</param>
        public DataSlotRefParameterCodeBrick(DataSlot lv, ParameterInfo target)
        {
            System.Diagnostics.Debug.Assert(lv != null);
            this.lv = lv;
            this.target = target;
        }

        public override void EmitPrepare(ProxyMemberBuilder meb)
        {
            lv.EmitLoadAddress((ProxyMemberBuilder)meb);
        }
    }

}
