Public Class MonitoringAspect
    Implements Aspect

    <[Call](Advice.Before), IncludeAll()> _
    Public Sub Start_Monitor(<JPContext()> ByVal context As Context, ByVal ParamArray args As Object())
        context.Tag = DateTime.Now
    End Sub

    <IncludeAll(), [Call](Advice.After)> _
    Public Sub Stop_Monitor(<JPContext()> ByVal context As Context, ByVal ParamArray args As Object())
        Dim executiontime As TimeSpan = DateTime.Now - CDate(context.Tag)
        Console.WriteLine("{0} done in {1}ms.", context.CurrentMethod.Name, executiontime.Milliseconds)
    End Sub

End Class


