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
        List<Panel> prizePanels = new List<Panel>();
        Color defaultColor = Color.Navy;
        Color highlightColor = Color.Yellow;
        Color wrongColor = Color.Red;

        int gameProgress = 1; // Start at question 1 (fixes off-by-one issue)
        int winCount = 0;
        List<QuestionObject> questions;
        bool gameStarted = false;
        QuestionObject question;
        bool answered = false;
        Random random = new Random();
        bool gameEnded = false;

        public GameLobby()
        {
            InitializeComponent();
            LoadQuestions();

            this.FormClosed += StartGame_FormClosed;
        }

        private void GameLobby_Load(object sender, EventArgs e)
        {
            gameStarted = true;

            // Store panels in order from lowest to highest prize
            prizePanels = new List<Panel>
            {
                panel1, panel2, panel3, panel4, panel5,
                panel6, panel7, panel8, panel9, panel10,
                panel11, panel12, panel13, panel17, panel16,
                panel15, panel19, panel18, panel14, panel20
            };

            ResetPrizePanels();
            HighlightCurrentPanel(); // highlight panel 1 when game starts
            ShowQuestion();
        }

        private void StartGame_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!gameEnded)
            {
                Application.OpenForms["StartGame"]?.Close();
            }
        }

        // ===============================
        // Question Loading
        // ===============================
        private void LoadQuestions()
        {
            string jsonText = File.ReadAllText("questions.json");
            questions = JsonSerializer.Deserialize<List<QuestionObject>>(jsonText);

            // Shuffle questions
            questions = questions.OrderBy(q => random.Next()).ToList();
        }

        public class QuestionObject
        {
            public int Id { get; set; }
            public string Question { get; set; }
            public List<string> Options { get; set; }
            public int Correct { get; set; }
        }

        // ===============================
        // UI Update & Highlighting
        // ===============================
        private void ResetPrizePanels()
        {
            foreach (var p in prizePanels)
            {
                p.BackColor = defaultColor;
                p.BorderStyle = BorderStyle.None;
            }
        }

        private void HighlightCurrentPanel()
        {
            ResetPrizePanels();

            int index = gameProgress - 1;
            if (index < 0) index = 0;
            if (index >= prizePanels.Count) index = prizePanels.Count - 1;

            Panel currentPanel = prizePanels[index];
            currentPanel.BorderStyle = BorderStyle.FixedSingle;
            currentPanel.BackColor = highlightColor;
        }

        private void MarkWrongAnswerPanel()
        {
            int index = gameProgress - 1;
            if (index < 0) index = 0;
            if (index >= prizePanels.Count) index = prizePanels.Count - 1;

            Panel currentPanel = prizePanels[index];
            currentPanel.BorderStyle = BorderStyle.FixedSingle;
            currentPanel.BackColor = wrongColor;
        }

        // ===============================
        // Question Flow
        // ===============================
        private void ShowQuestion()
        {
            answered = false;

            // Reset colors and visibility
            Aoption.FillColor = Color.DarkSlateGray;
            Boption.FillColor = Color.DarkSlateGray;
            Coption.FillColor = Color.DarkSlateGray;
            Doption.FillColor = Color.DarkSlateGray;

            Aoption.Visible = true;
            Boption.Visible = true;
            Coption.Visible = true;
            Doption.Visible = true;

            if (gameProgress > 20)
            {
                EndGame(true);
                return;
            }

            question = questions[gameProgress - 1];
            dQuestion.Text = question.Question;
            Aoption.Text = question.Options[0];
            Boption.Text = question.Options[1];
            Coption.Text = question.Options[2];
            Doption.Text = question.Options[3];

            HighlightCurrentPanel(); // ensure panel syncs with question
        }

        private async void HandleAnswer(int optionIndex, Guna.UI2.WinForms.Guna2Button optionButton)
        {
            if (answered) return;
            answered = true;

            if (question.Correct == optionIndex)
            {
                optionButton.FillColor = Color.DarkGreen;
                winCount++;

                await Task.Delay(800);

                gameProgress++; // Move to next question
                ShowQuestion(); // Highlight moves immediately with new question
            }
            else
            {
                optionButton.FillColor = Color.DarkRed;

                // Highlight the correct one
                if (question.Correct == 0) Aoption.FillColor = Color.DarkGreen;
                if (question.Correct == 1) Boption.FillColor = Color.DarkGreen;
                if (question.Correct == 2) Coption.FillColor = Color.DarkGreen;
                if (question.Correct == 3) Doption.FillColor = Color.DarkGreen;

                MarkWrongAnswerPanel();

                await Task.Delay(1500);
                EndGame(false);
            }
        }

        // ===============================
        // End Game
        // ===============================
        private void EndGame(bool allCorrect)
        {
            string message;

            if (allCorrect)
            {
                message = $"🏆 Congratulations! You answered all {winCount} correctly!";
            }
            else if (winCount == 0)
            {
                message = "❌ Game over!\nYou didn’t get any question right.";
            }
            else
            {
                message = $"Game over!\nYou answered {winCount} correctly.";
            }

            MessageBox.Show(message, "Game Finished", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Reset
            gameProgress = 1;
            winCount = 0;
            answered = false;

            // Return to StartGame
            gameEnded = true;
            Application.OpenForms["StartGame"].Show();
            Application.OpenForms["StartGame"].Opacity = 1;
            this.Close();
        }

        // ===============================
        // Lifelines
        // ===============================
        private void AskAudience_Click(object sender, EventArgs e)
        {
            if (!gameStarted || question == null || answered) return;
            AskAudience_Click_Lifeline();
        }

        private void AskAudience_Click_Lifeline()
        {
            int correctIndex = question.Correct;
            int[] percentages = new int[4];
            int total = 0;

            for (int i = 0; i < 4; i++)
            {
                if (i == correctIndex)
                    percentages[i] = random.Next(40, 71);
                else
                    percentages[i] = random.Next(5, 31);
                total += percentages[i];
            }

            int adjustment = 100 - total;
            percentages[correctIndex] += adjustment;

            Aoption.Text = $"{question.Options[0]} - {percentages[0]}%";
            Boption.Text = $"{question.Options[1]} - {percentages[1]}%";
            Coption.Text = $"{question.Options[2]} - {percentages[2]}%";
            Doption.Text = $"{question.Options[3]} - {percentages[3]}%";

            AskAudience.Enabled = false;
            AskAudience.Visible = false;
        }

        private void LifeLine_Click(object sender, EventArgs e)
        {
            if (!gameStarted || question == null || answered) return;

            int correctIndex = question.Correct;
            List<int> incorrectIndices = new List<int>();
            for (int i = 0; i < 4; i++)
                if (i != correctIndex) incorrectIndices.Add(i);

            int randomWrong = incorrectIndices[random.Next(incorrectIndices.Count)];

            for (int i = 0; i < 4; i++)
            {
                if (i != correctIndex && i != randomWrong)
                {
                    if (i == 0) Aoption.Visible = false;
                    if (i == 1) Boption.Visible = false;
                    if (i == 2) Coption.Visible = false;
                    if (i == 3) Doption.Visible = false;
                }
            }

            LifeLine.Enabled = false;
            LifeLine.Visible = false;
        }

        private void PhoneFriend_Click(object sender, EventArgs e)
        {
            if (!gameStarted || question == null || answered) return;

            PhoneFriend.Enabled = false;
            PhoneFriend.Visible = false;

            int correctIndex = question.Correct;
            string correctAnswer = question.Options[correctIndex];

            MessageBox.Show($"Your friend thinks the answer might be:\n{correctAnswer}", "Phone a Friend");
        }

        // ===============================
        // Button Clicks
        // ===============================
        private void Aoption_Click_1(object sender, EventArgs e) => HandleAnswer(0, Aoption);
        private void Boption_Click_1(object sender, EventArgs e) => HandleAnswer(1, Boption);
        private void Coption_Click_1(object sender, EventArgs e) => HandleAnswer(2, Coption);
        private void Doption_Click_1(object sender, EventArgs e) => HandleAnswer(3, Doption);
    }
}
