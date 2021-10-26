Public Class MyExampleClass

    Public Overridable Function GetHello(ByVal name As String) As String
        Return ("Hello " & name & ".")
    End Function

    Public Overridable Function GetHello(ByVal firstname As String, ByVal lastname As String) As String
        Return String.Concat(New String() {"Hello ", firstname, " ", lastname, "."})
    End Function

    Public Overridable Sub SayHello(ByVal name As String)
        Console.WriteLine("Hello {0}.", name)
    End Sub

End Class

