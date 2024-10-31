﻿Partial Class Form1
    ' Initialize form components
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Me.headerPanel = New System.Windows.Forms.Panel()
        Me.titleLabel = New System.Windows.Forms.Label()
        Me.instructionLabel = New System.Windows.Forms.Label()
        Me.inputTextBox = New System.Windows.Forms.TextBox()
        Me.activateButton = New System.Windows.Forms.Button()
        Me.exitButton = New System.Windows.Forms.Button()
        Me.footerPanel = New System.Windows.Forms.Panel()
        Me.animationTimer = New System.Windows.Forms.Timer(Me.components)
        Me.VisualEffectTimer = New System.Windows.Forms.Timer(Me.components)
        Me.headerPanel.SuspendLayout()
        Me.footerPanel.SuspendLayout()
        Me.SuspendLayout()
        '
        'headerPanel
        '
        Me.headerPanel.BackColor = System.Drawing.Color.FromArgb(CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer))
        Me.headerPanel.Controls.Add(Me.titleLabel)
        Me.headerPanel.Dock = System.Windows.Forms.DockStyle.Top
        Me.headerPanel.Location = New System.Drawing.Point(0, 0)
        Me.headerPanel.Name = "headerPanel"
        Me.headerPanel.Size = New System.Drawing.Size(600, 80)
        Me.headerPanel.TabIndex = 0
        '
        'titleLabel
        '
        Me.titleLabel.AutoSize = True
        Me.titleLabel.Font = New System.Drawing.Font("Arial", 20.0!, System.Drawing.FontStyle.Bold)
        Me.titleLabel.ForeColor = System.Drawing.Color.White
        Me.titleLabel.Location = New System.Drawing.Point(30, 20)
        Me.titleLabel.Name = "titleLabel"
        Me.titleLabel.Size = New System.Drawing.Size(403, 32)
        Me.titleLabel.TabIndex = 0
        Me.titleLabel.Text = "Antivirus Defender 10 Runner"
        '
        'instructionLabel
        '
        Me.instructionLabel.AutoSize = True
        Me.instructionLabel.Font = New System.Drawing.Font("Segoe UI", 12.0!)
        Me.instructionLabel.ForeColor = System.Drawing.Color.White
        Me.instructionLabel.Location = New System.Drawing.Point(120, 100)
        Me.instructionLabel.Name = "instructionLabel"
        Me.instructionLabel.Size = New System.Drawing.Size(213, 21)
        Me.instructionLabel.TabIndex = 1
        Me.instructionLabel.Text = "Enter the key to run the virus:"
        '
        'inputTextBox
        '
        Me.inputTextBox.BackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer))
        Me.inputTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.inputTextBox.Font = New System.Drawing.Font("Segoe UI", 12.0!)
        Me.inputTextBox.ForeColor = System.Drawing.Color.White
        Me.inputTextBox.Location = New System.Drawing.Point(100, 140)
        Me.inputTextBox.Name = "inputTextBox"
        Me.inputTextBox.Size = New System.Drawing.Size(400, 29)
        Me.inputTextBox.TabIndex = 2
        '
        'activateButton
        '
        Me.activateButton.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(200, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.activateButton.FlatAppearance.BorderSize = 0
        Me.activateButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.activateButton.Font = New System.Drawing.Font("Segoe UI", 14.0!, System.Drawing.FontStyle.Bold)
        Me.activateButton.ForeColor = System.Drawing.Color.White
        Me.activateButton.Location = New System.Drawing.Point(150, 200)
        Me.activateButton.Name = "activateButton"
        Me.activateButton.Size = New System.Drawing.Size(300, 50)
        Me.activateButton.TabIndex = 3
        Me.activateButton.Text = "Run Malware"
        Me.activateButton.UseVisualStyleBackColor = False
        '
        'exitButton
        '
        Me.exitButton.BackColor = System.Drawing.Color.Red
        Me.exitButton.FlatAppearance.BorderSize = 0
        Me.exitButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.exitButton.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.exitButton.ForeColor = System.Drawing.Color.White
        Me.exitButton.Location = New System.Drawing.Point(240, 10)
        Me.exitButton.Name = "exitButton"
        Me.exitButton.Size = New System.Drawing.Size(120, 50)
        Me.exitButton.TabIndex = 0
        Me.exitButton.Text = "Exit"
        Me.exitButton.UseVisualStyleBackColor = False
        '
        'footerPanel
        '
        Me.footerPanel.BackColor = System.Drawing.Color.FromArgb(CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.footerPanel.Controls.Add(Me.exitButton)
        Me.footerPanel.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.footerPanel.Location = New System.Drawing.Point(0, 350)
        Me.footerPanel.Name = "footerPanel"
        Me.footerPanel.Size = New System.Drawing.Size(600, 50)
        Me.footerPanel.TabIndex = 4
        '
        'VisualEffectTimer
        '
        '
        'Form1
        '
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(600, 400)
        Me.Controls.Add(Me.headerPanel)
        Me.Controls.Add(Me.instructionLabel)
        Me.Controls.Add(Me.inputTextBox)
        Me.Controls.Add(Me.activateButton)
        Me.Controls.Add(Me.footerPanel)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "Form1"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.headerPanel.ResumeLayout(False)
        Me.headerPanel.PerformLayout()
        Me.footerPanel.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    ' Draw a gradient background to create a glowing effect
    Private Sub Form1_Paint(sender As Object, e As PaintEventArgs)
        Dim g As Graphics = e.Graphics
        Dim brush As New Drawing2D.LinearGradientBrush(Me.ClientRectangle, Color.FromArgb(30, 30, 30), Color.FromArgb(80, 80, 80), 45)
        g.FillRectangle(brush, Me.ClientRectangle)
    End Sub

    ' Draw glow around the title label
    Private Sub HeaderPanel_Paint(sender As Object, e As PaintEventArgs)
        Dim g As Graphics = e.Graphics
        Dim glowBrush As New SolidBrush(Color.FromArgb(50, 255, 255, 255))
        Dim glowSize As Integer = 4
        Dim titleRect As New Rectangle(Me.titleLabel.Location.X - glowSize, Me.titleLabel.Location.Y - glowSize, Me.titleLabel.Width + glowSize * 2, Me.titleLabel.Height + glowSize * 2)

        ' Draw the glowing effect
        g.DrawString(Me.titleLabel.Text, Me.titleLabel.Font, glowBrush, titleRect.Location)
    End Sub

    ' Draw glow effect for buttons
    Private Sub Button_Paint(sender As Object, e As PaintEventArgs)
        Dim button As Button = CType(sender, Button)
        Dim g As Graphics = e.Graphics
        Dim glowBrush As New SolidBrush(Color.FromArgb(50, 255, 255, 255))
        Dim glowSize As Integer = 4
        Dim buttonRect As New Rectangle(-glowSize, -glowSize, button.Width + glowSize * 2, button.Height + glowSize * 2)

        ' Draw the glowing effect
        g.DrawString(button.Text, button.Font, glowBrush, New PointF(5, 5))
    End Sub

    Friend WithEvents animationTimer As Timer
    Private components As System.ComponentModel.IContainer
    Friend WithEvents VisualEffectTimer As Timer
End Class