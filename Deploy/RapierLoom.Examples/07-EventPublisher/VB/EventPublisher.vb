Public Class EventPublisher
    Inherits AspectAttribute

    <Introduce(GetType(IEventControl))> _
    Public Event ItemAdded As ItemAdded

    <Include("Add"), [Call](Advice.Before)> _
    Public Sub Add(Of T)(<JPTarget()> ByVal target As Object, ByVal item As T)
        RaiseEvent ItemAdded(target, item)
    End Sub

End Class

Public Interface IEventControl
    Event ItemAdded As ItemAdded
End Interface

Public Delegate Sub ItemAdded(ByVal sender As Object, ByVal item As Object)



