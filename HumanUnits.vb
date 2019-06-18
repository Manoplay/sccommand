Imports System.DirectoryServices.AccountManagement
Imports System.IO
Imports System.Xml.Serialization

Module HumanUnitManagement
    <SystemCall("Kick player (.*)")>
    Public Sub KickPlayer(player As String)
        Dim id As HumanUnit = GUIDs(player)
        If id IsNot Nothing Then
            id.Kick()
        End If
    End Sub

    Private ReadOnly Property cardinal As HumanUnit
        Get
            Dim h As HumanUnit = New HumanUnit()
            h.Durability = 150000
            h.SystemControlAuthority = 100
            h.ObjectControlAuthority = 100
            h.UnitID = "cardinal.exe"
            h.UnitName = "Cardinal"
            Return h
        End Get
    End Property

    Private ReadOnly Property toby As HumanUnit
        Get
            Dim h As HumanUnit = New HumanUnit()
            h.Durability = 1500000
            h.SystemControlAuthority = 101
            h.ObjectControlAuthority = 100
            h.UnitID = "d6ceea00-ac62-4a9f-af6c-af47c70409ad"
            h.UnitName = "Toby Parker"
            Return h
        End Get
    End Property

    Private ReadOnly Property kirito As HumanUnit
        Get
            Dim h As HumanUnit = New HumanUnit()
            h.Durability = 3289
            h.ObjectControlAuthority = 48
            h.SystemControlAuthority = 59
            h.UnitID = "NND7-6355"
            h.UnitName = "Kirito"
            Return h
        End Get
    End Property

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
        self = New HumanUnit() 'Genera l'entità locale
        self.SystemControlAuthority = Program.SCA
        self.ObjectControlAuthority = 0
        self.UnitID = Guid.NewGuid().ToString()
        self.UnitName = Environment.UserName
        GUIDs(self.UnitID) = self
        GUIDs("localhost") = GUIDs(self.UnitID)
        GUIDs(kirito.UnitID) = kirito
        GUIDs(toby.UnitID) = toby
        GUIDs(cardinal.UnitID) = cardinal
        ReloadUnitsFromDisk()
    End Sub

    Public Sub ReloadUnitsFromDisk()
        Dim root = "C:\Users\" + Environment.UserName + "\sccommand\HumanUnits\"
        If Directory.Exists(root) Then
            Dim files = Directory.GetFiles(root)
            For Each file In files
                Console.WriteLine(file)
                Dim hu = HumanUnit.Deserialize(file)
                GUIDs(hu.UnitID) = hu
            Next
        End If
    End Sub

    Public Sub Adhere(base As HumanUnit, target As HumanUnit)
        If base.Delegate = target Then
            Console.WriteLine(base.UnitName + " and " + target.UnitName + " are delegates. The operation could not be performed (NO WARN RECEIVED).")
            Return
        End If
        base.Friends.Add(target)
        Console.WriteLine("Adherence between " + base.UnitID + " (" + base.UnitName + ") and " + target.UnitID + " (" + target.UnitName + ") successfully applied.")
    End Sub

    <SystemCall("Reload")>
    Public Sub Reload()
        ReloadUnitsFromDisk()
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

    <SystemCall("Save human unit (.*)")>
    Public Sub Save(unitID As String)
        Dim a As HumanUnit = GUIDs(unitID)
        a.Serialize("C:\Users\" + Environment.UserName + "\sccommand\HumanUnits\" + a.UnitID + ".hu")
    End Sub

    <SystemCall("Inspect human (.*)")>
    Public Sub InspectSingleHuman(unitID As String)
        Dim a As HumanUnit = GUIDs(unitID)
        Console.WriteLine("Internal sccommand hash code: " + a.GetHashCode().ToString())
        Console.WriteLine("Unit ID: " + a.UnitID)
        Console.WriteLine(".NET Type: " + GetType(HumanUnit).AssemblyQualifiedName)
        Console.WriteLine("Shell Mirror Type: System.PublicInterface.Overworld.HumanUnit")
        Console.WriteLine("Unit name: " + a.UnitName)
        Console.WriteLine("System Control Authority: " + a.SystemControlAuthority.ToString())
        Console.WriteLine("Object Control Authority: " + a.ObjectControlAuthority.ToString())
        Console.WriteLine("Durability: " + a.Durability.ToString())
        If a.Delegate IsNot Nothing Then Console.WriteLine(If(My.Settings.UseTechnicalNames, "Delegate: ", "Best best friend: ") + a.Delegate.UnitName + " (" + a.Delegate.UnitID + ")")
        Console.WriteLine("Friends: ")
        If a.Friends.Count > 0 Then
            For i As Integer = 0 To a.Friends.Count - 1
                If a.Friends(i) IsNot Nothing Then Console.WriteLine(i.ToString() + ". " + a.Friends(i).UnitName + "(" + a.Friends(i).UnitID + ")")
            Next
        Else
            Console.WriteLine("1. Etere")
        End If
    End Sub

    Public Sub Disadhere(base As HumanUnit, target As HumanUnit)
        If base.Delegate = target Then
            Console.WriteLine(base.UnitName + " and " + target.UnitName + " are delegates. The operation could not be performed (NO WARN RECEIVED).")
            Return
        End If
        If base.Friends.Remove((base.SearchFriend(target.UnitID))) Then
            Console.WriteLine("Disadherence between " + base.UnitID + " (" + base.UnitName + ") and " + target.UnitID + " (" + target.UnitName + ") successfully applied.")
        Else
            Console.WriteLine("Disadherence between " + base.UnitID + " (" + base.UnitName + ") and " + target.UnitID + " (" + target.UnitName + ") not applied.")
        End If
    End Sub

    <SystemCall("Inspect entire human list")>
    Public Sub InspectHumans()
        For Each human In GUIDs
            If human.Value.GetType() Is GetType(HumanUnit) Then
                Console.WriteLine(DirectCast(human.Value, HumanUnit).UnitID + " " + DirectCast(human.Value, HumanUnit).UnitName)
            End If
        Next
    End Sub

    <SystemCall("Touch (.*) with (left|right) hand")>
    Public Sub Touch(guid As String, hand As String)
        If hand = "left" Then left = GUIDs(guid)
        If hand = "right" Then right = GUIDs(guid)
    End Sub

    <SystemCall("Generate human unit. Unit name (.*).( System Control Authority ([0-9]*))?(.)?( Object Control Authority ([0-9]*))?(.)?")>
    Public Sub AddHuman(Name As String, Optional pm1 As String = "", Optional sca As String = "1", Optional dot1 As String = "", Optional pm2 As String = "", Optional oca As String = "1", Optional dot2 As String = "")
        Dim h As HumanUnit = New HumanUnit()
        h.UnitID = Guid.NewGuid().ToString()
        h.UnitName = Name
        h.SystemControlAuthority = Val(sca)
        h.ObjectControlAuthority = Val(oca)
        GUIDs.Add(h.UnitID, h)
    End Sub

    <SystemCall("Link to human unit. Unit name (.*)")>
    Public Sub Point(name As String)
        Console.WriteLine("Failed to download " + name + ".ifo from http://80.103.90.150/users.php")
    End Sub

    <SystemCall("Check friendship between (.*) and (.*)")>
    Public Sub CheckFriendship(source As String, target As String)
        Dim baseUnit As HumanUnit = GUIDs(source)
        Dim targetUnit As HumanUnit = GUIDs(target)
#If MAGIC Then
        Dim magicMyHero As Process = Process.Start("Z:\MAGICSACKBOY\BOOTx128\MAGIC-MYHERO.DEKU.EXE", source + " " + target)
#Else
        If baseUnit.Delegate = targetUnit Then
            Console.WriteLine("2")
        ElseIf baseUnit.Friends.Contains(targetUnit) Then
            Console.WriteLine("1")
        Else
            Console.WriteLine("-1")
        End If
#End If
    End Sub

End Module

<Serializable>
<XmlRoot("Human")>
Public Class HumanUnit
    <XmlElement("Delegate")>
    Public Property delegateID As String = ""
    <XmlIgnore>
    Public Property [Delegate] As HumanUnit
        Get
            If GUIDs.ContainsKey(delegateID) Then Return GUIDs(delegateID) Else Return Nothing
        End Get
        Set(value As HumanUnit)
            delegateID = value.UnitID
        End Set
    End Property
    <XmlAttribute("Durability")>
    Public Property Durability As Integer = 100
    <XmlArray("Friends")>
    <XmlArrayItem("Friend")>
    Public Property StringFriends As List(Of String)
        Get
            Dim l As List(Of String) = New List(Of String)
            For Each item In Friends
                l.Add(item.UnitID)
            Next
            Return l
        End Get
        Set(value As List(Of String))
            Friends = New List(Of HumanUnit)
            For Each item In value
                Friends.Add(GUIDs(item))
            Next
        End Set
    End Property
    <XmlIgnore>
    Public Property Friends As List(Of HumanUnit) = New List(Of HumanUnit)()
    <XmlAttribute("UnitID")>
    Public Property UnitID As String
    <XmlAttribute("UnitName")>
    Public Property UnitName As String
    <XmlAttribute("SCA")>
    Public Property SystemControlAuthority As Integer = 0
    <XmlAttribute("OCA")>
    Public Property ObjectControlAuthority As Integer = 0
    Public Sub Kick()
        Durability = 0
    End Sub
    Public Sub New()

    End Sub

    Public Sub Serialize(outFile As String)
        Dim serializer = New XmlSerializer(GetType(HumanUnit))
        Dim fileStream As FileStream = File.Create(outFile)
        serializer.Serialize(fileStream, Me)
        fileStream.Close()
    End Sub

    Public Shared Function Deserialize(xmlFile As String) As HumanUnit
        Dim deserializer = New XmlSerializer(GetType(HumanUnit))
        Dim stream As FileStream = File.OpenRead(xmlFile)
        Dim output As HumanUnit = deserializer.Deserialize(stream)
        stream.Close()
        Return output
    End Function

    Public Shared Operator =(origin As HumanUnit, target As HumanUnit)
        Return origin.UnitID = target.UnitID
    End Operator

    Public Shared Operator <>(origin As HumanUnit, target As HumanUnit)
        Return origin.UnitID <> target.UnitID
    End Operator

    Public Function SearchFriend(UnitID As String) As HumanUnit
        For Each bff In Friends
            If bff.UnitID = UnitID Then
                Return bff
            End If
        Next
        Return Nothing
    End Function
End Class