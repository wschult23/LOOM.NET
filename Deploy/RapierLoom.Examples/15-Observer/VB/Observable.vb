Public Class ChangesState
    Inherits Attribute
End Class

<AttributeUsage(AttributeTargets.Class)> _
Public Class Observable
    Inherits AspectAttribute

    Private observers As List(Of IObserver) = New List(Of IObserver)

    <Finalize(Advice.Before)> _
    Public Sub FinalizeObservers()
        Console.WriteLine("Observable object is shut down. Unregistering running observers.")
        Me.observers.ForEach(New Action(Of IObserver)(AddressOf Me.Unregister))
    End Sub

    <IncludeAnnotated(GetType(ChangesState)), [Call](Advice.After)> _
    Public Sub Notify(<JPContext()> ByVal ctx As Context, ByVal ParamArray args As Object())
        Console.WriteLine("Notify all registered observers:")
        Dim observer As IObserver
        For Each observer In Me.observers
            Console.WriteLine(" Notify {0}.", observer.Name)
            observer.Notify(DirectCast(ctx.Instance, ISubject))
        Next
    End Sub

    <Introduce(GetType(ISubject), ExistingInterfaces.Advice)> _
    Public Sub Register(ByVal Observer As IObserver)
        If Not Me.observers.Contains(Observer) Then
            Me.observers.Add(Observer)
            Console.WriteLine(" {0} was registered.", Observer.Name)
        End If
    End Sub

    <Introduce(GetType(ISubject), ExistingInterfaces.Advice)> _
    Public Sub Unregister(ByVal Observer As IObserver)
        If Me.observers.Contains(Observer) Then
            Me.observers.Remove(Observer)
            Console.WriteLine(" {0} was unregistered.", Observer.Name)
        End If
    End Sub

End Class