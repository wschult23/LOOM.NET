Public Class Program

    Public Shared Sub Main(ByVal args As String())
        Dim monAspect As New MonitoringAspect
        Dim library As Library = Weaver.Create(Of Library)(monAspect)
        Dim number As UInt64 = CULng(30)
        Console.WriteLine("Computing {0}. fibonacci number.", number)
        Dim result As UInt64 = library.Fibonacci(number)
        Console.WriteLine("Result is {0}.", result)
        Console.Read()
    End Sub

End Class


