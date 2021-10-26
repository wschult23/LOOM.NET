// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Reflection;
using System.Reflection.Emit;

using Loom.Common;
using Loom.CodeBuilder.DynamicProxy.Parameter;
using Loom.CodeBuilder.DynamicProxy.CodeBricks;
using Loom.CodeBuilder.DynamicProxy.DataSlots;
using Loom.Runtime;
using Loom.JoinPoints.Implementation;
using Loom.CodeBuilder.DynamicProxy.Declarations;

namespace Loom.CodeBuilder.DynamicProxy
{
    /// <summary>
    /// Hier werden die unverwobenen Konstruktoraufrufe erzeugt
    /// Repräsentiert ein CreateAI/CreateTo Methodenpaar im ClassObject
    /// </summary>
    internal class ProxyNoAdviceConstructorBuilder : ProxyAdviceMemberBuilder
    {
        protected MethodBuilder micreatetomethod;
        protected ProxyConstructorBuilder constructorbuilder;
        protected Type[] paramtypes;

        public ProxyNoAdviceConstructorBuilder(ProxyTypeActivatorBuilder iwtbuilder, ProxyConstructorBuilder pcb, int aspectindex)
            :
            base(iwtbuilder, pcb.JoinPoint)
        {
            this.constructorbuilder = pcb;
            this.paramtypes = ClassObjectImpl.GetCreateMethodArgTypes(JoinPoint.ConstructorImplementation);
            if (aspectindex >= 0)
            {
                DataSlots.Aspect = new AspectArgument(iwtbuilder.AspectFields[aspectindex].ArrayPosition, iwtbuilder.AspectType);
            }
        }

        protected ProxyNoAdviceConstructorBuilder(ProxyTypeActivatorBuilder iwtbuilder, MethodBaseJoinPoint joinpoint, int aspectindex)
            :
            base(iwtbuilder, joinpoint)
        {
            this.constructorbuilder = null;
            this.paramtypes = ClassObjectImpl.GetCreateMethodArgTypes(JoinPoint.ConstructorImplementation);
            if (aspectindex >= 0)
            {
                DataSlots.Aspect = new AspectArgument(iwtbuilder.AspectFields[aspectindex].ArrayPosition, iwtbuilder.AspectType);
            }
        }

        public new ProxyConstructorJoinPoint JoinPoint
        {
            get
            {
                return (ProxyConstructorJoinPoint)joinpoint;
            }
        }

        public override MethodBaseJoinPoint BindJoinPointToType()
        {
            return ((IBindConstructor)joinpoint).BindToType(constructorbuilder.Implementation, JoinPoint.CreateAIMethod, micreatetomethod);
        }

        public new ProxyTypeActivatorBuilder DeclaringBuilder
        {
            get
            {
                return (ProxyTypeActivatorBuilder)base.DeclaringBuilder;
            }
        }

        public override CodeBrickBuilder DefineAspectCall(MethodInfo aspectmethod, Loom.JoinPoints.Advice advice)
        {
            throw new InvalidOperationException();
        }


        public override void Emit()
        {
            // den Konstruktor in der Zielklasse erzeugen
            constructorbuilder.Emit();
            // Wenn das ClassObject abstrakt ist, wird keine CreateTO implementierung benötigt
            if (!constructorbuilder.IsAbstract)
            {
                micreatetomethod = ClassObjectImpl.Impl__CreateTO(TypeBuilder, constructorbuilder.Implementation, paramtypes, true, false);
            }
        }

        protected override ContextClassDeclaration GetContextClassDeclaration()
        {
            System.Diagnostics.Debug.Assert(false);
            return null;
        }

    }

    /// <summary>
    /// Hier werden die Konstruktoraufrufe des Klassenobjektes erzeugt, die verwoben werden kännen
    /// </summary>
    internal class ProxyAdviceConstructorBuilder : ProxyNoAdviceConstructorBuilder
    {
        protected MethodBuilder mbcreateaimethod;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iwtbuilder"></param>
        /// <param name="pcb"></param>
        /// <param name="aspectindex"></param>
        public ProxyAdviceConstructorBuilder(ProxyTypeActivatorBuilder iwtbuilder, ProxyConstructorBuilder pcb, int aspectindex) :
            base(iwtbuilder, pcb, aspectindex)
        {
        }

        protected ProxyAdviceConstructorBuilder(ProxyTypeActivatorBuilder iwtbuilder, MethodBaseJoinPoint joinpoint, int aspectindex)
            :
            base(iwtbuilder, joinpoint, aspectindex)
        {
            this.constructorbuilder = null;
        }

        public new ProxyConstructorJoinPoint JoinPoint
        {
            get
            {
                return (ProxyConstructorJoinPoint)joinpoint;
            }
        }

        public override MethodBaseJoinPoint BindJoinPointToType()
        {
            return ((IBindConstructor)joinpoint).BindToType(constructorbuilder.Implementation, mbcreateaimethod, micreatetomethod);
        }

        public override CodeBrickBuilder DefineAspectCall(MethodInfo aspectmethod, Loom.JoinPoints.Advice advice)
        {
            return new ProxyAdviceConstructorCodeBrickBuilder(this, aspectmethod, advice);
        }

        public override void Emit()
        {
            base.Emit();

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

        protected override ContextClassDeclaration GetContextClassDeclaration()
        {
            return new CreateContextClassDeclaration(this);
        }

    }

}
