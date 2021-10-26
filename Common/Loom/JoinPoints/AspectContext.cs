// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Reflection;

namespace Loom.JoinPoints
{
    /// <summary>
    /// An object of this class represents the currently executed action on the target class. 
    /// </summary>
    /// <remarks>
    /// If your aspect method is interwoven with the <see cref="Advice">Advice.Around</see> parameter, you need this context to have control over the target class object. 
    /// To obtain a context object use the <see cref="JPContextAttribute"/> and define a join-point parameter in your aspect method.
    /// </remarks>
    /// <include file="Doc/JoinPointAttributes.xml" path="doc/JoinPointParameterExample/*"/>
    /// <seealso cref="JoinPointParameterAttribute"/>
    /// <seealso cref="JPContextAttribute"/>
    [Serializable]
    public abstract class Context
    {
        /// <summary>
        ///	Invokes the underlying method represented by this Context object with the specified parameters.
        /// </summary>
        /// <returns>an object containing the return value of the invoked method</returns>
        /// <exception cref="Loom.MethodInvocationException">Invocation is not allowed in this context.</exception>
        /// <remarks>
        /// You should call this method only in the context of <see cref="Advice">Advice.Around</see> join-points.
        /// </remarks>
        /// <example>
        /// This example shows how to use Context.Call():
        /**
            <code>
            public class TargetClass
            {
                public virtual void foo() 
                {
                    Console.WriteLine("foo");
                }

                public virtual int foo(int i)
                {
                    return i+1;
                }

                public virtual string foo(string s)
                {
                    return s + "bar";
                }
            }
            
           public class CallAspect : Aspect
            {
                [IncludeAll]
                [Call(Advice.Around)]
                public T foo&lt;T&gt;([JPContext] Context ctx, int i)
                {
                    Console.WriteLine("Now calling {0}.", ctx.CurrentMethod);
                    return (T)ctx.Call(i);    
                }
            }
        
            ...
            TargetClass a = Weaver.Create&lt;TargetClass&gt;(new CallAspect());

            a.foo();
            Console.WriteLine(a.foo(1));
            Console.WriteLine(a.foo("foo"));
            ...
     
     
            [Output]
            Now calling Int32 foo(Int32).
            2
            foobar
            </code> */
        /// </example>
        public object Call()
        {
            return Invoke(Type.EmptyTypes);
        }

