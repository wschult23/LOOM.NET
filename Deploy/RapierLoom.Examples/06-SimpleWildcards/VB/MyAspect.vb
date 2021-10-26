Public Class MyAspect
    Inherits AspectAttribute

    <[Call](Advice.Around), IncludeAll()> _
    Public Function foo(Of T)(ByVal u As T) As T
        Console.WriteLine("Aspect: Type in aspect was '{0}' with value '{1}'.", GetType(T), u.ToString)
        Return CType(Nothing, T)
    End Function

End Class

