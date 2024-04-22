using Melanchall.DryWetMidi.Multimedia;
using System;
using System.Data.Common;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Timers;



namespace Wpf_Karelia
{

    public partial class MainWindow : Window
    {
        Grid gridMain;
        int[,] minesArray;
        int ySize, xSize, BombsCount;
        BitmapImage bitmapImageFlag;
        BitmapImage bitmapImageMine;
        Timer timer;
        DateTime startTime;

        public MainWindow()
        {
            ySize = 10;
            xSize = 15;
            BombsCount = ySize * xSize / 8;
            bitmapImageFlag = new BitmapImage(new Uri("Flag.jpg", UriKind.Relative));
            bitmapImageMine = new BitmapImage(new Uri("Mine.jpg", UriKind.Relative));

            timer = new Timer();
            timer.Interval = 1000; 
            timer.Elapsed += TimerElapsed;

            InitializeComponent();
            StartTheGame();
            Methods.InitializeSound();

            ShowScore();
        }

        private void ShowScore()
        { scoreText.Text = string.Format("{0}", BombsCount); }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            TimeSpan elapsedTime = e.SignalTime - startTime;
            Dispatcher.Invoke(() =>
            {
                timerText.Text = elapsedTime.ToString(@"hh\:mm\:ss");
            });
        }

        private void StartTheGame()
        {
            minesArray = Methods.CreateMinesArray(ySize, xSize);
            DrawGrid();
            root.Children.Add(gridMain);
            Grid.SetRow(gridMain, 1);
           
            startTime = DateTime.Now;
            timer.Start();
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
            timer.Stop();
            var buttons = gridMain.Children.OfType<Button>();
            foreach (var button in buttons)
            { DrawCell(button, true); }
        }

        public bool DrawCell(Button btn, bool isGameOver = false)
        {
            int row = (int)btn.GetValue(Grid.RowProperty);
            int column = (int)btn.GetValue(Grid.ColumnProperty);
            int cell = minesArray[row, column];
            bool isMine = false;
            if (cell == -1)
            {
                Image mineImage = new Image();
                mineImage.Source = bitmapImageMine;
                mineImage.Stretch = Stretch.UniformToFill;
                btn.Content = mineImage;
                isMine = true;
                byte minebyte = (byte)(30 + ((byte)row + (byte)column) / 2);
                Methods.PlayNote(minebyte);
            }
            else if (cell == 0 && !isGameOver)
            {
                DrawCellAdjacentToZero(row, column);
            }
            else if (!isGameOver)
            {
                AddContentToButton(btn, cell);
            }      
            btn.Click -= btnToggleRun_Click; // change to btn.MouseLeftButtonDown -= btnToggleRun_Click;
            btn.MouseRightButtonDown -= BtnFlagged;
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
            Methods.PlayNote((byte)(cell + 60));

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
                flagImage.Source = bitmapImageFlag;
                btn.Content = flagImage;
                btn.Click -= btnToggleRun_Click;
                Methods.PlayNote(72);
                BombsCount -= 1;
                }
            else 
            {
                btn.Content = null;
                btn.Click += btnToggleRun_Click;
                BombsCount += 1;
            }
            ShowScore();
        }

        private void BtnRestart_Click(object sender, RoutedEventArgs e)
        {
            gridMain.Children.Clear();
            StartTheGame();
        }

        void btnToggleRun_Click(object sender, RoutedEventArgs e)
        {
            if (DrawCell(sender as Button))
            { GameOver(); }
        }

 
    }
}