        /// <summary>
        ///	Invokes the underlying method represented by this Context object with the specified parameters.
        /// </summary>
        /// <param name="arg1">the method's first argument</param>
        /// <typeparam name="A1">the type of the first argument</typeparam>
        /// <returns>An object containing the return value of the invoked method.</returns>
        /// <exception cref="MethodInvocationException">Invocation is not allowed in this context.</exception>
        /// <remarks>
        /// You should call this method only in the context of <see cref="Loom.JoinPoints.Advice.Around">Advice.Around</see> join-points.
        /// </remarks>
        /// <example>
        /// This example shows how to use Context.Call():
        /**
            <code>
            public class TargetClass
            {
                public virtual void foo() 
                {
                    Console.WriteLine("foo");
                }

                public virtual int foo(int i)
                {
                    return i+1;
                }

                public virtual string foo(string s)
                {
                    return s + "bar";
                }
            }
            
           public class CallAspect : Aspect
            {
                [IncludeAll]
                [Call(Advice.Around)]
                public T foo&lt;T&gt;([JPContext] Context ctx, int i)
                {
                    Console.WriteLine("Now calling {0}.", ctx.CurrentMethod);
                    return (T)ctx.Call(i);    
                }
            }
        
            ...
            TargetClass a = Weaver.Create&lt;TargetClass&gt;(new CallAspect());

            a.foo();
            Console.WriteLine(a.foo(1));
            Console.WriteLine(a.foo("foo"));
            ...
     
     
            [Output]
            Now calling Int32 foo(Int32).
            2
            foobar
            </code> */
        /// </example>
        public object Call<A1>(A1 arg1)
        {
            object[] args = new object[] { arg1 };
            return Invoke(args);
        }
        /// <summary>
        ///	Invokes the underlying method represented by this Context object with the specified parameters.
        /// </summary>
        /// <param name="arg1">the method's first argument</param>
        /// <param name="arg2">the method's second argument</param>
        /// <typeparam name="A1">the type of the first argument</typeparam>
        /// <typeparam name="A2">the type of the second argument</typeparam>
        /// <returns>An object containing the return value of the invoked method.</returns>
        /// <exception cref="MethodInvocationException">Invocation is not allowed in this context.</exception>
        /// <remarks>
        /// You should call this method only in the context of <see cref="Loom.JoinPoints.Advice.Around">Advice.Around</see> join-points.
        /// </remarks>
        /// <example>
        /// This example shows how to use Context.Call():
        /**
            <code>
            public class TargetClass
            {
                public virtual void foo() 
                {
                    Console.WriteLine("foo");
                }

                public virtual int foo(int i)
                {
                    return i+1;
                }

                public virtual string foo(string s)
                {
                    return s + "bar";
                }
            }
            
           public class CallAspect : Aspect
            {
                [IncludeAll]
                [Call(Advice.Around)]
                public T foo&lt;T&gt;([JPContext] Context ctx, int i)
                {
                    Console.WriteLine("Now calling {0}.", ctx.CurrentMethod);
                    return (T)ctx.Call(i);    
                }
            }
        
            ...
            TargetClass a = Weaver.Create&lt;TargetClass&gt;(new CallAspect());

            a.foo();
            Console.WriteLine(a.foo(1));
            Console.WriteLine(a.foo("foo"));
            ...
     
     
            [Output]
            Now calling Int32 foo(Int32).
            2
            foobar
            </code> */
        /// </example>
        public object Call<A1, A2>(A1 arg1, A2 arg2)
        {
            object[] args = new object[] { arg1, arg2 };
            return Invoke(args);
        }
        /// <summary>
        ///	Invokes the underlying method represented by this Context object with the specified parameters.
        /// </summary>
        /// <param name="arg1">the method's first argument</param>
        /// <param name="arg2">the method's second argument</param>
        /// <param name="arg3">the method's third argument</param>
        /// <typeparam name="A1">the type of the first argument</typeparam>
        /// <typeparam name="A2">the type of the second argument</typeparam>
        /// <typeparam name="A3">the type of the third argument</typeparam>
        /// <returns>An object containing the return value of the invoked method.</returns>
        /// <exception cref="MethodInvocationException">Invocation is not allowed in this context.</exception>
        /// <remarks>
        /// You should call this method only in the context of <see cref="Loom.JoinPoints.Advice.Around">Advice.Around</see> join-points.
        /// </remarks>
        /// <example>
        /// This example shows how to use Context.Call():
        /**
            <code>
            public class TargetClass
            {
                public virtual void foo() 
                {
                    Console.WriteLine("foo");
                }

                public virtual int foo(int i)
                {
                    return i+1;
                }

                public virtual string foo(string s)
                {
                    return s + "bar";
                }
            }
            
           public class CallAspect : Aspect
            {
                [IncludeAll]
                [Call(Advice.Around)]
                public T foo&lt;T&gt;([JPContext] Context ctx, int i)
                {
                    Console.WriteLine("Now calling {0}.", ctx.CurrentMethod);
                    return (T)ctx.Call(i);    
                }
            }
        
            ...
            TargetClass a = Weaver.Create&lt;TargetClass&gt;(new CallAspect());

            a.foo();
            Console.WriteLine(a.foo(1));
            Console.WriteLine(a.foo("foo"));
            ...
     
     
            [Output]
            Now calling Int32 foo(Int32).
            2
            foobar
            </code> */
        /// </example>
        public object Call<A1, A2, A3>(A1 arg1, A2 arg2, A3 arg3)
        {
            object[] args = new object[] { arg1, arg2, arg3 };
            return Invoke(args);
        }
        /// <summary>
        ///	Invokes the underlying method represented by this Context object with the specified parameters.
        /// </summary>
        /// <param name="arg1">the method's first argument</param>
        /// <param name="arg2">the method's second argument</param>
        /// <param name="arg3">the method's third argument</param>
        /// <param name="argsn">the method's last arguments</param>
        /// <typeparam name="A1">the type of the first argument</typeparam>
        /// <typeparam name="A2">the type of the second argument</typeparam>
        /// <typeparam name="A3">the type of the third argument</typeparam>
        /// <returns>An object containing the return value of the invoked method.</returns>
        /// <exception cref="MethodInvocationException">Invocation is not allowed in this context.</exception>
        /// <remarks>
        /// You should call this method only in the context of <see cref="Loom.JoinPoints.Advice.Around">Advice.Around</see> join-points.
        /// </remarks>
        /// <example>
        /// This example shows how to use Context.Call():
        /**
            <code>
            public class TargetClass
            {
                public virtual void foo() 
                {
                    Console.WriteLine("foo");
                }

                public virtual int foo(int i)
                {
                    return i+1;
                }

                public virtual string foo(string s)
                {
                    return s + "bar";
                }
            }
            
           public class CallAspect : Aspect
            {
                [IncludeAll]
                [Call(Advice.Around)]
                public T foo&lt;T&gt;([JPContext] Context ctx, int i)
                {
                    Console.WriteLine("Now calling {0}.", ctx.CurrentMethod);
                    return (T)ctx.Call(i);    
                }
            }
        
            ...
            TargetClass a = Weaver.Create&lt;TargetClass&gt;(new CallAspect());

            a.foo();
            Console.WriteLine(a.foo(1));
            Console.WriteLine(a.foo("foo"));
            ...
     
     
            [Output]
            Now calling Int32 foo(Int32).
            2
            foobar
            </code> */
        /// </example>
        public object Call<A1, A2, A3>(A1 arg1, A2 arg2, A3 arg3, params object[] argsn)
        {
            object[] args = new object[3 + argsn.Length];
            args[0] = arg1;
            args[1] = arg2;
            args[2] = arg3;
            argsn.CopyTo(args, 3);
            return Invoke(args);
        }

