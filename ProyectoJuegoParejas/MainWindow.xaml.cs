using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace ProyectoJuegoParejas
{
    public partial class MainWindow : Window
    {
        const int NUM_POSSIBLE_REPETITIONS = 2; // Number of cards of the same type
        const char INTERROGATION_SIGN = 's';    // Interrogation sign in "Webdings"
        LinearGradientBrush DEFAULT_CARD_BRUSH = new LinearGradientBrush(Colors.CadetBlue, Colors.White, new Point(0, 1), new Point(0, 0)); // Cards reverse color

        public bool onDelay = false;    // When two distinct cards are flipped
        public bool quit = false;    // When player give up
        public int numMovements = 0;    // Count player movements

        CardComparer comparingCards = new CardComparer();   // Card comparator

        public MainWindow()
        {
            InitializeComponent();
        }

        private void DrawGame(int totalPlayingCards)    // Initialize the game 
        {
            // Reset game each time that we draw the scene
            quit = false;
            numMovements = 0;
            ResetComparison();
            gameGrid.Children.Clear();
            gameGrid.RowDefinitions.Clear();
            gameGrid.ColumnDefinitions.Clear();

            // Set common controls to the scene
            ((DockPanel)giveUpBorder.Child).Children.Clear();
            Button giveUpButton = new Button()
            {
                Name = "giveUpButton",
                Margin = new Thickness(50, 0, 10, 0),
                Content = "Mostrar"
            };
            giveUpButton.Click += ShowAnswer_Click;
            DockPanel.SetDock(giveUpButton, Dock.Right);

            ((DockPanel)giveUpBorder.Child).Children.Add(giveUpButton);
            ProgressBar gameProgress = new ProgressBar()
            {
                Minimum = 0,
                Maximum = 1,
                Value = 0,
                BorderThickness = new Thickness(5),
                Margin = new Thickness(5)
            };
            DockPanel.SetDock(gameProgress, Dock.Left);
            ((DockPanel)giveUpBorder.Child).Children.Add(gameProgress);

            // Determine the characters of each card
            int columnLength = (int)Math.Sqrt(totalPlayingCards);   // Calculus based on how many cards are going to be in the scene

            List<char> randomCharacters = new List<char>();
            Random rnd = new Random();
            for (int i = 0; i < columnLength * columnLength / NUM_POSSIBLE_REPETITIONS; i++)
            {
                char actualChar = (char)rnd.Next('A', 'Z');
                if (randomCharacters.Contains(actualChar))
                {
                    i--;        // If random character is already inside the List 
                    continue;   //  we go back one in the loop   
                }
                for (int j = 0; j < NUM_POSSIBLE_REPETITIONS; j++)  // Add character to the list as many times as repetitions
                    randomCharacters.Add(actualChar);
            }

            // Set playing cards to the scene
            for (int i = 0; i < columnLength; i++)
            {
                RowDefinition rowDefinition = new RowDefinition
                {
                    Height = new GridLength(1, GridUnitType.Star)
                };
                gameGrid.RowDefinitions.Add(rowDefinition);

                ColumnDefinition columnDefinition = new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                };
                gameGrid.ColumnDefinitions.Add(columnDefinition);

                for (int j = 0; j < columnLength; j++)
                {
                    Border border = CreatePlayingCard(randomCharacters, rnd);

                    Grid.SetRow(border, i);
                    Grid.SetColumn(border, j);

                    gameGrid.Children.Add(border);
                }
            }
        }

        private Border CreatePlayingCard(List<char> randomCharacters, Random rnd)   // Initialize gaming card controls schema
        {
            int currentIndexChar = rnd.Next(0, randomCharacters.Count - 1);

            TextBlock frontCard = new TextBlock // Where character sign is going to be draw 
            {
                FontFamily = new FontFamily("Webdings"),
                Tag = randomCharacters[currentIndexChar].ToString(),    // The value of the playing card
                Text = INTERROGATION_SIGN.ToString()
            };
            Viewbox viewbox = new Viewbox   // To adjust font size of the TextBlock 
            {
                Child = frontCard
            };
            Border border = new Border  // To get color to background of the playing card. Flag indicates if that card is flipped and discovered it's twin 
            {
                Style = FindResource("PlayingCard") as Style,
                Tag = false,
                Background = DEFAULT_CARD_BRUSH,
                Child = viewbox
            };

            border.MouseDown += FlipCard;   // Attach event to method

            randomCharacters.RemoveAt(currentIndexChar);    // Removes the drawn character

            return border;
        }

        private void ShowAnswer_Click(object sender, RoutedEventArgs e) // Controls give up option 
        {
            if (!onDelay)   // If we are on delay we cannot surrender
            {
                foreach (Border c in gameGrid.Children)
                    ((TextBlock)((Viewbox)c.Child).Child).Text = ((TextBlock)((Viewbox)c.Child).Child).Tag.ToString();  // Turn every text in TextBlock into it's tag
                quit = true;
                if(comparingCards.card1 != null)
                    comparingCards.GetBorderCard1().Background = DEFAULT_CARD_BRUSH;    // Reset playing card color when only one playing card is flipped
            }
        }

        private void FlipCard(object sender, RoutedEventArgs e) // Shows the playing card value 
        {
            if(!quit || !onDelay)    
            {
                numMovements++;
                if (comparingCards.card1 == null || comparingCards.card2 == null)
                {
                    Border selectedObjectBorder = (Border)sender;
                    Viewbox selectedViewbox = (Viewbox)selectedObjectBorder.Child;
                    TextBlock selectedTextBlock = (TextBlock)selectedViewbox.Child;

                    selectedObjectBorder.Background = Brushes.White;
                    selectedTextBlock.Text = selectedTextBlock.Tag.ToString();

                    if (comparingCards.card1 != null)
                    {
                        comparingCards.card2 = selectedTextBlock;

                        if (comparingCards.card1.Tag.ToString() != comparingCards.card2.Tag.ToString())
                        {
                            int delaySeconds = 1;
                            onDelay = true;
                            DispatcherTimer timer = new DispatcherTimer
                            {
                                IsEnabled = false,
                                Interval = TimeSpan.FromMilliseconds(1000)
                            };
                            timer.Tick += delegate
                            {
                                delaySeconds--;
                                if (delaySeconds == 0 || onDelay == false)
                                {
                                    timer.Stop();
                                    UnflipCards();  // Timer ended and playing cards flip again
                                    onDelay = false;
                                }
                            };
                            timer.Start();
                        }
                        else
                        {
                            CheckState();
                            ResetComparison();
                        }
                    }
                    else
                        comparingCards.card1 = selectedTextBlock;
                }
            }
        }

        private void UnflipCards()  // Resets flipped cards into its default values 
        {
            comparingCards.card1.Text = INTERROGATION_SIGN.ToString();
            comparingCards.GetBorderCard1().Background = DEFAULT_CARD_BRUSH;

            comparingCards.card2.Text = INTERROGATION_SIGN.ToString();
            comparingCards.GetBorderCard2().Background = DEFAULT_CARD_BRUSH;

            ResetComparison();
        }

        private void ResetComparison()  // Reset the CardComparer 
        {
            comparingCards = new CardComparer(null, null);
        }

        private void CheckState()   // Evaluate game state and set flipped twin playing cards into true 
        {
            comparingCards.GetBorderCard1().Tag = true;
            comparingCards.GetBorderCard2().Tag = true;

            int numIncorrect = 0;
            int numCorrect = gameGrid.Children
                .Cast<Border>()
                .Count(b =>
                {
                    if(!(bool)b.Tag)
                        numIncorrect++;
                    return (bool)b.Tag;
                });

            ((DockPanel)giveUpBorder.Child).Children
                .Cast<Control>()
                .Single(c =>
                {
                    if (c is ProgressBar)
                        ((ProgressBar)c).Value = (double)numCorrect / (numIncorrect + numCorrect);  // Set value of the progress 
                    return c is ProgressBar;
                });
            if(numIncorrect <= 0)   // End game
                MessageBox.Show($"Felicidades, has completado el nivel en {numMovements / 2} movimientos", "Memo te felicita");
        }

        private void InitButton_Click(object sender, RoutedEventArgs e) // Game initialitation 
        {
            if (!onDelay)   // Only initializes when is no delay. Prevents setting default values errors
            {
                RadioButton radioButtonSelected = radioButtonContainer.Children.Cast<RadioButton>().Single(r => r.IsChecked == true);   // Get what RadioButton is checked
                DrawGame((int)Math.Pow(double.Parse(radioButtonSelected.Tag.ToString()), 2));   // Calculus to avoid making a vaiable or call "radioButtonSelected.Tag.ToString()" twice. Tag is number of playing cards
            }
        }
    }
}
