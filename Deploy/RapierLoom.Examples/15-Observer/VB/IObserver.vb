Public Interface IObserver
    Sub Notify(ByVal observedSubject As ISubject)
    ReadOnly Property Name() As String
End Interface


