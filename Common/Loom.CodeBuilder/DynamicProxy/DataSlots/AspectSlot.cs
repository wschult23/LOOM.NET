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

using Loom.Common;
using Loom.CodeBuilder.DynamicProxy.Declarations;

namespace Loom.CodeBuilder.DynamicProxy.DataSlots
{
    /// <summary>
    /// Representiert die Aspektinstanz im Proxy
    /// </summary>
    internal class AspectSlot : DataSlot
    {
        AspectField aspectfield;

        public AspectSlot(AspectField aspectfield)
        {
            this.aspectfield = aspectfield;
        }

        public override void EmitLoadValue(ProxyMemberBuilder meb, Type targettype)
        {
            ILGenerator ilgen = meb.ILGenerator;
            if (aspectfield.IsStatic)
            {
                ilgen.Emit(OpCodes.Ldsfld, aspectfield.FieldInfo);
            }
            else
            {
                ilgen.Emit(OpCodes.Ldarg_0);
                ilgen.Emit(OpCodes.Ldfld, aspectfield.FieldInfo);
            }
        }

        public override Type DataSlotType
        {
            get
            {
                return aspectfield.FieldInfo.FieldType;
            }
        }
    }

    /// <summary>
    /// Repräsentiert die Aspektinstanz im Initialisierungsvektor der CreateAI Methode
    /// </summary>
    internal class AspectArgument : DataSlot
    {
        int aspectpos;
        Type aspecttype;

        public AspectArgument(int aspectpos, Type aspecttype)
        {
            this.aspectpos = aspectpos;
            this.aspecttype = aspecttype;
        }

        public override void EmitLoadValue(ProxyMemberBuilder meb, Type targettype)
        {
            ILGenerator ilgen = meb.ILGenerator;
            ilgen.Emit(OpCodes.Ldarg_1);
            CommonCode.EmitLdci4(ilgen, aspectpos);
            ilgen.Emit(OpCodes.Ldelem_Ref);
            ilgen.Emit(OpCodes.Castclass, aspecttype);
        }

        public override Type DataSlotType
        {
            get
            {
                return aspecttype;
            }
        }
    }
}
