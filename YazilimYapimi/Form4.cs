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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace YazilimYapimi
{
    public partial class Form4 : Form
    {
        private int sifreKodu;
        private String temp;
        public Form4()
        {
            InitializeComponent();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string mailAdresi = textBox1.Text;

            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress("erennn971@gmail.com");
                mail.To.Add(mailAdresi);
                mail.Subject = "Şifre Sıfırlama Kodu";
                Random rnd = new Random();
                sifreKodu = rnd.Next(100000, 999999);
                temp = sifreKodu.ToString(); // temp değişkeni burada atanıyor.
                mail.Body = "Şifrenizi sıfırlamak için aşağıdaki kodu kullanınız: " + sifreKodu.ToString();

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new NetworkCredential("erennn971@gmail.com");
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);

                MessageBox.Show("Şifre sıfırlama kodu mail adresinize gönderilmiştir.Kodunuz=" + sifreKodu);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Mail gönderme hatası: " + ex.Message);
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            String girilenKod = textBox2.Text;

            if (girilenKod == temp) // temp değişkeni burada doğru bir şekilde kullanılıyor.
            {
                MessageBox.Show("Doğrulama başarılı! Şifrenizi sıfırlayabilirsiniz.");
                this.Hide();
                Form5 form5 = new Form5();
                form5.Show();
            }
            else
            {
                MessageBox.Show("Girilen kod yanlış. Lütfen tekrar deneyin.");
            }
        }
    }
}
