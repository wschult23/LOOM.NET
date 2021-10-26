Public Class Program

    Public Shared Sub Main(ByVal args As String())
        Dim mc As TargetClass = Weaver.Create(Of TargetClass)()
        mc.SetValue(42)
        Dim i As Integer = mc.CrossDomainCall("foo")
        Console.WriteLine("CrossDomainCall returns {0} in application domain ""{1}""", i, AppDomain.CurrentDomain.FriendlyName)
        Console.ReadKey()
    End Sub

End Class


