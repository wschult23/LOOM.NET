Public Class Program

    Public Shared Sub HelloLoom_ItemAdded(ByVal sender As Object, ByVal item As Object)
        Console.WriteLine("""{0}"" added to {1}.", item, sender)
    End Sub

    Public Shared Sub Main(ByVal args As String())
        Dim eventPublisher As New EventPublisher
        AddHandler eventPublisher.ItemAdded, New ItemAdded(AddressOf Program.HelloLoom_ItemAdded)
        Weaver.Create(Of List(Of Integer))(eventPublisher).Add(1)
        Weaver.Create(Of ArrayList)(eventPublisher).Add("foo")
        Dim mytarget As MyTargetClass = Weaver.Create(Of MyTargetClass)()
        AddHandler DirectCast(mytarget, IEventControl).ItemAdded, New ItemAdded(AddressOf Program.HelloLoom_ItemAdded)
        mytarget.Add(1.1)
        Console.ReadKey()
    End Sub

End Class


