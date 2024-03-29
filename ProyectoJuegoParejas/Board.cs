﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ProyectoJuegoParejas
{
    class Board : IEnumerable<PlayingCard>
    {
        const string FONT_FAMILY_CARD_SYMBOLS = "Webdings";
        const string PLAYING_CARDS_STYLE = "PlayingCard";

        public PlayingCard ComparingCard1 { get; set; }
        public PlayingCard ComparingCard2 { get; set; }

        public const char INTERROGATION_SIGN = 's';    // Interrogation sign in "Webdings"
        public readonly static LinearGradientBrush DEFAULT_CARD_BRUSH = new LinearGradientBrush(Colors.CadetBlue, Colors.White, new Point(0, 1), new Point(0, 0)); // Cards reverse color

        MainWindow windowGame = null;

        List<PlayingCard> playingCards = new List<PlayingCard>();

        public void RenderBoard(MainWindow windowGame, Grid gameGrid, List<char> randomCharacters, int fixedWidth)
        {
            int heigth = randomCharacters.Count / fixedWidth;
            int width = fixedWidth;

            this.windowGame = windowGame;
            playingCards = new List<PlayingCard>();

            // Set playing cards to the scene
            bool columnSet = false;
            Random rnd = new Random();
            for (int i = 0; i < heigth; i++)
            {
                RowDefinition rowDefinition = new RowDefinition();
                gameGrid.RowDefinitions.Add(rowDefinition);

                for (int j = 0; j < width; j++)
                {
                    if (!columnSet)
                    {
                        ColumnDefinition columnDefinition = new ColumnDefinition();
                        gameGrid.ColumnDefinitions.Add(columnDefinition);
                    }
                    PlayingCard playingCard = CreatePlayingCard(randomCharacters, rnd);

                    Border border = playingCard.Border;

                    Grid.SetRow(border, i);
                    Grid.SetColumn(border, j);

                    gameGrid.Children.Add(border);
                    playingCards.Add(playingCard);
                }
                columnSet = true;
            }
        }

        private PlayingCard CreatePlayingCard(List<char> randomCharacters, Random rnd)   // Initialize gaming card controls schema 
        {
            int currentIndexChar = rnd.Next(0, randomCharacters.Count); // -1 ??

            TextBlock frontCard = new TextBlock // Where character sign is going to be draw 
            {
                FontFamily = new FontFamily(FONT_FAMILY_CARD_SYMBOLS),
                Tag = randomCharacters[currentIndexChar].ToString(),    // The value of the playing card
                Text = INTERROGATION_SIGN.ToString()
            };
            Viewbox viewbox = new Viewbox   // To adjust font size of the TextBlock 
            {
                Child = frontCard
            };
            Border border = new Border  // To get color to background of the playing card
            {
                Style = windowGame.FindResource(PLAYING_CARDS_STYLE) as Style,
                Background = DEFAULT_CARD_BRUSH,
                Child = viewbox
            };

            border.MouseDown += windowGame.FlipCard;   // Attach event to method

            randomCharacters.RemoveAt(currentIndexChar);    // Removes the drawn character

            return new PlayingCard(frontCard,viewbox,border);
        }

        public PlayingCard this[Border border]
        {
            get
            {
                return playingCards.Find(p => p.Border == border);
            }
        }

        public IEnumerator<PlayingCard> GetEnumerator()
        {
            return ((IEnumerable<PlayingCard>)playingCards).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<PlayingCard>)playingCards).GetEnumerator();
        }
    }
}
