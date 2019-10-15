using System.Windows.Controls;

namespace ProyectoJuegoParejas
{
#pragma warning disable CS0659 // El tipo reemplaza a Object.Equals(object o), pero no reemplaza a Object.GetHashCode()
#pragma warning disable S1206 // "Equals(Object)" and "GetHashCode()" should be overridden in pairs
    class PlayingCard
#pragma warning restore S1206 // "Equals(Object)" and "GetHashCode()" should be overridden in pairs
#pragma warning restore CS0659 // El tipo reemplaza a Object.Equals(object o), pero no reemplaza a Object.GetHashCode()
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
    }
}
