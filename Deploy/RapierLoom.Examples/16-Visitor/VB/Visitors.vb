Public MustInherit Class Visitor
    Protected Sub New()
    End Sub

    <VisitorDispatch()> _
    Public Overridable Sub Visit(ByVal node As Expression)
        Console.WriteLine("{0} has no implementation for {1}", Me, node)
    End Sub

End Class

Public Class Print
    Inherits Visitor

    Public Overloads Sub Visit(ByVal op As BinaryOperation)
        op.Left.Accept(Me)
        Console.Write(" {0} ", op.Symbol)
        op.Right.Accept(Me)
    End Sub

    Public Overloads Sub Visit(ByVal lit As Literal)
        Console.Write(lit.Value)
    End Sub

    Public Overloads Sub Visit(ByVal op As UnaryOperation)
        Console.Write(" {0}", op.Symbol)
        op.Expr.Accept(Me)
    End Sub

End Class




Public Class Eval
    Inherits Visitor

    Private Shared Function GetValue(ByVal exp As Expression) As Integer
        Dim eval As Eval = Weaver.Create(Of Eval)(New Object(0 - 1) {})
        exp.Accept(eval)
        Return eval.value
    End Function

    Public Overloads Sub Visit(ByVal add As Add)
        Me._value = (Eval.GetValue(add.Left) + Eval.GetValue(add.Right))
    End Sub

    Public Overloads Sub Visit(ByVal lit As Literal)
        Me._value = lit.Value
    End Sub

    Public Overloads Sub Visit(ByVal neg As Neg)
        Me._value = -Eval.GetValue(neg.Expr)
    End Sub

    Public Overloads Sub Visit(ByVal subt As Subt)
        Me._value = (Eval.GetValue(subt.Left) - Eval.GetValue(subt.Right))
    End Sub

    Public ReadOnly Property Value() As Integer
        Get
            Return Me._value
        End Get
    End Property

    Private _value As Integer
End Class




