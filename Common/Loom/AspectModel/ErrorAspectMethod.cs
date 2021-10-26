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

using Loom;
using Loom.JoinPoints.Implementation;
using Loom.JoinPoints;

namespace Loom.AspectModel
{
    /// <summary>
    /// Wirft eine Exception, wenn es einen Match gibt
    /// </summary>
    internal class ErrorAspectMethod : CompilerSpecialAspectMethod
    {
        string message;

        internal ErrorAspectMethod(MethodInfo mi, TargetTypes targettype, string message)
            : base(mi, targettype)
        {
            // Rückgabewert
            this.message = message;
        }

        public override void Interweave(Loom.CodeBuilder.MethodCodeBuilder mcb)
        {
            throw new AspectWeaverException(9010, Resources.Errors.ERR_9010,((IJoinPointInfo)mcb).JoinPoint.ToString(), message);
        }
    }
}
