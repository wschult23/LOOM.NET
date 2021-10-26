// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization; // needed for defining exception .ctors
using System.Linq;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using System.Collections;
using Loom.CodeBuilder.Gripper;

namespace Loom.Compiler
{
         /// <summary>
        /// A mutator that modifies method bodies at the IL level.
        /// It injects a call to Console.WriteLine for each store
        /// to a local for which the PDB reader is able to provide a name.
        /// This is meant to distinguish programmer-defined locals from
        /// those introduced by the compiler.
        /// </summary>
        public class ILMutator : MetadataRewriter
        {

            PdbReader/*?*/ pdbReader = null;
            SymbolDictionary sd;

            public ILMutator(SymbolDictionary sd, IMetadataHost host, PdbReader/*?*/ pdbReader)
                : base(host)
            {
                this.pdbReader = pdbReader;
                this.sd = sd;
            }

            private void Interweave(ILGenerator generator, IMethodReference ctor)
            {
                var type=ctor.ContainingType;
                var typename = type.ToString();


                var generic = type as GenericTypeInstanceReference;
                if (generic != null)
                {
                    generator.Emit(OperationCode.Newobj, this.Rewrite(ctor));
                    return;
                }

                var loc = type.Locations.FirstOrDefault();
                if (loc==null)
                {
                    type = type.ResolvedType;
                    loc = type.Locations.FirstOrDefault();
                }

                var assemblyname = loc == null ? null : loc.Document.Name.Value;

                if (assemblyname == null)
                {
                    throw new AspectWeaverException(9200, Properties.Resources.ERR_9200, typename);
                }


                if (sd.NeedInstrumentation(assemblyname, typename))
                {
                    var refmodule = (IModule)sd.GetWeavingModule(assemblyname);
                    typename +=  Loom.Common.WeavingCodeNames.c_classObjectExtension;
                    var surrogate=refmodule.GetAllTypes().Where(tp=>tp.ToString()==typename).FirstOrDefault();

                    ITypeDefinitionMember createmethod=null;
                    foreach(IMethodDefinition candidate in surrogate.GetMatchingMembers(member=> member.Name.Value=="Create"))
                    {
                        createmethod = candidate;
                        if (ctor.ParameterCount == candidate.ParameterCount)
                        {
                            for (IEnumerator<IParameterTypeInformation> first = ctor.Parameters.GetEnumerator(), second = candidate.Parameters.GetEnumerator(); first.MoveNext() && second.MoveNext(); )
                            {
                                if (first.Current.Type.ToString() != second.Current.Type.ToString())
                                {
                                    createmethod = null;
                                }
                            }
                            if (createmethod != null)
                            {
                                break;
                            }
                        }
                        else
                        {
                            createmethod = null;
                        }
                    }
                    System.Diagnostics.Debug.Assert(createmethod != null);

                    generator.Emit(OperationCode.Call, createmethod);
                }
                else
                {
                    generator.Emit(OperationCode.Newobj, this.Rewrite(ctor));
                }
            }

            public override List<IAssemblyReference> Rewrite(List<IAssemblyReference> assemblyReferences)
            {
                var newassemblyReferences = base.Rewrite(assemblyReferences.ToList());
                foreach (var assref in assemblyReferences)
                {
                    var weavingmodule = (IModule)sd.GetWeavingModule(assref.Name.Value);
                    if (weavingmodule != null)
                    {
                        newassemblyReferences.Add(weavingmodule.ContainingAssembly);
                    }
                }
                return newassemblyReferences;
            }

            public override List<ITypeReference> Rewrite(List<ITypeReference> typeReferences)
            {
                typeReferences = base.Rewrite(typeReferences);
              
                return typeReferences;
            }

            List<ILocalDefinition> currentLocals;
            ILGenerator currentGenerator;
            IEnumerator<ILocalScope>/*?*/ scopeEnumerator;
            bool scopeEnumeratorIsValid;
            Stack<ILocalScope> scopeStack = new Stack<ILocalScope>();

