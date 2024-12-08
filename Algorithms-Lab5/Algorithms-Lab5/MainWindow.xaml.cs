using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using Algorithms_Lab5;
using Algorithms_Lab5._1task;
using Algorithms_Lab5.Tools;

namespace GraphEditor
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            GraphFrame.Content = new PageTask1();
        }

        private void MoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (GraphFrame.Content is PageTask1 pageTask1)
            {
                pageTask1.ActivateMoveMode();
            }
        }

        private void AddNodeButton_Click(object sender, RoutedEventArgs e)
        {
            if (GraphFrame.Content is PageTask1 pageTask1)
            {
                pageTask1.ActivateAddNodeMode();
            }
        }

        private void DeleteNodeButton_Click(object sender, RoutedEventArgs e)
        {
            if (GraphFrame.Content is PageTask1 pageTask1)
            {
                pageTask1.ActivateRemoveNodeMode();
            }
        }

        private void AddEdgeButton_Click(object sender, RoutedEventArgs e)
        {
            if (GraphFrame.Content is PageTask1 pageTask1)
            {
                pageTask1.ActivateAddEdgeMode();
            }
        }

        private void DeleteEdgeButton_Click(object sender, RoutedEventArgs e)
        {
            if (GraphFrame.Content is PageTask1 pageTask1)
            {
                pageTask1.ActivateRemoveEdgeMode();
            }
        }

        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            if (GraphFrame.Content is PageTask1 pageTask1)
            {
                try
                {
                    pageTask1.LoadGraph();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке графа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Граф отсутствует на текущей странице.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (GraphFrame.Content is PageTask1 pageTask1)
            {
                var saveGraphTool = new SaveGraph();
                saveGraphTool.Save(pageTask1.MainCanvas);
            }
            else
            {
                MessageBox.Show("Граф отсутствует на текущей странице.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private async void ExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            if (GraphFrame.Content is PageTask1 pageTask1)
            {
                // Предположим, что вы хотите начать обход с узла под номером 1 (Label = "1")
                string startNodeLabel = "1";

                var bfs = new BreadthFirstSearch(OutputTextBox);
                OutputTextBox.Clear();

                // Запускаем обход, используя GraphData из GraphManager
                await bfs.Execute(pageTask1.GraphManager.GraphData, startNodeLabel);
            }
            else
            {
                MessageBox.Show("Граф отсутствует на текущей странице.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
    }
}
