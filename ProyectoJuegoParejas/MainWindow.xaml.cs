using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace ProyectoJuegoParejas
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private struct CardComparer
        {
            public TextBlock card1;
            public TextBlock card2;
        }

        const int NUM_POSSIBLE_REPETITIONS = 2;
        const char INTERROGATION_SIGN = 's';
        Brush DEFAULT_CARD_BRUSH = Brushes.Gainsboro; // change solid color to fade

        public bool onDelay = false;

        CardComparer comparingCards = new CardComparer();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void DrawGame(int totalPlayingCards)
        {
            ResetComparison();
            gameGrid.Children.Clear();

            ((DockPanel)giveUpBorder.Child).Children.Clear();
            Button giveUpButton = new Button()
            {
                Name = "giveUpButton",
                Margin = new Thickness(50, 0, 10, 0),
                Content = "Mostrar"
            };
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

            int columnLength = (int)Math.Sqrt(totalPlayingCards);

            List<char> randomCharacters = new List<char>();
            Random rnd = new Random();
            for (int i = 0; i < columnLength * columnLength / 2; i++)
            {
                char actualChar = (char)rnd.Next('A', 'Z');
                if (randomCharacters.Contains(actualChar))
                {
                    i--;
                    continue;
                }
                for (int j = 0; j < NUM_POSSIBLE_REPETITIONS; j++)
                    randomCharacters.Add(actualChar);
            }

            gameGrid.RowDefinitions.Clear();
            gameGrid.ColumnDefinitions.Clear();

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
                    int currentIndexChar = rnd.Next(0,randomCharacters.Count - 1);

                    TextBlock frontCard = new TextBlock
                    {
                        FontFamily = new FontFamily("Webdings"),
                        Tag = randomCharacters[currentIndexChar].ToString(),
                        Text = INTERROGATION_SIGN.ToString()
                    };

                    Viewbox viewbox = new Viewbox
                    {
                        Child = frontCard
                    };

                    Border border = new Border
                    {
                        Tag = false,
                        Background = DEFAULT_CARD_BRUSH,
                        Margin = new Thickness(5),
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(5),
                        CornerRadius = new CornerRadius(5),
                        Child = viewbox
                    };
                    border.MouseDown += FlipCard;


                    randomCharacters.RemoveAt(currentIndexChar);

                    Grid.SetRow(border, i);
                    Grid.SetColumn(border, j);

                    gameGrid.Children.Add(border);
                }
            }
        }

        private void FlipCard(object sender, RoutedEventArgs e)
        {
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

                        DispatcherTimer timer = new DispatcherTimer();
                        timer.IsEnabled = false;
                        timer.Interval = TimeSpan.FromMilliseconds(1000);

                        int delaySeconds = 2;
                        onDelay = true;
                        timer.Start();
                        timer.Tick += delegate
                        {
                            delaySeconds--;
                            if (delaySeconds == 0)
                            {
                                timer.Stop();
                                UnflipCards();
                                onDelay = false;
                            }
                        };
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

        private void UnflipCards()
        {
            comparingCards.card1.Text = INTERROGATION_SIGN.ToString();
            ((Border)((Viewbox)comparingCards.card1.Parent).Parent).Background = DEFAULT_CARD_BRUSH;

            comparingCards.card2.Text = INTERROGATION_SIGN.ToString();
            ((Border)((Viewbox)comparingCards.card2.Parent).Parent).Background = DEFAULT_CARD_BRUSH;

            ResetComparison();
        }

        private void ResetComparison()
        {
            comparingCards.card1 = null;
            comparingCards.card2 = null;
        }

        private void CheckState()
        {
            ((Border)((Viewbox)comparingCards.card1.Parent).Parent).Tag = true;
            ((Border)((Viewbox)comparingCards.card2.Parent).Parent).Tag = true;

            int numCorrect = gameGrid.Children.Cast<Border>().Count(i => (bool)i.Tag);
            int numIncorrect = gameGrid.Children.Cast<Border>().Count(i => !(bool)i.Tag);

            ((DockPanel)(giveUpBorder).Child).Children
                .Cast<Control>()
                .Single(c =>
                {
                    if (c is ProgressBar)
                    {
                        ((ProgressBar)c).Value = (double)numCorrect / (numIncorrect + numCorrect);
                    }
                    return c is ProgressBar;
                });
        }

        private void InitButton_Click(object sender, RoutedEventArgs e)
        {
            if (!onDelay)
            {
                RadioButton radioButtonSelected = radioButtonContainer.Children.Cast<RadioButton>().Single(r => r.IsChecked == true);
                DrawGame((int)Math.Pow(double.Parse(radioButtonSelected.Tag.ToString()), 2));
            }
        }
    }
}
