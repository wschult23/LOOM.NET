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
using Loom.JoinPoints.Implementation;

namespace Loom.CodeBuilder.DynamicProxy.DataSlots
{
    internal abstract class DataSlot
    {
        /// <summary>
        /// Erzeugt den Initialisierungscode des Dataslots
        /// Bei lokalen Dataslots enthält dieser auch die Definition der Variable
        /// </summary>
        /// <param name="meb"></param>
        public virtual void EmitInitialization(ProxyMemberBuilder meb) { }
        /// <summary>
        /// Speichert den lokalen Parameter im IL-Code der Methode
        /// </summary>
        /// <param name="meb"></param>
        public virtual void EmitStoreValue(ProxyMemberBuilder meb)
        {
            throw new AspectWeaverException(9000, Loom.Resources.Errors.ERR_9000, "invalid LocalVariable.EmitStoreValue");
        }
        /// <summary>
        /// Erzeugt IL-Code, der die im Dataslot abgelegten Daten auf den STack Läd und in Targettype konvertiert
        /// </summary>
        /// <param name="meb"></param>
        /// <param name="targettype"></param>
        public abstract void EmitLoadValue(ProxyMemberBuilder meb, Type targettype);

        /// <summary>
        /// Erzeugt IL-Code, der die Addresse des Dataslots auf den Stack Läd und in den Targettype konvertiert
        /// </summary>
        /// <param name="meb"></param>
        public virtual void EmitLoadAddress(ProxyMemberBuilder meb)
        {
            throw new AspectWeaverException(9000, Loom.Resources.Errors.ERR_9000, "invalid LocalVariable.EmitLoadAddress");
        }

        /// <summary>
        /// Liefert den Typ des Dataslots
        /// </summary>
        public abstract Type DataSlotType
        {
            get;
        }

    }

    /// <summary>
    /// Eine Variable von beliebigen Typ auf dem lokalen Stack
    /// </summary>
    internal class LocalObject : DataSlot
    {
        // Die lokale Variable
        LocalBuilder lb;
        Type type;

        public LocalObject(Type type)
        {
            this.type = type;
        }

        public override void EmitInitialization(ProxyMemberBuilder meb)
        {
            ILGenerator ilgen = meb.ILGenerator;
            lb = ilgen.DeclareLocal(type);
        }

        public override void EmitLoadValue(ProxyMemberBuilder meb, Type targettype)
        {
            System.Diagnostics.Debug.Assert(!targettype.IsGenericParameter && !targettype.IsGenericType);

            meb.ILGenerator.Emit(OpCodes.Ldloc, lb);

            if (type.IsValueType && !targettype.IsValueType)
            {
                meb.ILGenerator.Emit(OpCodes.Box, type);
            }
        }

        public override void EmitStoreValue(ProxyMemberBuilder meb)
        {
            meb.ILGenerator.Emit(OpCodes.Stloc, lb);
        }

        public override void EmitLoadAddress(ProxyMemberBuilder meb)
        {
            meb.ILGenerator.Emit(OpCodes.Ldloca_S, lb);
        }

        public override Type DataSlotType
        {
            get
            {
                return type;
            }
        }
    }



    /// <summary>
    /// Dieser Initialisiert einen Argumentcontainer
    /// Es kann für jeden Aspektcode mehrere solcher Container geben, nämlich für jede vorkommende
    /// Arraylänge (des Containers) genau einen. Der Typ des Arrays ist immer object[] und wird später
    /// gecastet.
    /// </summary>
    internal abstract class LocalArgArray : DataSlot
    {
        int count;

        public int Count
        {
            get { return count; }
            set { count = value; }
        }
        int start;

        public int Start
        {
            get { return start; }
            set { start = value; }
        }

        protected LocalBuilder argarray;

        public LocalArgArray()
        {
        }


