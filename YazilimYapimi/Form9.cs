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
    public partial class Form9 : Form
    {
        private int loggedInUserId;

        public Form9()
        {
            InitializeComponent();
        }

        public Form9(int loggedInUserId)
        {
            this.loggedInUserId = loggedInUserId;
        }

        private void Form9_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string resimYolu = @"C:\Users\EREN\Downloads\indir.jpg";
            pictureBox2.Image = Image.FromFile(resimYolu);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }
    }
}
