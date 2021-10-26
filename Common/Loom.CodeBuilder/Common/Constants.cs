// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using Loom.CodeBuilder.DynamicProxy.CodeBricks;
using Loom.JoinPoints;

namespace Loom.Common
{
    /// <summary>
    /// Summary description for Constants.
    /// </summary>
    internal class WeavingCodeNames
    {
        public readonly static string c_moduleName = "Weave";
        public readonly static string c_aspectFieldName = "$aspect";
        public readonly static string c_baseCallerMethodName = "$Invoke_";
        public readonly static string c_interfaceMethodName = "$Impl_";
        public readonly static string c_baseStaticMethodInfoField = "$method_";
        public readonly static string c_baseCallerClassName = "$Call_";
        public readonly static string c_IntroduceContext_ClassName = "$Intro_";
        public readonly static string c_createInstanceName = "$CreateInstance";
        public readonly static string c_serialize_basetype = "#weave!__basetype";
        public readonly static string c_serialize_aspects = "#weave!__aspects";
        public readonly static string c_serialize_attributed = "#weave!__attributed";
        public readonly static string c_classObjectExtension = "$Class";
        public readonly static string c_LoomNamespace = "$$Loom";
        public readonly static string c_aspectFieldContainerPrefix = c_LoomNamespace + ".$Container";
        public readonly static string c_assemblyNamePrefix = "Loom.";

        private static Regex c_regClassObjectName = new Regex(@"(\[.*\])|([\+.`\[\]])", RegexOptions.Compiled);
        private static Regex c_regClassName = new Regex(@"(\[.*\])|([\+`\[\]])", RegexOptions.Compiled);

        private static int c_classtypeid = 0;
        public static string GetClassObjectName(Type basetype)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(c_LoomNamespace);
            sb.Append(".");
            sb.Append(basetype.Name);
            sb.Append('$');
            sb.Append(c_classtypeid++);
            sb.Append(c_classObjectExtension);
            return sb.ToString();
        }

        public static string GetClassName(Type targetclass)
        {
            return c_regClassName.Replace(targetclass.FullName, "$");
        }

