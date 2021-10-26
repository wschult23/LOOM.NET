// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection.Emit;

using Loom.CodeBuilder.DynamicProxy;
using Loom;
using Loom.Runtime;
using Loom.JoinPoints.Implementation;
using Loom.AspectModel;
using Loom.Runtime.Serialization;
using RapierLoom.CodeBuilder.DynamicProxy;
using Loom.CodeBuilder.DynamicProxy.Declarations;
using Loom.CodeBuilder.Gripper.DynamicProxy.Declarations;

namespace Loom.CodeBuilder.Rapier
{
    internal class RLProxyTypeBuilder : ProxyTypeBuilder
    {

        public RLProxyTypeBuilder(ModuleBuilder targetmodule, ClassObject parentclassobject)
            :
            base(targetmodule, parentclassobject, new ProxyTypeActivatorBuilder(targetmodule, parentclassobject))
        {
        }

        public RLProxyTypeBuilder(ModuleBuilder targetmodule, Type targetclass)
            :
            base(targetmodule, targetclass, new ProxyTypeActivatorBuilder(targetmodule))
        {
        }

        public override bool Interweave()
        {
            ICollection<IJoinPointEnumerator> aspectenum = joinpointcollection.GetAspects();
            if (targetclass.IsSerializable)
            {
                IJoinPointEnumerator seria = new JoinPointCollectionEnumerator(new SerializationAspectCoverageInfo());
                aspectenum.Add(seria);
            }
            return base.Interweave(aspectenum.GetEnumerator());
        }

        public override bool Interweave(Aspect aspect)
        {
            return base.Interweave(aspect);
        }

        public override ContextClassDeclaration CreateIntroduceContextClassDeclaration(ProxyIntroducedMethodBuilder meb)
        {
            return new RLIntroduceContextClassDeclaration(meb);
        }

        public override ContextClassDeclaration CreateCallContextClassDeclaration(ProxyAdviceMethodBuilder meb)
        {
            return new RLCallContextClassDeclaration(meb);
        }
    }
}