        /// <summary>
        /// Legt ein Argumentenarray an und initialisiert es mit allen nicht-ref Parametern
        /// Ref-Parameter werden erst kurz vor der übergabe aktualisiert (copy in/ copy out)
        /// es werden die argumente von targetstart..parameters.Length-firstparam in das Array verpackt
        /// </summary>
        /// <param name="ilgen">der Ilgenerator</param>
        /// <param name="parameters">die Parameter, die einzupacken sind</param>
        /// <param name="targetstart">Der erste Parameter auf dem Argumentenstack, der parameters[firstparam] entspricht</param>
        /// <param name="firstparam">Der erste Parameter, der verpackt werden soll</param>
        protected void EmitInitialization(ILGenerator ilgen, ParameterInfo[] parameters, int firstparam, int targetstart)
        {
            //Array anlegen, das Array ist immer object[] und wird erst später gecastet
            argarray = ilgen.DeclareLocal(typeof(object[]));
            CommonCode.EmitLdci4(ilgen, parameters.Length - firstparam);
            ilgen.Emit(OpCodes.Newarr, typeof(Object));
            ilgen.Emit(OpCodes.Stloc, argarray);
            //Werte Speichern
            for (int iPos = 0; iPos < parameters.Length - firstparam; iPos++)
            {
                Type partype = parameters[iPos + firstparam].ParameterType;
                // Refs kommen erst später
                if (partype.IsByRef) continue;

                ilgen.Emit(OpCodes.Ldloc, argarray);
                CommonCode.EmitLdci4(ilgen, iPos);
                CommonCode.EmitLdArg(ilgen, iPos + targetstart);
                if (partype.IsValueType || partype.IsGenericParameter)
                {
                    ilgen.Emit(OpCodes.Box, partype);
                }
                ilgen.Emit(OpCodes.Stelem_Ref);
            }
        }


        /// <summary>
        /// überträgt alle Ref-Parameter in das Array (copy in)
        /// dies kännten in der zwischenzeit von anderen Aspektmethoden auf dem
        /// Call-Stack geändert worden sein
        /// </summary>
        /// <param name="ilgen"></param>
        /// <param name="parameters"></param>
        /// <param name="firstparam"></param>
        /// <param name="targetstart"></param>
        /// <param name="targettype"></param>
        protected void EmitLoadValue(ILGenerator ilgen, ParameterInfo[] parameters, int firstparam, int targetstart, Type targettype)
        {
            for (int iPos = 0; iPos < parameters.Length - firstparam; iPos++)
            {
                Type partype = parameters[iPos].ParameterType;
                if (partype.IsByRef) // Ref-Parameter werden später bei Prepare geladen
                {
                    ilgen.Emit(OpCodes.Ldloc, argarray);
                    CommonCode.EmitLdci4(ilgen, iPos);
                    CommonCode.EmitLdArg(ilgen, iPos + targetstart);
                    Type elementtype = partype.GetElementType();
                    if (elementtype.IsValueType)
                    {
                        CommonCode.EmitLdInd(ilgen, elementtype);
                        ilgen.Emit(OpCodes.Box, elementtype);
                    }
                    else
                    {
                        ilgen.Emit(OpCodes.Ldind_Ref);
                    }
                    ilgen.Emit(OpCodes.Stelem_Ref);
                }
            }

            // Container auf den Stack
            ilgen.Emit(OpCodes.Ldloc, argarray);
            // ggf. in den richtigen Arraytyp casten
            if (targettype != typeof(object[]))
            {
                ilgen.Emit(OpCodes.Castclass, targettype);
            }
        }

        /// <summary>
        /// Erzeugt Code, der dafär sorgt, dass out Parameter aus einem Argumentenarray wieder zurück
        /// </summary>
        /// <param name="ilgen"></param>
        /// <param name="parameters"></param>
        /// <param name="firstparam"></param>
        /// <param name="targetstart"></param>
        protected void EmitStoreValue(ILGenerator ilgen, ParameterInfo[] parameters, int firstparam, int targetstart)
        {
            for (int iPos = 0; iPos < parameters.Length - firstparam; iPos++)
            {
                Type partype = parameters[iPos + firstparam].ParameterType;
                if (partype.IsByRef)
                {
                    Type t = partype.GetElementType();
                    CommonCode.EmitLdArg(ilgen, iPos + targetstart);
                    ilgen.Emit(OpCodes.Ldloc, argarray);
                    CommonCode.EmitLdci4(ilgen, iPos);
                    ilgen.Emit(OpCodes.Ldelem_Ref);
                    if (t.IsValueType)
                    {
                        ilgen.Emit(OpCodes.Unbox, t);
                        CommonCode.EmitLdInd(ilgen, t);
                    }
                    CommonCode.EmitStInd(ilgen, t);
                }
            }
        }