            public override IMethodBody Rewrite(IMethodBody methodBody)
            {
                this.currentLocals = new List<ILocalDefinition>(methodBody.LocalVariables);

                try
                {
                    ProcessOperations(methodBody);
                    var newBody = new ILGeneratorMethodBody(this.currentGenerator, methodBody.LocalsAreZeroed, (ushort)(methodBody.MaxStack + 1),
                      methodBody.MethodDefinition, this.currentLocals ?? new List<ILocalDefinition>(), Enumerable<ITypeDefinition>.Empty);
                    return newBody;
                }
                catch (ILMutatorException)
                {
                    Console.WriteLine("Internal error during IL mutation for the method '{0}'.",
                      MemberHelper.GetMemberSignature(methodBody.MethodDefinition, NameFormattingOptions.SmartTypeName));
                    return methodBody;
                }
                finally
                {
                    this.currentLocals = null;
                }
            }

            private void ProcessOperations(IMethodBody methodBody)
            {

                List<IOperation> operations = ((methodBody.Operations == null) ? new List<IOperation>() : new List<IOperation>(methodBody.Operations));
                int count = operations.Count;

                ILGenerator generator = new ILGenerator(this.host, methodBody.MethodDefinition);
                if (this.pdbReader != null)
                {
                    foreach (var ns in this.pdbReader.GetNamespaceScopes(methodBody))
                    {
                        foreach (var uns in ns.UsedNamespaces)
                            generator.UseNamespace(uns.NamespaceName.Value);
                    }
                }

                this.currentGenerator = generator;
                this.scopeEnumerator = this.pdbReader == null ? null : this.pdbReader.GetLocalScopes(methodBody).GetEnumerator();
                this.scopeEnumeratorIsValid = this.scopeEnumerator != null && this.scopeEnumerator.MoveNext();

                var methodName = MemberHelper.GetMemberSignature(methodBody.MethodDefinition, NameFormattingOptions.SmartTypeName);

                #region Record all offsets that appear as part of an exception handler
                Dictionary<uint, bool> offsetsUsedInExceptionInformation = new Dictionary<uint, bool>();
                foreach (var exceptionInfo in methodBody.OperationExceptionInformation ?? Enumerable<IOperationExceptionInformation>.Empty)
                {
                    uint x = exceptionInfo.TryStartOffset;
                    if (!offsetsUsedInExceptionInformation.ContainsKey(x)) offsetsUsedInExceptionInformation.Add(x, true);
                    x = exceptionInfo.TryEndOffset;
                    if (!offsetsUsedInExceptionInformation.ContainsKey(x)) offsetsUsedInExceptionInformation.Add(x, true);
                    x = exceptionInfo.HandlerStartOffset;
                    if (!offsetsUsedInExceptionInformation.ContainsKey(x)) offsetsUsedInExceptionInformation.Add(x, true);
                    x = exceptionInfo.HandlerEndOffset;
                    if (!offsetsUsedInExceptionInformation.ContainsKey(x)) offsetsUsedInExceptionInformation.Add(x, true);
                    if (exceptionInfo.HandlerKind == HandlerKind.Filter)
                    {
                        x = exceptionInfo.FilterDecisionStartOffset;
                        if (!offsetsUsedInExceptionInformation.ContainsKey(x)) offsetsUsedInExceptionInformation.Add(x, true);
                    }
                }
                #endregion Record all offsets that appear as part of an exception handler

                Dictionary<uint, ILGeneratorLabel> offset2Label = new Dictionary<uint, ILGeneratorLabel>();
                #region Pass 1: Make a label for each branch target
                for (int i = 0; i < count; i++)
                {
                    IOperation op = operations[i];
                    switch (op.OperationCode)
                    {
                        case OperationCode.Beq:
                        case OperationCode.Bge:
                        case OperationCode.Bge_Un:
                        case OperationCode.Bgt:
                        case OperationCode.Bgt_Un:
                        case OperationCode.Ble:
                        case OperationCode.Ble_Un:
                        case OperationCode.Blt:
                        case OperationCode.Blt_Un:
                        case OperationCode.Bne_Un:
                        case OperationCode.Br:
                        case OperationCode.Brfalse:
                        case OperationCode.Brtrue:
                        case OperationCode.Leave:
                        case OperationCode.Beq_S:
                        case OperationCode.Bge_S:
                        case OperationCode.Bge_Un_S:
                        case OperationCode.Bgt_S:
                        case OperationCode.Bgt_Un_S:
                        case OperationCode.Ble_S:
                        case OperationCode.Ble_Un_S:
                        case OperationCode.Blt_S:
                        case OperationCode.Blt_Un_S:
                        case OperationCode.Bne_Un_S:
                        case OperationCode.Br_S:
                        case OperationCode.Brfalse_S:
                        case OperationCode.Brtrue_S:
                        case OperationCode.Leave_S:
                            uint x = (uint)op.Value;
                            if (!offset2Label.ContainsKey(x))
                                offset2Label.Add(x, new ILGeneratorLabel());
                            break;
                        case OperationCode.Switch:
                            uint[] offsets = op.Value as uint[];
                            foreach (var offset in offsets)
                            {
                                if (!offset2Label.ContainsKey(offset))
                                    offset2Label.Add(offset, new ILGeneratorLabel());
                            }
                            break;
                        default:
                            break;
                    }
                }
                #endregion Pass 1: Make a label for each branch target

                #region Pass 2: Emit each operation, along with labels
                for (int i = 0; i < count; i++)
                {
                    IOperation op = operations[i];
                    ILGeneratorLabel label;
                    this.EmitDebugInformationFor(op);
                    #region Mark operation if it is a label for a branch
                    if (offset2Label.TryGetValue(op.Offset, out label))
                    {
                        generator.MarkLabel(label);
                    }
                    #endregion Mark operation if it is a label for a branch

                    #region Mark operation if it is pointed to by an exception handler
                    bool ignore;
                    uint offset = op.Offset;
                    if (offsetsUsedInExceptionInformation.TryGetValue(offset, out ignore))
                    {
                        foreach (var exceptionInfo in methodBody.OperationExceptionInformation)
                        {
                            if (offset == exceptionInfo.TryStartOffset)
                                generator.BeginTryBody();

                            // Never need to do anthing when offset == exceptionInfo.TryEndOffset because
                            // we pick up an EndTryBody from the HandlerEndOffset below
                            //  generator.EndTryBody();

                            if (offset == exceptionInfo.HandlerStartOffset)
                            {
                                switch (exceptionInfo.HandlerKind)
                                {
                                    case HandlerKind.Catch:
                                        generator.BeginCatchBlock(exceptionInfo.ExceptionType);
                                        break;
                                    case HandlerKind.Fault:
                                        generator.BeginFaultBlock();
                                        break;
                                    case HandlerKind.Filter:
                                        generator.BeginFilterBody();
                                        break;
                                    case HandlerKind.Finally:
                                        generator.BeginFinallyBlock();
                                        break;
                                }
                            }
                            if (exceptionInfo.HandlerKind == HandlerKind.Filter && offset == exceptionInfo.FilterDecisionStartOffset)
                            {
                                generator.BeginFilterBlock();
                            }
                            if (offset == exceptionInfo.HandlerEndOffset)
                                generator.EndTryBody();
                        }
                    }
                    #endregion Mark operation if it is pointed to by an exception handler

                    #region Emit operation along with any injection
                    switch (op.OperationCode)
                    {
                        #region Branches
                        case OperationCode.Beq:
                        case OperationCode.Bge:
                        case OperationCode.Bge_Un:
                        case OperationCode.Bgt:
                        case OperationCode.Bgt_Un:
                        case OperationCode.Ble:
                        case OperationCode.Ble_Un:
                        case OperationCode.Blt:
                        case OperationCode.Blt_Un:
                        case OperationCode.Bne_Un:
                        case OperationCode.Br:
                        case OperationCode.Brfalse:
                        case OperationCode.Brtrue:
                        case OperationCode.Leave:
                        case OperationCode.Beq_S:
                        case OperationCode.Bge_S:
                        case OperationCode.Bge_Un_S:
                        case OperationCode.Bgt_S:
                        case OperationCode.Bgt_Un_S:
                        case OperationCode.Ble_S:
                        case OperationCode.Ble_Un_S:
                        case OperationCode.Blt_S:
                        case OperationCode.Blt_Un_S:
                        case OperationCode.Bne_Un_S:
                        case OperationCode.Br_S:
                        case OperationCode.Brfalse_S:
                        case OperationCode.Brtrue_S:
                        case OperationCode.Leave_S:
                            generator.Emit(op.OperationCode, offset2Label[(uint)op.Value]);
                            break;
                        case OperationCode.Switch:
                            uint[] offsets = op.Value as uint[];
                            ILGeneratorLabel[] labels = new ILGeneratorLabel[offsets.Length];
                            for (int j = 0, n = offsets.Length; j < n; j++)
                            {
                                labels[j] = offset2Label[offsets[j]];
                            }
                            generator.Emit(OperationCode.Switch, labels);
                            break;
                        #endregion Branches
                        case OperationCode.Newobj:
                            {
                                IMethodReference methodReference = op.Value as IMethodReference;
                                if (methodReference != null)
                                {
                                    Interweave(generator, methodReference);
                                }
                            }
                            break;
                        #region Everything else
                        default:
                            if (op.Value == null)
                            {
                                generator.Emit(op.OperationCode);
                                break;
                            }
                            var typeCode = System.Convert.GetTypeCode(op.Value);
                            switch (typeCode)
                            {
                                case TypeCode.Byte:
                                    generator.Emit(op.OperationCode, (byte)op.Value);
                                    break;
                                case TypeCode.Double:
                                    generator.Emit(op.OperationCode, (double)op.Value);
                                    break;
                                case TypeCode.Int16:
                                    generator.Emit(op.OperationCode, (short)op.Value);
                                    break;
                                case TypeCode.Int32:
                                    generator.Emit(op.OperationCode, (int)op.Value);
                                    break;
                                case TypeCode.Int64:
                                    generator.Emit(op.OperationCode, (long)op.Value);
                                    break;
                                case TypeCode.Object:
                                    IFieldReference fieldReference = op.Value as IFieldReference;
                                    if (fieldReference != null)
                                    {
                                        generator.Emit(op.OperationCode, this.Rewrite(fieldReference));
                                        break;
                                    }
                                    ILocalDefinition localDefinition = op.Value as ILocalDefinition;
                                    if (localDefinition != null)
                                    {
                                        generator.Emit(op.OperationCode, localDefinition);
                                        break;
                                    }
                                    IMethodReference methodReference = op.Value as IMethodReference;
                                    if (methodReference != null)
                                    {
                                        generator.Emit(op.OperationCode, this.Rewrite(methodReference));
                                        break;
                                    }
                                    IParameterDefinition parameterDefinition = op.Value as IParameterDefinition;
                                    if (parameterDefinition != null)
                                    {
                                        generator.Emit(op.OperationCode, parameterDefinition);
                                        break;
                                    }
                                    ISignature signature = op.Value as ISignature;
                                    if (signature != null)
                                    {
                                        generator.Emit(op.OperationCode, signature);
                                        break;
                                    }
                                    ITypeReference typeReference = op.Value as ITypeReference;
                                    if (typeReference != null)
                                    {
                                        generator.Emit(op.OperationCode, this.Rewrite(typeReference));
                                        break;
                                    }
                                    throw new ILMutatorException("Should never get here: no other IOperation argument types should exist");
                                case TypeCode.SByte:
                                    generator.Emit(op.OperationCode, (sbyte)op.Value);
                                    break;
                                case TypeCode.Single:
                                    generator.Emit(op.OperationCode, (float)op.Value);
                                    break;
                                case TypeCode.String:
                                    generator.Emit(op.OperationCode, (string)op.Value);
                                    break;
                                default:
                                    // The other cases are the other enum values that TypeCode has.
                                    // But no other argument types should be in the Operations. ILGenerator cannot handle anything else,
                                    // so such IOperations should never exist.
                                    //case TypeCode.Boolean:
                                    //case TypeCode.Char:
                                    //case TypeCode.DateTime:
                                    //case TypeCode.DBNull:
                                    //case TypeCode.Decimal:
                                    //case TypeCode.Empty: // this would be the value for null, but the case when op.Value is null is handled before the switch statement
                                    //case TypeCode.UInt16:
                                    //case TypeCode.UInt32:
                                    //case TypeCode.UInt64:
                                    throw new ILMutatorException("Should never get here: no other IOperation argument types should exist");
                            }
                            break;
                        #endregion Everything else
                    }
                    #endregion Emit operation along with any injection

                }
                while (generator.InTryBody)
                    generator.EndTryBody();
                while (this.scopeStack.Count > 0)
                {
                    this.currentGenerator.EndScope();
                    this.scopeStack.Pop();
                }
                #endregion Pass 2: Emit each operation, along with labels

            }

