using System.Windows;
using System.Windows.Controls;
using Algorithms_Lab5.Algorithms;
using Algorithms_Lab5.Tools;
using Algorithms_Lab5.Utils;

namespace Algorithms_Lab5
{
    public partial class MainWindow : Window
    {
        public GraphManager GraphManager { get; private set; }
        public string SelectedStartNode { get; private set; } = null;
        public string SelectedEndNode { get; private set; } = null;
        private bool isSelectingStartNode = false;
        private bool isSelectingEndNode = false;
        private AddDirectedEdge _addDirectedEdge;
        
        public MainWindow()
        {
            InitializeComponent();
            GraphFrame.Content = new PageTask1();
            GraphManager = new GraphManager();
            
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

        private void AddDirectedEdgeButton_Click(object sender, RoutedEventArgs e)
        {
            if (GraphFrame.Content is PageTask1 pageTask1)
            {
                pageTask1.ActivateAddDirectedEdgeMode();
            }
        }
        
        private void DeleteEdgeButton_Click(object sender, RoutedEventArgs e)
        {
            if (GraphFrame.Content is PageTask1 pageTask1)
            {
                pageTask1.ActivateRemoveEdgeMode();
            }
        }
        
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            if (GraphFrame.Content is PageTask1 pageTask1)
            {
                var clearTool = new Clear(pageTask1.MainCanvas);
                clearTool.ClearCanvas();
            }
            else
            {
                MessageBox.Show("Полотно отсутствует на текущей странице.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
        
        //Кнопка "Сохранить"
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
        
        private void AlgorithmComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AlgorithmComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedAlgorithm = selectedItem.Content.ToString();

                if (GraphFrame.Content is PageTask1 pageTask1)
                {
                    if (selectedAlgorithm == "Транспортная сеть")
                    {
                        AddEdgeButton.Visibility = Visibility.Collapsed;
                        AddDirectedEdgeButton.Visibility = Visibility.Visible;
                        pageTask1.GraphManager.AddDirectedEdgeTool.SetTransportNetworkMode(true);
                    }
                    else
                    {
                        AddEdgeButton.Visibility = Visibility.Visible;
                        AddDirectedEdgeButton.Visibility = Visibility.Collapsed;
                        pageTask1.GraphManager.AddDirectedEdgeTool.SetTransportNetworkMode(false);
                    }
                }
            }
        }
        
        //Кнопка "Выполнить"
        private async void ExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            if (GraphFrame.Content is PageTask1 pageTask1)
            {
                pageTask1.NonActive();
                string selectedAlgorithm = (AlgorithmComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                if (string.IsNullOrEmpty(selectedAlgorithm))
                {
                    MessageBox.Show("Пожалуйста, выберите алгоритм.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                OutputTextBox.Clear();

                switch (selectedAlgorithm)
                {
                    case "Обход в ширину":
                        await SelectStartNodeAndExecute(async (start) =>
                        {
                            var bfs = new BreadthFirstSearch(OutputTextBox);
                            await bfs.Execute(pageTask1.GraphManager.GraphData, start);
                        });
                        break;

                    case "Обход в глубину":
                        await SelectStartNodeAndExecute(async (start) =>
                        {
                            var dfs = new DepthFirstSearch(OutputTextBox);
                            await dfs.Execute(pageTask1.GraphManager.GraphData, start);
                        });
                        break;

                    case "Минимальное остовное дерево":
                        await SelectStartNodeAndExecute(async (start) =>
                        {
                            var mst = new MinimumSpanningTree(OutputTextBox);
                            mst.SetSelectedStartNode(start);
                            await mst.Execute(pageTask1.GraphManager.GraphData);
                        });
                        break;

                    case "Кратчайший путь":
                        await SelectStartAndEndNodesAndExecute(async (start, end) =>
                        {
                            var shortestPath = new DijkstraShortestPath(OutputTextBox);
                            await shortestPath.Execute(pageTask1.GraphManager.GraphData, start, end);
                        });
                        break;

                    case "Транспортная сеть":
                        await SelectStartAndEndNodesAndExecute(async (start, end) =>
                        {
                            var ff = new FordFulkerson(OutputTextBox);
                            await ff.Execute(pageTask1.GraphManager.GraphData, start, end);
                        });
                        break;
                    
                    default:
                        OutputTextBox.AppendText("Выбранный алгоритм пока не реализован.\n");
                        break;
                }
                
                SelectedStartNode = null;
            }
            else
            {
                MessageBox.Show("Граф отсутствует на текущей странице.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private async Task SelectStartNodeAndExecute(Func<string, Task> action)
        {
            OutputTextBox.AppendText("Выберите начальный узел, кликнув по нему.\n");
            isSelectingStartNode = true;

            while (SelectedStartNode == null)
            {
                await Task.Delay(100);
            }

            isSelectingStartNode = false;
            OutputTextBox.AppendText($"Начальный узел выбран: {SelectedStartNode}\n");

            await action(SelectedStartNode);
        }
        
        // Метод для выбора начального и конечного узлов и выполнения действия
        private async Task SelectStartAndEndNodesAndExecute(Func<string, string, Task> action)
        {
            OutputTextBox.AppendText("Выберите начальный узел, кликнув по нему.\n");
            isSelectingStartNode = true;

            while (SelectedStartNode == null)
            {
                await Task.Delay(100);
            }

            isSelectingStartNode = false;
            OutputTextBox.AppendText($"Начальный узел выбран: {SelectedStartNode}\n");
            
            OutputTextBox.AppendText("Выберите конечный узел, кликнув по нему.\n");
            isSelectingEndNode = true;

            while (SelectedEndNode == null)
            {
                await Task.Delay(100);
            }

            isSelectingEndNode = false;
            OutputTextBox.AppendText($"Конечный узел выбран: {SelectedEndNode}\n");
            
            await action(SelectedStartNode, SelectedEndNode);
        }
        
        public void SelectNode(string nodeLabel)
        {
            if (isSelectingStartNode)
            {
                SelectedStartNode = nodeLabel;
                var nodeGrid = GraphManager.GraphData.GetNodeGrid(nodeLabel);
                isSelectingStartNode = false;
            }
            else if (isSelectingEndNode)
            {
                SelectedEndNode = nodeLabel;
                var nodeGrid = GraphManager.GraphData.GetNodeGrid(nodeLabel);
                isSelectingEndNode = false;
            }
        }
    }
}
