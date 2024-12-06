using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace Algorithms_Lab5.Tools
{
    public class RemoveNode
    {
        public bool IsActive { get; set; }

        public void Remove(Canvas canvas, MouseButtonEventArgs e)
        {
            if (!IsActive) return;

            // Получаем позицию клика
            Point clickPosition = e.GetPosition(canvas);

            // Узел, который нужно удалить
            Grid nodeToRemove = null;

            // Найдем узел, который был кликнут
            foreach (UIElement element in canvas.Children)
            {
                if (element is Grid grid)
                {
                    double left = Canvas.GetLeft(grid);
                    double top = Canvas.GetTop(grid);
                    double right = left + grid.ActualWidth;
                    double bottom = top + grid.ActualHeight;

                    if (clickPosition.X >= left && clickPosition.X <= right &&
                        clickPosition.Y >= top && clickPosition.Y <= bottom)
                    {
                        nodeToRemove = grid;
                        break;
                    }
                }
            }

            if (nodeToRemove == null) return;

            // Удаляем все ребра, связанные с узлом
            RemoveConnectedEdges(canvas, nodeToRemove);

            // Удаляем сам узел
            canvas.Children.Remove(nodeToRemove);
        }

        private void RemoveConnectedEdges(Canvas canvas, Grid node)
        {
            // Список для хранения ребер, которые нужно удалить
            List<UIElement> edgesToRemove = new List<UIElement>();

            foreach (UIElement element in canvas.Children)
            {
                if (element is Line line && line.Tag is Tuple<Grid, Grid, TextBlock> connectedNodes)
                {
                    // Проверяем, связан ли узел с началом или концом линии
                    if (connectedNodes.Item1 == node || connectedNodes.Item2 == node)
                    {
                        // Добавляем линию в список для удаления
                        edgesToRemove.Add(line);

                        // Удаляем текст веса
                        TextBlock weightTextBlock = connectedNodes.Item3;
                        if (weightTextBlock != null)
                        {
                            edgesToRemove.Add(weightTextBlock);
                        }
                    }
                }
            }

            // Удаляем линии и текстовые блоки из Canvas
            foreach (var edge in edgesToRemove)
            {
                canvas.Children.Remove(edge);
            }
        }
    }
}
