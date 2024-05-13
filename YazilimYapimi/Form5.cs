using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YazilimYapimi
{
    public partial class Form5 : Form
    {
       
        
        public String MailAdresi
        {
            get;set;
        }
        public Form5()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string yeniSifre =textBox1.Text;
            string connectionString = "Data Source=DESKTOP-SI71SRK;Initial Catalog=YazilimYapimi;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";
            string query = "UPDATE UserInterface SET Password = @yeniSifre WHERE Mail = @MailAdresi";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@yeniSifre", yeniSifre);
                    command.Parameters.AddWithValue("@MailAdresi",MailAdresi);
                    try
                    {
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Şifre başarıyla güncellendi.");
                            Form1 form1 = new Form1();
                            form1.Show();
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Şifre güncellenemedi. Lütfen tekrar deneyin.");
    }
}
                    catch (Exception ex)
                    {
                        MessageBox.Show("Hata: " + ex.Message);
                    }
                }
            }
        }
    }

        
 }

