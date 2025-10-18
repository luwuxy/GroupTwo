using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Group2
{
    public partial class GameLobby : Form
    {
        int gameProgress = 1;
        List<QuestionObject> questions;
        bool gameStarted = false;
        QuestionObject question;
        bool answered = false;

        public GameLobby()
        {
            InitializeComponent();
            LoadQuestions();
        }

        private void GameLobby_Load(object sender, EventArgs e)
        {
            // Automatically start the game when GameLobby opens
            MessageBox.Show($"Loaded {questions.Count} questions");
            gameStarted = true;
            ShowQuestion();
        }

        private void LoadQuestions()
        {
            string jsonText = File.ReadAllText("questions.json");
            questions = JsonSerializer.Deserialize<List<QuestionObject>>(jsonText);
        }

        public class QuestionObject
        {
            public int Id { get; set; }
            public string Question { get; set; }
            public List<string> Options { get; set; }
            public int Correct { get; set; }
        }

        private void ShowQuestion()
        {
            answered = false;

            // Reset colors for a clean start
            Aoption.FillColor = Color.DarkSlateGray;
            Boption.FillColor = Color.DarkSlateGray;
            Coption.FillColor = Color.DarkSlateGray;
            Doption.FillColor = Color.DarkSlateGray;

            // Get the current question
            question = questions.FirstOrDefault(q => q.Id == gameProgress);
            if (question == null) return; // no more questions

           // label49.Text = gameProgress.ToString();
            dQuestion.Text = question.Question;
            Aoption.Text = question.Options[0];
            Boption.Text = question.Options[1];
            Coption.Text = question.Options[2];
            Doption.Text = question.Options[3];
        }

        private async void ShowNextQuestion()
        {
            answered = true;
            gameProgress++;
            await Task.Delay(1500);
            ShowQuestion();
        }

        private void HandleAnswer(int optionIndex, Guna.UI2.WinForms.Guna2Button optionButton)
        {
            if (answered) return;

            if (question.Correct == optionIndex)
                optionButton.FillColor = Color.DarkGreen;
            else
                optionButton.FillColor = Color.DarkRed;

            ShowNextQuestion();
        }

        private void Boption_Click_1(object sender, EventArgs e)
        {
            HandleAnswer(1, Boption);
        }

        private void Aoption_Click_1(object sender, EventArgs e)
        {
            HandleAnswer(0, Aoption);
        }

        private void Coption_Click_1(object sender, EventArgs e)
        {
            HandleAnswer(2, Coption);
        }

        private void Doption_Click_1(object sender, EventArgs e)
        {
            HandleAnswer(3, Doption);
        }
    }
}
