using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace YazilimYapimi
{
    public partial class Form7 : Form
    {
        private string correctEnglishWord;
        private int loggedInUserId = 0;
        private List<string> askedQuestions = new List<string>();
        private int numQuestion = 0; // Kullanıcı tarafından belirlenen soru sayısı
        private string connectionString = "Data Source=DESKTOP-SI71SRK;Initial Catalog=YazilimYapimi;Integrated Security=True;Trust Server Certificate=True";

        public Form7(int userId)
        {
            InitializeComponent();
            loggedInUserId = userId;
            GetNumQuestionFromUserInterface();
        }

        private void GetNumQuestionFromUserInterface()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    string query = "SELECT NumQuestion FROM UserInterface WHERE UserID = @UserID";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@UserID", loggedInUserId);
                        numQuestion = (int)cmd.ExecuteScalar();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);

                }
            }
        }

        private void Form7_Load(object sender, EventArgs e)
        {
            ClearAskedQuestionsTable();
            NewQuestion();
        }

        private void NewQuestion()
        {
            // Tüm kelimelerin LevelofQuestion değeri 0 ise, numQuestion sayıda soru sorulacak
            if (CheckAllLevelsZero())
            {
                if (askedQuestions.Count >= numQuestion)
                {
                    MessageBox.Show("Soru sayısı sınırına ulaşıldı. Form kapatılıyor.");
                    this.Close();
                    return;
                }

                AskPastDueWords();
                AskAdditionalQuestions(numQuestion - askedQuestions.Count);
            }
            else
            {
                AskPastDueWords();
                AskAdditionalQuestions(numQuestion - askedQuestions.Count);
            }
            askedQuestions.Clear();

            string query1 = @"
                SELECT TOP 1 Turkish, English 
                FROM Words 
                WHERE UserID = @UserID 
                AND LevelofQuestion < 6
                AND (WordDate IS NULL OR WordDate <= GETDATE())
                AND Turkish NOT IN (SELECT Turkish FROM AskedQuestions)";

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
                                MessageBox.Show("Veritabanında sorulacak kelime bulunamadı.");
                                return;
                            }
                        }
                    }

                    string query2 = @"
                        SELECT TOP 3 English 
                        FROM Words 
                        WHERE Turkish != @Turkish 
                        AND UserID = @UserID 
                        ORDER BY NEWID()";

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
                    askedQuestions.Add(turkishWord);
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO AskedQuestions (Turkish) VALUES (@Turkish)", con))
                    {
                        cmd.Parameters.AddWithValue("@Turkish", turkishWord);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
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

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                if (isCorrect)
                {
                    MessageBox.Show("Doğru cevap!");

                    string updateWordDateQuery = @"
                        UPDATE Words 
                        SET LevelofQuestion = LevelofQuestion + 1, 
                            WordDate = CASE 
                                WHEN LevelofQuestion = 1 THEN DATEADD(day, 1, @WordDate)
                                WHEN LevelofQuestion = 2 THEN DATEADD(week, 1, @WordDate)
                                WHEN LevelofQuestion = 3 THEN DATEADD(month, 1, @WordDate)
                                WHEN LevelofQuestion = 4 THEN DATEADD(month, 3, @WordDate)
                                WHEN LevelofQuestion = 5 THEN DATEADD(month, 6, @WordDate)
                                WHEN LevelofQuestion = 6 THEN DATEADD(year, 1, @WordDate)
                                ELSE NULL END 
                        WHERE English = @English 
                        AND UserID = @UserID";

                    using (SqlCommand cmd = new SqlCommand(updateWordDateQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@WordDate", WordDate);
                        cmd.Parameters.AddWithValue("@English", correctEnglishWord);
                        cmd.Parameters.AddWithValue("@UserID", loggedInUserId);
                        cmd.ExecuteNonQuery();
                    }

                    string checkLevelQuery = @"
                        SELECT LevelofQuestion 
                        FROM Words 
                        WHERE English = @English 
                        AND UserID = @UserID";

                    using (SqlCommand cmd = new SqlCommand(checkLevelQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@English", correctEnglishWord);
                        cmd.Parameters.AddWithValue("@UserID", loggedInUserId);
                        int level = (int)cmd.ExecuteScalar();

                        if (level > 6)
                        {
                            string insertKnownWordQuery = "INSERT INTO KnownWords (UserID, English) VALUES (@UserID, @English)";
                            using (SqlCommand insertCmd = new SqlCommand(insertKnownWordQuery, con))
                            {
                                insertCmd.Parameters.AddWithValue("@UserID", loggedInUserId);
                                insertCmd.Parameters.AddWithValue("@English", correctEnglishWord);
                                insertCmd.ExecuteNonQuery();
                            }

                            string deleteWordQuery = "DELETE FROM Words WHERE English = @English AND UserID = @UserID";
                            using (SqlCommand deleteCmd = new SqlCommand(deleteWordQuery, con))
                            {
                                deleteCmd.Parameters.AddWithValue("@English", correctEnglishWord);
                                deleteCmd.Parameters.AddWithValue("@UserID", loggedInUserId);
                                deleteCmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Yanlış cevap. Tekrar deneyin.");
                    string resetCounterQuery = "UPDATE Words SET LevelofQuestion = 0 WHERE English = @English AND UserID = @UserID";
                    using (SqlCommand cmd = new SqlCommand(resetCounterQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@English", correctEnglishWord);
                        cmd.Parameters.AddWithValue("@UserID", loggedInUserId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            NewQuestion();
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

        private void ClearAskedQuestionsTable()
        {
            string clearQuery = "TRUNCATE TABLE AskedQuestions";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(clearQuery, con))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private bool CheckAllLevelsZero()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    string query = "SELECT COUNT(*) FROM Words WHERE UserID = @UserID AND LevelofQuestion > 0";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@UserID", loggedInUserId);
                        int count = (int)cmd.ExecuteScalar();

                        return count == 0;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                    return false; // Hatalı durum
                }
            }
        }

        private void AskPastDueWords()
        {
            string query = @"
                SELECT TOP 1 Turkish, English 
                FROM Words 
                WHERE UserID = @UserID 
                AND LevelofQuestion > 0
                AND (WordDate IS NOT NULL AND WordDate <= GETDATE())
                AND Turkish NOT IN (SELECT Turkish FROM AskedQuestions)
                ORDER BY WordDate ASC";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@UserID", loggedInUserId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string turkishWord = reader["Turkish"].ToString();
                                correctEnglishWord = reader["English"].ToString();
                                textBox1.Text = $"{turkishWord} kelimesinin anlamı nedir?";
                                askedQuestions.Add(turkishWord);
                                using (SqlCommand insertCmd = new SqlCommand("INSERT INTO AskedQuestions (Turkish) VALUES (@Turkish)", con))
                                {
                                    insertCmd.Parameters.AddWithValue("@Turkish", turkishWord);
                                    insertCmd.ExecuteNonQuery();
                                }
                            }
                            else
                            {

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

        private void AskAdditionalQuestions(int remainingQuestions)
        {
            for (int i = 0; i < remainingQuestions; i++)
            {
                string query = @"
                    SELECT TOP 1 Turkish, English 
                    FROM Words 
                    WHERE UserID = @UserID 
                    AND Turkish NOT IN (SELECT Turkish FROM AskedQuestions)
                    ORDER BY NEWID()";

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    try
                    {
                        con.Open();

                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("@UserID", loggedInUserId);

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    string turkishWord = reader["Turkish"].ToString();
                                    correctEnglishWord = reader["English"].ToString();
                                    textBox1.Text = $"{turkishWord} kelimesinin anlamı nedir?";

                                    askedQuestions.Add(turkishWord);

                   
                                    using (SqlCommand insertCmd = new SqlCommand("INSERT INTO AskedQuestions (Turkish) VALUES (@Turkish)", con))
                                    {
                                        insertCmd.Parameters.AddWithValue("@Turkish", turkishWord);
                                        insertCmd.ExecuteNonQuery();
                                    }
                                }
                                else
                                {

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
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
        }
    }
}
