Imports System.Data.Odbc
Imports System.Drawing
Imports System.Windows.Forms

Public Class pengajuan_ta
    Inherits UserControl

    Dim cmd As OdbcCommand
    Dim dr As OdbcDataReader
    Dim nim As String

    ' Deklarasi komponen UI
    Private txtJudulTA As TextBox
    Private txtDeskripsiTA As TextBox
    Private btnSubmit As Button
    Private btnReset As Button
    Private lblStatus As Label
    Private panelStatus As Panel

    Public Sub New(nim_login As String)
        InitializeComponent()
        nim = nim_login
        Connect()
        InitializeUI()
        CheckExistingTA()
    End Sub

    Private Sub InitializeUI()
        Me.Dock = DockStyle.Fill
        Me.BackColor = Color.White
        Me.AutoScroll = True

        ' === CONTAINER UTAMA ===
        Dim mainPanel As New Panel With {
            .Dock = DockStyle.Fill,
            .BackColor = Color.White,
            .AutoScroll = True,
            .Padding = New Padding(30)
        }

        '' === HEADER ===
        'Dim headerPanel As New Panel With {
        '    .Dock = DockStyle.Top,
        '    .Height = 70,
        '    .BackColor = Color.FromArgb(41, 128, 185)
        '}

        'Dim lblTitle As New Label With {
        '    .Text = "PENGAJUAN TUGAS AKHIR",
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
            .Padding = New Padding(40, 30, 40, 30),
            .BackColor = Color.White
        }

        ' === PANEL STATUS ===
        panelStatus = New Panel With {
            .Dock = DockStyle.Top,
            .Height = 60,
            .BackColor = Color.FromArgb(248, 249, 250),
            .BorderStyle = BorderStyle.FixedSingle,
            .Padding = New Padding(20),
            .Visible = False
        }

        lblStatus = New Label With {
            .Text = "",
            .Font = New Font("Segoe UI", 12, FontStyle.Bold),
            .ForeColor = Color.FromArgb(33, 37, 41),
            .Dock = DockStyle.Fill,
            .TextAlign = ContentAlignment.MiddleLeft
        }

        panelStatus.Controls.Add(lblStatus)
        contentPanel.Controls.Add(panelStatus)

        ' === PANEL FORM ===
        Dim formPanel As New Panel With {
            .Dock = DockStyle.Top,
            .Height = 400,
            .BackColor = Color.White,
            .Padding = New Padding(0, 20, 0, 0)
        }

        ' Label Judul TA
        Dim lblJudul As New Label With {
            .Text = "Judul Tugas Akhir *",
            .Font = New Font("Segoe UI", 12, FontStyle.Bold),
            .ForeColor = Color.FromArgb(33, 37, 41),
            .Location = New Point(0, 20),
            .Size = New Size(200, 25)
        }

        txtJudulTA = New TextBox With {
            .Location = New Point(0, 50),
            .Size = New Size(600, 35),
            .Font = New Font("Segoe UI", 11),
            .BorderStyle = BorderStyle.FixedSingle,
            .BackColor = Color.FromArgb(248, 249, 250),
            .Multiline = False
        }

        ' Label Deskripsi TA
        Dim lblDeskripsi As New Label With {
            .Text = "Deskripsi Tugas Akhir *",
            .Font = New Font("Segoe UI", 12, FontStyle.Bold),
            .ForeColor = Color.FromArgb(33, 37, 41),
            .Location = New Point(0, 110),
            .Size = New Size(200, 25)
        }

        txtDeskripsiTA = New TextBox With {
            .Location = New Point(0, 140),
            .Size = New Size(600, 120),
            .Font = New Font("Segoe UI", 11),
            .BorderStyle = BorderStyle.FixedSingle,
            .BackColor = Color.FromArgb(248, 249, 250),
            .Multiline = True,
            .ScrollBars = ScrollBars.Vertical
        }

        ' Panel Tombol
        Dim buttonPanel As New Panel With {
            .Location = New Point(0, 280),
            .Size = New Size(600, 50)
        }

        btnSubmit = New Button With {
            .Text = "AJUKAN TUGAS AKHIR",
            .Font = New Font("Segoe UI", 11, FontStyle.Bold),
            .ForeColor = Color.White,
            .BackColor = Color.FromArgb(40, 167, 69),
            .Size = New Size(200, 40),
            .Location = New Point(0, 0),
            .FlatStyle = FlatStyle.Flat
        }
        AddHandler btnSubmit.Click, AddressOf btnSubmit_Click

        btnReset = New Button With {
            .Text = "RESET FORM",
            .Font = New Font("Segoe UI", 11, FontStyle.Regular),
            .ForeColor = Color.FromArgb(33, 37, 41),
            .BackColor = Color.FromArgb(248, 249, 250),
            .Size = New Size(150, 40),
            .Location = New Point(220, 0),
            .FlatStyle = FlatStyle.Flat
        }
        AddHandler btnReset.Click, AddressOf btnReset_Click

        buttonPanel.Controls.Add(btnSubmit)
        buttonPanel.Controls.Add(btnReset)

        ' Tambahkan kontrol ke formPanel
        formPanel.Controls.Add(lblJudul)
        formPanel.Controls.Add(txtJudulTA)
        formPanel.Controls.Add(lblDeskripsi)
        formPanel.Controls.Add(txtDeskripsiTA)
        formPanel.Controls.Add(buttonPanel)

        contentPanel.Controls.Add(formPanel)

        ' === PANEL INFORMASI ===
        Dim infoPanel As New Panel With {
            .Dock = DockStyle.Top,
            .Height = 150,
            .BackColor = Color.FromArgb(248, 249, 250),
            .BorderStyle = BorderStyle.FixedSingle,
            .Padding = New Padding(20),
            .Margin = New Padding(0, 20, 0, 0)
        }

        Dim lblInfoTitle As New Label With {
            .Text = "Informasi Pengajuan",
            .Font = New Font("Segoe UI", 14, FontStyle.Bold),
            .ForeColor = Color.FromArgb(41, 128, 185),
            .Location = New Point(0, 0),
            .Size = New Size(200, 30)
        }

        Dim lblInfo1 As New Label With {
            .Text = "• Pastikan judul tugas akhir jelas dan deskriptif",
            .Font = New Font("Segoe UI", 10),
            .ForeColor = Color.FromArgb(108, 117, 125),
            .Location = New Point(0, 40),
            .Size = New Size(400, 20)
        }

        Dim lblInfo2 As New Label With {
            .Text = "• Deskripsi harus menjelaskan latar belakang dan tujuan penelitian",
            .Font = New Font("Segoe UI", 10),
            .ForeColor = Color.FromArgb(108, 117, 125),
            .Location = New Point(0, 65),
            .Size = New Size(500, 20)
        }

        Dim lblInfo3 As New Label With {
            .Text = "• Status pengajuan akan diverifikasi oleh admin",
            .Font = New Font("Segoe UI", 10),
            .ForeColor = Color.FromArgb(108, 117, 125),
            .Location = New Point(0, 90),
            .Size = New Size(400, 20)
        }

        infoPanel.Controls.Add(lblInfoTitle)
        infoPanel.Controls.Add(lblInfo1)
        infoPanel.Controls.Add(lblInfo2)
        infoPanel.Controls.Add(lblInfo3)

        contentPanel.Controls.Add(infoPanel)

        mainPanel.Controls.Add(contentPanel)
        Me.Controls.Add(mainPanel)
    End Sub

    Private Sub CheckExistingTA()
        Try
            cmd = New OdbcCommand("SELECT id_ta, judul_ta, deskripsi, status FROM tugas_akhir WHERE nim = ?", conn)
            cmd.Parameters.AddWithValue("@nim", nim)

            dr = cmd.ExecuteReader()
            If dr.Read() Then
                ' Mahasiswa sudah memiliki TA
                Dim status = dr("status").ToString()
                Dim judul = dr("judul_ta").ToString()
                Dim deskripsi = If(dr("deskripsi") Is DBNull.Value, "", dr("deskripsi").ToString())

                ShowStatusInfo(status, judul, deskripsi)

                ' Nonaktifkan form jika status bukan "Diajukan"
                If status <> "Diajukan" Then
                    DisableForm()
                End If
            Else
                ' Mahasiswa belum memiliki TA, form aktif
                EnableForm()
            End If
            dr.Close()

        Catch ex As Exception
            MessageBox.Show("Error checking existing TA: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ShowStatusInfo(status As String, judul As String, deskripsi As String)
        panelStatus.Visible = True

        Select Case status
            Case "Diajukan"
                lblStatus.Text = "✅ Tugas Akhir Anda telah diajukan dan sedang menunggu verifikasi."
                panelStatus.BackColor = Color.FromArgb(212, 237, 218) ' Hijau muda
                lblStatus.ForeColor = Color.FromArgb(21, 87, 36)

                ' Isi form dengan data yang sudah ada
                txtJudulTA.Text = judul
                txtDeskripsiTA.Text = deskripsi

            Case "Dibimbing"
                lblStatus.Text = "📚 Tugas Akhir Anda telah disetujui dan sedang dalam proses bimbingan."
                panelStatus.BackColor = Color.FromArgb(209, 231, 221) ' Biru muda
                lblStatus.ForeColor = Color.FromArgb(12, 84, 96)

            Case "Selesai"
                lblStatus.Text = "🎉 Selamat! Tugas Akhir Anda telah selesai."
                panelStatus.BackColor = Color.FromArgb(214, 233, 198) ' Hijau success
                lblStatus.ForeColor = Color.FromArgb(60, 118, 61)

            Case "Ditolak"
                lblStatus.Text = "❌ Pengajuan Tugas Akhir Anda ditolak. Silakan ajukan kembali dengan perbaikan."
                panelStatus.BackColor = Color.FromArgb(248, 215, 218) ' Merah muda
                lblStatus.ForeColor = Color.FromArgb(114, 28, 36)

                ' Enable form untuk pengajuan ulang
                EnableForm()
        End Select
    End Sub

    Private Sub EnableForm()
        txtJudulTA.Enabled = True
        txtDeskripsiTA.Enabled = True
        btnSubmit.Enabled = True
        btnReset.Enabled = True

        txtJudulTA.BackColor = Color.White
        txtDeskripsiTA.BackColor = Color.White
    End Sub

    Private Sub DisableForm()
        txtJudulTA.Enabled = False
        txtDeskripsiTA.Enabled = False
        btnSubmit.Enabled = False
        btnReset.Enabled = False

        txtJudulTA.BackColor = Color.FromArgb(248, 249, 250)
        txtDeskripsiTA.BackColor = Color.FromArgb(248, 249, 250)
    End Sub

    Private Sub btnSubmit_Click(sender As Object, e As EventArgs)
        If ValidateForm() Then
            SubmitTA()
        End If
    End Sub

    Private Sub btnReset_Click(sender As Object, e As EventArgs)
        ResetForm()
    End Sub

    Private Function ValidateForm() As Boolean
        If String.IsNullOrWhiteSpace(txtJudulTA.Text) Then
            MessageBox.Show("Judul Tugas Akhir harus diisi!", "Validasi Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtJudulTA.Focus()
            Return False
        End If

        If String.IsNullOrWhiteSpace(txtDeskripsiTA.Text) Then
            MessageBox.Show("Deskripsi Tugas Akhir harus diisi!", "Validasi Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtDeskripsiTA.Focus()
            Return False
        End If

        If txtJudulTA.Text.Length < 10 Then
            MessageBox.Show("Judul Tugas Akhir minimal 10 karakter!", "Validasi Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtJudulTA.Focus()
            Return False
        End If

        If txtDeskripsiTA.Text.Length < 50 Then
            MessageBox.Show("Deskripsi Tugas Akhir minimal 50 karakter!", "Validasi Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtDeskripsiTA.Focus()
            Return False
        End If

        Return True
    End Function

    Private Sub SubmitTA()
        Try
            ' Cek apakah sudah ada TA
            cmd = New OdbcCommand("SELECT id_ta FROM tugas_akhir WHERE nim = ?", conn)
            cmd.Parameters.AddWithValue("@nim", nim)

            Dim existingId = cmd.ExecuteScalar()

            If existingId IsNot Nothing Then
                ' Update TA yang sudah ada (untuk status Ditolak atau revisi)
                cmd = New OdbcCommand("UPDATE tugas_akhir SET judul_ta = ?, deskripsi = ?, status = 'Diajukan', tanggal_daftar = ? WHERE nim = ?", conn)
                cmd.Parameters.AddWithValue("@judul", txtJudulTA.Text.Trim())
                cmd.Parameters.AddWithValue("@deskripsi", txtDeskripsiTA.Text.Trim())
                cmd.Parameters.AddWithValue("@tanggal", DateTime.Now.ToString("yyyy-MM-dd"))
                cmd.Parameters.AddWithValue("@nim", nim)
            Else
                ' Insert TA baru
                cmd = New OdbcCommand("INSERT INTO tugas_akhir (nim, judul_ta, deskripsi, tanggal_daftar, status) VALUES (?, ?, ?, ?, 'Diajukan')", conn)
                cmd.Parameters.AddWithValue("@nim", nim)
                cmd.Parameters.AddWithValue("@judul", txtJudulTA.Text.Trim())
                cmd.Parameters.AddWithValue("@deskripsi", txtDeskripsiTA.Text.Trim())
                cmd.Parameters.AddWithValue("@tanggal", DateTime.Now.ToString("yyyy-MM-dd"))
            End If

            Dim result = cmd.ExecuteNonQuery()

            If result > 0 Then
                MessageBox.Show("Tugas Akhir berhasil diajukan! Silakan tunggu verifikasi dari admin.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

                ' Update status panel
                panelStatus.Visible = True
                lblStatus.Text = "✅ Tugas Akhir Anda telah diajukan dan sedang menunggu verifikasi."
                panelStatus.BackColor = Color.FromArgb(212, 237, 218)
                lblStatus.ForeColor = Color.FromArgb(21, 87, 36)

                ' Nonaktifkan form setelah pengajuan berhasil
                DisableForm()
            Else
                MessageBox.Show("Gagal mengajukan Tugas Akhir. Silakan coba lagi.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If

        Catch ex As Exception
            MessageBox.Show("Error submitting TA: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ResetForm()
        If MessageBox.Show("Apakah Anda yakin ingin mereset form? Semua data yang telah diisi akan hilang.", "Konfirmasi Reset", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            txtJudulTA.Text = ""
            txtDeskripsiTA.Text = ""
            txtJudulTA.Focus()
        End If
    End Sub

    ' Method untuk refresh data (bisa dipanggil dari parent form)
    Public Sub RefreshData()
        CheckExistingTA()
    End Sub
End Class