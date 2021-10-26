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

using Loom.CodeBuilder.DynamicProxy.DataSlots;

namespace Loom.CodeBuilder.DynamicProxy.Parameter
{
    /// <summary>
    /// Use this class if the caller and the callee have the same parameters.
    /// </summary>
    internal class SimpleParameterCodeBrick : ParameterCodeBrick
    {
        protected int source;

        /// <summary>
        /// </summary>
        /// <param name="source">die Position auf dem Argumentenstack für den Parameter</param>
        public SimpleParameterCodeBrick(int source)
        {
            this.source = source;
        }

        /// <summary>
        /// Generates following code
        /// ldarg parameternumber
        /// </summary>
        /// <param name="meb"></param>
        public override void EmitPrepare(ProxyMemberBuilder meb)
        {
            ILGenerator ilgen = ((IILGenerator)meb).ILGenerator;
            CommonCode.EmitLdArg(ilgen, source);
        }
    }

    /// <summary>
    /// Dieser CodeBrick wird verwendet, wenn Source-Parameter konkret und pitarget-Parameter Generisch sind
    /// </summary>
    internal class GenericParameterCodeBrick : SimpleParameterCodeBrick, IGenericParameterCodeBrick
    {
        protected Type targettype;
        protected Type sourcetype;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="source">Die Position auf dem Argumentenstack (dies ist nicht notwendigerweise die Position des Parameters in der Zielmethode, da es bei der Verwebung zur Verschiebung von Parameterpositionen kommen kann.</param>
        /// <param name="pisource">Der Typ, der auf dem Argumentenstack liegt oder liegen würde, also der Typ des Parameters der Zielmethode</param>
        /// <param name="pitarget">Der Typ, der auf den Stack geladen werden soll</param>
        public GenericParameterCodeBrick(int source, ParameterInfo pisource, ParameterInfo pitarget)
            :
            base(source)
        {
            this.sourcetype = GetParameterType(pisource);
            this.targettype = GetParameterType(pitarget);
        }

        private Type GetParameterType(ParameterInfo pi)
        {
            Type t = pi.ParameterType;
            return t.IsByRef ? t.GetElementType() : t;
        }

        public void AddGenericParameter(GenericDictionary generics)
        {
            if (!generics.ContainsKey(targettype))
            {
                generics.Add(targettype, sourcetype);
            }
        }
    }


    /// <summary>
    /// Erzeugt das laden eines Container-Parameters für eine Aspektmethode
    /// Hinter diesem steht ein Container-Initializer, es gibt für jede Array-Länge genau einen Initializer, 
    /// kann aber mehrerere ContainerParameter-Codebricks geben, da dies pro Aspektmethode gelten
    /// </summary>
    internal class ContainerParameterCodeBrick : ParameterCodeBrick
    {
        // Repräsentiert  die lokale Variable, die das Array der Parameter hälz
        private LocalArgArray argarray;
        // Der Containertyp
        private Type container;

        public ContainerParameterCodeBrick(LocalArgArray argarray, Type container)
        {
            this.argarray = argarray;
            this.container = container;
        }

        public override void EmitPrepare(ProxyMemberBuilder meb)
        {
            ProxyMemberBuilder pbm = (ProxyMemberBuilder)meb;
            argarray.EmitLoadValue(pbm, container);
        }

        public override void EmitPostprocessing(ProxyMemberBuilder meb)
        {
            // Referenzparameter zurückschreiben
            argarray.EmitStoreValue((ProxyMemberBuilder)meb);
        }
    }
}
