// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using Loom.Common;
using Loom.JoinPoints.Implementation;
using Loom.CodeBuilder.DynamicProxy.CodeBricks;


namespace Loom.CodeBuilder.DynamicProxy.Declarations
{
    /// <summary>
    /// Repräsentiert den Member, in dem der Aspekt gespeichert wird
    /// </summary>
    internal abstract class AspectField : Declaration
    {
        protected FieldInfo aspectfieldinfo;

        /// <summary>
        /// Die Position des Apektes im beim Konstruktor übergebenen AspektArray
        /// </summary>
        public int ArrayPosition;

        /// <summary>
        /// Der Konstruktor
        /// </summary>
        /// <param name="arraypos">Die Position des Apektes im beim Konstruktor übergebenen AspektArray</param>
        public AspectField()
        {
        }

        /// <summary>
        ///  Das Aspektfeld
        /// </summary>
        public FieldInfo FieldInfo
        {
            get { return aspectfieldinfo; }
        }

        public override void DefineMember(ProxyTypeBuilderBase tb)
        {
            aspectfieldinfo = tb.TypeBuilder.DefineField(WeavingCodeNames.c_aspectFieldName + ArrayPosition.ToString(), tb.AspectType, FieldAttributes.Public);
        }

        /// <summary>
        /// Ist das Feld Statisch?
        /// </summary>
        public virtual bool IsStatic 
        { 
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Ist der referenzierte Aspekt statisch?
        /// </summary>
        public abstract bool IsReferencedAspectStatic { get; }

        /// <summary>
        /// Dieser Code wird im ClassObject eingefügt, um die Aspektinstanz zu erzeugen
        /// </summary>
        /// <param name="ilgen"></param>
        public abstract void EmitInitialize(ILGenerator ilgen);

    }

    internal class DynamicAspectField : AspectField
    {
        public DynamicAspectField(int arrayPosition)
        {
            this.ArrayPosition = arrayPosition;
        }

        public override void EmitInitialize(ILGenerator ilgen)
        {
            throw new NotImplementedException();
        }

        public override bool IsReferencedAspectStatic
        {
            get { return false; }
        }

        public override void CreateType()
        {
        }
    }

    internal class InstanceAspectField:AspectField
    {
        protected IAspectInstantiationCodeBrick caaci;

        public InstanceAspectField(IAspectInstantiationCodeBrick caaci)
        {
            this.caaci=caaci;
        }

        public override void EmitInitialize(ILGenerator ilgen)
        {
            caaci.Emit(ilgen);
        }

        public override void CreateType()
        {
        }

        public override bool IsReferencedAspectStatic
        {
            get { return false; }
        }

    }

    internal class ContainerAspectField : InstanceAspectField
    {
        protected IBindToAspectFieldDeclaration aspectfield;

        public ContainerAspectField(IAspectInstantiationCodeBrick cad, IBindToAspectFieldDeclaration aspectfield)
            : base(cad)
        {
            this.aspectfield = aspectfield;
        }

        public override void DefineMember(ProxyTypeBuilderBase tb)
        {
            base.DefineMember(tb);
            if (aspectfield.AspectFieldInfo == null)
            {
                var fi=tb.GlobalAspectContainer.DefineField(this.caaci);
                aspectfield.Bind(fi);
            }
        }

        public override void EmitInitialize(ILGenerator ilgen)
        {
            ilgen.Emit(OpCodes.Ldsfld, aspectfield.AspectFieldInfo);
        }

        public override bool IsReferencedAspectStatic
        {
            get { return true; }
        }
    }

   
    internal class StaticAspectField : InstanceAspectField
    {
        public StaticAspectField(IAspectInstantiationCodeBrick cad)
            : base(cad)
        {
        }

        public override void DefineMember(ProxyTypeBuilderBase tb)
        {
            aspectfieldinfo = tb.TypeBuilder.DefineField(WeavingCodeNames.c_aspectFieldName + ArrayPosition.ToString(), tb.AspectType, FieldAttributes.Public | FieldAttributes.Static);
        }

        public override bool  IsStatic
        {
	        get { return true; }
        }

        public override bool NeedStaticInitialize
        {
            get
            {
                return true;
            }
        }

        public override void EmitStaticInitialization(ILGenerator ilgen)
        {
            caaci.Emit(ilgen);
            ilgen.Emit(OpCodes.Stsfld, aspectfieldinfo);
        }

        public override bool IsReferencedAspectStatic
        {
            get { return true; }
        }

        public override void EmitInitialize(ILGenerator ilgen)
        {
            ilgen.Emit(OpCodes.Ldsfld, aspectfieldinfo);
        }

       

        public override void CreateType()
        {
        }
    }

}
