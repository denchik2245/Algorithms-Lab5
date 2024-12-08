using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Algorithms_Lab5.Utils;

namespace Algorithms_Lab5.Tools
{
    public class RemoveEdge
    {
        public bool IsActive { get; set; }
        private readonly GraphData _graphData;

        public RemoveEdge(GraphData graphData)
        {
            _graphData = graphData;
        }

        public void Remove(Canvas canvas, MouseButtonEventArgs e)
        {
            if (!IsActive) return;

            HitTestResult hitResult = VisualTreeHelper.HitTest(canvas, e.GetPosition(canvas));
            if (hitResult?.VisualHit is Line edge)
            {
                if (edge.Tag is Tuple<Grid, Grid, TextBlock> taggedNodesWithWeight)
                {
                    Grid firstNode = taggedNodesWithWeight.Item1;
                    Grid secondNode = taggedNodesWithWeight.Item2;
                    TextBlock weightTextBlock = taggedNodesWithWeight.Item3;

                    string firstLabel = firstNode.Tag as string;
                    string secondLabel = secondNode.Tag as string;

                    // Удаляем ребро из GraphData
                    if (firstLabel != null && secondLabel != null)
                    {
                        _graphData.RemoveEdge(firstLabel, secondLabel);
                    }

                    // Удаляем текстовый блок веса
                    if (weightTextBlock != null)
                    {
                        canvas.Children.Remove(weightTextBlock);
                    }
                }

                // Удаляем линию (ребро) из Canvas
                canvas.Children.Remove(edge);
            }
            else
            {
                MessageBox.Show("Ребро для удаления не найдено.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}