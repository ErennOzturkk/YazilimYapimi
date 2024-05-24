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
        private int loggedInUserId;

        public Form6()
        {
            InitializeComponent();
        }

        public Form6(int loggedInUserId)
        {
            InitializeComponent();
            this.loggedInUserId = loggedInUserId;
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
            string Sentence = textBox4.Text;
            DateTime date = DateTime.Now;
            string ConnectionString = "Data Source=DESKTOP-SI71SRK;Initial Catalog=YazilimYapimi;Integrated Security=True;Trust Server Certificate=True";

            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();

                    string Query = "INSERT INTO Words (UserID, English, Turkish, Pathway, Sentence, WordDate) VALUES (@loggedInUserId, @English, @Turkish, @Pathway, @Sentence, @date)";

                    using (SqlCommand cmd = new SqlCommand(Query, con))
                    {
                        cmd.Parameters.AddWithValue("@loggedInUserId", loggedInUserId);
                        cmd.Parameters.AddWithValue("@English", English);
                        cmd.Parameters.AddWithValue("@Turkish", Turkish);
                        cmd.Parameters.AddWithValue("@Pathway", Pathway);
                        cmd.Parameters.AddWithValue("@Sentence", Sentence);
                        cmd.Parameters.AddWithValue("@date", date);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Word inserted");
                        }
                        else
                        {
                            MessageBox.Show("No word inserted");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3(loggedInUserId);
            form3.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
