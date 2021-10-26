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

using Loom.JoinPoints;
using Loom.JoinPoints.Implementation;

namespace Loom.CodeBuilder.DynamicProxy.Declarations
{
    internal class JPVariableDeclaration : Declaration
    {
        private static int uniqueprivateid = 0;
        FieldBuilder fb;
        ParameterInfo pi;
        Scope scope;

        public JPVariableDeclaration(ParameterInfo pi, Scope scope)
        {
            this.pi = pi;
            this.scope = scope;
        }

        public override void DefineMember(ProxyTypeBuilderBase tb)
        {
            string name = pi.Name;
            FieldAttributes attribute = 0;
            if ((scope & Scope.Static) != 0)
            {
                attribute |= FieldAttributes.Static;
            }
            if ((scope & Scope.Private) != 0)
            {
                attribute |= FieldAttributes.Private;
                StringBuilder sb = new StringBuilder(name.Length + 10);
                sb.Append("__");
                sb.Append(name);
                sb.Append(uniqueprivateid++);
                name = sb.ToString();
            }
            if ((scope & Scope.Public) != 0)
            {
                attribute |= FieldAttributes.Public;
            }
            Type t = pi.ParameterType;
            if (t.IsByRef) t = t.GetElementType();
            fb = tb.TypeBuilder.DefineField(name, t, attribute);
        }

        public override void CreateType()
        {
        }

        public FieldInfo FieldInfo
        {
            get
            {
                return fb;
            }
        }

        public Scope Scope
        {
            get { return scope; }
        }

        /// <summary>
        /// Liefert den Parameter, der diese Variable erstmalig deklarierte
        /// </summary>
        /// <remarks>
        /// Wird verwendet, um bei Konflikten die Methode zu finden, die im Konflikt steht
        /// </remarks>
        public ParameterInfo ParameterInfo
        {
            get { return pi; }
        }

        public Type DeclarationType
        {
            get
            {
                Type type = pi.ParameterType;
                if (type.IsByRef) type = type.GetElementType();
                return type;
            }
        }

    }
}
