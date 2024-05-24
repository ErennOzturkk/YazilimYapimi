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
    public partial class Form8 : Form
    {

        public Form8()
        {
            InitializeComponent();
        }
        private int loggedInUserId = 0;
        public Form8(int userId)
        {
            InitializeComponent();
            loggedInUserId = userId;
        }

        private int NumQuestions = 0;
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                int NumQuestions = int.Parse(textBox1.Text);
                MessageBox.Show("Value assigned successfully: " + NumQuestions);

                string connectionString = "Data Source=DESKTOP-SI71SRK;Initial Catalog=YazilimYapimi;Integrated Security=True;Trust Server Certificate=True";

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string Query = "UPDATE UserInterface SET NumQuestion = @NumQuestions WHERE UserID = @UserID";

                    using (SqlCommand cmd = new SqlCommand(Query, con))
                    {
                        cmd.Parameters.AddWithValue("@NumQuestions", NumQuestions);
                        cmd.Parameters.AddWithValue("@UserID", loggedInUserId);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Question count updated successfully.");
                        }
                        else
                        {
                            MessageBox.Show("Failed to update question count. User not found.");
                        }
                    }
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Please enter a valid integer.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
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
