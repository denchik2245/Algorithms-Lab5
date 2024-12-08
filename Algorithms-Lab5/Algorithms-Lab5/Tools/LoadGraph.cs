using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using Algorithms_Lab5.Utils;
using Microsoft.Win32;

namespace Algorithms_Lab5.Tools
{
    public class LoadGraph
    {
        private GraphManager _graphManager;
        public bool IsActive { get; set; }

        // Словарь для хранения соответствия индексов узлов их Grid
        private Dictionary<int, Grid> nodeGrids;

        public LoadGraph(GraphManager graphManager)
        {
            _graphManager = graphManager;
            nodeGrids = new Dictionary<int, Grid>();
        }

        public void Load(Canvas canvas)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                Title = "Выберите файл графа"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                var graphData = File.ReadAllLines(filePath);
                ParseGraphData(graphData, canvas);
            }
        }

        private void ParseGraphData(string[] graphData, Canvas canvas)
        {
            canvas.Children.Clear();

            // Очищаем граф и сбрасываем состояние инструментов
            //_graphManager.GraphData.Clear();
            _graphManager.AddNodeTool.ResetCount();

            nodeGrids.Clear();

            int nodeCount = graphData.Length;
            var nodes = new List<(int Index, double X, double Y)>();
            var adjacencyMatrix = new double[nodeCount, nodeCount];

            for (int i = 0; i < nodeCount; i++)
            {
                try
                {
                    var row = graphData[i].Trim().Split(';');

                    if (row.Length < nodeCount + 2)
                        throw new InvalidOperationException($"Строка {i + 1} имеет некорректную длину.");

                    for (int j = 0; j < nodeCount; j++)
                    {
                        if (!double.TryParse(row[j].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out double value) || value < 0)
                            throw new InvalidOperationException($"Некорректное значение матрицы смежности в строке {i + 1}, столбце {j + 1}.");

                        adjacencyMatrix[i, j] = value;
                    }

                    if (!double.TryParse(row[nodeCount].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out double x) ||
                        !double.TryParse(row[nodeCount + 1].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out double y))
                        throw new InvalidOperationException($"Некорректные координаты в строке {i + 1}.");

                    nodes.Add((i, x, y));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка в строке {i + 1}: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            _graphManager.AddNodeTool.IsActive = true;

            // Добавляем узлы
            foreach (var node in nodes)
            {
                string label = (node.Index + 1).ToString();
                Grid nodeGrid = _graphManager.AddNodeTool.Add(canvas, new Point(node.X, node.Y), label);
                if (nodeGrid == null)
                {
                    MessageBox.Show($"Не удалось добавить узел {node.Index}.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                nodeGrids[node.Index] = nodeGrid;
            }

            canvas.UpdateLayout();

            // Добавляем рёбра
            for (int i = 0; i < nodeCount; i++)
            {
                for (int j = i + 1; j < nodeCount; j++)
                {
                    if (adjacencyMatrix[i, j] > 0)
                    {
                        if (nodeGrids.TryGetValue(i, out Grid firstGrid) && nodeGrids.TryGetValue(j, out Grid secondGrid))
                        {
                            string weight = adjacencyMatrix[i, j].ToString(CultureInfo.InvariantCulture);

                            // Создаём TextBlock для веса
                            TextBlock weightText = _graphManager.AddEdgeTool.CreateWeightTextBlock(weight, canvas);

                            // Рисуем ребро
                            Line line = _graphManager.AddEdgeTool.DrawEdgeWithWeight(canvas, firstGrid, secondGrid, weightText);

                            // Добавляем ребро в GraphData
                            string firstLabel = (string)firstGrid.Tag;
                            string secondLabel = (string)secondGrid.Tag;
                            double w = adjacencyMatrix[i, j];
                            _graphManager.GraphData.AddEdge(firstLabel, secondLabel, w, line, weightText);
                        }
                        else
                        {
                            MessageBox.Show($"Не удалось найти узлы с индексами {i} или {j}.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }

            _graphManager.AddNodeTool.IsActive = false;
        }

    }

}
