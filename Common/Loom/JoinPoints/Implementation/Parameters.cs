// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Reflection;
using System.Collections.Generic;

using Loom.AspectModel;
using Loom.CodeBuilder;
using Loom.Common;

namespace Loom.JoinPoints.Implementation
{

    /// <summary>
    /// Typen der Rückgabewerte sind identisch (keine Generics)
    /// </summary>
    internal class SimpleResultType : IResultTypeMatch
    {
        Type returntype;

        public SimpleResultType(MethodInfo mi)
        {
            this.returntype = mi.ReturnType;
        }

        #region IResultTypeMatch Members

        /// <summary>
        /// Match, wenn beie Typen gleich sind
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsMatch(Type type)
        {
            return returntype == type;
        }

        public Type ResultType
        {
            get { return returntype; }
        }
        #endregion
    }

    /// <summary>
    /// Generics sind identisch
    /// </summary>
    internal class SimpleGenericResultType : IResultTypeMatch, IGenericParameter
    {
        Type[] constraints;
        Type generic;
        public SimpleGenericResultType(MethodInfo mi)
        {
            constraints = mi.ReturnType.GetGenericParameterConstraints();
            generic = mi.ReturnType;
        }

        #region IResultTypeMatch Members

        /// <summary>
        /// Match, wen der generische Typ striktere oder gleiche Einschränkungen hat
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsMatch(Type type)
        {
            if (!type.IsGenericParameter) return false;

            Type[] cmpconstraints = type.GetGenericParameterConstraints();

            // Alle Constraints hier müssen matchen
            foreach (Type tStricter in constraints)
            {
                bool bIsMatch = false;
                foreach (Type tLesser in cmpconstraints)
                {
                    if (tStricter.IsInterface == tLesser.IsInterface)
                    {
                        if (tStricter == tLesser || tStricter.IsSubclassOf(tLesser))
                        {
                            bIsMatch = true;
                            break;
                        }
                    }
                }
                if (!bIsMatch) return false;
            }
            return true;
        }

        public Type ResultType
        {
            get { return generic; }
        }

        #endregion

        #region IGenericParameter Members

        public Type GenericParameterType
        {
            get { return generic; }
        }

        #endregion
    }

    /// <summary>
    /// Generics sind identisch
    /// </summary>
    internal class AdvancedGenericResultType : IResultTypeMatch, IGenericParameter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">Der Rückgabetyp der Zielmethode</param>
        /// <param name="constraints">Die Constraints der Aspektmethode</param>
        /// <returns></returns>
        internal static bool IsMatch(Type type, Type[] constraints)
        {
            // voids Matchen nicht
            if (type == typeof(void)) return false;

            // Originalmethode liefert ein A
            // dann muss die Aspectmethode auch ein A liefern oder eine von A abgeleitete Klasse
            foreach (Type t in constraints)
            {
                if (t.IsEqualOrSubclassOrHasInterfaceOf(type))
                {
                    return true;
                }
            }
            // Keine Constraints -> Ok.
            return constraints.Length==0;
        }

        Type[] constraints;
        Type generic;
        public AdvancedGenericResultType(MethodInfo mi)
        {
            constraints = mi.ReturnType.GetGenericParameterConstraints();
            generic = mi.ReturnType;
        }

        #region IResultTypeMatch Members

        /// <summary>
        /// Match, wen der generische Typ striktere oder gleiche Einschränkungen hat
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsMatch(Type type)
        {
            if (type == typeof(void))
            {
                type = typeof(_Void);
            }
            return IsMatch(type,constraints);
        }

        public Type ResultType
        {
            get { return generic; }
        }

        #endregion

        #region IGenericParameter Members

        public Type GenericParameterType
        {
            get { return generic; }
        }

