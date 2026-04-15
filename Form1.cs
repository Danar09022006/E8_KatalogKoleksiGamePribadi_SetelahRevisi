using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KatalogGamePribadi
{
    public partial class Form1 : Form
    {
        KoneksiDB koneksi = new KoneksiDB();
        SqlConnection conn;
        string selectedGameId;
        SqlCommand cmd;
        SqlDataAdapter adapter;
        DataTable dt;

        public Form1()
        {
            InitializeComponent();
            // Inisialisasi koneksi menggunakan class KoneksiDB yang sudah kamu buat sebelumnya
            conn = koneksi.GetConn();
            HitungTotalGame(); // Panggil fungsi hitung total saat form pertama kali dimuat
        }

        

        private void ConnectDatabase()
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                    MessageBox.Show("Koneksi Berhasil!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Koneksi Gagal: " + ex.Message);
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            ConnectDatabase();
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            if (conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
                MessageBox.Show("Koneksi Terputus");
            }
        }

        private void btnRead_Click(object sender, EventArgs e)
        {
            try
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();
                dgvGames.Rows.Clear();
                dgvGames.Columns.Clear();
                dgvGames.Columns.Add("id_game", "id_game");
                dgvGames.Columns.Add("id_platform", "id_platform");
                dgvGames.Columns.Add("id_user", "id_user");
                dgvGames.Columns.Add("judul_game", "judul_game");
                dgvGames.Columns.Add("genre", "genre");
                dgvGames.Columns.Add("status_main", "status_main");

                string query = "SELECT * FROM Games";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    dgvGames.Rows.Add(
                    reader["id_game"].ToString(),
                    reader["id_platform"].ToString(),
                    reader["id_user"].ToString(),
                    reader["judul_game"].ToString(),
                    reader["genre"].ToString(),
                    reader["status_main"].ToString()
                    );
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menampilkan data: " + ex);

            }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            try
            {
                if (conn.State == ConnectionState.Closed) conn.Open();

                // Pastikan kolom id_user ada di dalam query INSERT
                string query = @"INSERT INTO Games (judul_game, id_platform, id_user, genre, status_main) 
                         VALUES (@judul, @platform, @id_user, @genre, @status)";

                SqlCommand cmd = new SqlCommand(query, conn);

                // Parameter input dari form
                cmd.Parameters.AddWithValue("@judul", txtJudul.Text);
                cmd.Parameters.AddWithValue("@platform", cbPlatform.SelectedIndex + 1);
                cmd.Parameters.AddWithValue("@genre", cbGenre.Text);
                cmd.Parameters.AddWithValue("@status", cbStatus.Text);

                // INI YANG PENTING: Kamu harus memberikan nilai untuk id_user.
                // Karena di database kamu sudah input user 'danar' dengan ID 1, 
                // maka kita isi angka 1 di sini sebagai default.
                cmd.Parameters.AddWithValue("@id_user", 1);

                int result = cmd.ExecuteNonQuery();

                if (result > 0)
                {
                    MessageBox.Show("Data game berhasil ditambahkan");
                    ClearForm();
                    btnRead.PerformClick();
                    HitungTotalGame(); // <--- Panggil di sini
                    btnRead.PerformClick();// Supaya tabel langsung update
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedGameId))
            {
                MessageBox.Show("Silakan pilih data dari tabel terlebih dahulu!");
                return;
            }

            try
            {
                if (conn.State == ConnectionState.Closed) conn.Open();

                // Query diubah: WHERE menggunakan id_game
                string query = @"UPDATE Games 
                         SET judul_game=@judul, genre=@genre, id_platform=@platform, status_main=@status 
                         WHERE id_game=@id";

                cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", selectedGameId); // Menggunakan ID sebagai kunci
                cmd.Parameters.AddWithValue("@judul", txtJudul.Text); // Sekarang aman untuk diubah
                cmd.Parameters.AddWithValue("@genre", cbGenre.Text);
                cmd.Parameters.AddWithValue("@platform", cbPlatform.SelectedIndex + 1);
                cmd.Parameters.AddWithValue("@status", cbStatus.Text);

                int result = cmd.ExecuteNonQuery();

                if (result > 0)
                {
                    MessageBox.Show("Data Berhasil Diperbarui!");
                    btnRead.PerformClick();
                    ClearForm();
                    selectedGameId = "";
                    HitungTotalGame(); // <--- Panggil di sini
                    btnRead.PerformClick();// Reset ID setelah update
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal Update: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (txtJudul.Text == "")
            {
                MessageBox.Show("Pilih data yang akan dihapus!");
                return;
            }

            // Konfirmasi dulu sebelum hapus (biar tidak salah pencet)
            if (MessageBox.Show("Hapus game '" + txtJudul.Text + "'?", "Konfirmasi", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    conn = koneksi.GetConn();
                    conn.Open();

                    string query = "DELETE FROM Games WHERE judul_game=@judul";
                    cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@judul", txtJudul.Text);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Data Berhasil Dihapus!");

                    conn.Close();
                    btnRead.PerformClick();
                    ClearForm();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal Hapus: " + ex.Message);
                    HitungTotalGame(); // <--- Panggil di sini
                    btnRead.PerformClick();
                }
            }
        }

        private void dgvGames_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvGames.Rows[e.RowIndex];

                // Simpan id_game ke variabel global
                selectedGameId = row.Cells["id_game"].Value.ToString();

                // Pindahkan data ke inputan
                txtJudul.Text = row.Cells["judul_game"].Value.ToString();
                cbGenre.Text = row.Cells["genre"].Value.ToString();
                cbPlatform.Text = row.Cells["id_platform"].Value.ToString();
                cbStatus.Text = row.Cells["status_main"].Value.ToString();
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (conn.State == ConnectionState.Closed) conn.Open();

                // Gunakan JOIN agar kolom yang muncul konsisten dengan tombol READ
                // Pakai Parameter @cari untuk keamanan
                string query = @"SELECT g.id_game, g.id_platform, g.id_user, g.judul_game, g.genre, g.status_main 
                         FROM Games g 
                         WHERE g.judul_game LIKE @cari";

                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                adapter.SelectCommand.Parameters.AddWithValue("@cari", "%" + txtSearch.Text + "%");

                DataTable dt = new DataTable();
                adapter.Fill(dt);

                // Sebelum mengisi data baru, kita bersihkan DataSource yang lama
                dgvGames.DataSource = null;
                dgvGames.Columns.Clear(); // Menghapus kolom duplikat

                dgvGames.DataSource = dt;

                conn.Close();
            }
            catch (Exception ex)
            {
                // Kosongkan agar tidak muncul popup error terus-menerus saat mengetik
            }
        }

        private void ClearForm()
        {
            txtJudul.Clear();
            cbPlatform.SelectedIndex = -1;
            cbGenre.SelectedIndex = -1;
            cbStatus.SelectedIndex = -1;
        }

        private void HitungTotalGame()
        {
            try
            {
                // Pastikan koneksi terbuka
                if (conn.State == ConnectionState.Closed) conn.Open();

                // Query SQL untuk menghitung baris
                string query = "SELECT COUNT(*) FROM Games";

                SqlCommand cmd = new SqlCommand(query, conn);

                // ExecuteScalar digunakan untuk mengambil satu nilai (hasil COUNT)
                // Kita konversi hasil object ke tipe int
                int total = Convert.ToInt32(cmd.ExecuteScalar());

                // Tampilkan ke label
                lblTotalRecord.Text = "Total Koleksi Game: " + total.ToString();
            }
            catch (Exception ex)
            {
                // Jangan tampilkan MessageBox agar tidak mengganggu, cukup log jika perlu
                Console.WriteLine("Error Hitung Data: " + ex.Message);
            }
            finally
            {
                // Jangan tutup koneksi jika fungsi ini dipanggil di tengah proses lain
                // atau sesuaikan dengan alur buka-tutup koneksi kamu
            }
        }

    }

}
