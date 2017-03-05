<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
    Inherits System.Windows.Forms.Form

    'Form reemplaza a Dispose para limpiar la lista de componentes.
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

    'Requerido por el Diseñador de Windows Forms
    Private components As System.ComponentModel.IContainer

    'NOTA: el Diseñador de Windows Forms necesita el siguiente procedimiento
    'Se puede modificar usando el Diseñador de Windows Forms.  
    'No lo modifique con el editor de código.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.txtProxies = New System.Windows.Forms.TextBox()
        Me.lbWProxies = New System.Windows.Forms.ListBox()
        Me.lblProxies = New System.Windows.Forms.Label()
        Me.lblWProxies = New System.Windows.Forms.Label()
        Me.btnGo = New System.Windows.Forms.Button()
        Me.btnClean = New System.Windows.Forms.Button()
        Me.btnExport = New System.Windows.Forms.Button()
        Me.SaveFileDialog1 = New System.Windows.Forms.SaveFileDialog()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.ArchivoToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CargarDocumentoToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SalirToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'txtProxies
        '
        Me.txtProxies.Location = New System.Drawing.Point(15, 49)
        Me.txtProxies.Multiline = True
        Me.txtProxies.Name = "txtProxies"
        Me.txtProxies.Size = New System.Drawing.Size(193, 173)
        Me.txtProxies.TabIndex = 0
        '
        'lbWProxies
        '
        Me.lbWProxies.FormattingEnabled = True
        Me.lbWProxies.Location = New System.Drawing.Point(214, 49)
        Me.lbWProxies.Name = "lbWProxies"
        Me.lbWProxies.Size = New System.Drawing.Size(193, 173)
        Me.lbWProxies.TabIndex = 1
        '
        'lblProxies
        '
        Me.lblProxies.Location = New System.Drawing.Point(12, 33)
        Me.lblProxies.Name = "lblProxies"
        Me.lblProxies.Size = New System.Drawing.Size(193, 13)
        Me.lblProxies.TabIndex = 2
        Me.lblProxies.Text = "Proxies (IP:PORT)"
        '
        'lblWProxies
        '
        Me.lblWProxies.Location = New System.Drawing.Point(211, 33)
        Me.lblWProxies.Name = "lblWProxies"
        Me.lblWProxies.Size = New System.Drawing.Size(196, 13)
        Me.lblWProxies.TabIndex = 3
        Me.lblWProxies.Text = "Proxies funcionales"
        '
        'btnGo
        '
        Me.btnGo.Location = New System.Drawing.Point(15, 261)
        Me.btnGo.Name = "btnGo"
        Me.btnGo.Size = New System.Drawing.Size(193, 27)
        Me.btnGo.TabIndex = 4
        Me.btnGo.Text = "Escanear"
        Me.btnGo.UseVisualStyleBackColor = True
        '
        'btnClean
        '
        Me.btnClean.Location = New System.Drawing.Point(15, 228)
        Me.btnClean.Name = "btnClean"
        Me.btnClean.Size = New System.Drawing.Size(193, 27)
        Me.btnClean.TabIndex = 5
        Me.btnClean.Tag = "Esto hará que se limpie el texto no deseado, y las proxies repetidas."
        Me.btnClean.Text = "Limpiar proxies"
        Me.btnClean.UseVisualStyleBackColor = True
        '
        'btnExport
        '
        Me.btnExport.Location = New System.Drawing.Point(214, 228)
        Me.btnExport.Name = "btnExport"
        Me.btnExport.Size = New System.Drawing.Size(193, 27)
        Me.btnExport.TabIndex = 6
        Me.btnExport.Text = "Exportar..."
        Me.btnExport.UseVisualStyleBackColor = True
        '
        'SaveFileDialog1
        '
        Me.SaveFileDialog1.Filter = "Text File|*.txt"
        Me.SaveFileDialog1.RestoreDirectory = True
        Me.SaveFileDialog1.Title = "¿Dónde exportar?"
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ArchivoToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(420, 24)
        Me.MenuStrip1.TabIndex = 0
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'ArchivoToolStripMenuItem
        '
        Me.ArchivoToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CargarDocumentoToolStripMenuItem, Me.ToolStripSeparator1, Me.SalirToolStripMenuItem})
        Me.ArchivoToolStripMenuItem.Name = "ArchivoToolStripMenuItem"
        Me.ArchivoToolStripMenuItem.Size = New System.Drawing.Size(60, 20)
        Me.ArchivoToolStripMenuItem.Text = "Archivo"
        '
        'CargarDocumentoToolStripMenuItem
        '
        Me.CargarDocumentoToolStripMenuItem.Name = "CargarDocumentoToolStripMenuItem"
        Me.CargarDocumentoToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.A), System.Windows.Forms.Keys)
        Me.CargarDocumentoToolStripMenuItem.Size = New System.Drawing.Size(216, 22)
        Me.CargarDocumentoToolStripMenuItem.Text = "Cargar documento"
        '
        'SalirToolStripMenuItem
        '
        Me.SalirToolStripMenuItem.Name = "SalirToolStripMenuItem"
        Me.SalirToolStripMenuItem.Size = New System.Drawing.Size(216, 22)
        Me.SalirToolStripMenuItem.Text = "Salir"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(213, 6)
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.Filter = "Text File|*.txt"
        Me.OpenFileDialog1.Title = "Nombre del archivo que desea cargar"
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(420, 330)
        Me.Controls.Add(Me.btnExport)
        Me.Controls.Add(Me.btnClean)
        Me.Controls.Add(Me.btnGo)
        Me.Controls.Add(Me.lblWProxies)
        Me.Controls.Add(Me.lblProxies)
        Me.Controls.Add(Me.lbWProxies)
        Me.Controls.Add(Me.txtProxies)
        Me.Controls.Add(Me.MenuStrip1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MainMenuStrip = Me.MenuStrip1
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmMain"
        Me.Text = "Proxy Cheker"
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents txtProxies As System.Windows.Forms.TextBox
    Friend WithEvents lbWProxies As System.Windows.Forms.ListBox
    Friend WithEvents lblProxies As System.Windows.Forms.Label
    Friend WithEvents lblWProxies As System.Windows.Forms.Label
    Friend WithEvents btnGo As System.Windows.Forms.Button
    Friend WithEvents btnClean As System.Windows.Forms.Button
    Friend WithEvents btnExport As System.Windows.Forms.Button
    Friend WithEvents SaveFileDialog1 As System.Windows.Forms.SaveFileDialog
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents ArchivoToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CargarDocumentoToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents SalirToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog

End Class
