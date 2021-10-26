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
    /// The Visitor has to implement the Visit method
    /// </summary>
    public abstract class Visitor
    {
        [VisitorDispatch]  // interweaves the VisitorDispatch aspect
        public virtual void Visit(Expression node)
        {
            Console.WriteLine("{0} has no implementation for {1}", this, node);
        }
    }
    
    /// <summary>
    /// The Print Visitor is able to print the expression
    /// </summary>
    public class Print : Visitor
    {
        public Print()
        {
        }

        public void Visit(Literal lit)
        {
            Console.Write(lit.Value);
        }

        public virtual void Visit(BinaryOperation op)
        {
            op.Left.Accept(this);
            Console.Write(" {0} ",op.Symbol);
            op.Right.Accept(this);
        }

        public virtual void Visit(UnaryOperation op)
        {
            Console.Write(" {0}",op.Symbol);
            op.Expr.Accept(this);
        }
    }

    public class Eval : Visitor
    {
        public Eval()
        {
        }

        private int value;

        public int Value
        {
            get { return this.value; }
        }

        private static int GetValue(Expression exp)
        {
            Eval eval = new Eval();
            exp.Accept(eval);
            return eval.value;
        }

        public void Visit(Literal lit)
        {
            value = lit.Value;
        }

        public void Visit(Add add)
        {
             value = GetValue(add.Left) + GetValue(add.Right);
        }
        
        public void Visit(Sub sub)
        {
            value = GetValue(sub.Left) - GetValue(sub.Right);
        }

        public void Visit(Neg neg)
        {
            value = -GetValue(neg.Expr);
        }
    }
}
