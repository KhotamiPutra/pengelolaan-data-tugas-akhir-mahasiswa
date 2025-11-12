Imports System.Data.Odbc
Imports System.Drawing

Public Class riwayat
    Inherits UserControl

    Dim cmd As OdbcCommand
    Dim da As OdbcDataAdapter
    Dim ds As DataSet
    Dim nim_login As String

    ' Komponen UI
    Dim dgvRiwayat As DataGridView
    Dim lblHeader As Label

    Public Sub New(nim As String)
        nim_login = nim
        InitializeComponent()
        Connect()
        BuildUI()
        LoadRiwayat()
    End Sub

    Private Sub BuildUI()
        Me.Dock = DockStyle.Fill
        Me.BackColor = Color.FromArgb(245, 247, 250)

        ' === Header ===
        lblHeader = New Label With {
            .Text = "RIWAYAT TUGAS AKHIR",
            .Font = New Font("Segoe UI Semibold", 18, FontStyle.Bold),
            .ForeColor = Color.FromArgb(33, 33, 33),
            .Dock = DockStyle.Top,
            .Height = 60,
            .TextAlign = ContentAlignment.MiddleCenter
        }
        Me.Controls.Add(lblHeader)

        ' === DataGridView ===
        dgvRiwayat = New DataGridView With {
            .Location = New Point(40, 100),
            .Width = 950,
            .Height = 400,
            .AllowUserToAddRows = False,
            .ReadOnly = True,
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            .BackgroundColor = Color.White,
            .BorderStyle = BorderStyle.Fixed3D
        }
        Me.Controls.Add(dgvRiwayat)
    End Sub

    Private Sub LoadRiwayat()
        Try
            Dim query As String = "
                SELECT 
                    t.judul_ta AS 'Judul Tugas Akhir',
                    IFNULL(d.nama_dosen, '-') AS 'Dosen Pembimbing',
                    t.status AS 'Status',
                    DATE_FORMAT(t.tanggal_daftar, '%d-%m-%Y') AS 'Tanggal Diajukan',
                    IFNULL(pb.tanggal_tugas, '-') AS 'Tanggal Ditugaskan'
                FROM tugas_akhir t
                LEFT JOIN pembimbing pb ON pb.id_ta = t.id_ta
                LEFT JOIN dosen d ON d.nidn = pb.nidn
                WHERE t.nim = ?
                ORDER BY t.id_ta DESC"

            da = New OdbcDataAdapter(query, conn)
            da.SelectCommand.Parameters.AddWithValue("@nim", nim_login)
            ds = New DataSet
            da.Fill(ds, "riwayat")
            dgvRiwayat.DataSource = ds.Tables("riwayat")

            ' Ganti warna baris berdasarkan status
            For Each row As DataGridViewRow In dgvRiwayat.Rows
                If row.Cells("Status").Value IsNot Nothing Then
                    Select Case row.Cells("Status").Value.ToString()
                        Case "Diajukan"
                            row.DefaultCellStyle.BackColor = Color.FromArgb(255, 249, 196)
                        Case "Dibimbing"
                            row.DefaultCellStyle.BackColor = Color.FromArgb(179, 229, 252)
                        Case "Selesai"
                            row.DefaultCellStyle.BackColor = Color.FromArgb(200, 230, 201)
                        Case "Ditolak"
                            row.DefaultCellStyle.BackColor = Color.FromArgb(239, 154, 154)
                    End Select
                End If
            Next

        Catch ex As Exception
            MessageBox.Show("Kesalahan memuat data riwayat: " & ex.Message)
        End Try
    End Sub
End Class
