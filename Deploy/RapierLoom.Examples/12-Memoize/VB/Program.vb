Public Class Program

    Public Shared Sub Main(ByVal args As String())
        Dim fibno As UInt32 = &H2A
        Dim t As New Fibonacci

        Dim start As DateTime = DateTime.Now
        Console.WriteLine("Calculating without aspect...")
        Dim result As UInt64 = t.Calculate(fibno)
        Dim span As TimeSpan = DateTime.Now - start
        Console.WriteLine("{0}. fibonacci number is {1}. Calculated in {2} ms.", fibno, result, span.TotalMilliseconds)

        t = Weaver.Create(Of Fibonacci)()
        start = DateTime.Now
        Console.WriteLine("Calculating with aspect...")
        result = t.Calculate(fibno)
        span = DateTime.Now - start
        Console.WriteLine("{0}. fibonacci number is {1}. Calculated in {2} ms.", fibno, result, span.TotalMilliseconds)

        fibno = (fibno + 5)
        start = DateTime.Now
        Console.WriteLine("Calculating again...")
        result = t.Calculate(fibno)
        span = DateTime.Now - start
        Console.WriteLine("{0}. fibonacci number is {1}. Calculated in {2} ms.", fibno, result, span.TotalMilliseconds)

        Console.ReadKey()
    End Sub

End Class

