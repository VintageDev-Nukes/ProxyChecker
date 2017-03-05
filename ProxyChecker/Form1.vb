Imports System.IO
Imports System.Net
Imports System.Threading
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.ComponentModel
Imports System.Drawing
Imports System.Management

Enum labelType
    Proxies
    WorkingProxies
    Estado
End Enum

Enum stateType
    Scan
    Clean
    Export
    Load
End Enum

Public Class frmMain

    Private Event lbWChanged As EventHandler

    Private ReadOnly proxyPattern As String = "\b(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\:[0-9]{1,5}\b"

    Private IndexPattern, notWorkingProxiesCount, numberOfProxies, linesCleaned, proxiesToLoad As Integer

    Private secondsToRead, secondsToExport, secondsToClean, MaxTime As Double

    Private isCleaned, isStarted, isExported, isLoading As Boolean

    Dim filePath, loadFilePath As String

    Dim TimerStart As DateTime
    Dim TimeSpent As TimeSpan

    Dim WithEvents pbEstado As New TextProgressBar

    Private ReadOnly Property ProxyList As String()
        Get
            Return txtProxies.Lines
        End Get
    End Property

    Private ReadOnly Property WorkingProxiesList As ListBox.ObjectCollection
        Get
            Return lbWProxies.Items
        End Get
    End Property


    Public Function SortData(ByVal textbox As TextBox, ByVal size As Integer) As String()

        Dim items(size - 1) As String

        If size <> textbox.Lines.Count Then
            Throw New ArgumentOutOfRangeException
        Else
            For i As Integer = 0 To textbox.Lines.Count - 1
                items(i) = textbox.Lines(i)
            Next

            Array.Sort(items)
        End If

        Return items

    End Function

    Public Function RemoveDuplicates(ByVal items As String()) As String()

        Dim noDupsArrList As New ArrayList()

        For i As Integer = 0 To items.Length - 1
            If Not noDupsArrList.Contains(items(i).Trim()) Then
                noDupsArrList.Add(items(i).Trim())
                RaiseEvent lbWChanged(Me, New EventArgs)
            End If
        Next

        Dim uniqueItems As String() = New String(noDupsArrList.Count - 1) {}

        noDupsArrList.CopyTo(uniqueItems)

        Return uniqueItems

    End Function

    Public Function RemNotMatchingLines(items As String(), matchPattern As String) As String()
        Dim itemList As List(Of String) = items.ToList()
        For i As Integer = 0 To items.Length - 1
            If Not Regex.IsMatch(items(i), matchPattern) Then
                itemList.RemoveAt(i)
            End If
        Next
        Return itemList.ToArray()
    End Function

    Public Function md5(ByVal input As String) As String
        Dim x As New System.Security.Cryptography.MD5CryptoServiceProvider()
        Dim bs As Byte() = System.Text.Encoding.UTF8.GetBytes(Input)
        bs = x.ComputeHash(bs)
        Dim s As New System.Text.StringBuilder()
        For Each b As Byte In bs
            s.Append(b.ToString("x2").ToLower())
        Next
        Dim password As String = s.ToString()
        Return password
    End Function

    Public Function base64_encode(ByVal input As String) As String
        Dim bytesToEncode As Byte()
        bytesToEncode = Encoding.UTF8.GetBytes(input)

        Dim encodedText As String
        encodedText = Convert.ToBase64String(bytesToEncode)

        Return (encodedText)
    End Function

    Public Function base64_decode(ByVal input As String) As String
        Dim decodedBytes As Byte()
        decodedBytes = Convert.FromBase64String(input)

        Dim decodedText As String
        decodedText = Encoding.UTF8.GetString(decodedBytes)

        Return (decodedText)
    End Function

    Function getIP(Optional ByVal webPath As String = "http://gimmeahit.x10host.com/c/getip.php") As String
        Return New System.Net.WebClient().DownloadString(webPath)
    End Function

    Function customGUID() As String
        Dim s As String = md5(base64_encode(getIP.Replace(".", ""))).ToUpper
        Dim guidText As String = s.Substring(0, 8) & "-" & s.Substring(8, 4) & "-" & s.Substring(12, 4) & "-" & s.Substring(16, 4) & "-" & s.Substring(20)
        Return guidText
    End Function

    Private Shadows Sub Load() Handles MyBase.Load

        Dim CommandLineArgs As ObjectModel.ReadOnlyCollection(Of String) = My.Application.CommandLineArgs

        If CommandLineArgs.Count > 0 Then
            For i As Integer = 0 To CommandLineArgs.Count - 1
                If i = 0 And File.Exists(CommandLineArgs(i)) Then
                    loadFilePath = CommandLineArgs(i)
                    Dim thLoad As New Thread(AddressOf LoadDocument)
                    thLoad.Start()
                End If
            Next
        End If

        With pbEstado
            .Minimum = 0
            .Maximum = 100
            .Step = 1
            .Value = 0
            .Size = New Size(395, 24)
            .Location = New Point(12, 294)
        End With

        Me.Controls.Add(pbEstado)

        btnExport.Enabled = False
        btnGo.Enabled = False
        btnClean.Enabled = False

        MaxTime = 10

    End Sub

    Private Shadows Sub Shown() Handles MyBase.Shown

        SetTextInfo("Preparado para comenzar...", labelType.Estado)

    End Sub

    Public Shadows Sub ExitApp() Handles MyBase.FormClosed
        If WorkingProxiesList.Count > 0 And Not isExported Then
            If MsgBox("¿Desea guardar los cambios antes de salir?", MsgBoxStyle.YesNo, "?") = MsgBoxResult.Yes Then
                SaveFileDialog1.ShowDialog()
            End If
        Else
            If System.Diagnostics.Process.GetCurrentProcess().Threads.Count > 0 Then
                For Each t As System.Diagnostics.ProcessThread In System.Diagnostics.Process.GetCurrentProcess().Threads
                    t.Dispose()
                Next t
            End If
            Application.Exit()
        End If
    End Sub

    Private Sub LoadDocument()

        If Not File.Exists(loadFilePath) Then
            Exit Sub
        End If

        Dim contents As String = File.ReadAllText(loadFilePath)
        Dim proxies As MatchCollection = Regex.Matches(contents, proxyPattern)
        proxiesToLoad = proxies.Count

        If pbEstado.InvokeRequired Then pbEstado.Invoke(Sub() pbEstado.Maximum = proxiesToLoad + 1) Else pbEstado.Maximum = proxiesToLoad + 1

        isLoading = True
        For Each mtc As Match In proxies

            If txtProxies.InvokeRequired Then txtProxies.Invoke(Sub() txtProxies.Text += mtc.Value & Environment.NewLine) Else txtProxies.Text += mtc.Value & Environment.NewLine

        Next mtc
        isLoading = False

        If pbEstado.InvokeRequired Then
            pbEstado.Invoke(Sub() pbEstado.Maximum = 1)
            pbEstado.Invoke(Sub() pbEstado.Value = 1)
        Else
            pbEstado.Maximum = 1
            pbEstado.Value = 1
        End If

    End Sub

    Private Sub SetTextInfo(Optional str As String = "", Optional type As labelType = labelType.Proxies)
        If type = labelType.Proxies Then
            If lblProxies.InvokeRequired Then lblProxies.Invoke(Sub() lblProxies.Text = "Proxies (IP:PORT)") Else lblProxies.Text = "Proxies (IP:PORT)"
            If Not String.IsNullOrWhiteSpace(str) Then
                If lblProxies.InvokeRequired Then lblProxies.Invoke(Sub() lblProxies.Text += " " + str) Else lblProxies.Text += " " + str
            End If
        ElseIf type = labelType.WorkingProxies Then
            If lblWProxies.InvokeRequired Then lblWProxies.Invoke(Sub() lblWProxies.Text = "Proxies funcionales") Else lblWProxies.Text = "Proxies funcionales"
            If Not String.IsNullOrWhiteSpace(str) Then
                If lblWProxies.InvokeRequired Then lblWProxies.Invoke(Sub() lblWProxies.Text += " " + str) Else lblWProxies.Text += " " + str
            End If
        ElseIf type = labelType.Estado Then
            If pbEstado.InvokeRequired Then pbEstado.Invoke(Sub() pbEstado.Text = str) Else pbEstado.Text = str
        End If
    End Sub

    Private Sub btnClean_Click(sender As Object, e As EventArgs) Handles btnClean.Click
        If Not isCleaned Then
            Dim thClean As New Thread(AddressOf Clean)
            thClean.Start()
        End If
    End Sub

    Private Sub lblEstado_Changed(sender As Object, e As EventArgs)
        Me.Refresh()
    End Sub

    Private Sub txtProxies_TextChanged(sender As Object, e As EventArgs) Handles txtProxies.TextChanged
        If Not isStarted Then
            If isLoading Then
                If pbEstado.InvokeRequired Then pbEstado.Invoke(Sub() pbEstado.Value = ProxyList.Length) Else pbEstado.Value = ProxyList.Length
            End If
            SetTextInfo("Preparado para comenzar...", labelType.Estado)
            If Not String.IsNullOrWhiteSpace(txtProxies.Text) Then
                SetTextInfo(String.Format("Preparado para comenzar a leer {0} proxies...", ProxyList.Length), labelType.Estado)
            End If
        End If
        btnGo.Enabled = Not String.IsNullOrEmpty(txtProxies.Text)
        btnClean.Enabled = Not String.IsNullOrEmpty(txtProxies.Text)
        SetTextInfo("(" & ProxyList.Length & ")")
        Me.Refresh()
    End Sub

    Private Sub lblProxies_TextChanged(sender As Object, e As EventArgs) Handles lblProxies.TextChanged
        Me.Refresh()
    End Sub

    Private Sub lblWProxies_TextChanged(sender As Object, e As EventArgs) Handles lblWProxies.TextChanged
        Me.Refresh()
    End Sub

    Private Sub btnGo_Click(sender As Object, e As EventArgs) Handles btnGo.Click
        Dim thScan As New Thread(AddressOf Scan)
        thScan.Start()
    End Sub

    Private Sub btnExport_Click(sender As Object, e As EventArgs) Handles btnExport.Click
        SaveFileDialog1.ShowDialog()
    End Sub

    Private Sub Export_OK(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles SaveFileDialog1.FileOk
        filePath = SaveFileDialog1.FileName
        Dim thExp As New Thread(AddressOf Export)
        thExp.Start()
    End Sub

    Private Sub Update_Progress(Optional sProxyList As stateType = stateType.Scan)

        Dim total As Integer = ProxyList.Length
        Dim action As String = "Procesando"

        If pbEstado.Maximum <> total Then
            If pbEstado.InvokeRequired Then pbEstado.Invoke(Sub() pbEstado.Maximum = total + 1) Else pbEstado.Maximum = total + 1
        End If

        Threading.Interlocked.Increment(IndexPattern)
        If pbEstado.InvokeRequired Then pbEstado.Invoke(Sub() pbEstado.PerformStep()) Else pbEstado.PerformStep()

        If sProxyList = stateType.Export Then
            total = WorkingProxiesList.Count
            action = "Exportando"
        ElseIf sProxyList = stateType.Load Then
            total = proxiesToLoad
            action = "Cargando"
        End If

        Dim iSpan As TimeSpan = TimeSpan.FromSeconds((total - IndexPattern + 1) * MaxTime)

        Dim state As String = If(total >= IndexPattern, String.Format("{0} {1} de {2} proxies... Quedan {3} para acabar...", action, IndexPattern.ToString, total.ToString, If(iSpan.Days <> 0, iSpan.Days.ToString.PadLeft(2, "0"c) & " d ", "") &
                        If(iSpan.Hours <> 0, iSpan.Hours.ToString.PadLeft(2, "0"c) & " h ", "") &
                        If(iSpan.Minutes <> 0, iSpan.Minutes.ToString.PadLeft(2, "0"c) & " m ", "") &
                        If(iSpan.Seconds <> 0, iSpan.Seconds.ToString.PadLeft(2, "0"c) & " s", "0 s")), "¡Acabado!")

        SetTextInfo(state, labelType.Estado)

    End Sub

    Private Sub Clean()
        Dim lines As Integer
        If String.IsNullOrEmpty(txtProxies.Text) Then
            SetTextInfo("Debe introducir proxies en la caja de texto con el siguiente formato (IP:PUERTO) separado por saltos de líneas.", labelType.Estado)
            Exit Sub
        Else
            TimerStart = Now
            SetTextInfo("Espere un momento...")
            If btnClean.InvokeRequired Then btnClean.Invoke(Sub() btnClean.Enabled = False) Else btnClean.Enabled = True
            lines = ProxyList.Length
            SetTextInfo("Buscando proxies...", labelType.Estado)
            Dim prcleaned As String() = RemNotMatchingLines(ProxyList, proxyPattern)
            If txtProxies.InvokeRequired Then txtProxies.Invoke(Sub() txtProxies.Text = String.Join(Environment.NewLine, prcleaned)) Else txtProxies.Text = String.Join(Environment.NewLine, prcleaned)
            Dim cleanedLines As Integer = lines - ProxyList.Length
            lines = ProxyList.Length
            SetTextInfo("Removiendo duplicados...", labelType.Estado)
            Dim remdup As String() = RemoveDuplicates(ProxyList)
            If txtProxies.InvokeRequired Then txtProxies.Invoke(Sub() txtProxies.Text = String.Join(Environment.NewLine, remdup)) Else txtProxies.Text = String.Join(Environment.NewLine, remdup)
            Dim dupLines As Integer = lines - ProxyList.Length
            lines = ProxyList.Length
            SetTextInfo("Ordenando líneas...", labelType.Estado)
            Dim sortedlines As String() = SortData(txtProxies, ProxyList.Length)
            If txtProxies.InvokeRequired Then txtProxies.Invoke(Sub() txtProxies.Text = String.Join(Environment.NewLine, sortedlines)) Else txtProxies.Text = String.Join(Environment.NewLine, sortedlines)
            SetTextInfo()
            SetTextInfo(String.Format("¡Limpieza acabada! [Proxies erróneas: {0}; proxies duplicadas: {1}; líneas borradas: {2}]", cleanedLines, dupLines, (lines - ProxyList.Length).ToString), labelType.Estado)
            TimeSpent = Now.Subtract(TimerStart)
            secondsToClean = TimeSpent.TotalSeconds
            linesCleaned = lines - ProxyList.Length
            isCleaned = True
        End If
    End Sub

    Private Sub Scan()
        If String.IsNullOrEmpty(txtProxies.Text) Then
            SetTextInfo("Debe introducir proxies en la caja de texto con el siguiente formato (IP:PUERTO) separado por saltos de líneas.", labelType.Estado)
            Exit Sub
        Else
            TimerStart = Now
            isStarted = True
            If txtProxies.InvokeRequired Then txtProxies.Invoke(Sub() txtProxies.Enabled = False) Else txtProxies.Enabled = False
            If btnGo.InvokeRequired Then btnGo.Invoke(Sub() btnGo.Enabled = False) Else btnGo.Enabled = False
            If btnExport.InvokeRequired Then btnExport.Invoke(Sub() btnExport.Enabled = False) Else btnExport.Enabled = False
            If Not isCleaned Then
                Clean()
                If String.IsNullOrWhiteSpace(txtProxies.Text) Then
                    SetTextInfo("Al limpiar las proxies la caja de texto se quedó en blanco.", labelType.Estado)
                    If txtProxies.InvokeRequired Then txtProxies.Invoke(Sub() txtProxies.Enabled = True) Else txtProxies.Enabled = True
                    If btnGo.InvokeRequired Then btnGo.Invoke(Sub() btnGo.Enabled = True) Else btnGo.Enabled = True
                    Exit Sub
                End If
                SetTextInfo("Comenzando a procesar después de haber limpiado las proxies...", labelType.Estado)
            End If
            If pbEstado.InvokeRequired Then pbEstado.Invoke(Sub() pbEstado.Maximum = ProxyList.Length + 1) Else pbEstado.Maximum = ProxyList.Length + 1
            Dim proxies As List(Of String) = ProxyList.ToList()
            For Each proxy As String In proxies
                Update_Progress()
                If Regex.IsMatch(proxy, proxyPattern) Then
                    Dim proxyPath As String = proxy.Replace(proxy.Substring(proxy.IndexOf(":")), "")
                    Dim proxyPort As Integer = Convert.ToInt32(proxy.Substring(proxy.IndexOf(":") + 1, proxy.Length - (proxy.IndexOf(":") + 1)))
                    Try
                        Dim r As WebRequest = HttpWebRequest.Create("http://gimmeahit.x10host.com/Proxy/loadDocument.php")
                        r.Timeout = MaxTime * 1000
                        r.Proxy = New WebProxy(proxyPath, proxyPort)
                        Dim re As HttpWebResponse = CType(r.GetResponse(), HttpWebResponse)
                        If re.StatusCode = HttpStatusCode.OK Then
                            WorkingProxiesList.Add(proxy)
                            RaiseEvent lbWChanged(Me, New EventArgs)
                        End If
                        re.Close()
                    Catch ex As Exception
                        Console.WriteLine(proxy & " is not working! Exception: " & ex.Message)
                        notWorkingProxiesCount += 1
                    End Try
                End If
            Next
            Update_Progress()
            isStarted = False
            IndexPattern = 0
            If pbEstado.InvokeRequired Then
                pbEstado.Invoke(Sub() pbEstado.Maximum = 1)
                pbEstado.Invoke(Sub() pbEstado.Value = 1)
            Else
                pbEstado.Maximum = 1
                pbEstado.Value = 1
            End If
            numberOfProxies = ProxyList.Length
            TimeSpent = Now.Subtract(TimerStart)
            secondsToRead = TimeSpent.TotalSeconds

            If txtProxies.InvokeRequired Then txtProxies.Invoke(Sub() txtProxies.Enabled = True) Else txtProxies.Enabled = True
            If btnGo.InvokeRequired Then btnGo.Invoke(Sub() btnGo.Enabled = True) Else btnGo.Enabled = True

            If WorkingProxiesList.Count = 0 Then
                SetTextInfo("(0)", labelType.WorkingProxies)
                If btnExport.InvokeRequired Then btnExport.Invoke(Sub() btnExport.Enabled = False) Else btnExport.Enabled = False
            Else
                If btnExport.InvokeRequired Then btnExport.Invoke(Sub() btnExport.Enabled = True) Else btnExport.Enabled = True
            End If

        End If
    End Sub

    Private Sub Export()

        TimerStart = Now

        SetTextInfo("Espere un momento...", labelType.WorkingProxies)
        SetTextInfo("Comenzando con la exportación...", labelType.Estado)

        File.AppendAllText(filePath, "[Lista de proxies]" & Environment.NewLine & "==================" & Environment.NewLine)

        If pbEstado.InvokeRequired Then
            pbEstado.Invoke(Sub() pbEstado.Maximum = WorkingProxiesList.Count + 1)
            pbEstado.Invoke(Sub() pbEstado.Value = 0)
        Else
            pbEstado.Maximum = WorkingProxiesList.Count + 1
            pbEstado.Value = 0
        End If

        For i As Integer = 0 To WorkingProxiesList.Count - 1
            Update_Progress(stateType.Export)
            Dim proxy As String = WorkingProxiesList(i).ToString
            File.AppendAllText(filePath, Environment.NewLine & proxy)
        Next

        TimeSpent = Now.Subtract(TimerStart)
        secondsToExport = TimeSpent.TotalSeconds

        Dim iSpan As TimeSpan = TimeSpan.FromSeconds(secondsToRead)
        Dim iSpan1 As TimeSpan = TimeSpan.FromSeconds(secondsToClean)
        Dim iSpan2 As TimeSpan = TimeSpan.FromSeconds(secondsToExport)

        File.AppendAllText(filePath, String.Format(Environment.NewLine & "[INFO GENERAL]" &
                                                   Environment.NewLine & "Total de proxies escaneadas: {0}" &
                                                   Environment.NewLine & "Proxies disponibles: {1}" &
                                                   Environment.NewLine & "Numero de proxies erróneas: {2}" &
                                                   Environment.NewLine & "Porcentaje:  {3} %" &
                                                   Environment.NewLine & "Tiempo tomado en leer: {4}" &
                                                   Environment.NewLine & "Tiempo medio por proxy en leer: {5} proxy/s" &
                                                   Environment.NewLine & "Tiempo tomado en exportar: {6}" &
                                                   Environment.NewLine & "Tiempo medio por proxy en exportar: {7} proxy/s" &
                                                   Environment.NewLine & "Líneas limpiadas: {8}" &
                                                   Environment.NewLine & "Tiempo tomado en limpiar: {9}" &
                                                   Environment.NewLine & "Tiempo medio en limpiar una línea: {10}" &
                                                   Environment.NewLine & Environment.NewLine & "==================" & Environment.NewLine & Environment.NewLine & "Esta lista fue generada con Proxy Cheker." & Environment.NewLine & "Una aplicación para comprobar si las proxies están disponibles." &
                                                   Environment.NewLine & "Ikillnukes ©  {11}",
                                                   numberOfProxies,
                                                   WorkingProxiesList.Count,
                                                   notWorkingProxiesCount,
                                                   (WorkingProxiesList.Count * 100 / numberOfProxies).ToString,
                                                   If(iSpan.Days <> 0, iSpan.Days.ToString.PadLeft(2, "0"c) & " d ", "") &
                        If(iSpan.Hours <> 0, iSpan.Hours.ToString.PadLeft(2, "0"c) & " h ", "") &
                        If(iSpan.Minutes <> 0, iSpan.Minutes.ToString.PadLeft(2, "0"c) & " m ", "") &
                        If(iSpan.Seconds <> 0, iSpan.Seconds.ToString.PadLeft(2, "0"c) & " s", ""),
                                                   (numberOfProxies / secondsToRead).ToString("F3", New Globalization.CultureInfo("en-US")),
                                                   If(iSpan2.Days <> 0, iSpan2.Days.ToString.PadLeft(2, "0"c) & " d ", "") &
                        If(iSpan2.Hours <> 0, iSpan2.Hours.ToString.PadLeft(2, "0"c) & " h ", "") &
                        If(iSpan2.Minutes <> 0, iSpan2.Minutes.ToString.PadLeft(2, "0"c) & " m ", "") &
                        If(iSpan2.Seconds <> 0, iSpan2.Seconds.ToString.PadLeft(2, "0"c) & " s", ""),
                                                   (WorkingProxiesList.Count / secondsToExport).ToString("F3", New Globalization.CultureInfo("en-US")),
                                                   linesCleaned,
                                                   If(iSpan1.Days <> 0, iSpan1.Days.ToString.PadLeft(2, "0"c) & " d ", "") &
                        If(iSpan1.Hours <> 0, iSpan1.Hours.ToString.PadLeft(2, "0"c) & " h ", "") &
                        If(iSpan1.Minutes <> 0, iSpan1.Minutes.ToString.PadLeft(2, "0"c) & " m ", "") &
                        If(iSpan1.Seconds <> 0, iSpan1.Seconds.ToString.PadLeft(2, "0"c) & " s", ""),
                                                   (linesCleaned / secondsToClean).ToString("F3", New Globalization.CultureInfo("en-US")),
                                                   Date.Today.Year))

        IndexPattern = 0
        If pbEstado.InvokeRequired Then
            pbEstado.Invoke(Sub() pbEstado.Maximum = 1)
            pbEstado.Invoke(Sub() pbEstado.Value = 1)
        Else
            pbEstado.Maximum = 1
            pbEstado.Value = 1
        End If

        isExported = True

        If btnExport.InvokeRequired Then btnExport.Invoke(Sub() btnExport.Enabled = False) Else btnExport.Enabled = False

        SetTextInfo("", labelType.WorkingProxies)
        SetTextInfo("¡Exportación terminada con éxito, gracias por confiar en nosotros! :D", labelType.Estado)

        Process.Start(filePath)

    End Sub

    Private Sub WorkingProxyChanged() Handles Me.lbWChanged
        SetTextInfo("(" & WorkingProxiesList.Count & ")", labelType.WorkingProxies)
    End Sub

    Private Sub CargarDocumentoToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CargarDocumentoToolStripMenuItem.Click
        OpenFileDialog1.ShowDialog()
    End Sub

    Private Sub SalirToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SalirToolStripMenuItem.Click
        If WorkingProxiesList.Count > 0 And Not isExported Then
            If MsgBox("¿Desea guardar los cambios antes de salir?", MsgBoxStyle.YesNo, "?") = MsgBoxResult.Yes Then
                SaveFileDialog1.ShowDialog()
            End If
        Else
            If System.Diagnostics.Process.GetCurrentProcess().Threads.Count > 0 Then
                For Each t As System.Diagnostics.ProcessThread In System.Diagnostics.Process.GetCurrentProcess().Threads
                    t.Dispose()
                Next t
            End If
            Application.Exit()
        End If
    End Sub

    Private Sub Load_OK(sender As Object, e As CancelEventArgs) Handles OpenFileDialog1.FileOk
        loadFilePath = OpenFileDialog1.FileName
        Dim thLoad As New Thread(AddressOf LoadDocument)
        thLoad.Start()
    End Sub

End Class

Public Class TextProgressBar
    Inherits ProgressBar

    Public Property ProgressTextColor As Color = Color.Black
    Public Property ProgressTextFont As Font = New Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular)

    Protected Overrides ReadOnly Property createparams() As CreateParams
        Get
            Dim result As createparams = MyBase.createparams
            If environment.osversion.platform = platformid.win32nt AndAlso environment.osversion.version.major >= 6 Then
                ' ws_ex_composited 
                result.exstyle = result.exstyle Or &H2000000
            End If

            Return result
        End Get
    End Property

    Protected Overrides Sub WndProc(ByRef m As Message)
        MyBase.WndProc(m)
        If m.Msg = &HF Then
            Using graphics As Graphics = CreateGraphics()
                Using brush As New SolidBrush(ProgressTextColor)
                    Dim textSize As SizeF = graphics.MeasureString(Text, ProgressTextFont)
                    graphics.DrawString(Text, ProgressTextFont, brush, (Width - textSize.Width) / 2, (Height - textSize.Height) / 2)
                End Using
            End Using
        End If
    End Sub

    <EditorBrowsable(EditorBrowsableState.Always)> _
    <Browsable(True)> _
    Public Overrides Property Text() As String
        Get
            Return MyBase.Text
        End Get
        Set(value As String)
            MyBase.Text = value
            Refresh()
        End Set
    End Property

    <EditorBrowsable(EditorBrowsableState.Always)> _
    <Browsable(True)> _
    Public Overrides Property Font() As Font
        Get
            Return MyBase.Font
        End Get
        Set(value As Font)
            MyBase.Font = value
            Refresh()
        End Set
    End Property
End Class
