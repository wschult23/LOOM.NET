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
using Loom.JoinPoints.Implementation;

namespace Loom.CodeBuilder
{
    /// <summary>
    /// Basisklasse zum verweben eines Typen
    /// </summary>
    internal abstract class InterwovenTypeBuilder
    {
        /// <summary>
        /// Verwebt eine Aspektklasse auf einem Typen
        /// </summary>
        /// <param name="aspect"></param>
        /// <returns>true, wenn verwoben wurde</returns>
        public abstract bool Interweave(Aspect aspect);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract bool Interweave();
    }
}
