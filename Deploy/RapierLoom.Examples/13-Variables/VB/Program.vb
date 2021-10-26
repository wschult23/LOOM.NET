Public Class Program

    Public Shared Sub Main(ByVal args As String())
        Dim target As TargetClass

        target = Weaver.Create(Of TargetClass)()
        target.Test()

        Console.Read()
    End Sub

End Class

