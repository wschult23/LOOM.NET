// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using Loom;

using Loom.JoinPoints;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Loom.UnitTests.AspectInformation
{
	/// <summary>Contains classes and aspects for testing interweaving.</summary>
	public class AspectInformationTestBase:TestBase
	{
		public class Aspect1:AspectAttribute
		{
			[IncludeAll]
			[Call(Advice.Before)]
			public void All(params object[] args)
			{
			}
		}
		public class Aspect2:Aspect1
		{
			[IncludeAll]
			[Call(Advice.Before)]
			public void All2(params object[] args)
			{
			}
		}
		public class Aspect3:AspectAttribute
		{
			[IncludeAll]
			[Call(Advice.Before)]
			public void All(params object[] args)
			{
			}
		}
	}

	[TestClass]
	public class IDynamicAspectInfoTest:AspectInformationTestBase
	{
        /*
        [TestMethod]
		public void DeclaringType()
		{
			AspectAttribute asp=new Aspect1();
			B b=Weaver.CreateInstance(typeof(B),null,asp) as B;
			IAspectInfo ai=b as IAspectInfo;
			Assert.IsNotNull(ai);
			Assert.AreEqual(typeof(B), ai.DeclaringType);
		}
        */
        [TestMethod]
        public void TargetClass()
        {
            AspectAttribute asp = new Aspect1();
            B b = Weaver.Create<B>(asp);
            IAspectInfo dai = b as IAspectInfo;
            Assert.IsNotNull(dai);
            Assert.AreEqual(typeof(B), dai.TargetClass ); //alternative: Weaver.GetDeclaringType(b)
        }
        
		[TestMethod]
		public void GetAspects1()
		{
            B b = Weaver.Create<B>();
            IAspectInfo dai = b as IAspectInfo;
			Assert.IsNull(dai,"IAspectInfo implementiert obwohl nicht erwartet");

            b=Weaver.Create<B>(new Aspect1());
            dai = b as IAspectInfo;
			Assert.IsNotNull(dai,"IAspectInfo nicht implementiert");
            Assert.AreEqual(0, dai.GetAspects(typeof(Aspect3)).Length);
        }

        [TestMethod]
        public void GetAspects2()
        {
            Aspect[] asp = new Aspect[] { new Aspect2(), new Aspect3() };
            B b = Weaver.Create<B>(asp);
            IAspectInfo dai = b as IAspectInfo;
            Assert.IsNotNull(dai, "IAspectInfo nicht implementiert");
            Assert.IsNotNull(dai.GetAspects(), "AspectAttribute nicht gefunden");
            Assert.AreEqual(2, dai.GetAspects().Length, "nicht die richtigen Aspekte gefunden");
            Assert.AreEqual(asp[0], dai.GetAspects()[0], "falschen AspectAttribute gefunden");
            Assert.AreEqual(asp[1], dai.GetAspects()[1], "falschen AspectAttribute gefunden");
            Assert.IsNotNull(dai.GetAspects(typeof(Aspect1)));
            Assert.AreEqual(1, dai.GetAspects(typeof(Aspect1)).Length);
            Assert.AreEqual(asp[0], dai.GetAspects(typeof(Aspect1))[0]);
            Assert.IsNotNull(dai.GetAspects(typeof(Aspect2)));
            Assert.AreEqual(1, dai.GetAspects(typeof(Aspect2)).Length);
            Assert.AreEqual(asp[0], dai.GetAspects(typeof(Aspect2))[0]);
            Assert.IsNotNull(dai.GetAspects(typeof(Aspect3)));
            Assert.AreEqual(1, dai.GetAspects(typeof(Aspect3)).Length);
            Assert.AreEqual(asp[1], dai.GetAspects(typeof(Aspect3))[0]);
        }
        
	}
}
