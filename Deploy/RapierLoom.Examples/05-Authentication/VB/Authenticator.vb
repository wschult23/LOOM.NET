Public Class Authenticator

    Public Shared Function Authenticate(ByVal Credential As Net.NetworkCredential) As Boolean
        If ((Credential.UserName.Equals("Peter")) AndAlso (Credential.Password.Equals("Pan"))) Then
            Console.WriteLine(String.Format("*** Authenticated as {0}.", Credential.UserName))
            Return True
        End If
        Console.WriteLine(String.Format("*** Unable to authenticate as {0}.", Credential.UserName))
        Return False
    End Function

End Class

