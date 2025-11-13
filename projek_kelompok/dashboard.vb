Imports System.Data.Odbc
Imports System.Drawing
Imports System.Windows.Forms

Public Class dashboard
    Inherits UserControl

    Dim cmd As OdbcCommand
    Dim da As OdbcDataAdapter
    Dim ds As DataSet

    ' Panel untuk statistik
    Dim panelStats As Panel
    Dim lblTotalMahasiswa, lblTotalDosen, lblTotalTA, lblTAPending As Label

    ' Tombol navigasi
    Dim btnMahasiswa, btnDosen, btnTA, btnBimbingan, btnPenilaian, btnLaporan As Button

    ' Chart placeholder (bisa diganti dengan chart control)
    Dim panelChart As Panel

    Public Sub New()
        InitializeComponent()
        Connect()
        BuildUI()
        LoadStatistics()
    End Sub

    Private Sub BuildUI()
        Me.Dock = DockStyle.Fill
        Me.BackColor = Color.FromArgb(240, 242, 245)

        ' === HEADER ===
        Dim lblHeader As New Label With {
            .Text = "DASHBOARD ADMIN - SISTEM TUGAS AKHIR",
            .Font = New Font("Segoe UI Semibold", 20, FontStyle.Bold),
            .ForeColor = Color.FromArgb(44, 62, 80),
            .Dock = DockStyle.Top,
            .Height = 80,
            .TextAlign = ContentAlignment.MiddleCenter
        }
        Me.Controls.Add(lblHeader)

        ' === PANEL STATISTIK ===
        panelStats = New Panel With {
            .Location = New Point(30, 100),
            .Size = New Size(900, 120),
            .BackColor = Color.White,
            .BorderStyle = BorderStyle.None
        }
        BuildStatisticsCards()
        Me.Controls.Add(panelStats)

        ' === TOMBOL NAVIGASI CEPAT ===
        Dim lblQuickNav As New Label With {
            .Text = "NAVIGASI CEPAT",
            .Font = New Font("Segoe UI Semibold", 14, FontStyle.Bold),
            .ForeColor = Color.FromArgb(52, 73, 94),
            .Location = New Point(30, 250),
            .Size = New Size(200, 30)
        }
        Me.Controls.Add(lblQuickNav)

        BuildQuickNavigation()

        ' === PANEL ACTIVITY/CHART ===
        panelChart = New Panel With {
            .Location = New Point(30, 400),
            .Size = New Size(900, 250),
            .BackColor = Color.White,
            .BorderStyle = BorderStyle.FixedSingle
        }
        BuildActivityPanel()
        Me.Controls.Add(panelChart)
    End Sub

    Private Sub BuildStatisticsCards()
        ' Card 1: Total Mahasiswa
        Dim card1 As New Panel With {
            .Location = New Point(20, 15),
            .Size = New Size(200, 90),
            .BackColor = Color.FromArgb(52, 152, 219),
            .BorderStyle = BorderStyle.None
        }

        Dim icon1 As New Label With {
            .Text = "👨‍🎓",
            .Font = New Font("Segoe UI", 24),
            .Location = New Point(15, 20),
            .Size = New Size(50, 50),
            .TextAlign = ContentAlignment.MiddleCenter,
            .ForeColor = Color.White
        }

        lblTotalMahasiswa = New Label With {
            .Text = "0",
            .Font = New Font("Segoe UI Semibold", 24, FontStyle.Bold),
            .Location = New Point(70, 15),
            .Size = New Size(100, 40),
            .ForeColor = Color.White
        }

        Dim label1 As New Label With {
            .Text = "Total Mahasiswa",
            .Font = New Font("Segoe UI", 10),
            .Location = New Point(70, 55),
            .Size = New Size(120, 20),
            .ForeColor = Color.White
        }

        card1.Controls.AddRange({icon1, lblTotalMahasiswa, label1})

        ' Card 2: Total Dosen
        Dim card2 As New Panel With {
            .Location = New Point(240, 15),
            .Size = New Size(200, 90),
            .BackColor = Color.FromArgb(46, 204, 113),
            .BorderStyle = BorderStyle.None
        }

        Dim icon2 As New Label With {
            .Text = "👨‍🏫",
            .Font = New Font("Segoe UI", 24),
            .Location = New Point(15, 20),
            .Size = New Size(50, 50),
            .TextAlign = ContentAlignment.MiddleCenter,
            .ForeColor = Color.White
        }

        lblTotalDosen = New Label With {
            .Text = "0",
            .Font = New Font("Segoe UI Semibold", 24, FontStyle.Bold),
            .Location = New Point(70, 15),
            .Size = New Size(100, 40),
            .ForeColor = Color.White
        }

        Dim label2 As New Label With {
            .Text = "Total Dosen",
            .Font = New Font("Segoe UI", 10),
            .Location = New Point(70, 55),
            .Size = New Size(120, 20),
            .ForeColor = Color.White
        }

        card2.Controls.AddRange({icon2, lblTotalDosen, label2})

        ' Card 3: Total Tugas Akhir
        Dim card3 As New Panel With {
            .Location = New Point(460, 15),
            .Size = New Size(200, 90),
            .BackColor = Color.FromArgb(155, 89, 182),
            .BorderStyle = BorderStyle.None
        }

        Dim icon3 As New Label With {
            .Text = "📚",
            .Font = New Font("Segoe UI", 24),
            .Location = New Point(15, 20),
            .Size = New Size(50, 50),
            .TextAlign = ContentAlignment.MiddleCenter,
            .ForeColor = Color.White
        }

        lblTotalTA = New Label With {
            .Text = "0",
            .Font = New Font("Segoe UI Semibold", 24, FontStyle.Bold),
            .Location = New Point(70, 15),
            .Size = New Size(100, 40),
            .ForeColor = Color.White
        }

        Dim label3 As New Label With {
            .Text = "Total Tugas Akhir",
            .Font = New Font("Segoe UI", 10),
            .Location = New Point(70, 55),
            .Size = New Size(120, 20),
            .ForeColor = Color.White
        }

        card3.Controls.AddRange({icon3, lblTotalTA, label3})

        ' Card 4: TA Pending
        Dim card4 As New Panel With {
            .Location = New Point(680, 15),
            .Size = New Size(200, 90),
            .BackColor = Color.FromArgb(231, 76, 60),
            .BorderStyle = BorderStyle.None
        }

        Dim icon4 As New Label With {
            .Text = "⏳",
            .Font = New Font("Segoe UI", 24),
            .Location = New Point(15, 20),
            .Size = New Size(50, 50),
            .TextAlign = ContentAlignment.MiddleCenter,
            .ForeColor = Color.White
        }

        lblTAPending = New Label With {
            .Text = "0",
            .Font = New Font("Segoe UI Semibold", 24, FontStyle.Bold),
            .Location = New Point(70, 15),
            .Size = New Size(100, 40),
            .ForeColor = Color.White
        }

        Dim label4 As New Label With {
            .Text = "TA Menunggu",
            .Font = New Font("Segoe UI", 10),
            .Location = New Point(70, 55),
            .Size = New Size(120, 20),
            .ForeColor = Color.White
        }

        card4.Controls.AddRange({icon4, lblTAPending, label4})

        panelStats.Controls.AddRange({card1, card2, card3, card4})
    End Sub

    Private Sub BuildQuickNavigation()
        ' Tombol Mahasiswa
        btnMahasiswa = CreateNavButton("👨‍🎓 Data Mahasiswa", Color.FromArgb(52, 152, 219), New Point(30, 290))
        AddHandler btnMahasiswa.Click, AddressOf btnMahasiswa_Click

        ' Tombol Dosen
        btnDosen = CreateNavButton("👨‍🏫 Data Dosen", Color.FromArgb(46, 204, 113), New Point(180, 290))
        AddHandler btnDosen.Click, AddressOf btnDosen_Click

        ' Tombol Tugas Akhir
        btnTA = CreateNavButton("📚 Tugas Akhir", Color.FromArgb(155, 89, 182), New Point(330, 290))
        AddHandler btnTA.Click, AddressOf btnTA_Click

        ' Tombol Bimbingan
        btnBimbingan = CreateNavButton("💬 Bimbingan", Color.FromArgb(241, 196, 15), New Point(480, 290))
        AddHandler btnBimbingan.Click, AddressOf btnBimbingan_Click

        ' Tombol Penilaian
        btnPenilaian = CreateNavButton("⭐ Penilaian", Color.FromArgb(230, 126, 34), New Point(630, 290))
        AddHandler btnPenilaian.Click, AddressOf btnPenilaian_Click

        ' Tombol Laporan
        btnLaporan = CreateNavButton("📊 Laporan", Color.FromArgb(149, 165, 166), New Point(780, 290))
        AddHandler btnLaporan.Click, AddressOf btnLaporan_Click

        Me.Controls.AddRange({btnMahasiswa, btnDosen, btnTA, btnBimbingan, btnPenilaian, btnLaporan})
    End Sub

    Private Function CreateNavButton(text As String, backColor As Color, location As Point) As Button
        Return New Button With {
            .Text = text,
            .Font = New Font("Segoe UI Semibold", 11, FontStyle.Bold),
            .Location = location,
            .Size = New Size(140, 60),
            .BackColor = backColor,
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat,
            .TextAlign = ContentAlignment.MiddleCenter
        }
    End Function

    Private Sub BuildActivityPanel()
        ' Header Activity
        Dim lblActivityHeader As New Label With {
            .Text = "AKTIVITAS TERBARU & STATISTIK",
            .Font = New Font("Segoe UI Semibold", 14, FontStyle.Bold),
            .ForeColor = Color.FromArgb(52, 73, 94),
            .Location = New Point(20, 15),
            .Size = New Size(300, 25)
        }
        panelChart.Controls.Add(lblActivityHeader)

        ' Recent Activities List
        Dim lstActivities As New ListBox With {
            .Location = New Point(20, 50),
            .Size = New Size(400, 180),
            .Font = New Font("Segoe UI", 10),
            .BorderStyle = BorderStyle.FixedSingle
        }

        ' Load recent activities
        LoadRecentActivities(lstActivities)
        panelChart.Controls.Add(lstActivities)

        ' Simple Statistics
        Dim lblStats As New Label With {
            .Text = "STATISTIK STATUS TA:",
            .Font = New Font("Segoe UI Semibold", 12, FontStyle.Bold),
            .Location = New Point(450, 50),
            .Size = New Size(200, 25),
            .ForeColor = Color.FromArgb(52, 73, 94)
        }
        panelChart.Controls.Add(lblStats)

        ' Status Statistics
        LoadTAStatusStatistics(panelChart)
    End Sub

    Private Sub LoadStatistics()
        Try
            ' Total Mahasiswa
            cmd = New OdbcCommand("SELECT COUNT(*) FROM mahasiswa", conn)
            lblTotalMahasiswa.Text = cmd.ExecuteScalar().ToString()

            ' Total Dosen
            cmd = New OdbcCommand("SELECT COUNT(*) FROM dosen", conn)
            lblTotalDosen.Text = cmd.ExecuteScalar().ToString()

            ' Total Tugas Akhir
            cmd = New OdbcCommand("SELECT COUNT(*) FROM tugas_akhir", conn)
            lblTotalTA.Text = cmd.ExecuteScalar().ToString()

            ' TA Pending (Diajukan)
            cmd = New OdbcCommand("SELECT COUNT(*) FROM tugas_akhir WHERE status = 'Diajukan'", conn)
            lblTAPending.Text = cmd.ExecuteScalar().ToString()

        Catch ex As Exception
            MessageBox.Show("Error loading statistics: " & ex.Message)
        End Try
    End Sub

    Private Sub LoadRecentActivities(lstActivities As ListBox)
        Try
            cmd = New OdbcCommand("
                SELECT 
                    CONCAT('TA: ', m.nama_mahasiswa, ' - ', ta.status) as activity,
                    ta.tanggal_daftar as date
                FROM tugas_akhir ta 
                JOIN mahasiswa m ON ta.nim = m.nim 
                ORDER BY ta.tanggal_daftar DESC 
                LIMIT 10", conn)

            Dim reader As OdbcDataReader = cmd.ExecuteReader()
            lstActivities.Items.Clear()

            While reader.Read()
                Dim activity = reader("activity").ToString()
                Dim dateStr = Convert.ToDateTime(reader("date")).ToString("dd/MM/yyyy")
                lstActivities.Items.Add($"{dateStr} - {activity}")
            End While
            reader.Close()

        Catch ex As Exception
            lstActivities.Items.Add("Error loading activities")
        End Try
    End Sub

    Private Sub LoadTAStatusStatistics(parentPanel As Panel)
        Try
            cmd = New OdbcCommand("
                SELECT status, COUNT(*) as count 
                FROM tugas_akhir 
                GROUP BY status", conn)

            Dim reader As OdbcDataReader = cmd.ExecuteReader()
            Dim yPos As Integer = 80

            While reader.Read()
                Dim status = reader("status").ToString()
                Dim count = reader("count").ToString()

                Dim lblStatus As New Label With {
                    .Text = $"{status}: {count}",
                    .Font = New Font("Segoe UI", 10),
                    .Location = New Point(450, yPos),
                    .Size = New Size(200, 20),
                    .ForeColor = Color.FromArgb(52, 73, 94)
                }
                parentPanel.Controls.Add(lblStatus)
                yPos += 25
            End While
            reader.Close()

        Catch ex As Exception
            ' Handle error
        End Try
    End Sub

    ' === EVENT HANDLERS FOR NAVIGATION ===
    Private Sub btnMahasiswa_Click(sender As Object, e As EventArgs)
        ' Panggil form mahasiswa
        MessageBox.Show("Membuka modul Data Mahasiswa")
        ' Contoh: MainForm.ShowUserControl(New crud_mahasiswa())
    End Sub

    Private Sub btnDosen_Click(sender As Object, e As EventArgs)
        MessageBox.Show("Membuka modul Data Dosen")
    End Sub

    Private Sub btnTA_Click(sender As Object, e As EventArgs)
        MessageBox.Show("Membuka modul Tugas Akhir")
    End Sub

    Private Sub btnBimbingan_Click(sender As Object, e As EventArgs)
        MessageBox.Show("Membuka modul Bimbingan")
    End Sub

    Private Sub btnPenilaian_Click(sender As Object, e As EventArgs)
        MessageBox.Show("Membuka modul Penilaian")
    End Sub

    Private Sub btnLaporan_Click(sender As Object, e As EventArgs)
        MessageBox.Show("Membuka modul Laporan")
    End Sub
End Class