﻿// ----------------------------------------------------------------
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

namespace Loom.JoinPoints.Implementation
{
    internal class AspectCoverageInfo 
    {
        private AspectClass aspectclass;

        public AspectClass AspectClass { get { return aspectclass; } }

        internal AspectCoverageInfo(AspectClass aspectclass)
        {
            this.aspectclass = aspectclass;
        }
    }
}
