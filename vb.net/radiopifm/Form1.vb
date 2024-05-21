Imports System.Net
Imports System.IO
Imports Renci.SshNet
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel

Public Class Form1
    Private sshClient As SshClient
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim psi As New ProcessStartInfo()
        psi.FileName = "cmd.exe"
        psi.RedirectStandardInput = True
        psi.RedirectStandardOutput = True
        psi.UseShellExecute = False
        psi.CreateNoWindow = True
        Dim process As Process = New Process()
        process.StartInfo = psi
        process.Start()
        process.StandardInput.WriteLine("ping raspberrypi.local")
        process.StandardInput.Close()
        Dim output As String = process.StandardOutput.ReadToEnd()
        AppendTextToRichTextBox(output)
        RichTextBox1.SelectionStart = RichTextBox1.TextLength
        RichTextBox1.ScrollToCaret()
        process.Close()
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Dim host As String = "raspberrypi.local"
        Dim username As String = "username"
        Dim password As String = "password"
        sshClient = New SshClient(host, username, password)
        Try
            sshClient.Connect()
            ExecuteCommand(sshClient, "sudo killall pi_fm_rds")
            ExecuteCommand(sshClient, "sudo killall python3")
            AppendTextToRichTextBox("Connected to Raspberry Pi at:" & host)
        Catch ex As Exception
            AppendTextToRichTextBox("error:" & ex.ToString)
        End Try
    End Sub
    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        Try
            Dim url As String = "http://raspberrypi.local:5000/temperature"
            Dim request As HttpWebRequest = WebRequest.Create(url)
            Dim response As HttpWebResponse = request.GetResponse()
            Dim reader As New StreamReader(response.GetResponseStream())
            Dim temperature As String = reader.ReadToEnd()
            response.Close()
            Label2.Text = temperature
        Catch ex As Exception
        End Try
    End Sub
    Private Function ExecuteCommand(client As SshClient, commandText As String)
        Dim command As SshCommand = client.CreateCommand(commandText)
        command.BeginExecute()
    End Function
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If sshClient IsNot Nothing AndAlso sshClient.IsConnected Then
            Try
                Dim commandResult As String = ExecuteCommand(sshClient, $"cd PiFmRds/src && sudo ./pi_fm_rds -freq {TextBox4.Text} -audio play.wav -pi {NumericUpDown1.Value} -rt ""{TextBox3.Text}"" -ps ""{TextBox2.Text}""")
                AppendTextToRichTextBox("at:" & TimeOfDay.Now & " started to play.")
                Label3.Text = "Playing:True"
                Label3.ForeColor = Color.Green
            Catch ex As Exception
            End Try
        Else
        End If
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        If sshClient IsNot Nothing AndAlso sshClient.IsConnected Then
            sshClient.Disconnect()
            RichTextBox1.AppendText("Disconnected From Raspberrypi" & vbNewLine)
        End If
    End Sub

    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick
        If sshClient IsNot Nothing AndAlso sshClient.IsConnected Then
            Label1.Text = "Status:Connected"
            Label1.ForeColor = Color.Green
            Label1.Refresh()
        Else
            Label1.Text = "Status:NotConnected"
            Label1.ForeColor = Color.Red
        End If
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        If sshClient IsNot Nothing AndAlso sshClient.IsConnected Then
            Try
                ExecuteCommand(sshClient, "sudo killall pi_fm_rds")
                Label3.Text = "Playing:False"
                AppendTextToRichTextBox("at:" & TimeOfDay.Now & " Stoped Playing")
                Label3.ForeColor = Color.Red
            Catch ex As Exception
            End Try
        End If
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        If sshClient IsNot Nothing AndAlso sshClient.IsConnected Then
            Try
                ExecuteCommand(sshClient, "cd PiFmRds/src && python3 tem.py")
                Timer1.Start()
                AppendTextToRichTextBox("at:" & TimeOfDay.Now & " Temp is on")
            Catch ex As Exception
            End Try
        End If
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        Timer1.Stop()
        If sshClient IsNot Nothing AndAlso sshClient.IsConnected Then
            Try
                ExecuteCommand(sshClient, "sudo killall python3")
                AppendTextToRichTextBox("at:" & TimeOfDay.Now & " Temp is off")
            Catch ex As Exception
            End Try
        End If
    End Sub
    Private Sub TrackBar1_Scroll(sender As Object, e As EventArgs) Handles TrackBar1.Scroll
        TextBox4.Text = TrackBar1.Value
    End Sub
    Private Sub TextBox1_Click(sender As Object, e As EventArgs) Handles TextBox1.Click
        Dim openFileDialog1 As New OpenFileDialog()
        openFileDialog1.Title = "Select WAV File"
        openFileDialog1.Filter = "WAV files (*.wav)|*.wav" 
        openFileDialog1.RestoreDirectory = True
        If openFileDialog1.ShowDialog() = DialogResult.OK Then
            TextBox1.Text = openFileDialog1.FileName
        End If
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        If File.Exists(TextBox1.Text) Then
            If sshClient IsNot Nothing AndAlso sshClient.IsConnected Then
                ExecuteCommand(sshClient, "sudo killall pi_fm_rds")
                ExecuteCommand(sshClient, "cd PiFmRds/src && rm play.wav")
                Dim host As String = "raspberrypi.local"
                Dim port As Integer = 22
                Dim username As String = "username"
                Dim password As String = "password"
                Dim localFilePath As String = TextBox1.Text
                Dim remoteFilePath As String = "/home/username/PiFmRds/src/play.wav"
                UploadFileUsingSftp(host, port, username, password, localFilePath, remoteFilePath)
            Else
            End If
        End If
    End Sub
    Public Sub UploadFileUsingSftp(host As String, port As Integer, username As String, password As String, localFilePath As String, remoteFilePath As String)
        Try
            AppendTextToRichTextBox("Connecting to " & host & " as " & username)
            AppendTextToRichTextBox("Local File Path: " & localFilePath)
            AppendTextToRichTextBox("Remote File Path: " & remoteFilePath)
            Using client As New SftpClient(host, port, username, password)
                client.Connect()
                If client.IsConnected Then
                    AppendTextToRichTextBox("Connected to " & host)
                Else
                    AppendTextToRichTextBox("Failed to connect to " & host)
                    Exit Sub
                End If
                If Not File.Exists(localFilePath) Then
                    AppendTextToRichTextBox("Local file does not exist: " & localFilePath)
                    Exit Sub
                End If
                Using fileStream As New FileStream(localFilePath, FileMode.Open)
                    client.UploadFile(fileStream, remoteFilePath)
                End Using
                client.Disconnect()
            End Using
            AppendTextToRichTextBox("File transferred successfully!")
            ExecuteCommand(sshClient, $"cd PiFmRds/src && sudo ./pi_fm_rds -freq {TextBox4.Text} -audio play.wav -pi {NumericUpDown1.Value} -rt ""{TextBox3.Text}"" -ps ""{TextBox2.Text}""")
            Label3.Text = "Playing:True"
            Label3.ForeColor = Color.Green
        Catch ex As Exception
            AppendTextToRichTextBox("Error: " & ex.Message)
        End Try
    End Sub
    Private Sub AppendTextToRichTextBox(text As String)
        If RichTextBox1.InvokeRequired Then
            RichTextBox1.Invoke(New Action(Of String)(AddressOf AppendTextToRichTextBox), text)
        Else
            RichTextBox1.AppendText(text & Environment.NewLine)
            RichTextBox1.ScrollToCaret()
        End If
    End Sub

    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        If sshClient IsNot Nothing AndAlso sshClient.IsConnected Then
            ExecuteCommand(sshClient, "sudo shutdown")
            AppendTextToRichTextBox("Shutdowned at " & TimeOfDay.Now)
        End If
    End Sub

    Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
        If sshClient IsNot Nothing AndAlso sshClient.IsConnected Then
            ExecuteCommand(sshClient, "sudo reboot")
            AppendTextToRichTextBox("Rebooted at " & TimeOfDay.Now)
        End If
    End Sub
End Class
