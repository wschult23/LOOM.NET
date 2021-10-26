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

using Loom;
using Loom.Common;
using Loom.CodeBuilder.DynamicProxy.Declarations;


namespace Loom.CodeBuilder.DynamicProxy.CodeBricks
{
    /// <summary>
    /// Baut das IAspectInfo-Interface in das Objet ein.
    /// </summary>
    internal class AspectInfoImpl
    {
        /// <summary>
        /// Implementert ein IAspectInfo-Interface auf einem neuen Typ
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="aspectfield"></param>
        /// <param name="targetclass"></param>
        /// <param name="mibaseGetAspects"></param>
        static internal MethodInfo AddIDynamicAspectInfoInterface(TypeBuilder tb, AspectField[] aspectfield, Type targetclass, MethodInfo mibaseGetAspects)
        {
            MethodInfo migetaspectprotected = ImplGetAspectProtected(tb, aspectfield, mibaseGetAspects);

            if (mibaseGetAspects == null)
            {
                MethodInfo migetaspectprivate = ImplGetAspectPrivate(tb, migetaspectprotected);
                tb.AddInterfaceImplementation(typeof(Loom.IAspectInfo));
                ImplGetAspectIf1(tb, migetaspectprivate);
                ImplGetAspectIf2(tb, migetaspectprivate);
                ImplTargetClass(tb, targetclass);
            }

            return migetaspectprotected;
        }

