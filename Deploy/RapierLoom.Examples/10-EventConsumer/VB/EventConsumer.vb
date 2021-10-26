Public Class EventConsumer
    Inherits AspectAttribute

    Private Shared boundMethods As List(Of Context) = New List(Of Context)

    Public Shared Sub Fire()
        Dim ctx As Context
        For Each ctx In EventConsumer.boundMethods
            ctx.Call()
        Next
    End Sub

    <Loom.JoinPoints.Call(Advice.Initialize), IncludeAll()> _
    Public Sub InitializeEvent(<JPContext()> ByVal ctx As Context)
        EventConsumer.boundMethods.Add(ctx)
    End Sub

End Class

