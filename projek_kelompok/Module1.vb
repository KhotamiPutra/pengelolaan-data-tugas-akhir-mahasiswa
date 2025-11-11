Imports System.Data.Odbc
Imports System.Data
Module Module1
    Public conn As OdbcConnection
    Public da As OdbcDataAdapter
    Public ds As DataSet
    Public strcon As String

    Public Sub Connect()
        strcon = "Driver={MySQL ODBC 8.1 ANSI Driver};database=projek_kel_2;server=localhost;uid=root"
        conn = New OdbcConnection(strcon)
        If conn.State = ConnectionState.Closed Then
            conn.Open()
        End If
    End Sub
End Module
