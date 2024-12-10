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

                    case "Минимальное остовное дерево":
                        OutputTextBox.AppendText("Выберите начальный узел, кликнув по нему.\n");
                        isSelectingStartNode = true;

                        while (SelectedStartNode == null)
                        {
                            await Task.Delay(100);
                        }

                        isSelectingStartNode = false;

                        OutputTextBox.AppendText($"Начальный узел выбран: {SelectedStartNode}\n");

                        var mst = new MinimumSpanningTree(OutputTextBox);
                        mst.SetSelectedStartNode(SelectedStartNode);
                        await mst.Execute(pageTask1.GraphManager.GraphData);

                        break;

                    default:
                        OutputTextBox.AppendText("Выбранный алгоритм пока не реализован.\n");
                        break;
                }
                
                SelectedStartNode = null;
                SelectedEndNode = null;
            }
            else
            {
                MessageBox.Show("Граф отсутствует на текущей странице.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        //Выбрать стартовый узел
        public void SelectStartNode(string nodeLabel)
        {
            if (!isSelectingStartNode) return;
            SelectedStartNode = nodeLabel;
            var nodeGrid = GraphManager.GraphData.GetNodeGrid(nodeLabel);
            OutputTextBox.AppendText($"Выбран узел {nodeLabel} в качестве начального.\n");
            
            isSelectingStartNode = false;
            RunSelectedAlgorithm();
        }
        
        //Запустить выбранный алгоритм
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

                    case "Минимальное остовное дерево":
                        var mst = new MinimumSpanningTree(OutputTextBox);
                        mst.SetSelectedStartNode(SelectedStartNode);
                        await mst.Execute(pageTask1.GraphManager.GraphData);
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
        

    }
}
