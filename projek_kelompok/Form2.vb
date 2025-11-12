Imports System.Data.Odbc

Public Class Form2
    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.WindowState = FormWindowState.Maximized
    End Sub


    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            Connect()

            Dim cmd As New OdbcCommand("SELECT * FROM mahasiswa WHERE nim=? AND nama_mahasiswa=?", conn)
            cmd.Parameters.AddWithValue("@nim", TextBox1.Text)
            cmd.Parameters.AddWithValue("@nama_mahasiswa", TextBox2.Text)

            Dim dr As OdbcDataReader = cmd.ExecuteReader()

            If dr.HasRows Then
                dr.Read()
                MessageBox.Show("Login berhasil! Selamat datang, " & dr("nama_mahasiswa").ToString(), "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)

                Dim namamhs As String = dr("nama_mahasiswa").ToString()
                Dim dashboardmhs As New Formmhs()
                dashboardmhs.SetWelcomeMessage(namamhs)
                dashboardmhs.Show()
                Me.Hide()
            Else
                MessageBox.Show("NIM atau Username salah!", "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Error)
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