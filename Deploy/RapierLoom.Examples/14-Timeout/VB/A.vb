Public Class A

    <Timeout(1000)> _
    Public Overridable Function LongOperation(ByVal i As Integer) As Integer
        Dim ts As TimeSpan
        Dim dt As DateTime = DateTime.Now
        Do
            ts = DateTime.Now - dt
        Loop While (ts.TotalMilliseconds < i)
        Return i
    End Function

End Class


