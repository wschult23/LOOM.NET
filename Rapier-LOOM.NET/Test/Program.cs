// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Loom;
using Loom.JoinPoints;
using System.Threading;
using Loom.AspectProperties;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using Loom.UnitTests.CreateWeaving;
using System.Data;

namespace Test
{

   

    public interface IF1
    {
        void Test<T>();
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class IntroduceAspectOnMethod : AspectAttribute
    {
        [Introduce(typeof(IF1),ExistingInterfaces.Advice)]
        public static void Test([JPContext] Context Context)
        {
            Context.Call();
        }
    }

    

    [CreateAspect(Per.Annotation)]
    public class MyAspect:AspectAttribute
    {
        public int i;
        public int r;
        static int r1 = 0;

        public MyAspect(int i)
        {
            this.i = i;
            this.r = r1++;
        }

        [Call(Advice.Before)]
        public void Test([JPContext] Context ctx)
        {
            Console.WriteLine("Before");
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    //[MyAspect(42)]
    [MyAspect2]
    public class A : IF1
    {
        void IF1.Test<T>()
        {
        }

        public virtual void bar()
        {
        }

        public virtual IEnumerable baz()
        {
            throw new Exception();
        }
    }

                public class MyAspect2 : AspectAttribute
                {


                    [IncludeAll]
                    [Call(Advice.Around)]
                    public T Trace<T>([JPContext]                 Context ctx,
                                      [JPVariable(Scope.Call)]    ref bool gate,
                                                                  params object[] args) where T : IEnumerable
                    {


                        return (T)Enumerable.Empty<object>();
                    }

                }

                public class DataReaderFacade
                {
                    protected IDictionary target;

                    public class IDictionaryAspect: Aspect
                    {
                        [Introduce(typeof(IDictionary)), IncludeAll]
                        public T Introduce<T>([JPContext] Context ctx, [JPVariable(Scope.Virtual)] ref IDictionary target, params object[] args)
                        {
                            return (T)ctx.InvokeOn(target, args);
                        }

                        /*[Introduce(typeof(ICollection)), IncludeAll]
                        public T Introduce2<T>([JPContext] Context ctx, [JPVariable(Scope.Virtual)] ref IDictionary target, params object[] args)
                        {
                            return (T)ctx.InvokeOn(target, args);
                        }
                        
                        [Introduce(typeof(IEnumerable)), IncludeNonInterwoven]
                        public T Introduce3<T>([JPContext] Context ctx, [JPVariable(Scope.Virtual)] ref IDictionary target, params object[] args)
                        {
                            return (T)ctx.InvokeOn(target, args);
                        } */
                    }

                    public DataReaderFacade(IDictionary target)
                    {
                        this.target = target;
                    }

                    public static IDictionary Create(IDictionary target)
                    {
                        return (IDictionary)Weaver.Create<DataReaderFacade>(new IDictionaryAspect(), target);
                    }
                }

    
    class HelloLoom
    {
        //static MyAspect asp1 = (MyAspect)typeof(A).GetCustomAttributes(typeof(MyAspect), true)[0];
        //static MyAspect asp2 = (MyAspect)typeof(A).GetMethod("foo").GetCustomAttributes(typeof(MyAspect), true)[0];
        static int i = 42;

        static void Main(string[] args)
        {
            //var a = Weaver.Create<A>();
            //a.bar();
            //a.baz();
            //a.Test<int>();
            //var test = new Loom.UnitTests.GenericsTest.MyTypeTest();
            //test.SetUp();
            //test.GenericsAfterReturningAspect();
            var d = new Dictionary<string, object>();
            var t= DataReaderFacade.Create(d);
            t.Add("Test", 42);

        }
    }
}
