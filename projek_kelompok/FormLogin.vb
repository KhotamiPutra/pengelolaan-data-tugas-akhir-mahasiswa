Imports System.Data.Odbc

Public Class FormLogin
    Private Sub FormLogin_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.WindowState = FormWindowState.Maximized
    End Sub


    Private Sub Buttonadmin_Click(sender As Object, e As EventArgs) Handles Buttonadmin.Click
        Dim loginadmin As New Form1()
        loginadmin.Show()
        Me.Hide()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim logindosen As New Form3()
        logindosen.Show()
        Me.Hide()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim loginmhs As New Form2()
        loginmhs.Show()
        Me.Hide()
    End Sub
End Class