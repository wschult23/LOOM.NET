// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using Loom.Common;
using Loom.CodeBuilder.DynamicProxy.Declarations;

namespace Loom.CodeBuilder.DynamicProxy.CodeBricks
{
    internal sealed class DelegateCallImpl
    {
        /// <summary>
        /// Baut ein Delegate zum aufruf der Privaten basismethode
        /// Bei Generics muss leider eine Hashtable aufgebaut werden, da das Generic object die Delegaten-Instanz selbst innehält
        /// Sonst kännte man den Code effizienter bauen, da das Delegate statisch in der Delegateklasse abgelegt werden 
        /// kännte.
        /// 
        /// </summary>
        /// <param name="pmeb"></param>
        /// <param name="basemethod"></param>
        /// <param name="param"></param>
        /// <param name="generics"></param>
        /// <returns></returns>
        internal static DelegateInfo DefineDelegate(ProxyMemberBuilder pmeb, MethodInfo basemethod, ParameterInfo[] param, Type[] generics)
        {
            DelegateInfo di;
            if (pmeb.Declarations.Delegates.TryGetValue(basemethod, out di))
            {
                return di;
            }

            // Delegatetyp definieren
            TypeBuilder tbdel = pmeb.TypeBuilder.DefineNestedType("InvokeDelegate" + Strings.GetHashString(basemethod), TypeAttributes.AnsiClass | TypeAttributes.AutoClass | TypeAttributes.NestedPublic | TypeAttributes.Sealed, typeof(MulticastDelegate));

            GenericTypeParameterBuilder[] genericdel = null;
            Type tbdelgeneric = tbdel;
            FieldBuilder fi;

            // Generische Parameter auf Zielmethode mappen und Feld anlegen
            if (generics != null)
            {
                string[] names = new string[generics.Length];
                for (int iPos = 0; iPos < generics.Length; iPos++)
                {
                    names[iPos] = generics[iPos].Name;
                }
                genericdel = tbdel.DefineGenericParameters(names);
                tbdelgeneric = tbdel.MakeGenericType(generics);
                fi = pmeb.TypeBuilder.DefineField("delegate_" + Strings.GetHashString(tbdel), typeof(Dictionary<Type, Delegate>), FieldAttributes.Public | FieldAttributes.NotSerialized);
            }
            else
            {
                fi = pmeb.TypeBuilder.DefineField("delegate_" + Strings.GetHashString(tbdel), typeof(Delegate), FieldAttributes.Public | FieldAttributes.NotSerialized);
            }

            ConstructorBuilder cbdel = tbdel.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.Standard, new Type[] { typeof(object), typeof(IntPtr) });
            cbdel.SetImplementationFlags(MethodImplAttributes.Runtime);

            Type[] paramtypes = new Type[param.Length];
            for (int i = 0; i < param.Length; i++)
            {
                Type t = param[i].ParameterType;
                if (t.IsGenericParameter)
                {
                    t = genericdel[t.GenericParameterPosition];
                }
                paramtypes[i] = t;
            }
            Type rettype = basemethod.ReturnType;
            if (rettype.IsGenericParameter) rettype = genericdel[rettype.GenericParameterPosition];

            MethodBuilder mbdel = tbdel.DefineMethod("Invoke", MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Public, rettype, paramtypes);
            mbdel.SetImplementationFlags(MethodImplAttributes.Runtime);

            di = new DelegateInfo(tbdel, mbdel, fi);
            pmeb.Declarations.Delegates.Add(basemethod, di);
            return di;
        }

        /// <summary>
        /// Baut code, der eine Delegateinstanz auf den STack erzeugt, damit die Zielmethode indirekt über das Delegate aufgerufen werden kann
        /// </summary>
        /// <param name="ilgen">Der ilgenerator</param>
        /// <param name="proxytype">der Typ des gerade zu bauenden proxies</param>
        /// <param name="fiinstance">der Member welcher die proxy-Instanz hält oder null, wenn es der this zeiger ist</param>
        /// <param name="di">der delegatentyp</param>
        /// <param name="basemethod">die aufzurufende methode</param>
        /// <param name="generics">die generischen parameter</param>
        internal static void EmitLdDelegate(ILGenerator ilgen, Type proxytype, FieldInfo fiinstance, DelegateInfo di, MethodInfo basemethod, Type[] generics)
        {
            EmitLdInstance(ilgen, proxytype, fiinstance);

            ilgen.Emit(OpCodes.Ldtoken, di.DelegateType);
            ilgen.Emit(OpCodes.Call, ReflectionObjects.c_mi_Type_GetTypeFromHandle);
            ilgen.Emit(OpCodes.Ldtoken, basemethod);
            // Wenn der Declaring-Type der Basismethode ein Generic ist, muss bei MethodBase.GetMethodFromHandle dieser mit übergeben werden
            if (basemethod.DeclaringType.IsGenericType)
            {
                ilgen.Emit(OpCodes.Ldtoken, basemethod.DeclaringType);
                ilgen.Emit(OpCodes.Call, ReflectionObjects.c_mi_MethodBase_MethodFromHandle_2);
            }
            else
            {
                ilgen.Emit(OpCodes.Call, ReflectionObjects.c_mi_MethodBase_MethodFromHandle_1);
            }
            ilgen.Emit(OpCodes.Castclass, typeof(MethodInfo));

            if (generics != null)
            {
                CommonCode.EmitLdTypeArray(ilgen, generics);

                EmitLdInstance(ilgen, proxytype, fiinstance);

                ilgen.Emit(OpCodes.Ldflda, di.Field);
                ilgen.Emit(OpCodes.Call, ReflectionObjects.c_mi_Common_CreateDelegate_2);
            }
            else
            {
                EmitLdInstance(ilgen, proxytype, fiinstance);


                ilgen.Emit(OpCodes.Ldflda, di.Field);

                ilgen.Emit(OpCodes.Call, ReflectionObjects.c_mi_Common_CreateDelegate_1);
            }
            ilgen.Emit(OpCodes.Castclass, di.DelegateType);
        }

        private static void EmitLdInstance(ILGenerator ilgen, Type proxytype, FieldInfo fiinstance)
        {
            ilgen.Emit(OpCodes.Ldarg_0);
            if (fiinstance != null)
            {
                ilgen.Emit(OpCodes.Ldfld, fiinstance);
                ilgen.Emit(OpCodes.Castclass, proxytype);
            }
        }

    }
}
