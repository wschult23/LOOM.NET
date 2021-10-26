// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Loom.AspectModel;
using System.Reflection;

namespace Loom.CodeBuilder.DynamicProxy.JoinPoints
{
    class ProxyAspectClass : AspectClass, IBindToAspectFieldDeclaration
    {
        public ProxyAspectClass(Type aspecttype, AspectClass baseaspect):
            base(aspecttype, baseaspect)
        {
        }

        protected FieldInfo field;

        public FieldInfo AspectFieldInfo { get { return field; } }

        public void Bind(FieldInfo field)
        {
            this.field = field;
        }

        public int MagicCookie; 
    }
}
