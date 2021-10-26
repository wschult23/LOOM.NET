// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Loom.CodeBuilder.DynamicProxy;
using Loom.JoinPoints.Implementation;
using Loom.Runtime.Serialization;
using System.Reflection.Emit;
using System.Reflection;
using Loom.Common;
using Loom.CodeBuilder.DynamicProxy.Declarations;
using Loom.CodeBuilder.DynamicProxy.CodeBricks;

namespace RapierLoom.CodeBuilder.DynamicProxy
{
    internal class SerializationAspectCoverageInfo : ProxyAspectCoverageInfo, IBindToAspectFieldDeclaration, IAspectInstantiationCodeBrick
    {
        static FieldInfo c_ciSerializationAspect = typeof(SerializationAspect).GetField("c_BaseObject",BindingFlags.Static|BindingFlags.Public);

        public SerializationAspectCoverageInfo():
            base(JoinPointCollection.GetAspectClass(typeof(SerializationAspect)))
        {
        }

        public override AspectField CreateAspectField()
        {
            return new ContainerAspectField(this, this);
        }

        public override void CheckEmptyConstructor()
        {
        }

        public void Emit(ILGenerator ilgen)
        {
            ilgen.Emit(OpCodes.Ldsfld, c_ciSerializationAspect);
        }
    }
}
