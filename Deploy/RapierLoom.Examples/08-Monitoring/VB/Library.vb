Public Class Library

    Private Function calcFibonacci(ByVal a As UInt64) As UInt64
        If (a > 40) Then
            Throw New ApplicationException("Error: Can only compute the first 40 fibonacci numbers.")
        End If
        If (a <= 2) Then
            Return CULng(1)
        End If
        Return (Me.calcFibonacci((a - 1)) + Me.calcFibonacci((a - 2)))
    End Function

    Public Overridable Function Fibonacci(ByVal a As UInt64) As UInt64
        Return Me.calcFibonacci(a)
    End Function

End Class


