// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections;

using Loom.CodeBuilder.DynamicProxy.Parameter;
using Loom.Common;
using Loom.Resources;
using Loom.Runtime;
using System.Collections.Generic;


namespace Loom.CodeBuilder.DynamicProxy
{
    /// <summary>
    /// Klasse um sich wiederholende Emit-Fälle abzubilden
    /// </summary>
    internal class CommonCode
    {
        /// <summary>
        /// Generates code firstparam load all parameters onto the stack
        /// </summary>
        /// <param name="ilgen"></param>
        /// <param name="start">Der erste Parameter 0=this bei Instanzmethoden</param>
        /// <param name="count">Anzahl der Parameter</param>
        public static void EmitLdArgList(ILGenerator ilgen, int start, int count)
        {
            for (int i = 0; i < count; i++)
            {
                CommonCode.EmitLdArg(ilgen, i + start);
            }
        }



        /// <summary>
        /// Läd den Wert einer Referenz egal ob Wertetyp oder Referenztyp
        /// Vorher: Oberster Stackwert = Referenz 
        /// Nachher: Oberster Stackwert = Wert
        /// </summary>
        /// <param name="ilgen"></param>
        /// <param name="t"></param>
        internal static void EmitLdUnbox(ILGenerator ilgen, Type t)
        {
            if (t.IsPrimitive)
            {
                EmitLdInd(ilgen, t.UnderlyingSystemType);
            }
            else
            {
                ilgen.Emit(OpCodes.Ldobj, t);
            }
        }

        /// <summary>
        /// Läd ein Argument auf den Stack
        /// </summary>
        /// <param name="ilgen"></param>
        /// <param name="iArg"></param>
        internal static void EmitLdArg(ILGenerator ilgen, int iArg)
        {
            switch (iArg)
            {
                case 0:
                    ilgen.Emit(OpCodes.Ldarg_0);
                    break;
                case 1:
                    ilgen.Emit(OpCodes.Ldarg_1);
                    break;
                case 2:
                    ilgen.Emit(OpCodes.Ldarg_2);
                    break;
                case 3:
                    ilgen.Emit(OpCodes.Ldarg_3);
                    break;
                default:
                    ilgen.Emit(OpCodes.Ldarg, iArg);
                    break;
            }
        }

        internal static void EmitLdci4(ILGenerator ilgen, int val)
        {
            switch (val)
            {
                case 0:
                    ilgen.Emit(OpCodes.Ldc_I4_0);
                    break;
                case 1:
                    ilgen.Emit(OpCodes.Ldc_I4_1);
                    break;
                case 2:
                    ilgen.Emit(OpCodes.Ldc_I4_2);
                    break;
                case 3:
                    ilgen.Emit(OpCodes.Ldc_I4_3);
                    break;
                case 4:
                    ilgen.Emit(OpCodes.Ldc_I4_4);
                    break;
                case 5:
                    ilgen.Emit(OpCodes.Ldc_I4_5);
                    break;
                case 6:
                    ilgen.Emit(OpCodes.Ldc_I4_6);
                    break;
                case 7:
                    ilgen.Emit(OpCodes.Ldc_I4_7);
                    break;
                case 8:
                    ilgen.Emit(OpCodes.Ldc_I4_8);
                    break;
                default:
                    ilgen.Emit(OpCodes.Ldc_I4, val);
                    break;
            }
        }

        /// <summary>
        /// Läd eine Local auf den Stack
        /// </summary>
        /// <param name="ilgen"></param>
        /// <param name="iArg"></param>
        internal static void EmitLdLoc(ILGenerator ilgen, int iArg)
        {
            switch (iArg)
            {
                case 0:
                    ilgen.Emit(OpCodes.Ldloc_0);
                    break;
                case 1:
                    ilgen.Emit(OpCodes.Ldloc_1);
                    break;
                case 2:
                    ilgen.Emit(OpCodes.Ldloc_2);
                    break;
                case 3:
                    ilgen.Emit(OpCodes.Ldloc_3);
                    break;
                default:
                    ilgen.Emit(OpCodes.Ldloc, iArg);
                    break;
            }
        }

