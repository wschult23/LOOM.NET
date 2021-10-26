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
using Loom.CodeBuilder.DynamicProxy.CodeBricks;
using Loom.CodeBuilder.DynamicProxy.Declarations;

namespace Loom.CodeBuilder.Gripper
{
    internal class GLClassTypeBuilder : ProxyTypeActivatorBuilder
    {
        ConstructorJoinPoint[] joinpoints;

        public GLClassTypeBuilder(ModuleBuilder targetmodule)
            :
            base(targetmodule)
        {
        }

        public override void DefineType(string typename, bool abstractimpl, ICollection<AspectField> aspects)
        {
            if (parentClassObjectType == null)
            {
                TypeBuilder tb = ClassObjectImpl.DefineClassObject(TargetModule, Common.WeavingCodeNames.GetClassObjectName(BaseType), TargetClass, proxytypebuilder.CurrentJoinPoints.CtorJoinPoints, true);
                parentClassObjectType = tb.CreateType();
                joinpoints = proxytypebuilder.CurrentJoinPoints.CtorJoinPoints;
            }

            this.typebuilder = ClassObjectImpl.DefineClassObject(TargetModule, typename, parentClassObjectType, proxytypebuilder.TypeBuilder, abstractimpl);

            foreach (Declaration ds in Declarations)
            {
                ds.DefineMember(this);
            }

            // GetAspects implementieren
            if (aspects != null)
            {
                ClassObjectImpl.Impl_GetAspects(this.typebuilder, aspects);
                FieldInfo fiobj = ClassObjectImpl.Imp_StaticCtor(this.typebuilder);
                foreach (ProxyConstructorJoinPoint jp in joinpoints)
                {
                    Type[] paramtypes = Common.Conversion.ToTypeArray(jp.Method.GetParameters());
                    ClassObjectImpl.Impl_Create(this.typebuilder, fiobj, jp.CreateAIMethod, TargetClass, paramtypes);
                }
            }

        }

    }
}
