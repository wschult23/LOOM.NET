Public Class Memoized
    Inherits AspectAttribute

    Private cache As Dictionary(Of ArgArray, Object) = New Dictionary(Of ArgArray, Object)

    <[Call](Advice.Around), IncludeAll()> _
    Public Function Memoizer(Of T)(<JPContext()> ByVal ctx As Context, ByVal ParamArray args As Object()) As T
        Dim result As Object
        Dim newArray As New ArgArray(args)
        result = Nothing
        If Not Me.cache.TryGetValue(newArray, result) Then
            result = ctx.Invoke(args)
            Me.cache.Add(newArray, result)
        End If
        Return DirectCast(result, T)
    End Function

End Class


