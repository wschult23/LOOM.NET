// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using Loom;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Loom.JoinPoints;



namespace Loom.UnitTests.BugReports
{
	/// <summary>
	/// Aufgenommen: 10.11.03
	/// Von: Kai K�hne
	/// Versionsstand: 1.0.34.0
	/// Diagnose: Endlosschleife beim Aufruf einer verwobenen virtuellen Interfacefunktion.
	/// </summary>
	[TestClass]
	public class Bug0001
	{
		public class TestAspect:AspectAttribute
		{
			bool bInvokedTwice=false;

			[Call(Advice.Around)] [IncludeAll]
            public T f<T>([JPContext] Context Context, params object[] args)
			{
				Assert.IsFalse(bInvokedTwice,"Aspect method has called twice");
				bInvokedTwice=true;
				// invoke original function
				object o=Context.Invoke(args);

				return (T)o;
			}
		}

		public interface ITest
		{
			int Add(int a, int b);
		}

		/// <summary>
		/// Say hello to the world.
		/// </summary>
		public class TestClass:ITest
		{
			public virtual int Add(int a, int b) 
			{
				return a+b;
			}
		}

		[TestMethod]
		public void FailedCall()
		{
			ITest hw = (ITest)Loom.Weaver.CreateInstance(typeof(TestClass), null, new TestAspect());
			try
			{
				hw.Add(6,5);
			}
			catch(Exception e) // n�tig durch einen Fehler in NUnit (Asserts in Subroutinen werden falsch durchgereicht);
			{
				Assert.Fail(e.Message);
			}
		}
	}

	/// <summary>
	/// Aufgenommen: 19.11.03
	/// Von: Kai K�hne
	/// Versionsstand: 1.01.36.0
	/// Diagnose: Context.Invoke + Context.InvokeOn -> invalid paramcount bei "params" Definitionen
	/// Wenn ein Parameterwildcard mit einer Methode gleicher Signatur verwoben wurde, dann wurden die
	/// Parameter nicht richtig verpackt.
	/// </summary>
	[TestClass]
	public class Bug0002
	{
		public class TestAspect:AspectAttribute
		{
			[Call(Advice.Around)] [IncludeAll]
            public T f<T>([JPContext] Context Context, params object[] args)
			{
				// invoke original function
				object o=Context.Invoke(args);

				return (T)o;
			}
		}

		public interface ITest
		{
			int Add(params object[] args);
		}

		/// <summary>
		/// Say hello to the world.
		/// </summary>
		public class TestClass:ITest
		{
			public int Add(params object[] args)
			{
				int res=0;
				foreach(object o in args)
					res+=Convert.ToInt32(o);
				return res;
			}
		}

		[TestMethod]
		public void FailedCall()
		{
			ITest hw = (ITest)Weaver.CreateInstance(typeof(TestClass), null, new TestAspect());
			try
			{
				hw.Add(6,5,7,8);
			}
			catch(Exception e) // n�tig durch einen Fehler in NUnit (Asserts in Subroutinen werden falsch durchgereicht);
			{
				System.Text.StringBuilder s=new System.Text.StringBuilder();
				for(Exception ex=e;ex!=null;ex=ex.InnerException)
				{
					s.Append(ex.Message);
					s.Append(" ");
				}
				Assert.Fail(s.ToString());
			}
		}
	}

	/// <summary>
	/// Aufgenommen: 04.12.03
	/// Von: Kai K�hne
	/// Versionsstand: 1.02.38.0
	/// Diagnose: Beim mehrfachen Verweben von Aspekten k�nnen virtuelle Methoden nicht abgeleitet werden
	/// "internal error" bei Create Instance
	/// </summary>
	[TestClass]
	public class Bug0003
	{
		public class DummyAspect:AspectAttribute
		{
			[Call(Advice.Around)]
			[IncludeAll]
            public T f<T>([JPContext] Context Context, params object[] args)
			{
				// invoke original function
				object o=Context.Invoke(args);

				return (T)o;
			}
		}

		public class TestAspect:AspectAttribute
		{
			[Call(Advice.Around)] 
			[IncludeAll]
            public T f<T>([JPContext] Context Context, params object[] args)
			{
				// invoke original function
				object o=Context.Invoke(args);

				return (T)o;
			}
		}

