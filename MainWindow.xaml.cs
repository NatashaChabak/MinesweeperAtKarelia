using Melanchall.DryWetMidi.Multimedia;
using System;
using System.Data.Common;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Timers;
using System.Windows.Input;



namespace Wpf_Karelia
{

    public partial class MainWindow : Window
    {
        Grid gridMain;
        int[,] minesArray;
        int ySize, xSize, minesCount;
        BitmapImage bitmapImageFlag;
        BitmapImage bitmapImageMine;
        Timer timer;
        DateTime startTime;
        bool isStarted;

        public MainWindow()
        {
            ySize = 10;
            xSize = 15;

            bitmapImageFlag = new BitmapImage(new Uri("Flag.jpg", UriKind.Relative));
            bitmapImageMine = new BitmapImage(new Uri("Mine.jpg", UriKind.Relative));

            timer = new Timer();
            timer.Interval = 1000;
            timer.Elapsed += TimerElapsed;

            InitializeComponent();
            StartTheGame();
         }

        private void StartTheGame()
        {
            minesCount = ySize * xSize / 8;
            minesArray = Methods.CreateMinesArray(ySize, xSize, minesCount);
            DrawGrid();
            root.Children.Add(gridMain);
            Grid.SetRow(gridMain, 1);

            startTime = DateTime.Now;
            timer.Start();
            ShowScore();
            gridMain.MouseWheel += GridMain_MouseWheel;
        }
        private void GameOver()
        {
            DrawAllMines();
            timer.Stop();
        }
        private void ShowScore()
        { scoreText.Text = string.Format("COUNT {0}", minesCount); }
        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            TimeSpan elapsedTime = e.SignalTime - startTime;
            Dispatcher.Invoke(() =>
            {
                timerText.Text = elapsedTime.ToString(@"hh\:mm\:ss");
            });
        }

        //======================================================Drawing=================================================
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
        private void DrawCheckCell(Button btn)
        {
            int row = (int)btn.GetValue(Grid.RowProperty);
            int column = (int)btn.GetValue(Grid.ColumnProperty);
            int cell = minesArray[row, column];
            switch (cell)
            {
                case -1:
                    GameOver();
                    break;
                case 0:
                    DrawCellAdjacentToZero(row, column);
                    break;
                default:
                    AddContentToButton(btn, cell);
                    break;
            }
            btn.Click -= Btn_Click;
            btn.MouseRightButtonDown -= Btn_RightClick;
        }
        private void DrawCellAdjacentToZero(int row, int column)
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
        private void DrawAllMines()
        {
            var buttons = gridMain.Children.OfType<Button>();
            foreach (var btn in buttons)
            {
                int row = (int)btn.GetValue(Grid.RowProperty);
                int column = (int)btn.GetValue(Grid.ColumnProperty);
                int cell = minesArray[row, column];
                if (cell == -1)
                {
                    Image mineImage = new Image();
                    mineImage.Source = bitmapImageMine;
                    mineImage.Stretch = Stretch.UniformToFill;
                    btn.Content = mineImage;
                    byte minebyte = (byte)(30 + ((byte)row + (byte)column) / 2);
                    Methods.PlayNote(minebyte);
                }
            }
        }

        //======================================================Buttons=================================================
        private Button AddButton()
        {
            Button button = new Button();
            button.Height = 50;
            button.Width = 50;
            button.Background = Brushes.LightGray;

            button.Foreground = Brushes.Black;
            button.BorderBrush = Brushes.Gray;

            button.BorderThickness = new System.Windows.Thickness(0, 0, 5, 5);

            button.Click += Btn_Click;
            button.MouseRightButtonDown += Btn_RightClick;
            return button;
        }
        private void AddContentToButton(Button btn, int cell)
        {
            if (cell != 0) btn.Content = cell; else btn.Content = "";
            byte color = (byte)(255 / ((cell==0) ? 0.2 : cell*2));
            btn.Background = new SolidColorBrush(Color.FromArgb(50, 255, 255, 255));
            btn.Foreground = new SolidColorBrush(Color.FromArgb(255, 20, color, 20));
            btn.FontSize = 18;
            btn.FontWeight = FontWeights.Bold;
            btn.Click -= Btn_Click;
            btn.MouseRightButtonDown -= Btn_RightClick;
            Methods.PlayNote((byte)(cell + 60));
        }
        private Button GetButtonFromGrid(Grid grid, int row, int column)
        {
            foreach (UIElement child in grid.Children)
            {
                if (Grid.GetRow(child) == row && Grid.GetColumn(child) == column && child is Button)
                {
                    return child as Button;
                }
            }
            return null;
        }

        //======================================================Actions=================================================
        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            isStarted = true;
            DrawCheckCell(sender as Button);
        }
        private void Btn_RightClick(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn.Content == null)
             {
                Image flagImage = new Image();
                flagImage.Source = bitmapImageFlag;
                btn.Content = flagImage;
                btn.Click -= Btn_Click;
                Methods.PlayNote(72);
                minesCount -= 1;
                }
            else 
            {
                btn.Content = null;
                btn.Click += Btn_Click;
                minesCount += 1;
            }
            ShowScore();
        }
        private void Btn_RestartClick(object sender, RoutedEventArgs e)
        {
            gridMain.Children.Clear();
            isStarted = false;
            StartTheGame();
        }
        private void GridMain_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (isStarted) { return; }
            ySize -= e.Delta / 120;
            xSize -= e.Delta / 120;

            gridMain.Children.Clear();
            StartTheGame();
        }
    }
}
