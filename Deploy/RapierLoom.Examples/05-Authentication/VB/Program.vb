Public Class Program

    Public Shared Sub Main(ByVal args As String())
        Dim m As MyExampleClass = Weaver.Create(Of MyExampleClass)()

        Try
            Console.WriteLine("Printing list:")
            m.PrintList()
            Console.WriteLine("Adding items to list:")
            m.Add("Entry 1")
            m.Add("100")
            m.Add(Guid.NewGuid)
            Console.WriteLine("Printing list:")
            m.PrintList()
        Catch exc As AuthenticationException
            Console.WriteLine(exc.Message)
        End Try

        Console.Read()
    End Sub

End Class


