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

using Loom;
using Loom.Runtime;
using Loom.Common;
using Loom.JoinPoints.Implementation;
using Loom.CodeBuilder.DynamicProxy.DataSlots;
using Loom.CodeBuilder.DynamicProxy.Declarations;

namespace Loom.CodeBuilder.DynamicProxy.CodeBricks
{
    /// <summary>
    /// Implementierung der generischen Context-Klasse
    /// </summary>
    internal class ContextClassImpl
    {

        /// <summary>
        /// fügt einem Typ eine von IntroducedContext abgeleitete Klasse als Inner-Klass hinzu
        /// </summary>
        /// <param name="pmeb"></param>
        /// <param name="tb"></param>
        /// <returns></returns>
        internal static ConstructorInfo DefineIntroductionContextClass(ProxyIntroducedMethodBuilder pmeb, out TypeBuilder tb)
        {
            MethodInfo ifc = pmeb.JoinPoint.MethodInterface;
            tb = pmeb.TypeBuilder.DefineNestedType(
                pmeb.DeclaringBuilder.GetContextClassName(),
                TypeAttributes.Sealed | TypeAttributes.NestedPublic | TypeAttributes.Serializable,
                typeof(IntroductionContext));

            // Wenn das Interface generisch ist, wird es an die Zielklasse gebunden
            Type[] genericargs = ifc.GetGenericArguments();
            if (genericargs != null && genericargs.Length != 0)
            {
                for (int iPos = 0; iPos < genericargs.Length; iPos++)
                {
                    genericargs[iPos] = pmeb.JoinPoint.DeclaringType;
                }
            }


            Type[] generics = MakeGeneric(genericargs, tb);

            ParameterInfo[] param = ifc.GetParameters();

            Implget_CurrentMethod(tb, ifc, generics);
            Implget_ReturnType(tb, ifc, generics);
            ImplGetParameterTypes(tb, param, generics);
            CommonMethodsImpl.Impl_get_TypeProperty(tb, ReflectionObjects.c_mi_AspectContext_getTargetClass, pmeb.DeclaringBuilder.TargetClass);
            ImplInvokeOn(tb, ifc, param, generics);

            ConstructorInfo ci = ImplCtor(tb, ReflectionObjects.c_ci_IntroductionContext_Object, ReflectionObjects.c_param_Object);
            if (generics != null)
            {
                Type newtype = tb.MakeGenericType(genericargs);
                return TypeBuilder.GetConstructor(newtype, ci);
            }
            return ci;

        }

        /// <summary>
        /// fügt einem Typ eine von CallContext abgeleitete Klasse als Inner-Klass hinzu
        /// </summary>
        /// <returns></returns>
        internal static ConstructorInfo DefineCallContextClass(ProxyAdviceMethodBuilder meb, out TypeBuilder tb)
        {
            MethodInfoJoinPoint jp = meb.JoinPoint;
            MethodInfo ifc = jp.MethodInterface;
            MethodInfo mi = ((IBaseCall)jp).BaseCall;
            ParameterInfo[] param = jp.Method.GetParameters();

            tb = meb.TypeBuilder.DefineNestedType(
                meb.DeclaringBuilder.GetContextClassName(),
                TypeAttributes.Sealed | TypeAttributes.NestedPublic | TypeAttributes.Serializable,
                typeof(CallContext));

            Type[] genericargs = mi.GetGenericArguments();
            Type[] generics = MakeGeneric(genericargs, tb);

            if (generics != null)
            {
                //mi=mi.MakeGenericMethod(generics);
                if (ifc != null)
                {
                    ifc = ifc.MakeGenericMethod(generics);
                }
            }
            if (ifc == null)
            {
                ifc = mi;
            }

            // Implementation bauen
            ConstructorBuilder cb = ImplCtor(tb, ReflectionObjects.c_ci_CallContext_Object, ReflectionObjects.c_param_Object);
            ImplInvoke(meb, tb, mi, param, generics);
            ImplInvokeOn(tb, ifc, param, generics);
            ImplGetParameterTypes(tb, param, generics);
            Implget_CurrentMethod(tb, jp.MethodImplementation, generics);
            Implget_ReturnType(tb, ifc, generics);
            CommonMethodsImpl.Impl_get_TypeProperty(tb, ReflectionObjects.c_mi_AspectContext_getTargetClass, jp.DeclaringType);
  
            if (generics != null)
            {
                Type newtype = tb.MakeGenericType(genericargs);
                return TypeBuilder.GetConstructor(newtype, cb);
            }

            return cb;
        }

