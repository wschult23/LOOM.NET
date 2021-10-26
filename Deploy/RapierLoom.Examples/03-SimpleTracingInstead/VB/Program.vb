Public Class Program

    Public Shared Sub Main(ByVal args As String())
        Dim traceAspect As New TracingAspect
        Dim myClass1 As MyExampleClass = Weaver.Create(Of MyExampleClass)(traceAspect)

        myClass1.SayHello("Peter")

        Console.WriteLine(myClass1.GetHello("Peter"))
        Console.WriteLine(myClass1.GetHello("Peter", "Piper"))
        Console.Read()
    End Sub

End Class

