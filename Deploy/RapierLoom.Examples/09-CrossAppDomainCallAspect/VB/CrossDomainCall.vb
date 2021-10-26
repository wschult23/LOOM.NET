Public Class CrossDomainCall
    Inherits MarshalByRefObject

    Public Function DoCallback(ByVal ctx As Context, ByVal args As Object()) As Object
        Return ctx.Invoke(args)
    End Function

End Class

<Serializable()> _
Public Class CrossDomainAttribute
    Inherits AspectAttribute

    Private appdomaincall As CrossDomainCall
    Private Shared domains As Dictionary(Of String, CrossDomainCall) = New Dictionary(Of String, CrossDomainCall)

    Public Sub New(ByVal domainname As String)
        If (Not AppDomain.CurrentDomain.FriendlyName Is domainname) Then
            SyncLock CrossDomainAttribute.domains
                If Not CrossDomainAttribute.domains.TryGetValue(domainname, Me.appdomaincall) Then
                    Dim appdomain As AppDomain = AppDomain.CreateDomain(domainname)
                    Me.appdomaincall = DirectCast(appdomain.CreateInstanceAndUnwrap(GetType(CrossDomainCall).Assembly.FullName, GetType(CrossDomainCall).FullName), CrossDomainCall)
                    CrossDomainAttribute.domains.Add(domainname, Me.appdomaincall)
                End If
            End SyncLock
        End If
    End Sub

    <[Call](Advice.Around), IncludeAll()> _
    Public Function DoCall(Of T)(<JPContext()> ByVal ctx As Context, ByVal ParamArray args As Object()) As T
        If (Not Me.appdomaincall Is Nothing) Then
            Return DirectCast(Me.appdomaincall.DoCallback(ctx, args), T)
        End If
        Return DirectCast(ctx.Invoke(args), T)
    End Function

End Class

