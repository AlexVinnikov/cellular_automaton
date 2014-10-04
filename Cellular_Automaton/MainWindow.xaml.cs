using System;
using System.Windows;

using System.Threading;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Shapes;

namespace Cellular_Automaton
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool action = false;
        ThreadStart ts;
        Thread t;

        int generation = 0;
        int cellSize = 2;
        int universeSize = 500;

        bool[,] _universe;
        bool[,] _universeBuffer;

        SolidColorBrush myBrush;

        WriteableBitmap bitmap;
        RenderTargetBitmap bmp;

        DispatcherTimer dispatcherTimer;

        public MainWindow()
        {
            InitializeComponent();
            txt_cellSize.Text = "20";
            txt_universeSize.Text = "500 (не работает)";
            txt_generations.Text = "Нет ни одного поколения";
        }

        /// <summary>
        /// Создает начальную вселенную клеток и запускает автомат
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_fill_Click(object sender, RoutedEventArgs e)
        {
            DrawingVisual t = new DrawingVisual();
            DrawingContext context = t.RenderOpen();

            int countCells = universeSize/cellSize;

            _universe = new bool[countCells, countCells];
            _universeBuffer = new bool[countCells, countCells];

            bitmap = new WriteableBitmap(universeSize,
               universeSize, 96, 96, PixelFormats.Bgra32, null);

            Random rand = new Random();
            for (int j = 0; j < countCells; j++)
            {
                for (int i = 0; i < countCells; i++)
                {
                    Rect rect = new Rect();
                    rect.Location = new Point(i * cellSize, j * cellSize);
                    rect.Size = new Size(cellSize, cellSize);

                    int r = rand.Next(1,999);
                    //if(r % 2 == 0)
                    //if ((j + i) % 2 == 0)
                    if ((i == j) || ((countCells - 1 - j) == i))
                    {

                        _universe[i, j] = true;
                        context.DrawRectangle(Brushes.Black, null, rect);
                    }
                    else
                    {
                        _universe[i, j] = false;
                        context.DrawRectangle(Brushes.White, null, rect);
                    }
                }
            }
            context.Close();
            RenderTargetBitmap bmp = new RenderTargetBitmap(universeSize, universeSize, 0, 0, PixelFormats.Pbgra32);
            bmp.Render(t);
            mainImage.Source = bmp;
        }


        /// <summary>
        /// Рисует клетку
        /// </summary>
        /// <param name="canvas"> UIElement Canvas</param>
        /// <param name="x">отступ по x</param>
        /// <param name="y">отступ по y</param>
        public void DrawCell(Image image, int x, int y, Color color)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                int blue = 0;
                int green = 0;
                int red = 0;
                int alpha = 255;

                if (color == Colors.White)
                {
                    blue = 255;
                    green = 255;
                    red = 255;
                }

                byte[] colorData = { (byte)blue, (byte)green, (byte)red, (byte)alpha };

                for (int i = 0; i < cellSize; i++)
                {
                    for (int j = 0; j < cellSize; j++)
                    {
                        Int32Rect rect = new Int32Rect(x + i, y + j, 1, 1);
                        int stride = (bitmap.PixelWidth * bitmap.Format.BitsPerPixel) / 8;
                        bitmap.WritePixels(rect, colorData, stride, 0);
                    }
                }


                image.Source = bitmap;
            }));


        }

        public void DrawCell(DrawingContext context, int x, int y, SolidColorBrush brush)
        {
            Rect r = new Rect();
            r.Location = new Point(x * cellSize, y * cellSize);
            r.Size = new Size(cellSize, cellSize);

            if (brush == Brushes.Black)
            {
                context.DrawRectangle(Brushes.Black, null, r);
            }
            else
            {
                context.DrawRectangle(Brushes.White, null, r);
            }

            
        }

        public void DrawCell(Canvas canvas, int x, int y, Color color)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                Rectangle rectangle = new Rectangle();
                rectangle.Width = cellSize;
                rectangle.Height = cellSize;
                // Create a SolidColorBrush and use it to
                // paint the rectangle.
                myBrush = new SolidColorBrush(Colors.Black);
                rectangle.Stroke = Brushes.Black;

                if (color == Colors.White)
                {
                    myBrush = new SolidColorBrush(Colors.White);
                    rectangle.Stroke = Brushes.White;
                }

                Canvas.SetLeft(rectangle, (double)x);
                Canvas.SetTop(rectangle, (double)y);

                rectangle.StrokeThickness = 1;
                rectangle.Fill = myBrush;

                canvas.Children.Add(rectangle);
            }));
        }
        /// <summary>
        /// Проверяет всех соседей клетки и решает, останется ли она жить на следующем шаге
        /// </summary>
        /// <param name="array">массив клеток</param>
        /// <param name="x">отступ по x</param>
        /// <param name="y">отступ по y</param>
        /// <returns>будет ли жива клетка на следующем шаге</returns>
        public bool CheckNeighbors(bool[,] array, int x, int y)
        {
            //количество живых соседей
            int alive = 0;
            //+--
            //-#-
            //---
            if (x - 1 >= 0 && y - 1 >= 0)
            {
                if (array[x - 1, y - 1] == true)
                {
                    alive++;
                }
            }

            //-+-
            //-#-
            //---
            if (x - 1 >= 0)
            {
                if (array[x - 1, y] == true)
                {
                    alive++;
                }
            }

            //--+
            //-#-
            //---
            if (x - 1 >= 0 && y + 1 < array.GetLength(1))
            {
                if (array[x - 1, y + 1] == true)
                {
                    alive++;
                }
            }

            //---
            //+#-
            //---
            if (y - 1 >= 0)
            {
                if (array[x, y - 1] == true)
                {
                    alive++;
                }
            }

            //---
            //-#+
            //---
            if (y + 1 < array.GetLength(1))
            {
                if (array[x, y + 1] == true)
                {
                    alive++;
                }
            }

            //---
            //-#-
            //+--
            if (x + 1 < array.GetLength(0) && y - 1 >= 0)
            {
                if (array[x + 1, y - 1] == true)
                {
                    alive++;
                }
            }

            //---
            //-#-
            //-+-
            if (x + 1 < array.GetLength(0))
            {
                if (array[x + 1, y] == true)
                {
                    alive++;
                }
            }

            //---
            //-#-
            //--+
            if (x + 1 < array.GetLength(0) && y + 1 < array.GetLength(1) && array[x + 1, y + 1] == true)
            {
                if (array[x + 1, y + 1] == true)
                {
                    alive++;
                }
            }

            //*******************
            //После того, как мы узнали сколько клетка имеет живых соседей, 
            //мы определяем останется ли она жива
            //*******************
            if ((alive < 2) || (alive > 3))
            {
                return false;
            }
            else
            {
                if ((!array[x, y]) && (alive != 3))
                {
                    return false;

                }
                else
                {
                    return true;
                }
            }

        }

        private void btn_start_Click(object sender, RoutedEventArgs e)
        {
            if (_universe != null)
            {
                dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
                dispatcherTimer.Tick += new EventHandler(RenderFrameBmp);
                dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 200);
                dispatcherTimer.Start();
            }
        }

        private void RenderFrameBmp(object sender, EventArgs e)
        {
            if (_universe != null)
            {
                generation++;

                DrawingVisual t = new DrawingVisual();
                DrawingContext context = t.RenderOpen();

                int countCells = universeSize / cellSize;
                action = true;
                
                for (int j = 0; j < countCells; j++)
                {
                    for (int i = 0; i < countCells; i++)
                    {
                        Rect rect = new Rect();
                        rect.Location = new Point(i * cellSize, j * cellSize);
                        rect.Size = new Size(cellSize, cellSize);

                        if (CheckNeighbors(_universe, j, i))
                        {
                            _universeBuffer[i, j] = true;
                            context.DrawRectangle(Brushes.Black, null, rect);
                        }
                        else
                        {
                            _universeBuffer[i, j] = false;
                            context.DrawRectangle(Brushes.White, null, rect);
                        }
                    }
                
                  }
                context.Close();
                bmp = new RenderTargetBitmap(universeSize, universeSize, 0, 0, PixelFormats.Pbgra32);
                bmp.Render(t);
                mainImage.Source = bmp;
                txt_generations.Text = generation.ToString();

                for (int j = 0; j < countCells; j++)
                {
                    for (int i = 0; i < countCells; i++)
                    {
                        _universe[i, j] = _universeBuffer[i, j];
                    }
                }
            }

        }

        private void RenderFrame(object sender, EventArgs e)
        {
            if (_universe != null)
            {
                int countCells = universeSize / cellSize;
                action = true;
                for (int j = 0; j < countCells; j++)
                {
                    for (int i = 0; i < countCells; i++)
                    {
                        if (CheckNeighbors(_universe, j, i))
                        {
                            _universeBuffer[i, j] = true;
                            DrawCell(mainImage, i * cellSize, j * cellSize, Colors.Black);
                        }
                        else
                        {
                            _universeBuffer[i, j] = false;
                            DrawCell(mainImage, i * cellSize, j * cellSize, Colors.White);
                        }
                    }
                }

                for (int j = 0; j < countCells; j++)
                {
                    for (int i = 0; i < countCells; i++)
                    {
                        _universe[i, j] = _universeBuffer[i, j];
                    }
                }
            }
        }

        private void btn_stop_Click(object sender, RoutedEventArgs e)
        {

            bmp.Clear();
            mainImage.Source = bmp;
            _universe = null;

            generation = 0;
            dispatcherTimer.Stop();
        }

        private void btn_pause_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Stop();
        }

        private void btn_fill_Click_1(object sender, RoutedEventArgs e)
        {
            DrawingVisual t = new DrawingVisual();
            DrawingContext context = t.RenderOpen();

            int countCells = universeSize/cellSize;

            _universe = new bool[countCells, countCells];
            _universeBuffer = new bool[countCells, countCells];

            for (int j = 0; j < countCells; j++)
            {
                for (int i = 0; i < countCells; i++)
                {
                    _universe[i, j] = false;
                }
            }

            _universe[2, 1] = true;
            _universe[3, 2] = true;
            _universe[3, 3] = true;
            _universe[2, 3] = true;
            _universe[1, 3] = true;


            Random rand = new Random();
            for (int j = 0; j < countCells; j++)
            {
                for (int i = 0; i < countCells; i++)
                {
                    Rect rect = new Rect();
                    rect.Location = new Point(i * cellSize, j * cellSize);
                    rect.Size = new Size(cellSize, cellSize);

                    if (_universe[i, j])
                    {
                        context.DrawRectangle(Brushes.Black, null, rect);
                    }
                    else
                    {
                        context.DrawRectangle(Brushes.White, null, rect);
                    }
                }
            }
            context.Close();
            bmp = new RenderTargetBitmap(universeSize, universeSize, 0, 0, PixelFormats.Pbgra32);
            bmp.Render(t);
            mainImage.Source = bmp;
        }

        private void txt_cellSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            cellSize = Convert.ToInt32(txt_cellSize.Text);
        }

    }
}
