using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace mastermind_1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timer = new DispatcherTimer();
        int timeRemaining = 10; 
        int attempts = 1;
        string[] chosenColor = new string[4];
        string[] allColors = { "white", "green", "blue", "red", "orange", "yellow" };
        
        public MainWindow()
        {
            
            InitializeComponent();
            this.Closing += Mastermind_Closing;
            this.KeyDown += (s, e) =>
            {
                if (e.Key == Key.F12 && Keyboard.Modifiers == ModifierKeys.Control)
                {
                    ToggleDebug();
                }
            };

            timer.Tick += Timer_Tick;
                timer.Interval = new TimeSpan(0, 0, 1);

            GenerateCode();



            codeTextBox.Text = $"{string.Join(", ", chosenColor)}";
            FillComboBoxes(ref allColors);

        }
       
        private void Mastermind_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var result = MessageBox.Show("Wilt u het spel vroegtijdig beeindigen?", $"poging {attempts}/10", MessageBoxButton.YesNo, MessageBoxImage.Warning);
       if(result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        
        }
        private bool IsCodeCracked() 
        {
            string[] userPickedColors =
            {
                firstComboBox.SelectedItem?.ToString(),
                 secondComboBox.SelectedItem?.ToString(),
                 thirdComboBox.SelectedItem?.ToString(),
                 fourthComboBox.SelectedItem?.ToString()
            };

            return userPickedColors.SequenceEqual(chosenColor);
             }
        private void AskToPlayAgain() {
            var result = MessageBox.Show($"You failed! De correcte code was {string.Join(", ", chosenColor)}. Nog eens proberen?", "Failed", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                ResetGame();
            }
            else
            {
                Application.Current.Shutdown();
            }
        }
        private void EnableInputs()
        {
            firstComboBox.IsEnabled = true;
            secondComboBox.IsEnabled = true;
            thirdComboBox.IsEnabled = true;
            fourthComboBox.IsEnabled = true;
            controlButton.IsEnabled = true;
        }
      private void ResetGame() {

            GenerateCode();
            historyListBox.Items.Clear();

            scoreLabel.Content = $"je score: 0";
            attempts = 1;
            Mastermind.Title = $"attempts: {attempts}";

            EnableInputs();
                }
        private int CalculateScore(string[] userColors)
        {
            int score = 0;
            Dictionary<string, int> colorCounts = new Dictionary<string, int>();

            foreach (var color in chosenColor)
            {
                if (colorCounts.ContainsKey(color))
                {
                    colorCounts[color]++;
                }

                else
                {
                    colorCounts[color] = 1;
                } 
            }

                for (int i = 0; i < userColors.Length; i++) {
                    if (userColors[i] == chosenColor[i])
                    {
                        colorCounts[userColors[i]]--;
                    }
                    else if (colorCounts.ContainsKey(userColors[i]) && colorCounts[userColors[i]] > 0) {
                        score += 1;
                        colorCounts[userColors[i]]--;
                    }
                    else
                    {
                        score += 2;
                    }
                }
            
            return score;
        }
        private string GenerateFeedback(string[] userColors)
        {
            int correctPosition = 0;
            int correctColor = 0;
            Dictionary<string, int> colorCounts = new Dictionary<string, int>();

            foreach (var color in chosenColor) { 
            
                if(colorCounts.ContainsKey(color))
                {
                    colorCounts[color]++;
                }
                else
                {
                    colorCounts[color] = 1;
                }
                    }

            for (int i = 1; i < userColors.Length; i++) {
                if (userColors[i] == chosenColor[i]) {
                    correctPosition++;
                    colorCounts[userColors[i]]--;
                }
            
            }

            for(int i = 0; i < userColors.Length; i++) {

                if (userColors[i] != chosenColor[i] && colorCounts.ContainsKey(userColors[i]) && colorCounts[userColors[i]] > 0)
                {
                    correctColor++;
                    colorCounts[userColors[i]]--;
                }
            }
            return $"{correctPosition} red, {correctColor} white";

        }
        private void UpdateHistory(string[] userColors, string feedback)
        {
            string attempt = $"{string.Join(", ", userColors)} - feedback: {feedback}";
            historyListBox.Items.Add(attempt);
        }
        private void DisableInput()
        {
            firstComboBox.IsEnabled = false;
            secondComboBox.IsEnabled = false;
            thirdComboBox.IsEnabled = false;
            fourthComboBox.IsEnabled = false;
            controlButton.IsEnabled = false;
        }


        private void GenerateCode()
        {

            Random rnd = new Random();
            for (int i = 0; i < 4; i++)
            {
                int color = rnd.Next(allColors.Length);
                chosenColor[i] = allColors[color];
            }

            StartCountDown(); 
        }
        //stop countdown timer 
        private void StopCountDown()
        {
            timer.Stop();

        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            timeRemaining--;
            timerLabel.Content = $"Tijd: {timeRemaining}";
           
            if (timeRemaining <= 0)
            {
                StopCountDown(); 
                HandleAttempt(); 
            }
        }
        //start countdown timer from 10 seconds and if the player click "check code" start new attempts
        private void StartCountDown()
        {
            timeRemaining = 10; 
            timer.Start(); 
          
        }
        private void UpdateTimerLabel()
        {
            timerLabel.Content = $"Tijd: {timeRemaining}";
        }
        private void ChoosingLabelColors(object sender, RoutedEventArgs e)
        {
        
            //
            ComboBox comboBox = (ComboBox)sender;
            if (comboBox.SelectedItem != null)
            {
                string selectedColor = comboBox.SelectedItem.ToString();
               
                SolidColorBrush colorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(selectedColor));

                switch (comboBox.Name)
                {
                    case "firstComboBox":
                        firstLabel.Background = colorBrush;
/*                        firstLabel.Content = selectedColor;
*/
                        break;
                    case "secondComboBox":
                        secondLabel.Background = colorBrush;
/*                        secondLabel.Content = selectedColor;
*/                        break;
                    case "thirdComboBox":
                        thirdLabel.Background = colorBrush;
/*                        thirdLabel.Content = selectedColor;
*/                        break;
                    case "fourthComboBox":
                        fourthLabel.Background = colorBrush;
/*                        fourthLabel.Content = selectedColor;
*/                        break;
                }
            }

        }
        private void HandleAttempt()
        {
          
           
            if (attempts >= 10) 
            {
                
                StopCountDown();
                MessageBox.Show($"Je hebt al je pogingen gebruikt! De geheime code was: {string.Join(", ", chosenColor)}", "Spel over", MessageBoxButton.OK, MessageBoxImage.Information);
                DisableInput();
                AskToPlayAgain();
                return;
            }
            if(IsCodeCracked())
            {
                StopCountDown();
                MessageBox.Show($"Code is gekraakt in {attempts} pogingen. Wil je nog eens?", "WINNER", MessageBoxButton.YesNo);
                AskToPlayAgain();
                return;
            }

            attempts++;
            Mastermind.Title = $"Poging {attempts}/10";
            StartCountDown(); 
        }
        private void FillComboBoxes(ref string[] items)
        {
            foreach (var item in items)
            {
                firstComboBox.Items.Add(item);
                secondComboBox.Items.Add(item);
                thirdComboBox.Items.Add(item);
                fourthComboBox.Items.Add(item);
            }

          
        }
        //make the textbox visible or unvisible by pressing ctrl F12
        private void ToggleDebug()
        {
          
            if(codeTextBox.Visibility == Visibility.Hidden)
            {
                codeTextBox.Visibility = Visibility.Visible;
            }
            else
            {
                codeTextBox.Visibility = Visibility.Hidden;

            }
        }


        private void SetBorderColor(int index, Color color)
        {
            SolidColorBrush borderBrush = new SolidColorBrush(color);
            switch (index)
            {
                case 0:
                    firstLabel.BorderBrush = borderBrush;
                    firstLabel.BorderThickness = new Thickness(2);
                    break;
                case 1:
                    secondLabel.BorderBrush = borderBrush;
                    secondLabel.BorderThickness = new Thickness(2);
                    break;
                case 2:
                    thirdLabel.BorderBrush = borderBrush;
                    thirdLabel.BorderThickness = new Thickness(2);
                    break;
                case 3:
                    fourthLabel.BorderBrush = borderBrush;
                    fourthLabel.BorderThickness = new Thickness(2);
                    break;
            }
        }
        private void controlButton_Click(object sender, RoutedEventArgs e)
        {
            HandleAttempt();



            string[] userPickedColors =  {
                                 firstComboBox.SelectedItem.ToString(),
                                 secondComboBox.SelectedItem.ToString(),
                                 thirdComboBox.SelectedItem.ToString(),
                                 fourthComboBox.SelectedItem.ToString() 
            };  
          

            int score = CalculateScore(userPickedColors);
            scoreLabel.Content = $"je score: {score}";

            string feedback = GenerateFeedback(userPickedColors);
            UpdateHistory(userPickedColors, feedback);


            for (int i = 0; i< userPickedColors.Length; i++)
            {
                if (userPickedColors[i] == chosenColor[i])
                {
                    SetBorderColor(i, Colors.DarkRed);

                }
                else if(chosenColor.Contains(userPickedColors[i])) {
                   
                    SetBorderColor(i, Colors.Wheat);

                }
                else
                {
                    SetBorderColor(i, Colors.Transparent);

                }
            }

        }
    }
}