        /// <summary>
        /// Läd einen Primitiven Typen von einer Referenz
        /// Vorher: Oberster Stackwert = Referenz 
        /// Nachher: Oberster Stackwert = Wert
        /// </summary>
        /// <param name="ilgen"></param>
        /// <param name="t"></param>
        internal static void EmitLdInd(ILGenerator ilgen, Type t)
        {
            // geht leider nicht mit switch
            if (t == typeof(System.Boolean) || t == typeof(System.SByte))
                ilgen.Emit(OpCodes.Ldind_I1);
            else if (t == typeof(System.Int16))
                ilgen.Emit(OpCodes.Ldind_I2);
            else if (t == typeof(System.Int32))
                ilgen.Emit(OpCodes.Ldind_I4);
            else if (t == typeof(System.Int64))
                ilgen.Emit(OpCodes.Ldind_I8);
            else if (t == typeof(System.Single))
                ilgen.Emit(OpCodes.Ldind_R4);
            else if (t == typeof(System.Double))
                ilgen.Emit(OpCodes.Ldind_R8);
            else if (t == typeof(System.Byte))
                ilgen.Emit(OpCodes.Ldind_U1);
            else if (t == typeof(System.UInt16) || t == typeof(System.Char))
                ilgen.Emit(OpCodes.Ldind_U2);
            else if (t == typeof(System.UInt32))
                ilgen.Emit(OpCodes.Ldind_U4);
            else if (t == typeof(System.UInt64))
                ilgen.Emit(OpCodes.Ldind_I8);
            else if (t == typeof(System.UIntPtr) || (t == typeof(System.IntPtr)))
            {
                if (System.Environment.Is64BitProcess)
                {
                    ilgen.Emit(OpCodes.Ldind_I8);
                }
                else
                {
                    ilgen.Emit(OpCodes.Ldind_I4);
                }
            }
            else if (!t.IsPrimitive)
                ilgen.Emit(OpCodes.Ldobj, t);
            else
            {
                ThrowInvalidTypeException(t);
            }
        }

        private static void ThrowInvalidTypeException(Type t)
        {
            string source = "unknown";
            if (t.IsGenericParameter && t.DeclaringMethod != null)
            {
                source = t.DeclaringMethod.Name;
            }
            else if (t.DeclaringType != null)
            {
                source = t.DeclaringType.FullName;
            }
            
            throw new AspectWeaverException(1011, CodeBuilderErrors.ERR_1012, source, t.FullName);
        }

        /// <summary>
        /// Speichert Wert in eine Referenz
        /// </summary>
        /// <param name="ilgen"></param>
        /// <param name="t"></param>
        internal static void EmitStInd(ILGenerator ilgen, Type t)
        {
            // geht leider nicht mit switch
            if (!t.IsPrimitive)
                ilgen.Emit(OpCodes.Stobj, t);
            else if (t == typeof(System.Boolean) || t == typeof(System.SByte) || t == typeof(System.Byte))
                ilgen.Emit(OpCodes.Stind_I1);
            else if (t == typeof(System.Int16) || t == typeof(System.UInt16) || t == typeof(System.Char))
                ilgen.Emit(OpCodes.Stind_I2);
            else if (t == typeof(System.Int32) || t == typeof(System.UInt32))
                ilgen.Emit(OpCodes.Stind_I4);
            else if (t == typeof(System.Int64) || t == typeof(System.UInt64))
                ilgen.Emit(OpCodes.Stind_I8);
            else if (t == typeof(System.Single))
                ilgen.Emit(OpCodes.Stind_R4);
            else if (t == typeof(System.Double))
                ilgen.Emit(OpCodes.Stind_R8);
            else if (t == typeof(System.UIntPtr) || (t == typeof(System.IntPtr)))
            {
                if (System.Environment.Is64BitProcess)
                {
                    ilgen.Emit(OpCodes.Stind_I8);
                }
                else
                {
                    ilgen.Emit(OpCodes.Stind_I4);
                }
            }
            else
            {
                ThrowInvalidTypeException(t);
            }
        }

