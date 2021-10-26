// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Reflection;
using Loom.AspectModel;
using Loom.JoinPoints.Implementation;

namespace Loom.CodeBuilder
{
    /// <summary>
    /// Die Basisklasse zum Erzeugen von Methoden in einem TypeBuilder
    /// Für die konkrete Codeerzeugung kommen CodeBricks und
    /// ParameterCodeBricks zum Einsatz. MethodCodeBuilder selbst liefern
    /// nur die Factory für die konkrete Implementation.
    /// </summary>
    internal abstract class MethodCodeBuilder
    {
        protected MethodCodeBuilder()
        {
        }

        /// <summary>
        /// Methode erzeugen
        /// </summary>
        public abstract void Emit();

        /// <summary>
        /// verwebt die methoden
        /// </summary>
        /// <param name="methods"></param>
        public void Interweave(AspectMemberCollection methods)
        {
            if (methods != null)
            {
                foreach (AspectMember am in methods)
                {
                    am.Interweave(this);
                }
            }
        }

        /// <summary>
        /// fügt in den MethodenCode einen Aspektaufruf ein
        /// </summary>
        /// <param name="aspectmethod"></param>
        /// <param name="advice"></param>
        /// <returns></returns>
        public abstract CodeBrickBuilder DefineAspectCall(MethodInfo aspectmethod, Loom.JoinPoints.Advice advice);

    }
}
