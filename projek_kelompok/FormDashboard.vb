Public Class FormDashboard
    Private Sub FormDashboard_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.WindowState = FormWindowState.Maximized
        ShowPage(New dashboard())
    End Sub
    Private Sub ShowPage(page As UserControl)
        Panel1.Controls.Clear()
        page.Dock = DockStyle.Fill
        Panel1.Controls.Add(page)
    End Sub

    Private Sub dashboard_Click(sender As Object, e As EventArgs) Handles dashboard.Click
        ShowPage(New dashboard())
    End Sub

    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles Label1.Click
        ShowPage(New data_mahasiswa())
    End Sub

    Private Sub Label2_Click(sender As Object, e As EventArgs) Handles Label2.Click
        ShowPage(New data_dosen())
    End Sub

    Private Sub Label3_Click(sender As Object, e As EventArgs) Handles Label3.Click
        ShowPage(New laporan())
    End Sub

    Private Sub Label4_Click(sender As Object, e As EventArgs) Handles Label4.Click
        ShowPage(New data_prodi())
    End Sub
End Class