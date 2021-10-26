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
using Loom;
using Loom.JoinPoints;
using System.Threading;

namespace AsyncronousExample
{
    /// <summary>
    /// This is the Base-Class for the Future{T} implementation
    /// </summary>
    public abstract class Futures
    {
        internal abstract void StartTask(Context ctx, object[] args);
    }

    /// <summary>
    /// A future is a stand-in for a computational result that is initially unknown but becomes available at a later time
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Future<T> : Futures
    {
        /// <summary>
        /// The parallel task
        /// </summary>
        Task<Future<T>> task;
        /// <summary>
        /// The computational result
        /// </summary>
        T result;

        /// <summary>
        /// This constructor will be used by the aspect to create the callers instance of the future
        /// </summary>
        public Future()
        {
        }

        /// <summary>
        /// This constructor will be used by the conversion operator to create the callees instance of the future
        /// </summary>
        /// <param name="result"></param>
        private Future(T result)
        {
            this.result = result;
        }

        /// <summary>
        /// Internal method to start a parallel task
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="args"></param>
        internal override void StartTask(Context ctx, object[] args)
        {
            task = new Task<Future<T>>(() => (Future<T>)ctx.Invoke(args));
            task.Start();
        }

        /// <summary>
        /// Wait until the result is available
        /// </summary>
        public void Wait()
        {
            if (task != null)
            {
                task.Wait();
            }
        }

        /// <summary>
        /// A continuation, executed when the result is available
        /// </summary>
        /// <param name="action"></param>
        public void ContinueWith(Action<T> action)
        {
            if (task == null)
            {
                action.Invoke(result);
            }
            else
            {
                task.ContinueWith(ctask => action.Invoke(ctask.Result));
            }
        }

        /// <summary>
        /// Retreives the result value
        /// </summary>
        public T Result 
        { 
            get 
            {
                if (task != null)
                {
                    this.result = task.Result.result;
                    task = null;
                }
                return result;
            } 
        }

        /// <summary>
        /// Converts a future into the result
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static implicit operator T(Future<T> from)
        {
            return from.Result;
        }

        /// <summary>
        /// Converts a result into a future
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static implicit operator Future<T>(T from)
        {
            return new Future<T>(from);
        }

        public override bool Equals(object obj)
        {
            return Result.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Result.GetHashCode();
        }

        public override string ToString()
        {
            return Result.ToString();
        }
    }

    /// <summary>
    /// Marks a method as asynchronous
    /// </summary>
    public class AsynchronousAttribute : AspectAttribute
    {
        /// <summary>
        /// This Advice will catch all methods with no return value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ctx"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        [Call(Advice.Around), IncludeAll]
        public T CallAsyncVoid<T>([JPContext] Context ctx, params object[] args) where T : _Void
        {
            var task = new Task(() => ctx.Invoke(args));
            task.Start();
            return (T)_Void.Value;
        }

        /// <summary>
        /// This Advice will catch all methods returning a Future{T} 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ctx"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        [Call(Advice.Around), IncludeAll]
        public T CallAsyncValue<T>([JPContext] Context ctx, params object[] args) where T : Futures, new()
        {
            var retval = new T();
            retval.StartTask(ctx, args);
            return retval;
        }

        /// <summary>
        /// This advice will generate a error message if the method could not be interwoven with one of the advices above
        /// </summary>
        /// <param name="args"></param>
        [Error("Asyncronous methods must return a Future<T> or nothing (void)"), IncludeNonInterwoven]
        public void CallAsyncError(params object[] args) { }
    }
}
