using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Group2.Form1;

namespace Group2
{
    public partial class Form1 : Form
    {
        int gameProgress = 1;
        bool gameStarted = false;
        bool answered = false;
        List<QuestionObject> questions;
        QuestionObject question;
        Random random = new Random();
        int winCount = 0;

        public Form1()
        {
            InitializeComponent();

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

        private void ShowQuestion() // separate function to show a new question
        {
            // set answered to false to ensure that the buttons will register input.
            answered = false;

            if (gameStarted)
            {
                Aoption.FillColor = Color.DarkSlateGray;
                Boption.FillColor = Color.DarkSlateGray;
                Coption.FillColor = Color.DarkSlateGray;
                Doption.FillColor = Color.DarkSlateGray;

                // Make sure all options are visible when showing new question
                Aoption.Visible = true;
                Boption.Visible = true;
                Coption.Visible = true;
                Doption.Visible = true;

                // Reset percentage labels
                Aoption.Text = question.Options[0];
                Boption.Text = question.Options[1];
                Coption.Text = question.Options[2];
                Doption.Text = question.Options[3];
            }

            questions = questions.OrderBy(q => random.Next()).ToList();
            question = questions[gameProgress];

            label49.Text = gameProgress.ToString();
            label50.Text = question.Question;
            Aoption.Text = question.Options[0];
            Boption.Text = question.Options[1];
            Coption.Text = question.Options[2];
            Doption.Text = question.Options[3];
        }

        // this pretty much just reruns the same function above, alongside increasing gameProgress to continue the game.
        private async void ShowNextQuestion()
        {
            // set answered to true, so that the buttons won't register any inputs until the question changes.
            answered = true;
            gameProgress++;

            await Task.Delay(1500);

            // disable phone a friend panel once used.
            if (panel14.Visible == true) panel14.Visible = false;

            // this code will run if the game is finished (if gameProgress is more than 20 which is max questions).
            // feel free to put anything else needed here for after the game ends.
            if (gameProgress > 20)
            {
                label49.Text = "";
                label50.Text = $"Questions answered correctly: {winCount}";
                Aoption.Text = "";
                Boption.Text = "";
                Coption.Text = "";
                Doption.Text = "";

                Aoption.FillColor = Color.DarkSlateGray;
                Boption.FillColor = Color.DarkSlateGray;
                Coption.FillColor = Color.DarkSlateGray;
                Doption.FillColor = Color.DarkSlateGray;

                return;
            }
            ShowQuestion();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (gameStarted) return;

            ShowQuestion();
            gameStarted = true;
        }

        private void Aoption_Click(object sender, EventArgs e)
        {
            // check if the question has already been answered. if it is, simply return.
            if (answered) return;

            if (question.Correct == 0)
            {
                Aoption.FillColor = Color.DarkGreen;
                winCount++;
            }
            else
            {
                Aoption.FillColor = Color.DarkRed;
            }

            ShowNextQuestion();
        }

        private void Boption_Click(object sender, EventArgs e)
        {
            if (answered) return;

            if (question.Correct == 1)
            {
                Boption.FillColor = Color.DarkGreen;
                winCount++;
            }
            else
            {
                Boption.FillColor = Color.DarkRed;
            }

            ShowNextQuestion();
        }

        private void Coption_Click(object sender, EventArgs e)
        {
            if (answered) return;

            if (question.Correct == 2)
            {
                Coption.FillColor = Color.DarkGreen;
                winCount++;
            }
            else
            {
                Coption.FillColor = Color.DarkRed;
            }

            ShowNextQuestion();
        }

        private void Doption_Click(Object sender, EventArgs e)
        {
            if (answered) return;

            if (question.Correct == 3)
            {
                Doption.FillColor = Color.DarkGreen;
                winCount++;
            }
            else
            {
                Doption.FillColor = Color.DarkRed;
            }

            ShowNextQuestion();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            // Check if game has started and there's a current question
            if (!gameStarted || question == null || answered) return;

            // Disable the Ask the Audience button after use
            pictureBox1.Enabled = false;
            pictureBox1.Visible = false;

            // Get audience results and display them on the answer buttons
            ShowAudienceResultsOnButtons();
        }

        private void ShowAudienceResultsOnButtons()
        {
            int correctIndex = question.Correct;

            // Generate percentages that add up to 100%
            int[] percentages = new int[4];
            int total = 0;

            // Give the correct answer a higher base percentage
            for (int i = 0; i < 4; i++)
            {
                if (i == correctIndex)
                {
                    percentages[i] = random.Next(40, 71); // Correct answer gets 40-70%
                }
                else
                {
                    percentages[i] = random.Next(5, 31); // Wrong answers get 5-30%
                }
                total += percentages[i];
            }

            // Adjust to make sure total is 100%
            int adjustment = 100 - total;
            percentages[correctIndex] += adjustment;

            // Update the button texts to show percentages
            Aoption.Text = $"{question.Options[0]} - {percentages[0]}%";
            Boption.Text = $"{question.Options[1]} - {percentages[1]}%";
            Coption.Text = $"{question.Options[2]} - {percentages[2]}%";
            Doption.Text = $"{question.Options[3]} - {percentages[3]}%";
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            // Check if game has started and there's a current question
            if (!gameStarted || question == null || answered) return;

            // Disable the 50:50 button after use
            pictureBox2.Enabled = false;
            pictureBox2.Visible = false;

            // Get the two remaining options (one correct, one random wrong)
            string[] remainingOptions = GetFiftyFiftyOptions();

            // Hide options that are NOT in the remaining options
            HideNonRemainingOptions(remainingOptions);
        }

        private string[] GetFiftyFiftyOptions()
        {
            // Get the correct answer index and text
            int correctIndex = question.Correct;
            string correctAnswer = question.Options[correctIndex];

            // Create a list of incorrect answer indices
            List<int> incorrectIndices = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                if (i != correctIndex)
                {
                    incorrectIndices.Add(i);
                }
            }

            // Randomly select one incorrect answer
            int randomIncorrectIndex = incorrectIndices[random.Next(incorrectIndices.Count)];
            string randomIncorrectAnswer = question.Options[randomIncorrectIndex];

            // Return the two options as array - for example: ["B", "D"]
            return new string[] { correctAnswer, randomIncorrectAnswer };
        }

