Public Class Formmhs

    Public nim_login As String

    Private Sub Formmhs_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.WindowState = FormWindowState.Maximized
        ShowPage(New dashboard_mhs(nim_login))
    End Sub
    Private Sub ShowPage(page As UserControl)
        Panel1.Controls.Clear()
        page.Dock = DockStyle.Fill
        Panel1.Controls.Add(page)
    End Sub

    Private Sub dashboard_Click(sender As Object, e As EventArgs) Handles dashboard.Click
        ShowPage(New dashboard_mhs(nim_login))
    End Sub

    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles Label1.Click
        ShowPage(New pengajuan_ta(nim_login))
    End Sub

    Private Sub Label2_Click(sender As Object, e As EventArgs) Handles Label2.Click
        ShowPage(New bimbingan(nim_login))
    End Sub
    Private Sub Label4_Click(sender As Object, e As EventArgs) Handles Label4.Click
        ShowPage(New riwayat(nim_login))
    End Sub

    Private Sub Label5_Click(sender As Object, e As EventArgs) Handles Label5.Click
        Dim login As New FormLogin()
        login.StartPosition = FormStartPosition.CenterScreen
        login.Show()
        Me.Close()
    End Sub
End Class