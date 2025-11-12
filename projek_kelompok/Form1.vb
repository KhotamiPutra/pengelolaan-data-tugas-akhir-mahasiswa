Imports System.Data.Odbc

Public Class Form1
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.WindowState = FormWindowState.Maximized
    End Sub


    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            Connect()

            Dim cmd As New OdbcCommand("SELECT * FROM admin WHERE username=? AND password=?", conn)
            cmd.Parameters.AddWithValue("@username", TextBox1.Text)
            cmd.Parameters.AddWithValue("@password", TextBox2.Text)

            Dim dr As OdbcDataReader = cmd.ExecuteReader()

            If dr.HasRows Then
                dr.Read()
                MessageBox.Show("Login berhasil! Selamat datang, " & dr("nama_admin").ToString(), "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)

                Dim dashboard As New FormDashboard()
                dashboard.Show()
                Me.Hide()
            Else
                MessageBox.Show("Username atau password salah!", "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If

            dr.Close()
            conn.Close()
        Catch ex As Exception
            MessageBox.Show("Terjadi Kesalahan" & ex.Message)
        End Try
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim formlogin As New FormLogin()
        formlogin.Show()
        Me.Hide()

    End Sub
End Class
