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

        

