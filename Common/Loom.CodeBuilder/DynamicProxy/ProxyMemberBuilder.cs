// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Reflection;
using System.Reflection.Emit;

using Loom.JoinPoints.Implementation;
using Loom.CodeBuilder.DynamicProxy.DataSlots;
using Loom.CodeBuilder.DynamicProxy.Declarations;
using Loom.CodeBuilder.DynamicProxy.CodeBricks;
using System.Collections.Generic;
using System.Text;

namespace Loom.CodeBuilder.DynamicProxy
{
    /// <summary>
    /// Basisklasse für alle Builder in Loom.CodeBuilder
    /// </summary>
    internal abstract class ProxyMemberBuilder : MethodCodeBuilder, IILGenerator, IJoinPointInfo
    {
        // der Parent Type TypeBuilder
        private ProxyTypeBuilderBase tb;
        // Lokale Variablen des Verwebungscodes
        private LocalDataSlotCollection dataslots;
        // der IL Generator für den Verwebungscode
        protected ILGenerator ilgen;
        // der JoinPoint
        protected MethodBaseJoinPoint joinpoint;
        // wenn eine Contextklasse angelegt werden musste, wird sie hier vorgehalten
        ContextClassDeclaration context;
        /// <summary>
        /// Variablen, die pro methode geshared werden
        /// </summary>
        protected Dictionary<string, JPVariableDeclaration> localsharedvariables;
        /// <summary>
        /// Variablen, die pro call geshared werden
        /// </summary>
        protected Dictionary<string, DataSlot> callsharedvariables;

        protected ProxyMemberBuilder(ProxyTypeBuilderBase tb, JoinPoint jp)
        {
            this.tb = tb;
            this.dataslots = new LocalDataSlotCollection();
            this.joinpoint = (MethodBaseJoinPoint)jp;
        }

        /// <summary>
        /// Der JoinPoint, auf den sich der Builder bezieht
        /// </summary>
        public MethodBaseJoinPoint JoinPoint
        {
            get
            {
                return joinpoint;
            }
        }

        /// <summary>
        /// Pure Join point information
        /// </summary>
        JoinPoint IJoinPointInfo.JoinPoint
        {
            get
            {
                return joinpoint;
            }
        }

        /// <summary>
        /// Leitet vom JoinPoint einen neuen Joinpoit für den gebauten Typen ab
        /// </summary>
        /// <returns></returns>
        public abstract MethodBaseJoinPoint BindJoinPointToType();

        /// <summary>
        /// Passt die generische Argumentliste für die neu erzeugte Methode an.
        /// </summary>
        /// <param name="meb"></param>
        /// <param name="basemethod"></param>
        private static void DefineGenericArgs(MethodBuilder meb, MethodInfo basemethod)
        {
            if (basemethod.IsGenericMethod)
            {
                Type[] args = basemethod.GetGenericArguments();
                string[] names = new string[args.Length];
                for (int iPos = 0; iPos < args.Length; iPos++)
                {
                    names[iPos] = args[iPos].Name;
                }
                meb.DefineGenericParameters(names);
            }
        }

        /// <summary>
        /// Definiert die neue Methode
        /// Wenn vorher kein MethodCode festgelegt wurde, wird ein passender CodeBrick
        /// für den Aufruf der Basisklasse erzeugt.
        /// </summary>
        /// <returns></returns>
        protected MethodBuilder DefineVirtualMethod(TypeBuilder tb, MethodInfo basemethod, Type returntype, Type[] parametertypes)
        {
            MethodAttributes methodattributes;
            // kein Interface - einfach überschreiben
            methodattributes = basemethod.Attributes;//|MethodAttributes.Virtual;

            if ((methodattributes & MethodAttributes.NewSlot) != 0)
                methodattributes -= MethodAttributes.NewSlot; // override

            if ((methodattributes & MethodAttributes.Abstract) != 0)
                methodattributes -= MethodAttributes.Abstract;

            MethodBuilder meb = tb.DefineMethod(basemethod.Name, methodattributes, basemethod.ReturnType, parametertypes);
            DefineGenericArgs(meb, basemethod);

            return meb;
        }