        /// <summary>
        ///	Invokes the target method with the specified parameters.
        /// </summary>
        /// <param name="args">an array of arguments that match in number, order, and type the parameters of the interwoven method</param>
        /// <returns>An object containing the return value of the invoked method.</returns>
        /// <exception cref="MethodInvocationException">Invocation is not allowed in this context.</exception>
        /// <remarks>
        /// You should call this method only in the context of <see cref="Loom.JoinPoints.Advice.Around">Advice.Around</see> join-points. 
        /// Use this method to proceed the execution of the target method, if your aspect method receives the arguments in an object array.
        /// </remarks>
        /// <example>
        /// This example shows how to use Context.Invoke():
        /**
            <code>
            public class TargetClass
            {
                public virtual void foo() 
                {
                    Console.WriteLine("foo");
                }

                public virtual int foo(int i)
                {
                    return i+1;
                }

                public virtual string foo(string s)
                {
                    return s + "bar";
                }
            }
            
            public class InvokeAspect : Aspect
            {
                [IncludeAll]
                [Call(Advice.Around)]
                public T catchAll&lt;T&gt;([JPContext] Context ctx, params object[] args)
                {
                    Console.WriteLine("Now calling {0}.", ctx.CurrentMethod);
                    return (T)ctx.Invoke(args);
                }
            }
        
            ...
            TargetClass a = Weaver.Create&lt;TargetClass&gt;(new InvokeAspect());

            a.foo();
            Console.WriteLine(a.foo(1));
            Console.WriteLine(a.foo("foo"));
            ...
     
     
            [Output]
            Now calling Void foo().
            foo
            Now calling Int32 foo(Int32).
            2
            Now calling System.String foo(System.String).
            foobar
            </code> */
        /// </example>
        public virtual object Invoke(object[] args)
        {
            throw new MethodInvocationException(Resources.Errors.ERR_9001);
        }


