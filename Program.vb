Imports System
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Security.Principal
Imports System.Text.RegularExpressions

Public Class SystemCall
    Inherits Attribute
    Public RegEx As Regex
    Public Sub New(RegEx As String)
        Me.RegEx = New Regex(RegEx)
    End Sub
End Class

Public Class SystemContext
    Inherits Attribute
    Public Name As String
    Public Sub New(Name As String)
        Me.Name = Name
    End Sub
End Class

Module Program

    Public Property User As String = WindowsPrincipal.Current.Identity.Name
    Public Property SCA As Integer = 1
    Public Property Commands As Dictionary(Of Regex, MethodInfo) = New Dictionary(Of Regex, MethodInfo)()

    Sub ImportModule(name As String)
        Dim amod = Type.GetType(name)
        Dim methods = amod.GetMethods()
        For Each method As MethodInfo In methods
            If method.CustomAttributes.Count <> 0 AndAlso method.CustomAttributes.First().AttributeType = GetType(SystemCall) Then
                Dim [structure] As String = method.CustomAttributes.First().ConstructorArguments.First().Value
                Commands(New Regex([structure])) = method
                VWrite("Imported function " + method.Name + " with structure " + [structure])
            End If
        Next
    End Sub

    Public Verbose As Boolean = False

    Public Sub VWrite(text As String)
        If (Verbose) Then
            Console.WriteLine(text)
        End If
    End Sub

    Sub Invoke(commandbuffer As String, <CallerMemberName()> Optional sender As String = "")
        For Each Command As KeyValuePair(Of Regex, MethodInfo) In Commands
            If Command.Key.Match(commandbuffer).Success Then
                Dim cargs As List(Of String) = New List(Of String)()
                For Each group As Group In Command.Key.Match(commandbuffer).Groups
                    cargs.Add(group.Value)
                Next
                cargs.RemoveAt(0)
                'Try
                Command.Value.Invoke(Nothing, cargs.ToArray())
                Return
                'Catch ex As Exception
                'If My.Settings.UseTechnicalNames Then
                'Console.WriteLine("The Shell did not return a value, or the System Call could not be invoked.")
                'Console.WriteLine("Error id: " + ex.Message)
                'Else
                'onsole.WriteLine("The invocation of the command raised an error. Ask a minister.")
                ' End If
                'Debug.Fail(ex.Source, ex.Message)
                'Debug.Assert(False)
                'End Try
            End If
        Next
    End Sub

    Sub Main(args As String())
        Dim commandBuffer As String = ""

        ' Analyze modules
        ImportModule("sccommand.Program")
        ImportModule("sccommand.Time")
        ImportModule("sccommand.RootCommands")
        ImportModule("sccommand.HumanUnitManagement")
#If SECRET Then
        ImportModule("sccommand.Secret")
        Secret.DidImport()
