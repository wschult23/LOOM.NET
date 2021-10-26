// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

using Loom;
using Loom.JoinPoints;
using System.Threading;
using System.Threading.Tasks;

namespace Test
{
    public abstract class Futures
    {
        internal abstract void StartTask(Context ctx, object[] args);
    }

    public class Future<T>:Futures
    {
        Task<Future<T>> task;
        T result;

        public Future()
        {
        }

        internal override void StartTask(Context ctx, object[] args)
        {
            task = new Task<Future<T>>(() => (Future<T>)ctx.Invoke(args));
            task.Start();
        }

        private Future(T result)
        {
            this.result = result;
        }

        public static implicit operator T(Future<T> from)
        {
            return from.task != null ? from.task.Result.result : from.result;
        }

        public static implicit operator Future<T>(T from)
        {
            return new Future<T>(from);
        }
        
        public void Await(Action<T> action)
        {
            action.Invoke(task != null ? task.Result.result : result);
        }

        public T Result { get { return task != null ? task.Result.result : result; } }

        public void Wait()
        {
            if(task!=null)
            {
                task.Wait();
            }
        }
    }

    public class AsynchronousAttribute : AspectAttribute
    {
        [Call(Advice.Around), IncludeAll]
        public T CallAsyncVoid<T>([JPContext] Context ctx, params object[] args) where T : _Void
        {
            ctx.Invoke(args);
            return (T)_Void.Value;
        }
        
        [Call(Advice.Around), IncludeNonInterwoven]
        public T CallAsyncValue<T>([JPContext] Context ctx, params object[] args) where T:Futures,new()
        {
            var retval=new T();
            retval.StartTask(ctx, args);
            return retval;
        }

        [Error("Asyncronous methods must return a Future<T>"), IncludeNonInterwoven]
        public void CallAsyncError(params object[] args) { }
    }
}
