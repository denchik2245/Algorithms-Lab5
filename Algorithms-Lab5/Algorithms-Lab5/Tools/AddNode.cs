using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GraphEditor.Tools
{
    public class AddNode
    {
        private int _count = 0;
        public bool IsActive { get; set; }

        public void Add(Canvas canvas, Point position)
        {
            if (!IsActive) return;

            _count++;

            // Создаём эллипс 70x70
            Ellipse ellipse = new Ellipse
            {
                Width = 70,
                Height = 70,
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#478ECC"))
            };

            // Текстовое поле с номером узла
            TextBlock textBlock = new TextBlock
            {
                Text = _count.ToString(),
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

            // Устанавливаем позицию (центрируем по клику)
            Canvas.SetLeft(container, position.X - 35);
            Canvas.SetTop(container, position.Y - 35);

            canvas.Children.Add(container);
        }
    }
}