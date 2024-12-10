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
            
            Canvas.SetLeft(container, position.X - 35);
            Canvas.SetTop(container, position.Y - 35);
            
            container.Tag = nodeLabel;
            
            // Обновленный обработчик клика
            container.MouseLeftButtonDown += (sender, args) =>
            {
                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    string nodeLabel = container.Tag.ToString();
                    mainWindow.SelectNode(nodeLabel);
                }
            };
            
            canvas.Children.Add(container);
            _graphData.AddNode(nodeLabel, container);

            return container;
        }

        public void ResetCount()
        {
            _count = 0;
        }
    }
}