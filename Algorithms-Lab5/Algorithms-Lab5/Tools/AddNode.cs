using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Algorithms_Lab5.Utils;

namespace Algorithms_Lab5.Tools
{
    public class AddNode
    {
        private int _count = 0;
        public bool IsActive { get; set; }
        private readonly GraphData _graphData;

        public AddNode(GraphData graphData)
        {
            _graphData = graphData;
        }

        public Grid Add(Canvas canvas, Point position, string label = null)
        {
            if (!IsActive) return null;

            _count++;

            // Используем заданную метку или внутренний счётчик
            string nodeLabel = label ?? _count.ToString();

            // Создаём эллипс 70x70
            Ellipse ellipse = new Ellipse
            {
                Width = 70,
                Height = 70,
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#478ECC")),
                StrokeThickness = 5
            };

            // Текстовое поле с номером узла
            TextBlock textBlock = new TextBlock
            {
                Text = nodeLabel,
                Foreground = Brushes.White,
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextAlignment = TextAlignment.Center
            };

            // Контейнер для совмещения текста и эллипса
            Grid container = new Grid
            {
                Width = 70,
                Height = 70
            };
            container.Children.Add(ellipse);
            container.Children.Add(textBlock);

            // Устанавливаем позицию (центрируем по заданным координатам)
            Canvas.SetLeft(container, position.X - 35);
            Canvas.SetTop(container, position.Y - 35);

            // Присваиваем метку узла в Tag для дальнейшей идентификации
            container.Tag = nodeLabel;

            // Добавляем обработчик клика для взаимодействия с узлом (например, для добавления рёбер)
            container.MouseLeftButtonDown += (sender, args) =>
            {
                // Можно добавить взаимодействие с AddEdgeTool или другими инструментами
            };

            // Добавляем узел на Canvas
            canvas.Children.Add(container);

            // Добавляем узел в GraphData, передавая Grid для связи
            _graphData.AddNode(nodeLabel, container);

            return container;
        }

        public void ResetCount()
        {
            _count = 0;
        }
    }
}