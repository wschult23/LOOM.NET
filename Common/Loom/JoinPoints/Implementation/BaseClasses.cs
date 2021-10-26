// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

using Loom.AspectModel;
using Loom.CodeBuilder;
using Loom.Common;

namespace Loom.JoinPoints.Implementation
{
    internal class GenericBase : IGenericParameter
    {
        protected Type[] constraints;
        protected Type generictype;

        public GenericBase(Type generictype)
        {
            this.generictype = generictype;
            if (generictype.IsByRef)
            {
                this.constraints = generictype.GetElementType().GetGenericParameterConstraints();
            }
            else
            {
                this.constraints = generictype.GetGenericParameterConstraints();
            }
        }

        #region IResultTypeMatch Members

        /// <summary>
        /// Checkt ob ein Zielmethodenparameter in die Constraints der Aspektmethode passt
        /// </summary>
        /// <param name="type">der Typ des Zielmethodenparametrers</param>
        /// <returns></returns>
        public bool IsMatch(Type type)
        {
            if (type.IsByRef != generictype.IsByRef) return false;
            if (type.IsByRef)
            {
                type = type.GetElementType();
            }

            foreach (Type t in constraints)
            {
                if (!type.IsEqualOrSubclassOrHasInterfaceOf(t))
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region IGenericParameter Members

        public Type GenericParameterType
        {
            get { return generictype.IsByRef ? generictype.GetElementType() : generictype; }
        }

        #endregion
    }
}
