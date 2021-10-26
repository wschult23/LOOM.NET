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
using System.Reflection.Emit;
using Loom.Common;
using System.Reflection;
using Loom.CodeBuilder.DynamicProxy.CodeBricks;

namespace Loom.CodeBuilder.DynamicProxy.Declarations
{
    class GlobalAspectContainer
    {
        static int c_container = 0;

        private ILGenerator ilgen;
        private TypeBuilder typebuilder;
        private int objno;


        public GlobalAspectContainer(ModuleBuilder module)
        {
            this.typebuilder = module.DefineType(WeavingCodeNames.c_aspectFieldContainerPrefix + (c_container++), TypeAttributes.Public | TypeAttributes.Sealed, typeof(object));
            var cctor = this.typebuilder.DefineConstructor(MethodAttributes.Static, CallingConventions.Standard, Common.ReflectionObjects.c_param_);
            this.ilgen = cctor.GetILGenerator();
        }

        public FieldInfo DefineField(IAspectInstantiationCodeBrick cb)
        {
            var fi=typebuilder.DefineField("_obj"+(objno++), typeof(Aspect), FieldAttributes.Static | FieldAttributes.Public);
            cb.Emit(ilgen);
            ilgen.Emit(OpCodes.Stsfld, fi);
            return fi;
        }
        
        public void CreateType()
        {
            ilgen.Emit(OpCodes.Ret);
            typebuilder.CreateType();
        }
    }
}
