Imports System.DirectoryServices

Module HumanUnitManagement
    <SystemCall("Kick player (.*)")>
    Public Sub KickPlayer(player As String)
        Dim id As HumanUnit = GUIDs(player)
        If id IsNot Nothing Then
            id.Kick()
        End If
    End Sub

    Public Property left As HumanUnit
    Public Property right As HumanUnit
    Public Property self As HumanUnit

    Public Sub Init()
#If MYHERO_QUIRKLY Or MAGIC Then
        left As HumanUnit = MyHero.GetOpenPort("Z:\DEVICES\OPENPORT1.SYS").GetObject()
        right As HumanUnit = MyHero.GetOpenPort("Z:\DEVICES\OPENPORT2.SYS").GetObject()
#End If
#If NETWORK Then
        Dim root = New DirectoryEntry("WinNT:")
        For Each computers As DirectoryEntry In root.Children
            For Each computer As DirectoryEntry In computers.Children

                If computer.Name <> "Schema" Then

                    GUIDs.Add(Guid.NewGuid().ToString, computer)

                End If
            Next
            left = New HumanUnit(computers.Children(0))
            right = New HumanUnit(computers.Children(1))
        Next
#End If
        self = New HumanUnit()
        self.SystemControlAuthority = Program.SCA
        self.ObjectControlAuthority = 0
        self.UnitID = Guid.NewGuid().ToString()
        self.UnitName = Program.User
        GUIDs(self.UnitID) = self
    End Sub

    <SystemCall("Transfer human durability. (Left|Right|Self) to (left|right|self).")>
    Public Sub TransferDurability(source As String, target As String)
        Dim origin As HumanUnit = Nothing
        Dim human As HumanUnit = Nothing
        Select Case source
            Case "Left"
                origin = left
            Case "Right"
                origin = right
            Case "Self"
                origin = self
        End Select
        Select Case target
            Case "left"
                human = left
            Case "right"
                human = right
            Case "self"
                human = self
        End Select
        origin.Durability -= 50
        human.Durability += 50
    End Sub

    <SystemCall("Inspect entire human list")>
    Public Sub InspectHumans()
        For Each human In GUIDs
            If human.Value.GetType() Is GetType(HumanUnit) Then
                Console.WriteLine(DirectCast(human.Value, HumanUnit).UnitID + " " + DirectCast(human.Value, HumanUnit).UnitName)
            End If
        Next
    End Sub
End Module

Public Class HumanUnit
    Public Property Durability As Integer = 100
    Public Property UnitID As String
    Public Property UnitName As String
    Public Property SystemControlAuthority As Integer
    Public Property ObjectControlAuthority As Integer
    Public Sub Kick()
        Durability = 0
    End Sub
    Public Shared Operator =(orig As HumanUnit, dest As HumanUnit)
        Return orig.UnitID = dest.UnitID
    End Operator
    Public Shared Operator <>(orig As HumanUnit, dest As HumanUnit)
        Return orig.UnitID <> dest.UnitID
    End Operator
    Public Sub New()

    End Sub
    Public Sub New(computer As DirectoryEntry)
        UnitID = computer.Guid.ToString()
        UnitName = computer.Name
        SystemControlAuthority = 5 ' Un computer non può avere un'autorità più alta
        ObjectControlAuthority = 0 ' Non ha un corpo
    End Sub
End Class