        /// <summary>
        /// Diese Methode wird auf das Interface gemappt. Es handelt sich hierbei um eine private Implemntierung
        /// Die Verwebung erfolgt auf einer Ersatzfunktion
        /// </summary>
        protected static MethodBuilder DefineAndEmitInterfaceMethod(TypeBuilder tb, MethodInfo methodinterface, Type returntype, Type[] parametertypes, MethodInfo mimethodimplementation)
        {
            string methodname = Common.Strings.GetInterfaceMethodName(methodinterface);
            MethodAttributes methodattributes = MethodAttributes.NewSlot | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Private | MethodAttributes.Final;

            MethodBuilder mbbasecallmethod = tb.DefineMethod(methodname, methodattributes, returntype, parametertypes);

            DefineGenericArgs(mbbasecallmethod, methodinterface);
            tb.DefineMethodOverride(mbbasecallmethod, methodinterface);

            ILGenerator ilgen = mbbasecallmethod.GetILGenerator();
            CommonCode.EmitLdArgList(ilgen, 0, parametertypes.Length + 1);
            ilgen.Emit(OpCodes.Tailcall);
            ilgen.EmitCall(OpCodes.Call, mimethodimplementation, null);
            ilgen.Emit(OpCodes.Ret);

            return mbbasecallmethod;
        }

        /// <summary>
        /// Definiert die neue Methode
        /// Wenn vorher kein MethodCode festgelegt wurde, wird ein passender CodeBrick
        /// für den Aufruf der Basisklasse erzeugt.
        /// </summary>
        /// <returns></returns>
        protected static MethodBuilder DefineInterfaceImplementationMethod(TypeBuilder tb, MethodInfo methodinterface, Type returntype, Type[] parametertypes)
        {
            string methodname = Common.Strings.GetInterfaceMethodImplName(methodinterface);

            MethodAttributes methodattributes = MethodAttributes.HideBySig | MethodAttributes.Private | MethodAttributes.Family;

            // build method body
            MethodBuilder mbmethodimplementation = tb.DefineMethod(methodname, methodattributes, returntype, parametertypes);

            DefineGenericArgs(mbmethodimplementation, methodinterface);

            return mbmethodimplementation;
        }


        /// <summary>
        /// returns the targettype builder object
        /// </summary>
        public ProxyTypeBuilderBase DeclaringBuilder
        {
            get { return tb; }
        }

        public TypeBuilder TypeBuilder
        {
            get { return tb.TypeBuilder; }
        }

        #region IILGenerator Members
        public ILGenerator ILGenerator
        {
            get { return ilgen; }
        }

        public LocalDataSlotCollection DataSlots
        {
            get { return dataslots; }
        }

        public DeclarationCollection Declarations
        {
            get { return tb.Declarations; }
        }

        public virtual Dictionary<string, JPVariableDeclaration> LocalSharedVariables
        {
            get
            {
                if (localsharedvariables == null)
                {
                    localsharedvariables = new Dictionary<string, JPVariableDeclaration>();
                    Declarations.LocalSharedVariables.Add(localsharedvariables);
                }
                return localsharedvariables;
            }
        }

        public virtual Dictionary<string, DataSlot> CallSharedVariables
        {
            get
            {
                if (callsharedvariables == null)
                {
                    callsharedvariables = new Dictionary<string, DataSlot>();
                }
                return callsharedvariables;
            }
        }


        #endregion

        public virtual ContextClassDeclaration ContextClassDeclaration
        {
            get
            {
                if (context == null)
                {
                    // Richtige deklaration holen
                    context = GetContextClassDeclaration();
                    // Beim Proxytypebuilder registrieren
                    Declarations.ContextClasses.Add(context);
                }
                return context;
            }
        }

        protected abstract ContextClassDeclaration GetContextClassDeclaration();

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetType().Name);
            sb.Append(" JoinPoint=\"");
            sb.Append(JoinPoint.ToString());
            sb.Append("\"");
            return sb.ToString();
        }
    }


}
