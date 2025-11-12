Imports System.Data.Odbc
Imports System.Drawing

Public Class pengajuan_ta
    Inherits UserControl

    Dim cmd As OdbcCommand
    Dim dr As OdbcDataReader
    Dim nim_login As String

    ' Komponen utama
    Dim lblJudul, lblDeskripsi, lblStatus, lblJudulTA, lblKetStatus As Label
    Dim txtJudul, txtDeskripsi As TextBox
    Dim btnAjukan, btnBatal As Button
    Dim panelStatus As Panel

    Public Sub New(nim As String)
        nim_login = nim
        InitializeComponent()
        Connect()
        BuildUI()
        LoadPengajuan()
    End Sub

    Private Sub BuildUI()
        Me.Dock = DockStyle.Fill
        Me.BackColor = Color.FromArgb(245, 247, 250)

        ' === Header ===
        Dim lblHeader As New Label With {
            .Text = "PENGAJUAN TUGAS AKHIR",
            .Font = New Font("Segoe UI Semibold", 18, FontStyle.Bold),
            .ForeColor = Color.FromArgb(33, 33, 33),
            .Dock = DockStyle.Top,
            .Height = 60,
            .TextAlign = ContentAlignment.MiddleCenter
        }
        Me.Controls.Add(lblHeader)

        ' === Layout utama ===
        Dim container As New Panel With {
            .Dock = DockStyle.Fill,
            .Padding = New Padding(50)
        }
        Me.Controls.Add(container)
        container.BringToFront()

        ' Label Judul TA
        lblJudul = New Label With {
            .Text = "Judul Tugas Akhir:",
            .Font = New Font("Segoe UI", 11, FontStyle.Regular),
            .AutoSize = True,
            .Location = New Point(50, 40)
        }
        container.Controls.Add(lblJudul)

        ' Textbox Judul
        txtJudul = New TextBox With {
            .Font = New Font("Segoe UI", 11),
            .Width = 500,
            .Location = New Point(50, 70)
        }
        container.Controls.Add(txtJudul)

        ' Label Deskripsi
        lblDeskripsi = New Label With {
            .Text = "Deskripsi Singkat:",
            .Font = New Font("Segoe UI", 11, FontStyle.Regular),
            .AutoSize = True,
            .Location = New Point(50, 120)
        }
        container.Controls.Add(lblDeskripsi)

        ' Textbox Deskripsi
        txtDeskripsi = New TextBox With {
            .Multiline = True,
            .Font = New Font("Segoe UI", 11),
            .Width = 500,
            .Height = 120,
            .Location = New Point(50, 150)
        }
        container.Controls.Add(txtDeskripsi)

        ' Tombol Ajukan
        btnAjukan = New Button With {
            .Text = "Ajukan Judul",
            .Font = New Font("Segoe UI Semibold", 11, FontStyle.Bold),
            .Width = 160,
            .Height = 40,
            .Location = New Point(50, 290),
            .BackColor = Color.FromArgb(76, 175, 80),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat
        }
        AddHandler btnAjukan.Click, AddressOf btnAjukan_Click
        container.Controls.Add(btnAjukan)

        ' Tombol Batalkan
        btnBatal = New Button With {
            .Text = "Batalkan Pengajuan",
            .Font = New Font("Segoe UI Semibold", 11, FontStyle.Bold),
            .Width = 200,
            .Height = 40,
            .Location = New Point(220, 290),
            .BackColor = Color.FromArgb(244, 67, 54),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat,
            .Visible = False
        }
        AddHandler btnBatal.Click, AddressOf btnBatal_Click
        container.Controls.Add(btnBatal)

        ' Panel Status
        panelStatus = New Panel With {
            .Width = 300,
            .Height = 120,
            .BackColor = Color.White,
            .Location = New Point(600, 70),
            .Padding = New Padding(10),
            .BorderStyle = BorderStyle.FixedSingle
        }
        container.Controls.Add(panelStatus)

        lblJudulTA = New Label With {
            .Text = "Status Pengajuan:",
            .Font = New Font("Segoe UI Semibold", 11, FontStyle.Bold),
            .Dock = DockStyle.Top,
            .Height = 25,
            .TextAlign = ContentAlignment.MiddleCenter
        }
        panelStatus.Controls.Add(lblJudulTA)

        lblKetStatus = New Label With {
            .Text = "Belum Mengajukan",
            .Font = New Font("Segoe UI", 12, FontStyle.Bold),
            .ForeColor = Color.FromArgb(97, 97, 97),
            .Dock = DockStyle.Fill,
            .TextAlign = ContentAlignment.MiddleCenter
        }
        panelStatus.Controls.Add(lblKetStatus)
    End Sub

    Private Sub LoadPengajuan()
        Try
            Dim query As String = "SELECT * FROM tugas_akhir WHERE nim=? ORDER BY id_ta DESC LIMIT 1"
            cmd = New OdbcCommand(query, conn)
            cmd.Parameters.AddWithValue("@nim", nim_login)
            dr = cmd.ExecuteReader()

            If dr.Read() Then
                txtJudul.Text = dr("judul_ta").ToString()
                txtDeskripsi.Text = dr("deskripsi").ToString()
                lblKetStatus.Text = dr("status").ToString()

                Select Case dr("status").ToString()
                    Case "Diajukan"
                        panelStatus.BackColor = Color.FromArgb(255, 249, 196) ' kuning
                        btnAjukan.Enabled = False
                        btnBatal.Visible = True
                    Case "Ditolak"
                        panelStatus.BackColor = Color.FromArgb(239, 154, 154) ' merah
                        btnAjukan.Enabled = True
                        btnBatal.Visible = False
                    Case "Dibimbing"
                        panelStatus.BackColor = Color.FromArgb(179, 229, 252) ' biru
                        btnAjukan.Enabled = False
                        btnBatal.Visible = False
                    Case "Selesai"
                        panelStatus.BackColor = Color.FromArgb(200, 230, 201) ' hijau
                        btnAjukan.Enabled = False
                        btnBatal.Visible = False
                    Case Else
                        panelStatus.BackColor = Color.LightGray
                End Select
            Else
                lblKetStatus.Text = "Belum Mengajukan"
                panelStatus.BackColor = Color.LightGray
            End If
            dr.Close()

        Catch ex As Exception
            MessageBox.Show("Kesalahan saat memuat data pengajuan: " & ex.Message)
        End Try
    End Sub

    Private Sub btnAjukan_Click(sender As Object, e As EventArgs)
        Try
            If txtJudul.Text.Trim() = "" Or txtDeskripsi.Text.Trim() = "" Then
                MessageBox.Show("Isi judul dan deskripsi terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Dim query As String = "INSERT INTO tugas_akhir (nim, judul_ta, deskripsi, status, tanggal_daftar) VALUES (?, ?, ?, 'Diajukan', CURDATE())"
            cmd = New OdbcCommand(query, conn)
            cmd.Parameters.AddWithValue("@nim", nim_login)
            cmd.Parameters.AddWithValue("@judul_ta", txtJudul.Text)
            cmd.Parameters.AddWithValue("@deskripsi", txtDeskripsi.Text)
            cmd.ExecuteNonQuery()

            MessageBox.Show("Judul Tugas Akhir berhasil diajukan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
            LoadPengajuan()

        Catch ex As Exception
            MessageBox.Show("Kesalahan saat mengajukan TA: " & ex.Message)
        End Try
    End Sub

    Private Sub btnBatal_Click(sender As Object, e As EventArgs)
        Try
            Dim konfirmasi As DialogResult = MessageBox.Show("Apakah Anda yakin ingin membatalkan pengajuan?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            If konfirmasi = DialogResult.Yes Then
                Dim query As String = "DELETE FROM tugas_akhir WHERE nim=? AND status='Diajukan'"
                cmd = New OdbcCommand(query, conn)
                cmd.Parameters.AddWithValue("@nim", nim_login)
                cmd.ExecuteNonQuery()

                MessageBox.Show("Pengajuan berhasil dibatalkan.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
                LoadPengajuan()
            End If
        Catch ex As Exception
            MessageBox.Show("Kesalahan saat membatalkan pengajuan: " & ex.Message)
        End Try
    End Sub

End Class
