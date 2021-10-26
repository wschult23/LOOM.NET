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

namespace VisitorExample
{
    /// <summary>
    /// This example shows how to implement the visitor design pattern using a very simple aspect method.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            // create a new expression
            Expression exp = new Add(new Literal(23), new Neg(new Literal(2)));

            // create a new print visitor
            Print print = new Print();
            // visit the expression, to print it out
            print.Visit(exp);

            Console.Write(" = ");
            
            // create a new eval visitor
            Eval eval = new Eval();
            // visit the expression to evaluate
            eval.Visit(exp);
            
            
            Console.WriteLine(eval.Value);

            Console.Read();
        }
    }
}
