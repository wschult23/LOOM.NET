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
using Loom;
using Loom.JoinPoints;
using Loom.AspectProperties;
using System.Collections;

namespace Loom.DependecyInjection
{
    /// <summary>
    /// This is an aspect for dependency injection
    /// You can place this aspect on a dependant property or initializer method
    /// </summary>
    public class Inject:AspectAttribute
    {
        /// <summary>
        /// Should the reference automatically be disposed if the parent object receives a Dispose() message
        /// </summary>
        public bool AutoDispose = true;

        /// <summary>
        /// Registred types
        /// </summary>
        private static Dictionary<Type, IClassFactory> registry = new Dictionary<Type, IClassFactory>();

        /// <summary>
        /// Adds a referenced Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="referencedObjects"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        private T AddReferencedObject<T>(ref ArrayList referencedObjects, T obj)
        {
            if (AutoDispose)
            {
                if (referencedObjects == null)
                {
                    referencedObjects = new ArrayList();
                }
                referencedObjects.Add(obj);
            }
            return obj;
        }

        /// <summary>
        /// This advice will automatically inject registred objects to a property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ctx"></param>
        /// <param name="obj"></param>
        [Access(Advice.Initialize),IncludeAll]
        public void InjectProperty<T>(  [JPContext] Context ctx, 
                                        [JPVariable(Scope.Protected)] ref ArrayList referencedObjects,
                                        T obj)
        {
            ctx.Call(AddReferencedObject(ref referencedObjects, GetInstance<T>()));
        }

       

        /// <summary>
        /// This advice will automatically inject registred objects to a initializer Method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ctx"></param>
        /// <param name="obj"></param>
        [Call(Advice.Initialize), IncludeAll]
        public void InjectMethod<T>(    [JPContext] Context ctx, 
                                        [JPVariable(Scope.Protected)] ref ArrayList referencedObjects,
                                        T obj)
        {
            ctx.Call(AddReferencedObject(ref referencedObjects, GetInstance<T>()));
        }

        /// <summary>
        /// This introduction implements the IDsiposabe interface to ensure that all referenced object
        /// are subsequently disposed
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="disposed"></param>
        /// <param name="referencedObjects"></param>
        [Introduce(typeof(IDisposable), ExistingInterfaces.Advice)]
        public static void Dispose([JPContext] Context ctx,
                                   [JPVariable(Scope.Private)] ref bool disposed,
                                   [JPVariable(Scope.Protected)] ArrayList referencedObjects)
        {
            if(!disposed)
            {
                disposed=true;
                if (referencedObjects != null)
                {
                    foreach(var obj in referencedObjects)
                    {
                        var disp = obj as IDisposable;
                        if (disp != null)
                        {
                            disp.Dispose();
                        }
                    }
                }
            }
            if (!ctx.HasJoinPoint())
            {
                ctx.Call();
            }
        }

        /// <summary>
        /// Returns a instance of a registered class
        /// </summary>
        /// <typeparam name="T">the interface of the class</typeparam>
        /// <returns>the instance</returns>
        public static T GetInstance<T>()
        {
            IClassFactory factory;
            if(!registry.TryGetValue(typeof(T), out factory))
            {
                throw new ApplicationException("Interface not registred");
            }
            return (T)factory.Create();
        }

        /// <summary>
        /// Registers a class that implements a specific interface
        /// </summary>
        /// <typeparam name="INTERFACE"></typeparam>
        /// <typeparam name="CLASS"></typeparam>
        public static void RegisterClass<INTERFACE, CLASS>() where CLASS:class, new()
        {
            registry.Add(typeof(INTERFACE), Weaver.GetClassFactory<CLASS>());
        }

    }
}
