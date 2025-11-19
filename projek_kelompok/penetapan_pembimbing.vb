Imports System.Data.Odbc
Imports System.Drawing
Imports System.Windows.Forms

Public Class penetapan_pembimbing
    Inherits UserControl

    Dim cmd As OdbcCommand
    Dim da As OdbcDataAdapter
    Dim ds As DataSet

    Dim dgvTA As DataGridView
    Dim cbDosen As ComboBox
    Dim btnTetapkan As Button

    Dim selectedIdTA As Integer = -1

    Public Sub New()
        InitializeComponent()
        Me.Dock = DockStyle.Fill
        Me.BackColor = Color.FromArgb(245, 245, 248)
        Connect()
        BuildUI()
        LoadDataTA()
        LoadDosen()
    End Sub

    Private Sub BuildUI()

        ' Title
        Dim lblHeader As New Label With {
            .Text = "Penetapan Pembimbing Tugas Akhir",
            .Font = New Font("Segoe UI Semibold", 20, FontStyle.Bold),
            .ForeColor = Color.FromArgb(44, 62, 80),
            .Dock = DockStyle.Top,
            .Height = 70,
            .TextAlign = ContentAlignment.MiddleCenter
        }
        Me.Controls.Add(lblHeader)

        ' DataGridView
        dgvTA = New DataGridView With {
            .Location = New Point(30, 90),
            .Size = New Size(900, 350),
            .ReadOnly = True,
            .AllowUserToAddRows = False,
            .RowHeadersVisible = False,
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            .BackgroundColor = Color.White
        }
        AddHandler dgvTA.CellClick, AddressOf dgvTA_CellClick
        Me.Controls.Add(dgvTA)

        ' Label dosen
        Dim lblDosen As New Label With {
            .Text = "Pilih Dosen Pembimbing:",
            .Font = New Font("Segoe UI", 11, FontStyle.Bold),
            .Location = New Point(30, 470)
        }
        Me.Controls.Add(lblDosen)

        ' ComboBox dosen
        cbDosen = New ComboBox With {
            .Location = New Point(30, 500),
            .Width = 300,
            .DropDownStyle = ComboBoxStyle.DropDownList
        }
        Me.Controls.Add(cbDosen)

        ' Button tetapkan
        btnTetapkan = New Button With {
            .Text = "TETAPKAN PEMBIMBING",
            .Font = New Font("Segoe UI", 11, FontStyle.Bold),
            .Location = New Point(350, 498),
            .Size = New Size(200, 35),
            .BackColor = Color.FromArgb(52, 152, 219),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat
        }
        AddHandler btnTetapkan.Click, AddressOf btnTetapkan_Click
        Me.Controls.Add(btnTetapkan)
    End Sub

    ' ===============================
    ' LOAD DATA TA KE DATAGRID
    ' ===============================
    Private Sub LoadDataTA()
        Try
            Dim query As String =
                "SELECT 
                    ta.id_ta,
                    m.nama_mahasiswa,
                    ta.nim,
                    ta.judul_ta,
                    ta.status,
                    (SELECT nama_dosen FROM dosen d 
                     JOIN pembimbing p ON d.nidn = p.nidn 
                     WHERE p.id_ta = ta.id_ta LIMIT 1) AS pembimbing
                FROM tugas_akhir ta
                JOIN mahasiswa m ON ta.nim = m.nim
                ORDER BY ta.id_ta DESC"

            da = New OdbcDataAdapter(query, conn)
            ds = New DataSet()
            da.Fill(ds)

            dgvTA.DataSource = ds.Tables(0)

            dgvTA.Columns("id_ta").HeaderText = "ID TA"
            dgvTA.Columns("nama_mahasiswa").HeaderText = "Nama"
            dgvTA.Columns("nim").HeaderText = "NIM"
            dgvTA.Columns("judul_ta").HeaderText = "Judul TA"
            dgvTA.Columns("status").HeaderText = "Status"
            dgvTA.Columns("pembimbing").HeaderText = "Pembimbing"

            dgvTA.Columns("judul_ta").Width = 250

        Catch ex As Exception
            MessageBox.Show("Error load TA: " & ex.Message)
        End Try
    End Sub

    ' ===============================
    ' LOAD DOSEN
    ' ===============================
    Private Sub LoadDosen()
        Try
            cmd = New OdbcCommand("SELECT nidn, nama_dosen FROM dosen", conn)
            Dim rd = cmd.ExecuteReader()

            cbDosen.Items.Clear()

            While rd.Read()
                cbDosen.Items.Add(rd("nidn") & " - " & rd("nama_dosen"))
            End While

            rd.Close()
        Catch ex As Exception
            MessageBox.Show("Error load dosen: " & ex.Message)
        End Try
    End Sub

    ' ===============================
    ' GET SELECTED TA
    ' ===============================
    Private Sub dgvTA_CellClick(sender As Object, e As DataGridViewCellEventArgs)
        If e.RowIndex >= 0 Then
            selectedIdTA = dgvTA.Rows(e.RowIndex).Cells("id_ta").Value
        End If
    End Sub

    ' ===============================
    ' TETAPKAN PEMBIMBING
    ' ===============================
    Private Sub btnTetapkan_Click(sender As Object, e As EventArgs)
        If selectedIdTA = -1 Then
            MessageBox.Show("Pilih data TA terlebih dahulu.")
            Return
        End If

        If cbDosen.SelectedIndex = -1 Then
            MessageBox.Show("Pilih dosen pembimbing.")
            Return
        End If

        Dim nidn As String = cbDosen.SelectedItem.ToString().Split(" - ")(0)

        Try
            ' cek apakah sudah punya pembimbing
            cmd = New OdbcCommand("SELECT COUNT(*) FROM pembimbing WHERE id_ta = " & selectedIdTA, conn)
            Dim count As Integer = cmd.ExecuteScalar()

            If count > 0 Then
                MessageBox.Show("Mahasiswa ini sudah memiliki pembimbing.")
                Return
            End If

            ' insert pembimbing
            cmd = New OdbcCommand("
                INSERT INTO pembimbing (id_ta, nidn, peran, tanggal_tugas)
                VALUES (" & selectedIdTA & ", '" & nidn & "', 'Pembimbing Utama', CURDATE())", conn)

            cmd.ExecuteNonQuery()

            MessageBox.Show("Pembimbing berhasil ditetapkan!")

            LoadDataTA()

        Catch ex As Exception
            MessageBox.Show("Error tetapkan pembimbing: " & ex.Message)
        End Try
    End Sub

End Class
