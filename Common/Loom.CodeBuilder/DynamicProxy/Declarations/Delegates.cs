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

namespace Loom.CodeBuilder.DynamicProxy.Declarations
{
    internal class DelegateInfo : Declaration
    {
        /// <summary>
        /// Der Typ des Delegates
        /// </summary>
        public Type DelegateType;
        /// <summary>
        /// Die Invokemethode
        /// </summary>
        public MethodInfo Invoke;
        /// <summary>
        /// Der Delegateslot
        /// </summary>
        public FieldInfo Field;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="delegatetype"></param>
        /// <param name="invoke"></param>
        /// <param name="fidelegate"></param>
        public DelegateInfo(Type delegatetype, MethodInfo invoke, FieldInfo fidelegate)
        {
            this.DelegateType = delegatetype;
            this.Invoke = invoke;
            this.Field = fidelegate;
        }

        public DelegateInfo MakeGenericDelegate(params Type[] generic)
        {
            Type type = DelegateType.MakeGenericType(generic);
            return new DelegateInfo(type, TypeBuilder.GetMethod(type, Invoke), Field);
        }

        public override void CreateType()
        {
            ((TypeBuilder)DelegateType).CreateType();
        }

        public override void DefineMember(ProxyTypeBuilderBase tb)
        {
        }
    }


}
