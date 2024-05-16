using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace YazilimYapimi
{
    public partial class Form4 : Form
    {
        private int sifreKodu;
        private String temp;
        public string mailAdresi;


        public Form4()
        {
            InitializeComponent();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }



        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            mailAdresi = textBox1.Text;
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            mail.From = new MailAddress("erennn971@gmail.com");
            mail.To.Add(textBox1.Text);
            mail.Subject = "Şifre Sıfırlama Kodu";
            Random rnd = new Random();
            int sifreKodu = rnd.Next(100000, 999999);
            temp = sifreKodu.ToString();
            mail.Body = "Şifrenizi sıfırlamak için aşağıdaki kodu kullanınız: " + sifreKodu.ToString();
            SmtpServer.Port = 587;
            SmtpServer.Credentials = new NetworkCredential("erennn971@gmail.com", "trbd cojk axfp qhuh");
            SmtpServer.EnableSsl = true;
            SmtpServer.Send(mail);
            MessageBox.Show("Şifre sıfırlama kodu mail adresinize gönderilmiştir. Kodunuz: " + sifreKodu.ToString());


        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            String girilenKod = textBox2.Text;


            if (girilenKod == temp)
            {
                MessageBox.Show("Doğrulama başarılı! Şifrenizi sıfırlayabilirsiniz.");

                Form5 form5 = new Form5();
                form5.MailAdresi = mailAdresi;
                form5.Show();
                this.Hide();

            }
            else
            {
                MessageBox.Show("Girilen kod yanlış. Lütfen tekrar deneyin.");
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Hide();
        }
    }
}
