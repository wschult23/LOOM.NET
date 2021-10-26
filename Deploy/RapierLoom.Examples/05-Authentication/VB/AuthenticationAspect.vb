Public Class AuthenticationAspect
    Inherits AspectAttribute

    Private credential As Net.NetworkCredential

    <IncludeAll(), [Call](Advice.Around)> _
    Public Function AuthenticateAction(Of T)(<JPContext()> ByVal context As Context, ByVal ParamArray parameters As Object()) As T
        Console.WriteLine("*** Action needs authentication.")
        If (Me.credential Is Nothing) Then
            Console.Write("Username: ")
            Dim username As String = Console.ReadLine
            Console.Write("Password: ")
            Dim password As String = Console.ReadLine
            Me.credential = New Net.NetworkCredential(username, password)
        End If

        If Not Authenticator.Authenticate(Me.credential) Then
            Throw New AuthenticationException("Error: Authentication failed.")
        End If

        Return DirectCast(context.Invoke(parameters), T)
    End Function



End Class


