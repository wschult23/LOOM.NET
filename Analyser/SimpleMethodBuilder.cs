// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using Loom.CodeBuilder;

namespace Loom.Analyzer
{
    internal class AspectCallCodeBrick : CodeBrickBuilder
    {
        StringBuilder sb;

        public AspectCallCodeBrick(StringBuilder sb)
        {
            this.sb = sb;
        }

        public override void AddSimpleParameter(ParameterInfo pi)
        {
            sb.AppendFormat("simpleparam[{0} {1}], ", pi.ParameterType.Name, pi.Name);
        }

        public override void AddGenericParameter(ParameterInfo pi)
        {
            sb.AppendFormat("genericparam[{0} {1}], ", pi.ParameterType.Name, pi.Name);
        }

        public override void AddAspectParameter(ParameterInfo pi, Loom.JoinPoints.ParamType apt, Loom.JoinPoints.Scope scope)
        {
            sb.AppendFormat("aspectparam<{2}>[{0} {1}], ", pi.ParameterType.Name, pi.Name, apt.ToString());
        }

        public override void AddParameterContainer(ParameterInfo container)
        {
            sb.AppendFormat("containerparam[{0} {1}], ", container.ParameterType.Name, container.Name);
        }

        public override void CreateCodeBrick()
        {
            if (sb[sb.Length - 1] == ' ') sb.Length -= 2;
            sb.Append(")\n");
        }
    }

    internal class SimpleMethodBuilder:MethodCodeBuilder
    {
        StringBuilder sb;
        string indent;
        public SimpleMethodBuilder(StringBuilder sb, string indent)
        {
            this.sb = sb;
            this.indent = indent;
        }

        public override void Emit()
        {
        }

        public override CodeBrickBuilder DefineAspectCall(MethodInfo aspectmethod, Loom.JoinPoints.Advice advice)
        {
            sb.AppendFormat("{3}call {0} {1}.{2}",advice.ToString(),aspectmethod.DeclaringType.Name,aspectmethod.Name,indent);
            Type[] generics = aspectmethod.GetGenericArguments();
            if (generics.Length > 0)
            {
                sb.Append("<");
                foreach (Type t in generics)
                {
                    sb.Append(t.Name);
                    sb.Append(",");
                }
                sb.Length--;
                sb.Append(">");
            }
            sb.Append("(");
            return new AspectCallCodeBrick(sb);
        }
    }
}
