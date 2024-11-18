<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form2
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form2))
        Me.CountDownTimer = New System.Windows.Forms.Timer(Me.components)
        Me.timerLabel = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'CountDownTimer
        '
        Me.CountDownTimer.Interval = 1000
        '
        'timerLabel
        '
        Me.timerLabel.AutoSize = True
        Me.timerLabel.Font = New System.Drawing.Font("Segoe UI", 20.0!, System.Drawing.FontStyle.Bold)
        Me.timerLabel.ForeColor = System.Drawing.Color.GreenYellow
        Me.timerLabel.Location = New System.Drawing.Point(3, 9)
        Me.timerLabel.Name = "timerLabel"
        Me.timerLabel.Size = New System.Drawing.Size(224, 37)
        Me.timerLabel.TabIndex = 0
        Me.timerLabel.Text = "Remaning Time:"
        '
        'Form2
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(230, 65)
        Me.Controls.Add(Me.timerLabel)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "Form2"
        Me.Opacity = 0.7R
        Me.Text = "Form2"
        Me.TopMost = True
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents CountDownTimer As Timer
    Friend WithEvents timerLabel As Label
End Class
