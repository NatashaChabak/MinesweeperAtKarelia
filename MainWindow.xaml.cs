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
            InitializeComponent();

            //initializeArray (2 dimensional) - random b + neighbors
            minesArray = CreateMinesArray(10, 10);

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
                array[rnd.Next(y), rnd.Next(x)] = -1;
            }
            for (int i = 0; i < y; i++)
            {
                for (int j = 0; j < x; j++)
                {
                    if (array[i, j] == -1)
                    {
                        for (int k = -1; k < 2; k++)
                        {
                            for (int l = -1; l < 2; l++)
                            {
                                if ((0 <= i + k && i + k < y) && (0 <= j + l && j + l < x))
                                { 
                                    if (array[i + k, j + l] != -1) { array[i + k, j + l]++; }
                                }
                            }
                        }
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

            // Set the foreground color of the button (text color)
            button.Foreground = Brushes.Black;

            // Set the border brush and thickness to give it a border
            button.BorderBrush = Brushes.Gray;
            button.BorderThickness = new System.Windows.Thickness(5);

            button.Click += btnToggleRun_Click; //draw
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
                Image image = new Image();
                image.Source = new BitmapImage(new Uri("OIP.jpg", UriKind.Relative));
                image.Stretch = Stretch.UniformToFill;
                btn.Content = image;
                isMine = true;
            }
            else 
            { 
                btn.Content = cell; 
            }
          
            btn.Click -= btnToggleRun_Click;
            btn.MouseRightButtonDown -= btnSigned;
            return isMine;
        }

        public void btnSigned(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn.Content == null)
                {
                    btn.Content = "*";
                    btn.Click -= btnToggleRun_Click;
                }
            else if (btn.Content.Equals("*")) {
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