        private void HideNonRemainingOptions(string[] remainingOptions)
        {
            // Check each option and hide if it's not in the remaining options
            if (!remainingOptions.Contains(Aoption.Text))
                Aoption.Visible = false;

            if (!remainingOptions.Contains(Boption.Text))
                Boption.Visible = false;

            if (!remainingOptions.Contains(Coption.Text))
                Coption.Visible = false;

            if (!remainingOptions.Contains(Doption.Text))
                Doption.Visible = false;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            if (!gameStarted || question == null || answered) return;

            pictureBox3.Enabled = false;
            pictureBox3.Visible = false;

            int correctIndex = question.Correct;
            string correctAnswer = question.Options[correctIndex];

            panel14.Visible = true;
            label51.Text = $"Your friend thinks the answer might be:\n{correctAnswer}";
        }

        // LEFTOVER EMPTY FUNCTIONS. I'll remove these later.

        private void guna2CustomGradientPanel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void label24_Click(object sender, EventArgs e)
        {
        }

        private void guna2ShadowPanel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void label27_Click(object sender, EventArgs e)
        {
        }

        private void label26_Click(object sender, EventArgs e)
        {
        }

        private void label33_Click(object sender, EventArgs e)
        {
        }

        private void panel14_Paint(object sender, PaintEventArgs e)
        {
        }

        private void label50_Click(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void label52_Click(object sender, EventArgs e)
        {
        }

        private void label49_Click(object sender, EventArgs e)
        {
        }
    }
}