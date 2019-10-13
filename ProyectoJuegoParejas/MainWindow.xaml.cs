﻿using System;
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

        public bool onDelay = false;    // When two distinct cards are flipped
        public bool quit = false;    // When player give up or winds
        public int numMovements = 0;    // Count player movements
        internal Board board = new Board(); // Storage data

        ProgressBar currentProgress;

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            debugDifficultyRadioButton.Visibility = Visibility.Visible;
#endif
            currentProgress = AddControls();
        }
        
        private void DrawGame(int totalPlayingCards)    // Initialize the game 
        {
            ResetGame();

            int columnLength = (int)Math.Sqrt(totalPlayingCards);   // Calculus based on how many cards are going to be in the scene
            board.RenderBoard(this, gameGrid, GenerateCardCharacters(columnLength));
        }

        private List<char> GenerateCardCharacters(int columnLength)   // Generates the signs of the playing cards 
        {
            List<char> randomCharacters = new List<char>();
            Random rnd = new Random();
            for (int i = 0; i < columnLength * columnLength / NUM_POSSIBLE_REPETITIONS; i++)
            {
                char actualChar = rnd.Next(0, 2) == 0 ? (char)rnd.Next('A', 'Z') : (char)rnd.Next('a', 'z');    // 49 possible characters ('s' doesn't count)
                if (randomCharacters.Contains(actualChar) || actualChar == Board.INTERROGATION_SIGN)
                {
                    i--;        // _ If random character is already inside
                    continue;   // |_   or is 's', move back one
                }
                for (int j = 0; j < NUM_POSSIBLE_REPETITIONS; j++)  // Add character to the list as many times as repetitions
                    randomCharacters.Add(actualChar);
            }
            return randomCharacters;
        }
            
        private void ResetGame()    // Reset game each time that we draw the scene 
        {
            quit = false;
            numMovements = 0;
            ResetComparison();
            gameGrid.Children.Clear();
            gameGrid.RowDefinitions.Clear();
            gameGrid.ColumnDefinitions.Clear();
            currentProgress.Value = 0;
            currentProgress.Foreground = Brushes.LightGreen;
        }

        private ProgressBar AddControls()  // Set common controls to the scene 
        {
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

            return gameProgress;
        }

        private void ShowAnswer_Click(object sender, RoutedEventArgs e) // Controls give up option 
        {
            if (!onDelay && !quit)   // If we are on delay we cannot surrender
            {
                quit = true;
                foreach (PlayingCard playingCard in board)
                    playingCard.FrontCard.Text = playingCard.ToString();
                if (board.ComparingCard1 != null)
                    board.ComparingCard1.Border.Background = Board.DEFAULT_CARD_BRUSH;
                currentProgress.Foreground = Brushes.IndianRed;
            }
        }

        public void FlipCard(object sender, RoutedEventArgs e) // Shows the playing card value 
        {
            if(!quit && !onDelay)    
            {
                numMovements++;
                if (board.ComparingCard1 == null || board.ComparingCard2 == null)
                {
                    PlayingCard selectedPlayingCard = board[(Border)sender];

                    selectedPlayingCard.Border.Background = Brushes.White;
                    selectedPlayingCard.FrontCard.Text = selectedPlayingCard.FrontCard.Tag.ToString();
                    if (!selectedPlayingCard.IsFlipped)
                    {
                        if (board.ComparingCard1 != null && board.ComparingCard1 != selectedPlayingCard)
                        {
                            board.ComparingCard2 = selectedPlayingCard;

                            if (board.ComparingCard1.FrontCard.Tag.ToString() != board.ComparingCard2.FrontCard.Tag.ToString())
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
                            board.ComparingCard1 = selectedPlayingCard;
                    }
                }
            }
        }

        private void UnflipCards()  // Resets flipped cards into its default values 
        {
            board.ComparingCard1.FrontCard.Text = Board.INTERROGATION_SIGN.ToString();
            board.ComparingCard1.Border.Background = Board.DEFAULT_CARD_BRUSH;

            board.ComparingCard2.FrontCard.Text = Board.INTERROGATION_SIGN.ToString();
            board.ComparingCard2.Border.Background = Board.DEFAULT_CARD_BRUSH;

            ResetComparison();
        }

        private void ResetComparison()  // Reset the CardComparer 
        {
            board.ComparingCard1 = null;
            board.ComparingCard2 = null;
        }

        private void CheckState()   // Evaluate game state and set flipped twin playing cards into true 
        {
            board.ComparingCard1.IsFlipped = true;
            board.ComparingCard2.IsFlipped = true;

            int numIncorrect = 0;
            int numCorrect = board.Count(c =>
            {
                if (c.IsFlipped)
                    return true;
                else
                {
                    numIncorrect++;
                    return false;
                }
            });

            currentProgress.Value = (double)numCorrect / (numIncorrect + numCorrect);  // Set value of the progress

            if (numIncorrect <= 0)   // End game
            {
                quit = true;
                MessageBox.Show($"Felicidades, has completado el nivel en {numMovements / 2} movimientos", "Memo te felicita");
            }
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
