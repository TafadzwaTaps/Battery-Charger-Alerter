Public Class Change_Email
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If My.Settings.Email = TextBox1.Text And My.Settings.Password = TextBox2.Text Then
            If TextBox4.Text = TextBox5.Text Then
                My.Settings.Email = TextBox3.Text
                My.Settings.Password = TextBox4.Text
                My.Settings.Save()
                My.Settings.Email = TextBox3.Text = Form1.TextBox1.Text
                My.Settings.Password = TextBox4.Text = Form1.TextBox2.Text
            Else
                MsgBox("PASSWORD DOESNT MATCH", MsgBoxStyle.Exclamation)
            End If
        Else
            MsgBox("WRONG CURRENT USERNAME AND PASSWORD", MsgBoxStyle.Critical)
        End If
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If My.Computer.FileSystem.DirectoryExists("C:\Password\PasswordForm\") Then

            'If Folder Exist Do Nothing
        Else

            'If folder Does not Exist,Create it
            MkDir("C:\Password\PasswordForm\")

        End If

        If My.Computer.FileSystem.DirectoryExists("C:\Password\PasswordForm\" + TextBox3.Text) Then

            MsgBox("Error - Account Already Exists")

        Else

            MkDir("C:\Password\PasswordForm\" + TextBox3.Text)

            Dim a As New System.IO.StreamWriter("C:\Password\PasswordForm\" + TextBox3.Text + "\Username.txt")

            a.WriteLine(TextBox3.Text)

            a.Close()

            Dim b As New System.IO.StreamWriter("C:\Password\PasswordForm\" + TextBox3.Text + "\Password.txt")

            b.WriteLine(TextBox4.Text)

            b.Close()

            MessageBox.Show("New Details Successfully Saved")

            My.Settings.Save()
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.Close()
        Form1.Show()
    End Sub
End Class