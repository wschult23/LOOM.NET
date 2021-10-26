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

using Loom;
using Loom.Runtime;
using Loom.AspectModel;
using Loom.JoinPoints.Implementation;
using Loom.CodeBuilder.DynamicProxy;
using Loom.CodeBuilder.Gripper.DynamicProxy.Declarations;
using Loom.CodeBuilder.DynamicProxy.Declarations;

namespace Loom.CodeBuilder.Gripper
{
    internal class GLProxyTypeBuilder : ProxyTypeBuilder
    {
        public GLProxyTypeBuilder(ModuleBuilder targetmodule, Type targetclass)
            :
            base(targetmodule, targetclass, new GLClassTypeBuilder(targetmodule))
        {
        }


        public override bool Interweave()
        {
            ICollection<IJoinPointEnumerator> aspectenum = joinpointcollection.GetAspects();
            if (aspectenum.Count > 0)
            {
                if (targetclass.IsNested)
                {
                    AspectWeaver.WriteWeaverMessage(WeaverMessages.MessageType.Warning, 9010, "Skipping {0}: Nested types are currently not supported.", targetclass.FullName);
                    return false;
                }
                else if (targetclass.IsGenericTypeDefinition)
                {
                    AspectWeaver.WriteWeaverMessage(WeaverMessages.MessageType.Warning,9010, "Skipping {0}: Generic types are currently not supported.",targetclass.FullName);
                    return false;
                }
                else
                {
                    Console.WriteLine(targetclass.FullName);
                }
            }
            return base.Interweave(aspectenum.GetEnumerator());
        }

        public override bool Interweave(Aspect aspect)
        {
            return base.Interweave(aspect);
        }

        public override ContextClassDeclaration CreateIntroduceContextClassDeclaration(ProxyIntroducedMethodBuilder meb)
        {
            return new GLIntroduceContextClassDeclaration(meb);
        }

        public override ContextClassDeclaration CreateCallContextClassDeclaration(ProxyAdviceMethodBuilder meb)
        {
            return new GLCallContextClassDeclaration(meb);
        }
    }
}