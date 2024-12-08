using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Algorithms_Lab5.Utils;

namespace Algorithms_Lab5.Tools
{
    public class Move
    {
        private bool _isDragging = false;
        private Point _startPoint;
        private Grid _selectedNode;
        private Canvas _canvas;
        private Point _virtualPosition;
        private readonly GraphData _graphData;

        public bool IsActive { get; set; }

        public Move(GraphData graphData)
        {
            _graphData = graphData;
        }

        public void Initialize(Canvas canvas)
        {
            _canvas = canvas;
        }

        public void StartDrag(MouseButtonEventArgs e)
        {
            if (!IsActive || _canvas == null) return;

            HitTestResult hitResult = VisualTreeHelper.HitTest(_canvas, e.GetPosition(_canvas));
            if (hitResult?.VisualHit is Ellipse && VisualTreeHelper.GetParent(hitResult.VisualHit) is Grid grid)
            {
                _isDragging = true;
                _selectedNode = grid;
                _startPoint = e.GetPosition(_canvas);

                // Повышаем ZIndex выбранного узла
                Canvas.SetZIndex(_selectedNode, 10);
            }
        }

        public void Drag(MouseEventArgs e)
        {
            if (_isDragging && _selectedNode != null)
            {
                Point currentPoint = e.GetPosition(_canvas);

                double offsetX = currentPoint.X - _startPoint.X;
                double offsetY = currentPoint.Y - _startPoint.Y;

                // Виртуальная позиция узла
                _virtualPosition = new Point(Canvas.GetLeft(_selectedNode) + offsetX, Canvas.GetTop(_selectedNode) + offsetY);

                // Обновляем реальную позицию узла
                Canvas.SetLeft(_selectedNode, _virtualPosition.X);
                Canvas.SetTop(_selectedNode, _virtualPosition.Y);

                // Обновляем рёбра сразу же, чтобы они двигались вместе с узлом
                UpdateEdges(_selectedNode, _virtualPosition.X, _virtualPosition.Y);

                // Обновляем стартовую точку для дальнейших перемещений
                _startPoint = currentPoint;
            }
        }

        public void EndDrag()
        {
            if (_isDragging)
            {
                _isDragging = false;

                // Обновляем координаты узла в GraphData
                if (_selectedNode != null && _selectedNode.Tag is string nodeLabel)
                {
                    double newX = _virtualPosition.X + (_selectedNode.ActualWidth / 2);
                    double newY = _virtualPosition.Y + (_selectedNode.ActualHeight / 2);
                    _graphData.UpdateNodePosition(nodeLabel, newX, newY);
                }

                // Обновляем рёбра, чтобы они не накладывались на узлы
                UpdateEdges(_selectedNode, _virtualPosition.X, _virtualPosition.Y);

                // Возвращаем ZIndex узла к исходному значению
                Canvas.SetZIndex(_selectedNode, 1);

                _selectedNode = null;
            }
        }

        public void UpdateEdges(Grid node, double newLeft, double newTop)
        {
            // Получаем новый центр перемещаемого узла
            double newCenterX = newLeft + (node.ActualWidth / 2);
            double newCenterY = newTop + (node.ActualHeight / 2);

            // Перебираем все элементы на Canvas
            foreach (UIElement element in _canvas.Children)
            {
                if (element is Line line)
                {
                    if (line.Tag is Tuple<Grid, Grid, TextBlock> connectedNodes)
                    {
                        Grid startNode = connectedNodes.Item1;
                        Grid endNode = connectedNodes.Item2;

                        // Если это ребро связано с перемещаемым узлом, обновляем его координаты
                        if (startNode == node || endNode == node)
                        {
                            // Добавляем небольшой отступ, чтобы ребра не накладывались на узлы
                            double offset = 5; // Небольшой отступ от центра узла, чтобы избежать наложений

                            if (startNode == node)
                            {
                                line.X1 = newCenterX + offset;
                                line.Y1 = newCenterY + offset;
                            }

                            if (endNode == node)
                            {
                                line.X2 = newCenterX + offset;
                                line.Y2 = newCenterY + offset;
                            }

                            // Обновляем положение текстового блока (веса ребра)
                            TextBlock weightTextBlock = connectedNodes.Item3;
                            Canvas.SetLeft(weightTextBlock, (line.X1 + line.X2) / 2 - weightTextBlock.ActualWidth / 2);
                            Canvas.SetTop(weightTextBlock, (line.Y1 + line.Y2) / 2 - weightTextBlock.ActualHeight / 2);
                        }
                    }
                }
            }
        }
    }

}
