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
using Loom.Runtime;
using Loom.JoinPoints;
using Loom.CodeBuilder.DynamicProxy.CodeBricks;
using Loom.CodeBuilder.DynamicProxy.Declarations;

namespace Loom.CodeBuilder.DynamicProxy.DataSlots
{
    internal abstract class ContextDataSlot : DataSlot
    {
        protected ContextClassDeclaration declcontext;

        public override Type DataSlotType
        {
            get
            {
                return declcontext.ConstructorInfo.DeclaringType;
            }
        }
    }

    /// <summary>
    /// Context mit Instanz als einzigen Konstruktorparameter als lokale Variable
    /// </summary>
    internal class LocalContextVariable : ContextDataSlot
    {
        protected LocalBuilder context;

        public LocalContextVariable(ContextClassDeclaration decl)
        {
            this.declcontext = decl;
        }

        public override void EmitLoadValue(ProxyMemberBuilder meb, Type targettype)
        {
            meb.ILGenerator.Emit(OpCodes.Ldloc, context);
        }

        /// <summary>
        /// Eine neue Contextklasse generieren und abspeichern
        /// </summary>
        /// <param name="meb"></param>
        public override void EmitInitialization(ProxyMemberBuilder meb)
        {
            ILGenerator ilgen = meb.ILGenerator;
            context = ilgen.DeclareLocal(typeof(Context));

            ilgen.Emit(OpCodes.Ldarg_0);
            ilgen.Emit(OpCodes.Newobj, declcontext.ConstructorInfo);
            ilgen.Emit(OpCodes.Stloc, context);
        }

    }

    /// <summary>
    /// Create-Kontext als lokale Variable (1 zusätzlicher Parameter für die Aspekte)
    /// </summary>
    internal class LocalCreateContextVariable : LocalContextVariable
    {
        public LocalCreateContextVariable(CreateContextClassDeclaration decl) :
            base(decl)
        {
        }

        /// <summary>
        /// Eine neue Contextklasse generieren und abspeichern
        /// </summary>
        /// <param name="meb"></param>
        public override void EmitInitialization(ProxyMemberBuilder meb)
        {
            ILGenerator ilgen = meb.ILGenerator;
            context = ilgen.DeclareLocal(typeof(Context));

            ilgen.Emit(OpCodes.Ldarg_0);
            ilgen.Emit(OpCodes.Ldarg_1);
            ilgen.Emit(OpCodes.Newobj, declcontext.ConstructorInfo);
            ilgen.Emit(OpCodes.Stloc, context);
        }
    }

    /// <summary>
    /// Kontext auf dem Evaluationsstack Stack
    /// </summary>
    internal class StackContextValue : ContextDataSlot
    {
        public StackContextValue(ContextClassDeclaration decl)
        {
            this.declcontext = decl;
        }

        /// <summary>
        /// Eine neue Contextklasse generieren und abspeichern
        /// </summary>
        /// <param name="meb"></param>
        /// <param name="targettype"></param>
        public override void EmitLoadValue(ProxyMemberBuilder meb, Type targettype)
        {
            ILGenerator ilgen = meb.ILGenerator;
            ilgen.Emit(OpCodes.Ldarg_0);
            ilgen.Emit(OpCodes.Newobj, declcontext.ConstructorInfo);
        }
    }
}
