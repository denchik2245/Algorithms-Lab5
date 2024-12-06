using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Algorithms_Lab5.Tools
{
    public class RemoveNode
    {
        public bool IsActive { get; set; } // Флаг для активации режима удаления узлов

        public void Remove(Canvas canvas, MouseButtonEventArgs e)
        {
            if (!IsActive) return;

            // Получаем позицию клика относительно Canvas
            Point clickPosition = e.GetPosition(canvas);

            // Ищем элемент на позиции клика
            foreach (UIElement element in canvas.Children)
            {
                if (element is Grid grid) // Проверяем, является ли элемент узлом (Grid)
                {
                    // Получаем позицию Grid на Canvas
                    double left = Canvas.GetLeft(grid);
                    double top = Canvas.GetTop(grid);
                    double right = left + grid.ActualWidth;
                    double bottom = top + grid.ActualHeight;

                    // Проверяем, находится ли клик внутри границ Grid
                    if (clickPosition.X >= left && clickPosition.X <= right &&
                        clickPosition.Y >= top && clickPosition.Y <= bottom)
                    {
                        // Удаляем элемент с Canvas
                        canvas.Children.Remove(grid);
                        break;
                    }
                }
            }
        }
    }
}