        private static int c_classid = 0;
        public static string GetGenericClassName(Type basetype)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(c_LoomNamespace);
            sb.Append(".");
            sb.Append(c_regClassObjectName.Replace(basetype.FullName, "$"));
            sb.Append('$');
            sb.Append(c_classid++);
            return sb.ToString();
        }
    }

    internal class SimpleObjects
    {
        public readonly static object[] c_emptyobjectarray = new object[0];
        public readonly static AspectAttribute[] c_emptyaspectarray = new AspectAttribute[0];
    }

    /// <summary>
    ///  enthält alle Reflection-Objecte wie MethodInfos, die zum erzeugen von Code benötigt werden
    ///  
    /// </summary>
    internal class ReflectionObjects
    {
        public readonly static Type[] c_param_ = System.Reflection.Emit.TypeBuilder.EmptyTypes;
        public readonly static Type[] c_param_ArrayList_Type = new Type[] { typeof(ArrayList), typeof(Type) };
        public readonly static Type[] c_param_ObjectArray = new Type[] { typeof(object[]) };
        public readonly static Type[] c_param_Object_ObjectArray = new Type[] { typeof(object), typeof(object[]) };
        public readonly static Type[] c_param_Type = new Type[] { typeof(Type) };
        public readonly static Type[] c_param_Object = new Type[] { typeof(object) };
        public readonly static Type[] c_param_AspectArray_ObjectArray = new Type[] { typeof(Aspect[]), typeof(object[]) };
        public readonly static Type[] c_param_TypeActivation_AspectArray = new Type[] { typeof(Runtime.ClassObject), typeof(Aspect[]) };
        public readonly static Type[] c_param_List1Type = new Type[] { typeof(List<Type>) };
        public readonly static Type[] c_param_Exception = new Type[] { typeof(Exception) };
        public readonly static Type[] c_param_bool = new Type[] { typeof(bool) };


        public readonly static MethodInfo c_mi_MethodBase_MethodFromHandle_1;
        public readonly static MethodInfo c_mi_MethodBase_MethodFromHandle_2;
        public readonly static ConstructorInfo c_ci_CallContext_Object;
        public readonly static FieldInfo c_fi_CallContext_instance;
        public readonly static ConstructorInfo c_ci_ArrayList_;
        public readonly static MethodInfo c_mi_ArrayList_ToArray;
        public readonly static MethodInfo c_mi_Type_GetTypeFromHandle;
        public readonly static MethodInfo c_mi_IAspectInfo_GetAspect_1;
        public readonly static MethodInfo c_mi_IAspectInfo_GetAspect_2;
        public readonly static MethodInfo c_mi_IAspectInfo_TargetClass;
        public readonly static MethodInfo c_mi_Common_AddAspect;
        public readonly static MethodInfo c_mi_Common_CreateDelegate_1;
        public readonly static MethodInfo c_mi_Common_CreateDelegate_2;
        public readonly static MethodInfo c_mi_Common_ReplaceTypedNull;
        public readonly static ConstructorInfo c_ci_IntroductionContext_Object;
        public readonly static MethodInfo c_mi_ClassObject_getType;
        public readonly static MethodInfo c_mi_ClassObject_getTargetClass;
        public readonly static MethodInfo c_mi_ClassObject_CreateInstanceInternal;
        public readonly static MethodInfo c_mi_ClassObject_GetAspectArray;
        public readonly static ConstructorInfo c_ci_CreateContext_AspectArray;
        public readonly static FieldInfo c_fi_CreateContext_classobject;
        public readonly static FieldInfo c_fi_CreateContext_aspects;
        public readonly static MethodInfo c_mi_List1Type_Insert;
        public readonly static MethodInfo c_mi_Object_GetType;
        public readonly static MethodInfo c_mi_AspectContext_getTargetClass;
        public readonly static MethodInfo c_mi_Recall_CreateRecallDelegate;
        public readonly static MethodInfo c_mi_RecallDelegate_Invoke;
        public readonly static ConstructorInfo c_ci_MissingTargetMethodException_ObjectArray;
        public readonly static ConstructorInfo c_ci_InvalidInvocationArgsException;
        public readonly static MethodInfo c_mi_MethodInfo_GetGenericMethodDefinition;
        public readonly static MethodInfo c_mi_TypedNull_IsType;

        // Late Bound Members
        public static ConstructorInfo c_ci_AbstractClassException_;


        public static void Initialize(Type typeAbstractClassException)
        {
            c_ci_AbstractClassException_ = typeAbstractClassException.GetConstructor(new Type[] { typeof(Runtime.ClassObject) });
            System.Diagnostics.Debug.Assert(c_ci_AbstractClassException_ != null);
        }

        static ReflectionObjects()
        {
            Type tp;

            tp = typeof(MethodBase);

            c_mi_MethodBase_MethodFromHandle_1 = tp.GetMethod("GetMethodFromHandle", new Type[] { typeof(RuntimeMethodHandle) });
            System.Diagnostics.Debug.Assert(c_mi_MethodBase_MethodFromHandle_1 != null);

            c_mi_MethodBase_MethodFromHandle_2 = tp.GetMethod("GetMethodFromHandle", new Type[] { typeof(RuntimeMethodHandle), typeof(RuntimeTypeHandle) });
            System.Diagnostics.Debug.Assert(c_mi_MethodBase_MethodFromHandle_2 != null);

            tp = typeof(ArrayList);

            c_ci_ArrayList_ = tp.GetConstructor(c_param_);
            System.Diagnostics.Debug.Assert(c_ci_ArrayList_ != null);

            c_mi_ArrayList_ToArray = tp.GetMethod("ToArray", c_param_Type);
            System.Diagnostics.Debug.Assert(c_mi_ArrayList_ToArray != null);

            c_mi_Type_GetTypeFromHandle = typeof(Type).GetMethod("GetTypeFromHandle");
            System.Diagnostics.Debug.Assert(c_mi_Type_GetTypeFromHandle != null);

            tp = typeof(Loom.IAspectInfo);

            c_mi_IAspectInfo_GetAspect_1 = tp.GetMethod("GetAspects", c_param_);
            System.Diagnostics.Debug.Assert(c_mi_IAspectInfo_GetAspect_1 != null);

            c_mi_IAspectInfo_GetAspect_2 = tp.GetMethod("GetAspects", c_param_Type);
            System.Diagnostics.Debug.Assert(c_mi_IAspectInfo_GetAspect_2 != null);

            tp = typeof(Loom.IAspectInfo);

            c_mi_IAspectInfo_TargetClass = tp.GetMethod("get_TargetClass", c_param_);
            System.Diagnostics.Debug.Assert(c_mi_IAspectInfo_TargetClass != null);

            tp = typeof(Loom.Runtime.CallContext);

            c_ci_CallContext_Object = tp.GetConstructor(c_param_Object);
            System.Diagnostics.Debug.Assert(c_ci_CallContext_Object != null);

            c_fi_CallContext_instance = tp.GetField("instance", BindingFlags.NonPublic | BindingFlags.Instance);
            System.Diagnostics.Debug.Assert(c_fi_CallContext_instance != null);

            c_ci_IntroductionContext_Object = typeof(Loom.Runtime.IntroductionContext).GetConstructor(c_param_Object);
            System.Diagnostics.Debug.Assert(c_ci_IntroductionContext_Object != null);

            tp = typeof(Loom.Runtime.ClassObject);

            c_mi_ClassObject_getType = tp.GetProperty("Type").GetGetMethod();
            System.Diagnostics.Debug.Assert(c_mi_ClassObject_getType != null);

            c_mi_ClassObject_getTargetClass = tp.GetProperty("TargetClass").GetGetMethod();
            System.Diagnostics.Debug.Assert(c_mi_ClassObject_getTargetClass != null);

            c_mi_ClassObject_CreateInstanceInternal = tp.GetMethod("CreateInstanceInternal", BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public, null, c_param_AspectArray_ObjectArray, null);
            System.Diagnostics.Debug.Assert(c_mi_ClassObject_CreateInstanceInternal != null);

            c_mi_ClassObject_GetAspectArray = tp.GetMethod("GetAspects", BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public, null, c_param_bool, null);
            System.Diagnostics.Debug.Assert(c_mi_ClassObject_GetAspectArray != null);

            tp = typeof(Loom.Runtime.CreateContext);

            c_ci_CreateContext_AspectArray = tp.GetConstructor(c_param_TypeActivation_AspectArray);
            System.Diagnostics.Debug.Assert(c_ci_CreateContext_AspectArray != null);

            c_fi_CreateContext_aspects = tp.GetField("aspects", BindingFlags.NonPublic | BindingFlags.Instance);
            System.Diagnostics.Debug.Assert(c_fi_CreateContext_aspects != null);

            c_fi_CreateContext_classobject = tp.GetField("classobject", BindingFlags.NonPublic | BindingFlags.Instance);
            System.Diagnostics.Debug.Assert(c_fi_CreateContext_classobject != null);

            c_mi_List1Type_Insert = typeof(List<Type>).GetMethod("Insert", new Type[] { typeof(int), typeof(Type) });
            System.Diagnostics.Debug.Assert(c_mi_List1Type_Insert != null);

            c_mi_Object_GetType = typeof(object).GetMethod("GetType", c_param_);
            System.Diagnostics.Debug.Assert(c_mi_Object_GetType != null);

            c_mi_AspectContext_getTargetClass = tp.GetProperty("TargetClass").GetGetMethod();
            System.Diagnostics.Debug.Assert(c_mi_AspectContext_getTargetClass != null);

            c_mi_RecallDelegate_Invoke = typeof(Loom.Runtime.RecallDelegate).GetMethod("Invoke");
            System.Diagnostics.Debug.Assert(c_mi_RecallDelegate_Invoke != null);

            c_ci_MissingTargetMethodException_ObjectArray = typeof(Loom.MissingTargetMethodException).GetConstructor(c_param_ObjectArray);
            System.Diagnostics.Debug.Assert(c_ci_MissingTargetMethodException_ObjectArray != null);

            c_ci_InvalidInvocationArgsException = typeof(Loom.InvalidInvocationArgsException).GetConstructor(c_param_);
            System.Diagnostics.Debug.Assert(c_ci_InvalidInvocationArgsException != null);

            c_mi_MethodInfo_GetGenericMethodDefinition = typeof(MethodInfo).GetMethod("GetGenericMethodDefinition");
            System.Diagnostics.Debug.Assert(c_mi_MethodInfo_GetGenericMethodDefinition != null);

            tp = typeof(Loom.Runtime.Common);

            c_mi_Common_AddAspect = tp.GetMethod("AddAspect");
            System.Diagnostics.Debug.Assert(c_mi_Common_AddAspect != null);

            c_mi_Common_CreateDelegate_2 = tp.GetMethod("CreateDelegate", new Type[] { typeof(object), typeof(Type), typeof(MethodInfo), typeof(Type[]), typeof(System.Collections.Generic.Dictionary<Type, Delegate>).MakeByRefType() });
            System.Diagnostics.Debug.Assert(c_mi_Common_CreateDelegate_2 != null);

            c_mi_Common_CreateDelegate_1 = tp.GetMethod("CreateDelegate", new Type[] { typeof(object), typeof(Type), typeof(MethodInfo), typeof(Delegate).MakeByRefType() });
            System.Diagnostics.Debug.Assert(c_mi_Common_CreateDelegate_1 != null);

            c_mi_Common_ReplaceTypedNull = tp.GetMethod("ReplaceTypedNull", c_param_ObjectArray);
            System.Diagnostics.Debug.Assert(c_mi_Common_ReplaceTypedNull != null);

            c_mi_Recall_CreateRecallDelegate = typeof(Loom.Runtime.Recall).GetMethod("CreateRecallDelegate", new Type[] { typeof(Loom.Runtime.RecallDelegate).MakeByRefType(), typeof(Loom.Runtime.MethodContext), typeof(object[]) });
            System.Diagnostics.Debug.Assert(c_mi_Recall_CreateRecallDelegate != null);

            c_mi_TypedNull_IsType = typeof(Loom.TypedValue).GetMethod("IsType", c_param_Type);
            System.Diagnostics.Debug.Assert(c_mi_TypedNull_IsType != null);
        }

    }
}
