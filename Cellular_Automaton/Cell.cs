using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Cellular_Automaton
{
    public class Cell
    {
        public Point _point { get; private set; }
        public SolidColorBrush _color { get; private set; }
        public int _size { get; private set; }

        public Cell(Point point, SolidColorBrush color, int size)
        {
            _point = point;
            _color = color;
            _size = size;
        }

        public void DrawCell(Canvas canvas)
        {
            Rectangle rect = new Rectangle
            {
                Width = _size,
                Height = _size,
                Fill = _color,
                Stroke = _color,
            };

            Canvas.SetLeft(rect, _point.X);
            Canvas.SetTop(rect, _point.Y);

            canvas.Children.Add(rect);
        }

        /// <summary>
        /// Проверяет соседей клетки
        /// </summary>
        /// <returns>жива ли клетка на следующем ходе</returns>
        public bool Alive( Canvas canvas)
        {
            //количество живых соседей
            int alive = 0;
            //+--
            //-#-
            //---
            if (this._point.X - 1 >= 0 && this._point.Y - 1 >= 0)
            {
                alive++;
            }

            //-+-
            //-#-
            //---
            if (this._point.Y - 1 >= 0)
            {
                alive++;
            }

            //--+
            //-#-
            //---
            if (this._point.X + 1 < canvas.Width && this._point.Y - 1 >= 0)
            {
                alive++;
            }

            //---
            //+#-
            //---
            if (this._point.X - 1 >= 0)
            {
                alive++;
            }

            //---
            //-#+
            //---
            if (this._point.X + 1 < canvas.Width)
            {
                alive++;
            }

            //---
            //-#-
            //+--
            if (this._point.X - 1 >= 0 && this._point.Y + 1 < canvas.Height)
            {
                alive++;
            }

            //---
            //-#-
            //-+-
            if (this._point.Y + 1 < canvas.Height)
            {
                alive++;
            }

            //---
            //-#-
            //--+
            if (this._point.X + 1 <canvas.Width && this._point.Y + 1 < canvas.Height)
            {
                alive++;
            }

            //*******************
            //После того, как мы узнали сколько клетка имеет живых соседей, 
            //мы изменяем или нет ее статус во временном массиве
            //*******************
            if ((alive < 2) || (alive > 3))
            {
                return false;
            }
            else
            {
                if (alive != 3)
                {
                    return false;

                }
                else
                {
                    return true;
                }
            }

        }
    }
}
