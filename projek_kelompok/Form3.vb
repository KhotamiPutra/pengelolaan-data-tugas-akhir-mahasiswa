Imports System.Data.Odbc

Public Class Form3
    Private Sub Form3_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.WindowState = FormWindowState.Maximized
    End Sub


    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            Connect()

            Dim cmd As New OdbcCommand("SELECT * FROM dosen WHERE nidn=? AND nama_dosen=?", conn)
            cmd.Parameters.AddWithValue("@nidn", TextBox1.Text)
            cmd.Parameters.AddWithValue("@nama_dosen", TextBox2.Text)

            Dim dr As OdbcDataReader = cmd.ExecuteReader()

            If dr.HasRows Then
                dr.Read()
                MessageBox.Show("Login berhasil! Selamat datang, " & dr("nama_dosen").ToString(), "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)

                Dim namaDosen As String = dr("nama_dosen").ToString()
                Dim dashboarddsn As New Formdosen()
                dashboarddsn.SetWelcomeMessage(namaDosen)
                dashboarddsn.Show()
                Me.Hide()
            Else
                MessageBox.Show("NIP atau Username salah!", "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Error)
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