		public interface ITest
		{
			int Add(params object[] args);
		}

		/// <summary>
		/// Say hello to the world.
		/// </summary>
		public class TestClass:ITest
		{
			public virtual int Add(params object[] args)
			{
				int res=0;
				foreach(object o in args)
					res+=Convert.ToInt32(o);
				return res;
			}
		}

		[TestMethod]
		public void FailedCall()
		{
			ITest hw = (ITest)Weaver.CreateInstance(typeof(TestClass), null, new AspectAttribute[]{new TestAspect(), new DummyAspect()});
			try
			{
				hw.Add(6,5,7,8);
			}
			catch(Exception e) // n�tig durch einen Fehler in NUnit (Asserts in Subroutinen werden falsch durchgereicht);
			{
				System.Text.StringBuilder s=new System.Text.StringBuilder();
				for(Exception ex=e;ex!=null;ex=ex.InnerException)
				{
					s.Append(ex.Message);
					s.Append(" ");
				}
				Assert.Fail(s.ToString());
			}
		}
	}

	/// <summary>
	/// Aufgenommen: 29.03.04
	/// Von: Kai K�hne
	/// Versionsstand: 1.20.50.2
	/// Bei Funktionen mit out- und ref-übergabeparametern die Advice.Instead verwoben werden 
	/// kommen �nderungen bei den Parametern der Zielmethode nicht an
	/// Grund: Es wird ein Objekt-Array gebaut, welches die Parameter h�lt und die �nderungen
	/// werden von dort nicht zurückkopiert.
	/// </summary>
	[TestClass]
	public class Bug0004
	{
		public class Trace:AspectAttribute
		{
			[IncludeAll()]
			[Call(Advice.Around)]
            public T f<T>([JPContext] Context Context, params object[] args)
			{
				object result=Context.Invoke(args);
				return (T)result;
			}
		}

		public enum result{undefined, ok, notok};

		public interface IBase
		{
			void change(ref result r);
			void change2(out result r2); 
		}

		public class Base:IBase
		{
			public Base()
			{}

			public void change(ref result r)
			{
				r=result.ok;
			}

			public void change2(out result r2)
			{
				r2=result.ok;
			}
		}

		[TestMethod]
		public void Failure1()
		{
			Trace t=new Trace();
			IBase a=(IBase)Weaver.CreateInstance(typeof(Base), null, t);
			
			result res=result.notok;
			a.change(ref res);
			Assert.AreEqual(result.ok,res,"result hasn't changed");
		}

		[TestMethod]
		public void Failure2()
		{
			Trace t=new Trace();
			IBase a=(IBase)Weaver.CreateInstance(typeof(Base), null, t);
			
			result res=result.notok;
			a.change2(out res);
			Assert.AreEqual(result.ok,res,"result hasn't changed");
		}
	}

	/// <summary>
	/// Aufgenommen: 30.06.04
	/// Von: Kai K�hne (E-Mail von Matthew T. Clarkson 24.05.04)
	/// Versionsstand: 1.20.106.0
	/// Bei Verwebung von Properties wird IncludeAll Attribut ignoriert.
	/// </summary>
	[TestClass]
	public class Bug0005
	{
		static int fGetCalled;
		static int fSetCalled;

		[TestInitialize]
		public void SetUp()
		{
			fGetCalled = 0;
			fSetCalled = 0;
		}

		public class Trace:AspectAttribute
		{
     

            [Access(Advice.Before)]
            [IncludeAll]
            public void set_f(bool value)
            {
                fSetCalled++;
            }

            [Access(Advice.Before)]
            [IncludeAll]
            public void get_f()
            {
                fGetCalled++;
            }



		}

		public interface IBase
		{
			bool Target
			{
				get ;
				set	;
			}
		}

		public class Base:IBase
		{
			bool _target = false;
			public bool Target
			{
				get
				{
					return _target;
				}
				set
				{
					_target = value;
				}
			}
		}

