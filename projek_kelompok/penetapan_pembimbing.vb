Imports System.Data.Odbc
Imports System.Drawing

Public Class penetapan_pembimbing
    Inherits UserControl

    Dim cmd As OdbcCommand
    Dim da As OdbcDataAdapter
    Dim ds As DataSet
    Dim selectedIdTA As Integer = 0

    Dim dgvTA As DataGridView
    Dim cmbPembimbing As ComboBox
    Dim btnSimpan As Button

    Public Sub New()
        InitializeComponent()
        Connect() ' ini panggil dari Module1
        BuildUI()
        LoadDataTA()
        LoadPembimbing()
    End Sub



    Private Sub BuildUI()
        Me.Dock = DockStyle.Fill
        Me.BackColor = Color.FromArgb(245, 247, 250)

        ' === Header ===
        Dim lblHeader As New Label With {
            .Text = "PENETAPAN DOSEN PEMBIMBING",
            .Font = New Font("Segoe UI Semibold", 18, FontStyle.Bold),
            .Dock = DockStyle.Top,
            .Height = 60,
            .TextAlign = ContentAlignment.MiddleCenter
        }
        Me.Controls.Add(lblHeader)

        ' === DataGridView ===
        dgvTA = New DataGridView With {
            .Location = New Point(40, 80),
            .Width = 900,
            .Height = 350,
            .AllowUserToAddRows = False,
            .ReadOnly = True,
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            .BackgroundColor = Color.White
        }
        AddHandler dgvTA.CellClick, AddressOf dgvTA_CellClick
        Me.Controls.Add(dgvTA)

        ' === ComboBox Pembimbing ===
        Dim lblPemb As New Label With {
            .Text = "Pilih Pembimbing:",
            .Font = New Font("Segoe UI", 11),
            .Location = New Point(40, 450)
        }
        Me.Controls.Add(lblPemb)

        cmbPembimbing = New ComboBox With {
            .Font = New Font("Segoe UI", 10),
            .Location = New Point(200, 446),
            .Width = 250
        }
        Me.Controls.Add(cmbPembimbing)

        ' === Tombol Simpan ===
        btnSimpan = New Button With {
            .Text = "Simpan Pembimbing",
            .Font = New Font("Segoe UI Semibold", 10, FontStyle.Bold),
            .Location = New Point(480, 444),
            .Width = 180,
            .Height = 50,
            .BackColor = Color.FromArgb(76, 175, 80),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat
        }
        AddHandler btnSimpan.Click, AddressOf btnSimpan_Click
        Me.Controls.Add(btnSimpan)
    End Sub

    Private Sub LoadDataTA()
        Try
            da = New OdbcDataAdapter("
            SELECT 
                t.id_ta AS 'ID',
                m.nim AS 'NIM',
                m.nama_mahasiswa AS 'Nama Mahasiswa',
                t.judul_ta AS 'Judul',
                IFNULL(d.nama_dosen, '-') AS 'Pembimbing',
                t.status AS 'Status'
            FROM tugas_akhir t
            JOIN mahasiswa m ON t.nim = m.nim
            LEFT JOIN pembimbing pb ON pb.id_ta = t.id_ta
            LEFT JOIN dosen d ON pb.nidn = d.nidn
            WHERE t.status IN ('Diajukan', 'Dibimbing')
            ORDER BY t.id_ta DESC", conn)

            ds = New DataSet
            da.Fill(ds, "tugas_akhir")
            dgvTA.DataSource = ds.Tables("tugas_akhir")
            dgvTA.Columns("ID").Visible = False
        Catch ex As Exception
            MessageBox.Show("Kesalahan memuat data TA: " & ex.Message)
        End Try
    End Sub


    Private Sub LoadPembimbing()
        Try
            da = New OdbcDataAdapter("SELECT nidn, nama_dosen FROM dosen ORDER BY nama_dosen ASC", conn)
            ds = New DataSet
            da.Fill(ds, "dosen")
            cmbPembimbing.DataSource = ds.Tables("dosen")
            cmbPembimbing.DisplayMember = "nama_dosen"
            cmbPembimbing.ValueMember = "nidn"
        Catch ex As Exception
            MessageBox.Show("Kesalahan memuat data dosen pembimbing: " & ex.Message)
        End Try
    End Sub


    Private Sub dgvTA_CellClick(sender As Object, e As DataGridViewCellEventArgs)
        If e.RowIndex >= 0 Then
            selectedIdTA = Convert.ToInt32(dgvTA.Rows(e.RowIndex).Cells("ID").Value)
        End If
    End Sub

    Private Sub btnSimpan_Click(sender As Object, e As EventArgs)
        Try
            If selectedIdTA = 0 Then
                MessageBox.Show("Pilih mahasiswa terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Dim nidn As String = cmbPembimbing.SelectedValue.ToString()

            Dim cekQuery As String = "SELECT COUNT(*) FROM pembimbing WHERE id_ta = ?"
            cmd = New OdbcCommand(cekQuery, conn)
            cmd.Parameters.AddWithValue("@id_ta", selectedIdTA)
            Dim sudahAda As Integer = Convert.ToInt32(cmd.ExecuteScalar())

            If sudahAda > 0 Then
                Dim updateQuery As String = "UPDATE pembimbing SET nidn=?, tanggal_tugas=CURDATE() WHERE id_ta=?"
                cmd = New OdbcCommand(updateQuery, conn)
                cmd.Parameters.AddWithValue("@nidn", nidn)
                cmd.Parameters.AddWithValue("@id_ta", selectedIdTA)
                cmd.ExecuteNonQuery()

                MessageBox.Show("Data pembimbing berhasil diperbarui!", "Update Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                Dim insertQuery As String = "INSERT INTO pembimbing (id_ta, nidn, peran, tanggal_tugas) VALUES (?, ?, 'Pembimbing Utama', CURDATE())"
                cmd = New OdbcCommand(insertQuery, conn)
                cmd.Parameters.AddWithValue("@id_ta", selectedIdTA)
                cmd.Parameters.AddWithValue("@nidn", nidn)
                cmd.ExecuteNonQuery()

                MessageBox.Show("Dosen pembimbing berhasil ditetapkan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If

            cmd = New OdbcCommand("UPDATE tugas_akhir SET status='Dibimbing' WHERE id_ta=?", conn)
            cmd.Parameters.AddWithValue("@id_ta", selectedIdTA)
            cmd.ExecuteNonQuery()

            LoadDataTA()
            selectedIdTA = 0

        Catch ex As Exception
            MessageBox.Show("Kesalahan saat menyimpan pembimbing: " & ex.Message)
        End Try
    End Sub


End Class