        /// <summary>
        ///	Invokes the target method on a different instance. 
        /// </summary>
        /// <param name="target">points to an existing target class object</param>
        /// <param name="args">an array of arguments that match in number, order, and type the parameters of the interwoven method</param>
        /// <returns>An object containing the return value of the invoked method.</returns>
        /// <exception cref="MethodInvocationException">Invocation is not allowed in this context.</exception>
        /// <remarks>
        /// <para>
        /// You should call this method only in the context of <see cref="Loom.JoinPoints.Advice.Around">Advice.Around</see> join-points. 
        /// </para>
        /// <para>
        /// If the target method was called via an interface, <paramref name="target"/> 
        /// must implement the same interface. Otherwise <paramref name="target"/> must be a target class instance.
        /// </para>        /// </remarks>
        /// <example>
        /// This example shows how to use Context.InvokeOn():
        /**
            <code>
            public interface IFoo 
            { 
                void foo(int i); 
            } 

            public class A:IFoo 
            { 
                public void foo(int i) 
                { 
                    Console.WriteLine("A called"); 
                } 
            } 
            public class B:IFoo 
            { 
                public void foo(int i) 
                { 
                    Console.WriteLine("B called"); 
                } 
            } 

            public class MyAspect : Aspect 
            { 
                private B b; 

                [Create(Advice.Before)] 
                public void create() 
                { 
                    b = new B(); 
                } 

                [Call(Advice.Around)] 
                [Include(typeof(IFoo))] 
                public T foo&lt;T&gt;([JPContext] Context ctx, params object[] args) 
                { 
                    return (T)ctx.InvokeOn(b, args); 
                } 
            } 

            ...
            IFoo a = Weaver.Create&lt;A&gt;(new MyAspect()); 
            a.foo(42);
            ...
            </code> */
        /// </example>
        public virtual object InvokeOn(object target, object[] args)
        {
            throw new Loom.MethodInvocationException(Resources.Errors.ERR_9001);
        }

        /// <summary>
        /// Restarts the invocation of the target method with different arguments.
        /// </summary>
        /// <remarks>
        /// The target method must have an overloaded version where the parameter signature matches the given arguments.
        /// Otherwise a <see cref="MissingMethodException"/> will be thrown.
        /// </remarks>
        /// <param name="args">arguments for the target method</param>
        /// <returns>the result of the invoked target method</returns>
        /// <exception cref="MissingMethodException">No matching public method was found.</exception>
        /// <example>
        /// This example shows how to use Context.ReCall():
        /**
            <code>
            public class TargetClass
            {
                public virtual void foo(int i)
                {
                    Console.WriteLine("foo({0})",i);
                }

                public virtual void foo(string s)
                {
                    Console.WriteLine("foo !\"{0}\"!",i);
                }
            }
            
           public class ReCallAspect : Aspect
           {
                [Call(Advice.Around)]
                public void foo([JPContext] Context ctx, int i)
                {
                    ctx.ReCall(i.ToString());
                }
            }
        
            ...
            TargetClass a = new TargetClass();

            a.foo(42);
            a.foo("bar");

            a = Weaver.Create&lt;TargetClass&gt;(new ReCallAspect());

            a.foo(42);
            a.foo("bar");
            ...
     
     
            [Output]
            foo(42)
            foo !"bar"!
                 
            foo !"42"!
            foo !"bar"!
            </code> */
        /// </example>
        public virtual object ReCall(params object[] args)
        {
            throw new Loom.MethodInvocationException(Resources.Errors.ERR_9001);
        }

        /// <summary>
        /// Restarts the Invocation of the target method with different arguments on a different instance.
        /// </summary>
        /// <remarks>
        /// The target method must have an overloaded version where the parameter signature matches the given arguments.
        /// Otherwise a <see cref="MissingMethodException"/> will be thrown.
        /// </remarks>
        /// <exception cref="MissingMethodException">No matching public method was found.</exception>
        /// <param name="target">points to an existing target class object</param>
        /// <param name="args">arguments for the target method</param>
        /// <returns>the result of the invoked target method</returns>
        public virtual object ReCallOn(object target, params object[] args)
        {
            throw new Loom.MethodInvocationException(Resources.Errors.ERR_9001);
        }

