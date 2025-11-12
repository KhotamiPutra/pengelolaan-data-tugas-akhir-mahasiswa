Imports System.Data.Odbc
Imports System.IO
Imports System.Drawing

Public Class bimbingan
    Inherits UserControl

    Dim cmd As OdbcCommand
    Dim da As OdbcDataAdapter
    Dim ds As DataSet
    Dim nim_login As String
    Dim id_ta As Integer = 0
    Dim selectedFilePath As String = ""

    ' Komponen utama
    Dim lblJudul, lblFile, lblList As Label
    Dim txtFile As TextBox
    Dim cmbBab As ComboBox
    Dim btnBrowse, btnUpload As Button
    Dim dgvBimbingan As DataGridView

    Public Sub New(nim As String)
        nim_login = nim
        InitializeComponent()
        Connect()
        BuildUI()
        LoadIdTA()
        LoadDataBimbingan()
    End Sub

    Private Sub BuildUI()
        Me.Dock = DockStyle.Fill
        Me.BackColor = Color.FromArgb(245, 247, 250)

        ' ==== Header ====
        Dim lblHeader As New Label With {
            .Text = "BIMBINGAN TUGAS AKHIR",
            .Font = New Font("Segoe UI Semibold", 18, FontStyle.Bold),
            .ForeColor = Color.FromArgb(33, 33, 33),
            .Dock = DockStyle.Top,
            .Height = 60,
            .TextAlign = ContentAlignment.MiddleCenter
        }
        Me.Controls.Add(lblHeader)

        ' ==== Container ====
        Dim container As New Panel With {
            .Dock = DockStyle.Fill,
            .Padding = New Padding(40)
        }
        Me.Controls.Add(container)
        container.BringToFront()

        ' ==== Label File ====
        lblFile = New Label With {
            .Text = "Pilih File Bimbingan:",
            .Font = New Font("Segoe UI", 11),
            .Location = New Point(30, 30)
        }
        container.Controls.Add(lblFile)

        ' ==== ComboBox Bab ====
        cmbBab = New ComboBox With {
            .Font = New Font("Segoe UI", 10),
            .Location = New Point(30, 60),
            .Width = 120
        }
        cmbBab.Items.AddRange(New String() {"Bab 1", "Bab 2", "Bab 3", "Bab 4", "Bab 5"})
        cmbBab.SelectedIndex = 0
        container.Controls.Add(cmbBab)

        ' ==== Textbox File ====
        txtFile = New TextBox With {
            .Width = 350,
            .Font = New Font("Segoe UI", 10),
            .Location = New Point(170, 60),
            .ReadOnly = True
        }
        container.Controls.Add(txtFile)

        ' ==== Tombol Browse ====
        btnBrowse = New Button With {
            .Text = "Browse...",
            .Font = New Font("Segoe UI", 10),
            .Location = New Point(530, 58),
            .Width = 100
        }
        AddHandler btnBrowse.Click, AddressOf btnBrowse_Click
        container.Controls.Add(btnBrowse)

        ' ==== Tombol Upload ====
        btnUpload = New Button With {
            .Text = "Upload",
            .Font = New Font("Segoe UI Semibold", 10, FontStyle.Bold),
            .Location = New Point(650, 58),
            .Width = 100,
            .BackColor = Color.FromArgb(76, 175, 80),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat
        }
        AddHandler btnUpload.Click, AddressOf btnUpload_Click
        container.Controls.Add(btnUpload)

        ' ==== Label List ====
        lblList = New Label With {
            .Text = "Riwayat Bimbingan:",
            .Font = New Font("Segoe UI Semibold", 12, FontStyle.Bold),
            .Location = New Point(30, 110)
        }
        container.Controls.Add(lblList)

        ' ==== DataGridView ====
        dgvBimbingan = New DataGridView With {
            .Location = New Point(30, 140),
            .Width = 950,
            .Height = 400,
            .AllowUserToAddRows = False,
            .ReadOnly = True,
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            .BackgroundColor = Color.White
        }
        container.Controls.Add(dgvBimbingan)
    End Sub

    Private Sub LoadIdTA()
        Try
            Dim query As String = "SELECT id_ta FROM tugas_akhir WHERE nim=? ORDER BY id_ta DESC LIMIT 1"
            cmd = New OdbcCommand(query, conn)
            cmd.Parameters.AddWithValue("@nim", nim_login)
            Dim dr As OdbcDataReader = cmd.ExecuteReader()
            If dr.Read() Then
                id_ta = Convert.ToInt32(dr("id_ta"))
            Else
                id_ta = 0
            End If
            dr.Close()
        Catch ex As Exception
            MessageBox.Show("Gagal memuat ID TA: " & ex.Message)
        End Try
    End Sub

    Private Sub LoadDataBimbingan()
        Try
            dgvBimbingan.Columns.Clear()
            da = New OdbcDataAdapter("
                SELECT 
                    bab AS 'BAB',
                    nama_file AS 'File',
                    tanggal_bimbingan AS 'Tanggal',
                    status_bab AS 'Status',
                    IFNULL(catatan_dosen, '-') AS 'Catatan Dosen'
                FROM bimbingan
                WHERE id_ta='" & id_ta & "'", conn)
            ds = New DataSet
            da.Fill(ds, "bimbingan")
            dgvBimbingan.DataSource = ds.Tables("bimbingan")

            ' Ubah warna baris sesuai status
            For Each row As DataGridViewRow In dgvBimbingan.Rows
                Select Case row.Cells("Status").Value.ToString()
                    Case "Menunggu"
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 249, 196)
                    Case "Direvisi"
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 205, 210)
                    Case "Disetujui"
                        row.DefaultCellStyle.BackColor = Color.FromArgb(200, 230, 201)
                End Select
            Next
        Catch ex As Exception
            MessageBox.Show("Kesalahan memuat data bimbingan: " & ex.Message)
        End Try
    End Sub

    Private Sub btnBrowse_Click(sender As Object, e As EventArgs)
        Dim ofd As New OpenFileDialog()
        ofd.Filter = "File Dokumen (*.pdf;*.docx)|*.pdf;*.docx"
        If ofd.ShowDialog() = DialogResult.OK Then
            selectedFilePath = ofd.FileName
            txtFile.Text = Path.GetFileName(selectedFilePath)
        End If
    End Sub

    Private Sub btnUpload_Click(sender As Object, e As EventArgs)
        Try
            If selectedFilePath = "" Then
                MessageBox.Show("Pilih file terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            If id_ta = 0 Then
                MessageBox.Show("Anda belum memiliki data Tugas Akhir!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            ' === Simpan file ke folder lokal ===
            Dim folder As String = Path.Combine(Application.StartupPath, "Uploads")
            If Not Directory.Exists(folder) Then Directory.CreateDirectory(folder)

            ' Nama file unik: NIM_BAB.pdf
            Dim fileName As String = nim_login & "_" & cmbBab.Text.Replace(" ", "_") & Path.GetExtension(selectedFilePath)
            Dim destination As String = Path.Combine(folder, fileName)
            File.Copy(selectedFilePath, destination, True)

            ' === Simpan ke database ===
            Dim query As String = "
                INSERT INTO bimbingan (id_ta, tanggal_bimbingan, bab, nama_file, status_bab) 
                VALUES (?, CURDATE(), ?, ?, 'Menunggu')"
            cmd = New OdbcCommand(query, conn)
            cmd.Parameters.AddWithValue("@id_ta", id_ta)
            cmd.Parameters.AddWithValue("@bab", cmbBab.Text)
            cmd.Parameters.AddWithValue("@nama_file", fileName)
            cmd.ExecuteNonQuery()

            MessageBox.Show("File bimbingan berhasil dikirim!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
            LoadDataBimbingan()
            txtFile.Clear()

        Catch ex As Exception
            MessageBox.Show("Kesalahan saat upload: " & ex.Message)
        End Try
    End Sub
End Class
