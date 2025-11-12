Imports System.Data.Odbc

Public Class data_dosen
    Dim cmd As OdbcCommand
    Dim da As OdbcDataAdapter
    Dim ds As DataSet
    Dim dr As OdbcDataReader

    Dim selectedNIDN As String = ""

    Private Sub FormDosen_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Connect()
        LoadData()
        MakeResponsive()
    End Sub


    ' =============== RESPONSIF (TAMPILAN PENUH) ===================
    Private Sub MakeResponsive()
        ' Setiap kontrol mengikuti lebar form
        For Each ctrl As Control In Me.Controls
            ctrl.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        Next

        ' Agar DataGridView otomatis menyesuaikan form penuh
        With DataGridView1
            .Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            .AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            .MultiSelect = False
            .AllowUserToResizeColumns = False
            .AllowUserToResizeRows = False
            .ReadOnly = True
            .RowHeadersVisible = False
            .DefaultCellStyle.WrapMode = DataGridViewTriState.True
            .BackgroundColor = Color.White
            .BorderStyle = BorderStyle.Fixed3D
        End With
    End Sub



    ' =============== LOAD DATA GRID ===================
    Private Sub LoadData()
        da = New OdbcDataAdapter("SELECT nidn, nama_dosen, bidang_keahlian, email FROM dosen ORDER BY nama_dosen ASC", conn)
        ds = New DataSet
        da.Fill(ds, "dosen")
        DataGridView1.DataSource = ds.Tables("dosen")

        ' Ubah judul kolom agar lebih rapi
        DataGridView1.Columns("nidn").HeaderText = "NIDN"
        DataGridView1.Columns("nama_dosen").HeaderText = "Nama Dosen"
        DataGridView1.Columns("bidang_keahlian").HeaderText = "Bidang Keahlian"
        DataGridView1.Columns("email").HeaderText = "Email"

        ' Tambahkan kolom tombol hapus (jika belum ada)
        If DataGridView1.Columns("btnHapus") Is Nothing Then
            Dim btnDelete As New DataGridViewButtonColumn()
            btnDelete.Name = "btnHapus"
            btnDelete.HeaderText = "Aksi"
            btnDelete.Text = "Hapus"
            btnDelete.UseColumnTextForButtonValue = True
            btnDelete.Width = 80
            DataGridView1.Columns.Add(btnDelete)
        End If

        ' Buat kolom memenuhi form
        DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
    End Sub



    ' =============== SIMPAN DATA ===================
    Private Sub BtnSimpan_Click(sender As Object, e As EventArgs) Handles btnSimpan.Click
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()

            ' Validasi input
            If txtnidn.Text.Trim() = "" Or txtnama.Text.Trim() = "" Then
                MessageBox.Show("NIDN dan Nama dosen tidak boleh kosong!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            If selectedNIDN = "" Then
                ' === SIMPAN BARU ===
                cmd = New OdbcCommand("INSERT INTO dosen (nidn, nama_dosen, bidang_keahlian, email) VALUES (?,?,?,?)", conn)
                cmd.Parameters.AddWithValue("@nidn", txtnidn.Text.Trim())
                cmd.Parameters.AddWithValue("@nama_dosen", txtnama.Text.Trim())
                cmd.Parameters.AddWithValue("@bidang_keahlian", txtbidang.Text.Trim())
                cmd.Parameters.AddWithValue("@email", txtemail.Text.Trim())
                cmd.ExecuteNonQuery()
                MessageBox.Show("Data dosen baru berhasil disimpan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                ' === EDIT DATA ===
                If MessageBox.Show("Apakah kamu yakin ingin mengubah data ini?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                    cmd = New OdbcCommand("UPDATE dosen SET nama_dosen=?, bidang_keahlian=?, email=? WHERE nidn=?", conn)
                    cmd.Parameters.AddWithValue("@nama_dosen", txtnama.Text.Trim())
                    cmd.Parameters.AddWithValue("@bidang_keahlian", txtbidang.Text.Trim())
                    cmd.Parameters.AddWithValue("@email", txtemail.Text.Trim())
                    cmd.Parameters.AddWithValue("@nidn", selectedNIDN.Trim())
                    cmd.ExecuteNonQuery()
                    MessageBox.Show("Data dosen berhasil diubah!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
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
            Dim nama As String = DataGridView1.Rows(e.RowIndex).Cells("nama_dosen").Value.ToString()

            If MessageBox.Show($"Yakin mau hapus data dosen '{nama}' (NIDN: {nidn})?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.Yes Then
                cmd = New OdbcCommand("DELETE FROM dosen WHERE nidn=?", conn)
                cmd.Parameters.AddWithValue("@nidn", nidn)
                cmd.ExecuteNonQuery()
                LoadData()
                MessageBox.Show("Data berhasil dihapus!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        End If
    End Sub



    ' =============== KLIK DATA GRID UNTUK EDIT ===================
    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        If e.RowIndex >= 0 Then
            selectedNIDN = DataGridView1.Rows(e.RowIndex).Cells("nidn").Value.ToString()
            txtnidn.Text = selectedNIDN
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
        selectedNIDN = ""
    End Sub

    Private Sub Panel1_Paint(sender As Object, e As PaintEventArgs) Handles Panel1.Paint

    End Sub

    Private Sub btnEdit_Click(sender As Object, e As EventArgs) 

    End Sub

    Private Sub txtemail_TextChanged(sender As Object, e As EventArgs) Handles txtemail.TextChanged

    End Sub

    Private Sub Label5_Click(sender As Object, e As EventArgs) Handles Label5.Click

    End Sub

    Private Sub txtbidang_TextChanged(sender As Object, e As EventArgs) Handles txtbidang.TextChanged

    End Sub

    Private Sub Label4_Click(sender As Object, e As EventArgs) Handles Label4.Click

    End Sub

    Private Sub txtnama_TextChanged(sender As Object, e As EventArgs) Handles txtnama.TextChanged

    End Sub

    Private Sub Label2_Click(sender As Object, e As EventArgs) Handles Label2.Click

    End Sub

    Private Sub txtnidn_TextChanged(sender As Object, e As EventArgs) Handles txtnidn.TextChanged

    End Sub

    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles Label1.Click

    End Sub

    Private Sub Label3_Click(sender As Object, e As EventArgs) Handles Label3.Click

    End Sub
End Class