        public override Type DataSlotType
        {
            get
            {
                return typeof(object[]);
            }
        }
    }

    /// <summary>
    /// Argarray für normale Proxymethoden
    /// </summary>
    internal class ProxyMethodLocalArgArray : LocalArgArray
    {
        public override void EmitInitialization(ProxyMemberBuilder meb)
        {
            MethodBase mibc = meb.JoinPoint.Method;
            int targetstart = mibc.IsStatic ? Start : Start + 1;
            EmitInitialization(meb.ILGenerator, mibc.GetParameters(), Start, targetstart);
        }

        public override void EmitLoadValue(ProxyMemberBuilder meb, Type targettype)
        {
            MethodBase mibc = meb.JoinPoint.Method;
            int targetstart = mibc.IsStatic ? Start : Start + 1;
            EmitLoadValue(meb.ILGenerator, mibc.GetParameters(), Start, targetstart, targettype);
        }

        public override void EmitStoreValue(ProxyMemberBuilder meb)
        {
            MethodBase mibc = meb.JoinPoint.Method;
            int targetstart = mibc.IsStatic ? Start : Start + 1;
            EmitStoreValue(meb.ILGenerator, mibc.GetParameters(), Start, targetstart);
        }
    }

    /// <summary>
    /// überladung um lokales Array auf CreateAI-Methode anzupassen
    /// Die Argumentliste enthält am anfang die ASpekte
    /// </summary>
    internal class CreateMethodLocalArgArray : LocalArgArray
    {
        public override void EmitInitialization(ProxyMemberBuilder meb)
        {
            EmitInitialization(meb.ILGenerator, meb.JoinPoint.Method.GetParameters(), Start, 2);
        }

        public override void EmitLoadValue(ProxyMemberBuilder meb, Type targettype)
        {
            EmitLoadValue(meb.ILGenerator, meb.JoinPoint.Method.GetParameters(), Start, 2, targettype);
        }

        public override void EmitStoreValue(ProxyMemberBuilder meb)
        {
            EmitStoreValue(meb.ILGenerator, meb.JoinPoint.Method.GetParameters(), Start, 2);
        }
    }

    internal class NullLocalArgArray : LocalArgArray
    {
        public override void EmitInitialization(ProxyMemberBuilder meb)
        {
            ILGenerator ilgen = meb.ILGenerator;
            MethodBase mibc = meb.JoinPoint.Method;
            ParameterInfo[] parameters = mibc.GetParameters();
            int targetstart = mibc.IsStatic ? Start : Start + 1;

            //Array anlegen, das Array ist immer object[] und wird erst später gecastet
            argarray = ilgen.DeclareLocal(typeof(object[]));
            CommonCode.EmitLdci4(ilgen, parameters.Length - Start);
            ilgen.Emit(OpCodes.Newarr, typeof(Object));
            ilgen.Emit(OpCodes.Stloc, argarray);
            //Werte Speichern
            for (int iPos = 0; iPos < parameters.Length - Start; iPos++)
            {
                Type partype = parameters[iPos + Start].ParameterType;

                if (partype.IsValueType)
                {
                    ilgen.Emit(OpCodes.Ldloc, argarray);
                    CommonCode.EmitLdci4(ilgen, iPos);

                    CommonCode.EmitInitValueType(ilgen, partype);

                    ilgen.Emit(OpCodes.Box, partype);

                    ilgen.Emit(OpCodes.Stelem_Ref);
                }
            }
        }

        public override void EmitLoadValue(ProxyMemberBuilder meb, Type targettype)
        {
            ILGenerator ilgen = meb.ILGenerator;
            // Container auf den Stack
            ilgen.Emit(OpCodes.Ldloc, argarray);
            // ggf. in den richtigen Arraytyp casten
            if (targettype != typeof(object[]))
            {
                ilgen.Emit(OpCodes.Castclass, targettype);
            }
        }

        public override void EmitStoreValue(ProxyMemberBuilder meb)
        {
        }
    }
}
