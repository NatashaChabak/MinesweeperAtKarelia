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

        public MainWindow()
        {
            InitializeComponent();

            //initializeArray (2 dimensional) - random b + neighbors
            DrawGrid();
            this.Content = gridMain;

        }

        private void DrawGrid()

        {
            gridMain = new Grid();

            gridMain.ShowGridLines = true;
            for (int i = 0; i < 5; i++)
            {
                gridMain.ColumnDefinitions.Add(new ColumnDefinition());
                gridMain.RowDefinitions.Add(new RowDefinition());
            }

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
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
            button.Click += btnToggleRun_Click; //draw
            button.MouseRightButtonDown += btnSigned;
            return button;
        }

        //public void GameOver()
        //{
        //    Grid gridMain = new Grid();
        //    for (int i = 0; i < 5; i++)
        //    {
        //        for (int j = 0; j < 5; j++)
        //        {
        //            DrawCell(i, j);
        //        }
        //    }
        //}

        //public int DrawCell(Button btn, int x, int y)
        //{
        //    //get from Array
        //    return valueFromArray;
        //}

        public void btnSigned(object sender, RoutedEventArgs e)
        {
            // get rid of the star ?
              (sender as Button).Content = "*";
            //  btn.Click -= btnToggleRun_Click; if star=true
        }

        private void btnToggleRun_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;

            int _row = (int)btn.GetValue(Grid.RowProperty);
            int _column = (int)btn.GetValue(Grid.ColumnProperty);

            //btn.Content = drawCell(btn, _row, _column);
            btn.Content = _row + " " + _column;
            btn.Click -= btnToggleRun_Click;
            btn.MouseRightButtonDown -= btnSigned;

        }

    }
}
