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

        public Form6(int userId)
        {
            InitializeComponent();
            loggedInUserId = userId;
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
            DateTime WordDate = DateTime.Now;
            string Sentence = textBox4.Text;
            string ConnectionString = "Data Source=DESKTOP-SI71SRK;Initial Catalog=YazilimYapimi;Integrated Security=True;Trust Server Certificate=True";

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();

                string insertWordQuery = @"
                INSERT INTO Words (English, Turkish, Pathway, WordDate, Sentence, UserID) 
                VALUES (@English, @Turkish, @Pathway, @WordDate, @Sentence, @UserID);
                SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(insertWordQuery, con))
                {
                    cmd.Parameters.AddWithValue("@English", English);
                    cmd.Parameters.AddWithValue("@Turkish", Turkish);
                    cmd.Parameters.AddWithValue("@Pathway", Pathway);
                    cmd.Parameters.AddWithValue("@WordDate", WordDate);
                    cmd.Parameters.AddWithValue("@Sentence", Sentence);
                    cmd.Parameters.AddWithValue("@UserID", loggedInUserId);

                    object result = cmd.ExecuteScalar();
                    int newWordID = Convert.ToInt32(result);

                    if (newWordID > 0)
                    {
                        string KnowingCounterQuery = @"
                        INSERT INTO KnowingCounter (WordID, WordCounter) 
                        VALUES (@WordID, 0)";

                        using (SqlCommand knowingCounterCmd = new SqlCommand(KnowingCounterQuery, con))
                        {
                            knowingCounterCmd.Parameters.AddWithValue("@WordID", newWordID);

                            int knowingCounterRowsAffected = knowingCounterCmd.ExecuteNonQuery();

                            if (knowingCounterRowsAffected > 0)
                            {
                                MessageBox.Show("Word added and KnowingCounter initialized to 0.");
                            }
                            else
                            {
                                MessageBox.Show("Word added but KnowingCounter initialization failed.");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("No word added.");
                    }
                }
            }
        }
    

    private void pictureBox6_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3(loggedInUserId); 
            form3.Show();
            this.Hide();
        }
    }
}

