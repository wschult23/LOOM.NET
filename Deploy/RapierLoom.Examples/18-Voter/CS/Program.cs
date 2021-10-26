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

namespace VotingExample
{
    [VotingAspect(new Type[]{
        typeof(RealCalculatorVersionA), 
        typeof(RealCalculatorVersionB), 
        typeof(RealCalculatorVersionC)})]
    public abstract class Calculator
    {
        public Calculator()
        {
            Console.WriteLine("instance of {0} was build.", GetType().ToString());
        }

        public abstract int Add(int a, int b);
        public abstract int Sub(int a, int b);
    }

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
            Calculator calc = Weaver.Create<Calculator>();

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