        private static Type[] MakeGeneric(Type[] genericargs, TypeBuilder tb)
        {
            Type[] generics = null; // generische Parameter der Contextklasse
            if (genericargs != null && genericargs.Length > 0)
            {
                string[] names = new string[genericargs.Length];
                for (int iPos = 0; iPos < genericargs.Length; iPos++)
                {
                    names[iPos] = genericargs[iPos].Name;
                }
                generics = tb.DefineGenericParameters(names);
            }
            return generics;
        }

        /// <summary>
        /// fügt einem Typ eine von CreateContext abgeleitete Klasse als Inner-Klass hinzu
        /// </summary>
        /// <returns></returns>
        internal static ConstructorInfo DefineCreateContextClass(ProxyAdviceConstructorBuilder meb, out TypeBuilder tb)
        {
            tb = meb.TypeBuilder.DefineNestedType(
                meb.DeclaringBuilder.GetContextClassName(),
                TypeAttributes.Sealed | TypeAttributes.NestedPublic | TypeAttributes.Serializable,
                typeof(CreateContext));

            // Implementation bauen
            ConstructorBuilder cb = ImplCtor(tb, ReflectionObjects.c_ci_CreateContext_AspectArray, ReflectionObjects.c_param_TypeActivation_AspectArray);
            ImplInvoke(meb, tb);
            ImplGetParameterTypes(tb, meb.JoinPoint.Method.GetParameters(), null);
            Implget_CurrentMethod(tb, meb.JoinPoint.ConstructorImplementation, null);
            CommonMethodsImpl.Impl_get_TypeProperty(tb, ReflectionObjects.c_mi_AspectContext_getTargetClass, meb.JoinPoint.DeclaringType);

            return cb;
        }

        /// <summary>
        /// Baut den Konstruktor
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="cibasector"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private static ConstructorBuilder ImplCtor(TypeBuilder tb, ConstructorInfo cibasector, Type[] parameters)
        {
            ConstructorBuilder cb = tb.DefineConstructor(
                cibasector.Attributes,
                cibasector.CallingConvention,
                parameters);

            ILGenerator ilgen = cb.GetILGenerator();

            CommonCode.EmitLdArgList(ilgen, 0, parameters.Length + 1);
            ilgen.Emit(OpCodes.Tailcall);
            ilgen.Emit(OpCodes.Call, cibasector);
            ilgen.Emit(OpCodes.Ret);
            return cb;
        }

