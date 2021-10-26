// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Loom;
using Loom.JoinPoints;

namespace VisitorExample
{
    public class VisitorDispatch : AspectAttribute
    {
        // is interweaved with all visitor objects
        [Call(Advice.Around)]
        public void Visit([JPContext] Context ctx, Expression node)
        {
            // Recalls the method of the target node with the concrete implementation of the expression node
            ctx.ReCall(node);
        }
    }
}
