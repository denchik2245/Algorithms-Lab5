using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.Generic;
using Algorithms_Lab5.Utils;

namespace Algorithms_Lab5.Tools
{
   public class RemoveNode
    {
        public bool IsActive { get; set; }
        private readonly GraphData _graphData;

        public RemoveNode(GraphData graphData)
        {
            _graphData = graphData;
        }

        public void Remove(Canvas canvas, MouseButtonEventArgs e)
        {
            if (!IsActive) return;

            // Получаем позицию клика
            Point clickPosition = e.GetPosition(canvas);

            // Узел для удаления
            Grid nodeToRemove = null;

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

            if (nodeToRemove == null)
            {
                MessageBox.Show("Узел для удаления не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Удаление всех рёбер, связанных с узлом
            RemoveConnectedEdges(canvas, nodeToRemove);

            // Удаляем узел из GraphData
            if (nodeToRemove.Tag is string nodeLabel)
            {
                _graphData.RemoveNode(nodeLabel);
            }

            // Удаляем узел визуально
            canvas.Children.Remove(nodeToRemove);
        }

        private void RemoveConnectedEdges(Canvas canvas, Grid node)
        {
            // Список для удаления рёбер и их текстовых блоков
            var elementsToRemove = new List<UIElement>();

            // Перебираем Canvas.Children, добавляя элементы для удаления в список
            foreach (UIElement element in canvas.Children.OfType<UIElement>().ToList()) // Создаём временный список
            {
                if (element is Line line && line.Tag is Tuple<Grid, Grid, TextBlock> connectedNodes)
                {
                    // Если узел связан с началом или концом рёбра
                    if (connectedNodes.Item1 == node || connectedNodes.Item2 == node)
                    {
                        // Добавляем линию в список для удаления
                        elementsToRemove.Add(line);

                        // Добавляем текст веса, если он есть
                        if (connectedNodes.Item3 is TextBlock weightTextBlock)
                        {
                            elementsToRemove.Add(weightTextBlock);
                        }

                        // Удаляем ребро из GraphData
                        string firstLabel = connectedNodes.Item1.Tag as string;
                        string secondLabel = connectedNodes.Item2.Tag as string;

                        if (!string.IsNullOrEmpty(firstLabel) && !string.IsNullOrEmpty(secondLabel))
                        {
                            _graphData.RemoveEdge(firstLabel, secondLabel);
                        }
                    }
                }
            }

            // Удаляем элементы из Canvas вне итерации
            foreach (var element in elementsToRemove)
            {
                canvas.Children.Remove(element);
            }
        }
    }

}
