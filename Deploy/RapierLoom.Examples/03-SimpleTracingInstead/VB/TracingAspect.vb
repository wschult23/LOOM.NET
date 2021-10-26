Public Class TracingAspect
    Inherits AspectAttribute

    <[Call](Advice.Before), Include("SayHello")> _
    Public Sub Foo(ByVal name As String)
        Console.WriteLine("This is before entering a method.")
    End Sub

    <IncludeAll(), [Call](Advice.Before)> _
    Public Sub Foo2(<JPRetVal()> ByVal retval As String, ByVal name As String)
        Console.WriteLine("This is before entering a method with the string parameter '{0}' which returns a string value.", name)
    End Sub

    <[Call](Advice.Before), IncludeAll()> _
    Public Sub Foo3(<JPRetVal()> ByVal retval As String, ByVal param1 As String, ByVal param2 As String)
        Console.WriteLine("This is before entering a method with two string parameters '{0}', '{1}' which returns a string value.", param1, param2)
    End Sub

    <[Call](Advice.Around)> _
    Public Sub SayHello(ByVal name As String)
        Console.WriteLine("This is not the SayHello method. But I know who you are {0}.", name)
    End Sub

End Class


