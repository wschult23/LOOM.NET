Public Class MyAspect
    Inherits AspectAttribute

    <IncludeAll(), [Call](Advice.AfterThrowing)> _
    Public Function Foo(Of T)(<JPException()> ByVal exception As Exception, ByVal name As String) As T
        Console.WriteLine("ERROR: An exception of type {0} occured.", exception.GetType.FullName)
        Console.WriteLine("Details:" & ChrW(10) & "{0}", exception.Message)
        Return CType(Nothing, T)
    End Function

    <IncludeAll(), [Call](Advice.AfterReturning)> _
    Public Sub Foo2(<JPRetVal()> ByRef returnValue As String, ByVal name As String)
        If returnValue.Contains("Paul") Then
            returnValue = "Don't want to say hello to Paul..."
        End If
    End Sub

End Class

