Imports NCurses.Lib.Curses
Imports NCurses.Lib

Module HumanUnitDesigner
    Public Sub HumanUnitDesigner()
        If Not Interactive Then Throw New InvalidOperationException("Interactive command handler is required.")
        Clear()
        Console.WindowWidth = 150
        Data.Add("huid", New CursesData("Human Unit ID: ", 2, 2, ConsoleColor.Yellow))
        Refresh()
        Dim hu = Console.ReadLine()
        Data.Add("huidvalue", New CursesData(hu, Len("Human Unit ID: ") + 2, 2))
        Data.Add("huname", New CursesData("Human Unit name: ", 2, 4, ConsoleColor.Green))
        Refresh()
        Dim huname = Console.ReadLine()
        Data.Add("hunamevalue", New CursesData(huname, Len("Human Unit name: ") + 2, 4))
        Data.Add("husca", New CursesData("System Control Authority: ", 2, 6, ConsoleColor.Red))
        Refresh()
        Dim sca = Console.ReadLine()
        Data.Add("huscavalue", New CursesData(sca, Len("System Control Authority: ") + 2, 6))
        Data.Add("huoca", New CursesData("Object Control Authority: ", 2, 8, ConsoleColor.Blue))
        Refresh()
        Dim os = Console.ReadLine()
        Data.Add("huocavalue", New CursesData(os, Len("System Control Authority: ") + 2, 8))
        Data.Add("hudur", New CursesData("Durability: ", 2, 10, ConsoleColor.Cyan))
        Refresh()
        Dim durability = Console.ReadLine
        Dim units = GUIDs.Select(Of HumanUnit)(Function(a As KeyValuePair(Of String, Object))
                                                   If a.Value.GetType() Is GetType(HumanUnit) Then
                                                       Return a.Value
                                                   Else
                                                       Return Nothing
                                                   End If
                                               End Function)
        Dim pages = units.Count() / (Console.WindowHeight - 5)
        Dim drawnData As Integer
        Dim curPage As Integer = 0
        Dim key As ConsoleKeyInfo
        Dim isZero = False, isSettingDelegate = False
        Dim [delegate] As Integer = -1
        Dim buf As Integer = 0
        Dim choices As List(Of Integer) = New List(Of Integer)
        While key.Key <> ConsoleKey.Enter
            Clear()
            Data.Clear()
            Data.Add("friends", New CursesData("Friends", 2, 2))
            For i As Integer = 0 To Console.WindowHeight - 5
                If units(curPage * (Console.WindowHeight - 5) + i) IsNot Nothing Then
                    drawnData += 1
                    Data.Add("hu" + i.ToString(), New CursesData(i.ToString() + ". " + units(curPage * (Console.WindowHeight - 5) + i).UnitName + " " + If(choices.Contains(i + curPage * (Console.WindowHeight - 5)), "[x]", "[ ]"), 2, 2 + i, If([delegate] = i + curPage * (Console.WindowHeight - 5), ConsoleColor.Green, ConsoleColor.White)))
                End If
            Next
            Refresh()
            key = Console.ReadKey()
            If key.Key = ConsoleKey.PageDown Then
                If curPage < pages - 1 Then
                    curPage += 1
                End If
            ElseIf key.Key = ConsoleKey.D Then
                isSettingDelegate = True
            Else
                If buf < 10 And Not isZero Then
                    If key.KeyChar = "0" Then
                        isZero = True
                    End If
                    buf = Val(key.KeyChar) * 10
                ElseIf buf < 100 Then
                    buf = buf + Val(key.KeyChar)
                    If isSettingDelegate Then
                        [delegate] = buf + curPage * (Console.WindowHeight - 5)
                        isSettingDelegate = False
                    Else
                        choices.Add(buf + curPage * (Console.WindowHeight - 5))
                    End If
                    buf = 0
                    isZero = False
                End If
            End If
        End While
        Dim xhu = New HumanUnit()
        xhu.UnitID = hu
        xhu.UnitName = huname
        xhu.Durability = Val(durability)
        xhu.SystemControlAuthority = Val(sca)
        xhu.ObjectControlAuthority = Val(os)
        For Each i As Integer In choices
            If units(i) IsNot Nothing Then
                xhu.Friends.Add(units(i))
            End If
        Next
        If [delegate] <> -1 Then
            xhu.Delegate = units([delegate])
        End If
        GUIDs(hu) = xhu
        Console.WriteLine("Compiling human unit. Done")
        Console.WriteLine("Generating fluctlight. Failed: permission denied (only My Hero Quirkly supports Fluctlight generation. This is Windows)")
        Console.WriteLine("Generating entity. Done")
        InspectSingleHuman(hu)
    End Sub
End Module
