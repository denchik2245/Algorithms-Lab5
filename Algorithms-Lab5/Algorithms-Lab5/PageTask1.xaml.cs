using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Algorithms_Lab5.Tools;
using GraphEditor.Tools;

namespace GraphEditor.Pages
{
    public partial class PageTask1 : Page
    {
        private Move _moveTool = new Move();
        private AddNode _addNodeTool = new AddNode();
        private RemoveNode _removeNodeTool = new RemoveNode();
        private AddEdge _addEdgeTool = new AddEdge();
        private RemoveEdge _removeEdgeTool = new RemoveEdge();
        private LoadGraph _loadGraphTool = new LoadGraph();

        public PageTask1()
        {
            InitializeComponent();
            _moveTool.Initialize(MainCanvas);
        }

        // Активируем режимы
        //Перемещение
        public void ActivateMoveMode()
        {
            _addNodeTool.IsActive = false;
            _removeNodeTool.IsActive = false;
            _addEdgeTool.IsActive = false;
            _removeEdgeTool.IsActive = false;
            _moveTool.IsActive = true;
        }
        
        //Добавить узел
        public void ActivateAddNodeMode()
        {
            _addNodeTool.IsActive = true;
            _removeNodeTool.IsActive = false;
            _addEdgeTool.IsActive = false;
            _moveTool.IsActive = false;
        }
        
        //Удалить узел
        public void ActivateRemoveNodeMode()
        {
            _addNodeTool.IsActive = false;
            _removeNodeTool.IsActive = true;
            _addEdgeTool.IsActive = false;
            _moveTool.IsActive = false;
        }
        
        //Добавить ребро
        public void ActivateAddEdgeMode()
        {
            _addNodeTool.IsActive = false;
            _removeNodeTool.IsActive = false;
            _addEdgeTool.IsActive = true;
            _moveTool.IsActive = false;
        }
        
        //Удалить ребро
        public void ActivateRemoveEdgeMode()
        {
            _addNodeTool.IsActive = false;
            _removeNodeTool.IsActive = false;
            _addEdgeTool.IsActive = false;
            _removeEdgeTool.IsActive = true;
            _moveTool.IsActive = false;
        }
        
        //Загрузить файл
        public void LoadGraph()
        {
            var loadGraphTool = new LoadGraph();
            loadGraphTool.Load(MainCanvas);
        }
        
        // Обработчик клика по Canvas
        private void MainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_moveTool.IsActive)
            {
                _moveTool.StartDrag(e);
            }
            else if (_addNodeTool.IsActive)
            {
                Point clickPosition = e.GetPosition(MainCanvas);
                _addNodeTool.Add(MainCanvas, clickPosition);
            }
            else if (_removeNodeTool.IsActive)
            {
                _removeNodeTool.Remove(MainCanvas, e);
            }
            else if (_addEdgeTool.IsActive)
            {
                if (e.OriginalSource is FrameworkElement clickedElement)
                {
                    _addEdgeTool.SelectNode(MainCanvas, clickedElement.Parent as UIElement);
                }
            }
            else if (_removeEdgeTool.IsActive)
            {
                _removeEdgeTool.Remove(MainCanvas, e);
            }
            else if (_loadGraphTool.IsActive)
            {
                _loadGraphTool.Load(MainCanvas);
            }
        }
        
        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_moveTool.IsActive)
            {
                _moveTool.Drag(e);
            }
        }

        private void MainCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_moveTool.IsActive)
            {
                _moveTool.EndDrag();
            }
        }
    }
}