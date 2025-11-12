Imports System.Data.Odbc
Public Class Formmhs
    Public Sub SetWelcomeMessage(ByVal nama As String)
        Label1.Text = "Selamat Datang, " & nama
    End Sub

    Private Sub Formmhs_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.WindowState = FormWindowState.Maximized
    End Sub

End Class