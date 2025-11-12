Imports System.Data.Odbc

Public Class data_prodi
    Dim cmd As OdbcCommand
    Dim da As OdbcDataAdapter
    Dim ds As DataSet
    Dim dr As OdbcDataReader

    Dim selectedID As String = ""

    Private Sub data_prodi_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Connect()
        LoadData()
        MakeResponsive()
    End Sub

    ' =============== RESPONSIF ===================
    Private Sub MakeResponsive()
        For Each ctrl As Control In Me.Controls
            ctrl.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        Next
        DataGridView1.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
    End Sub


    ' =============== LOAD DATA GRID ===================
    Private Sub LoadData()
        da = New OdbcDataAdapter("SELECT id_prodi, nama_prodi FROM prodi ORDER BY id_prodi ASC", conn)
        ds = New DataSet
        da.Fill(ds, "prodi")
        DataGridView1.DataSource = ds.Tables("prodi")

        ' Tambahkan tombol hapus hanya sekali
        If DataGridView1.Columns("btnHapus") Is Nothing Then
            Dim btnDelete As New DataGridViewButtonColumn()
            btnDelete.Name = "btnHapus"
            btnDelete.HeaderText = "Aksi"
            btnDelete.Text = "Hapus"
            btnDelete.UseColumnTextForButtonValue = True
            DataGridView1.Columns.Add(btnDelete)
        End If

        ' Sesuaikan lebar kolom
        DataGridView1.Columns("id_prodi").HeaderText = "ID Prodi"
        DataGridView1.Columns("nama_prodi").HeaderText = "Nama Prodi"
        DataGridView1.Columns("id_prodi").Width = 100
        DataGridView1.Columns("nama_prodi").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
    End Sub


    ' =============== SIMPAN / EDIT DATA ===================
    Private Sub BtnSimpan_Click(sender As Object, e As EventArgs) Handles btnSimpan.Click
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()

            ' Pastikan nama prodi tidak kosong
            If txtNamaProdi.Text.Trim() = "" Then
                MessageBox.Show("Nama prodi tidak boleh kosong!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            ' === Tambah data baru ===
            If selectedID = "" Then
                cmd = New OdbcCommand("INSERT INTO prodi (nama_prodi) VALUES (?)", conn)
                cmd.Parameters.AddWithValue("@nama_prodi", txtNamaProdi.Text.Trim())
                cmd.ExecuteNonQuery()
                MessageBox.Show("Data prodi baru berhasil disimpan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)

                ' === Edit data lama ===
            Else
                If MessageBox.Show("Apakah kamu yakin ingin mengubah data ini?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                    cmd = New OdbcCommand("UPDATE prodi SET nama_prodi=? WHERE id_prodi=?", conn)
                    cmd.Parameters.AddWithValue("@nama_prodi", txtNamaProdi.Text.Trim())
                    cmd.Parameters.AddWithValue("@id_prodi", selectedID)
                    cmd.ExecuteNonQuery()
                    MessageBox.Show("Data berhasil diperbarui!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End If

            ClearForm()
            LoadData()

        Catch ex As Exception
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    ' =============== HAPUS DATA ===================
    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick
        ' Jika klik tombol hapus
        If e.ColumnIndex = DataGridView1.Columns("btnHapus").Index AndAlso e.RowIndex >= 0 Then
            Dim id_prodi As String = DataGridView1.Rows(e.RowIndex).Cells("id_prodi").Value.ToString()
            Dim nama As String = DataGridView1.Rows(e.RowIndex).Cells("nama_prodi").Value.ToString()

            If MessageBox.Show("Yakin mau hapus data prodi '" & nama & "'?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.Yes Then
                cmd = New OdbcCommand("DELETE FROM prodi WHERE id_prodi=?", conn)
                cmd.Parameters.AddWithValue("@id_prodi", id_prodi)
                cmd.ExecuteNonQuery()
                LoadData()
                ClearForm()
                MessageBox.Show("Data berhasil dihapus!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        End If
    End Sub


    ' =============== KLIK DATA GRID UNTUK EDIT ===================
    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        ' Hindari error jika klik header
        If e.RowIndex >= 0 Then
            selectedID = DataGridView1.Rows(e.RowIndex).Cells("id_prodi").Value.ToString()
            txtNamaProdi.Text = DataGridView1.Rows(e.RowIndex).Cells("nama_prodi").Value.ToString()
        End If
    End Sub


    ' =============== CLEAR FORM ===================
    Private Sub ClearForm()
        txtNamaProdi.Clear()
        selectedID = ""
    End Sub
End Class