#End If
        HumanUnitManagement.Init()



        ImportModule("sccommand.Reflection")


        For Each arg As String In args
            If arg.StartsWith("/") Then
                Select Case arg
                    Case "/V"
                        Version()
                        Return
                    Case "/v"
                        Verbose = True
                End Select
            Else
                commandBuffer += arg + " "
            End If
        Next

        commandBuffer = commandBuffer.Trim()

        If commandBuffer = "" Then
            While commandBuffer <> "Exit"
                commandBuffer = Console.ReadLine()
                Invoke(commandBuffer)
            End While
        Else
            Invoke(commandBuffer)
        End If




        VWrite(commandBuffer)
    End Sub

    ''' <summary>
    ''' Prints the version of sccommand.
    ''' </summary>
    Sub Version()
        Console.WriteLine("sccommand v. 1.0.0 - A System Call parser")
    End Sub

    <SystemCall("Login (?<User>.*)")>
    Sub Login(user As String)
        Dim amod = Type.GetType(user)
        If amod Is Nothing Then
            amod = Type.GetType("sccommand." + user)
        End If
        amod.InvokeMember("DidLogin", BindingFlags.InvokeMethod, Nothing, Nothing, Nothing)
        Console.WriteLine("Login succeeded as " + user)
    End Sub

    <SystemCall("Print (.*)")>
    Sub Print(message As String)
        Console.WriteLine(message)
    End Sub

    Property GUIDs As Dictionary(Of String, Object) = New Dictionary(Of String, Object)()

    <SystemCall("Show message box. Message (?<Message>.*)")>
    Sub MessageBox(message As String)
        MsgBox(message)
    End Sub

    <SystemCall("Use (technical|simple) terms")>
    Sub SetTechnical(mode As String)
        If mode = "simple" Then
            My.Settings.UseTechnicalNames = False
        Else
            My.Settings.UseTechnicalNames = True
        End If
        My.Settings.Save()
    End Sub

    <SystemCall("Inspect entire command list")>
    Sub InspectCommandList()
        For Each command As KeyValuePair(Of Regex, MethodInfo) In Commands
            Dim name As String = command.Value.CustomAttributes.First().ConstructorArguments.First().Value
            Dim context As String = command.Value.ReflectedType.Name
            Console.WriteLine(context + "      " + name)
        Next
    End Sub

    <SystemCall("Disadhere (.*) (neutral|low|high|very low|very high) oxidation (.*)")>
    Sub Disadhere(base As String, oxidation As String, target As String)
        If GUIDs(base).GetType() Is GetType(HumanUnit) Then
            Console.WriteLine("Using HumanUnit disadherence command...")
            Console.WriteLine("Ignored parameter 'oxidation' (" + oxidation + ")")
            HumanUnitManagement.Disadhere(GUIDs(base), GUIDs(target))
        End If
    End Sub

    <SystemCall("Adhere (.*) (neutral|low|high|very low|very high) oxidation (.*)")>
    Sub Adhere(base As String, oxidation As String, target As String)
        If GUIDs(base).GetType() Is GetType(HumanUnit) Then
            Console.WriteLine("Using HumanUnit adherence command...")
            Console.WriteLine("Ignored parameter 'oxidation' (" + oxidation + ")")
            HumanUnitManagement.Adhere(GUIDs(base), GUIDs(target))
        End If
    End Sub

    <SystemCall("Generate (luminous|thermal|umbral|windowed|controlled|programmed|metallic|cryogenic) element(. Form element (.*) shape)?(.)?")>
    Sub Generate(type As String, fe As String, shape As String, dot As String)
        Select Case type
            Case "luminous"
                Dim luminous As System.Windows.Forms.Form = New Windows.Forms.Form()
                luminous.SetDesktopBounds(My.Computer.Screen.Bounds.X, My.Computer.Screen.Bounds.Y, My.Computer.Screen.Bounds.Width, My.Computer.Screen.Bounds.Height)
                luminous.BackColor = Drawing.Color.White
                luminous.Text = "NBTD-" + Str(Math.Floor(Rnd() * 10000)) + "<Luminous>"
                Select Case shape
                    Case "sword"
                        Console.WriteLine("The sword shape does not exist for the luminous element. Ignoring parameter.")
                    Case Else
                        Console.WriteLine("No such shape: " + shape)
                End Select
                luminous.ShowDialog()
            Case "thermal"
                Dim luminous As System.Windows.Forms.Form = New Windows.Forms.Form()
                luminous.SetDesktopBounds(My.Computer.Screen.Bounds.X, My.Computer.Screen.Bounds.Y, My.Computer.Screen.Bounds.Width, My.Computer.Screen.Bounds.Height)
                If IO.Directory.Exists("C:\WINDOWS\MuscleDB") Then
                    luminous.BackgroundImage = New Drawing.Bitmap(IO.Directory.GetFiles("C:\WINDOWS\MuscleDB\" + shape + ".pd\")(0))
                Else
                    luminous.BackColor = Drawing.Color.LightGoldenrodYellow
                End If
                luminous.Text = "NBTD-" + Str(Math.Floor(Rnd() * 10000)) + "<Thermal>"
                luminous.ShowDialog()
            Case "umbral"
                Process.Start("http://meteo.it")
            Case "windowed"
                Dim w As System.Windows.Forms.Form = New Windows.Forms.Form()
                Dim g As Guid = Guid.NewGuid()
                Console.WriteLine(g.ToString())
                GUIDs.Add(g.ToString(), w)
                w.Show()
            Case "programmed"
                Dim p As ProcessStartInfo = New ProcessStartInfo("C:\WINDOWS\SYSTEM32\CMD.EXE", "/C " + shape)
                Dim p2 = Process.Start(p)
            Case "metallic"
                Dim luminous As System.Windows.Forms.Form = New Windows.Forms.Form()
                luminous.SetDesktopBounds(My.Computer.Screen.Bounds.X, My.Computer.Screen.Bounds.Y, My.Computer.Screen.Bounds.Width, My.Computer.Screen.Bounds.Height)
                luminous.BackColor = Drawing.Color.DimGray
                luminous.Text = "NBTD-" + Str(Math.Floor(Rnd() * 10000)) + "<Metallic>"
                luminous.ShowDialog()
        End Select
    End Sub

    <SystemCall("Close window (.*)")>
    Sub CloseWindow(Guid As String)
        Dim w As Windows.Forms.Form = GUIDs(Guid)
        w.Close()
    End Sub

    <SystemCall("Get user info")>
    Sub UserInfo()
        Console.WriteLine("User name: {0}. System Control Authority: {1}", User, SCA)
    End Sub

    <SystemCall("Read file (?<FileName>.*)")>
    Sub ReadFile(file As String)
        Console.WriteLine(IO.File.ReadAllText(file))
    End Sub

    <SystemCall("Multi arged void (?<A>[a-z]*) to (?<V>[a-z]*)")>
    Sub MultiArgumentedVoid(x As String, y As String)
        Console.WriteLine("Ascissa " + x + " ordinata " + y)
    End Sub

    <SystemCall("Play sound at (?<Path>.*)")>
    Sub PlaySound(sound As String)
        My.Computer.Audio.Play(sound)
    End Sub

    <SystemCall("Iterate through (.*): (.*)")>
    Sub Iterate(array As String, command As String)
        Dim obj = InterpretObject(array)
        If GetType(IEnumerable).IsAssignableFrom(obj.GetType()) Then
            For Each item In obj
                GUIDs("item") = item
                Invoke(command)
            Next
            GUIDs.Remove("item")
            Return
        Else
            Console.WriteLine("Not iteratable")
        End If
    End Sub

    Function InterpretObject([object] As String) As Object
        If [object].Contains("of") Then
            Dim arrayOf = [object].Split({"of"}, StringSplitOptions.RemoveEmptyEntries)
            Dim reversed = arrayOf.Reverse()
            Dim consider As Object
            For i As Integer = 0 To reversed.Count - 1
                If i = 0 Then
                    consider = GUIDs(reversed(i).Trim())
                    Continue For
                End If
#Disable Warning BC42104 ' La variabile è stata usata prima dell'assegnazione di un valore
                consider = TryCast(consider.GetType().GetMember(reversed(i).Trim())(0), PropertyInfo).GetValue(consider)
#Enable Warning BC42104 ' La variabile è stata usata prima dell'assegnazione di un valore
            Next
            Return consider
        End If
        Return Nothing
    End Function

End Module

Module Time

    <SystemCall("Get local time")>
    Public Sub GetTime()
        Console.WriteLine(My.Computer.Clock.LocalTime)
    End Sub

    <SystemCall("Get delta time")>
    Public Sub DeltaTime()
        Console.WriteLine(My.Computer.Clock.TickCount / 1000)
    End Sub

    <SystemCall("Rewind (?<Time>[0-9]*) seconds")>
    Public Sub Rewind(timeS As String)
        Dim time As Integer = Val(timeS)
        Windows.Forms.SendKeys.SendWait(Windows.Forms.Keys.MediaPreviousTrack)
        Console.WriteLine("PakyMorl rewound by " + (time * Math.Sqrt(2)).ToString() + " seconds.")
    End Sub
End Module

<SystemContext("RootContext")>
Module RootCommands
    <SystemCall("Check root status")>
    Public Sub CheckRoot()
        Console.WriteLine(IIf(New WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator), "Root mode enabled", "Root mode disabled"))
    End Sub
End Module