            private void EmitDebugInformationFor(IOperation operation)
            {
                this.currentGenerator.MarkSequencePoint(operation.Location);
                if (this.scopeEnumerator == null) return;
                ILocalScope/*?*/ currentScope = null;
                while (this.scopeStack.Count > 0)
                {
                    currentScope = this.scopeStack.Peek();
                    if (operation.Offset < currentScope.Offset + currentScope.Length) break;
                    this.scopeStack.Pop();
                    this.currentGenerator.EndScope();
                    currentScope = null;
                }
                while (this.scopeEnumeratorIsValid)
                {
                    currentScope = this.scopeEnumerator.Current;
                    if (currentScope.Offset <= operation.Offset && operation.Offset < currentScope.Offset + currentScope.Length)
                    {
                        this.scopeStack.Push(currentScope);
                        this.currentGenerator.BeginScope();
                        foreach (var local in this.pdbReader.GetVariablesInScope(currentScope))
                            this.currentGenerator.AddVariableToCurrentScope(local);
                        foreach (var constant in this.pdbReader.GetConstantsInScope(currentScope))
                            this.currentGenerator.AddConstantToCurrentScope(constant);
                        this.scopeEnumeratorIsValid = this.scopeEnumerator.MoveNext();
                    }
                    else
                        break;
                }
            }