		[TestMethod]
		public void FailedCall()
		{
			Trace t=new Trace();
			//IBase a=(IBase)Weaver.CreateInstance(typeof(Base), null, t);
            IBase a = Weaver.Create<Base>(t);

			a.Target = true;
			Assert.AreEqual( true, a.Target,"pitarget attribute set/get wasn't called");
			Assert.AreEqual( 1, fGetCalled,"weaver attribute get wasn't called");
			Assert.AreEqual( 1, fSetCalled,"weaver attribute set wasn't called");
		}
	}
	/// <summary>
	/// Aufgenommen: 24.09.04
	/// Von: Janin Jeske
	/// Versionsstand: 1.20.106.0
	/// Verwebung einer Klasse mit einer Methode mit R�ckgabewert Integer aber keinem übergabeparameter
	/// mit einem Aspekt mit einer Aspektmethode mit Verbindungspunkt Loom.Call(Advice.AfterReturning) schl�gt fehl. 
	/// </summary>
	[TestClass]
	public class Bug0006
	{
		public static int i=0;
		public class Aspect:AspectAttribute
		{
			[Call(Advice.AfterReturning)]
			[IncludeAll]
			public void method([JPRetVal] int retval)
			{
				i=1;
			}
		}
		public interface IBase
		{
			int method();
		}
		public class Base:IBase
		{
			public Base()
			{}

			public int method()
			{
				return 1;
			}
		}
		[TestMethod]
		public void Failure()
		{
			Aspect a=new Aspect();
            IBase b = Weaver.Create<Base>(a);			
			b.method();
			Assert.AreEqual(1,i,"aspectmethod not called");
		}
	}
	/// <summary>
	/// Aufgenommen: 24.09.2004
	/// Von: Janin Jeske
	/// Versionsstand: 1.20.106.0
	/// Bei der Verwebung einer Klasse mit mehreren Aspekten im Aspekt-Array mit dem Verbindungspunkt
	/// Loom.Create(Advice.Around) werden die Aspekte nicht in der Reihenfolge wie im Array angegeben verwoben.
	/// </summary>
	[TestClass]
	public class Bug0007
	{	
		public static int order=0;

        [TestInitialize]
        public void SetUp()
        {
            order = 0;
        }

		public class Aspectbase:AspectAttribute
		{
			public int i=0;
		}
		public class Aspect1:Aspectbase
		{
			[Create(Advice.Around)]
            [IncludeAll]
            public T method<T>([JPContext] Context Context, params object[] args)
			{	
				i=++order;
				object o=Context.Invoke(args);
				return (T)o;
			}
		}
		public class Aspect2:Aspectbase
		{
			[Create(Advice.Around)]
            [IncludeAll]
            public T method2<T>([JPContext] Context Context, params object[] args)
			{
				i=++order;	
				object o=Context.Invoke(args);
				return (T)o;
			}
		}
		public class Base
		{
			public Base()
			{}
		}
		[TestMethod]
		public void Failure()
		{
			Aspect1 a1=new Aspect1();
			Aspect2 a2=new Aspect2();
			Loom.AspectAttribute[] aspects=new AspectAttribute[2];
			aspects[0]=a1;
			aspects[1]=a2;
			Base b=(Base)Weaver.CreateInstance(typeof(Base),null,aspects);
			// Bei Instead wird der zuletzt verwobene zuerst aufgerufen
			Assert.AreEqual(1,a2.i,"Aspect array jpinterwoven in wrong order"); 
			Assert.AreEqual(2,a1.i,"Aspect array jpinterwoven in wrong order");
		}
	}

	/// <summary>
	/// Aufgenommen: 12.8.05
	/// Von: Wolfgang Schult
	/// Versionsstand 1.50.401.0
	/// Bei einer Advice.AfterReturning Verwebung mit Wildcards und ref Parametern, wird 
	/// der ref-Parameter nicht richtig zurückgeschrieben
	/// </summary>
	[TestClass]
	public class Bug0008
	{
		public class TestAspect:AspectAttribute
		{
			[Call(Advice.AfterReturning)]
			[IncludeAll]
			public void AfterReturning(object[] args)
			{
			}
		}

		[TestAspect]
		public class TestClass
		{
			public virtual void Test1(ref Decimal d)
			{
				d++;
			}

			public virtual void Test2(ref Decimal d)
			{
				d++;
				Test1(ref d);
				d++;
			}
		}

		[TestMethod]
		public void Failure()
		{
			TestClass t=(TestClass)Weaver.CreateInstance(typeof(TestClass));
			//Weaver.Saveprovider();
			Decimal d=0;
			t.Test1(ref d);
			Assert.AreEqual(1,d);
			t.Test2(ref d);
			Assert.AreEqual(4,d);
		}
	}



