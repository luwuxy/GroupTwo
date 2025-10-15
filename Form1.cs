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
        List<QuestionObject> questions;
        bool gameStarted = false;
        QuestionObject question;
        bool answered = false;

        public Form1()
        {
            InitializeComponent();

            string jsonText = File.ReadAllText("questions.json");
            questions = JsonSerializer.Deserialize<List<QuestionObject>>(jsonText);
        }

        // idk if I should remove these empty functions. they're not used, but I don't wanna break anything
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

        private void pictureBox2_Click(object sender, EventArgs e)
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
            }

            question = questions.FirstOrDefault(q => q.Id == gameProgress);

            label49.Text = gameProgress.ToString();
            label50.Text = question.Question;
            Aoption.Text = question.Options[0];
            Boption.Text = question.Options[1];
            Coption.Text = question.Options[2];
            Doption.Text = question.Options[3];
        }

        // this pretty much just reruns the same function above, but with a delay
        private async void ShowNextQuestion()
        {
            // set answered to true, so that the buttons won't register any inputs until the question changes.
            answered = true;
            gameProgress++;

            await Task.Delay(1500);
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
            }
            else
            {
                Doption.FillColor = Color.DarkRed;
            }

            ShowNextQuestion();
        }
    }
}
