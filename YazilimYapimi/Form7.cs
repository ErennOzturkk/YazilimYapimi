using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace YazilimYapimi
{
    public partial class Form7 : Form
    {
        private int loggedInUserId = 0;
        private string connectionString = "Data Source=DESKTOP-SI71SRK;Initial Catalog=YazilimYapimi;Integrated Security=True;Encrypt=True;TrustServerCertificate=True"; 
        public Form7(int userId)
        {
            InitializeComponent();
            loggedInUserId = userId;
            LoadUserData(); 
        }

        private void LoadUserData()
        {

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Words WHERE UserID = @UserID"; 
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserID", loggedInUserId);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();


                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    dataGridView1.DataSource = dt;

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Veri yüklenirken bir hata oluştu: " + ex.Message);
                }
            }
        }
    }
}


