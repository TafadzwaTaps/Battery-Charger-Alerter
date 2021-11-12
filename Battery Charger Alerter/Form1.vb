Imports System.Speech.Recognition
Imports System.Speech.Synthesis
Imports System.Net.Mail

Public Class Form1

    Dim WithEvents Reco As New SpeechRecognitionEngine
    Dim psBattery As PowerStatus = SystemInformation.PowerStatus
    Dim perFull As Single = psBattery.BatteryLifePercent
    Dim EmailDetails As String
    Dim PassWordDetails As String

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        Dim synth As New SpeechSynthesizer

        Reco.SetInputToDefaultAudioDevice()

        Dim gram As New SrgsGrammar.SrgsDocument

        Dim txtspeech As New SrgsGrammar.SrgsRule("speech")

        Dim respondspch As New SrgsGrammar.SrgsOneOf("current battery level", "CURRENT BATTERY LEVEL", "battery used up", "BATTERY USED UP")

        Dim respond As New SrgsGrammar.SrgsText

        txtspeech.Add(respondspch)

        gram.Rules.Add(txtspeech)

        gram.Root = txtspeech

        Reco.LoadGrammar(New Grammar(gram))

        Reco.RecognizeAsync()

    End Sub

    Private Sub reco_RecognizeCompleted(ByVal sender As Object, ByVal e As RecognizeCompletedEventArgs) Handles Reco.RecognizeCompleted

        Reco.RecognizeAsync(RecognizeMode.Single)

    End Sub

    Private Sub Reco_SpeechRecognized(ByVal sender As Object, ByVal e As RecognitionEventArgs) Handles Reco.SpeechRecognized
        Dim random As New Random
        Dim number As Integer = random.Next(1, 10)
        Dim synth As New SpeechSynthesizer
        Try
            Select Case e.Result.Text.ToUpper
                Case Is = "current battery level", "CURRENT BATTERY LEVEL"
                    synth.Speak("current battery level is" & ProgressBar1.Value = perFull * 100)
                Case Is = "battery used up", "BATTERY USED UP"
                    synth.Speak("battery used up is" & 100 - ProgressBar1.Value & "%")
                Case Else
                    MsgBox("command not recognized", MsgBoxStyle.Exclamation)
            End Select
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub


    Public Function getBatteryStatus() As Integer
        Dim obj As Object, obj2 As Object, stat As Integer
        '  Get Battery Status
        '  Return Value Meaning
        '0 No battery
        '1 The battery is discharging.
        '2 The system has access to AC so no battery is being discharged. However, the battery is not necessarily charging.
        '3 Fully Charged
        '4 Low
        '5 Critical
        '6 Charging
        '7 Charging and High
        '8 Charging and Low
        '9 Charging and Critical
        '10 Undefined
        '11 Partially Charged
        stat = 0
        obj = GetObject("winmgmts:").InstancesOf("Win32_Battery")
        For Each obj2 In obj 'loop in objects
            stat = obj2.BatteryStatus
        Next
        getBatteryStatus = stat
    End Function
    Private Sub RefreshStatus()

        Dim power As PowerStatus = SystemInformation.PowerStatus

        Select Case power.PowerLineStatus
            Case PowerLineStatus.Online
                MainsPower.Checked = True
                Exit Select
            Case PowerLineStatus.Offline
                MainsPower.Checked = False
                Exit Select
            Case PowerLineStatus.Unknown
                MainsPower.CheckState = CheckState.Indeterminate
                Exit Select
        End Select

    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim psBattery As PowerStatus = SystemInformation.PowerStatus
        Dim perFull As Single = psBattery.BatteryLifePercent
        Timer1.Start()
        Timer2.Start()
        RefreshStatus()
        MsgBox("Battery status: " & getBatteryStatus(), vbInformation + vbOKOnly, "Battery Status")
        MsgBox("PLEASE TAKE NOTE THAT AS SOON AS BATTERY LIFE IS 100% REMOVE THE CHARGER IMMEDIATELY TO PREVENT OVERCHARING AND HELP PREVENT DAMAGE TO YOUR BATTERY EVEN IF IT SAYS 100% CHARGING ", MsgBoxStyle.Information)
        MsgBox("SHOULD YOU WISH TO EMAIL ALERT WHEN THE BATTERY IS FULL, PLEASE ENTER YOU CREDENTIALS UNDER THE EMAIL SECTION", MsgBoxStyle.Information)
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim power As PowerStatus = SystemInformation.PowerStatus
        Dim percent As Single = power.BatteryLifePercent
        Dim psBattery As PowerStatus = SystemInformation.PowerStatus
        Dim perFull As Single = psBattery.BatteryLifePercent

        Me.Text = "Battinfo - " & perFull * 100 & "%" & " CHARGED"
        If percent * 100 < 100 Then
            MsgBox("BATTERY PERCENT LIFE PRESENT IS: " & percent * 100 & "%", MsgBoxStyle.Information)
            RefreshStatus()
        Else
            MsgBox("BATTERY LIFE IS FULL CHARGED DISCONNECT CHARGER TO PREVENT OVERCHARGING OF BATTERY " & percent * 100 & "%" & " BATTERY NOT CHARGING", MsgBoxStyle.Information)
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim EmailAddress As String = TextBox1.Text
        Dim EmailPassword As String = TextBox2.Text
        My.Settings.Email = EmailAddress
        My.Settings.Password = EmailPassword
        My.Settings.Save()
        EmailDetails = My.Settings.Email
        PassWordDetails = My.Settings.Password
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Dim psBattery As PowerStatus = SystemInformation.PowerStatus
        Dim perFull As Single = psBattery.BatteryLifePercent

        If perFull * 100 > 100 Then
            ProgressBar1.Value = 100
        ElseIf perFull * 100 < 100 Then
            ProgressBar1.Value = perFull * 100
        End If
        Label5.Text = "Battery Used Up : " & 100 - ProgressBar1.Value & "%"
        Label3.Text = "Remaining time : " & Math.Round(psBattery.BatteryLifeRemaining / 60, 0) & " min"

        If psBattery.PowerLineStatus = PowerLineStatus.Offline Then
            Me.Text = "Battinfo - " & perFull * 100 & "%" & " discharging"
            Timer1.Interval = 30000
            Label4.Text = "Power Mode : " & "On Battery"
            PictureBox5.Hide()

        ElseIf psBattery.PowerLineStatus = PowerLineStatus.Online Then
            Me.Text = "Battinfo - " & perFull * 100 & "%" & " charging "
            ProgressBar1.Value = 100
            Label4.Text = "Power Mode : " & "On Charger"
            PictureBox5.Show()
        Else
            Me.Text = "Battinfo - " & perFull * 100 & "%" & " CHARGED"
        End If '

    End Sub

    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick
        Dim psBattery As PowerStatus = SystemInformation.PowerStatus
        Dim perFull As Single = psBattery.BatteryLifePercent
        Dim synth As New SpeechSynthesizer
        Dim count As Integer = 0

        Dim myTrack = My.Resources.Lowkey_Glidin__Ramzoid_x_LouisLowWaters_Remix__AudioTrimmer_com__2

        ProgressBar1.Value = perFull * 100

        If ProgressBar1.Value = 10 Then
            Do
                synth.Speak("warning.. battery critically low... now at 10%")
                synth.Speak("please plug in your charger")
            Loop Until count = 2
            Timer2.Stop()
        End If
        If ProgressBar1.Value = 12 Then
            Timer2.Start()
        End If

        If ProgressBar1.Value = 25 Then
            synth.Speak("battery has reached quarter level")
            synth.Speak("now currently at 50%")
            Timer2.Stop()
        End If
        If ProgressBar1.Value = 27 Then
            Timer2.Start()
        End If

        If ProgressBar1.Value = 50 Then
            synth.Speak("battery has reached half way level")
            synth.Speak("now currently at 50%")
            Timer2.Stop()
        End If
        If ProgressBar1.Value = 52 Then
            Timer2.Start()
        End If

        If ProgressBar1.Value = 75 Then
            synth.Speak("battery currently at three quarter level")
            synth.Speak("now currently at 75%")
            Timer2.Stop()
        End If
        If ProgressBar1.Value = 77 Then
            Timer2.Start()
        End If

        If ProgressBar1.Value = 100 Then
            synth.Speak("BATTERY LIFE IS FULL CHARGED DISCONNECT CHARGER TO PREVENT OVERCHARGING OF BATTERY")
            Timer2.Stop()
            My.Computer.Audio.Play(myTrack, AudioPlayMode.Background)
            Try
                Threading.Thread.Sleep(10000)
                SendEmail()
            Catch ex As Exception
                MsgBox(ex.Message)
            End Try
        End If

    End Sub

    Private Sub Timer3_Tick(sender As Object, e As EventArgs) Handles Timer3.Tick
        RefreshStatus()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Label2.Text = "Battery status is " & ComboBox1.SelectedItem.ToString()
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        My.Computer.Audio.Stop()
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Me.Hide()
        Change_Email.Show()
    End Sub

    Private Sub SendEmail()
        Try
            Dim SmtpServer As New SmtpClient()
            Dim mail As New MailMessage()
            SmtpServer.Port = 587
            SmtpServer.Host = "smtp.gmail.com"
            SmtpServer.EnableSsl = True
            SmtpServer.UseDefaultCredentials = True
            SmtpServer.Credentials = New Net.NetworkCredential($"{EmailDetails}", $"{PassWordDetails}")
            mail = New MailMessage()
            mail.From = New MailAddress("fedinando.r@gmail.com", Environment.MachineName)
            mail.To.Add(TextBox1.Text)
            mail.Subject = "Computer Battery Full"
            mail.IsBodyHtml = False
            mail.Body = "Hello this is a message from your personal computer, BATTERY LIFE IS FULLY CHARGED PLEASE DISCONNECT CHARGER TO PREVENT OVERCHARGING OF BATTERY Do not reply this message"
            SmtpServer.Send(mail)
            MsgBox("mail send")
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub
End Class
