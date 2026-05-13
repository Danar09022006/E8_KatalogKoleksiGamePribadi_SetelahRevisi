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
        BindingSource bsGames = new BindingSource();

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
                if (conn.State == ConnectionState.Closed) conn.Open();

                // Memanggil VIEW, bukan tabel langsung
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM vw_KatalogGames", conn);
                dt = new DataTable();
                da.Fill(dt);

                // Menggunakan Data Binding (Syarat 4 & 5)
                bsGames.DataSource = dt;
                dgvGames.DataSource = bsGames;
                bindingNavigator1.BindingSource = bsGames; // Menghubungkan Navigator

                HitungTotalGame();
            }
            catch (Exception ex) { MessageBox.Show("Error: ⚠️Data Tidak Terbaca⚠️ " + ex.Message); }
            finally { conn.Close(); }
        }

        
        

        private void btnCreate_Click(object sender, EventArgs e)
        {
            try
            {
                if (conn.State == ConnectionState.Closed) conn.Open();

                // Memanggil Stored Procedure
                cmd = new SqlCommand("sp_InsertGame", conn);
                cmd.CommandType = CommandType.StoredProcedure; // Penting!

                cmd.Parameters.AddWithValue("@judul", txtJudul.Text.Trim());
                cmd.Parameters.AddWithValue("@platform", cbPlatform.SelectedIndex + 1);
                cmd.Parameters.AddWithValue("@user", 1);
                cmd.Parameters.AddWithValue("@genre", cbGenre.Text);
                cmd.Parameters.AddWithValue("@status", cbStatus.Text);

                int result = cmd.ExecuteNonQuery(); // Tambahkan 'int result =' di sini

                if (result > 0)
                {
                    MessageBox.Show("Data game berhasil ditambahkan!");
                    ClearForm();
                    btnRead.PerformClick();
                    HitungTotalGame();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan:" + ex.Message);
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
                

                cmd = new SqlCommand("sp_UpdateGame", conn);
                cmd.CommandType = CommandType.StoredProcedure; // Penting!  
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
                    // 1. Buka koneksi dengan aman (Cukup 1 baris ini saja)
                    if (conn.State == ConnectionState.Closed) conn.Open();

                    // 2. Langsung panggil Stored Procedure (hapus string query yang lama)
                    cmd = new SqlCommand("sp_DeleteGame", conn);
                    cmd.CommandType = CommandType.StoredProcedure; // Penting!

                    // 3. Kirimkan parameter ID
                    cmd.Parameters.AddWithValue("@id", selectedGameId);

                    // 4. Eksekusi
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Data Berhasil Dihapus!");

                    // 5. Refresh form dan tabel
                    ClearForm();
                    btnRead.PerformClick();
                    HitungTotalGame();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal Hapus: " + ex.Message);
                }
                finally
                {
                    // Pastikan koneksi selalu ditutup, baik berhasil maupun error
                    if (conn.State == ConnectionState.Open) conn.Close();
                }
            }
        }
        


            private void dgvGames_CellClick(object sender, DataGridViewCellEventArgs e)
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

                cmd = new SqlCommand("sp_SearchGame", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@keyword", txtSearch.Text); // Parameter dikirim, tapi di dalam SQL-nya digabung secara mentah

                SqlDataAdapter daSearch = new SqlDataAdapter(cmd);
                DataTable dtSearch = new DataTable();
                daSearch.Fill(dtSearch);

                bsGames.DataSource = dtSearch; // Update binding
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
                //
            }
        }

        private void btnInjection_Click(object sender, EventArgs e)
        {
            // Tambahkan peringatan agar kamu tidak tidak sengaja mengkliknya
            DialogResult dialog = MessageBox.Show(
                "PERINGATAN: Ini akan mendemonstrasikan SQL Injection yang meretas database dan mengubah semua status game menjadi 'Tamat'. Lanjutkan eksekusi?",
                "Simulasi Serangan SQL Injection",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (dialog == DialogResult.Yes)
            {
                try
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();

                    // Ini adalah Payload SQL Injection-nya
                    // Akan memotong query LIKE milik pencarian, menutupnya, lalu menyisipkan query UPDATE
                    string payload = "x%'; UPDATE Games SET status_main = 'Tamat'; --";

                    cmd = new SqlCommand("sp_SearchGame", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@keyword", payload);

                    // Kita pakai ExecuteNonQuery karena payload berisi query UPDATE yang memanipulasi data
                    cmd.ExecuteNonQuery();

                    MessageBox.Show(
                        "💥 Serangan Berhasil! Semua status game telah diubah menjadi 'Tamat' secara paksa. Lihat perubahannya di tabel.",
                        "System Hacked",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);

                    // Refresh tabel untuk melihat kerusakan yang terjadi
                    btnRead.PerformClick();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal melakukan simulasi: " + ex.Message);
                }
                finally
                {
                    if (conn.State == ConnectionState.Open) conn.Close();
                }
            }
        }

        private void btnResetData_Click(object sender, EventArgs e)
        {
            // Tambahkan konfirmasi agar tidak tidak sengaja ter-reset
            if (MessageBox.Show("Kembalikan semua data game ke kondisi semula?", "Konfirmasi Reset", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();

                    // Query: Hapus isi tabel Games, lalu isi kembali dari Games_Backup
                    // Karena id_game adalah otomatis (Identity), kita sebutkan kolomnya satu per satu
                    string query = @"
                IF OBJECT_ID('dbo.Games_Backup') IS NOT NULL
                BEGIN
                    DELETE FROM dbo.Games;
                    
                    INSERT INTO dbo.Games (judul_game, id_platform, id_user, genre, status_main)
                    SELECT judul_game, id_platform, id_user, genre, status_main 
                    FROM dbo.Games_Backup;
                END
                ELSE
                BEGIN
                    THROW 51000, 'Tabel Backup tidak ditemukan!', 1;
                END";

                    cmd = new SqlCommand(query, conn);
                    cmd.CommandType = CommandType.Text; // Gunakan Text karena ini raw query, bukan Stored Procedure
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Data berhasil direset! Kondisi database telah kembali normal.", "Reset Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Refresh tampilan tabel dan total record
                    btnRead.PerformClick();
                    HitungTotalGame();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal mereset data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    if (conn.State == ConnectionState.Open) conn.Close();
                }
            }
        }
    }

}