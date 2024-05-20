using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace YazilimYapimi
{
    public partial class Form7 : Form
    {
        private string correctEnglishWord;
        private int loggedInUserId = 0;
        public Form7(int userId)
        {
            InitializeComponent();
            loggedInUserId = userId;
        }

        private void NewQuestion()
        {
            string connectionString = "Data Source=DESKTOP-SI71SRK;Initial Catalog=YazilimYapimi;Integrated Security=True;Trust Server Certificate=True";
            string query1 = "SELECT TOP 1 Turkish, English FROM Words WHERE UserID = @UserID ORDER BY NEWID()";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    string turkishWord = "";
                    correctEnglishWord = "";

                    using (SqlCommand cmd = new SqlCommand(query1, con))
                    {
                        cmd.Parameters.AddWithValue("@UserID", loggedInUserId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                turkishWord = reader["Turkish"].ToString();
                                correctEnglishWord = reader["English"].ToString();
                                textBox1.Text = $"{turkishWord} kelimesinin anlamı nedir?";
                            }
                            else
                            {
                                MessageBox.Show("Veritabanında kelime bulunamadı.");
                                return;
                            }
                        }
                    }

                    string query2 = "SELECT TOP 3 English FROM Words WHERE Turkish != @Turkish AND UserID = @UserID ORDER BY NEWID()";

                    List<string> englishWords = new List<string>();

                    using (SqlCommand cmd = new SqlCommand(query2, con))
                    {
                        cmd.Parameters.AddWithValue("@Turkish", turkishWord);
                        cmd.Parameters.AddWithValue("@UserID", loggedInUserId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                englishWords.Add(reader["English"].ToString());
                            }
                        }
                    }

                    Random rnd = new Random();
                    int correctIndex = rnd.Next(4);
                    englishWords.Insert(correctIndex, correctEnglishWord);
                    radioButton1.Text = englishWords[0];
                    radioButton1.Tag = (englishWords[0] == correctEnglishWord);

                    radioButton2.Text = englishWords[1];
                    radioButton2.Tag = (englishWords[1] == correctEnglishWord);

                    radioButton3.Text = englishWords[2];
                    radioButton3.Tag = (englishWords[2] == correctEnglishWord);

                    radioButton4.Text = englishWords[3];
                    radioButton4.Tag = (englishWords[3] == correctEnglishWord);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
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
            Form3 form3 = new Form3(loggedInUserId);
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

            DateTime WordDate = DateTime.Now;
            string connectionString = "Data Source=DESKTOP-SI71SRK;Initial Catalog=YazilimYapimi;Integrated Security=True;Trust Server Certificate=True";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                if (isCorrect)
                {
                    MessageBox.Show("Doğru cevap!");

                    // WordDate güncelleme ve WordCounter artırma
                    string updateWordDateQuery = "UPDATE Words SET WordDate = @WordDate WHERE English = @English AND UserID = @UserID";
                    string incrementCounterQuery = "UPDATE KnowingCounter SET WordCounter = WordCounter + 1 WHERE WordID = (SELECT WordID FROM Words WHERE English = @English AND UserID = @UserID)";

                    using (SqlCommand cmd = new SqlCommand(updateWordDateQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@WordDate", WordDate);
                        cmd.Parameters.AddWithValue("@English", correctEnglishWord);
                        cmd.Parameters.AddWithValue("@UserID", loggedInUserId);
                        cmd.ExecuteNonQuery();
                    }

                    using (SqlCommand cmd = new SqlCommand(incrementCounterQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@English", correctEnglishWord);
                        cmd.Parameters.AddWithValue("@UserID", loggedInUserId);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("WordCounter updated successfully.");

                            string selectCounterQuery = "SELECT WordCounter FROM KnowingCounter WHERE WordID = (SELECT WordID FROM Words WHERE English = @English AND UserID = @UserID)";
                            using (SqlCommand counterCmd = new SqlCommand(selectCounterQuery, con))
                            {
                                counterCmd.Parameters.AddWithValue("@English", correctEnglishWord);
                                counterCmd.Parameters.AddWithValue("@UserID", loggedInUserId);
                                int counter = (int)counterCmd.ExecuteScalar();

                                if (counter >= 6)
                                {
                                    string insertKnownWordQuery = "INSERT INTO KnownWords (UserID, English) VALUES (@UserID, @English)";
                                    using (SqlCommand insertCmd = new SqlCommand(insertKnownWordQuery, con))
                                    {
                                        insertCmd.Parameters.AddWithValue("@UserID", loggedInUserId);
                                        insertCmd.Parameters.AddWithValue("@English", correctEnglishWord);
                                        int insertedRows = insertCmd.ExecuteNonQuery();

                                        if (insertedRows > 0)
                                        {
                                            MessageBox.Show("Kelime bilindiği için KnownWords tablosuna eklendi.");

                                            DateTime nextQuestionDate = WordDate.AddDays(1);
                                            string insertKnownWordYesterdayQuery = "INSERT INTO KnownWordsYesterday (UserID, English, NextQuestionDate) VALUES (@UserID, @English, @NextQuestionDate)";
                                            using (SqlCommand insertYesterdayCmd = new SqlCommand(insertKnownWordYesterdayQuery, con))
                                            {
                                                insertYesterdayCmd.Parameters.AddWithValue("@UserID", loggedInUserId);
                                                insertYesterdayCmd.Parameters.AddWithValue("@English", correctEnglishWord);
                                                insertYesterdayCmd.Parameters.AddWithValue("@NextQuestionDate", nextQuestionDate);
                                                int insertedYesterdayRows = insertYesterdayCmd.ExecuteNonQuery();

                                                if (insertedYesterdayRows > 0)
                                                {
                                                    MessageBox.Show("Bilinen kelimenin sorulma tarihi belirlendi.");
                                                }
                                                else
                                                {
                                                    MessageBox.Show("Bilinen kelimenin sorulma tarihi belirlenirken bir hata oluştu.");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show("Kelimenin KnownWords tablosuna eklenmesi sırasında bir hata oluştu.");
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Failed to update WordCounter.");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Yanlış cevap. Tekrar deneyin.");
                    string resetCounterQuery = "UPDATE KnowingCounter SET WordCounter = 0 WHERE WordID = (SELECT WordID FROM Words WHERE English = @English AND UserID = @UserID)";

                    using (SqlCommand cmd = new SqlCommand(resetCounterQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@English", correctEnglishWord);
                        cmd.Parameters.AddWithValue("@UserID", loggedInUserId);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("WordCounter reset successfully.");
                        }
                        else
                        {
                            MessageBox.Show("Failed to reset WordCounter.");
                        }
                    }
                }
            }

            NewQuestion();
        }
    }
}
