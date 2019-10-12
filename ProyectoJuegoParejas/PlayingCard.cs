using System.Windows.Controls;

namespace ProyectoJuegoParejas
{
    class PlayingCard
    {
        public TextBlock FrontCard { get; private set; }
        public Viewbox Viewbox { get; private set; }
        public Border Border { get; private set; }
        public bool IsFlipped { get; set; }

        public PlayingCard()
        {
        }

        public PlayingCard(TextBlock frontCard, Viewbox viewbox, Border border)
        {
            FrontCard = frontCard;
            Viewbox = viewbox;
            Border = border;
        }

        public override bool Equals(object obj)
        {
            return FrontCard.Tag == ((PlayingCard)obj).FrontCard.Tag;
        }

        public override string ToString()
        {
            return FrontCard.Tag.ToString();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
