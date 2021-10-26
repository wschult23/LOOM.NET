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
using Loom.JoinPoints.Implementation;
using System.Reflection;
using Loom.Configuration;
using Loom.AspectProperties;
using Loom.AspectModel;

namespace Loom.CodeBuilder.DynamicProxy.JoinPoints
{
    internal class ProxyAspectCoverageInfoFactory:IJoinPointClassFactory
    {
        public AspectCoverageInfo CreateAspectCoverageInfoFromConfig(AspectClass aspectclass, AspectConfig config)
        {
            return new ConfigAspectCoverageInfo(aspectclass, config);
        }

        public AspectCoverageInfo CreateAspectCoverageInfoFromCustomAttribute(AspectClass ac, CustomAttributeData cad)
        {
            return new CustomAttributeAspectCoverageInfo(ac, cad);
        }

        public AspectClass CreateAspectClass(Type aspecttype, AspectClass baseaspect)
        {
            return new ProxyAspectClass(aspecttype, baseaspect);
        }
    }
}
