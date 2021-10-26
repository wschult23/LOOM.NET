<Observable()> _
Public Class TargetClass

    Public State As Integer = 0

    <ChangesState()> _
    Public Overridable Sub Method1()
        Console.WriteLine("Method1 is called.")
        Me.State += 1
    End Sub

    Public Overridable Sub Method2()
        Console.WriteLine("Method2 is called.")
    End Sub

    <ChangesState()> _
    Public Overridable Sub Method3()
        Console.WriteLine("Method3 is called.")
        Me.State -= 1
    End Sub

End Class


