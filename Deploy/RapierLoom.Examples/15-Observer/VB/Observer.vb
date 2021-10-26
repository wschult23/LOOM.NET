Friend Class Observer
    Implements IObserver

    ReadOnly Property Name() As String Implements IObserver.Name
        Get
            Return Me._name
        End Get
    End Property

    Private _name As String

    Public Sub New(ByVal Name As String)
        Me._name = Name
    End Sub

    Private Sub Notify(ByVal ObservedSubject As ISubject) Implements IObserver.Notify
        Console.WriteLine(" {0} was notified for {1}. State changed to {2}.", Me._name, ObservedSubject, DirectCast(ObservedSubject, TargetClass).State)
    End Sub

End Class


