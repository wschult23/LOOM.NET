Public Class MyExampleClass

    Private list As List(Of Object) = New List(Of Object)

    <AuthenticationAspect()> _
    Public Overridable Sub Add(ByVal newItem As Object)
        Me.list.Add(newItem)
        Console.WriteLine("Added {0} to list. Countet {1} items in list.", newItem, Me.list.Count)
    End Sub

    <AuthenticationAspect()> _
    Public Overridable Sub Delete(ByVal index As Integer)
        If (index < Me.list.Count) Then
            Me.list.RemoveAt(index)
            Console.WriteLine("Removed item at position {0}.", index)
        End If
    End Sub

    Public Overridable Sub PrintList()
        Dim o As Object
        For Each o In Me.list
            Console.WriteLine(o)
        Next
    End Sub

End Class


