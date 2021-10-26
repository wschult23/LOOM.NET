Public Class Program

    Public Shared Sub Main(ByVal args As String())
        Program.Run()
        GC.Collect()
        Console.Read()
    End Sub

    Public Shared Sub Run()
        Dim target As TargetClass = Weaver.Create(Of TargetClass)()
        Dim observer1 As New Observer("oberserver1")
        Dim observer2 As New Observer("oberserver2")
        Console.WriteLine("Registering observers on target class:")
        Dim subject As ISubject = DirectCast(target, ISubject)
        subject.Register(observer1)
        subject.Register(observer2)
        target.Method1()
        target.Method2()
        subject.Unregister(observer1)
        target.Method3()
    End Sub

End Class


