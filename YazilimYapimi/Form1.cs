using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace YazilimYapimi
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }
        private void Form1_Load(object sender, EventArgs e)
        {


        }
        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }
        private int loggedInUserId=0;
        private void button1_Click(object sender, EventArgs e)
        {
            string Mail = textBox1.Text;
            string Password = textBox2.Text;

            string ConnectionString = "Data Source=DESKTOP-SI71SRK;Initial Catalog=YazilimYapimi;Integrated Security=True;Trust Server Certificate=True";
            SqlConnection con = new SqlConnection(ConnectionString);

            con.Open();

            string Query = "SELECT UserID FROM UserInterface WHERE (Mail = @Mail OR Username = @Mail) AND Password = @Password";
            SqlCommand cmd = new SqlCommand(Query, con);
            cmd.Parameters.AddWithValue("@Mail", Mail);
            cmd.Parameters.AddWithValue("@Password", Password);

            object result = cmd.ExecuteScalar();
            con.Close();

            if (result != null)
            {
                int userId = (int)result;
                MessageBox.Show("Giriþ baþarýlý!");
                Form3 form3 = new Form3(userId);
                form3.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Giriþ baþarýsýz. Lütfen mail ve þifrenizi kontrol edin.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form2 form2 = new Form2();
            form2.Show();
        }

        private void textBox1_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
        private void textBox2_Enter(object sender, EventArgs e)
        {
            if (textBox2.Text == "Password")
            {
                textBox2.Text = "";
                textBox2.ForeColor = Color.Black;
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                textBox1.Text = "Username";
                textBox1.ForeColor = Color.Silver;
            }
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                textBox2.Text = "Password";
                textBox2.ForeColor = Color.Silver;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text == "Username")
            {
                textBox1.Text = "";
                textBox1.ForeColor = Color.Black;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form4 form4 = new Form4();
            form4.Show();
        }
    }
}
