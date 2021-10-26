// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Loom.JoinPoints;

namespace Loom.UnitTests
{
    public class ScopeTestBase : TestBase
    {
        public class LocalTargetClass
        {
            public int foocall;
            public int barcall;

            public virtual void foo()
            {
                foocall++;
            }

            public virtual void bar()
            {
                barcall++;
            }
        }

        public class PublicTargetClass
        {
            public int i;

            public virtual void Test()
            {
                i++;
            }
        }

        public class PublicTargetClass2
        {
            public virtual void Test()
            {
            }
        }


        public class ProtectedTargetClass
        {
            public int PublicValue
            {
                get { return this.i; }
            }

            protected int i;

            public virtual void Test()
            {
                i++;
            }
        }

        public class PrivateTargetClass
        {
            public int PublicValue
            {
                get { return this.i; }
            }

            private int i;

            public virtual void Test()
            {
                i++;
            }
        }


        public class PublicAspect : AspectAttribute
        {
            public int Value;

            [Call(Advice.Before)]
            public void Test([JPVariable(Scope.Public)] ref int i)
            {
                this.Value = ++i;
            }

            [Call(Advice.After)]
            [IncludeAll]
            public void Test2([JPVariable(Scope.Public)] ref int i)
            {
                this.Value = ++i;
            }
        }

        public class ProtectedAspect : AspectAttribute
        {
            public int Value;

            [Call(Advice.Before)]
            public void Test([JPVariable(Scope.Protected)] ref int i)
            {
                this.Value = ++i;
            }

            [Call(Advice.After)]
            [IncludeAll]
            public void Test2([JPVariable(Scope.Protected)] ref int i)
            {
                this.Value = ++i;
            }
        }

        public class PrivateAspect : AspectAttribute
        {
            public int Value;

            [Call(Advice.Before)]
            public void Test([JPVariable(Scope.Private)] ref int i)
            {
                Assert.AreEqual(0, i);
                this.Value = ++i;
            }

            [Call(Advice.After)]
            [IncludeAll]
            public void Test2([JPVariable(Scope.Private)] ref int i)
            {
                Assert.AreEqual(1, i);
                this.Value = ++i;
            }
        }

        public class PrivateAspect2 : PrivateAspect
        {
        }


        public class OverrideAspect : AspectAttribute
        {
            [Call(Advice.Before)]
            public void Test([JPVariable(Scope.Override)] ref int i2)
            {
            }
        }

        
        public class LocalAspect : AspectAttribute
        {
            public int Value;

            [Call(Advice.Before)]
            [IncludeAll]
            public void Call1([JPVariable(Scope.Local)] ref int i)
            {
                Value = ++i;
            }
            
            [Call(Advice.Before)]
            [IncludeAll]
            public void Call2([JPVariable(Scope.Local)] ref int i)
            {
                Value = ++i;
            }
        }

        public class InconsistentAspect : AspectAttribute
        {
            [Call(Advice.Before)]
            public void Test([JPVariable(Scope.Public)] ref int i)
            {
            }

            [Call(Advice.After)]
            [IncludeAll]
            public void Test2([JPVariable(Scope.Private)] ref int i)
            {
            }
        }

        public class IllegalAspect1 : AspectAttribute
        {
            [Call(Advice.Before)]
            public void Test([JPVariable(Scope.Private | Scope.Public)] ref int i)
            {
            }
        }

        public class IllegalAspect2 : AspectAttribute
        {
            [Call(Advice.Before)]
            public void Test([JPVariable(Scope.Private | Scope.Override)] ref int i)
            {
            }
        }

        public class IllegalAspect3 : AspectAttribute
        {
            [Call(Advice.Before)]
            public void Test([JPVariable(Scope.Static | Scope.Override)] ref int i)
            {
            }
        }

    }

    [TestClass]
    public class ScopeTests : ScopeTestBase
    {
        #region Scope.Private

        [TestMethod]
        public void Private_Private_Test()
        {
            PrivateAspect aspect = new PrivateAspect();
            PrivateTargetClass target = Weaver.Create<PrivateTargetClass>(aspect);
            target.Test();

            Assert.AreEqual(2, aspect.Value);
            Assert.AreEqual(1, target.PublicValue);
        }

        [TestMethod]
        public void Private_Private_Test2()
        {
            PrivateAspect aspect1 = new PrivateAspect();
            PrivateAspect2 aspect2 = new PrivateAspect2();
            PrivateTargetClass target = Weaver.Create<PrivateTargetClass>(new Aspect[2] { aspect1, aspect2 } );
            target.Test();

            Assert.AreEqual(2, aspect1.Value);
            Assert.AreEqual(2, aspect2.Value);
            Assert.AreEqual(1, target.PublicValue);
        }

        [TestMethod]
        public void Private_Protected_Test()
        {
            PrivateAspect aspect = new PrivateAspect();
            ProtectedTargetClass target = Weaver.Create<ProtectedTargetClass>(aspect);
            target.Test();

            Assert.AreEqual(2, aspect.Value);
            Assert.AreEqual(1, target.PublicValue);
        }

