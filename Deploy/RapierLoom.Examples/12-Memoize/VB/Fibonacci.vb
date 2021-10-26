Public Class Fibonacci

    <Memoized()> _
    Public Overridable Function Calculate(ByVal x As UInt32) As UInt64
        If (x < 2) Then
            Return CULng(x)
        End If
        Return (Me.Calculate((x - 1)) + Me.Calculate((x - 2)))
    End Function

End Class


