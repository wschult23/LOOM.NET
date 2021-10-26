Public Class Program

    Public Shared Sub Main(ByVal args As String())
        Dim exp As Expression = New Add(New Literal(&H17), New Neg(New Literal(2)))
        Weaver.Create(Of Print)().Visit(exp)
        Console.Write(" = ")
        Dim eval As Eval = Weaver.Create(Of Eval)(New Object(0 - 1) {})
        eval.Visit(exp)
        Console.WriteLine(eval.Value)
        Console.Read()
    End Sub

End Class

