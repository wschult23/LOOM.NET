<Serializable()> _
Public Class TargetClass

    Private i As Integer

    <CrossDomain("PrivateAppDomain")> _
    Public Overridable Function CrossDomainCall(ByVal s As String) As Integer
        Console.WriteLine("CrossDomainCall(""{0}"") called in application domain ""{1}""", s, AppDomain.CurrentDomain.FriendlyName)
        Return Me.i
    End Function

    Public Overridable Sub SetValue(ByVal i As Integer)
        Console.WriteLine("SetValue({0}) called in application domain ""{1}""", i, AppDomain.CurrentDomain.FriendlyName)
        Me.i = i
    End Sub

End Class