        #endregion
    }

    /// <summary>
    /// is used for standart aspect method parameters
    /// evaluates parameter attributes
    /// </summary>
    internal class SimpleParameter : IParameterMatch
    {
        private ParameterInfo pi;

        public SimpleParameter(ParameterInfo pi)
        {
            this.pi = pi;
        }

        /// <summary>
        /// überpräft, ob ein Aspektmethodenparameter mit einem Zielmethodenparameter verwoben werden kann
        /// </summary>
        /// <param name="epi">Der aktuelle Parameter</param>
        /// <returns></returns>
        bool IParameterMatch.IsMatch(System.Collections.IEnumerator epi)
        {
            if (epi == null) return false;
            return pi.ParameterType == ((ParameterInfo)epi.Current).ParameterType;
        }

        void IParameterMatch.AddParameter(CodeBrickBuilder ppf)
        {
            ppf.AddSimpleParameter(pi);
        }
    }

    /// <summary>
    /// is used for object return type and object[] parameter
    /// </summary>
    internal class WildcardParameter : IParameterMatch
    {
        ParameterInfo pi;

        public WildcardParameter(ParameterInfo pi)
        {
            this.pi = pi;
        }

        /// <summary>
        /// Aspectparameter ist ein Wildcard
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        bool IParameterMatch.IsMatch(System.Collections.IEnumerator en)
        {
            // Leere Parameterlisten sind ok
            if (en == null) return true;

            System.Diagnostics.Debug.Assert(pi.ParameterType.IsArray);
            Type type = pi.ParameterType.GetElementType();
            do
            {
                Type ptype = ((ParameterInfo)en.Current).ParameterType;
                if (ptype.IsByRef) ptype = ptype.GetElementType();

                if (!ptype.IsSubclassOf(type) && type != ptype)
                {
                    return false;
                }
            }
            while (en.MoveNext());

            return true;
        }

        void IParameterMatch.AddParameter(CodeBrickBuilder ppf)
        {
            ppf.AddParameterContainer(pi);
        }
    }

    internal class AspectParameter : IParameterMatch
    {
        ParamType apt;
        Scope scope;
        protected ParameterInfo pi;

        public AspectParameter(ParameterInfo pi, ParamType apt)
        {
            this.apt = apt;
            this.pi = pi;
            this.scope = Scope.Private;
        }

        public AspectParameter(ParameterInfo pi, ParamType apt, Scope scope)
        {
            this.apt = apt;
            this.pi = pi;
            this.scope = scope;
        }

        #region IParameterMatch Members

        /// <summary>
        /// IsMatch wird immer auf dem mit diesem Attribut versehenen Parameter aufgerufen, es muss nur sichergestellt werden,
        /// das der Typ ok ist.
        /// </summary>
        /// <param name="epi"></param>
        /// <returns></returns>
        public bool IsMatch(System.Collections.IEnumerator epi)
        {
            // Parameterlose Targets sind ok.
            if (epi == null) return true;

            // Aspektparameter därfen nur am Anfang stehen, das ist der Fall, wenn die Targetparameter noch nicht angefasst wurden
            if (((ParameterInfo)epi.Current).Position != 0)
            {
                ParameterInfo pi = (ParameterInfo)epi.Current;
                throw new AspectWeaverException(10, Loom.Resources.Errors.ERR_0010, ((MethodInfo)pi.Member).DeclaringType.FullName, ((MethodInfo)pi.Member).Name, pi.Name);
            }
            epi.Reset();
            return true;
        }

        void IParameterMatch.AddParameter(CodeBrickBuilder ppf)
        {
            ppf.AddAspectParameter(pi, apt, scope);
        }

        #endregion

    }

    internal class RetValAspectParameter : AspectParameter, IResultTypeMatch
    {
        public RetValAspectParameter(ParameterInfo pi) :
            base(pi, ParamType.RetVal)
        {
        }

        #region IResultTypeMatch Members

        public bool IsMatch(Type type)
        {
            return type == pi.ParameterType || type == pi.ParameterType.GetElementType();
        }

        public Type ResultType
        {
            get { return pi.ParameterType; }
        }

        #endregion
    }

    internal class GenericRetValAspectParameter : RetValAspectParameter, IGenericParameter, IResultTypeMatch
    {
        private Type[] constraints;

        public GenericRetValAspectParameter(ParameterInfo pi) :
            base(pi)
        {
            Type t = pi.ParameterType;
            if (t.IsByRef) t = t.GetElementType();
            this.constraints = t.GetGenericParameterConstraints();
        }

        #region IGenericParameter Members

        public Type GenericParameterType
        {
            get
            {
                return pi.ParameterType.IsByRef ? pi.ParameterType.GetElementType() : pi.ParameterType;
            }
        }

        #endregion

        #region IResultTypeMatch Members

        bool IResultTypeMatch.IsMatch(Type type)
        {
            return AdvancedGenericResultType.IsMatch(type,constraints);
        }

        Type IResultTypeMatch.ResultType
        {
            get { return pi.ParameterType; }
        }

        #endregion
    }

    internal class GenericWildcardParameter : GenericBase, IParameterMatch
    {
        ParameterInfo pi;

        /// <summary>
        /// Dictionary, welches die 
        /// </summary>
        /// <param name="pi"></param>
        public GenericWildcardParameter(ParameterInfo pi) :
            base(pi.ParameterType)
        {
            this.pi = pi;
        }

        #region IParameterMatch Members

        /// <summary>
        /// Check ob Generic Match vorliegt erfolgt in mehreren schritten:
        /// </summary>
        /// <param name="epi"></param>
        /// <returns></returns>
        public bool IsMatch(System.Collections.IEnumerator epi)
        {
            if (epi == null) return false;

            Type paramtype = ((ParameterInfo)epi.Current).ParameterType;
            return base.IsMatch(paramtype);
        }

        public void AddParameter(CodeBrickBuilder ppf)
        {
            ppf.AddGenericParameter(pi);
        }

        #endregion
    }

    internal class GenericParameter : GenericBase, IParameterMatch
    {
        ParameterInfo pi;

        /// <summary>
        /// Dictionary, welches die 
        /// </summary>
        /// <param name="pi"></param>
        public GenericParameter(ParameterInfo pi)
            :
            base(pi.ParameterType)
        {
            this.pi = pi;
        }

        #region IParameterMatch Members

        /// <summary>
        /// Check ob Generics äquivalent sind
        /// </summary>
        /// <param name="epi"></param>
        /// <returns></returns>
        public bool IsMatch(System.Collections.IEnumerator epi)
        {
            if (epi == null) return false;

            Type paramtype = ((ParameterInfo)epi.Current).ParameterType;
            if (paramtype.IsByRef)
            {
                if (!generictype.IsByRef) return false;
                paramtype = paramtype.GetElementType();
            }
            if (!paramtype.IsGenericParameter) return false;
            Type[] cmpconstraints = paramtype.GetGenericParameterConstraints();
            if (cmpconstraints.Length != constraints.Length) return false;
            foreach (Type cmptype in cmpconstraints)
            {
                bool bFound = false;
                foreach (Type tp in constraints)
                {
                    if (tp == cmptype)
                    {
                        bFound = true;
                        break;
                    }
                }
                if (!bFound) return false;
            }

            return true;
        }

        public void AddParameter(CodeBrickBuilder ppf)
        {
            ppf.AddGenericParameter(pi);
        }

        #endregion
    }
}