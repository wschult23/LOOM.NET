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
using Loom.CodeBuilder;
using Loom.AspectModel;
using Loom.JoinPoints.Implementation;

namespace Loom.Analyzer
{
    public class Analyzer:IJoinPointClassFactory, AspectWeaver.IAspectWeaverEvent
    {
        public Analyzer()
        {
            AspectWeaver.EventHandler = this;
            Loom.JoinPoints.Implementation.JoinPointCollection.Initialize(this);
        }

        public void InterweaveType(Type targetclass)
        {
            SimpleTypeBuilder stb = new SimpleTypeBuilder(targetclass);
            
            stb.Interweave();
        }

        AspectClass IJoinPointClassFactory.CreateAspectClass(Type aspecttype, AspectClass baseaspect)
        {
 	        return new AspectClass(aspecttype, baseaspect);
        }

        AspectCoverageInfo IJoinPointClassFactory.CreateAspectCoverageInfoFromConfig(AspectClass aspectclass, Loom.Configuration.AspectConfig config)
        {
 	        return new AspectCoverageInfo(aspectclass);
        }

        AspectCoverageInfo IJoinPointClassFactory.CreateAspectCoverageInfoFromCustomAttribute(AspectClass aspectclass, CustomAttributeData cad)
        {
 	        return new AspectCoverageInfo(aspectclass);
        }

        public bool HasTarget
        {
            get { return WeaverMessages != null; }
        }

        public event WeaverMessages.WeaverMessageEventHandler WeaverMessages;

        public void Fire(WeaverMessages.WeaverMessageEventArgs arg)
        {
            WeaverMessages(this, arg);
        }
    }
}
