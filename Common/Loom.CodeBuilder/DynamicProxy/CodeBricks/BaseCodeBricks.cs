// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Reflection;
using System.Reflection.Emit;

using Loom.JoinPoints.Implementation;
using Loom.CodeBuilder.DynamicProxy.Declarations;

namespace Loom.CodeBuilder.DynamicProxy.CodeBricks
{

    internal abstract class SingletonCodeBrick<T> : CodeBrick where T : new()
    {
        private static T c_obj = new T();
        public static T GetObject()
        {
            return c_obj;
        }

    }

    /// <summary>
    /// Erzeugt IL-Code für den Aufruf der Basismethode.
    /// Wenn Basismethode privat, erfolgt der Aufruf über ein deelegate
    /// <code>
    ///		ldarg.0 // instance pointer
    ///		ldarg.1 // argument 1
    ///		ldarg.2 // argument 2
    ///		// further method arguments
    ///		call basemethod()
    /// </code>
    /// </summary>
    internal class BaseMethodCall : SingletonCodeBrick<BaseMethodCall>
    {
        public override Type Emit(ProxyMemberBuilder wmeb)
        {
            ILGenerator ilgen = wmeb.ILGenerator;

            ParameterInfo[] param = wmeb.JoinPoint.Method.GetParameters();

            IBaseCall ibc = wmeb.JoinPoint as IBaseCall;
            MethodInfo mi = ibc.BaseCall;

            if (mi.IsPrivate)
            {
                Type[] generics = null;
                if (mi.IsGenericMethod)
                {
                    generics = mi.GetGenericArguments();
                }

                ProxyMemberBuilder pmeb = (ProxyMemberBuilder)wmeb;
                DelegateInfo di = DelegateCallImpl.DefineDelegate(pmeb, mi, param, generics);
                if (generics != null) di = di.MakeGenericDelegate(generics);
                DelegateCallImpl.EmitLdDelegate(ilgen, wmeb.TypeBuilder, null, di, mi, generics);
                mi = di.Invoke;
            }
            else
            {
                ilgen.Emit(OpCodes.Ldarg_0);
            }
            // Parameter auf den Stack
            for (int i = 0; i < param.Length; i++)
            {
                CommonCode.EmitLdArg(ilgen, i + 1);
            }
            ilgen.Emit(OpCodes.Call, mi);

            return mi.ReturnType;
        }
    }

    internal class BaseConstructorCall : SingletonCodeBrick<BaseConstructorCall>
    {
        public override Type Emit(ProxyMemberBuilder wmeb)
        {
            ProxyConstructorJoinPoint jp = (ProxyConstructorJoinPoint)wmeb.JoinPoint;
            ParameterInfo[] param = jp.ConstructorImplementation.GetParameters();

            // Parameter auf den Stack (this + Konstruktorparameter+ Aspecktarray)
            for (int i = 0; i <= param.Length + 1; i++)
            {
                CommonCode.EmitLdArg(wmeb.ILGenerator, i);
            }
            wmeb.ILGenerator.Emit(OpCodes.Call, ((IBaseCall)wmeb.JoinPoint).BaseCall);

            return jp.DeclaringType;
        }
    }
}
