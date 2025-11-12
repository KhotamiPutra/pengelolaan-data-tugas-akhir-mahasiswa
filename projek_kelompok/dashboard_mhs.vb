Imports System.Data.Odbc
Imports System.Drawing

Public Class dashboard_mhs
    Inherits UserControl

    Dim cmd As OdbcCommand
    Dim dr As OdbcDataReader
    Dim nim_login As String = ""

    ' Panel dan label
    Dim pnlStatus, pnlPembimbing, pnlBimbingan, pnlNilai As New Panel()
    Dim lblStatus, lblPembimbing, lblBimbingan, lblNilai As New Label()
    Dim lblTitleStatus, lblTitlePembimbing, lblTitleBimbingan, lblTitleNilai As New Label()

    Public Sub New(nim As String)
        nim_login = nim
        InitializeComponent()
        Connect()
        BuildUI()
        LoadDashboard()
    End Sub

    Private Sub BuildUI()
        ' ==== Properti UserControl ====
        Me.Dock = DockStyle.Fill
        Me.BackColor = Color.FromArgb(245, 247, 250)

        ' ==== Judul Utama ====
        Dim lblHeader As New Label With {
        .Text = "DASHBOARD MAHASISWA",
        .Font = New Font("Segoe UI Semibold", 20, FontStyle.Bold),
        .ForeColor = Color.FromArgb(55, 71, 79),
        .Dock = DockStyle.Top,
        .Height = 60,
        .TextAlign = ContentAlignment.MiddleCenter
    }
        Me.Controls.Add(lblHeader)

        ' ==== Container ====
        Dim container As New FlowLayoutPanel With {
        .Dock = DockStyle.Fill,
        .Padding = New Padding(40, 10, 40, 40),
        .BackColor = Color.Transparent,
        .AutoScroll = True,
        .WrapContents = True,
        .FlowDirection = FlowDirection.LeftToRight
    }
        Me.Controls.Add(container)
        container.BringToFront()

        ' ==== Pastikan 4 elemen dibuat ====
        Dim panels(3) As Panel
        Dim titles(3) As Label
        Dim values(3) As Label
        Dim titleTexts() As String = {"Status Tugas Akhir", "Dosen Pembimbing", "Jumlah Bimbingan", "Nilai Akhir"}

        For i As Integer = 0 To 3
            panels(i) = New Panel()
            titles(i) = New Label()
            values(i) = New Label()

            ' Panel
            With panels(i)
                .Width = 250
                .Height = 150
                .BackColor = Color.White
                .Margin = New Padding(20)
                .Padding = New Padding(15)
                .BorderStyle = BorderStyle.FixedSingle
            End With

            ' Label judul
            With titles(i)
                .Text = titleTexts(i)
                .Font = New Font("Segoe UI Semibold", 11, FontStyle.Bold)
                .ForeColor = Color.FromArgb(90, 90, 90)
                .Dock = DockStyle.Top
                .Height = 35
                .TextAlign = ContentAlignment.MiddleLeft
            End With

            ' Label isi
            With values(i)
                .Text = "-"
                .Font = New Font("Segoe UI", 16, FontStyle.Bold)
                .Dock = DockStyle.Fill
                .TextAlign = ContentAlignment.MiddleCenter
                .ForeColor = Color.FromArgb(33, 33, 33)
            End With

            panels(i).Controls.Add(values(i))
            panels(i).Controls.Add(titles(i))
            container.Controls.Add(panels(i))
        Next

        ' Simpan agar bisa diubah di LoadDashboard()
        pnlStatus = panels(0)
        pnlPembimbing = panels(1)
        pnlBimbingan = panels(2)
        pnlNilai = panels(3)
        lblStatus = values(0)
        lblPembimbing = values(1)
        lblBimbingan = values(2)
        lblNilai = values(3)
    End Sub


    Private Sub LoadDashboard()
        Try
            Dim query As String = "
                SELECT t.status, 
                       IFNULL(d.nama_dosen, '-') AS nama_dosen, 
                       COUNT(b.id_bimbingan) AS total_bimbingan,
                       IFNULL(AVG(p.nilai_akhir), 0) AS rata_nilai
                FROM tugas_akhir t
                LEFT JOIN pembimbing pb ON pb.id_ta = t.id_ta
                LEFT JOIN dosen d ON d.nidn = pb.nidn
                LEFT JOIN bimbingan b ON b.id_ta = t.id_ta
                LEFT JOIN penilaian p ON p.id_ta = t.id_ta
                WHERE t.nim = ?
                GROUP BY t.id_ta, t.status, d.nama_dosen;
            "
            cmd = New OdbcCommand(query, conn)
            cmd.Parameters.AddWithValue("@nim", nim_login)
            dr = cmd.ExecuteReader()

            If dr.Read() Then
                lblStatus.Text = dr("status").ToString()
                lblPembimbing.Text = dr("nama_dosen").ToString()
                lblBimbingan.Text = dr("total_bimbingan").ToString() & " kali"
                lblNilai.Text = dr("rata_nilai").ToString()
            Else
                lblStatus.Text = "Belum Mengajukan"
                lblPembimbing.Text = "-"
                lblBimbingan.Text = "0 kali"
                lblNilai.Text = "-"
            End If
            dr.Close()

            ' Warna panel status berdasarkan status TA
            Select Case lblStatus.Text
                Case "Diajukan"
                    pnlStatus.BackColor = Color.FromArgb(255, 249, 196)
                Case "Dibimbing"
                    pnlStatus.BackColor = Color.FromArgb(227, 242, 253)
                Case "Selesai"
                    pnlStatus.BackColor = Color.FromArgb(200, 230, 201)
                Case "Ditolak"
                    pnlStatus.BackColor = Color.FromArgb(255, 205, 210)
                Case Else
                    pnlStatus.BackColor = Color.FromArgb(238, 238, 238)
            End Select

        Catch ex As Exception
            MessageBox.Show("Kesalahan memuat dashboard: " & ex.Message)
        End Try
    End Sub
End Class
