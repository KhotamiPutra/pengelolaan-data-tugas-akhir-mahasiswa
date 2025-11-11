Imports System.Data.Odbc
Public Class data_dosen
    Dim cmd As OdbcCommand
    Dim da As OdbcDataAdapter
    Dim ds As DataSet
    Dim dr As OdbcDataReader

    Dim selectedNIM As String = ""

    Private Sub FormDosen_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Connect()
        LoadData()
        MakeResponsive()
    End Sub

    ' =============== RESPONSIF ===================
    Private Sub MakeResponsive()
        ' Agar semua komponen menyesuaikan ukuran form/panel
        For Each ctrl As Control In Me.Controls
            ctrl.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        Next
        DataGridView1.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
    End Sub



    ' =============== LOAD DATA GRID ===================
    Private Sub LoadData()
        da = New OdbcDataAdapter("SELECT nidn,nama_dosen,bidang_keahlian,email FROM dosen", conn)
        ds = New DataSet
        da.Fill(ds, "dosen")
        DataGridView1.DataSource = ds.Tables("dosen")

        ' Tambah kolom tombol hapus
        If DataGridView1.Columns("btnHapus") Is Nothing Then
            Dim btnDelete As New DataGridViewButtonColumn()
            btnDelete.Name = "btnHapus"
            btnDelete.HeaderText = "Aksi"
            btnDelete.Text = "Hapus"
            btnDelete.UseColumnTextForButtonValue = True
            DataGridView1.Columns.Add(btnDelete)
        End If
    End Sub

    ' =============== SIMPAN DATA ===================
    Private Sub BtnSimpan_Click(sender As Object, e As EventArgs) Handles btnSimpan.Click
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()

            If selectedNIM = "" Then
                ' === SIMPAN BARU ===
                cmd = New OdbcCommand("INSERT INTO dosen (nidn, nama_dosen, bidang_keahlian, email) VALUES (?,?,?,?)", conn)
                cmd.Parameters.AddWithValue("@nidn", txtnidn.Text.Trim())
                cmd.Parameters.AddWithValue("@nama_dosen", txtnama.Text)
                cmd.Parameters.AddWithValue("@bidang_keahlian", txtbidang.Text)
                cmd.Parameters.AddWithValue("@email", txtemail.Text)
                cmd.ExecuteNonQuery()
                MessageBox.Show("Data berhasil disimpan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                ' === EDIT DATA ===
                If MessageBox.Show("Apakah kamu yakin ingin mengubah data ini?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                    cmd = New OdbcCommand("UPDATE dosen SET nama_dosen=?, bidang_keahlian=?, email=? WHERE nidn=?", conn)
                    cmd.Parameters.AddWithValue("@nama_dosen", txtnama.Text)
                    cmd.Parameters.AddWithValue("@bidang_keahlian", txtbidang.Text)
                    cmd.Parameters.AddWithValue("@email", txtemail.Text)
                    cmd.Parameters.AddWithValue("@nidn", selectedNIM.Trim())
                    cmd.ExecuteNonQuery()
                    MessageBox.Show("Data berhasil diubah!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
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
        If e.ColumnIndex = DataGridView1.Columns("btnHapus").Index AndAlso e.RowIndex >= 0 Then
            Dim nidn As String = DataGridView1.Rows(e.RowIndex).Cells("nidn").Value.ToString()
            If MessageBox.Show("Yakin mau hapus data ini?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.Yes Then
                cmd = New OdbcCommand("DELETE FROM dosen WHERE nidn=?", conn)
                cmd.Parameters.AddWithValue("@nidn", nidn)
                cmd.ExecuteNonQuery()
                LoadData()
            End If
        End If
    End Sub

    ' =============== KLIK DATA GRID UNTUK EDIT ===================
    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        If e.RowIndex >= 0 Then
            selectedNIM = DataGridView1.Rows(e.RowIndex).Cells("nidn").Value.ToString()
            txtnidn.Text = selectedNIM
            txtnama.Text = DataGridView1.Rows(e.RowIndex).Cells("nama_dosen").Value.ToString()
            txtbidang.Text = DataGridView1.Rows(e.RowIndex).Cells("bidang_keahlian").Value.ToString()
            txtemail.Text = DataGridView1.Rows(e.RowIndex).Cells("email").Value.ToString()
        End If
    End Sub

    ' =============== CLEAR FORM ===================
    Private Sub ClearForm()
        txtnidn.Clear()
        txtnama.Clear()
        txtbidang.Clear()
        txtemail.Clear()
        selectedNIM = ""
    End Sub
End Class
