Public Class MyExampleClass

    Public Overridable Function foo1(ByVal s As String) As String
        Console.WriteLine("Target Class: Was {0}.", s)
        Return s
    End Function

    Public Overridable Function foo2(ByVal i As Integer) As Integer
        Console.WriteLine("Target Class: Was {0}.", i)
        Return i
    End Function

    Public Overridable Function foo3(ByVal s As String) As Integer
        Console.WriteLine("Target Class: Was {0}.", s)
        Return s.Length
    End Function

End Class


