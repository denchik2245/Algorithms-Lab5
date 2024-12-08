using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Algorithms_Lab5.Utils;

namespace Algorithms_Lab5.Tools
{
    public class AddEdge
    {
        private Grid _firstNode;
        private bool _isSelectingFirstNode = true;
        public bool IsActive { get; set; }
        private readonly GraphData _graphData;

        public AddEdge(GraphData graphData)
        {
            _graphData = graphData;
        }

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
                Point firstNodeCenter = GetNodeCenter(_firstNode);
                Point secondNodeCenter = GetNodeCenter(node);

                const double nodeRadius = 35;

                Point startPoint = GetPointOnCircleBorder(firstNodeCenter, secondNodeCenter, nodeRadius);
                Point endPoint = GetPointOnCircleBorder(secondNodeCenter, firstNodeCenter, nodeRadius);

                string firstLabel = (string)_firstNode.Tag;
                string secondLabel = (string)node.Tag;

                // Создаём TextBlock для отображения веса
                TextBlock weightText = CreateWeightTextBlock("0", canvas);

                // Рисуем ребро с весом "0" и добавляем в GraphData
                Line edge = DrawEdgeWithWeight(canvas, _firstNode, node, weightText);
                _graphData.AddEdge(firstLabel, secondLabel, 0, edge, weightText);

                _firstNode = null;
                _isSelectingFirstNode = true;
            }
        }

        private Point GetNodeCenter(Grid node)
        {
            double left = Canvas.GetLeft(node);
            double top = Canvas.GetTop(node);
            double centerX = left + 35;
            double centerY = top + 35;
            return new Point(centerX, centerY);
        }

        private Point GetPointOnCircleBorder(Point circleCenter, Point targetPoint, double radius)
        {
            double dx = targetPoint.X - circleCenter.X;
            double dy = targetPoint.Y - circleCenter.Y;
            double distance = Math.Sqrt(dx * dx + dy * dy);

            if (distance == 0) return circleCenter;

            double normalizedX = dx / distance;
            double normalizedY = dy / distance;

            double borderX = circleCenter.X + normalizedX * radius;
            double borderY = circleCenter.Y + normalizedY * radius;

            return new Point(borderX, borderY);
        }

        public Line DrawEdgeWithWeight(Canvas canvas, Grid firstNode, Grid secondNode, TextBlock weightText)
        {
            Point firstNodeCenter = GetNodeCenter(firstNode);
            Point secondNodeCenter = GetNodeCenter(secondNode);

            const double nodeRadius = 35;

            Point startPoint = GetPointOnCircleBorder(firstNodeCenter, secondNodeCenter, nodeRadius);
            Point endPoint = GetPointOnCircleBorder(secondNodeCenter, firstNodeCenter, nodeRadius);

            // Создаём линию для рёбра
            Line edge = new Line
            {
                X1 = startPoint.X,
                Y1 = startPoint.Y,
                X2 = endPoint.X,
                Y2 = endPoint.Y,
                Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2A2D32")),
                StrokeThickness = 5
            };

            // Устанавливаем Tag у Line для связи с TextBlock и узлами
            edge.Tag = Tuple.Create(firstNode, secondNode, weightText);

            double midX = (startPoint.X + endPoint.X) / 2;
            double midY = (startPoint.Y + endPoint.Y) / 2;
            Canvas.SetLeft(weightText, midX);
            Canvas.SetTop(weightText, midY);

            // Добавляем элементы на Canvas
            canvas.Children.Add(edge);
            canvas.Children.Add(weightText);

            return edge;
        }

        public TextBlock CreateWeightTextBlock(string weight, Canvas canvas)
        {
            TextBlock weightText = new TextBlock
            {
                Text = weight,
                Foreground = Brushes.Black,
                Background = Brushes.White,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Padding = new Thickness(2)
            };

            weightText.MouseLeftButtonDown += (sender, args) =>
            {
                string input = Microsoft.VisualBasic.Interaction.InputBox("Введите новый вес ребра:", "Изменить вес", weightText.Text);
                if (double.TryParse(input, out double newWeight) && newWeight >= 0)
                {
                    weightText.Text = newWeight.ToString(CultureInfo.InvariantCulture);

                    // Обновляем вес в GraphData
                    string firstLabel = (string)_firstNode.Tag;
                    string secondLabel = (string)((Grid)canvas.Children.Cast<UIElement>().FirstOrDefault(e => e is Grid && Canvas.GetLeft((Grid)e) == Canvas.GetLeft(weightText))).Tag;
                    _graphData.AddEdge(firstLabel, secondLabel, newWeight, null, weightText);
                }
                else
                {
                    MessageBox.Show("Некорректное значение веса. Введите неотрицательное число.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            return weightText;
        }
    }
}