        /// <summary>
        /// Implementier das DeclaringType-Property
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="targetclass"></param>
        private static void ImplTargetClass(TypeBuilder tb, Type targetclass)
        {
            MethodBuilder meb = tb.DefineMethod(
                Common.Strings.GetInterfaceMethodName(ReflectionObjects.c_mi_IAspectInfo_TargetClass),
                MethodAttributes.NewSlot | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Private | MethodAttributes.Final,
                typeof(Type), ReflectionObjects.c_param_);
            tb.DefineMethodOverride(meb, ReflectionObjects.c_mi_IAspectInfo_TargetClass);
            ILGenerator ilgen = meb.GetILGenerator();
            ilgen.Emit(OpCodes.Ldtoken, targetclass);
            ilgen.Emit(OpCodes.Tailcall);
            ilgen.Emit(OpCodes.Call, ReflectionObjects.c_mi_Type_GetTypeFromHandle);
            ilgen.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// Diese Methode wird von der privaten __GetAspects aufgerufen.
        /// Sie ruft __AddAspect auf dem verwobenen Aspect und wenn die Basisklasse auch eine
        /// verwobene Klasse ist dann __GetAspects auf der Basisklasse auf
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="aspectfields"></param>
        /// <param name="mibaseimpl"></param>
        /// <returns></returns>
        private static MethodInfo ImplGetAspectProtected(TypeBuilder tb, AspectField[] aspectfields, MethodInfo mibaseimpl)
        {
            // Aspektimplementation rufen
            MethodAttributes attr = MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.Virtual;
            if (mibaseimpl == null) attr |= MethodAttributes.NewSlot;

            MethodBuilder meb = tb.DefineMethod("__GetAspects", attr, typeof(void), ReflectionObjects.c_param_ArrayList_Type);
            ILGenerator ilgen = meb.GetILGenerator();

            // Basismethode aufrufen
            if (mibaseimpl != null)
            {
                ilgen.Emit(OpCodes.Ldarg_0);
                ilgen.Emit(OpCodes.Ldarg_1);
                ilgen.Emit(OpCodes.Ldarg_2);
                ilgen.Emit(OpCodes.Call, mibaseimpl);
            }

            // Und die ASpekte reintuen
            foreach (AspectField decl in aspectfields)
            {
                if (decl.IsStatic)
                {
                    ilgen.Emit(OpCodes.Ldsfld, decl.FieldInfo);
                }
                else
                {
                    ilgen.Emit(OpCodes.Ldarg_0);
                    ilgen.Emit(OpCodes.Ldfld, decl.FieldInfo);
                }
                ilgen.Emit(OpCodes.Ldarg_1);
                ilgen.Emit(OpCodes.Ldarg_2);
                ilgen.Emit(OpCodes.Call, ReflectionObjects.c_mi_Common_AddAspect); // hier noch kein Tailcall
            }

            ilgen.Emit(OpCodes.Ret);

            return meb;
        }

        /// <summary>
        /// Diese Methode wird von den Interface-Implementierungen aufgerufen
        /// Sie baut ein ArrayList, in dem die Aspekte durch aufruf von der äffentlichen __GetAspects-Methode
        /// eingesammelt werden. Das ArrayList wird dann in ein Aspect[] umgewandelt
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="migetaspect"></param>
        /// <returns></returns>
        private static MethodInfo ImplGetAspectPrivate(TypeBuilder tb, MethodInfo migetaspect)
        {
            MethodBuilder meb = tb.DefineMethod("__GetAspects", MethodAttributes.Private | MethodAttributes.HideBySig, typeof(Aspect[]), ReflectionObjects.c_param_Type);

            ILGenerator ilgen = meb.GetILGenerator();
            LocalBuilder lbArray = ilgen.DeclareLocal(typeof(ArrayList));
            ilgen.Emit(OpCodes.Ldarg_0);
            ilgen.Emit(OpCodes.Newobj, ReflectionObjects.c_ci_ArrayList_);
            ilgen.Emit(OpCodes.Dup);
            ilgen.Emit(OpCodes.Stloc_0);
            ilgen.Emit(OpCodes.Ldarg_1);
            ilgen.Emit(OpCodes.Callvirt, migetaspect);
            ilgen.Emit(OpCodes.Ldloc_0);
            ilgen.Emit(OpCodes.Ldtoken, typeof(Aspect));
            ilgen.Emit(OpCodes.Call, ReflectionObjects.c_mi_Type_GetTypeFromHandle);
            ilgen.Emit(OpCodes.Callvirt, ReflectionObjects.c_mi_ArrayList_ToArray);
            ilgen.Emit(OpCodes.Castclass, typeof(Aspect[]));

            ilgen.Emit(OpCodes.Ret);

            return meb;
        }

        /// <summary>
        /// Die erste überladung des IAspectInfo-Interfaces
        /// Ruft die private __GetAspects Implementierung mit typeof(Aspects) als Parameter auf
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="migetaspectprivate"></param>
        static void ImplGetAspectIf1(TypeBuilder tb, MethodInfo migetaspectprivate)
        {
            MethodBuilder meb = tb.DefineMethod(
                Common.Strings.GetInterfaceMethodName(ReflectionObjects.c_mi_IAspectInfo_GetAspect_1),
                MethodAttributes.NewSlot | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Private | MethodAttributes.Final,
                typeof(Aspect[]), ReflectionObjects.c_param_);
            tb.DefineMethodOverride(meb, ReflectionObjects.c_mi_IAspectInfo_GetAspect_1);
            ILGenerator ilgen = meb.GetILGenerator();
            ilgen.Emit(OpCodes.Ldarg_0);
            ilgen.Emit(OpCodes.Ldnull);
            ilgen.Emit(OpCodes.Tailcall);
            ilgen.Emit(OpCodes.Call, migetaspectprivate);
            ilgen.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// Die zweite überladung des IAspectInfo-Interfaces
        /// Ruft die private __GetAspects Implementierung auf
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="migetaspectprivate"></param>
        static void ImplGetAspectIf2(TypeBuilder tb, MethodInfo migetaspectprivate)
        {
            MethodBuilder meb = tb.DefineMethod(
                Common.Strings.GetInterfaceMethodName(ReflectionObjects.c_mi_IAspectInfo_GetAspect_2),
                MethodAttributes.NewSlot | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Private | MethodAttributes.Final,
                typeof(Aspect[]), ReflectionObjects.c_param_Type);
            tb.DefineMethodOverride(meb, ReflectionObjects.c_mi_IAspectInfo_GetAspect_2);
            ILGenerator ilgen = meb.GetILGenerator();
            ilgen.Emit(OpCodes.Ldarg_0);
            ilgen.Emit(OpCodes.Ldarg_1);
            ilgen.Emit(OpCodes.Tailcall);
            ilgen.Emit(OpCodes.Call, migetaspectprivate);
            ilgen.Emit(OpCodes.Ret);
        }
    }
}
