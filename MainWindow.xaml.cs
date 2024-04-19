using System;
using System.Data.Common;
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
        int ySize, xSize;

        public MainWindow()
        {
            ySize = 10;
            xSize = 15;
            minesArray = Methods.CreateMinesArray(ySize, xSize);
            InitializeComponent();
            DrawGrid();
            root.Children.Add(gridMain);
            Grid.SetRow(gridMain, 1);
        }

  

        private void DrawGrid()

        {
            gridMain = new Grid();

            gridMain.ShowGridLines = false;
            for (int i = 0; i < xSize; i++)
            {
                gridMain.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int i = 0; i < ySize; i++)
            {
                gridMain.RowDefinitions.Add(new RowDefinition());
            }

            for (int i = 0; i < xSize; i++)
            {
                for (int j = 0; j < ySize; j++)
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
            button.MouseRightButtonDown += BtnFlagged;
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
            btn.MouseRightButtonDown -= BtnFlagged; //change to BtnFlagged
            return isMine;
        }
        public void DrawCellAdjacentToZero(int row, int column)
        {
            Button btn = GetButtonFromGrid(gridMain, row, column);
            int cell = minesArray[row, column];

            if (cell >= 0 && btn.Content == null)
            {
                AddContentToButton(btn, cell);
                if (cell == 0)
                { 
                for (int i = -1; i < 2; i++)
                {
                    for (int j = -1; j < 2; j++)
                    {
                        if (row + i >= 0 && row + i < ySize && column + j >= 0 && column + j < xSize)
                        {
                            DrawCellAdjacentToZero(row + i, column + j);
                        }
                    }
                }
                }
            }
        }

        private void AddContentToButton(Button btn, int cell)
        {
            if (cell != 0) btn.Content = cell; else btn.Content = "";
            byte color = (byte)(255 / ((cell==0) ? 0.2 : cell*2));
            btn.Background = new SolidColorBrush(Color.FromArgb(50, 255, 255, 255));
            btn.Foreground = new SolidColorBrush(Color.FromArgb(255, 20, color, 20));
            btn.FontSize = 18;
            btn.FontWeight = FontWeights.Bold;
            btn.Click -= btnToggleRun_Click;
            btn.MouseRightButtonDown -= BtnFlagged;

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

        public void BtnFlagged(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn.Content == null)
             {
                Image flagImage = new Image();
                flagImage.Source = new BitmapImage(new Uri("Flag.jpg", UriKind.Relative));
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
