Public Class Expression
    Public Sub Accept(ByVal visitor As Visitor)
        visitor.Visit(Me)
    End Sub
End Class

Public Class Literal
    Inherits Expression

    Public Sub New(ByVal Value As Integer)
        _value = Value
    End Sub

    Public ReadOnly Property Value() As Integer
        Get
            Return _value
        End Get
    End Property

    Private _value As Integer
End Class

Public MustInherit Class Operation
    Inherits Expression

    Protected Sub New()
    End Sub

    Public MustOverride ReadOnly Property Symbol() As String
End Class

Public MustInherit Class BinaryOperation
    Inherits Operation

    Protected Sub New(ByVal left As Expression, ByVal right As Expression)
        Me._left = left
        Me._right = right
    End Sub

    Public ReadOnly Property Left() As Expression
        Get
            Return Me._left
        End Get
    End Property

    Public ReadOnly Property Right() As Expression
        Get
            Return Me._right
        End Get
    End Property

    Private _left As Expression
    Private _right As Expression
End Class


Public MustInherit Class UnaryOperation
    Inherits Operation

    Protected Sub New(ByVal expr As Expression)
        Me._expr = expr
    End Sub


    Public ReadOnly Property Expr() As Expression
        Get
            Return Me._expr
        End Get
    End Property

    Private _expr As Expression
End Class


Public Class Add
    Inherits BinaryOperation

    Public Sub New(ByVal left As Expression, ByVal right As Expression)
        MyBase.New(left, right)
    End Sub

    Public Overrides ReadOnly Property Symbol() As String
        Get
            Return "+"
        End Get
    End Property

End Class

Public Class Subt
    Inherits BinaryOperation
    Public Sub New(ByVal left As Expression, ByVal right As Expression)
        MyBase.New(left, right)
    End Sub

    Public Overrides ReadOnly Property Symbol() As String
        Get
            Return "-"
        End Get
    End Property

End Class

Public Class Neg
    Inherits UnaryOperation

    Public Sub New(ByVal exp As Expression)
        MyBase.New(exp)
    End Sub

    Public Overrides ReadOnly Property Symbol() As String
        Get
            Return "-"
        End Get
    End Property

End Class







