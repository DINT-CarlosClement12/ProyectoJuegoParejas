using System.Windows.Controls;

namespace ProyectoJuegoParejas
{
    class CardComparer
    {
        public TextBlock card1 { get; set; }
        public TextBlock card2 { get; set; }

        public CardComparer()
        {
        }

        public CardComparer(TextBlock card1, TextBlock card2)
        {
            this.card1 = card1;
            this.card2 = card2;
        }

        public Border GetBorderCard1()
        {
            return ((Border)((Viewbox)card1.Parent).Parent);
        }

        public Border GetBorderCard2()
        {
            return ((Border)((Viewbox)card2.Parent).Parent);
        }
    }
}
