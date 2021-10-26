Public Class Timeout
    Inherits AspectAttribute

    Private dueTime As Integer
    Private timercb As TimerCallback

    Public Sub New(ByVal dueTime As Integer)
        Me.dueTime = dueTime
        Me.timercb = New TimerCallback(AddressOf Me.TimerCallback)
    End Sub

    <IncludeAll(), [Call](Advice.AfterThrowing)> _
    Public Function Fail(Of T)(<JPContext()> ByVal ctx As Context, <JPException()> ByVal ex As Exception, ByVal ParamArray args As Object()) As T
        If TypeOf ex Is ThreadAbortException Then
            Thread.ResetAbort()
            Throw New TimeoutException
        End If
        Throw ex
    End Function

    <IncludeAll(), [Call](Advice.AfterReturning)> _
    Public Sub Finish(<JPContext()> ByVal ctx As Context, ByVal ParamArray args As Object())
        DirectCast(ctx.Tag, Timer).Dispose()
    End Sub

    <IncludeAll(), [Call](Advice.Before)> _
    Public Sub InitTimer(<JPContext()> ByVal ctx As Context, ByVal ParamArray args As Object())
        ctx.Tag = New Timer(Me.timercb, Thread.CurrentThread, Me.dueTime, -1)
    End Sub

    Private Sub TimerCallback(ByVal thread As Object)
        DirectCast(thread, Thread).Abort()
    End Sub


    
End Class