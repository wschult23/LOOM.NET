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
using System.Threading.Tasks;
using Loom.Runtime;

namespace Loom
{
    /// <summary>
    /// Interface to create interwoven instances of a given type via a class factory.
    /// </summary>
    /// <remarks>
    /// With a <see cref="ClassFactory{T}"/> you have efficient lock free mechanism to instantiate a huge amount of interwoven objects of the same type. 
    /// You can obtain a calss factory by using the <see cref="O:Loom.Weaver.GetClassFactory"/> method.
    /// </remarks>
    public interface IClassFactory
    {
        /// <summary>
        /// Creates an instance of an interwoven target class represented by this factory.
        /// </summary>
        /// <param name="args">An array of arguments that match in number, order, and type the parameters of the constructor to invoke. If <paramref name="args"/> is an empty array or <value>NULL</value>, the constructor that takes no parameters (the default constructor) is invoked.</param>
        /// <returns>An instance of the newly created object with potentially interwoven aspects.</returns>
        object Create(params object[] args);
    }

    /// <summary>
    /// Interface to create interwoven instances of a given <typeparamref name="T"/> via a class factory.
    /// </summary>
    /// <typeparam name="T">The type of the interwoven object to create.</typeparam>
    /// <remarks>
    /// With a <see cref="ClassFactory{T}"/> you have efficient lock free mechanism to instantiate a huge amount of interwoven objects of the same type. 
    /// You can obtain a calss factory by using the <see cref="O:Loom.Weaver.GetClassFactory"/> method.
    /// </remarks>
    public interface IClassFactory<T>:IClassFactory
    {
        /// <summary>
        /// Creates a typed instance using the constructor of <typeparamref name="T"/> that best matches the specified parameters. 
        /// </summary>
        /// <param name="args">An array of arguments that match in number, order, and type the parameters of the constructor to invoke. If <paramref name="args"/> is an empty array or <value>NULL</value>, the constructor that takes no parameters (the default constructor) is invoked.</param>
        /// <returns>An instance of the newly created object with potentially interwoven aspects.</returns>
        new T Create(params object[] args);
    }

    /// <summary>
    /// The factory if the class object is already known
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class ClassFactory<T> : IClassFactory<T>
    {
        /// <summary>
        /// The ClassObject
        /// </summary>
        private ClassObject classobj;
        /// <summary>
        /// The dynamic arrays
        /// </summary>
        private Aspect[] dynamicaspects;

        /// <summary>
        /// Internal use.
        /// </summary>
        /// <param name="dynamicaspects"></param>
        /// <param name="classobj"></param>
        internal ClassFactory(ClassObject classobj, Aspect[] dynamicaspects)
        {
            this.dynamicaspects = dynamicaspects;
            this.classobj = classobj;
        }
        /// <summary>
        /// Creates an instance of the interwoven class <typeparamref name="T"/>.
        /// </summary>
        /// <param name="args">An array of arguments that match in number, order, and type the parameters of the constructor to invoke. If <paramref name="args"/> is an empty array or <value>NULL</value>, the constructor that takes no parameters (the default constructor) is invoked.</param>
        /// <returns></returns>
        public T Create(params object[] args)
        {
            return (T)classobj.CreateInstance(args, dynamicaspects);
        }

        /// <summary>
        /// Creates an instance of the interwowen class.
        /// </summary>
        /// <param name="args">An array of arguments that match in number, order, and type the parameters of the constructor to invoke. If <paramref name="args"/> is an empty array or <value>NULL</value>, the constructor that takes no parameters (the default constructor) is invoked.</param>
        /// <returns></returns>
        object IClassFactory.Create(params object[] args)
        {
            return classobj.CreateInstance(args, dynamicaspects);
        }
    }

    /// <summary>
    /// The factory for lazy weaving
    /// </summary>
    /// <typeparam name="TYPE"></typeparam>
    internal class AsyncClassFactory<TYPE> : IClassFactory<TYPE>
    {
        /// <summary>
        /// The ClassObject
        /// </summary>
        private volatile ClassObject classobj;
        /// <summary>
        /// The dynamic arrays
        /// </summary>
        private volatile Aspect[] dynamicaspects;
        /// <summary>
        /// The weaving task
        /// </summary>
        private Task weavingtask;

        /// <summary>
        /// Internal use.
        /// </summary>
        /// <param name="dynamicaspects"></param>
        internal AsyncClassFactory(Aspect[] dynamicaspects)
        {
            this.dynamicaspects = dynamicaspects;
            weavingtask = new Task(() => classobj = Weaver.GetClassObject(typeof(TYPE), this.dynamicaspects));
            weavingtask.Start();
        }
        /// <summary>
        /// Creates an instance of <typeparamref name="TYPE"/>.
        /// </summary>
        /// <param name="args">An array of arguments that match in number, order, and type the parameters of the constructor to invoke. If <paramref name="args"/> is an empty array or <value>NULL</value>, the constructor that takes no parameters (the default constructor) is invoked.</param>
        /// <returns></returns>
        public TYPE Create(params object[] args)
        {
            return (TYPE)CreateInstance(args);
        }

        /// <summary>
        /// Creates an instance of the interwowen class.
        /// </summary>
        /// <param name="args">An array of arguments that match in number, order, and type the parameters of the constructor to invoke. If <paramref name="args"/> is an empty array or <value>NULL</value>, the constructor that takes no parameters (the default constructor) is invoked.</param>
        /// <returns></returns>
        object IClassFactory.Create(params object[] args)
        {
            return CreateInstance(args);
        }

        /// <summary>
        /// Internal use.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private object CreateInstance(object[] args)
        {
            if (classobj == null)
            {
                weavingtask.Wait();
            }
            return (TYPE)classobj.CreateInstance(args, dynamicaspects);
        }
    }

}
