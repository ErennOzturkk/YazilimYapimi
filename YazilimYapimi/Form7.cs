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
        private string correctEnglishWord;
        public Form7()
        {
            InitializeComponent();
        }
        private void NewQuestion()
        {
            string connectionString = "Data Source=DESKTOP-SI71SRK;Initial Catalog=YazilimYapimi;Integrated Security=True;Trust Server Certificate=True";


            string query1 = "SELECT TOP 1 Turkish, English FROM Words ORDER BY NEWID()";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    string turkishWord = "";
                    correctEnglishWord = "";


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

                    radioButton1.Tag = englishWords[0] == correctEnglishWord;
                    radioButton2.Tag = englishWords[1] == correctEnglishWord;
                    radioButton3.Tag = englishWords[2] == correctEnglishWord;
                    radioButton4.Tag = englishWords[3] == correctEnglishWord;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Veritabanına bağlanırken bir hata oluştu: " + ex.Message);
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form7_Load(object sender, EventArgs e)
        {
        NewQuestion();
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

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3();
            form3.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            bool isCorrect = false;
            if (radioButton1.Checked && (bool)radioButton1.Tag)
                isCorrect = true;
            else if (radioButton2.Checked && (bool)radioButton2.Tag)
                isCorrect = true;
            else if (radioButton3.Checked && (bool)radioButton3.Tag)
                isCorrect = true;
            else if (radioButton4.Checked && (bool)radioButton4.Tag)
                isCorrect = true;

            if (isCorrect)
            {
                DateTime WordDate = DateTime.Now;
                MessageBox.Show("Doğru cevap!");
                string connectionString = "Data Source=DESKTOP-SI71SRK;Initial Catalog=YazilimYapimi;Integrated Security=True;Trust Server Certificate=True";

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "UPDATE Words SET WordDate = @WordDate WHERE English = @English";
                    string query2 = "SELECT WordDate FROM Words WHERE English = @English";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@WordDate", WordDate);
                        cmd.Parameters.AddWithValue("@English", correctEnglishWord);
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            else
                MessageBox.Show("Yanlış cevap. Tekrar deneyin.");
            NewQuestion();
        }
    }
}
