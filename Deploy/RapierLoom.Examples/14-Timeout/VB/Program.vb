Public Class Program

    Public Shared Sub CallOperation(ByVal a As A, ByVal simulatedtimeinprogress As Integer)
        Dim ts As TimeSpan
        Dim dt As DateTime = DateTime.Now
        Try
            a.LongOperation(simulatedtimeinprogress)
            ts = DateTime.Now - dt
            Console.WriteLine("Method returns after {0} ms", ts.TotalMilliseconds)
        Catch exception1 As TimeoutException
            ts = DateTime.Now - dt
            Console.WriteLine("Timeout after {0} ms", ts.TotalMilliseconds)
        End Try
    End Sub

    Public Shared Sub Main(ByVal args As String())
        Dim a As A = Weaver.Create(Of A)()
        Program.CallOperation(a, 200)
        Program.CallOperation(a, 2000)
        Program.CallOperation(a, 20000)
        Console.ReadKey()
    End Sub

End Class

