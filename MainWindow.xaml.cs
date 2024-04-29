using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Timers;
using System.Windows.Input;
using System.Collections.Generic;

namespace Wpf_Karelia
{
    public partial class MainWindow : Window
    {
        Grid gridMain;
        int[,] minesArray;
        int ySize = 10;
        int ratio = 16;
        int xSize, minesCount, unOpenedCellsCount;
        BitmapImage bitmapImageFlag, bitmapImageMine;
        System.Timers.Timer timer;
        DateTime startTime;
        bool isStarted;
        TextBlock winText;

        public MainWindow()
        {
            bitmapImageFlag = new BitmapImage(new Uri("Flag.jpg", UriKind.Relative));
            bitmapImageMine = new BitmapImage(new Uri("Mine.jpg", UriKind.Relative));

            timer = new System.Timers.Timer();
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
            winText = new TextBlock();
            winText.HorizontalAlignment = HorizontalAlignment.Center;
            winText.VerticalAlignment = VerticalAlignment.Center;
            winText.FontSize = 38;
            winText.Text = "Congrats";
            winText.Background = Brushes.Wheat;
            root.Children.Add(winText);
            Grid.SetRow(winText, 1);
            DisableButtons(false);
            Methods.PlayWinChordProgression(new Random().Next(48, 60));
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

        private void DrawMine(Button btn)
        {
            int row = (int)btn.GetValue(Grid.RowProperty);
            int column = (int)btn.GetValue(Grid.ColumnProperty);
            int cell = minesArray[row, column];
            if (cell == -1)
            {
                Image mineImage = new Image();
                mineImage.Source = bitmapImageMine;
                mineImage.Height = 0.8 * btn.ActualHeight;
                mineImage.Width = 0.8 * btn.ActualWidth;
                mineImage.Stretch = Stretch.UniformToFill;
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
            button.Background = Brushes.LightGray;
            button.HorizontalAlignment = HorizontalAlignment.Stretch;
            button.VerticalAlignment = VerticalAlignment.Stretch;

            button.Foreground = Brushes.Black;
            button.BorderBrush = Brushes.Gray;
            button.BorderThickness = new System.Windows.Thickness(0, 0, 5, 5);

            button.Click += Btn_Click;
            button.MouseRightButtonDown += Btn_RightClick;
            return button;
        }
        private void AddContentToButton(Button btn, int cell)
        {
            if (cell != 0) btn.Content = cell;
            else btn.Content = "";
            byte color = (byte)(255 / ((cell==0) ? 0.2 : cell*2));
            btn.Background = new SolidColorBrush(Color.FromArgb(50, 255, 255, 255));
            btn.Foreground = new SolidColorBrush(Color.FromArgb(255, 20, color, 20));
            btn.FontSize = 18;
           // btn.FontStretch = FontStretches.ExtraExpanded;
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
        private void Btn_RightClick(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn.Content == null)
             {
                if (minesCount == 0 ) { return; } 
                Image flagImage = new Image();
                flagImage.Source = bitmapImageFlag;
                flagImage.HorizontalAlignment = HorizontalAlignment.Stretch;
                btn.Content = flagImage;
                btn.Click -= Btn_Click;
                Methods.PlayNote((byte)(72 - minesCount));
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
        private void BtnRestart_Click(object sender, RoutedEventArgs e)
        {
            Methods.PlayGlissando(new Random().Next(60, 72), 24);
            gridMain.Children.Clear();
            isStarted = false;
            root.Children.Remove(winText);
            StartTheGame();
        }
        private void GridMain_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (isStarted)  return;
            ySize -= e.Delta / 120;
            ySize = (ySize < 4) ? 4 : ySize;
            Methods.PlayGlissandoPentatonic(ySize * 7, (e.Delta > 0 ? 1 : -1 ) * 20, 50);
            gridMain.Children.Clear();
            StartTheGame();
        }
    }
}
