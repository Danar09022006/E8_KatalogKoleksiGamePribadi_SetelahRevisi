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
                HitungTotalGame();
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
                if ( (txtJudul.Text.Trim().Length < 3))
                {
                    MessageBox.Show("Judul game terlalu pendek! Masukkan minimal 3 karakter.", "Validasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                    
                }
            }
        }


    