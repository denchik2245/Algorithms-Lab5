using System.Windows.Controls;
using System.Windows.Shapes;
using Algorithms_Lab5.Tools;

namespace Algorithms_Lab5.Utils;

public class GraphManager
{
    public Dictionary<int, Grid> Nodes { get; private set; } = new Dictionary<int, Grid>();
    public List<Line> Edges { get; private set; } = new List<Line>();
    public GraphData GraphData { get; private set; } = new GraphData();
    public AddNode AddNodeTool { get; private set; }
    public AddEdge AddEdgeTool { get; private set; }
    public RemoveNode RemoveNodeTool { get; private set; }
    public RemoveEdge RemoveEdgeTool { get; private set; }

    public GraphManager()
    {
        AddNodeTool = new AddNode(GraphData);
        AddEdgeTool = new AddEdge(GraphData);
        RemoveNodeTool = new RemoveNode(GraphData);
        RemoveEdgeTool = new RemoveEdge(GraphData);
    }

    // Методы для управления графом
}