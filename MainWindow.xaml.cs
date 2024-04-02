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


namespace Wpf_Karelia
{

    public partial class MainWindow : Window
    {
        Grid gridMain;
        int[,] minesArray;

        public MainWindow()
        {
            minesArray = CreateMinesArray(10, 10);
            InitializeComponent();
            DrawGrid();
            root.Children.Add(gridMain);
            Grid.SetRow(gridMain, 1);
            //GameOver();
        }

        private int[,] CreateMinesArray(int y, int x)
        {
            int [,] array = new int[y, x];
            int minesCount = y * x / 10;
            Random rnd = new Random();
            for (int c = 0; c < minesCount; c++)
            {
                int xR = rnd.Next(x);
                int yR = rnd.Next(y);

                if (array[yR, xR] == -1) { continue; }
                
                array[yR, xR] = -1;

                int startX = (xR - 1) < 0 ? xR : xR - 1;
                int startY = (yR - 1) < 0 ? yR : yR - 1;
                int finX = (xR + 2) > x ? xR : xR + 1;
                int finY = (yR + 2) > y ? yR : yR + 1;


                for (int k = startY; k < finY + 1; k++)
                {
                    for (int l = startX; l < finX + 1; l++)
                    {
                        if (array[k, l] != -1) { array[k, l]++; }
                    }
                }
            }
            return array;
        }

        private void DrawGrid()

        {
            gridMain = new Grid();

            gridMain.ShowGridLines = false;
            for (int i = 0; i < 10; i++)
            {
                gridMain.ColumnDefinitions.Add(new ColumnDefinition());
                gridMain.RowDefinitions.Add(new RowDefinition());
            }

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    Button button = AddButton();
                    Grid.SetColumn(button, i);
                    Grid.SetRow(button, j);
                    gridMain.Children.Add(button);
                }
            }

        }

        public Button AddButton()
        {
            Button button = new Button();
            button.Height = 50;
            button.Width = 50;
            button.Background = Brushes.LightGray;

            button.Foreground = Brushes.Black;
            button.BorderBrush = Brushes.Gray;
            
            button.BorderThickness = new System.Windows.Thickness(0, 0, 5, 5);
   
            button.Click += btnToggleRun_Click; 
            button.MouseRightButtonDown += btnSigned;
            return button;
        }

        public void GameOver()
        {
            var buttons = gridMain.Children.OfType<Button>();
            foreach (var button in buttons)
            { DrawCell(button); }
        }

        public bool DrawCell(Button btn)
        {
            int _row = (int)btn.GetValue(Grid.RowProperty);
            int _column = (int)btn.GetValue(Grid.ColumnProperty);
            int cell = minesArray[_row, _column];
            bool isMine = false;
            if (cell == -1)
            {
                Image mineImage = new Image();
                mineImage.Source = new BitmapImage(new Uri("OIP.jpg", UriKind.Relative));
                mineImage.Stretch = Stretch.UniformToFill;
                btn.Content = mineImage;
                isMine = true;
            }
            else 
            { 
                btn.Content = cell;
                btn.Foreground = getColor(cell);
                btn.FontSize = 18; 
                btn.FontWeight = FontWeights.Bold;
            }      
            btn.Click -= btnToggleRun_Click;
            btn.MouseRightButtonDown -= btnSigned;
            return isMine;
        }

        public SolidColorBrush getColor(int cell)
        {
            if (cell == 0) { return Brushes.Gray; }
            else if (cell == 1) { return Brushes.Blue; }
            else if (cell == 2) { return Brushes.Green; }
            else if (cell == 3) { return Brushes.Red; }
            else if (cell == 4) { return Brushes.Brown; }
            else { return Brushes.Yellow; }
        }

        public void btnSigned(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn.Content == null)
             {
                Image flagImage = new Image();
                flagImage.Source = new BitmapImage(new Uri("OIP.jpg", UriKind.Relative));
                flagImage.Stretch = Stretch.UniformToFill;
                btn.Content = flagImage;
                btn.Click -= btnToggleRun_Click;
                }
            else 
            {
                btn.Content = null;
                btn.Click += btnToggleRun_Click;
            }
        }

        private void btnToggleRun_Click(object sender, RoutedEventArgs e)
        {
            if (DrawCell(sender as Button))
            { GameOver(); }
        }

    }
}
