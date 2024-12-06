using System.Globalization;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using GraphEditor.Tools;

namespace Algorithms_Lab5.Tools
{
    public class LoadGraph
    {
        private AddNode addNode;
        private AddEdge addEdge;
        public bool IsActive { get; set; }

        public LoadGraph()
        {
            addNode = new AddNode
            {
                IsActive = true
            };
            addEdge = new AddEdge();
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

            int nodeCount = graphData.Length;
            var nodes = new List<(int Index, double X, double Y)>();
            var adjacencyMatrix = new int[nodeCount, nodeCount];

            for (int i = 0; i < nodeCount; i++)
            {
                try
                {
                    var row = graphData[i].Trim().Split(';');
                    
                    if (row.Length < nodeCount + 2)
                    {
                        throw new InvalidOperationException($"Строка {i + 1} имеет некорректную длину. Ожидалось {nodeCount + 2} значений, а получено {row.Length}.");
                    }
                    
                    for (int j = 0; j < nodeCount; j++)
                    {
                        if (!int.TryParse(row[j].Trim(), out int value))
                        {
                            throw new InvalidOperationException($"Некорректное значение в строке {i + 1}, столбце {j + 1}: {row[j]}");
                        }
                        
                        if (value < 0)
                        {
                            throw new InvalidOperationException($"Некорректное значение матрицы смежности в строке {i + 1}, столбце {j + 1}: {value}. Ожидалось неотрицательное число.");
                        }

                        adjacencyMatrix[i, j] = value;
                    }
                    
                    if (!double.TryParse(row[nodeCount].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out double x))
                    {
                        throw new InvalidOperationException($"Некорректное значение координаты X в строке {i + 1}: {row[nodeCount]}");
                    }

                    if (!double.TryParse(row[nodeCount + 1].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out double y))
                    {
                        throw new InvalidOperationException($"Некорректное значение координаты Y в строке {i + 1}: {row[nodeCount + 1]}");
                    }

                    nodes.Add((i, x, y));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при обработке строки {i + 1}: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            // Создаём рёбра сначала
            for (int i = 0; i < nodeCount; i++)
            {
                for (int j = i + 1; j < nodeCount; j++)
                {
                    if (adjacencyMatrix[i, j] > 0)
                    {
                        Point firstNodeCenter = new Point(nodes[i].X, nodes[i].Y);
                        Point secondNodeCenter = new Point(nodes[j].X, nodes[j].Y);
                        addEdge.DrawEdgeWithWeight(canvas, firstNodeCenter, secondNodeCenter, null, null);
                    }
                }
            }
            
            foreach (var node in nodes)
            {
                addNode.Add(canvas, new Point(node.X, node.Y));
            }
        }
    }
}
