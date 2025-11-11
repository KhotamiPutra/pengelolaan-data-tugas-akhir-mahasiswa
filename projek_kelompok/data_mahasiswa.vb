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
        ' Agar semua komponen menyesuaikan ukuran form/panel
        For Each ctrl As Control In Me.Controls
            ctrl.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        Next
        DataGridView1.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
    End Sub

    ' =============== COMBOBOX PRODI ===================
    Private Sub LoadProdi()
        cmbprodi.Items.Clear()
        cmd = New OdbcCommand("SELECT * FROM prodi", conn)
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
        da = New OdbcDataAdapter("SELECT m.nim, m.nama_mahasiswa, p.nama_prodi, m.semester, m.email, m.no_telp FROM mahasiswa m JOIN prodi p ON m.prodi=p.id_prodi", conn)
        ds = New DataSet
        da.Fill(ds, "mahasiswa")
        DataGridView1.DataSource = ds.Tables("mahasiswa")

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
        If selectedNIM = "" Then
            ' Simpan baru
            Dim prodiId = DirectCast(cmbprodi.SelectedItem, Object).Value
            cmd = New OdbcCommand("INSERT INTO mahasiswa (nim, nama_mahasiswa, prodi, semester, email, no_telp) VALUES (?,?,?,?,?,?)", conn)
            cmd.Parameters.AddWithValue("@nim", txtnim.Text)
            cmd.Parameters.AddWithValue("@nama", txtnama.Text)
            cmd.Parameters.AddWithValue("@prodi", prodiId)
            cmd.Parameters.AddWithValue("@semester", txtsemester.Text)
            cmd.Parameters.AddWithValue("@email", txtemail.Text)
            cmd.Parameters.AddWithValue("@telp", txttelp.Text)
            cmd.ExecuteNonQuery()
            MessageBox.Show("Data berhasil disimpan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            ' Konfirmasi edit
            If MessageBox.Show("Apakah kamu yakin ingin mengubah data ini?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                Dim prodiId = DirectCast(cmbprodi.SelectedItem, Object).Value
                cmd = New OdbcCommand("UPDATE mahasiswa SET nama_mahasiswa=?, prodi=?, semester=?, email=?, no_telp=? WHERE nim=?", conn)
                cmd.Parameters.AddWithValue("@nama", txtnama.Text)
                cmd.Parameters.AddWithValue("@prodi", prodiId)
                cmd.Parameters.AddWithValue("@semester", txtsemester.Text)
                cmd.Parameters.AddWithValue("@email", txtemail.Text)
                cmd.Parameters.AddWithValue("@telp", txttelp.Text)
                cmd.Parameters.AddWithValue("@nim", selectedNIM)
                cmd.ExecuteNonQuery()
                MessageBox.Show("Data berhasil diubah!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        End If

        ClearForm()
        LoadData()
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

    Private Sub Panel1_Paint(sender As Object, e As PaintEventArgs) Handles Panel1.Paint

    End Sub

    Private Sub btnEdit_Click(sender As Object, e As EventArgs) Handles btnEdit.Click

    End Sub

    Private Sub txttelp_TextChanged(sender As Object, e As EventArgs) Handles txttelp.TextChanged

    End Sub

    Private Sub Label6_Click(sender As Object, e As EventArgs) Handles Label6.Click

    End Sub

    Private Sub txtemail_TextChanged(sender As Object, e As EventArgs) Handles txtemail.TextChanged

    End Sub

    Private Sub Label5_Click(sender As Object, e As EventArgs) Handles Label5.Click

    End Sub

    Private Sub txtsemester_TextChanged(sender As Object, e As EventArgs) Handles txtsemester.TextChanged

    End Sub

    Private Sub Label4_Click(sender As Object, e As EventArgs) Handles Label4.Click

    End Sub

    Private Sub cmbprodi_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbprodi.SelectedIndexChanged

    End Sub

    Private Sub Label3_Click(sender As Object, e As EventArgs) Handles Label3.Click

    End Sub

    Private Sub txtnama_TextChanged(sender As Object, e As EventArgs) Handles txtnama.TextChanged

    End Sub

    Private Sub Label2_Click(sender As Object, e As EventArgs) Handles Label2.Click

    End Sub

    Private Sub txtnim_TextChanged(sender As Object, e As EventArgs) Handles txtnim.TextChanged

    End Sub

    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles Label1.Click

    End Sub
End Class
