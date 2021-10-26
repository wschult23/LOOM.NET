// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Threading;

using Loom;
using Loom.JoinPoints;

namespace TimeoutExample
{
    public class Timeout : AspectAttribute
    {
        private TimerCallback timercb;  // delegates the timer signal to the handling method
        private int dueTime;            // stores the upper time boundary for the operation

        public Timeout(int dueTime)
        {
            this.dueTime = dueTime;
            this.timercb = new TimerCallback(TimerCallback);    // init the callback with the TimerCallback method
        }

        // create a new timer, which invokdes the callback method after the dueTime was overstepped
        [Call(Advice.Before)]   // indicates, that this aspect is called before the execution of the target methods app code
        [IncludeAll]            // includes all methods
        public void InitTimer([JPContext] Context ctx, params object[] args)  // [JPContext]Context ctx  - creates a new context object for every aspect interwoven with a target methods
        {
            ctx.Tag = new Timer(timercb, Thread.CurrentThread, dueTime, System.Threading.Timeout.Infinite);  // store the timer object with the callback delegate into the context tag
        }

        // this aspect is only called / reached, if the operation finished in time, the TimerCallback method was not called and so no ThreadAbortException was thrown 
        [Call(Advice.AfterReturning)]   // indicates, that this aspect method is called after the method returns
        [IncludeAll]                    // includes all methods
        public void Finish([JPContext] Context ctx, params object[] args)
        {
            ((Timer)ctx.Tag).Dispose();     // disposes / stops the timer, so that no Exception can occur
        }

        [Call(Advice.AfterThrowing)]    // indicates, that this aspect is called, if an exception is thrown inside the target method or one of their aspects
        [IncludeAll]                    // includes all methods
        public T Fail<T>([JPContext] Context ctx, [JPException] Exception ex, params object[] args)
        {
            if (ex is ThreadAbortException)
            {
                Thread.ResetAbort();            
                throw new TimeoutException();   // throw new Exception and handle this in the main app
            }
            throw ex;
        }


        private void TimerCallback(object thread)
        {
            Thread t = (Thread)thread;  
            t.Abort();  // raises the ThreadAbortException and indirectly calls the Fail Method
        }
    }
}
