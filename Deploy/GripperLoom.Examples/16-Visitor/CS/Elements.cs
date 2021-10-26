// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace VisitorExample
{
    /// <summary>
    /// The Expression accepts different visitors; Print and Eval
    /// </summary>
    public class Expression
    {
        public void Accept(Visitor visitor)
        {
            // here the visitors Visit() Method is called
            visitor.Visit(this);
        }
    }

    public class Literal : Expression
    {
        private int value;

        public int Value
        {
            get { return this.value; }
        }

        public Literal(int value)
        {
            this.value = value;
        }
    }

    public abstract class Operation : Expression
    {
        public abstract string Symbol { get; }
    }

    public abstract class BinaryOperation : Operation
    {
        protected BinaryOperation(Expression left, Expression right)
        {
            this.left = left;
            this.right = right;
        }

        private Expression left;
        public Expression Left
        {
            get { return left; }
        }

        private Expression right;
        public Expression Right
        {
            get { return right; }
        }

    }

    public abstract class UnaryOperation : Operation
    {
        Expression expr;

        public Expression Expr
        {
            get { return expr; }
        }

        protected UnaryOperation(Expression expr)
        {
            this.expr = expr;
        }
    }

    public class Add : BinaryOperation
    {
        public Add(Expression left, Expression right)
            :
            base(left, right)
        {
        }

        public override string Symbol
        {
            get { return "+"; }
        }
    }

    public class Sub : BinaryOperation
    {
        public Sub(Expression left, Expression right)
            :
            base(left, right)
        {
        }

        public override string Symbol
        {
            get { return "-"; }
        }

    }

    public class Neg : UnaryOperation
    {
        public Neg(Expression exp)
            : 
            base(exp)
        {
        }

        public override string Symbol
        {
            get { return "-"; }
        }

    }
}
