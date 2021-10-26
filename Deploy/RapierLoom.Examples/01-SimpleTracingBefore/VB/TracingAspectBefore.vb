Public Class TracingAspectBefore
    Inherits AspectAttribute

    <[Call](Advice.Before)> _
    Public Sub SayHello(ByVal name As String)
        Console.WriteLine("This is before entering method SayHello.")
    End Sub

End Class

