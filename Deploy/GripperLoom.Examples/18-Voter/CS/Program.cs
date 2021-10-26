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

/// This example shows, how a voting mechanism can be implemented with LOOM.NET
namespace VotingExample
{
    /// <summary>
    /// This is the abstract base clas wich implements the interface 
    /// </summary>
    [VotingAspect(new Type[]{
        typeof(RealCalculatorVersionA), 
        typeof(RealCalculatorVersionB), 
        typeof(RealCalculatorVersionC)})]
    public class Calculator
    {
        public Calculator()
        {
            Console.WriteLine("instance of {0} was build.", GetType().ToString());
        }

        public virtual int Add(int a, int b) { return default(int);}
        public virtual int Sub(int a, int b) { return default(int); }
    }

    /// <summary>
    /// The first implementation of the base class
    /// </summary>
    public class RealCalculatorVersionA : Calculator
    {
        public override int Add(int a, int b)
        {
            return a + b;
        }

        public override int Sub(int a, int b)
        {
            return a + b;
        }
    }

    /// <summary>
    /// The second implementation of the base class
    /// </summary>
    public class RealCalculatorVersionB : Calculator
    {
        public override int Add(int a, int b)
        {
            return a - b;
        }

        public override int Sub(int a, int b)
        {
            return a - b;
        }
    }

    /// <summary>
    /// The third implementation of the base class
    /// </summary>
    public class RealCalculatorVersionC : Calculator
    {
        public override int Add(int a, int b)
        {
            return a + b;
        }

        public override int Sub(int a, int b)
        {
            return a * b;
        }
    }



    public class Program
    {
        static void Main(string[] args)
        {
            // create my class objects
            Calculator calc = new Calculator();

            try
            {
                Console.Write("2 + 3 = ");
                Console.WriteLine(calc.Add(2, 3));
            }
            catch (Exception)
            {
                Console.WriteLine("there iAs no majority to obtain a result");
            }
            try
            {
                Console.Write("0 - 0 = ");
                Console.WriteLine(calc.Sub(0, 0));
            }
            catch (Exception)
            {
                Console.WriteLine("there is no majority to obtain a result");
            }
            try
            {
                Console.Write("5 - 3 = ");
                Console.WriteLine(calc.Sub(5, 3));
            }
            catch (Exception)
            {
                Console.WriteLine("there is no majority to obtain a result");
            }

            Console.ReadKey();
        }
    }
}
