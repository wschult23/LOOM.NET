Public Class Program

    Public Shared Sub Main(ByVal args As String())
        Dim aspect As New MyAspect
        Dim targetClass As MyExampleClass = Weaver.Create(Of MyExampleClass)(aspect)

        Console.WriteLine("Calling targets foo1 method...")
        targetClass.foo1("Hello foo1")
        Console.WriteLine("Calling targets foo2 method...")
        targetClass.foo2(2)
        Console.WriteLine("Calling targets foo3 method...")
        targetClass.foo3("Hello foo3")

        Console.ReadKey()
    End Sub

End Class