using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Algorithms_Lab5.Tools
{
    public class SaveGraph
    {
        public void Save(Canvas canvas)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                FileName = "graph.csv"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;
                var adjacencyMatrix = GenerateAdjacencyMatrixWithCoordinates(canvas);
                SaveMatrixToCsv(adjacencyMatrix, filePath);
            }
        }

        private List<(Grid Node, double X, double Y)> GetNodesWithCoordinates(Canvas canvas)
        {
            var nodes = new List<(Grid Node, double X, double Y)>();
            foreach (UIElement element in canvas.Children)
            {
                if (element is Grid grid)
                {
                    double x = Canvas.GetLeft(grid) + (grid.ActualWidth / 2);
                    double y = Canvas.GetTop(grid) + (grid.ActualHeight / 2);
                    nodes.Add((grid, x, y));
                }
            }
            return nodes;
        }

        private string[,] GenerateAdjacencyMatrixWithCoordinates(Canvas canvas)
        {
            var nodesWithCoordinates = GetNodesWithCoordinates(canvas);
            int nodeCount = nodesWithCoordinates.Count;

            string[,] matrix = new string[nodeCount, nodeCount + 2];

            for (int i = 0; i < nodeCount; i++)
            {
                for (int j = 0; j < nodeCount; j++)
                {
                    matrix[i, j] = "0";
                }
                matrix[i, nodeCount] = nodesWithCoordinates[i].X.ToString(System.Globalization.CultureInfo.InvariantCulture);
                matrix[i, nodeCount + 1] = nodesWithCoordinates[i].Y.ToString(System.Globalization.CultureInfo.InvariantCulture);
            }

            foreach (UIElement element in canvas.Children)
            {
                if (element is Line line && line.Tag is Tuple<Grid, Grid, TextBlock> connectedNodes)
                {
                    Grid startNode = connectedNodes.Item1;
                    Grid endNode = connectedNodes.Item2;

                    int startIndex = nodesWithCoordinates.FindIndex(n => n.Node == startNode);
                    int endIndex = nodesWithCoordinates.FindIndex(n => n.Node == endNode);

                    if (startIndex != -1 && endIndex != -1)
                    {
                        matrix[startIndex, endIndex] = "1";
                        matrix[endIndex, startIndex] = "1";
                    }
                }
            }

            return matrix;
        }

        private void SaveMatrixToCsv(string[,] matrix, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                int rows = matrix.GetLength(0);
                int cols = matrix.GetLength(1);

                for (int i = 0; i < rows; i++)
                {
                    var row = new List<string>();
                    for (int j = 0; j < cols; j++)
                    {
                        row.Add(matrix[i, j] ?? "");
                    }
                    writer.WriteLine(string.Join(";", row));
                }
            }
        }
    }
}
