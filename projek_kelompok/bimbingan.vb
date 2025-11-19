Imports System.Data.Odbc
Imports System.Drawing
Imports System.Windows.Forms
Imports System.IO

Public Class bimbingan
    Inherits UserControl

    Dim cmd As OdbcCommand
    Dim dr As OdbcDataReader
    Dim nim As String
    Dim id_ta As Integer = 0

    ' Deklarasi komponen UI
    Private cmbBab As ComboBox
    Private txtCatatan As TextBox
    Private btnUpload As Button
    Private btnSubmit As Button
    Private lblFileName As Label
    Private dgvRiwayat As DataGridView
    Private ofdFile As OpenFileDialog
    Private filePath As String = ""
    Private lblStatusInfo As Label

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
        '    .Height = 70,
        '    .BackColor = Color.FromArgb(41, 128, 185)
        '}

        'Dim lblTitle As New Label With {
        '    .Text = "BIMBINGAN TUGAS AKHIR",
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

        ' === PANEL STATUS ===
        Dim statusPanel As New Panel With {
            .Dock = DockStyle.Top,
            .Height = 50,
            .BackColor = Color.FromArgb(248, 249, 250),
            .BorderStyle = BorderStyle.FixedSingle,
            .Padding = New Padding(15)
        }

        lblStatusInfo = New Label With {
            .Text = "Memuat data...",
            .Font = New Font("Segoe UI", 11, FontStyle.Regular),
            .ForeColor = Color.FromArgb(33, 37, 41),
            .Dock = DockStyle.Fill,
            .TextAlign = ContentAlignment.MiddleLeft
        }

        statusPanel.Controls.Add(lblStatusInfo)
        contentPanel.Controls.Add(statusPanel)

        ' === PANEL FORM BIMBINGAN ===
        Dim formPanel As New Panel With {
            .Dock = DockStyle.Top,
            .Height = 300,
            .BackColor = Color.White,
            .Padding = New Padding(0, 20, 0, 0),
            .Margin = New Padding(0, 10, 0, 0)
        }

        ' Label Bab
        Dim lblBab As New Label With {
            .Text = "Pilih Bab *",
            .Font = New Font("Segoe UI", 12, FontStyle.Bold),
            .ForeColor = Color.FromArgb(33, 37, 41),
            .Location = New Point(0, 0),
            .Size = New Size(150, 25)
        }

        cmbBab = New ComboBox With {
            .Location = New Point(0, 30),
            .Size = New Size(300, 35),
            .Font = New Font("Segoe UI", 11),
            .DropDownStyle = ComboBoxStyle.DropDownList
        }

        ' Isi pilihan bab
        cmbBab.Items.AddRange({"Bab 1 - Pendahuluan", "Bab 2 - Landasan Teori", "Bab 3 - Metodologi",
                              "Bab 4 - Implementasi dan Pengujian", "Bab 5 - Kesimpulan dan Saran"})

        ' Label Upload File
        Dim lblUpload As New Label With {
            .Text = "Upload File Bab *",
            .Font = New Font("Segoe UI", 12, FontStyle.Bold),
            .ForeColor = Color.FromArgb(33, 37, 41),
            .Location = New Point(0, 80),
            .Size = New Size(150, 25)
        }

        btnUpload = New Button With {
            .Text = "Pilih File PDF",
            .Font = New Font("Segoe UI", 10),
            .ForeColor = Color.FromArgb(33, 37, 41),
            .BackColor = Color.FromArgb(248, 249, 250),
            .Size = New Size(150, 35),
            .Location = New Point(0, 110),
            .FlatStyle = FlatStyle.Flat
        }
        AddHandler btnUpload.Click, AddressOf btnUpload_Click

        lblFileName = New Label With {
            .Text = "Belum ada file dipilih",
            .Font = New Font("Segoe UI", 9),
            .ForeColor = Color.FromArgb(108, 117, 125),
            .Location = New Point(160, 118),
            .Size = New Size(400, 20)
        }

        ' Label Catatan
        Dim lblCatatan As New Label With {
            .Text = "Catatan untuk Dosen",
            .Font = New Font("Segoe UI", 12, FontStyle.Bold),
            .ForeColor = Color.FromArgb(33, 37, 41),
            .Location = New Point(0, 160),
            .Size = New Size(200, 25)
        }

        txtCatatan = New TextBox With {
            .Location = New Point(0, 190),
            .Size = New Size(600, 80),
            .Font = New Font("Segoe UI", 10),
            .BorderStyle = BorderStyle.FixedSingle,
            .BackColor = Color.FromArgb(248, 249, 250),
            .Multiline = True,
            .ScrollBars = ScrollBars.Vertical
        }

        ' Tombol Submit
        btnSubmit = New Button With {
            .Text = "KIRIM UNTUK BIMBINGAN",
            .Font = New Font("Segoe UI", 11, FontStyle.Bold),
            .ForeColor = Color.White,
            .BackColor = Color.FromArgb(40, 167, 69),
            .Size = New Size(200, 40),
            .Location = New Point(0, 280),
            .FlatStyle = FlatStyle.Flat
        }
        AddHandler btnSubmit.Click, AddressOf btnSubmit_Click

        ' Tambahkan kontrol ke formPanel
        formPanel.Controls.Add(lblBab)
        formPanel.Controls.Add(cmbBab)
        formPanel.Controls.Add(lblUpload)
        formPanel.Controls.Add(btnUpload)
        formPanel.Controls.Add(lblFileName)
        formPanel.Controls.Add(lblCatatan)
        formPanel.Controls.Add(txtCatatan)
        formPanel.Controls.Add(btnSubmit)

        contentPanel.Controls.Add(formPanel)

        ' === PANEL RIWAYAT BIMBINGAN ===
        Dim riwayatPanel As New Panel With {
            .Dock = DockStyle.Fill,
            .BackColor = Color.White,
            .Padding = New Padding(0, 20, 0, 0)
        }

        Dim lblRiwayatTitle As New Label With {
            .Text = "RIWAYAT BIMBINGAN",
            .Font = New Font("Segoe UI", 14, FontStyle.Bold),
            .ForeColor = Color.FromArgb(41, 128, 185),
            .Location = New Point(0, 0),
            .Size = New Size(250, 30)
        }

        riwayatPanel.Controls.Add(lblRiwayatTitle)

        ' DataGridView untuk riwayat bimbingan
        dgvRiwayat = New DataGridView With {
            .Location = New Point(0, 40),
            .Size = New Size(900, 400),
            .BackgroundColor = Color.White,
            .BorderStyle = BorderStyle.FixedSingle,
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            .ReadOnly = True,
            .RowHeadersVisible = False,
            .AllowUserToAddRows = False,
            .AllowUserToDeleteRows = False
        }

        ' Setup columns
        dgvRiwayat.Columns.Add("bab", "Bab")
        dgvRiwayat.Columns.Add("tanggal", "Tanggal Bimbingan")
        dgvRiwayat.Columns.Add("status", "Status")
        dgvRiwayat.Columns.Add("catatan_dosen", "Catatan Dosen")
        dgvRiwayat.Columns.Add("file", "File")

        ' Styling DataGridView
        dgvRiwayat.ColumnHeadersDefaultCellStyle = New DataGridViewCellStyle With {
            .BackColor = Color.FromArgb(41, 128, 185),
            .ForeColor = Color.White,
            .Font = New Font("Segoe UI", 10, FontStyle.Bold)
        }

        dgvRiwayat.EnableHeadersVisualStyles = False

        riwayatPanel.Controls.Add(dgvRiwayat)
        contentPanel.Controls.Add(riwayatPanel)

        ' Setup OpenFileDialog
        ofdFile = New OpenFileDialog With {
            .Filter = "PDF Files (*.pdf)|*.pdf",
            .Title = "Pilih File Bab Tugas Akhir"
        }

        mainPanel.Controls.Add(contentPanel)
        Me.Controls.Add(mainPanel)
    End Sub

    Private Sub LoadData()
        GetIDTA()
        LoadStatusInfo()
        LoadRiwayatBimbingan()
        CheckFormAvailability()
    End Sub

    Private Sub GetIDTA()
        Try
            cmd = New OdbcCommand("SELECT id_ta FROM tugas_akhir WHERE nim = ?", conn)
            cmd.Parameters.AddWithValue("@nim", nim)

            Dim result = cmd.ExecuteScalar()
            If result IsNot Nothing Then
                id_ta = Convert.ToInt32(result)
            Else
                id_ta = 0
            End If

        Catch ex As Exception
            id_ta = 0
        End Try
    End Sub

    Private Sub LoadStatusInfo()
        Try
            If id_ta > 0 Then
                cmd = New OdbcCommand("SELECT ta.status, d.nama_dosen " &
                                      "FROM tugas_akhir ta " &
                                      "LEFT JOIN pembimbing p ON ta.id_ta = p.id_ta " &
                                      "LEFT JOIN dosen d ON p.nidn = d.nidn " &
                                      "WHERE ta.id_ta = ? AND p.peran = 'Pembimbing Utama'", conn)
                cmd.Parameters.AddWithValue("@id_ta", id_ta)

                dr = cmd.ExecuteReader()
                If dr.Read() Then
                    Dim status = dr("status").ToString()
                    Dim dosen = If(dr("nama_dosen") Is DBNull.Value, "Belum ditentukan", dr("nama_dosen").ToString())

                    lblStatusInfo.Text = $"Status TA: {status} | Dosen Pembimbing: {dosen}"

                    If status = "Dibimbing" Then
                        lblStatusInfo.ForeColor = Color.FromArgb(40, 167, 69)
                    Else
                        lblStatusInfo.ForeColor = Color.FromArgb(108, 117, 125)
                    End If
                Else
                    lblStatusInfo.Text = "Status TA: - | Dosen Pembimbing: Belum ditentukan"
                    lblStatusInfo.ForeColor = Color.FromArgb(108, 117, 125)
                End If
                dr.Close()
            Else
                lblStatusInfo.Text = "Anda belum mengajukan Tugas Akhir"
                lblStatusInfo.ForeColor = Color.FromArgb(220, 53, 69)
            End If

        Catch ex As Exception
            lblStatusInfo.Text = "Error memuat data"
            lblStatusInfo.ForeColor = Color.Red
        End Try
    End Sub

    Private Sub CheckFormAvailability()
        If id_ta = 0 Then
            DisableForm("Anda belum mengajukan Tugas Akhir")
        Else
            cmd = New OdbcCommand("SELECT status FROM tugas_akhir WHERE id_ta = ?", conn)
            cmd.Parameters.AddWithValue("@id_ta", id_ta)

            Dim status = cmd.ExecuteScalar()?.ToString()

            If status = "Dibimbing" Then
                EnableForm()
            Else
                DisableForm($"Form bimbingan hanya tersedia untuk status 'Dibimbing'. Status saat ini: {status}")
            End If
        End If
    End Sub

    Private Sub EnableForm()
        cmbBab.Enabled = True
        btnUpload.Enabled = True
        txtCatatan.Enabled = True
        btnSubmit.Enabled = True

        cmbBab.BackColor = Color.White
        txtCatatan.BackColor = Color.White
    End Sub

    Private Sub DisableForm(message As String)
        cmbBab.Enabled = False
        btnUpload.Enabled = False
        txtCatatan.Enabled = False
        btnSubmit.Enabled = False

        cmbBab.BackColor = Color.FromArgb(248, 249, 250)
        txtCatatan.BackColor = Color.FromArgb(248, 249, 250)

        If Not String.IsNullOrEmpty(message) Then
            lblStatusInfo.Text = message
            lblStatusInfo.ForeColor = Color.FromArgb(220, 53, 69)
        End If
    End Sub

    Private Sub btnUpload_Click(sender As Object, e As EventArgs)
        If ofdFile.ShowDialog() = DialogResult.OK Then
            filePath = ofdFile.FileName
            lblFileName.Text = Path.GetFileName(filePath)
            lblFileName.ForeColor = Color.FromArgb(40, 167, 69)
        End If
    End Sub

    Private Sub btnSubmit_Click(sender As Object, e As EventArgs)
        If ValidateForm() Then
            SubmitBimbingan()
        End If
    End Sub

    Private Function ValidateForm() As Boolean
        If cmbBab.SelectedIndex = -1 Then
            MessageBox.Show("Pilih bab terlebih dahulu!", "Validasi Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            cmbBab.Focus()
            Return False
        End If

        If String.IsNullOrEmpty(filePath) Then
            MessageBox.Show("Pilih file PDF terlebih dahulu!", "Validasi Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            btnUpload.Focus()
            Return False
        End If

        ' Cek ekstensi file
        If Path.GetExtension(filePath).ToLower() <> ".pdf" Then
            MessageBox.Show("Hanya file PDF yang diizinkan!", "Validasi Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        ' Cek ukuran file (max 10MB)
        Dim fileInfo As New FileInfo(filePath)
        If fileInfo.Length > 10 * 1024 * 1024 Then
            MessageBox.Show("Ukuran file maksimal 10MB!", "Validasi Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        Return True
    End Function

    Private Sub SubmitBimbingan()
        Try
            ' Extract bab number dari pilihan
            Dim babText As String = cmbBab.SelectedItem.ToString()
            Dim babNumber As String = babText.Split(" ")(1) ' Mengambil angka bab

            ' Generate nama file yang unik
            Dim fileName As String = $"{nim}_Bab_{babNumber}_{DateTime.Now:yyyyMMddHHmmss}.pdf"

            ' Simpan file ke folder (dalam real implementation)
            ' Untuk sekarang kita simpan path filenya saja di database
            Dim savedFilePath As String = Path.Combine("bimbingan_files", fileName)

            ' Insert data bimbingan ke database
            cmd = New OdbcCommand("INSERT INTO bimbingan (id_ta, tanggal_bimbingan, bab, nama_file, catatan_dosen, status_bab) " &
                                  "VALUES (?, ?, ?, ?, ?, 'Menunggu')", conn)
            cmd.Parameters.AddWithValue("@id_ta", id_ta)
            cmd.Parameters.AddWithValue("@tanggal", DateTime.Now.ToString("yyyy-MM-dd"))
            cmd.Parameters.AddWithValue("@bab", babText)
            cmd.Parameters.AddWithValue("@nama_file", fileName)
            cmd.Parameters.AddWithValue("@catatan", If(String.IsNullOrWhiteSpace(txtCatatan.Text), DBNull.Value, txtCatatan.Text.Trim()))

            Dim result = cmd.ExecuteNonQuery()

            If result > 0 Then
                MessageBox.Show("Bab berhasil dikirim untuk bimbingan! Silakan tunggu review dari dosen.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

                ' Reset form
                ResetForm()

                ' Reload riwayat
                LoadRiwayatBimbingan()
            Else
                MessageBox.Show("Gagal mengirim bimbingan. Silakan coba lagi.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If

        Catch ex As Exception
            MessageBox.Show("Error submitting bimbingan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ResetForm()
        cmbBab.SelectedIndex = -1
        txtCatatan.Text = ""
        filePath = ""
        lblFileName.Text = "Belum ada file dipilih"
        lblFileName.ForeColor = Color.FromArgb(108, 117, 125)
    End Sub

    Private Sub LoadRiwayatBimbingan()
        Try
            dgvRiwayat.Rows.Clear()

            If id_ta > 0 Then
                cmd = New OdbcCommand("SELECT bab, tanggal_bimbingan, status_bab, catatan_dosen, nama_file " &
                                      "FROM bimbingan WHERE id_ta = ? ORDER BY tanggal_bimbingan DESC", conn)
                cmd.Parameters.AddWithValue("@id_ta", id_ta)

                dr = cmd.ExecuteReader()

                While dr.Read()
                    Dim bab = dr("bab").ToString()
                    Dim tanggal = Convert.ToDateTime(dr("tanggal_bimbingan")).ToString("dd/MM/yyyy")
                    Dim status = dr("status_bab").ToString()
                    Dim catatan = If(dr("catatan_dosen") Is DBNull.Value, "-", dr("catatan_dosen").ToString())
                    Dim file = dr("nama_file").ToString()

                    ' Tambahkan row ke DataGridView
                    Dim rowIndex = dgvRiwayat.Rows.Add(bab, tanggal, status, catatan, file)

                    ' Warna baris berdasarkan status
                    Select Case status
                        Case "Disetujui"
                            dgvRiwayat.Rows(rowIndex).DefaultCellStyle.BackColor = Color.FromArgb(212, 237, 218)
                        Case "Direvisi"
                            dgvRiwayat.Rows(rowIndex).DefaultCellStyle.BackColor = Color.FromArgb(255, 243, 205)
                        Case "Menunggu"
                            dgvRiwayat.Rows(rowIndex).DefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250)
                    End Select
                End While
                dr.Close()
            End If

        Catch ex As Exception
            MessageBox.Show("Error loading riwayat bimbingan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Method untuk refresh data
    Public Sub RefreshData()
        LoadData()
    End Sub
End Class