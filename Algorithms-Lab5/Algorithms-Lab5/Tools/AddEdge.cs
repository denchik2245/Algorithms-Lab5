using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Algorithms_Lab5.Tools
{
    public class AddEdge
    {
        private Grid _firstNode;
        private bool _isSelectingFirstNode = true;

        public bool IsActive { get; set; }

        public void SelectNode(Canvas canvas, UIElement clickedElement)
        {
            if (!IsActive || clickedElement is not Grid node) return;

            if (_isSelectingFirstNode)
            {
                _firstNode = node;
                _isSelectingFirstNode = false;
            }
            else
            {
                Point firstNodeCenter = GetNodeCenter(canvas, _firstNode);
                Point secondNodeCenter = GetNodeCenter(canvas, node);

                // Учитываем радиус круга (в данном случае 35, так как круг 70x70)
                const double nodeRadius = 35;
                
                Point startPoint = GetPointOnCircleBorder(firstNodeCenter, secondNodeCenter, nodeRadius);
                Point endPoint = GetPointOnCircleBorder(secondNodeCenter, firstNodeCenter, nodeRadius);
                
                DrawEdgeWithWeight(canvas, startPoint, endPoint, _firstNode, node);
                
                _firstNode = null;
                _isSelectingFirstNode = true;
            }
        }

        private Point GetNodeCenter(Canvas canvas, Grid node)
        {
            // Получаем центр узла
            double left = Canvas.GetLeft(node);
            double top = Canvas.GetTop(node);
            double centerX = left + (node.ActualWidth / 2);
            double centerY = top + (node.ActualHeight / 2);
            return new Point(centerX, centerY);
        }

        private Point GetPointOnCircleBorder(Point circleCenter, Point targetPoint, double radius)
        {
            double dx = targetPoint.X - circleCenter.X;
            double dy = targetPoint.Y - circleCenter.Y;
            double distance = System.Math.Sqrt(dx * dx + dy * dy);
            
            double normalizedX = dx / distance;
            double normalizedY = dy / distance;
            
            double borderX = circleCenter.X + normalizedX * radius;
            double borderY = circleCenter.Y + normalizedY * radius;

            return new Point(borderX, borderY);
        }

        public void DrawEdgeWithWeight(Canvas canvas, Point startPoint, Point endPoint, Grid firstNode, Grid secondNode)
        {
            // Создаем линию между двумя точками
            Line edge = new Line
            {
                X1 = startPoint.X,
                Y1 = startPoint.Y,
                X2 = endPoint.X,
                Y2 = endPoint.Y,
                Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2A2D32")),
                StrokeThickness = 5
            };

            // Создаем текстовый блок для веса ребра
            TextBlock weightText = new TextBlock
            {
                Text = "0", // По умолчанию вес = 0
                Foreground = Brushes.Black,
                Background = Brushes.White,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Padding = new Thickness(2)
            };
            
            Canvas.SetLeft(weightText, (startPoint.X + endPoint.X) / 2 - weightText.ActualWidth / 2);
            Canvas.SetTop(weightText, (startPoint.Y + endPoint.Y) / 2 - weightText.ActualHeight / 2);
            
            edge.Tag = new Tuple<Grid, Grid, TextBlock>(firstNode, secondNode, weightText);
            
            weightText.MouseLeftButtonDown += (sender, args) =>
            {
                string input = Microsoft.VisualBasic.Interaction.InputBox("Введите новый вес ребра:", "Изменить вес", weightText.Text);
                if (int.TryParse(input, out int newWeight))
                {
                    weightText.Text = newWeight.ToString();
                }
            };
            
            canvas.Children.Add(edge);
            canvas.Children.Add(weightText);
        }
    }
}