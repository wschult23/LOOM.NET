' Windows Installer utility to execute SQL statements against an installer database
' For use with Windows Scripting Host, CScript.exe or WScript.exe
' Copyright (c) Microsoft Corporation. All rights reserved.
' Demonstrates the script-driven database queries and updates
'
Option Explicit


' Connect to Windows installer object
On Error Resume Next
Dim installer : Set installer = Nothing
Set installer = Wscript.CreateObject("WindowsInstaller.Installer") : CheckError

' Open database

Dim database : Set database = installer.OpenDatabase("..\..\Installer\LoomInstaller.msi", 1): CheckError

' Process SQL statements
Dim view, msicommand, record
Set view = database.OpenView("SELECT Component_ FROM File WHERE FileName='GL.EXE|gl.exe'") : CheckError
view.Execute : CheckError

Set record = view.Fetch
If record Is Nothing Then 
	WScript.Echo "Component Not found"
	Wscript.Quit 2
End If
msicommand = "INSERT INTO Environment(Environment, Name, Value, Component_) VALUES ('1', '=LOOMBIN', '[TARGETDIR]bin', '" & record.StringData(1) & "')"
Set view = database.OpenView(msicommand) : CheckError
WScript.Echo msicommand
view.Execute : CheckError

msicommand = "INSERT INTO Environment(Environment, Name, Value, Component_) VALUES ('2', '=LOOMLIB', '[PersonalFolder]The Loom.Net Project\lib', '" & record.StringData(1) & "')"
Set view = database.OpenView(msicommand) : CheckError
WScript.Echo msicommand
view.Execute : CheckError

msicommand = "INSERT INTO InstallExecuteSequence(Action,Condition,Sequence) VALUES ('ScheduleReboot', '', '2250')"
Set view = database.OpenView(msicommand) : CheckError
WScript.Echo msicommand
view.Execute : CheckError

database.Commit
Wscript.Quit 0

Sub CheckError
	Dim message, errRec
	If Err = 0 Then Exit Sub
	message = Err.Source & " " & Hex(Err) & ": " & Err.Description
	If Not installer Is Nothing Then
		Set errRec = installer.LastErrorRecord
		If Not errRec Is Nothing Then message = message & vbLf & errRec.FormatText
	End If
	Fail message
End Sub

Sub Fail(message)
	Wscript.Echo message
	Wscript.Quit 2
End Sub
