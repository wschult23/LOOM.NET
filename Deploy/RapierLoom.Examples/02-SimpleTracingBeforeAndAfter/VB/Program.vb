Public Class Program

    Public Shared Sub Main(ByVal args As String())
        Dim traceAspect As New TracingAspect
        Dim myClass1 As MyExampleClass

        myClass1 = Weaver.Create(Of MyExampleClass)(traceAspect)
        myClass1.SayHello("Peter")

        Console.Read()
    End Sub

End Class