        /// <summary>
        /// Gets the original <see cref="System.Type"/> of the current interwoven instance.
        /// </summary>
        public abstract Type TargetClass { get; }

        /// <summary>
        /// Gets the <see cref="System.Type"/> of the current interwoven instance.
        /// </summary>
        public abstract Type InterwovenClass { get; }

        /// <summary>
        /// Returns a <see cref="System.Reflection.MethodBase"/> object representing the currently executing interwoven target class method.
        /// </summary>
        public abstract MethodBase CurrentMethod { get; }

        /// <summary>
        /// Indicates if the <see cref="Context"/> has an underlying join-point.
        /// </summary>
        /// <remarks>
        /// If the <see cref="Context"/> object has no underlying join-point, all calls to <see cref="O:Call"/> and/or <see cref="O:Invoke"/> will fail. This is true,
        /// if your aspect introduces new methods on a target class.
        /// </remarks>
        /// <returns>true: if ther s a join-point, false otherwise</returns>
        public virtual bool HasJoinPoint()
        {
            return false;
        }

        /// <summary>
        /// Returns the currently interwoven instance. 
        /// </summary>
        /// <exception cref="MethodInvocationException">Advice is not allowed in this context.</exception>
        public virtual object Instance
        {
            get
            {
                throw new Loom.MethodInvocationException(Resources.Errors.ERR_9001);
            }
        }

        /// <summary>
        /// Returns the type of each parameter for the interwoven target class method.
        /// </summary>
        /// <remarks>
        /// If the target class method contains generic parameters, the resulting type will be the closed (bound) type of this generic parameter.
        /// Use <see cref="Context.CurrentMethod"/> and <see cref="System.Reflection.MethodBase.GetParameters"/> to obtain the open (unbound) type.
        /// </remarks>
        /// <returns>a type array</returns>
        public abstract Type[] GetParameterTypes();

        /// <summary>
        /// Gets the return type of the interwoven target class method method. 
        /// </summary>
        /// <remarks>
        /// If return type of the target class method is a generic type, this property will return the closed (bound) type.
        /// Use <see cref="Context.CurrentMethod"/> and <see cref="System.Reflection.MethodInfo.ReturnType"/> to obtain the open (unbound) type.
        /// </remarks>
        public abstract Type ReturnType { get; }

        /// <summary>
        /// tag store
        /// </summary>
        protected object tag;
        #region help
        /// <summary>
        /// Gets or sets an object that contains data to associate with the context.
        /// </summary>
        /// <value>
        /// An object that contains information that is associated with the context.
        /// </value>
        /// <remarks>
        /// The <see cref="Tag"/> property can be used to store any object that you want to associate with a context. Usually
        /// the <see cref="Tag"/> is used to share information between several aspect calls within the same context.
        /// </remarks>
        /// <example>
        /**
            <code>
            public class Trace : Aspect
            {
                private int counter;
         
                [IncludeAll]
                [Call(Advice.Before)] 
                public void f([JPContext] Context ctx, params object[] args)
                {
                    System.Diagnostics.Debug.WriteLine("{0} Before: {1} was called.", counter, ctx.MethodName);
                    ctx.Tag=counter++;
                }
            	
                [IncludeAll]
                [Call(Advice.After)] 
                public void f([JPContext] Context ctx, params object[] args)
                {
                    System.Diagnostics.Debug.WriteLine("{0} After: {1} was called.", Context.Tag, ctx.MethodName);
                }
            }
            </code>*/
        /// </example>
        #endregion
        public object Tag
        {
            get
            {
                return tag;
            }
            set
            {
                tag = value;
            }
        }


        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="Context"/>.
        /// </summary>
        /// <returns>a <see cref="System.String"/> representing the current method</returns>
        public override string ToString()
        {
            return Common.Convert.ToString(CurrentMethod);
        }

    }
}
