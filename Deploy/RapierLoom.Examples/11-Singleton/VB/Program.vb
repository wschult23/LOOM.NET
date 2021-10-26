Public Class Program

    Public Shared Sub Main(ByVal args As String())
        Dim a1 As A = Weaver.Create(Of A)()
        Dim a2 As A = Weaver.Create(Of A)()
        Console.WriteLine(Object.ReferenceEquals(a1, a2))
        Dim b1 As B = Weaver.Create(Of B)()
        Dim b2 As B = Weaver.Create(Of B)()
        Console.WriteLine(Object.ReferenceEquals(b1, b2))
        Console.ReadKey()
    End Sub

End Class