        /// <summary>
        /// Baut die InvokeInternal überladung
        /// </summary>
        /// <param name="pmeb"></param>
        /// <param name="tb"></param>
        /// <param name="method"></param>
        /// <param name="param"></param>
        /// <param name="generics"></param>
        private static void ImplInvoke(ProxyAdviceMethodBuilder pmeb, TypeBuilder tb, MethodInfo method, ParameterInfo[] param, Type[] generics)
        {
            if (method.IsAbstract) return;

            MethodBuilder meb = tb.DefineMethod("Invoke", MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.Public, typeof(object), ReflectionObjects.c_param_ObjectArray);
            ILGenerator ilgen = meb.GetILGenerator();
            LocalBuilder[] refparams;

            MethodInfo mibound = generics != null ? method.MakeGenericMethod(generics) : method;

            // Zielobjekt laden
            if (method.IsPrivate)
            {
                DelegateInfo di = DelegateCallImpl.DefineDelegate(pmeb, method, param, generics);
                if (generics != null) di = di.MakeGenericDelegate(generics);
                DelegateCallImpl.EmitLdDelegate(ilgen, pmeb.TypeBuilder, ReflectionObjects.c_fi_CallContext_instance, di, mibound, generics);
                mibound = di.Invoke;
            }
            else
            {
                ilgen.Emit(OpCodes.Ldarg_0);
                ilgen.Emit(OpCodes.Ldfld, ReflectionObjects.c_fi_CallContext_instance);
                ilgen.Emit(OpCodes.Castclass, mibound.DeclaringType);
            }

            // Parameterarray auf den Stack laden
            Type[] parameters = Common.Conversion.ToTypeArray(param);
            refparams = CommonCode.EmitLdArrayToStack(ilgen, il => il.Emit(OpCodes.Ldarg_1), parameters, 0, generics, true);

            // Originalmethode rufen
            ilgen.EmitCall(OpCodes.Call, mibound, null);
            // Referenzparameter und Returnalue anpassen
            CommonCode.EmitLdRefLocalsToArray(ilgen, il=>il.Emit(OpCodes.Ldarg_1), parameters, 0, generics, refparams);
            CommonCode.EmitAdjustReturnValue(ilgen, mibound.ReturnType, typeof(object));
            ilgen.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// Baut die InvokeInternal überladung
        /// </summary>
        /// <param name="pmeb"></param>
        /// <param name="tb"></param>
        private static void ImplInvoke(ProxyAdviceConstructorBuilder pmeb, TypeBuilder tb)
        {
            MethodBuilder meb = tb.DefineMethod("Invoke", MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.Public, typeof(object), ReflectionObjects.c_param_ObjectArray);
            ILGenerator ilgen = meb.GetILGenerator();
            MethodInfo mi = pmeb.JoinPoint.CreateAIMethod;

            ilgen.Emit(OpCodes.Ldarg_0);
            ilgen.Emit(OpCodes.Ldfld, ReflectionObjects.c_fi_CreateContext_classobject);
            ilgen.Emit(OpCodes.Castclass, mi.DeclaringType);
            ilgen.Emit(OpCodes.Ldarg_0);
            ilgen.Emit(OpCodes.Ldfld, ReflectionObjects.c_fi_CreateContext_aspects);

            Type[] param = Conversion.ToTypeArray(pmeb.JoinPoint.ConstructorImplementation.GetParameters());
            LocalBuilder[] refparams = CommonCode.EmitLdArrayToStack(ilgen, il => il.Emit(OpCodes.Ldarg_1), param, 0, null, true);
            // Originalmethode rufen
            if (refparams.Length == 0)
            {
                ilgen.Emit(OpCodes.Tailcall);
                ilgen.Emit(OpCodes.Call, mi);
            }
            else
            {
                ilgen.Emit(OpCodes.Call, mi);
                CommonCode.EmitLdRefLocalsToArray(ilgen, il => il.Emit(OpCodes.Ldarg_1), param, 0, null, refparams);
            }
            ilgen.Emit(OpCodes.Ret);
        }


        /// <summary>
        /// Baut die InvokeOnInternal überladung
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="baseifc"></param>
        /// <param name="param"></param>
        /// <param name="generics"></param>
        private static void ImplInvokeOn(TypeBuilder tb, MethodInfo baseifc, ParameterInfo[] param, Type[] generics)
        {
            MethodBuilder meb = tb.DefineMethod("InvokeOn", MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.Public, typeof(object), ReflectionObjects.c_param_Object_ObjectArray);
            ILGenerator ilgen = meb.GetILGenerator();
            // Zielobjekt in das Interface casten
            ilgen.Emit(OpCodes.Ldarg_1);
            ilgen.Emit(OpCodes.Castclass, baseifc.DeclaringType);
            // Parameterarray auf den Stack laden
            Type[] parameters = Common.Conversion.ToTypeArray(param);
            LocalBuilder[] refparams = CommonCode.EmitLdArrayToStack(ilgen, il => il.Emit(OpCodes.Ldarg_2), parameters, 0, generics, true);
            // Originalmethode rufen
            ilgen.Emit(OpCodes.Callvirt, baseifc);
            // Referenzparameter und Returnalue anpassen
            CommonCode.EmitLdRefLocalsToArray(ilgen, il => il.Emit(OpCodes.Ldarg_2), parameters, 0, generics, refparams);
            CommonCode.EmitAdjustReturnValue(ilgen, Common.Conversion.ResolveReturnType(baseifc, generics), typeof(object));
            ilgen.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// Baut das CurrentMethod Name Property
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="mb"></param>
        /// <param name="generics"></param>
        private static void Implget_CurrentMethod(TypeBuilder tb, MethodBase mb, Type[] generics)
        {
            MethodBuilder meb = tb.DefineMethod("get_CurrentMethod", MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.Public | MethodAttributes.SpecialName, typeof(MethodBase), ReflectionObjects.c_param_);
            ILGenerator ilgen = meb.GetILGenerator();
            MethodInfo mi = mb as MethodInfo;
            if (mi != null)
            {
                if (mi.IsGenericMethod)
                {
                    mi = mi.MakeGenericMethod(generics);
                    ilgen.Emit(OpCodes.Ldtoken, mi);
                    ilgen.Emit(OpCodes.Call, ReflectionObjects.c_mi_MethodBase_MethodFromHandle_1);
                    ilgen.Emit(OpCodes.Castclass, typeof(MethodInfo));
                    ilgen.Emit(OpCodes.Callvirt, ReflectionObjects.c_mi_MethodInfo_GetGenericMethodDefinition);
                }
                else
                {
                    ilgen.Emit(OpCodes.Ldtoken, mi);
                    ilgen.Emit(OpCodes.Call, ReflectionObjects.c_mi_MethodBase_MethodFromHandle_1);
                }
            }
            else
            {
                ilgen.Emit(OpCodes.Ldtoken, (ConstructorInfo)mb);
                ilgen.Emit(OpCodes.Call, ReflectionObjects.c_mi_MethodBase_MethodFromHandle_1);
            }
            ilgen.Emit(OpCodes.Ret);
        }


        /// <summary>
        /// Baut das ReturnType Property
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="baseifc"></param>
        /// <param name="generics"></param>
        private static void Implget_ReturnType(TypeBuilder tb, MethodInfo baseifc, Type[] generics)
        {
            MethodBuilder meb = tb.DefineMethod("get_ReturnType", MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.Public | MethodAttributes.SpecialName, typeof(Type), ReflectionObjects.c_param_);
            ILGenerator ilgen = meb.GetILGenerator();
            ilgen.Emit(OpCodes.Ldtoken, Common.Conversion.ResolveReturnType(baseifc, generics));
            ilgen.Emit(OpCodes.Tailcall);
            ilgen.Emit(OpCodes.Call, ReflectionObjects.c_mi_Type_GetTypeFromHandle);
            ilgen.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// gibt alle Methodenparameter zurück
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="param"></param>
        /// <param name="generics"></param>
        private static void ImplGetParameterTypes(TypeBuilder tb, ParameterInfo[] param, Type[] generics)
        {
            MethodBuilder meb = tb.DefineMethod("GetParameterTypes", MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.Public, typeof(Type[]), ReflectionObjects.c_param_);
            ILGenerator ilgen = meb.GetILGenerator();

            CommonCode.EmitLdci4(ilgen, param.Length);
            ilgen.Emit(OpCodes.Newarr, typeof(Type));
            for (int iPos = 0; iPos < param.Length; iPos++)
            {
                ilgen.Emit(OpCodes.Dup);
                CommonCode.EmitLdci4(ilgen, iPos);
                Type t = param[iPos].ParameterType;
                if (t.IsGenericParameter) // muss übersetzt werden in den generischen Parameter der Klasse
                {
                    ilgen.Emit(OpCodes.Ldtoken, generics[t.GenericParameterPosition]);
                }
                else
                {
                    if (t.IsByRef) t = t.GetElementType();
                    ilgen.Emit(OpCodes.Ldtoken, t);
                }
                ilgen.Emit(OpCodes.Call, ReflectionObjects.c_mi_Type_GetTypeFromHandle);
                ilgen.Emit(OpCodes.Stelem_Ref);
            }
            ilgen.Emit(OpCodes.Ret);
        }

        public static void Impl_ReCallIndirect(TypeBuilder tb)
        {
            MethodBuilder meb = tb.DefineMethod("ReCall", MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.Public, typeof(object), ReflectionObjects.c_param_ObjectArray);

            FieldBuilder fb = tb.DefineField("__recall", typeof(RecallDelegate), FieldAttributes.Static | FieldAttributes.Private);

            ILGenerator ilgen = meb.GetILGenerator();
            Label lbexist = ilgen.DefineLabel();
            ilgen.Emit(OpCodes.Ldsfld, fb);
            ilgen.Emit(OpCodes.Ldnull);
            ilgen.Emit(OpCodes.Ceq);
            ilgen.Emit(OpCodes.Brfalse_S, lbexist);
            ilgen.Emit(OpCodes.Ldsflda, fb);
            ilgen.Emit(OpCodes.Ldarg_0);
            ilgen.Emit(OpCodes.Ldarg_1);
            ilgen.Emit(OpCodes.Call, ReflectionObjects.c_mi_Recall_CreateRecallDelegate);
            ilgen.MarkLabel(lbexist);
            ilgen.Emit(OpCodes.Ldsfld, fb);
            ilgen.Emit(OpCodes.Ldarg_0);
            ilgen.Emit(OpCodes.Ldfld, ReflectionObjects.c_fi_CallContext_instance);
            ilgen.Emit(OpCodes.Ldarg_1);
            ilgen.Emit(OpCodes.Tailcall);
            ilgen.Emit(OpCodes.Call, ReflectionObjects.c_mi_RecallDelegate_Invoke);
            ilgen.Emit(OpCodes.Ret);
        }

        public static void Impl_ReCallDirect(TypeBuilder tb, Type basetype, CommonMethodsImpl.SortedJoinPoints micandidates)
        {
            MethodBuilder meb = tb.DefineMethod("ReCall", MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.Public, typeof(object), ReflectionObjects.c_param_ObjectArray);

            ILGenerator ilgen = meb.GetILGenerator();
            CommonMethodsImpl.ImplDynamicDispatch(ilgen, micandidates, il => il.Emit(OpCodes.Ldarg_1), il =>
            {
                ilgen.Emit(OpCodes.Ldarg_0);
                ilgen.Emit(OpCodes.Ldfld, ReflectionObjects.c_fi_CallContext_instance);
                ilgen.Emit(OpCodes.Castclass, basetype);
            });
        }
    }

}
