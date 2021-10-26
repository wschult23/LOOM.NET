Public Class VisitorDispatch
    Inherits AspectAttribute

    <[Call](Advice.Around)> _
    Public Sub Visit(<JPContext()> ByVal ctx As Context, ByVal node As Expression)
        ctx.ReCall(New Object() {node})
    End Sub

End Class