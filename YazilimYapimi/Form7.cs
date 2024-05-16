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
    public partial class Form7 : Form
    {
        public Form7()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form7_Load(object sender, EventArgs e)
        {

            string connectionString = "Data Source=DESKTOP-SI71SRK;Initial Catalog=YazilimYapimi;Integrated Security=True;Trust Server Certificate=True";


            string query1 = "SELECT TOP 1 Turkish, English FROM Words ORDER BY NEWID()";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    string turkishWord = "";
                    string correctEnglishWord = "";


                    using (SqlCommand cmd = new SqlCommand(query1, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                turkishWord = reader["Turkish"].ToString();
                                correctEnglishWord = reader["English"].ToString();
                                textBox1.Text = turkishWord;
                                textBox1.Text = $"{turkishWord} kelimesinin anlamı nedir?";
                            }
                            else
                            {
                                MessageBox.Show("Veritabanında kelime bulunamadı.");
                                return;
                            }
                        }
                    }

                    string query2 = "SELECT TOP 3 English FROM Words WHERE Turkish != @Turkish ORDER BY NEWID()";

                    List<string> englishWords = new List<string>();

                    using (SqlCommand cmd = new SqlCommand(query2, con))
                    {
                        cmd.Parameters.AddWithValue("@Turkish", turkishWord);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                englishWords.Add(reader["English"].ToString());
                            }
                        }
                    }


                    englishWords.Add(correctEnglishWord);


                    var rnd = new Random();
                    englishWords = englishWords.OrderBy(x => rnd.Next()).ToList();


                    textBox2.Text = englishWords[0];
                    textBox3.Text = englishWords[1];
                    textBox4.Text = englishWords[2];
                    textBox5.Text = englishWords[3];
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Veritabanına bağlanırken bir hata oluştu: " + ex.Message);
                }
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
