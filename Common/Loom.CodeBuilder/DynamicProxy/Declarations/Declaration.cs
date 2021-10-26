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

using Loom.JoinPoints.Implementation;

namespace Loom.CodeBuilder.DynamicProxy.Declarations
{
    internal abstract class Declaration
    {
        /// <summary>
        /// Wird aufgerufen, um die Deklaration im ProxyTypeBuilder zu definieren (Phase 1)
        /// </summary>
        /// <param name="tb"></param>
        public abstract void DefineMember(ProxyTypeBuilderBase tb);
        /// <summary>
        /// zeigt an, dass eine statische initialisierung benötigt wird
        /// </summary>
        public virtual bool NeedStaticInitialize
        {
            get
            {
                return false;
            }
        }
        /// <summary>
        /// Dieser Code wird im statischen Constructor eingefügt, um den Member zu initialisieren
        /// </summary>
        /// <param name="ilgen"></param>
        public virtual void EmitStaticInitialization(ILGenerator ilgen) {}


        /// <summary>
        /// Wird abschliessend aufgerufen, um den Typen fertig zu bauen (Phase 2)
        /// </summary>
        public abstract void CreateType();
    }


    /// <summary>
    /// Represents DeclarationCollection of the Proxy
    /// </summary>
    internal class DeclarationCollection : IEnumerable<Declaration>
    {
        private AspectField[] aspect;
        private List<ContextClassDeclaration> contextclasses;
        private Dictionary<MethodInfo, DelegateInfo> delegates;
        private List<Dictionary<string, JPVariableDeclaration>> localsharedvariables;
        private Dictionary<string, JPVariableDeclaration> sharedvariables;

        public AspectField[] Aspects
        {
            get { return aspect; }
            set { aspect = value; }
        }

        public List<ContextClassDeclaration> ContextClasses
        {
            get
            {
                if (contextclasses == null) contextclasses = new List<ContextClassDeclaration>();
                return contextclasses;
            }
        }

        public Dictionary<MethodInfo, DelegateInfo> Delegates
        {
            get
            {
                if (delegates == null) delegates = new Dictionary<MethodInfo, DelegateInfo>();
                return delegates;
            }
        }

        public Dictionary<string, JPVariableDeclaration> SharedVariables
        {
            get
            {
                if (sharedvariables == null) sharedvariables = new Dictionary<string, JPVariableDeclaration>();
                return sharedvariables;
            }
        }

        public List<Dictionary<string, JPVariableDeclaration>> LocalSharedVariables
        {
            get
            {
                if (localsharedvariables == null) localsharedvariables = new List<Dictionary<string, JPVariableDeclaration>>();
                return localsharedvariables;
            }
        }


        #region IEnumerable<Declaration> Members

        public IEnumerator<Declaration> GetEnumerator()
        {
            if (aspect != null)
            {
                foreach (Declaration aspectdecl in aspect)
                {
                    yield return aspectdecl;
                }
            }
            if (contextclasses != null)
            {
                foreach (Declaration decl in contextclasses)
                {
                    yield return decl;
                }
            }
            if (delegates != null)
            {
                foreach (Declaration di in delegates.Values)
                {
                    yield return di;
                }
            }
            if (sharedvariables != null)
            {
                foreach (Declaration di in sharedvariables.Values)
                {
                    yield return di;
                }
            }
            if (localsharedvariables != null)
            {
                foreach (Dictionary<string, JPVariableDeclaration> dict in localsharedvariables)
                {
                    foreach (Declaration di in dict.Values)
                    {
                        yield return di;
                    }
                }
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }


}
