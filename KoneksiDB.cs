using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KatalogGamePribadi
{
    internal class KoneksiDB
    {
        public SqlConnection GetConn()
        {
            // Server Name dan Catalog yang kamu berikan tadi
            string connString = @"Data Source=DANRHZQI\DANAR09; Initial Catalog=KatalogGameDB; Integrated Security=True";

            SqlConnection conn = new SqlConnection(connString);
            return conn;
        }
    }
}