            private bool TryGetLocalName(ILocalDefinition local, out string localNameFromPDB)
            {
                string localName = local.Name.Value;
                localNameFromPDB = null;
                if (this.pdbReader != null)
                {
                    foreach (IPrimarySourceLocation psloc in this.pdbReader.GetPrimarySourceLocationsForDefinitionOf(local))
                    {
                        if (psloc.Source.Length > 0)
                        {
                            localNameFromPDB = psloc.Source;
                            break;
                        }
                    }
                }
                return localNameFromPDB != null;
            }

            /// <summary>
            /// Exceptions thrown during extraction. Should not escape this class.
            /// </summary>
            private class ILMutatorException : Exception
            {
                /// <summary>
                /// Exception specific to an error occurring in the contract extractor
                /// </summary>
                public ILMutatorException() { }
                /// <summary>
                /// Exception specific to an error occurring in the contract extractor
                /// </summary>
                public ILMutatorException(string s) : base(s) { }
                /// <summary>
                /// Exception specific to an error occurring in the contract extractor
                /// </summary>
                public ILMutatorException(string s, Exception inner) : base(s, inner) { }
                /// <summary>
                /// Exception specific to an error occurring in the contract extractor
                /// </summary>
                public ILMutatorException(SerializationInfo info, StreamingContext context) : base(info, context) { }
            }
        }


}
