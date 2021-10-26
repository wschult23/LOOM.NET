Public Class MyExampleClass

    Public Overridable Function SayHello(ByVal name As String) As String
        If (name.Length <= 0) Then
            Throw New ApplicationException("Nobody to say hello to.")
        End If
        Return ("Hello " & name & ".")
    End Function

End Class

