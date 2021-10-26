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

using Loom.CodeBuilder.DynamicProxy.Declarations;

namespace Loom.CodeBuilder.DynamicProxy.DataSlots
{
    /// <summary>
    /// Dataslot für ein bereits vorhandenes Feld in der Zielklasse
    /// </summary>
    internal class FieldAccessDataSlot : DataSlot
    {
        FieldInfo fi;
        JPVariableDeclaration decl;

        public FieldAccessDataSlot(FieldInfo fi)
        {
            this.fi = fi;
        }

        public FieldAccessDataSlot(JPVariableDeclaration decl)
        {
            this.decl = decl;
        }

        public override void EmitInitialization(ProxyMemberBuilder meb)
        {
            if (fi == null)
            {
                fi = decl.FieldInfo;
            }
        }

        public override void EmitLoadValue(ProxyMemberBuilder meb, Type targettype)
        {
            ILGenerator ilgen = meb.ILGenerator;
            if (fi.IsStatic)
            {
                ilgen.Emit(OpCodes.Ldsfld, fi);
            }
            else
            {
                ilgen.Emit(OpCodes.Ldarg_0);
                ilgen.Emit(OpCodes.Ldfld, fi);
            }
        }

        public override void EmitLoadAddress(ProxyMemberBuilder meb)
        {
            ILGenerator ilgen = meb.ILGenerator;
            if (fi.IsStatic)
            {
                ilgen.Emit(OpCodes.Ldsflda, fi);
            }
            else
            {
                ilgen.Emit(OpCodes.Ldarg_0);
                ilgen.Emit(OpCodes.Ldflda, fi);
            }
        }

        public override Type DataSlotType
        {
            get { return fi.FieldType; }
        }
    }

    /// <summary>
    /// Dataslot für ein bereits vorhandenes Feld in der Zielklasse für den Zugriff aus dem Klassenobjekt
    /// </summary>
    internal class ExternFieldAccessDataSlot : DataSlot
    {
        FieldInfo fi;
        JPVariableDeclaration decl;

        public ExternFieldAccessDataSlot(FieldInfo fi)
        {
            this.fi = fi;
        }

        public ExternFieldAccessDataSlot(JPVariableDeclaration decl)
        {
            this.decl = decl;
        }

        public override void EmitInitialization(ProxyMemberBuilder meb)
        {
            if (fi == null)
            {
                fi = decl.FieldInfo;
            }
        }

        public override void EmitLoadValue(ProxyMemberBuilder meb, Type targettype)
        {
            ILGenerator ilgen = meb.ILGenerator;
            if (fi.IsStatic)
            {
                ilgen.Emit(OpCodes.Ldsfld, fi);
            }
            else
            {
                meb.DataSlots.RetVal.EmitLoadValue(meb, fi.DeclaringType);
                ilgen.Emit(OpCodes.Ldfld, fi);
            }
        }

        public override void EmitLoadAddress(ProxyMemberBuilder meb)
        {
            ILGenerator ilgen = meb.ILGenerator;
            if (fi.IsStatic)
            {
                ilgen.Emit(OpCodes.Ldsflda, fi);
            }
            else
            {
                meb.DataSlots.RetVal.EmitLoadValue(meb, fi.DeclaringType);
                ilgen.Emit(OpCodes.Ldflda, fi);
            }
        }

        public override Type DataSlotType
        {
            get { return fi.FieldType; }
        }
    }


}
