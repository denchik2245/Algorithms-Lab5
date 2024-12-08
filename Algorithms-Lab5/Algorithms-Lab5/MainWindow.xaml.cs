using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Algorithms_Lab5;
using Algorithms_Lab5._1task;
using Algorithms_Lab5.Tools;
using Algorithms_Lab5.Utils;

namespace GraphEditor
{
    public partial class MainWindow : Window
    {
        public GraphManager GraphManager { get; private set; }
        public string SelectedStartNode { get; private set; } = null;
        public string SelectedEndNode { get; private set; } = null; // Для конечной вершины
        private bool isSelectingStartNode = false;
        private bool isSelectingEndNode = false; // Для выбора конечной вершины
        
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
        
        //Кнопка "Выполнить"
        private async void ExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            if (GraphFrame.Content is PageTask1 pageTask1)
            {
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
                        OutputTextBox.AppendText("Выберите начальный узел, кликнув по нему.\n");
                        isSelectingStartNode = true;

                        while (SelectedStartNode == null)
                        {
                            await Task.Delay(100);
                        }

                        isSelectingStartNode = false;

                        OutputTextBox.AppendText($"Начальный узел выбран: {SelectedStartNode}\n");

                        var bfs = new BreadthFirstSearch(OutputTextBox);
                        await bfs.Execute(pageTask1.GraphManager.GraphData, SelectedStartNode);

                        break;

                    case "Обход в глубину":
                        OutputTextBox.AppendText("Выберите начальный узел, кликнув по нему.\n");
                        isSelectingStartNode = true;

                        while (SelectedStartNode == null)
                        {
                            await Task.Delay(100);
                        }

                        isSelectingStartNode = false;

                        OutputTextBox.AppendText($"Начальный узел выбран: {SelectedStartNode}\n");

                        var dfs = new DepthFirstSearch(OutputTextBox);
                        await dfs.Execute(pageTask1.GraphManager.GraphData, SelectedStartNode);

                        break;

                    case "Кратчайший путь":
                        // Выбор начального узла
                        OutputTextBox.AppendText("Выберите начальный узел, кликнув по нему.\n");
                        isSelectingStartNode = true;

                        while (SelectedStartNode == null)
                        {
                            await Task.Delay(100); // Ожидание выбора
                        }

                        isSelectingStartNode = false;
                        OutputTextBox.AppendText($"Начальный узел выбран: {SelectedStartNode}\n");

                        // Выбор конечного узла
                        OutputTextBox.AppendText("Выберите конечный узел, кликнув по нему.\n");
                        isSelectingEndNode = true;

                        while (SelectedEndNode == null)
                        {
                            await Task.Delay(100); // Ожидание выбора
                        }

                        isSelectingEndNode = false;
                        OutputTextBox.AppendText($"Конечный узел выбран: {SelectedEndNode}\n");

                        // Проверка выбора
                        if (SelectedStartNode == null || SelectedEndNode == null)
                        {
                            MessageBox.Show("Необходимо выбрать начальный и конечный узлы.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        int startNode = int.Parse(SelectedStartNode);
                        int endNode = int.Parse(SelectedEndNode);

                        // Подготовка данных графа
                        var nodes = new Dictionary<int, Grid>();
                        var edges = new List<Line>();
                        var graphData = pageTask1.GraphManager.GraphData;

                        foreach (var nodeLabel in graphData.Nodes)
                        {
                            var nodeGrid = graphData.GetNodeGrid(nodeLabel);
                            if (nodeGrid != null)
                            {
                                nodes[int.Parse(nodeLabel)] = nodeGrid;
                            }
                        }

                        foreach (var fromNode in graphData.Nodes)
                        {
                            foreach (var (toNode, _) in graphData.GetNeighbors(fromNode))
                            {
                                var edge = graphData.GetEdge(fromNode, toNode);
                                if (edge != null && !edges.Contains(edge))
                                {
                                    edges.Add(edge);
                                }
                            }
                        }

                        var graphDict = new Dictionary<int, List<(int, double)>>();
                        foreach (var fromNode in graphData.Nodes)
                        {
                            var neighbors = new List<(int, double)>();
                            foreach (var (toNode, weight) in graphData.GetNeighbors(fromNode))
                            {
                                neighbors.Add((int.Parse(toNode), weight));
                            }
                            graphDict[int.Parse(fromNode)] = neighbors;
                        }

                        // Запуск алгоритма кратчайшего пути
                        var shortestPathFinder = new ShortestPathFinder(pageTask1.MainCanvas, OutputTextBox);
                        await shortestPathFinder.FindShortestPath(
                            graphDict,
                            startNode,
                            endNode,
                            nodes,
                            edges
                        );

                        // Сброс выбора узлов
                        SelectedStartNode = null;
                        SelectedEndNode = null;
                        break;

                    default:
                        OutputTextBox.AppendText("Выбранный алгоритм пока не реализован.\n");
                        break;
                }

                // Сброс выбора узлов
                SelectedStartNode = null;
                SelectedEndNode = null;
            }
            else
            {
                MessageBox.Show("Граф отсутствует на текущей странице.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        
        public void SelectStartNode(string nodeLabel)
        {
            if (!isSelectingStartNode) return;

            SelectedStartNode = nodeLabel;

            // Подсвечиваем выбранный узел
            var nodeGrid = GraphManager.GraphData.GetNodeGrid(nodeLabel);
            if (nodeGrid != null && nodeGrid.Children[0] is Ellipse ellipse)
            {
                ellipse.Stroke = new SolidColorBrush(Colors.Yellow); // Подсветка обводки
                ellipse.StrokeThickness = 6;
            }

            OutputTextBox.AppendText($"Выбран узел {nodeLabel} в качестве начального.\n");

            // Выход из режима выбора и запуск алгоритма
            isSelectingStartNode = false;
            RunSelectedAlgorithm();
        }
        
        private async void RunSelectedAlgorithm()
        {
            if (SelectedStartNode == null)
            {
                MessageBox.Show("Начальный узел не выбран.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (GraphFrame.Content is PageTask1 pageTask1)
            {
                string selectedAlgorithm = (AlgorithmComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                OutputTextBox.Clear();

                switch (selectedAlgorithm)
                {
                    case "Обход в ширину":
                        var bfs = new BreadthFirstSearch(OutputTextBox);
                        await bfs.Execute(pageTask1.GraphManager.GraphData, SelectedStartNode);
                        break;

                    case "Обход в глубину":
                        var dfs = new DepthFirstSearch(OutputTextBox);
                        await dfs.Execute(pageTask1.GraphManager.GraphData, SelectedStartNode);
                        break;

                    default:
                        OutputTextBox.AppendText("Выбранный алгоритм пока не реализован.\n");
                        break;
                }
            }
            else
            {
                MessageBox.Show("Граф отсутствует на текущей странице.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void SetupNodeClickHandlers(Dictionary<int, Grid> nodes)
        {
            foreach (var node in nodes.Values)
            {
                node.MouseDown += Node_Click; // Добавляем обработчик клика
            }
        }

        private void Node_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Grid nodeGrid)
            {
                var nodeId = nodeGrid.Tag?.ToString();
                if (nodeId != null)
                {
                    if (isSelectingStartNode && SelectedStartNode == null)
                    {
                        SelectedStartNode = nodeId;
                        OutputTextBox.AppendText($"Начальный узел выбран: {nodeId}\n");
                    }
                    else if (isSelectingEndNode && SelectedEndNode == null)
                    {
                        SelectedEndNode = nodeId;
                        OutputTextBox.AppendText($"Конечный узел выбран: {nodeId}\n");
                    }
                }
            }
        }

    }
}
