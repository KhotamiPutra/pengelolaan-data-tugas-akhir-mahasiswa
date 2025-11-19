Imports System.Data.Odbc
Imports System.Drawing
Imports System.Windows.Forms

Public Class dashboard_mhs
    Inherits UserControl

    Dim cmd As OdbcCommand
    Dim dr As OdbcDataReader
    Dim nim As String

    ' Deklarasi komponen UI
    Private lblStatusTA As Label
    Private lblDosenPembimbing As Label
    Private lblJumlahBimbingan As Label
    Private lblNilaiAkhir As Label

    Public Sub New(nim_login As String)
        InitializeComponent()
        nim = nim_login
        Connect()
        InitializeUI()
        LoadData()
    End Sub

    Private Sub InitializeUI()
        Me.Dock = DockStyle.Fill
        Me.BackColor = Color.White
        Me.AutoScroll = True

        ' === CONTAINER UTAMA ===
        Dim mainPanel As New Panel With {
            .Dock = DockStyle.Fill,
            .BackColor = Color.White,
            .AutoScroll = True
        }

        '' === HEADER ===
        'Dim headerPanel As New Panel With {
        '    .Dock = DockStyle.Top,
        '    .Height = 80,
        '    .BackColor = Color.FromArgb(41, 128, 185)
        '}

        'Dim lblTitle As New Label With {
        '    .Text = "DASHBOARD MAHASISWA",
        '    .Font = New Font("Segoe UI", 16, FontStyle.Bold),
        '    .ForeColor = Color.White,
        '    .Dock = DockStyle.Left,
        '    .TextAlign = ContentAlignment.MiddleLeft,
        '    .Padding = New Padding(30, 0, 0, 0)
        '}

        'headerPanel.Controls.Add(lblTitle)
        'mainPanel.Controls.Add(headerPanel)

        ' === CONTENT PANEL ===
        Dim contentPanel As New Panel With {
            .Dock = DockStyle.Fill,
            .Padding = New Padding(30),
            .BackColor = Color.White
        }

        ' === PANEL STATISTIK ===
        Dim statsPanel As New Panel With {
            .Width = 400,
            .Height = 300,
            .BackColor = Color.FromArgb(248, 249, 250),
            .BorderStyle = BorderStyle.FixedSingle,
            .Padding = New Padding(20)
        }

        ' Label Status Tugas Akhir
        Dim lblStatusTitle As New Label With {
            .Text = "Status Tugas Akhir",
            .Font = New Font("Segoe UI", 12, FontStyle.Regular),
            .ForeColor = Color.FromArgb(108, 117, 125),
            .Location = New Point(0, 20),
            .Size = New Size(200, 25)
        }

        lblStatusTA = New Label With {
            .Text = "Loading...",
            .Font = New Font("Segoe UI", 14, FontStyle.Bold),
            .ForeColor = Color.FromArgb(41, 128, 185),
            .Location = New Point(0, 50),
            .Size = New Size(200, 30)
        }

        ' Label Dosen Pembimbing
        Dim lblDosenTitle As New Label With {
            .Text = "Dosen Pembimbing",
            .Font = New Font("Segoe UI", 12, FontStyle.Regular),
            .ForeColor = Color.FromArgb(108, 117, 125),
            .Location = New Point(0, 100),
            .Size = New Size(200, 25)
        }

        lblDosenPembimbing = New Label With {
            .Text = "Loading...",
            .Font = New Font("Segoe UI", 14, FontStyle.Bold),
            .ForeColor = Color.FromArgb(41, 128, 185),
            .Location = New Point(0, 130),
            .Size = New Size(350, 30)
        }

        ' Label Jumlah Bimbingan
        Dim lblBimbinganTitle As New Label With {
            .Text = "Jumlah Bimbingan",
            .Font = New Font("Segoe UI", 12, FontStyle.Regular),
            .ForeColor = Color.FromArgb(108, 117, 125),
            .Location = New Point(0, 180),
            .Size = New Size(200, 25)
        }

        lblJumlahBimbingan = New Label With {
            .Text = "Loading...",
            .Font = New Font("Segoe UI", 14, FontStyle.Bold),
            .ForeColor = Color.FromArgb(41, 128, 185),
            .Location = New Point(0, 210),
            .Size = New Size(200, 30)
        }

        ' Tambahkan kontrol ke statsPanel
        statsPanel.Controls.Add(lblStatusTitle)
        statsPanel.Controls.Add(lblStatusTA)
        statsPanel.Controls.Add(lblDosenTitle)
        statsPanel.Controls.Add(lblDosenPembimbing)
        statsPanel.Controls.Add(lblBimbinganTitle)
        statsPanel.Controls.Add(lblJumlahBimbingan)

        ' === PANEL NILAI ===
        Dim nilaiPanel As New Panel With {
            .Width = 400,
            .Height = 200,
            .BackColor = Color.FromArgb(248, 249, 250),
            .BorderStyle = BorderStyle.FixedSingle,
            .Padding = New Padding(20),
            .Location = New Point(0, 320)
        }

        Dim lblNilaiTitle As New Label With {
            .Text = "Nilai Akhir",
            .Font = New Font("Segoe UI", 12, FontStyle.Regular),
            .ForeColor = Color.FromArgb(108, 117, 125),
            .Location = New Point(0, 20),
            .Size = New Size(200, 25)
        }

        lblNilaiAkhir = New Label With {
            .Text = "0,000000",
            .Font = New Font("Segoe UI", 24, FontStyle.Bold),
            .ForeColor = Color.FromArgb(220, 53, 69),
            .Location = New Point(0, 50),
            .Size = New Size(300, 50)
        }

        nilaiPanel.Controls.Add(lblNilaiTitle)
        nilaiPanel.Controls.Add(lblNilaiAkhir)

        ' === PANEL INFO TAMBAHAN ===
        Dim infoPanel As New Panel With {
            .Width = 400,
            .Height = 300,
            .BackColor = Color.FromArgb(248, 249, 250),
            .BorderStyle = BorderStyle.FixedSingle,
            .Padding = New Padding(20),
            .Location = New Point(450, 0)
        }

        Dim lblInfoTitle As New Label With {
            .Text = "Informasi Tugas Akhir",
            .Font = New Font("Segoe UI", 14, FontStyle.Bold),
            .ForeColor = Color.FromArgb(41, 128, 185),
            .Location = New Point(0, 20),
            .Size = New Size(250, 30)
        }

        ' Info judul TA
        Dim lblJudulTitle As New Label With {
            .Text = "Judul TA:",
            .Font = New Font("Segoe UI", 11, FontStyle.Regular),
            .ForeColor = Color.FromArgb(108, 117, 125),
            .Location = New Point(0, 70),
            .Size = New Size(80, 25)
        }

        Dim lblJudulTA As New Label With {
            .Text = "Loading...",
            .Font = New Font("Segoe UI", 11, FontStyle.Bold),
            .ForeColor = Color.FromArgb(33, 37, 41),
            .Location = New Point(90, 70),
            .Size = New Size(280, 40)
        }

        ' Info tanggal daftar
        Dim lblTanggalTitle As New Label With {
            .Text = "Tanggal Daftar:",
            .Font = New Font("Segoe UI", 11, FontStyle.Regular),
            .ForeColor = Color.FromArgb(108, 117, 125),
            .Location = New Point(0, 120),
            .Size = New Size(120, 25)
        }

        Dim lblTanggalDaftar As New Label With {
            .Text = "Loading...",
            .Font = New Font("Segoe UI", 11, FontStyle.Bold),
            .ForeColor = Color.FromArgb(33, 37, 41),
            .Location = New Point(120, 120),
            .Size = New Size(200, 25)
        }

        ' Info progress
        Dim lblProgressTitle As New Label With {
            .Text = "Progress:",
            .Font = New Font("Segoe UI", 11, FontStyle.Regular),
            .ForeColor = Color.FromArgb(108, 117, 125),
            .Location = New Point(0, 170),
            .Size = New Size(80, 25)
        }

        Dim progressBar As New ProgressBar With {
            .Location = New Point(90, 170),
            .Size = New Size(250, 25),
            .Minimum = 0,
            .Maximum = 100,
            .Value = 0
        }

        Dim lblProgressPercent As New Label With {
            .Text = "0%",
            .Font = New Font("Segoe UI", 10, FontStyle.Bold),
            .ForeColor = Color.FromArgb(41, 128, 185),
            .Location = New Point(345, 170),
            .Size = New Size(40, 25)
        }

        infoPanel.Controls.Add(lblInfoTitle)
        infoPanel.Controls.Add(lblJudulTitle)
        infoPanel.Controls.Add(lblJudulTA)
        infoPanel.Controls.Add(lblTanggalTitle)
        infoPanel.Controls.Add(lblTanggalDaftar)
        infoPanel.Controls.Add(lblProgressTitle)
        infoPanel.Controls.Add(progressBar)
        infoPanel.Controls.Add(lblProgressPercent)

        ' Tambahkan semua panel ke contentPanel
        contentPanel.Controls.Add(statsPanel)
        contentPanel.Controls.Add(nilaiPanel)
        contentPanel.Controls.Add(infoPanel)

        mainPanel.Controls.Add(contentPanel)
        Me.Controls.Add(mainPanel)

        ' Simpan referensi untuk update data
        Me.lblJudulTA = lblJudulTA
        Me.lblTanggalDaftar = lblTanggalDaftar
        Me.progressBar = progressBar
        Me.lblProgressPercent = lblProgressPercent
    End Sub

    Private Sub LoadData()
        LoadStatusTA()
        LoadDosenPembimbing()
        LoadJumlahBimbingan()
        LoadNilaiAkhir()
        LoadInfoTA()
        LoadProgress()
    End Sub

    Private Sub LoadStatusTA()
        Try
            cmd = New OdbcCommand("SELECT status FROM tugas_akhir WHERE nim = ?", conn)
            cmd.Parameters.AddWithValue("@nim", nim)

            Dim result = cmd.ExecuteScalar()
            If result IsNot Nothing Then
                Dim status = result.ToString()
                lblStatusTA.Text = status

                ' Set warna berdasarkan status
                Select Case status
                    Case "Diajukan"
                        lblStatusTA.ForeColor = Color.FromArgb(255, 193, 7) ' Kuning
                    Case "Dibimbing"
                        lblStatusTA.ForeColor = Color.FromArgb(40, 167, 69) ' Hijau
                    Case "Selesai"
                        lblStatusTA.ForeColor = Color.FromArgb(0, 123, 255) ' Biru
                    Case "Ditolak"
                        lblStatusTA.ForeColor = Color.FromArgb(220, 53, 69) ' Merah
                    Case Else
                        lblStatusTA.ForeColor = Color.FromArgb(108, 117, 125) ' Abu-abu
                End Select
            Else
                lblStatusTA.Text = "Belum Mengajukan"
                lblStatusTA.ForeColor = Color.FromArgb(108, 117, 125)
            End If

        Catch ex As Exception
            lblStatusTA.Text = "Error"
            lblStatusTA.ForeColor = Color.Red
        End Try
    End Sub

    Private Sub LoadDosenPembimbing()
        Try
            cmd = New OdbcCommand("SELECT d.nama_dosen " &
                                  "FROM pembimbing p " &
                                  "JOIN dosen d ON p.nidn = d.nidn " &
                                  "JOIN tugas_akhir ta ON p.id_ta = ta.id_ta " &
                                  "WHERE ta.nim = ? AND p.peran = 'Pembimbing Utama'", conn)
            cmd.Parameters.AddWithValue("@nim", nim)

            Dim result = cmd.ExecuteScalar()
            If result IsNot Nothing Then
                lblDosenPembimbing.Text = result.ToString()
            Else
                lblDosenPembimbing.Text = "Belum Ditentukan"
            End If

        Catch ex As Exception
            lblDosenPembimbing.Text = "Error Load Data"
        End Try
    End Sub

    Private Sub LoadJumlahBimbingan()
        Try
            cmd = New OdbcCommand("SELECT COUNT(*) " &
                                  "FROM bimbingan b " &
                                  "JOIN tugas_akhir ta ON b.id_ta = ta.id_ta " &
                                  "WHERE ta.nim = ?", conn)
            cmd.Parameters.AddWithValue("@nim", nim)

            Dim result = cmd.ExecuteScalar()
            If result IsNot Nothing Then
                lblJumlahBimbingan.Text = result.ToString() & " kali"
            Else
                lblJumlahBimbingan.Text = "0 kali"
            End If

        Catch ex As Exception
            lblJumlahBimbingan.Text = "Error"
        End Try
    End Sub

    Private Sub LoadNilaiAkhir()
        Try
            cmd = New OdbcCommand("SELECT p.nilai_akhir " &
                                  "FROM penilaian p " &
                                  "JOIN tugas_akhir ta ON p.id_ta = ta.id_ta " &
                                  "WHERE ta.nim = ?", conn)
            cmd.Parameters.AddWithValue("@nim", nim)

            Dim result = cmd.ExecuteScalar()
            If result IsNot Nothing AndAlso Not IsDBNull(result) Then
                Dim nilai = Convert.ToDecimal(result)
                lblNilaiAkhir.Text = nilai.ToString("F6")

                ' Set warna berdasarkan nilai
                If nilai >= 80 Then
                    lblNilaiAkhir.ForeColor = Color.FromArgb(40, 167, 69) ' Hijau untuk nilai baik
                ElseIf nilai >= 60 Then
                    lblNilaiAkhir.ForeColor = Color.FromArgb(255, 193, 7) ' Kuning untuk nilai sedang
                Else
                    lblNilaiAkhir.ForeColor = Color.FromArgb(220, 53, 69) ' Merah untuk nilai rendah
                End If
            Else
                lblNilaiAkhir.Text = "0,000000"
                lblNilaiAkhir.ForeColor = Color.FromArgb(108, 117, 125)
            End If

        Catch ex As Exception
            lblNilaiAkhir.Text = "0,000000"
            lblNilaiAkhir.ForeColor = Color.FromArgb(108, 117, 125)
        End Try
    End Sub

    Private Sub LoadInfoTA()
        Try
            cmd = New OdbcCommand("SELECT judul_ta, tanggal_daftar FROM tugas_akhir WHERE nim = ?", conn)
            cmd.Parameters.AddWithValue("@nim", nim)

            dr = cmd.ExecuteReader()
            If dr.Read() Then
                lblJudulTA.Text = If(dr("judul_ta") Is DBNull.Value, "Belum ada judul", dr("judul_ta").ToString())
                lblTanggalDaftar.Text = If(dr("tanggal_daftar") Is DBNull.Value, "-", Convert.ToDateTime(dr("tanggal_daftar")).ToString("dd/MM/yyyy"))
            Else
                lblJudulTA.Text = "Belum mengajukan TA"
                lblTanggalDaftar.Text = "-"
            End If
            dr.Close()

        Catch ex As Exception
            lblJudulTA.Text = "Error load data"
            lblTanggalDaftar.Text = "-"
        End Try
    End Sub

    Private Sub LoadProgress()
        Try
            ' Hitung progress berdasarkan status bimbingan
            cmd = New OdbcCommand("SELECT COUNT(*) as total, " &
                                  "SUM(CASE WHEN status_bab = 'Disetujui' THEN 1 ELSE 0 END) as selesai " &
                                  "FROM bimbingan b " &
                                  "JOIN tugas_akhir ta ON b.id_ta = ta.id_ta " &
                                  "WHERE ta.nim = ?", conn)
            cmd.Parameters.AddWithValue("@nim", nim)

            dr = cmd.ExecuteReader()
            If dr.Read() Then
                Dim total = Convert.ToInt32(dr("total"))
                Dim selesai = Convert.ToInt32(dr("selesai"))

                If total > 0 Then
                    Dim progress = CInt((selesai / total) * 100)
                    progressBar.Value = progress
                    lblProgressPercent.Text = progress & "%"

                    ' Set warna progress bar
                    If progress >= 80 Then
                        progressBar.ForeColor = Color.FromArgb(40, 167, 69)
                    ElseIf progress >= 50 Then
                        progressBar.ForeColor = Color.FromArgb(255, 193, 7)
                    Else
                        progressBar.ForeColor = Color.FromArgb(0, 123, 255)
                    End If
                Else
                    progressBar.Value = 0
                    lblProgressPercent.Text = "0%"
                End If
            Else
                progressBar.Value = 0
                lblProgressPercent.Text = "0%"
            End If
            dr.Close()

        Catch ex As Exception
            progressBar.Value = 0
            lblProgressPercent.Text = "0%"
        End Try
    End Sub

    ' Deklarasi untuk komponen yang digunakan di LoadProgress
    Private progressBar As ProgressBar
    Private lblJudulTA As Label
    Private lblTanggalDaftar As Label
    Private lblProgressPercent As Label
End Class