<Loom.AspectProperties.CreateAspect(AspectProperties.Per.Class)> _
Public Class Singleton
    Inherits AspectAttribute

    Private obj As Object = Nothing

    <Create(Advice.Around)> _
    Public Function Create(Of T)(<JPContext()> ByVal ctx As Context) As T
        If (Me.obj Is Nothing) Then
            Me.obj = ctx.Call
        End If
        Return DirectCast(Me.obj, T)
    End Function

End Class


