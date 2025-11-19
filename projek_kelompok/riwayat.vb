Imports System.Data.Odbc
Imports System.Drawing
Imports System.Windows.Forms

Public Class riwayat
    Inherits UserControl

    Dim cmd As OdbcCommand
    Dim dr As OdbcDataReader
    Dim nim As String
    Dim id_ta As Integer = 0

    ' Deklarasi komponen UI
    Private dgvRiwayat As DataGridView
    Private lblTotalBimbingan As Label
    Private lblDisetujui As Label
    Private lblDirevisi As Label
    Private lblMenunggu As Label
    Private cmbFilterStatus As ComboBox
    Private cmbFilterBab As ComboBox
    Private txtCari As TextBox
    Private btnApplyFilter As Button

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
        '    .Text = "RIWAYAT BIMBINGAN",
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
            .Dock = DockStyle.Top,
            .Height = 100,
            .BackColor = Color.FromArgb(248, 249, 250),
            .BorderStyle = BorderStyle.FixedSingle,
            .Padding = New Padding(20)
        }

        ' Label Total Bimbingan
        Dim lblTotalTitle As New Label With {
            .Text = "Total Bimbingan",
            .Font = New Font("Segoe UI", 11, FontStyle.Bold),
            .ForeColor = Color.FromArgb(108, 117, 125),
            .Location = New Point(20, 15),
            .Size = New Size(150, 25)
        }

        lblTotalBimbingan = New Label With {
            .Text = "0",
            .Font = New Font("Segoe UI", 24, FontStyle.Bold),
            .ForeColor = Color.FromArgb(41, 128, 185),
            .Location = New Point(20, 40),
            .Size = New Size(100, 40)
        }

        ' Label Disetujui
        Dim lblDisetujuiTitle As New Label With {
            .Text = "Disetujui",
            .Font = New Font("Segoe UI", 11, FontStyle.Bold),
            .ForeColor = Color.FromArgb(108, 117, 125),
            .Location = New Point(180, 15),
            .Size = New Size(100, 25)
        }

        lblDisetujui = New Label With {
            .Text = "0",
            .Font = New Font("Segoe UI", 24, FontStyle.Bold),
            .ForeColor = Color.FromArgb(40, 167, 69),
            .Location = New Point(180, 40),
            .Size = New Size(100, 40)
        }

        ' Label Direvisi
        Dim lblDirevisiTitle As New Label With {
            .Text = "Direvisi",
            .Font = New Font("Segoe UI", 11, FontStyle.Bold),
            .ForeColor = Color.FromArgb(108, 117, 125),
            .Location = New Point(340, 15),
            .Size = New Size(100, 25)
        }

        lblDirevisi = New Label With {
            .Text = "0",
            .Font = New Font("Segoe UI", 24, FontStyle.Bold),
            .ForeColor = Color.FromArgb(255, 193, 7),
            .Location = New Point(340, 40),
            .Size = New Size(100, 40)
        }

        ' Label Menunggu
        Dim lblMenungguTitle As New Label With {
            .Text = "Menunggu",
            .Font = New Font("Segoe UI", 11, FontStyle.Bold),
            .ForeColor = Color.FromArgb(108, 117, 125),
            .Location = New Point(500, 15),
            .Size = New Size(100, 25)
        }

        lblMenunggu = New Label With {
            .Text = "0",
            .Font = New Font("Segoe UI", 24, FontStyle.Bold),
            .ForeColor = Color.FromArgb(108, 117, 125),
            .Location = New Point(500, 40),
            .Size = New Size(100, 40)
        }

        statsPanel.Controls.Add(lblTotalTitle)
        statsPanel.Controls.Add(lblTotalBimbingan)
        statsPanel.Controls.Add(lblDisetujuiTitle)
        statsPanel.Controls.Add(lblDisetujui)
        statsPanel.Controls.Add(lblDirevisiTitle)
        statsPanel.Controls.Add(lblDirevisi)
        statsPanel.Controls.Add(lblMenungguTitle)
        statsPanel.Controls.Add(lblMenunggu)

        contentPanel.Controls.Add(statsPanel)

        ' === PANEL FILTER ===
        Dim filterPanel As New Panel With {
            .Dock = DockStyle.Top,
            .Height = 80,
            .BackColor = Color.White,
            .Padding = New Padding(0, 15, 0, 0)
        }

        ' Label Filter
        Dim lblFilter As New Label With {
            .Text = "Filter Data:",
            .Font = New Font("Segoe UI", 11, FontStyle.Bold),
            .ForeColor = Color.FromArgb(33, 37, 41),
            .Location = New Point(0, 5),
            .Size = New Size(100, 25)
        }

        ' Filter Status
        Dim lblStatus As New Label With {
            .Text = "Status:",
            .Font = New Font("Segoe UI", 9),
            .ForeColor = Color.FromArgb(108, 117, 125),
            .Location = New Point(0, 35),
            .Size = New Size(50, 20)
        }

        cmbFilterStatus = New ComboBox With {
            .Location = New Point(0, 55),
            .Size = New Size(150, 25),
            .Font = New Font("Segoe UI", 9),
            .DropDownStyle = ComboBoxStyle.DropDownList
        }
        cmbFilterStatus.Items.AddRange({"Semua Status", "Menunggu", "Direvisi", "Disetujui"})
        cmbFilterStatus.SelectedIndex = 0

        ' Filter Bab
        Dim lblBab As New Label With {
            .Text = "Bab:",
            .Font = New Font("Segoe UI", 9),
            .ForeColor = Color.FromArgb(108, 117, 125),
            .Location = New Point(160, 35),
            .Size = New Size(30, 20)
        }

        cmbFilterBab = New ComboBox With {
            .Location = New Point(160, 55),
            .Size = New Size(180, 25),
            .Font = New Font("Segoe UI", 9),
            .DropDownStyle = ComboBoxStyle.DropDownList
        }
        cmbFilterBab.Items.AddRange({"Semua Bab", "Bab 1 - Pendahuluan", "Bab 2 - Landasan Teori",
                                   "Bab 3 - Metodologi", "Bab 4 - Implementasi dan Pengujian",
                                   "Bab 5 - Kesimpulan dan Saran"})
        cmbFilterBab.SelectedIndex = 0

        ' Pencarian
        Dim lblCari As New Label With {
            .Text = "Pencarian:",
            .Font = New Font("Segoe UI", 9),
            .ForeColor = Color.FromArgb(108, 117, 125),
            .Location = New Point(350, 35),
            .Size = New Size(60, 20)
        }

        txtCari = New TextBox With {
            .Location = New Point(350, 55),
            .Size = New Size(200, 25),
            .Font = New Font("Segoe UI", 9),
            .BorderStyle = BorderStyle.FixedSingle,
            .BackColor = Color.White
        }
        ' Set placeholder text manually
        txtCari.Text = "Cari berdasarkan bab, catatan, atau file..."
        txtCari.ForeColor = Color.Gray
        AddHandler txtCari.GotFocus, AddressOf txtCari_GotFocus
        AddHandler txtCari.LostFocus, AddressOf txtCari_LostFocus

        ' Tombol Apply Filter
        btnApplyFilter = New Button With {
            .Text = "TERAPKAN FILTER",
            .Font = New Font("Segoe UI", 9, FontStyle.Bold),
            .ForeColor = Color.White,
            .BackColor = Color.FromArgb(40, 167, 69),
            .Size = New Size(120, 25),
            .Location = New Point(560, 55),
            .FlatStyle = FlatStyle.Flat
        }
        AddHandler btnApplyFilter.Click, AddressOf btnApplyFilter_Click

        ' Tombol Reset Filter
        Dim btnResetFilter As New Button With {
            .Text = "RESET FILTER",
            .Font = New Font("Segoe UI", 9),
            .ForeColor = Color.FromArgb(33, 37, 41),
            .BackColor = Color.FromArgb(248, 249, 250),
            .Size = New Size(100, 25),
            .Location = New Point(690, 55),
            .FlatStyle = FlatStyle.Flat
        }
        AddHandler btnResetFilter.Click, AddressOf btnResetFilter_Click

        filterPanel.Controls.Add(lblFilter)
        filterPanel.Controls.Add(lblStatus)
        filterPanel.Controls.Add(cmbFilterStatus)
        filterPanel.Controls.Add(lblBab)
        filterPanel.Controls.Add(cmbFilterBab)
        filterPanel.Controls.Add(lblCari)
        filterPanel.Controls.Add(txtCari)
        filterPanel.Controls.Add(btnApplyFilter)
        filterPanel.Controls.Add(btnResetFilter)

        contentPanel.Controls.Add(filterPanel)

        ' === PANEL TABEL RIWAYAT ===
        Dim tablePanel As New Panel With {
            .Dock = DockStyle.Fill,
            .BackColor = Color.White,
            .Padding = New Padding(0, 15, 0, 0)
        }

        ' Label Judul Tabel
        Dim lblTableTitle As New Label With {
            .Text = "DAFTAR RIWAYAT BIMBINGAN",
            .Font = New Font("Segoe UI", 12, FontStyle.Bold),
            .ForeColor = Color.FromArgb(41, 128, 185),
            .Location = New Point(0, 0),
            .Size = New Size(250, 25)
        }
        tablePanel.Controls.Add(lblTableTitle)

        ' DataGridView untuk riwayat bimbingan
        dgvRiwayat = New DataGridView With {
            .Location = New Point(0, 30),
            .Size = New Size(900, 400),
            .BackgroundColor = Color.White,
            .BorderStyle = BorderStyle.FixedSingle,
            .ReadOnly = True,
            .RowHeadersVisible = False,
            .AllowUserToAddRows = False,
            .AllowUserToDeleteRows = False,
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        }

        ' Setup columns
        dgvRiwayat.Columns.Add("no", "No")
        dgvRiwayat.Columns.Add("bab", "Bab")
        dgvRiwayat.Columns.Add("tanggal", "Tanggal Bimbingan")
        dgvRiwayat.Columns.Add("status", "Status")
        dgvRiwayat.Columns.Add("catatan_dosen", "Catatan Dosen")
        dgvRiwayat.Columns.Add("file", "Nama File")

        ' Styling DataGridView
        dgvRiwayat.ColumnHeadersDefaultCellStyle = New DataGridViewCellStyle With {
            .BackColor = Color.FromArgb(41, 128, 185),
            .ForeColor = Color.White,
            .Font = New Font("Segoe UI", 10, FontStyle.Bold),
            .Alignment = DataGridViewContentAlignment.MiddleCenter
        }

        dgvRiwayat.EnableHeadersVisualStyles = False

        ' Style untuk kolom
        dgvRiwayat.Columns("no").Width = 50
        dgvRiwayat.Columns("no").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        dgvRiwayat.Columns("tanggal").Width = 120
        dgvRiwayat.Columns("tanggal").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        dgvRiwayat.Columns("status").Width = 100
        dgvRiwayat.Columns("status").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

        tablePanel.Controls.Add(dgvRiwayat)
        contentPanel.Controls.Add(tablePanel)

        mainPanel.Controls.Add(contentPanel)
        Me.Controls.Add(mainPanel)
    End Sub

    ' Methods untuk placeholder text
    Private Sub txtCari_GotFocus(sender As Object, e As EventArgs)
        If txtCari.Text = "Cari berdasarkan bab, catatan, atau file..." Then
            txtCari.Text = ""
            txtCari.ForeColor = Color.Black
        End If
    End Sub

    Private Sub txtCari_LostFocus(sender As Object, e As EventArgs)
        If String.IsNullOrWhiteSpace(txtCari.Text) Then
            txtCari.Text = "Cari berdasarkan bab, catatan, atau file..."
            txtCari.ForeColor = Color.Gray
        End If
    End Sub

    Private Sub LoadData()
        GetIDTA()
        LoadStatistics()
        LoadRiwayatBimbingan()
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

    Private Sub LoadStatistics()
        Try
            If id_ta > 0 Then
                ' Total bimbingan
                cmd = New OdbcCommand("SELECT COUNT(*) FROM bimbingan WHERE id_ta = ?", conn)
                cmd.Parameters.AddWithValue("@id_ta", id_ta)
                lblTotalBimbingan.Text = cmd.ExecuteScalar().ToString()

                ' Bimbingan disetujui
                cmd = New OdbcCommand("SELECT COUNT(*) FROM bimbingan WHERE id_ta = ? AND status_bab = 'Disetujui'", conn)
                cmd.Parameters.AddWithValue("@id_ta", id_ta)
                lblDisetujui.Text = cmd.ExecuteScalar().ToString()

                ' Bimbingan direvisi
                cmd = New OdbcCommand("SELECT COUNT(*) FROM bimbingan WHERE id_ta = ? AND status_bab = 'Direvisi'", conn)
                cmd.Parameters.AddWithValue("@id_ta", id_ta)
                lblDirevisi.Text = cmd.ExecuteScalar().ToString()

                ' Bimbingan menunggu
                cmd = New OdbcCommand("SELECT COUNT(*) FROM bimbingan WHERE id_ta = ? AND status_bab = 'Menunggu'", conn)
                cmd.Parameters.AddWithValue("@id_ta", id_ta)
                lblMenunggu.Text = cmd.ExecuteScalar().ToString()
            Else
                lblTotalBimbingan.Text = "0"
                lblDisetujui.Text = "0"
                lblDirevisi.Text = "0"
                lblMenunggu.Text = "0"
            End If

        Catch ex As Exception
            MessageBox.Show("Error loading statistics: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub LoadRiwayatBimbingan()
        Try
            dgvRiwayat.Rows.Clear()

            If id_ta > 0 Then
                Dim query As String = "SELECT bab, tanggal_bimbingan, status_bab, catatan_dosen, nama_file " &
                                      "FROM bimbingan WHERE id_ta = ? "

                ' Apply filters
                Dim parameters As New List(Of OdbcParameter)()
                parameters.Add(New OdbcParameter("@id_ta", id_ta))

                If cmbFilterStatus.SelectedIndex > 0 Then
                    query &= "AND status_bab = ? "
                    parameters.Add(New OdbcParameter("@status", cmbFilterStatus.SelectedItem.ToString()))
                End If

                If cmbFilterBab.SelectedIndex > 0 Then
                    query &= "AND bab = ? "
                    parameters.Add(New OdbcParameter("@bab", cmbFilterBab.SelectedItem.ToString()))
                End If

                ' Handle placeholder text in search
                Dim searchText As String = txtCari.Text
                If searchText <> "Cari berdasarkan bab, catatan, atau file..." AndAlso Not String.IsNullOrWhiteSpace(searchText) Then
                    query &= "AND (bab LIKE ? OR catatan_dosen LIKE ? OR nama_file LIKE ?) "
                    parameters.Add(New OdbcParameter("@search", $"%{searchText}%"))
                    parameters.Add(New OdbcParameter("@search2", $"%{searchText}%"))
                    parameters.Add(New OdbcParameter("@search3", $"%{searchText}%"))
                End If

                query &= "ORDER BY tanggal_bimbingan DESC"

                cmd = New OdbcCommand(query, conn)
                For Each param As OdbcParameter In parameters
                    cmd.Parameters.Add(param)
                Next

                dr = cmd.ExecuteReader()

                Dim counter As Integer = 1
                While dr.Read()
                    Dim bab = dr("bab").ToString()
                    Dim tanggal = Convert.ToDateTime(dr("tanggal_bimbingan")).ToString("dd/MM/yyyy")
                    Dim status = dr("status_bab").ToString()
                    Dim catatan = If(dr("catatan_dosen") Is DBNull.Value, "-", dr("catatan_dosen").ToString())
                    Dim file = dr("nama_file").ToString()

                    ' Tambahkan row ke DataGridView
                    Dim rowIndex = dgvRiwayat.Rows.Add(counter.ToString(), bab, tanggal, status, catatan, file)

                    ' Warna baris berdasarkan status
                    Select Case status
                        Case "Disetujui"
                            dgvRiwayat.Rows(rowIndex).DefaultCellStyle.BackColor = Color.FromArgb(212, 237, 218)
                            dgvRiwayat.Rows(rowIndex).Cells("status").Style.ForeColor = Color.FromArgb(21, 87, 36)
                        Case "Direvisi"
                            dgvRiwayat.Rows(rowIndex).DefaultCellStyle.BackColor = Color.FromArgb(255, 243, 205)
                            dgvRiwayat.Rows(rowIndex).Cells("status").Style.ForeColor = Color.FromArgb(133, 100, 4)
                        Case "Menunggu"
                            dgvRiwayat.Rows(rowIndex).DefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250)
                            dgvRiwayat.Rows(rowIndex).Cells("status").Style.ForeColor = Color.FromArgb(108, 117, 125)
                    End Select

                    counter += 1
                End While
                dr.Close()

                If dgvRiwayat.Rows.Count = 0 Then
                    dgvRiwayat.Rows.Add("1", "Tidak ada data yang sesuai dengan filter", "-", "-", "-", "-")
                    dgvRiwayat.Rows(0).DefaultCellStyle.ForeColor = Color.FromArgb(108, 117, 125)
                    dgvRiwayat.Rows(0).DefaultCellStyle.Font = New Font("Segoe UI", 9, FontStyle.Italic)
                End If
            Else
                dgvRiwayat.Rows.Add("1", "Belum ada riwayat bimbingan", "-", "-", "-", "-")
                dgvRiwayat.Rows(0).DefaultCellStyle.ForeColor = Color.FromArgb(108, 117, 125)
                dgvRiwayat.Rows(0).DefaultCellStyle.Font = New Font("Segoe UI", 9, FontStyle.Italic)
            End If

        Catch ex As Exception
            MessageBox.Show("Error loading riwayat bimbingan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnApplyFilter_Click(sender As Object, e As EventArgs)
        LoadRiwayatBimbingan()
        MessageBox.Show("Filter berhasil diterapkan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub btnResetFilter_Click(sender As Object, e As EventArgs)
        cmbFilterStatus.SelectedIndex = 0
        cmbFilterBab.SelectedIndex = 0
        txtCari.Text = "Cari berdasarkan bab, catatan, atau file..."
        txtCari.ForeColor = Color.Gray
        LoadRiwayatBimbingan()
        MessageBox.Show("Filter berhasil direset!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    ' Method untuk refresh data
    Public Sub RefreshData()
        LoadData()
    End Sub
End Class