    /// <summary>
    /// Aufgenommen: 07.02.2007
    /// Von: Benjamin Sch�ler
    /// Versionsstand 2.0.0.600
    /// Advices mit AfterReturning k�nnen mittels return den R�ckgabewert der Zielmethode nicht überschreiben.
    /// </summary>
    [TestClass]
    public class Bug0009
    {
        public class TestAspect : AspectAttribute
        {
            [Call(Advice.AfterReturning)]
            [IncludeAll]
            public void AfterReturning([JPRetVal] ref int returnValue, params object[] parameters)
            {
                returnValue = 1;
            }
        }


        public interface IBase
        {
            int Test1(string test);
        }
        
        public class TestClass : IBase
        {
            public virtual int Test1(string test)
            {
                return 0;   
            }
        }

        [TestMethod]
        public void Failure()
        {
            TestAspect a = new TestAspect();
            TestClass t = Weaver.Create<TestClass>(a);

            Assert.AreEqual(1, t.Test1(""), "Return value in Aspect was not overwritten.");
        }
    }

    /// <summary>
    /// Aufgenommen: 29.05.2007
    /// Von: Benjamin Sch�ler
    /// Versionsstand 2.0.830.0
    /// Klassen mit generischem Parameter werden erfolgreich mit Aspekten und Kontext verwoben.
    /// Klassen ohne generische Parameter, sondern nur mit generischem Parameter in der Zielmethode werden 
    /// im einfachen Fall zwar verwoben, jedoch nicht mit dem JPContext Attribut.
    /// </summary>
    [TestClass]
    public class Bug0010
    {
        public class BeforeAspect : Aspect
        {
            [Call(Advice.Before)]
            [Include("Test")]
            public void Test(params object[] args)
            {

            }
        }

        public class BeforeAspectWithContext : Aspect
        {
            [Call(Advice.Before)]
            [Include("Test")]
            public void Test([JPContext] Context ctx, params object[] args)
            {

            }
        }


        public class TestGenericMethod
        {
            public virtual void Test<T>(T i)
            {
            }
        }

        public class TestGenericClass<T>
        {
            public virtual void Test(T i)
            {
            }
        }

        [TestMethod]
        public void Failure()
        {
            TestGenericClass<int> t = Weaver.Create<TestGenericClass<int>>(new BeforeAspect());
            t.Test(1);

            TestGenericMethod t2 = Weaver.Create<TestGenericMethod>(new BeforeAspect());
            t2.Test<int>(1);

            TestGenericClass<int> t3 = Weaver.Create<TestGenericClass<int>>(new BeforeAspectWithContext());
            t3.Test(1);

            TestGenericMethod t4 = Weaver.Create<TestGenericMethod>(new BeforeAspectWithContext());
            t4.Test<int>(1);
        }
    }

    /// <summary>
    /// Aufgenommen: 05.09.2007
    /// Von: Benjamin Sch�ler
    /// Versionsstand 2.2.980.0
    /// Eine Aspektmethode die mittels des new operators überschrieben wird, ruft nach dem eigneen aufruf 
    /// noch die bereits überschriebene originalmethode der elternklasse auf.
    /// </summary>
    [TestClass]
    public class Bug0011
    {
        public class MyAspect1 : Aspect
        {
            public bool f1 = false;
            public bool f2 = false;
            [IncludeAll]
            [Call(Advice.Before)]
            public virtual void foo()
            {
                throw new ApplicationException("Overriden method was called.");
            }

            [IncludeAll]
            [Call(Advice.Before)]
            public virtual void foo2()
            {
                f1 = true;
            }
        }

        public class MyAspect2 : MyAspect1
        {
            [IncludeAll]
            [Call(Advice.Before)]
            public override void foo()
            {
                f2 = true;
            }

        }

        public class TargetClass
        {
            public virtual void foo()
            {
            }
        }

        [TestMethod]
        public void Failure()
        {
            var a=new MyAspect2();
            var c=Weaver.Create<TargetClass>(a);
            c.foo();
            Assert.IsTrue(a.f1);
            Assert.IsTrue(a.f2);
        }
    
    }
     
}
