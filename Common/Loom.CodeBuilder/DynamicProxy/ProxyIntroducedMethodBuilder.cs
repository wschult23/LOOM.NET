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
using Loom.Runtime;
using Loom.CodeBuilder.DynamicProxy.CodeBricks;
using Loom.CodeBuilder.DynamicProxy.DataSlots;
using Loom.JoinPoints.Implementation;
using Loom.CodeBuilder.DynamicProxy.Declarations;

namespace Loom.CodeBuilder.DynamicProxy
{

    /// <summary>
    /// Baut eine introduction
    /// </summary>
    internal class ProxyIntroducedMethodBuilder : ProxyMemberBuilder
    {
        /// <summary>
        /// code bricks
        /// </summary>
        private CodeBrick methodcode;
        /// <summary>
        /// Die Implementation der Interfacemethode
        /// </summary>
        private MethodBuilder mbimplementation;
        /// <summary>
        /// Die Implementation der Advices
        /// </summary>
        private MethodBuilder mbbasecallimplementation;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tb">Der InterwovenTypeBuilder</param>
        /// <param name="jp"></param>
        /// <param name="aspectindex"></param>
        public ProxyIntroducedMethodBuilder(ProxyTypeBuilderBase tb, JoinPoint jp, int aspectindex) :
            base(tb, jp)
        {
            if (aspectindex >= 0)
            {
                this.DataSlots.Aspect = new AspectSlot(tb.Declarations.Aspects[aspectindex]);
            }
        }

        public new MethodInfoJoinPoint JoinPoint
        {
            get
            {
                return (MethodInfoJoinPoint)joinpoint;
            }
        }

        public override MethodBaseJoinPoint BindJoinPointToType()
        {
            return ((IBindMethod)joinpoint).BindToType(mbimplementation, mbbasecallimplementation);
        }

        #region IDefineMethodCode
        public void DefineMethodCode(CodeBrick cb)
        {
            if (methodcode != null) throw new AspectWeaverException(1004, Loom.Resources.CodeBuilderErrors.ERR_1004, DeclaringBuilder.AspectType.FullName, JoinPoint.ToString());
            methodcode = cb;
        }
        #endregion


        public override CodeBrickBuilder DefineAspectCall(MethodInfo aspectmethod, Loom.JoinPoints.Advice advice)
        {
            return new ProxyIntroducedMethodCodeBrickBuilder(this, aspectmethod);
        }


        public override void Emit()
        {
            if (methodcode == null) throw new AspectWeaverException(1005, Loom.Resources.CodeBuilderErrors.ERR_1004, DeclaringBuilder.AspectType.FullName, JoinPoint.ToString());

            MethodInfo methodinterface = JoinPoint.MethodInterface;
            Type[] parametertypes = Conversion.ToTypeArray(methodinterface.GetParameters());

            mbbasecallimplementation = DefineInterfaceImplementationMethod(TypeBuilder, methodinterface, methodinterface.ReturnType, parametertypes);
            mbimplementation = DefineAndEmitInterfaceMethod(TypeBuilder, methodinterface, methodinterface.ReturnType, parametertypes, mbbasecallimplementation);

            this.ilgen = mbbasecallimplementation.GetILGenerator();

            // Initialisierungscode rendern
            foreach (DataSlot lv in DataSlots)
            {
                lv.EmitInitialization(this);
            }

            CommonCode.EmitAdjustReturnValue(ilgen, methodcode.Emit(this), methodinterface.ReturnType);

            ilgen.Emit(OpCodes.Ret);

            this.ilgen = null;
        }


        protected override ContextClassDeclaration GetContextClassDeclaration()
        {
            return this.DeclaringBuilder.CreateIntroduceContextClassDeclaration(this);
        }
    }
}