        /// <summary>
        /// Läd einen Wert aus einem Array
        /// </summary>
        /// <param name="ilgen"></param>
        /// <param name="t"></param>
        internal static void EmitLdElem(ILGenerator ilgen, Type t)
        {
            if (t.IsPrimitive)
            {
                t = t.UnderlyingSystemType;
                if (t == typeof(System.Boolean) || t == typeof(System.SByte))
                    ilgen.Emit(OpCodes.Ldelem_I1);
                else if (t == typeof(System.Int16))
                    ilgen.Emit(OpCodes.Ldelem_I2);
                else if (t == typeof(System.Int32))
                    ilgen.Emit(OpCodes.Ldelem_I4);
                else if (t == typeof(System.Int64) || t == typeof(System.UInt64))
                    ilgen.Emit(OpCodes.Ldelem_I8);
                else if (t == typeof(System.Single))
                    ilgen.Emit(OpCodes.Ldelem_R4);
                else if (t == typeof(System.Double))
                    ilgen.Emit(OpCodes.Ldelem_R8);
                else if (t == typeof(System.Byte))
                    ilgen.Emit(OpCodes.Ldelem_U1);
                else if (t == typeof(System.UInt16))
                    ilgen.Emit(OpCodes.Ldelem_U2);
                else if (t == typeof(System.UInt32))
                    ilgen.Emit(OpCodes.Ldelem_U4);
                else if (t == typeof(System.UIntPtr) || (t == typeof(System.IntPtr)))
                {
                    if (System.Environment.Is64BitProcess)
                    {
                        ilgen.Emit(OpCodes.Ldelem_I8);
                    }
                    else
                    {
                        ilgen.Emit(OpCodes.Ldelem_I4);
                    }

                }
                else
                {
                    ThrowInvalidTypeException(t);
                }
            }
            else
            {
                ilgen.Emit(OpCodes.Ldelem_Ref);
            }
        }

        /// <summary>
        /// Läd einen Wert aus einem Array
        /// </summary>
        /// <param name="ilgen"></param>
        /// <param name="t"></param>
        internal static void EmitStElem(ILGenerator ilgen, Type t)
        {
            if (t.IsPrimitive)
            {
                t = t.UnderlyingSystemType;
                if (t == typeof(System.Boolean) || t == typeof(System.SByte) || t == typeof(System.Byte))
                    ilgen.Emit(OpCodes.Stelem_I1);
                else if (t == typeof(System.Int16) || t == typeof(System.UInt16))
                    ilgen.Emit(OpCodes.Stelem_I2);
                else if (t == typeof(System.Int32) || t == typeof(System.UInt32))
                    ilgen.Emit(OpCodes.Stelem_I4);
                else if (t == typeof(System.Int64) || t == typeof(System.UInt64))
                    ilgen.Emit(OpCodes.Stelem_I8);
                else if (t == typeof(System.Single))
                    ilgen.Emit(OpCodes.Stelem_R4);
                else if (t == typeof(System.Double))
                    ilgen.Emit(OpCodes.Stelem_R8);
                else if (t == typeof(System.UIntPtr) || (t == typeof(System.IntPtr)))
                {
                    if (System.Environment.Is64BitProcess)
                    {
                        ilgen.Emit(OpCodes.Stelem_I8);
                    }
                    else
                    {
                        ilgen.Emit(OpCodes.Stelem_I4);
                    }

                }
                else
                {
                    ThrowInvalidTypeException(t);
                }
            }
            else
            {
                ilgen.Emit(OpCodes.Stelem_Ref);
            }
        }

        /// <summary>
        /// Läd einen konstanten Wert oder ein konstantes eindimensionales Array auf den Stack
        /// </summary>
        /// <param name="ilgen"></param>
        /// <param name="t">Der Elementtyp/ArrayTyp</param>
        /// <param name="value">Der konstante Wert</param>
        internal static void EmitLdConst(ILGenerator ilgen, Type t, object value)
        {
            if (t.IsPrimitive)
            {
                if (t == typeof(System.Boolean) ||
                    t == typeof(System.SByte) ||
                    t == typeof(System.Int16) ||
                    t == typeof(System.Int32) ||
                    t == typeof(System.Byte) ||
                    t == typeof(System.UInt16) ||
                    t == typeof(System.UInt32) ||
                    t == typeof(System.Char))
                {
                    ilgen.Emit(OpCodes.Ldc_I4, System.Convert.ToInt32(value));
                }
                else if (t == typeof(System.Int64))
                    ilgen.Emit(OpCodes.Ldc_I8, (Int64)value);
                else if (t == typeof(System.Single))
                    ilgen.Emit(OpCodes.Ldc_R4, (Single)value);
                else if (t == typeof(System.Double))
                    ilgen.Emit(OpCodes.Ldc_R8, (Double)value);
                else if (t == typeof(System.UInt64))
                    ilgen.Emit(OpCodes.Ldc_I8, System.Convert.ToInt64(value));
                else
                {
                    ThrowInvalidTypeException(t);
                }
            }
            else
            {
                if (t.IsSubclassOf(typeof(System.Enum)))
                {
                    EmitLdci4(ilgen, (int)value);
                }
                else if (t == typeof(string))
                {
                    if (value == null)
                    {
                        ilgen.Emit(OpCodes.Ldnull);
                    }
                    else
                    {
                        ilgen.Emit(OpCodes.Ldstr, (string)value);
                    }
                }
                else if (t == typeof(Type))
                {
                    ilgen.Emit(OpCodes.Ldtoken, (Type)value);
                    ilgen.Emit(OpCodes.Call, ReflectionObjects.c_mi_Type_GetTypeFromHandle);
                }
                else
                {
                    ThrowInvalidTypeException(t);
                }
            }
        }

