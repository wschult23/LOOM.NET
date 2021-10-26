Public Class Program

    Public Shared Sub Main(ByVal args As String())
        Dim myAspect As New MyAspect
        Dim myObject As MyExampleClass = Weaver.Create(Of MyExampleClass)(myAspect)

        Console.WriteLine("Saying hello to Peter...")
        Console.WriteLine(myObject.SayHello("Peter"))
        Console.WriteLine("Saying hello to nobody...")
        Console.WriteLine(myObject.SayHello(""))
        Console.WriteLine("Saying hello to Paul...")
        Console.WriteLine(myObject.SayHello("Paul"))
        Console.Read()
    End Sub

End Class

