using System;
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
        int ySize = 10;
        int ratio = 16;
        int xSize, minesCount, unOpenedCellsCount;
        BitmapImage bitmapImageFlag, bitmapImageMine, bitmapImageWin;
        Timer timer;
        DateTime startTime;
        bool isStarted;
        Image winImage;

        public MainWindow()
        {
            bitmapImageFlag = new BitmapImage(new Uri("FlagFin.png", UriKind.Relative));
            bitmapImageMine = new BitmapImage(new Uri("Mine.jpg", UriKind.Relative));
            bitmapImageWin = new BitmapImage(new Uri("Winners.jpeg", UriKind.Relative));
            timer = new Timer();
            timer.Interval = 1000;
            timer.Elapsed += TimerElapsed;
            Methods.PlayNote(0); 
            InitializeComponent();
            StartTheGame();
         }
        private void StartTheGame()
        {
            xSize = ySize * ratio / 10;
            minesCount = ySize * xSize / 8;
            minesArray = Methods.CreateMinesArray(minesCount, ySize, xSize);
            unOpenedCellsCount = ySize * xSize - minesCount;
            gridMain = DrawGrid();
            root.Children.Add(gridMain);
            Grid.SetRow(gridMain, 1);
            startTime = DateTime.Now;
            timer.Start();
            ShowScore();
            gridMain.MouseWheel += GridMain_MouseWheel;
        }
        private void GameOver()
        {
            timer.Stop();
            DisableButtons(true);
        }
        private void WonTheGame()
        {
            timer.Stop();
            winImage = new Image();
            winImage.Source = bitmapImageWin;
            SetImageProperties(winImage, this.ActualHeight, this.ActualWidth);     
            root.Children.Add(winImage);        
            Grid.SetRow(winImage, 1);
            DisableButtons(false);
            Methods.PlayWinChordProgression(new Random().Next(48, 60));
        }

        private void ShowScore()
        { scoreText.Text = string.Format("Mines: {0}", minesCount); }
        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            TimeSpan elapsedTime = e.SignalTime - startTime;
            Dispatcher.Invoke(() =>
            {
                timerText.Text = elapsedTime.ToString(@"mm\:ss");
            });
        }

        private Grid DrawGrid()
        {
            var grid = new Grid();
            grid.ShowGridLines = false;
            for (int i = 0; i < xSize ; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int i = 0; i < ySize; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
            }
            for (int i = 0; i < xSize; i++)
            {
                for (int j = 0; j < ySize; j++)
                {
                    Button button = AddButton();
                    Grid.SetColumn(button, i);
                    Grid.SetRow(button, j);
                    grid.Children.Add(button);
                }
            }
            return grid;

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
                    Methods.PlayNote((byte)(2 * cell + 48));
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
                if (cell != 0)
                {
                    byte cellByte = (byte)((48 + row + column));
                    Methods.PlayNote(cellByte, 10);
                }
                else
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
        private void DisableButtons(bool isLost)
        {
            var buttons = gridMain.Children.OfType<Button>();
            foreach (var btn in buttons)
            {
                btn.Click -= Btn_Click;
                btn.MouseRightButtonDown -= Btn_RightClick;
                if (isLost) DrawMine(btn);
            }
        }

        private void SetImageProperties(Image image, double ActualHeight, double ActualWidth)
        {
           image.Height = 0.8 * ActualHeight;
           image.Width = 0.8 * ActualWidth;
           //image.HorizontalAlignment = HorizontalAlignment.Stretch;
           //image.VerticalAlignment = VerticalAlignment.Stretch;
        }

        private void DrawMine(Button btn)
        {
            int row = (int)btn.GetValue(Grid.RowProperty);
            int column = (int)btn.GetValue(Grid.ColumnProperty);
            int cell = minesArray[row, column];
            if (cell == -1)
            {
                Image mineImage = new Image();
                mineImage.Source = bitmapImageMine;
                SetImageProperties(mineImage, btn.ActualHeight, btn.ActualWidth);
                btn.Content = mineImage;
                byte minebyte = ((byte)((30 + (row + column) / 2)));
                Methods.PlayNote(minebyte, 20);
            }
        }

        //======================================================Buttons=================================================
        private Button AddButton()
        {
            Button button = new Button();
            button.MinHeight = 50;
            button.MinWidth = 50;
            button.Background = Brushes.DarkGray;
            button.HorizontalAlignment = HorizontalAlignment.Stretch;
            button.VerticalAlignment = VerticalAlignment.Stretch;
            button.Foreground = Brushes.Black;
            button.BorderBrush = Brushes.Gray;
            button.BorderThickness = new System.Windows.Thickness(0, 0, 5, 5);
            button.Click += Btn_Click;
            button.MouseRightButtonDown += Btn_RightClick;
            button.SizeChanged += Button_SizeChanged;
            return button;
        }

        private void Button_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Button btn = sender as Button;
            btn.FontSize = 0.6 * btn.ActualHeight;
            if (btn.Content != null && btn.Content.GetType() == typeof(Image))
            {
                Image image = btn.Content as Image;
                image.Height = 0.8 * btn.ActualHeight;
                image.Width = 0.8 * btn.ActualWidth;
            }
        }

        private void AddContentToButton(Button btn, int cell)
        {
            if (cell != 0) btn.Content = cell;
            else btn.Content = "";
            btn.Background = new SolidColorBrush(Color.FromArgb(50, 255, 255, 255));
            switch (cell)
            {
                case 0:
                    btn.Foreground = Brushes.Transparent;
                    break;
                case 1:
                    btn.Foreground = Brushes.Green;
                    break;
                case 2:
                    btn.Foreground = Brushes.Red;
                    break;
                case 3:
                    btn.Foreground = Brushes.Blue;
                    break;
                case 4:
                    btn.Foreground = Brushes.Yellow;
                    break;
                case 5:
                    btn.Foreground = Brushes.Orange;
                    break;
                case 6:
                    btn.Foreground = Brushes.Purple;
                    break;
                case 7:
                    btn.Foreground = Brushes.Cyan;
                    break;
                case 8:
                    btn.Foreground = Brushes.Magenta;
                    break;
            }
            btn.FontSize = 0.6 * btn.ActualHeight;
            btn.FontWeight = FontWeights.Bold;
            btn.Click -= Btn_Click;
            btn.MouseRightButtonDown -= Btn_RightClick;
            if (--unOpenedCellsCount == 0) { WonTheGame(); }
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

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            int sideRatio = 100 * (int)this.ActualWidth / (int)this.ActualHeight;
            if (!CheckSize((int)this.ActualHeight, (int)this.ActualWidth) && CheckSize((int)e.PreviousSize.Height, (int)e.PreviousSize.Width))
            {
                this.Height = e.PreviousSize.Height;
                this.Width = e.PreviousSize.Width;
            }
        }

        private Boolean CheckSize(int Height, int Width)
        {
            int sideRatio = 100 * Width / Height;
            if (sideRatio > 170 || sideRatio < 130 || (Height / this.ySize < 55)) { return false; }
            return true;
        }

        private void Btn_RightClick(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn.Content == null)
            {
                if (minesCount == 0 ) { return; } 
                Image flagImage = new Image();
                flagImage.Source = bitmapImageFlag;
                SetImageProperties(flagImage, btn.ActualHeight, btn.ActualWidth);
                btn.Content = flagImage;
                //btn.Background = Brushes.DarkGray;
                btn.Click -= Btn_Click;
                Methods.PlayNote((byte)(72 - minesCount));
                minesCount -= 1;
                }
            else 
            {
                btn.Content = null;
               //btn.Background = Brushes.LightGray;
                btn.Click += Btn_Click;
                minesCount += 1;
            }
            ShowScore();
        }
        private void BtnRestart_Click(object sender, RoutedEventArgs e)
        {
            Methods.PlayGlissando(new Random().Next(60, 72), 24);
            gridMain.Children.Clear();
            isStarted = false;
            root.Children.Remove(winImage);
            StartTheGame();
        }
        private void GridMain_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (isStarted)  return;
            ySize -= e.Delta / 120;
            ySize = (ySize < 5) ? 5 : ySize;
            ySize = (ySize > 15) ? 15 : ySize;
            Methods.PlayGlissandoPentatonic(ySize * 7, (e.Delta > 0 ? 1 : -1 ) * 20, 50);
            gridMain.Children.Clear();
            StartTheGame();
        }
    }
}
