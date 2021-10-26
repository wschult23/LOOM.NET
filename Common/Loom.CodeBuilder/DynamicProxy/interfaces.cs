// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

using Loom.CodeBuilder.DynamicProxy.Parameter;
using Loom.CodeBuilder.DynamicProxy.CodeBricks;
using Loom.CodeBuilder.DynamicProxy.DataSlots;
using Loom.CodeBuilder.DynamicProxy.Declarations;
using Loom.JoinPoints.Implementation;

namespace Loom.CodeBuilder.DynamicProxy
{

    /// <summary>
    /// Dieses Interface wird genutzt, um an einen ILGenerator zur erhalten
    /// </summary>
    internal interface IILGenerator
    {
        /// <summary>
        /// Der Generator
        /// </summary>
        ILGenerator ILGenerator
        {
            get;
        }

        LocalDataSlotCollection DataSlots
        {
            get;
        }

        DeclarationCollection Declarations
        {
            get;
        }
    }

    internal class ArgArrayDictionary : Dictionary<int, LocalArgArray> { };
    /// <summary>
    /// Hier werden alle Localen Variablen und ihre CodeBricks der Methode gespeichert
    /// </summary>
    internal class LocalDataSlotCollection : IEnumerable<DataSlot>
    {
        ////////////////////////////////////////////////////////////
        // Factory-Zugriff
        /// <summary>
        /// 
        /// </summary>
        private DataSlot retval;

        internal DataSlot RetVal
        {
            get { return retval; }
            set { retval = value; }
        }

        private DataSlot exception;

        internal DataSlot Exception
        {
            get { return exception; }
            set { exception = value; }
        }

        private ArgArrayDictionary argarrays;
        internal ArgArrayDictionary ArgArrays
        {
            get { return argarrays; }
            set { argarrays = value; }
        }

        private List<DataSlot> variables;
        internal List<DataSlot> Variables
        {
            get { return variables == null ? variables = new List<DataSlot>() : variables; }
        }

        private ContextDataSlot context;

        /// <summary>
        /// Der Context taucht hier auf, für die lokale benutzung und noch einmal
        /// global in den declarations
        /// </summary>
        internal ContextDataSlot Context
        {
            get { return context; }
            set { context = value; }
        }


        private DataSlot aspect;

        internal DataSlot Aspect
        {
            get { return aspect; }
            set { aspect = value; }
        }


        ///////////////////////////////////////////////////
        // MethodBuilder-Zugriff
        #region IEnumerable<DataSlot> Members

        public IEnumerator<DataSlot> GetEnumerator()
        {
            if (aspect != null) yield return aspect;
            if (retval != null) yield return retval;
            if (exception != null) yield return exception;
            if (context != null) yield return context;
            if (argarrays != null) foreach (DataSlot lv in argarrays.Values) yield return lv;
            if (variables != null) foreach (DataSlot lv in variables) yield return lv;
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
