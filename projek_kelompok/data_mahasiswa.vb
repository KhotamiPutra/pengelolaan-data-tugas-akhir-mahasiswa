Imports System.Data.Odbc

Public Class data_mahasiswa
    Dim cmd As OdbcCommand
    Dim da As OdbcDataAdapter
    Dim ds As DataSet
    Dim dr As OdbcDataReader

    Dim selectedNIM As String = ""

    Private Sub FormMahasiswa_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Connect()
        LoadProdi()
        LoadData()
        MakeResponsive()
    End Sub

    ' =============== RESPONSIF ===================
    Private Sub MakeResponsive()
        ' Setiap kontrol menyesuaikan ukuran form
        For Each ctrl As Control In Me.Controls
            ctrl.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        Next

        ' DataGridView menyesuaikan penuh layar (atas-bawah, kiri-kanan)
        DataGridView1.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right

        ' Pengaturan tambahan agar grid menyesuaikan isi layar
        DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        DataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
        DataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        DataGridView1.MultiSelect = False
        DataGridView1.AllowUserToResizeColumns = False
        DataGridView1.AllowUserToResizeRows = False
        DataGridView1.ReadOnly = True
        DataGridView1.RowHeadersVisible = False
        DataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True
    End Sub


    ' =============== COMBOBOX PRODI ===================
    Private Sub LoadProdi()
        cmbprodi.Items.Clear()
        cmd = New OdbcCommand("SELECT * FROM prodi ORDER BY nama_prodi ASC", conn)
        dr = cmd.ExecuteReader()
        While dr.Read()
            cmbprodi.Items.Add(New With {.Text = dr("nama_prodi").ToString(), .Value = dr("id_prodi")})
        End While
        cmbprodi.DisplayMember = "Text"
        cmbprodi.ValueMember = "Value"
        dr.Close()
    End Sub


    ' =============== LOAD DATA GRID ===================
    Private Sub LoadData()
        da = New OdbcDataAdapter("
            SELECT m.nim, 
                   m.nama_mahasiswa, 
                   p.nama_prodi, 
                   m.semester, 
                   m.email, 
                   m.no_telp 
            FROM mahasiswa m 
            JOIN prodi p ON m.prodi = p.id_prodi
            ORDER BY m.nim ASC", conn)

        ds = New DataSet
        da.Fill(ds, "mahasiswa")
        DataGridView1.DataSource = ds.Tables("mahasiswa")

        ' Ubah header kolom agar lebih rapi
        DataGridView1.Columns("nim").HeaderText = "NIM"
        DataGridView1.Columns("nama_mahasiswa").HeaderText = "Nama Mahasiswa"
        DataGridView1.Columns("nama_prodi").HeaderText = "Program Studi"
        DataGridView1.Columns("semester").HeaderText = "Semester"
        DataGridView1.Columns("email").HeaderText = "Email"
        DataGridView1.Columns("no_telp").HeaderText = "No. Telepon"

        ' Jika kolom tombol hapus belum ada, tambahkan
        If DataGridView1.Columns("btnHapus") Is Nothing Then
            Dim btnDelete As New DataGridViewButtonColumn()
            btnDelete.Name = "btnHapus"
            btnDelete.HeaderText = "Aksi"
            btnDelete.Text = "Hapus"
            btnDelete.UseColumnTextForButtonValue = True
            btnDelete.Width = 80
            DataGridView1.Columns.Add(btnDelete)
        End If

        ' Agar tabel langsung menyesuaikan layar
        DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
    End Sub


    ' =============== SIMPAN / EDIT DATA ===================
    Private Sub BtnSimpan_Click(sender As Object, e As EventArgs) Handles btnSimpan.Click
        Try
            If txtnim.Text.Trim() = "" Or txtnama.Text.Trim() = "" Then
                MessageBox.Show("NIM dan Nama tidak boleh kosong!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Dim prodiId = DirectCast(cmbprodi.SelectedItem, Object).Value

            If selectedNIM = "" Then
                ' === SIMPAN BARU ===
                cmd = New OdbcCommand("INSERT INTO mahasiswa (nim, nama_mahasiswa, prodi, semester, email, no_telp) VALUES (?,?,?,?,?,?)", conn)
                cmd.Parameters.AddWithValue("@nim", txtnim.Text)
                cmd.Parameters.AddWithValue("@nama", txtnama.Text)
                cmd.Parameters.AddWithValue("@prodi", prodiId)
                cmd.Parameters.AddWithValue("@semester", txtsemester.Text)
                cmd.Parameters.AddWithValue("@email", txtemail.Text)
                cmd.Parameters.AddWithValue("@telp", txttelp.Text)
                cmd.ExecuteNonQuery()
                MessageBox.Show("Data mahasiswa berhasil disimpan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                ' === EDIT DATA ===
                If MessageBox.Show("Apakah kamu yakin ingin mengubah data ini?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                    cmd = New OdbcCommand("UPDATE mahasiswa SET nama_mahasiswa=?, prodi=?, semester=?, email=?, no_telp=? WHERE nim=?", conn)
                    cmd.Parameters.AddWithValue("@nama", txtnama.Text)
                    cmd.Parameters.AddWithValue("@prodi", prodiId)
                    cmd.Parameters.AddWithValue("@semester", txtsemester.Text)
                    cmd.Parameters.AddWithValue("@email", txtemail.Text)
                    cmd.Parameters.AddWithValue("@telp", txttelp.Text)
                    cmd.Parameters.AddWithValue("@nim", selectedNIM)
                    cmd.ExecuteNonQuery()
                    MessageBox.Show("Data mahasiswa berhasil diubah!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
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
            Dim nim As String = DataGridView1.Rows(e.RowIndex).Cells("nim").Value.ToString()
            If MessageBox.Show("Yakin mau hapus data ini?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.Yes Then
                cmd = New OdbcCommand("DELETE FROM mahasiswa WHERE nim=?", conn)
                cmd.Parameters.AddWithValue("@nim", nim)
                cmd.ExecuteNonQuery()
                LoadData()
                MessageBox.Show("Data berhasil dihapus!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        End If
    End Sub


    ' =============== KLIK DATA GRID UNTUK EDIT ===================
    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        If e.RowIndex >= 0 Then
            selectedNIM = DataGridView1.Rows(e.RowIndex).Cells("nim").Value.ToString()
            txtnim.Text = selectedNIM
            txtnama.Text = DataGridView1.Rows(e.RowIndex).Cells("nama_mahasiswa").Value.ToString()
            txtsemester.Text = DataGridView1.Rows(e.RowIndex).Cells("semester").Value.ToString()
            txtemail.Text = DataGridView1.Rows(e.RowIndex).Cells("email").Value.ToString()
            txttelp.Text = DataGridView1.Rows(e.RowIndex).Cells("no_telp").Value.ToString()
            cmbprodi.Text = DataGridView1.Rows(e.RowIndex).Cells("nama_prodi").Value.ToString()
        End If
    End Sub


    ' =============== CLEAR FORM ===================
    Private Sub ClearForm()
        txtnim.Clear()
        txtnama.Clear()
        txtsemester.Clear()
        txtemail.Clear()
        txttelp.Clear()
        cmbprodi.SelectedIndex = -1
        selectedNIM = ""
    End Sub
End Class
