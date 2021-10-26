Public Class Program

    Public Shared Sub Main(ByVal args As String())
        Dim aspect As New TracingAspectBefore
        Dim myclass1 As MyExampleClass

        myclass1 = Weaver.Create(Of MyExampleClass)(aspect)
        myclass1.SayHello("Peter")

        Console.Read()
    End Sub

End Class