        /// <summary>
        /// Läd ein konstantes Array auf den Stack
        /// </summary>
        /// <typeparam name="T">Der Elementtyp der übergebenen Collection</typeparam>
        /// <param name="ilgen"></param>
        /// <param name="arr">Eine Collection von Elementen, die auf den Stack geladen werden sollen</param>
        /// <param name="elemtype"></param>
        /// <param name="argumenttype">Das Typ Collection-Elements</param>
        /// <param name="argumentvalue">Der Wert des Collection Elements</param>
        public static void EmitLdConstArray<T>(ILGenerator ilgen, ICollection<T> arr, Type elemtype, Func<T,Type> argumenttype, Func<T, object> argumentvalue)
        {
            CommonCode.EmitLdci4(ilgen, arr.Count);
            ilgen.Emit(OpCodes.Newarr, elemtype);
            int iPos = 0;
            if (elemtype.IsValueType)
            {
                foreach (var arrarg in arr)
                {
                    Type argtype = argumenttype.Invoke(arrarg);
                    System.Diagnostics.Debug.Assert(argtype == elemtype);
                    ilgen.Emit(OpCodes.Dup);
                    CommonCode.EmitLdci4(ilgen, iPos++);
                    CommonCode.EmitLdConst(ilgen, argtype, argumentvalue.Invoke(arrarg));
                    CommonCode.EmitStElem(ilgen, argtype);
                }
            }
            else
            {
                foreach (var arrarg in arr)
                {
                    Type argtype = argumenttype.Invoke(arrarg);
                    ilgen.Emit(OpCodes.Dup);
                    CommonCode.EmitLdci4(ilgen, iPos++);
                    if (argtype.IsArray)
                    {
                        EmitLdConstArray(ilgen, (ICollection<T>)argumentvalue.Invoke(arrarg), argtype.GetElementType(), argumenttype, argumentvalue);
                    }
                    else 
                    {
                        CommonCode.EmitLdConst(ilgen, argtype, argumentvalue.Invoke(arrarg));
                        if (argtype.IsValueType)
                        {
                            ilgen.Emit(OpCodes.Box, argtype);
                        }
                    }
                    ilgen.Emit(OpCodes.Stelem_Ref);
                }
            }
        }

        /// <summary>
        /// Converts the return value firstparam the correct targettype
        /// </summary>
        /// <param name="ilgen"></param>
        /// <param name="returntype"></param>
        /// <param name="expectedtype"></param>
        internal static void EmitAdjustReturnValue(ILGenerator ilgen, Type returntype, Type expectedtype)
        {
            if (expectedtype == returntype) return;
            if (returntype != typeof(void) && expectedtype == typeof(void))
            {
                ilgen.Emit(OpCodes.Pop);
            }
            else if (expectedtype.IsValueType)
            {
                if (returntype == typeof(object))
                {
                    ilgen.Emit(OpCodes.Unbox, expectedtype);
                    EmitLdUnbox(ilgen, expectedtype);
                }
            }
            else if (expectedtype == typeof(object))
            {
                if (returntype == typeof(void))
                {
                    ilgen.Emit(OpCodes.Ldnull);
                }
                else if (returntype.IsValueType || returntype.IsGenericParameter)
                {
                    ilgen.Emit(OpCodes.Box, returntype);
                }
            }
            else
            {
                ilgen.Emit(OpCodes.Castclass, expectedtype);
            }
        }

