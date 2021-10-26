Public Class Aspect2
    Inherits AspectAttribute

    <[Call](Advice.Before)> _
    Public Sub Test(<JPVariable(Scope.Private)> ByRef hidden As Integer, <JPVariable(Scope.Virtual)> ByRef visible As Integer)
        Console.WriteLine("Aspect2: hidden={0}, visible={1}", CInt(hidden), CInt(visible))
        hidden += 1
        visible += 1
    End Sub

End Class

