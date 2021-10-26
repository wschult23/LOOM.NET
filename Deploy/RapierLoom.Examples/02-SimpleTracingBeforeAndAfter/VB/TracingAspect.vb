Public Class TracingAspect
    Inherits AspectAttribute

    <Include("SayHello"), [Call](Advice.After)> _
    Public Sub Foo(ByVal name As String)
        Console.WriteLine("This is after leaving method SayHello.")
    End Sub

    <[Call](Advice.Before)> _
    Public Sub SayHello(ByVal name As String)
        Console.WriteLine("This is before entering method SayHello.")
    End Sub

End Class


