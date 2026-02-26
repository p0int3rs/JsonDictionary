using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using JsonDictionary.Model;

namespace JsonDictionary
{
    public partial class GameWindow : Window
    {
        private Dictionary<string, Word> _words;
        private static readonly Random _random = new Random();

        private string _currentCorrectAnswer;
        private int _score = 0;
        private int _questionIndex = 0;
        private int _totalQuestionCount = 10;

        private List<string> _unusedWords;

        public GameWindow(Dictionary<string, Word> words)
        {
            InitializeComponent();

            _words = words;
            _unusedWords = _words.Keys.ToList();

            StartGame();
        }

        private void StartGame()
        {
            _score = 0;
            _questionIndex = 0;
            _unusedWords = _words.Keys.ToList();

            NextQuestion();
        }

        private void NextQuestion()
        {
            if (_questionIndex >= _totalQuestionCount || _unusedWords.Count == 0)
            {
                MessageBox.Show($"Game Over!\nScore: {_score}");
                Close();
                return;
            }

            _questionIndex++;

            var questionData = GenerateQuestion();

            QuestionWordText.Text = questionData.questionWord;

            AnswerButton1.Content = questionData.answers[0];
            AnswerButton2.Content = questionData.answers[1];
            AnswerButton3.Content = questionData.answers[2];
            AnswerButton4.Content = questionData.answers[3];

            _currentCorrectAnswer = questionData.correctAnswer;

            ScoreText.Text = $"Score: {_score}";
            QuestionCounterText.Text = $"Question {_questionIndex} / {_totalQuestionCount}";
        }

        private (string questionWord, List<string> answers, string correctAnswer) GenerateQuestion()
        {
            // Doğru kelime seç
            string correctWord = _unusedWords[_random.Next(_unusedWords.Count)];
            _unusedWords.Remove(correctWord);

            Word correctWordObj = _words[correctWord];

            // Doğru meaning
            string correctMeaning = correctWordObj.GetRandomMeaning();

            HashSet<string> answers = new HashSet<string>();
            answers.Add(correctMeaning);

            var allKeys = _words.Keys.ToList();

            // 3 yanlış meaning üret
            while (answers.Count < 4)
            {
                string randomWord = allKeys[_random.Next(allKeys.Count)];

                if (randomWord == correctWord)
                    continue;

                string randomMeaning = _words[randomWord].GetRandomMeaning();

                if (!string.IsNullOrWhiteSpace(randomMeaning))
                    answers.Add(randomMeaning);
            }

            var shuffled = answers.OrderBy(x => _random.Next()).ToList();

            return (correctWord, shuffled, correctMeaning);
        }

        private async void Answer_Click(object sender, RoutedEventArgs e)
        {
            Button clicked = sender as Button;
            string selectedAnswer = clicked.Content.ToString();

            DisableAnswerButtons();

            if (selectedAnswer == _currentCorrectAnswer)
            {
                _score++;
                clicked.Background = new SolidColorBrush(Color.FromRgb(46, 125, 50)); // yeşil
            }
            else
            {
                clicked.Background = new SolidColorBrush(Color.FromRgb(198, 40, 40)); // kırmızı
                HighlightCorrectAnswer();
            }

            await Task.Delay(1000);

            ResetButtonColors();
            EnableAnswerButtons();
            NextQuestion();
        }

        private void HighlightCorrectAnswer()
        {
            foreach (var btn in new[] { AnswerButton1, AnswerButton2, AnswerButton3, AnswerButton4 })
            {
                if (btn.Content.ToString() == _currentCorrectAnswer)
                {
                    btn.Background = new SolidColorBrush(Color.FromRgb(46, 125, 50));
                }
            }
        }

        private void DisableAnswerButtons()
        {
            AnswerButton1.IsEnabled = false;
            AnswerButton2.IsEnabled = false;
            AnswerButton3.IsEnabled = false;
            AnswerButton4.IsEnabled = false;
        }

        private void EnableAnswerButtons()
        {
            AnswerButton1.IsEnabled = true;
            AnswerButton2.IsEnabled = true;
            AnswerButton3.IsEnabled = true;
            AnswerButton4.IsEnabled = true;
        }

        private void ResetButtonColors()
        {
            Brush defaultBrush = new SolidColorBrush(Color.FromRgb(45, 45, 48));

            AnswerButton1.Background = defaultBrush;
            AnswerButton2.Background = defaultBrush;
            AnswerButton3.Background = defaultBrush;
            AnswerButton4.Background = defaultBrush;
        }
    }
}