Public Class Program

    Public Shared Sub Main(ByVal args As String())
        Weaver.Create(Of A)()
        Weaver.Create(Of B)()
        EventConsumer.Fire()
        Console.ReadKey()
    End Sub

End Class


