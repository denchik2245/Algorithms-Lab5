using System.Windows;
using Algorithms_Lab5.Tools;
using GraphEditor.Pages;

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
    }
}
