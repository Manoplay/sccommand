Imports System.Reflection

Module Reflection

    <SystemCall("Get type info for (.*)")>
    Public Sub GetTypeInfo(type As String)
        Dim t As Type = System.Type.GetType(type, True, True)
        Console.WriteLine(".NET Reflection name: " + t.AssemblyQualifiedName)
        Console.WriteLine(".NET GUID: " + t.GUID.ToString())
        Console.WriteLine("Members:")
        For Each member In t.GetMembers()
            Console.WriteLine("- " + member.Name + " (" + member.MemberType.ToString() + ")")
        Next
    End Sub

    <SystemCall("Reflect type (.*)")>
    Public Sub ReflectType(type As String)
        Dim t As Type = System.Type.GetType(type, True, True)
        Dim g As String = Guid.NewGuid().ToString()
        GUIDs(g) = t
        Console.WriteLine(g)
    End Sub

    <SystemCall("Get method info for (.*) on (.*)")>
    Public Sub GetMethodInfo(method As String, type As String)
        Dim t As Type = System.Type.GetType(type, True, True)
        Dim m As MethodInfo = t.GetMember(method)(0)
        Console.WriteLine(".NET Reflection name: " + t.Name)
        Console.WriteLine(".NET GUID: " + t.GUID.ToString())
        Console.WriteLine(".NET Arguments:")
        For Each arg In m.GetGenericArguments()
            Console.WriteLine("- " + arg.FullName)
        Next
    End Sub

    <SystemCall("Generate reflected element. Form element (.*) shape.")>
    Public Sub GenerateReflected(form As String)
        Dim t As Type = System.Type.GetType(form, True, True)
        Dim s As String = Guid.NewGuid().ToString()
        GUIDs(s) = Activator.CreateInstance(t)
        Console.WriteLine(s)
    End Sub

    <SystemCall("Set property (.*) to (.*) for (.*)")>
    Public Sub SetProperty(prop As String, value As String, guid As String)
        Dim t As Type = GUIDs(guid).GetType()
        Dim p As PropertyInfo = t.GetMember(prop)(0)
        Dim o = p.PropertyType
        Select Case o.Name
            Case "String"
                p.SetValue(GUIDs(guid), value)
            Case "Integer"
                p.SetValue(GUIDs(guid), Val(value))
            Case "Boolean"
                p.SetValue(GUIDs(guid), value = "true")
            Case Else
                p.SetValue(GUIDs(guid), GUIDs(value))
        End Select
    End Sub

    <SystemCall("Invoke method (.*) on (.*)( with (.*))?")>
    Public Sub Invoke(method As String, type_guid As String, isArged As String, args As String)
        Try
            Dim t As Type = System.Type.GetType(type_guid, True, True)
            Dim m As MethodInfo = t.GetMember(method)(0)
            If isArged <> "" Then
                m.Invoke(Nothing, args.Split(";"))
            Else
                m.Invoke(Nothing, Nothing)
            End If
        Catch
            Dim t As Type = GUIDs(type_guid).GetType()
            Dim m As MethodInfo = t.GetMember(method)(0)
            If isArged <> "" Then
                m.Invoke(GUIDs(type_guid), args.Split(";"))
            Else
                m.Invoke(GUIDs(type_guid), Nothing)
            End If
        End Try
    End Sub

End Module
