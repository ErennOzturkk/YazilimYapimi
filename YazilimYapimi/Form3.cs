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

    public partial class Form3 : Form
    {
        private int loggedInUserId = 0;
        public Form3(int userId)
        {
            InitializeComponent();
            loggedInUserId = userId;
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }



        private void button5_Click(object sender, EventArgs e)
        {
            Form8 form8 = new Form8(loggedInUserId);
            form8.Show();
            this.Hide();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string connectionString = "Data Source=DESKTOP-SI71SRK;Initial Catalog=YazilimYapimi;Integrated Security=True;Trust Server Certificate=True";
            string checkUserQuery = "SELECT LastQuizDate FROM QuizTimer WHERE UserID = @UserID";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(checkUserQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@UserID", loggedInUserId);
                        object result = cmd.ExecuteScalar();

                        if (result == null || result == DBNull.Value)
                        {
                            string insertQuery = "INSERT INTO QuizTimer (UserID, LastQuizDate) VALUES (@UserID, GETDATE())";
                            using (SqlCommand insertCmd = new SqlCommand(insertQuery, con))
                            {
                                insertCmd.Parameters.AddWithValue("@UserID", loggedInUserId);
                                insertCmd.ExecuteNonQuery();
                            }

                            Form9 form9 = new Form9(loggedInUserId);
                            form9.Show();
                            this.Hide();
                        }
                        else
                        {
                            DateTime lastQuizDate = (DateTime)result;
                            DateTime currentTime = DateTime.Now;

                            if ((currentTime - lastQuizDate).TotalHours >= 24)
                            {
                                string updateQuery = "UPDATE QuizTimer SET LastQuizDate = GETDATE() WHERE UserID = @UserID";
                                using (SqlCommand updateCmd = new SqlCommand(updateQuery, con))
                                {
                                    updateCmd.Parameters.AddWithValue("@UserID", loggedInUserId);
                                    updateCmd.ExecuteNonQuery();
                                }

                                Form9 form9 = new Form9(loggedInUserId);
                                form9.Show();
                                this.Hide();
                            }
                            else
                            {
                                MessageBox.Show("24 saat içinde quiz'e sadece bir kez girebilirsiniz.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form6 form6 = new Form6(loggedInUserId);
            form6.Show();
            this.Hide();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form7 form7 = new Form7(loggedInUserId);
            form7.Show();
            this.Hide();
        }
    }
}

