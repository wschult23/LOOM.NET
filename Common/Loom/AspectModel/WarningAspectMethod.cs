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
using Loom.JoinPoints.Implementation;
using Loom.JoinPoints;

namespace Loom.AspectModel
{
    /// <summary>
    /// Wirft eine Exception, wenn es einen Match gibt
    /// </summary>
    internal class WarningAspectMethod : CompilerSpecialAspectMethod
    {
        string message;

        internal WarningAspectMethod(MethodInfo mi, TargetTypes targettypes, string message)
            : base(mi, targettypes)
        {
            // Rückgabewert
            this.message = message;
        }

        public override void Interweave(Loom.CodeBuilder.MethodCodeBuilder mcb)
        {
            AspectWeaver.WriteWeaverMessage(Loom.WeaverMessages.MessageType.Warning, 9010, Resources.Warnings.WARN_9010, ((IJoinPointInfo)mcb).JoinPoint.ToString(), message);
        }
    }
}
