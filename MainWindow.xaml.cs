using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;


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

        private int[,] CreateMinesArray(int ySize, int xSize)
        {
            int [,] array = new int[ySize, xSize];
            int minesCount = 5 + ySize * xSize / 10;
            Random rnd = new Random();
            while (minesCount > 0)
            {
                int xR = rnd.Next(xSize);
                int yR = rnd.Next(ySize);

                if (array[yR, xR] == -1) { continue; }
                
                array[yR, xR] = -1;
                minesCount--;

                int startX = (xR - 1) < 0 ? xR : xR - 1;
                int startY = (yR - 1) < 0 ? yR : yR - 1;
                int finX = (xR + 2) > xSize ? xR : xR + 1;
                int finY = (yR + 2) > ySize ? yR : yR + 1;

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
            { DrawCell(button, true); }
        }

        public bool DrawCell(Button btn, bool isGameOver = false)
        {
            int _row = (int)btn.GetValue(Grid.RowProperty);
            int _column = (int)btn.GetValue(Grid.ColumnProperty);
            int cell = minesArray[_row, _column];
            bool isMine = false;
            if (cell == -1)
            {
                Image mineImage = new Image();
                mineImage.Source = new BitmapImage(new Uri("Mine.jpg", UriKind.Relative));
                mineImage.Stretch = Stretch.UniformToFill;
                btn.Content = mineImage;
                isMine = true;
            }
            else if (cell == 0 && !isGameOver)
            {
                DrawCellAdjacentToZero(_row, _column);
            }
            else if (!isGameOver)
            {
                AddContentToButton(btn, cell);
            }      
            btn.Click -= btnToggleRun_Click; // change to btn.MouseLeftButtonDown -= btnToggleRun_Click;
            btn.MouseRightButtonDown -= btnSigned; //change to BtnFlagged
            return isMine;
        }
        public void DrawCellAdjacentToZero(int row, int column)
        {
            Button btn = GetButtonFromGrid(gridMain, row, column);
            int cell = minesArray[row, column];

            if (minesArray[row, column] == 0 && btn.Content == null)
            {
                AddContentToButton(btn, cell);
                for (int i = -1; i < 2; i++)
                {
                    for (int j = -1; j < 2; j++)
                    {
                        if (row + i >= 0 && row + i < 10 && column + j >= 0 && column + j < 10)
                        {
                            DrawCellAdjacentToZero(row + i, column + j);
                        }
                    }
                }
            }
        }

        private void AddContentToButton(Button btn, int cell)
        {
            btn.Content = cell;
            byte color = (byte)(255 / ((cell==0) ? 0.8 : cell));
            //btn.Foreground = getColor(cell);
            btn.Background = new SolidColorBrush(Color.FromArgb(128, color, color, color));
            // btn.Background = getColor(cell);
            //btn.Background.Opacity = 0.5;
            btn.FontSize = 18;
            btn.FontWeight = FontWeights.Bold;
        }

        public Button GetButtonFromGrid(Grid grid, int row, int column)
        {
            foreach (UIElement child in grid.Children)
            {
                if (Grid.GetRow(child) == row && Grid.GetColumn(child) == column && child is Button)
                {
                    return child as Button;
                }
            }

            return null; // Return null if no button is found at the specified row and column
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
                flagImage.Source = new BitmapImage(new Uri("Flag.jpg", UriKind.Relative));
               // flagImage.Stretch = Stretch.UniformToFill;
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
