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
using System.Reflection;
using Loom.Configuration;
using Loom.AspectModel;

namespace Loom.JoinPoints.Implementation
{
    internal interface IJoinPointClassFactory
    {
        AspectClass CreateAspectClass(Type aspecttype, AspectClass baseaspect);
        AspectCoverageInfo CreateAspectCoverageInfoFromConfig(AspectClass aspectclass, AspectConfig config);
        AspectCoverageInfo CreateAspectCoverageInfoFromCustomAttribute(AspectClass aspectclass, CustomAttributeData cad);
    }
}