        internal static LocalBuilder[] c_emptyLocalBuilderArray = new LocalBuilder[0];
        /// <summary>
        /// Läd ein Argumentenarray (Objectarray mit den Methodenargumenten) auf den Stack
        /// </summary>
        /// <remarks>
        /// Wenn eine Methode mit referenzparametern aufgerufen wird, die Argumente aber in einem Parameterarray vorliegen,
        /// so sind für alle Referenzparameter lokale Variablen des Referenzparametertyps angelegt worden. Diese lokalen
        /// Parameter werden als Referenz an die Methode übergeben. Nach dem Aufruf der Methode müssen die Inhalte der lokalen
        /// Variablen wieder zurück in das Array gespielt werden. Der Code wird durch diese Methode erzeugt
        /// </remarks>
        /// <param name="ilgen"></param>
        /// <param name="arraypos">nummer des parameters welcher das Array enthält</param>
        /// <param name="start">der erste Parameter ab dem das Paraminfoobjekt ausgewertet wird</param>
        /// <param name="parameters">Parameterinformation der Zielmethode</param>
        /// <param name="generics">konkreten generischen parameter oder null</param>
        /// <returns>Ein Array mit lokalen Variablen, in denen die Referenzparameter gespeichert sind</returns>
        internal static LocalBuilder[] EmitLdArrayToStack(ILGenerator ilgen, Action<ILGenerator> ldargarray, Type[] parameters, int start, Type[] generics, bool checklen)
        {
            ArrayList locals = null;
            ///Check array length
            if (checklen)
            {
                ldargarray(ilgen);
                ilgen.Emit(OpCodes.Ldlen);
                ilgen.Emit(OpCodes.Conv_I4);
                CommonCode.EmitLdci4(ilgen, parameters.Length);
                var ldargs = ilgen.DefineLabel();
                ilgen.Emit(OpCodes.Beq, ldargs);
                ilgen.Emit(OpCodes.Newobj, ReflectionObjects.c_ci_InvalidInvocationArgsException);
                ilgen.Emit(OpCodes.Throw);
                ilgen.MarkLabel(ldargs);
            }
            // load array to stack
            for (int iPos = start; iPos < parameters.Length; iPos++)
            {
                Type t = parameters[iPos];
                ldargarray(ilgen);
                CommonCode.EmitLdci4(ilgen, iPos);
                ilgen.Emit(OpCodes.Ldelem_Ref);

                Type tp;
                if (t.IsByRef)
                {
                    tp = t.GetElementType();
                }
                else
                {
                    tp = t;
                }
                if (tp.IsGenericParameter)
                {
                    tp = generics[tp.GenericParameterPosition];
                }
                if (tp.IsValueType)
                {
                    ilgen.Emit(OpCodes.Unbox, tp);
                    CommonCode.EmitLdUnbox(ilgen, tp);
                }
                else
                {
                    if (tp.IsGenericParameter)
                    {
                        ilgen.Emit(OpCodes.Unbox_Any, tp);
                    }
                    else
                    {
                        if (tp != typeof(object))
                        {
                            ilgen.Emit(OpCodes.Castclass, tp);
                        }
                    }
                }

                if (t.IsByRef) // copy in 
                {
                    if (locals == null) locals = new ArrayList();
                    LocalBuilder lb = ilgen.DeclareLocal(tp);
                    locals.Add(lb);
                    ilgen.Emit(OpCodes.Stloc, lb);
                    ilgen.Emit(OpCodes.Ldloca_S, lb);
                }
            }

            return locals == null ? c_emptyLocalBuilderArray : (LocalBuilder[])locals.ToArray(typeof(LocalBuilder));
        }

