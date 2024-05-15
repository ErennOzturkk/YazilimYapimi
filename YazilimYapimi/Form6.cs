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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace YazilimYapimi
{
    public partial class Form6 : Form
    {
        public Form6()
        {
            InitializeComponent();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string Turkish = textBox1.Text;
            string English = textBox2.Text;
            string Pathway = textBox3.Text;
            DateTime currentDate = DateTime.Now;
            string Sentence = textBox4.Text;
            string ConnectionString = "Data Source=DESKTOP-SI71SRK;Initial Catalog=YazilimYapimi;Integrated Security=True;Trust Server Certificate=True";

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();

                string Query = "INSERT INTO Words (English, Turkish, Pathway, Date, Sentence) VALUES (@English, @Turkish, @Pathway, @Date,@Sentence)";

                using (SqlCommand cmd = new SqlCommand(Query, con))
                {
                    cmd.Parameters.AddWithValue("@English", English);
                    cmd.Parameters.AddWithValue("@Turkish", Turkish);
                    cmd.Parameters.AddWithValue("@Pathway", Pathway);
                    cmd.Parameters.AddWithValue("@Date", currentDate);
                    cmd.Parameters.AddWithValue("@Sentence", Sentence);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Word added");
                    }
                    else
                    {
                        MessageBox.Show("No word added");
                    }
                }
            }
        }
    }
}

