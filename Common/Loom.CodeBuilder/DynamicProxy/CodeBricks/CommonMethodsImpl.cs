// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;

using Loom.Common;
using Loom.JoinPoints.Implementation;

namespace Loom.CodeBuilder.DynamicProxy.CodeBricks
{
    internal class CommonMethodsImpl
    {
        public class SortedJoinPoints : SortedList<ParameterContainer, MethodInfo> { };

        /// <summary>
        /// Sucht alle Candidaten für einen Dynamic Dispatch
        /// </summary>
        /// <param name="targetclass"></param>
        /// <param name="methodname"></param>
        /// <returns></returns>
        public static CommonMethodsImpl.SortedJoinPoints FindCandidates(Type targetclass, string methodname)
        {
            CommonMethodsImpl.SortedJoinPoints micandidates = new CommonMethodsImpl.SortedJoinPoints();
            JoinPointCollection jpcol = JoinPointCollection.GetJoinPoints(targetclass);

            foreach (MethodInfoJoinPoint jp in jpcol.MethodJoinPoints)
            {
                if (jp.Method.Name == methodname) micandidates.Add(Conversion.ToTypeArray(jp.Method.GetParameters()), ((IBaseCall)jp).BaseCall);
            }
            return micandidates;
        }


        /// <summary>
        /// Implementiert ein Property, welches einen angegebenen Typ zurückliefert
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="mibase">die Basisimplementierung des Properties ClassObject</param>
        /// <param name="type">Der Typ der geliefert werden soll</param>
        public static void Impl_get_TypeProperty(TypeBuilder tb, MethodInfo mibase, Type type)
        {
            MethodAttributes attr = mibase.Attributes;
            if ((attr & MethodAttributes.NewSlot) != 0) attr -= MethodAttributes.NewSlot;
            if ((attr & MethodAttributes.Abstract) != 0) attr -= MethodAttributes.Abstract;

            MethodBuilder mb = tb.DefineMethod(
                mibase.Name,
                attr,
                mibase.ReturnType,
                ReflectionObjects.c_param_);

            ILGenerator ilgen = mb.GetILGenerator();
            ilgen.Emit(OpCodes.Ldtoken, type);
            ilgen.Emit(OpCodes.Tailcall);
            ilgen.Emit(OpCodes.Call, ReflectionObjects.c_mi_Type_GetTypeFromHandle);
            ilgen.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// Implementiert ein Dispatch anhand der übergebenen Parameter indem gegen die Methoden im Dictionary gematcht wird
        /// </summary>
        /// <param name="ilgen">der ILGenerator</param>
        /// <param name="methods">die Methoden, die dispatched werden</param>
        /// <param name="isstatic">ist die implementierung statisch</param>
        public static void ImplDynamicDispatch(ILGenerator ilgen, SortedJoinPoints methods, Action<ILGenerator> ldargarray, Action<ILGenerator> ldinst)
        {
            int prevparamcount = -1;
            // Typisierte Null
            LocalBuilder loctypednull = null;
            // Defaultlabel da default(Label) leider den Wert des ersten DefineLabel hat
            Label lbdefault = ilgen.DefineLabel();
            // Sprungmarke zur nächsten Prüfung der Parameterlänge
            Label lbnextparamcount = lbdefault;
            // Sprungmarke zur nächsten Prüfung der Parametertypen
            Label lbnextparamlist = lbdefault;
            List<Label> arrfailedtypechecks = new List<Label>();
            foreach (KeyValuePair<ParameterContainer, MethodInfo> kvpair in methods)
            {
                // für jeden neuen Parameteranzahl prüfen ob diese übereinstimmt
                Type[] parameters = kvpair.Key;
                if (prevparamcount != parameters.Length)
                {
                    if (prevparamcount != -1) ilgen.MarkLabel(lbnextparamcount);
                    lbnextparamcount = ilgen.DefineLabel();

                    prevparamcount = parameters.Length;

                    ldargarray(ilgen);
                    ilgen.Emit(OpCodes.Ldlen);
                    ilgen.Emit(OpCodes.Conv_I4);
                    CommonCode.EmitLdci4(ilgen, prevparamcount);
                    ilgen.Emit(OpCodes.Ceq);
                    ilgen.Emit(OpCodes.Brfalse, lbnextparamcount);
                    // Die TypPrüfung war erfolglos, das Label merken und später auf die Endemarke setzen
                    if (lbnextparamlist != lbdefault)
                    {
                        arrfailedtypechecks.Add(lbnextparamlist);
                        lbnextparamlist = lbdefault;
                    }
                }
                // Typen ok?
                if (lbnextparamlist != lbdefault) ilgen.MarkLabel(lbnextparamlist);
                if (parameters.Length > 0) lbnextparamlist = ilgen.DefineLabel();
                // Sprungmarke zum nächsten Parametertypcheck
                Label lbnextparam = lbdefault;
                for (int iPos = 0; iPos < parameters.Length; iPos++)
                {
                    if (lbnextparam != lbdefault) ilgen.MarkLabel(lbnextparam);

                    ldargarray(ilgen);
                    CommonCode.EmitLdci4(ilgen, iPos);
                    ilgen.Emit(OpCodes.Ldelem_Ref);
                    // wenn null dann sagt isinst false, das passt aber trotzdem in einen Klassentyp
                    lbnextparam = ilgen.DefineLabel();

                    Type tp = parameters[iPos];
                    if (!tp.IsValueType && !tp.IsByRef)
                    {
                        // Wertetypen können mit einer Typed-Null belegt sein.
                        if (loctypednull == null)
                        {
                            loctypednull = ilgen.DeclareLocal(typeof(TypedValue));
                        }
                        // Sprungmarke wenn Parameter nicht null ist
                        Label lbpop;

                        // Stimmen die Typen?
                        lbpop = ilgen.DefineLabel();
                        ilgen.Emit(OpCodes.Dup);
                        ilgen.Emit(OpCodes.Isinst, tp);
                        ilgen.Emit(OpCodes.Ldnull);
                        ilgen.Emit(OpCodes.Ceq);
                        ilgen.Emit(OpCodes.Brtrue_S, lbpop);
                        ilgen.Emit(OpCodes.Pop);
                        ilgen.Emit(OpCodes.Br, lbnextparam);
                        ilgen.MarkLabel(lbpop);
                        // Ist es eine typisierte Null?
                        lbpop = ilgen.DefineLabel();
                        ilgen.Emit(OpCodes.Dup);
                        ilgen.Emit(OpCodes.Isinst, typeof(Loom.TypedValue));
                        ilgen.Emit(OpCodes.Stloc, loctypednull);
                        ilgen.Emit(OpCodes.Ldloc, loctypednull);
                        ilgen.Emit(OpCodes.Ldnull);
                        ilgen.Emit(OpCodes.Ceq);
                        ilgen.Emit(OpCodes.Brtrue_S, lbpop);
                        ilgen.Emit(OpCodes.Ldloc, loctypednull);
                        ilgen.Emit(OpCodes.Ldtoken, tp);
                        ilgen.Emit(OpCodes.Call, ReflectionObjects.c_mi_Type_GetTypeFromHandle);
                        ilgen.Emit(OpCodes.Callvirt, ReflectionObjects.c_mi_TypedNull_IsType);
                        ilgen.Emit(OpCodes.Brfalse_S, lbpop);
                        ilgen.Emit(OpCodes.Pop);
                        ilgen.Emit(OpCodes.Br_S, lbnextparam);
                        ilgen.MarkLabel(lbpop);

                        // ist es null ?
                        ilgen.Emit(OpCodes.Ldnull);
                        ilgen.Emit(OpCodes.Ceq);
                        ilgen.Emit(OpCodes.Brfalse, lbnextparamlist);

                    }
                    else
                    {
                        if (tp.IsByRef) tp = tp.GetElementType();
                        // Stimmen die Typen?
                        ilgen.Emit(OpCodes.Isinst, tp);
                        ilgen.Emit(OpCodes.Ldnull);
                        ilgen.Emit(OpCodes.Ceq);
                        ilgen.Emit(OpCodes.Brtrue, lbnextparamlist);
                    }
                }
                if (lbnextparam != lbdefault) ilgen.MarkLabel(lbnextparam);

                // Schauen, ob eine typednull dabei war, wenn ja muss das array gepatcht werden muss
                // loctypenull ist in jedem fall != null, wenn es einen Match gab und eine typednull dabei war
                // sonst ist es in jedem fall null
                if (loctypednull != null)
                {
                    Label lbTyped = ilgen.DefineLabel();
                    ilgen.Emit(OpCodes.Ldloc, loctypednull);
                    ilgen.Emit(OpCodes.Ldnull);
                    ilgen.Emit(OpCodes.Ceq);
                    ilgen.Emit(OpCodes.Brtrue_S, lbTyped);
                    ldargarray(ilgen);
                    ilgen.Emit(OpCodes.Call, ReflectionObjects.c_mi_Common_ReplaceTypedNull);
                    ilgen.MarkLabel(lbTyped);
                }

                // Parameter auf den Stack laden
                ldinst(ilgen);
                
                LocalBuilder[] refparams = CommonCode.EmitLdArrayToStack(ilgen, ldargarray, parameters, 0, null, false);
                // Konstruktor rufen
                bool bTailCall = (refparams.Length == 0) && (kvpair.Value.IsConstructor || (!kvpair.Value.ReturnType.IsValueType && !kvpair.Value.ReturnType.IsGenericType && !(kvpair.Value.ReturnType == typeof(void))));
                if (bTailCall)
                {
                    ilgen.Emit(OpCodes.Tailcall);
                    ilgen.Emit(OpCodes.Callvirt, kvpair.Value);
                }
                else
                {
                    ilgen.Emit(OpCodes.Callvirt, kvpair.Value);
                    CommonCode.EmitAdjustReturnValue(ilgen, kvpair.Value.ReturnType, typeof(object));
                    CommonCode.EmitLdRefLocalsToArray(ilgen, ldargarray, parameters, 0, null, refparams);
                }
                ilgen.Emit(OpCodes.Ret);
            }

            // Label der letzten Typechecks setzen
            if (lbnextparamlist != lbdefault) ilgen.MarkLabel(lbnextparamlist);
            foreach (Label lb in arrfailedtypechecks)
            {
                ilgen.MarkLabel(lb);
            }

            if (lbnextparamcount != lbdefault) ilgen.MarkLabel(lbnextparamcount);
            ldargarray(ilgen);
            ilgen.Emit(OpCodes.Newobj, ReflectionObjects.c_ci_MissingTargetMethodException_ObjectArray);
            ilgen.Emit(OpCodes.Throw);
        }

    }
}
