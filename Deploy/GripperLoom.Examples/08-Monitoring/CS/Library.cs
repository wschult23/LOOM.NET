// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Monitoring
{
    [MonitoringAspect]
    public class Library
    {
        public virtual ulong Fibonacci(ulong a)
        {
            return calcFibonacci(a);
        }


        #region private methods - not matched

        private ulong calcFibonacci(ulong a)
        {
            if (a > 40)
                throw new ApplicationException("Error: Can only compute the first 40 fibonacci numbers.");

            if (a <= 2)
                return 1;
            else
                return (calcFibonacci(a - 1) + calcFibonacci(a - 2));
        }

        #endregion

    }
}
