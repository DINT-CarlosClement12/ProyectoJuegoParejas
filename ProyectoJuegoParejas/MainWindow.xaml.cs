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

namespace ProyectoJuegoParejas
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DrawGame(4);
        }

        private void DrawGame(int columnLength)
        {
            int halfColumnLength = columnLength / 2;
            for (int i = 0; i < halfColumnLength; i++)
            {
                RowDefinition rowDefinition = new RowDefinition();
                rowDefinition.Height = new GridLength(1, GridUnitType.Star);
                gameGrid.RowDefinitions.Add(rowDefinition);

                ColumnDefinition columnDefinition = new ColumnDefinition();
                columnDefinition.Width = new GridLength(1, GridUnitType.Star);
                gameGrid.ColumnDefinitions.Add(columnDefinition);

                for (int j = 0; j < halfColumnLength; j++)
                {
                    /*Image img = new Image();
                    img.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "/angel.png"));
                    Grid.SetRow(img, i);
                    Grid.SetColumn(img, j);
                    gameGrid.Children.Add(img);*/
                    TextBlock txtBlock = new TextBlock();
                    txtBlock.FontFamily = 
                    txtBlock.Text = "c";
                }
            }
        }
    }
}
