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
using Loom.CodeBuilder.DynamicProxy.CodeBricks;

namespace Loom.CodeBuilder.DynamicProxy
{
    /// <summary>
    /// Baut Klassenobjektimplementationen für sealed classes
    /// </summary>
    internal class ProxySealedNoAdviceConstructorBuilder : ProxyNoAdviceConstructorBuilder
    {
        public ProxySealedNoAdviceConstructorBuilder(ProxyTypeActivatorBuilder iwtbuilder, MethodBaseJoinPoint jp) :
            base(iwtbuilder, jp, -1)
        {
        }

        public override void Emit()
        {
            micreatetomethod = ClassObjectImpl.Impl__CreateTO(TypeBuilder, JoinPoint.ConstructorImplementation, paramtypes, true, true);
        }
    }

    /// <summary>
    /// Baut Klassenobjektimplementation Create Advices für sealed classes
    /// </summary>
    internal class ProxySealedAdviceConstructorBuilder : ProxyAdviceConstructorBuilder
    {
        public ProxySealedAdviceConstructorBuilder(ProxyTypeActivatorBuilder iwtbuilder, MethodBaseJoinPoint jp, int aspectindex)
            :
            base(iwtbuilder, jp, aspectindex)
        {
        }

        public override void Emit()
        {
            micreatetomethod = ClassObjectImpl.Impl__CreateTO(TypeBuilder, JoinPoint.ConstructorImplementation, paramtypes, true, true);

            if (methodcode == null)
            {
                methodcode = BaseConstructorCall.GetObject();
            }

            TypeBuilder tb = DeclaringBuilder.TypeBuilder;

            // BuildMethod
            mbcreateaimethod = tb.DefineMethod(
                JoinPoint.CreateAIMethod.Name,
                MethodAttributes.FamANDAssem | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                typeof(object),
                paramtypes);

            EmitMethod(mbcreateaimethod);
        }

    }
}