        /// <summary>
        /// Läd alle Referenzparameter aus den lokalen Substituten zurück ins Array, wird nach EmitLdArrayToStack aufgerufen
        /// </summary>
        /// <remarks>
        /// Wenn eine Methode mit referenzparametern aufgerufen wird, die Argumente aber in einem Parameterarray vorliegen,
        /// so sind für alle Referenzparameter lokale Variablen des Referenzparametertyps angelegt worden. Diese lokalen
        /// Parameter werden als Referenz an die Methode übergeben. Nach dem Aufruf der Methode müssen die Inhalte der lokalen
        /// Variablen wieder zurück in das Array gespielt werden. Der Code wird durch diese Methode erzeugt
        /// </remarks>
        /// <param name="ilgen"></param>
        /// <param name="parameters">Parameterinformation der Zielmethode</param>
        /// <param name="arraypos">nummer des parameters welcher das Array enthält</param>
        /// <param name="refparams">lokale Variablen, in denen die Ref-Parameter gespeichert sind</param>
        /// <param name="generics">gebundene generische typen</param>
        /// <param name="start">erster Parameter der vom parameters ausgewertet wird</param>
        internal static void EmitLdRefLocalsToArray(ILGenerator ilgen, Action<ILGenerator> ldargarray, Type[] parameters, int start, Type[] generics, LocalBuilder[] refparams)
        {
            if (refparams.Length == 0) return;

            int locals = 0;
            for (int iPos = start; iPos < parameters.Length; iPos++)
            {
                Type t = parameters[iPos];
                if (t.IsByRef) // copy out
                {
                    Type tp = t.GetElementType();
                    ldargarray(ilgen);
                    CommonCode.EmitLdci4(ilgen, iPos);
                    ilgen.Emit(OpCodes.Ldloc, refparams[locals]);
                    if (tp.IsValueType)
                    {
                        ilgen.Emit(OpCodes.Box, tp);
                    }
                    ilgen.Emit(OpCodes.Stelem_Ref);

                    locals++;
                }
            }
        }

        /// <summary>
        /// Läd alle Referenzparameter zurück ins Array, wird nach EmitLdArrayToStack aufgerufen
        /// Wenn eine Methode mit Referenzparametern aufgerufen wird, es aber noch einArgumentenarrays gibt,
        /// auf dem parallel operiert wird, so muss das Array nach dem Methodenaufruf geupdatet werden
        /// </summary>
        /// <param name="ilgen"></param>
        /// <param name="parameters">Parameterinformation der Zielmethode</param>
        /// <param name="arraypos">nummer des parameters welcher das Array enthält</param>
        internal static void EmitLdRefParamsToArray(ILGenerator ilgen, ParameterInfo[] parameters, int arraypos)
        {
            int locals = 0;
            for (int iPos = 0; iPos < parameters.Length; iPos++)
            {
                Type t = parameters[iPos].ParameterType;
                if (t.IsByRef) // copy out
                {
                    Type tp = t.GetElementType();
                    CommonCode.EmitLdArg(ilgen, arraypos);
                    CommonCode.EmitLdci4(ilgen, iPos);
                    ilgen.Emit(OpCodes.Ldloc, locals);
                    if (tp.IsValueType)
                    {
                        ilgen.Emit(OpCodes.Box, tp);
                    }
                    ilgen.Emit(OpCodes.Stelem_Ref);

                    locals++;
                }
            }
        }

        /// <summary>
        /// Läd ein Array mit Typobjekten auf den Stack
        /// </summary>
        /// <param name="ilgen"></param>
        /// <param name="types"></param>
        public static void EmitLdTypeArray(ILGenerator ilgen, Type[] types)
        {
            CommonCode.EmitLdci4(ilgen, types.Length);
            ilgen.Emit(OpCodes.Newarr, typeof(Type));
            for (int iPos = 0; iPos < types.Length; iPos++)
            {
                ilgen.Emit(OpCodes.Dup);
                CommonCode.EmitLdci4(ilgen, iPos);
                ilgen.Emit(OpCodes.Ldtoken, types[iPos]);
                ilgen.Emit(OpCodes.Call, ReflectionObjects.c_mi_Type_GetTypeFromHandle);
                ilgen.Emit(OpCodes.Stelem_Ref);
            }
        }

        /// <summary>
        /// Legt einen initialisierten Wertetypen auf den Stack
        /// </summary>
        /// <param name="ilgen"></param>
        /// <param name="type"></param>
        internal static void EmitInitValueType(ILGenerator ilgen, Type type)
        {
            // Nachfolgend wird eine Variante gewählt um ein Default-Objekt zu erzeugen und auf den Stack zu legen.
            ConstructorInfo ci = type.GetConstructor(ReflectionObjects.c_param_);
            if (ci != null)
            {
                ilgen.Emit(OpCodes.Newobj, ci);
            }
            else if (type == typeof(System.Double))
            {
                ilgen.Emit(OpCodes.Ldc_R8, 0.0);
            }
            else if (type == typeof(System.Single))
            {
                ilgen.Emit(OpCodes.Ldc_R4, (Single)0.0);
            }
            else
            {
                ilgen.Emit(OpCodes.Ldc_I4_0);
                if (type == typeof(System.Int64) || type == typeof(System.UInt64))
                {
                    ilgen.Emit(OpCodes.Conv_I8);
                }
            }
        }
    }
}
