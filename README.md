**Skenario Serangan (Tombol Demo SQL Injection):**
Untuk mempermudah pengujian, telah ditambahkan tombol khusus `btnInjection` pada antarmuka aplikasi. Tombol ini mensimulasikan seorang peretas yang memasukkan payload berbahaya ke dalam parameter pencarian.

**Payload yang digunakan:**
`x%'; UPDATE Games SET status_main = 'Tamat'; --`

**Alur Eksekusi:**
1. Tombol akan mengirimkan payload tersebut ke parameter `@keyword`.
2. Di dalam SQL Server, Dynamic SQL akan merangkai string menjadi:
   `SELECT * FROM vw_KatalogGames WHERE judul_game LIKE '%x%'; UPDATE Games SET status_main = 'Tamat'; --%'`
3. Tanda titik koma (`;`) akan menutup perintah Select, dan SQL Server akan mengeksekusi perintah lanjutan yaitu `UPDATE`. 
4. Tanda `--` mengabaikan sisa string di belakangnya agar tidak terjadi error sintaks.
5. Hasil akhirnya: Seluruh baris di tabel `Games` akan dimanipulasi secara paksa sehingga kolom `status_main` berubah menjadi 'Tamat', mendemonstrasikan kerentanan yang fatal pada integritas data.
