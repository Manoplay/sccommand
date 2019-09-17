''' <summary>
''' Rappresenta un contesto come <see cref="AzioneCattolica"/> o la <see cref="Scuola"/>, avente un amministratore, dei livelli e una costante K. Permette di ottenere una Local Control Authority.
''' </summary>
Public MustInherit Class Context
    Public Property Name As String
    Public MustOverride ReadOnly Property Administrator As HumanUnit
    Public Property LeveledMembers As Dictionary(Of HumanUnit, Integer)
    MustOverride ReadOnly Property k As Integer

    Public Function GetLCA(user As HumanUnit) As Integer
        If user = Administrator Then Return 100 Else Return LeveledMembers(user) * k
    End Function
End Class

<SystemContext("ContextManagement")>
Public Module ContextManagement
    <SystemCall("Subscribe to context (.*)")>
    Public Sub SubscribeToContext(contextName As String)
        Dim context As Context
        Dim type = System.Type.GetType(contextName, True, True)
        If type.IsSubclassOf(GetType(Context)) Then context = Activator.CreateInstance(type) Else Throw New ArgumentException("Specified name does not design a Context.")
        Contexts.Add(context)
    End Sub
End Module