        [TestMethod]
        public void Private_Public_Test()
        {
            PrivateAspect aspect = new PrivateAspect();
            PublicTargetClass target = Weaver.Create<PublicTargetClass>(aspect);
            target.Test();

            Assert.AreEqual(2, aspect.Value);
            Assert.AreEqual(1, target.i);
        }

        [TestMethod]
        public void Private_Public_Test2()
        {
            PrivateAspect aspect = new PrivateAspect();
            PublicTargetClass2 target = Weaver.Create<PublicTargetClass2>(aspect);
            target.Test();

            Assert.AreEqual(2, aspect.Value);
        }

        #endregion


        #region Scope.Protected

        [TestMethod]
        public void Protected_Private_Test()
        {
            ProtectedAspect aspect = new ProtectedAspect();
            PrivateTargetClass target = Weaver.Create<PrivateTargetClass>(aspect);
            target.Test();

            Assert.AreEqual(2, aspect.Value);
            Assert.AreEqual(1, target.PublicValue);
        }

        [TestMethod]
        public void Protected_Protected_Test()
        {
            ProtectedAspect aspect = new ProtectedAspect();
            ProtectedTargetClass target = Weaver.Create<ProtectedTargetClass>(aspect);
            target.Test();

            Assert.AreEqual(aspect.Value, target.PublicValue);
        }

        [TestMethod]
        [ExpectedException(typeof(AspectWeaverException))]
        public void Protected_Public_Test()
        {
            ProtectedAspect aspect = new ProtectedAspect();
            PublicTargetClass target = Weaver.Create<PublicTargetClass>(aspect);
        }

        #endregion

        
        #region Scope Public

        [TestMethod]
        public void Public_Private_Test()
        {
            PublicAspect aspect = new PublicAspect();
            PrivateTargetClass target = Weaver.Create<PrivateTargetClass>(aspect);
            target.Test();
            Assert.AreEqual(2, aspect.Value);
            Assert.AreEqual(1, target.PublicValue);
        }

        [TestMethod]
        public void Public_Protected_Test()
        {
            PublicAspect aspect = new PublicAspect();
            ProtectedTargetClass target = Weaver.Create<ProtectedTargetClass>(aspect);
            target.Test();
            Assert.AreEqual(aspect.Value, target.PublicValue);
        }


        [TestMethod]
        public void Public_Public_Test()
        {
            PublicAspect aspect = new PublicAspect();
            PublicTargetClass target = Weaver.Create<PublicTargetClass>(aspect);
            target.Test();
            Assert.AreEqual(aspect.Value, target.i);
        }

        #endregion


        #region Scope Local

        [TestMethod]
        public void Local_Test()
        {
            LocalAspect aspect = new LocalAspect();
            LocalTargetClass target1 = Weaver.Create<LocalTargetClass>(aspect);
            target1.foo();
            Assert.AreEqual(2, aspect.Value);
            Assert.AreEqual(1, target1.foocall);

            LocalTargetClass target2 = Weaver.Create<LocalTargetClass>(aspect);
            target2.bar();
            Assert.AreEqual(2, aspect.Value);
            Assert.AreEqual(1, target2.barcall);
        }

        #endregion

        [TestMethod]
        [ExpectedException(typeof(AspectWeaverException))]
        public void Override_Test()
        {
            OverrideAspect aspect = new OverrideAspect();
            PublicTargetClass target = Weaver.Create<PublicTargetClass>(aspect);
        }

        /// <summary>
        /// (Private, Public/Protected und Local sind drei unabhaengige Klassen von Variablen, die in
        /// unterschiedlichen Scopes implementiert werden. Daher gibt es auch keine Namenskonflikte.
        /// Wuerde man versuchen dies als Fehler zu interpretieren, muesste man bei der Prüfung auch gegen
        /// die Variablen der anderen Scopes testen.
        /// </summary>
        /// <remarks>
        /// Theoretisch zwar richtig, praktisch lassen wir den Fall als gueltig gelten, da es sich nicht effizient implementieren laesst
        /// </remarks>
        [Ignore()]
        [TestMethod]
        [ExpectedException(typeof(AspectWeaverException))]
        public void Inconsistent_Test()
        {
            InconsistentAspect aspect = new InconsistentAspect();
            PublicTargetClass2 target = Weaver.Create<PublicTargetClass2>(aspect);
        }


        [TestMethod]
        [ExpectedException(typeof(AspectWeaverException))]
        public void IllegalCombination1()
        {
            IllegalAspect1 aspect = new IllegalAspect1();
            PublicTargetClass target = Weaver.Create<PublicTargetClass>(aspect);
        }

        [TestMethod]
        [ExpectedException(typeof(AspectWeaverException))]
        public void IllegalCombination2()
        {
            IllegalAspect2 aspect = new IllegalAspect2();
            PublicTargetClass target = Weaver.Create<PublicTargetClass>(aspect);
        }

        [TestMethod]
        [ExpectedException(typeof(AspectWeaverException))]
        public void IllegalCombination3()
        {
            IllegalAspect3 aspect = new IllegalAspect3();
            PublicTargetClass target = Weaver.Create<PublicTargetClass>(aspect);
        }

    }

    
     
}
