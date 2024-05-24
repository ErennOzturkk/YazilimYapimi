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
                    dataGridView1.DataSource = dt; // DataGridview'e veri yükle

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Veri yüklenirken bir hata oluştu: " + ex.Message);
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3(loggedInUserId);
            form3.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // DataGridview'deki verileri alın
            DataTable dt = (DataTable)dataGridView1.DataSource;

            // Çıktı dosyasını oluşturun
            string fileName = "WordList.txt";
            System.IO.StreamWriter file = new System.IO.StreamWriter(fileName);

            // Her satırı dosyaya yazın
            foreach (DataRow row in dt.Rows)
            {
                // Her sütundaki değeri ayrı bir satıra yazın
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    file.WriteLine(row[i].ToString());
                }
                file.WriteLine(); // Satır sonu ekleyin
            }

            // Dosyayı kapatın
            file.Close();

            // Kullanıcıya çıktı dosyasının yolunu bildirin
            MessageBox.Show("Veriler " + fileName + " dosyasına kaydedildi.", "Bilgi");
        }
    }
}