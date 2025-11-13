Imports System.Data.Odbc
Imports System.Drawing
Imports System.Windows.Forms

Public Class laporan
    Inherits UserControl

    Dim cmd As OdbcCommand
    Dim da As OdbcDataAdapter
    Dim ds As DataSet

    ' Controls
    Dim cmbJenisLaporan, cmbPeriode, cmbProdi As ComboBox
    Dim dtpDari, dtpSampai As DateTimePicker
    Dim btnGenerate, btnExport As Button
    Dim dgvLaporan As DataGridView
    Dim lblSummary As Label

    Public Sub New()
        InitializeComponent()
        Connect()
        BuildUI()
        LoadFilters()
    End Sub

    Private Sub BuildUI()
        Me.Dock = DockStyle.Fill
        Me.BackColor = Color.FromArgb(245, 247, 250)

        ' === HEADER ===
        Dim lblHeader As New Label With {
            .Text = "LAPORAN SISTEM TUGAS AKHIR",
            .Font = New Font("Segoe UI Semibold", 18, FontStyle.Bold),
            .Dock = DockStyle.Top,
            .Height = 60,
            .TextAlign = ContentAlignment.MiddleCenter
        }
        Me.Controls.Add(lblHeader)

        ' === FILTER PANEL ===
        Dim panelFilter As New Panel With {
            .Location = New Point(30, 80),
            .Size = New Size(900, 100),
            .BackColor = Color.White,
            .BorderStyle = BorderStyle.FixedSingle
        }

        ' Jenis Laporan
        Dim lblJenis As New Label With {.Text = "Jenis Laporan:", .Location = New Point(20, 15), .Width = 100}
        cmbJenisLaporan = New ComboBox With {
            .Location = New Point(120, 12),
            .Width = 200,
            .DropDownStyle = ComboBoxStyle.DropDownList
        }
        cmbJenisLaporan.Items.AddRange({"Progress Bimbingan", "Penilaian TA", "Mahasiswa Aktif", "Dosen Pembimbing", "Status TA"})

        ' Periode
        Dim lblPeriode As New Label With {.Text = "Periode:", .Location = New Point(20, 45), .Width = 100}
        cmbPeriode = New ComboBox With {
            .Location = New Point(120, 42),
            .Width = 150,
            .DropDownStyle = ComboBoxStyle.DropDownList
        }
        cmbPeriode.Items.AddRange({"Semua", "Hari Ini", "Minggu Ini", "Bulan Ini", "Tahun Ini", "Custom"})

        ' Tanggal Custom
        Dim lblDari As New Label With {.Text = "Dari:", .Location = New Point(350, 15), .Width = 40}
        dtpDari = New DateTimePicker With {.Location = New Point(390, 12), .Width = 120, .Format = DateTimePickerFormat.Short}

        Dim lblSampai As New Label With {.Text = "Sampai:", .Location = New Point(520, 15), .Width = 50}
        dtpSampai = New DateTimePicker With {.Location = New Point(570, 12), .Width = 120, .Format = DateTimePickerFormat.Short}

        ' Prodi Filter
        Dim lblProdi As New Label With {.Text = "Program Studi:", .Location = New Point(350, 45), .Width = 100}
        cmbProdi = New ComboBox With {.Location = New Point(450, 42), .Width = 200}

        ' Tombol
        btnGenerate = CreateButton("Generate Laporan", Color.FromArgb(52, 152, 219), New Point(700, 12))
        btnExport = CreateButton("Export Excel", Color.FromArgb(46, 204, 113), New Point(700, 42))

        AddHandler btnGenerate.Click, AddressOf btnGenerate_Click
        AddHandler btnExport.Click, AddressOf btnExport_Click

        panelFilter.Controls.AddRange({lblJenis, cmbJenisLaporan, lblPeriode, cmbPeriode,
                                     lblDari, dtpDari, lblSampai, dtpSampai,
                                     lblProdi, cmbProdi, btnGenerate, btnExport})
        Me.Controls.Add(panelFilter)

        ' === SUMMARY LABEL ===
        lblSummary = New Label With {
            .Text = "Pilih jenis laporan dan klik Generate",
            .Font = New Font("Segoe UI", 10, FontStyle.Italic),
            .Location = New Point(30, 190),
            .Size = New Size(900, 25),
            .TextAlign = ContentAlignment.MiddleLeft
        }
        Me.Controls.Add(lblSummary)

        ' === DATAGRIDVIEW ===
        dgvLaporan = New DataGridView With {
            .Location = New Point(30, 220),
            .Size = New Size(900, 350),
            .AllowUserToAddRows = False,
            .ReadOnly = True,
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            .BackgroundColor = Color.White
        }
        Me.Controls.Add(dgvLaporan)
    End Sub

    Private Function CreateButton(text As String, backColor As Color, location As Point) As Button
        Return New Button With {
            .Text = text,
            .Font = New Font("Segoe UI", 9),
            .Location = location,
            .Width = 120,
            .Height = 30,
            .BackColor = backColor,
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat
        }
    End Function

    Private Sub LoadFilters()
        ' Load data prodi
        Try
            da = New OdbcDataAdapter("SELECT id_prodi, nama_prodi FROM prodi ORDER BY nama_prodi", conn)
            ds = New DataSet
            da.Fill(ds, "prodi")
            cmbProdi.DataSource = ds.Tables("prodi")
            cmbProdi.DisplayMember = "nama_prodi"
            cmbProdi.ValueMember = "id_prodi"
            cmbProdi.SelectedIndex = 0
        Catch ex As Exception
            MessageBox.Show("Error loading prodi: " & ex.Message)
        End Try

        ' Set default values
        cmbJenisLaporan.SelectedIndex = 0
        cmbPeriode.SelectedIndex = 0
        dtpDari.Value = DateTime.Now.AddMonths(-1)
        dtpSampai.Value = DateTime.Now
    End Sub

    Private Sub btnGenerate_Click(sender As Object, e As EventArgs)
        If cmbJenisLaporan.SelectedItem Is Nothing Then
            MessageBox.Show("Pilih jenis laporan terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Try
            Dim jenisLaporan As String = cmbJenisLaporan.SelectedItem.ToString()
            Dim query As String = ""

            Select Case jenisLaporan
                Case "Progress Bimbingan"
                    query = "
                    SELECT 
                        m.nim AS 'NIM',
                        m.nama_mahasiswa AS 'Nama Mahasiswa',
                        ta.judul_ta AS 'Judul TA',
                        d.nama_dosen AS 'Pembimbing',
                        COUNT(b.id_bimbingan) AS 'Total Bimbingan',
                        SUM(CASE WHEN b.status_bab = 'Disetujui' THEN 1 ELSE 0 END) AS 'Bab Disetujui',
                        ta.status AS 'Status'
                    FROM tugas_akhir ta
                    JOIN mahasiswa m ON ta.nim = m.nim
                    LEFT JOIN pembimbing p ON ta.id_ta = p.id_ta
                    LEFT JOIN dosen d ON p.nidn = d.nidn
                    LEFT JOIN bimbingan b ON ta.id_ta = b.id_ta
                    WHERE m.prodi = " & cmbProdi.SelectedValue & "
                    GROUP BY m.nim, m.nama_mahasiswa, ta.judul_ta, d.nama_dosen, ta.status
                    ORDER BY m.nama_mahasiswa"

                Case "Penilaian TA"
                    query = "
                    SELECT 
                        m.nim AS 'NIM',
                        m.nama_mahasiswa AS 'Nama',
                        ta.judul_ta AS 'Judul TA',
                        pn.nilai_akhir AS 'Nilai Akhir',
                        d.nama_dosen AS 'Dosen Penilai',
                        pn.tanggal_nilai AS 'Tanggal Penilaian',
                        CASE 
                            WHEN pn.nilai_akhir >= 85 THEN 'A'
                            WHEN pn.nilai_akhir >= 75 THEN 'B' 
                            WHEN pn.nilai_akhir >= 65 THEN 'C'
                            WHEN pn.nilai_akhir >= 55 THEN 'D'
                            ELSE 'E'
                        END AS 'Grade'
                    FROM tugas_akhir ta
                    JOIN mahasiswa m ON ta.nim = m.nim
                    JOIN penilaian pn ON ta.id_ta = pn.id_ta
                    JOIN dosen d ON pn.nidn = d.nidn
                    WHERE ta.status = 'Selesai'
                    ORDER BY pn.nilai_akhir DESC"

                Case "Mahasiswa Aktif"
                    query = "
                    SELECT 
                        m.nim AS 'NIM',
                        m.nama_mahasiswa AS 'Nama',
                        p.nama_prodi AS 'Program Studi',
                        m.semester AS 'Semester',
                        m.email AS 'Email',
                        m.no_telp AS 'Telepon',
                        COUNT(ta.id_ta) AS 'Jumlah TA'
                    FROM mahasiswa m
                    JOIN prodi p ON m.prodi = p.id_prodi
                    LEFT JOIN tugas_akhir ta ON m.nim = ta.nim
                    WHERE m.prodi = " & cmbProdi.SelectedValue & "
                    GROUP BY m.nim, m.nama_mahasiswa, p.nama_prodi, m.semester, m.email, m.no_telp
                    ORDER BY m.nama_mahasiswa"

                Case "Dosen Pembimbing"
                    query = "
                    SELECT 
                        d.nidn AS 'NIDN',
                        d.nama_dosen AS 'Nama Dosen',
                        d.bidang_keahlian AS 'Bidang Keahlian',
                        COUNT(p.id_pembimbing) AS 'Jumlah Bimbingan',
                        AVG(pn.nilai_akhir) AS 'Rata-rata Nilai'
                    FROM dosen d
                    LEFT JOIN pembimbing p ON d.nidn = p.nidn
                    LEFT JOIN tugas_akhir ta ON p.id_ta = ta.id_ta
                    LEFT JOIN penilaian pn ON ta.id_ta = pn.id_ta
                    GROUP BY d.nidn, d.nama_dosen, d.bidang_keahlian
                    ORDER BY COUNT(p.id_pembimbing) DESC"

                Case "Status TA"
                    query = "
                    SELECT 
                        ta.status AS 'Status',
                        COUNT(*) AS 'Jumlah',
                        CONCAT(ROUND(COUNT(*) * 100.0 / (SELECT COUNT(*) FROM tugas_akhir), 2), '%') AS 'Persentase'
                    FROM tugas_akhir ta
                    GROUP BY ta.status
                    ORDER BY COUNT(*) DESC"
            End Select

            ' Apply date filter if needed
            If cmbPeriode.SelectedItem.ToString() = "Custom" Then
                If query.Contains("WHERE") Then
                    query += " AND ta.tanggal_daftar BETWEEN '" & dtpDari.Value.ToString("yyyy-MM-dd") & "' AND '" & dtpSampai.Value.ToString("yyyy-MM-dd") & "'"
                Else
                    query += " WHERE ta.tanggal_daftar BETWEEN '" & dtpDari.Value.ToString("yyyy-MM-dd") & "' AND '" & dtpSampai.Value.ToString("yyyy-MM-dd") & "'"
                End If
            End If

            da = New OdbcDataAdapter(query, conn)
            ds = New DataSet
            da.Fill(ds, "laporan")
            dgvLaporan.DataSource = ds.Tables("laporan")

            ' Update summary
            lblSummary.Text = $"Laporan {jenisLaporan} - {ds.Tables("laporan").Rows.Count} data ditemukan"

        Catch ex As Exception
            MessageBox.Show("Error generating report: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnExport_Click(sender As Object, e As EventArgs)
        If dgvLaporan.Rows.Count = 0 Then
            MessageBox.Show("Tidak ada data untuk diexport!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Try
            Dim saveDialog As New SaveFileDialog With {
                .Filter = "Excel Files|*.xlsx",
                .FileName = $"Laporan_TA_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
            }

            If saveDialog.ShowDialog() = DialogResult.OK Then
                ' Simple export to CSV (bisa dikembangkan dengan library Excel)
                Using writer As New IO.StreamWriter(saveDialog.FileName)
                    ' Write header
                    Dim headers As String = ""
                    For Each column As DataGridViewColumn In dgvLaporan.Columns
                        headers += column.HeaderText & ","
                    Next
                    writer.WriteLine(headers.TrimEnd(","))

                    ' Write data
                    For Each row As DataGridViewRow In dgvLaporan.Rows
                        If Not row.IsNewRow Then
                            Dim rowData As String = ""
                            For Each cell As DataGridViewCell In row.Cells
                                rowData += cell.Value.ToString() & ","
                            Next
                            writer.WriteLine(rowData.TrimEnd(","))
                        End If
                    Next
                End Using

                MessageBox.Show($"Data berhasil diexport ke: {saveDialog.FileName}", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If

        Catch ex As Exception
            MessageBox.Show("Error exporting data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class