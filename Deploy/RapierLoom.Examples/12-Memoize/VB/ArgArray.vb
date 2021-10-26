Friend Class ArgArray

    Private argArray As Object()

    Public Sub New(ByVal args As Object())
        Me.argArray = args
    End Sub

    Public Overrides Function Equals(ByVal obj As Object) As Boolean
        Dim comparedObject As ArgArray = TryCast(obj, ArgArray)
        If ((Not comparedObject Is Nothing) AndAlso (comparedObject.argArray.Length = Me.argArray.Length)) Then
            Dim i As Integer
            For i = 0 To Me.argArray.Length - 1
                If Not Me.argArray(i).Equals(comparedObject.argArray(i)) Then
                    Return False
                End If
            Next i
            Return True
        End If
        Return False
    End Function

    Public Overrides Function GetHashCode() As Integer
        Dim hashcode As Integer = 0
        Dim arg As Object
        For Each arg In Me.argArray
            hashcode = (hashcode + arg.GetHashCode)
        Next
        Return hashcode
    End Function

End Class

