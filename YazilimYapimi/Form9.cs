﻿using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace YazilimYapimi
{
    public partial class Form9 : Form
    {
        private List<WordData> wordList = new List<WordData>();
        private int loggedInUserId = 0;
        private int numQuestion = 0;
        private string connectionString = "Data Source=DESKTOP-SI71SRK;Initial Catalog=YazilimYapimi;Integrated Security=True;Trust Server Certificate=True";
        private Random rnd = new Random();
        private WordData currentWord;

        public Form9(int userId)
        {
            InitializeComponent();
            loggedInUserId = userId;
            GetNumQuestionFromUserInterface();
            LoadWords();
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

        private void Form9_Load(object sender, EventArgs e)
        {
            ClearAskedQuestionsTable();
            NewQuestion();
        }

        private void LoadWords()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    // İlk olarak, LevelofQuestion > 0 olan kelimeleri al
                    string query = @"
                SELECT Turkish, English, Pathway, LevelofQuestion, WordDate 
                FROM Words 
                WHERE UserID = @UserID 
                AND LevelofQuestion > 0 
                AND (WordDate IS NOT NULL AND WordDate <= GETDATE()) 
                ORDER BY WordDate ASC";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@UserID", loggedInUserId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read() && wordList.Count < numQuestion)
                            {
                                WordData word = new WordData
                                {
                                    Turkish = reader["Turkish"].ToString(),
                                    English = reader["English"].ToString(),
                                    Pathway = reader["Pathway"].ToString(),
                                    // LevelofQuestion'ı 1'den başlatan atama:
                                    LevelofQuestion = (int)reader["LevelofQuestion"],
                                    WordDate = reader.IsDBNull(reader.GetOrdinal("WordDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("WordDate"))
                                };
                                wordList.Add(word);
                            }
                        }
                    }

                    // Eğer wordList'in kapasitesi numQuestion'dan düşükse, LevelofQuestion = 1 olan kelimeleri ekle
                    if (wordList.Count < numQuestion)
                    {
                        query = @"
                    SELECT Turkish, English, Pathway, LevelofQuestion, WordDate 
                    FROM Words 
                    WHERE UserID = @UserID 
                    AND LevelofQuestion = 1
                    ORDER BY NEWID()";

                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("@UserID", loggedInUserId);

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read() && wordList.Count < numQuestion)
                                {
                                    WordData word = new WordData
                                    {
                                        Turkish = reader["Turkish"].ToString(),
                                        English = reader["English"].ToString(),
                                        Pathway = reader["Pathway"].ToString(),
                                        // LevelofQuestion'ı 1'den başlatan atama:
                                        LevelofQuestion = (int)reader["LevelofQuestion"],
                                        WordDate = reader.IsDBNull(reader.GetOrdinal("WordDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("WordDate"))
                                    };
                                    wordList.Add(word);
                                }
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

        private void NewQuestion()
        {
            // Eğer kelime listesi boş ise, soruları durdur
            if (wordList.Count == 0)
            {
                MessageBox.Show("Tüm sorular tamamlandı. Form kapatılıyor.");
                this.Close();
                return;
            }

            int randomIndex = rnd.Next(wordList.Count);
            currentWord = wordList[randomIndex]; // Burada hata oluşuyor olabilir

            textBox1.Text = $"{currentWord.Turkish} kelimesinin anlamı nedir?";
            LoadImage(currentWord.Pathway);

            // Kelimelerin tekil olduğu ve aynı kelimeyi tekrar sorgulamadığınızdan emin olmak için bir HashSet kullanın
            HashSet<string> englishWords = new HashSet<string> { currentWord.English };

            // Şıklarda tekrar olmaması için 3 farklı kelime ekleyin
            while (englishWords.Count < 4)
            {
                int randomIndex2 = rnd.Next(wordList.Count);
                // Eğer kelime zaten `englishWords`'de değilse ekleyin
                if (!englishWords.Contains(wordList[randomIndex2].English))
                {
                    englishWords.Add(wordList[randomIndex2].English);
                }
            }

            // `englishWords` şimdi 4 farklı kelime içeriyor

            // Şıkları karıştırın
            List<string> shuffledEnglishWords = ShuffleList(englishWords.ToList());

            radioButton1.Text = shuffledEnglishWords[0];
            radioButton1.Tag = (shuffledEnglishWords[0] == currentWord.English);
            radioButton2.Text = shuffledEnglishWords[1];
            radioButton2.Tag = (shuffledEnglishWords[1] == currentWord.English);
            radioButton3.Text = shuffledEnglishWords[2];
            radioButton3.Tag = (shuffledEnglishWords[2] == currentWord.English);
            radioButton4.Text = shuffledEnglishWords[3];
            radioButton4.Tag = (shuffledEnglishWords[3] == currentWord.English);

            // Soru sorulduktan sonra kelimeyi listeden kaldır
            wordList.RemoveAt(randomIndex);
        }

        private void LoadImage(string pathway)
        {
            try
            {
                pictureBox2.ImageLocation = pathway;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Resim yüklenirken bir hata oluştu: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool isCorrect = false;
            if ((bool)radioButton1.Tag && radioButton1.Checked)
            {
                isCorrect = true;
            }
            else if ((bool)radioButton2.Tag && radioButton2.Checked)
            {
                isCorrect = true;
            }
            else if ((bool)radioButton3.Tag && radioButton3.Checked)
            {
                isCorrect = true;
            }
            else if ((bool)radioButton4.Tag && radioButton4.Checked)
            {
                isCorrect = true;
            }

            if (isCorrect)
            {
                MessageBox.Show("Doğru cevap!");

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string updateWordDateQuery = @"
UPDATE Words 
SET LevelofQuestion = LevelofQuestion + 1, 
    WordDate = CASE 
        WHEN LevelofQuestion + 1 = 2 THEN DATEADD(day, 1, GETDATE())
        WHEN LevelofQuestion + 1 = 3 THEN DATEADD(week, 1, GETDATE())
        WHEN LevelofQuestion + 1 = 4 THEN DATEADD(month, 1, GETDATE())
        WHEN LevelofQuestion + 1 = 5 THEN DATEADD(month, 3, GETDATE())
        WHEN LevelofQuestion + 1 = 6 THEN DATEADD(month, 6, GETDATE())
        WHEN LevelofQuestion + 1 = 7 THEN DATEADD(year, 1, GETDATE())
        ELSE NULL END
WHERE English = @English 
AND UserID = @UserID;";

                    using (SqlCommand cmd = new SqlCommand(updateWordDateQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@English", currentWord.English);
                        cmd.Parameters.AddWithValue("@UserID", loggedInUserId);
                        cmd.ExecuteNonQuery();
                    }

                    int level = currentWord.LevelofQuestion + 1;
                    if (level == 7) // LevelofQuestion 7'den sonra kelime bilinmiş sayılıyor
                    {
                        string insertKnownWordQuery = "INSERT INTO KnownWords (UserID, English) VALUES (@UserID, @English)";
                        using (SqlCommand insertCmd = new SqlCommand(insertKnownWordQuery, con))
                        {
                            insertCmd.Parameters.AddWithValue("@UserID", loggedInUserId);
                            insertCmd.Parameters.AddWithValue("@English", currentWord.English);
                            insertCmd.ExecuteNonQuery();
                        }

                        string deleteWordQuery = "DELETE FROM Words WHERE English = @English AND UserID = @UserID";
                        using (SqlCommand deleteCmd = new SqlCommand(deleteWordQuery, con))
                        {
                            deleteCmd.Parameters.AddWithValue("@English", currentWord.English);
                            deleteCmd.Parameters.AddWithValue("@UserID", loggedInUserId);
                            deleteCmd.ExecuteNonQuery();
                        }
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

        private List<string> ShuffleList(List<string> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = rnd.Next(i + 1);
                string temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }

            return list;
        }
    }

    public class WordData
    {
        public string Turkish { get; set; }
        public string English { get; set; }
        public string Pathway { get; set; }
        public int LevelofQuestion { get; set; }
        public DateTime? WordDate { get; set; }
    }
}