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

        public PageTask1()
        {
            InitializeComponent();
            _moveTool.Initialize(MainCanvas);
        }

        // Активируем режимы
        public void ActivateAddNodeMode()
        {
            _addNodeTool.IsActive = true;
            _removeNodeTool.IsActive = false;
            _addEdgeTool.IsActive = false;
        }

        public void ActivateMoveMode()
        {
            _addNodeTool.IsActive = false;
            _removeNodeTool.IsActive = false;
            _addEdgeTool.IsActive = false;
            _removeEdgeTool.IsActive = false;
            _moveTool.IsActive = true;
        }
        
        public void ActivateRemoveNodeMode()
        {
            _addNodeTool.IsActive = false;
            _removeNodeTool.IsActive = true;
            _addEdgeTool.IsActive = false;
        }
        
        public void ActivateAddEdgeMode()
        {
            _addNodeTool.IsActive = false;
            _removeNodeTool.IsActive = false;
            _addEdgeTool.IsActive = true;
        }
        
        public void ActivateRemoveEdgeMode()
        {
            _addNodeTool.IsActive = false;
            _removeNodeTool.IsActive = false;
            _addEdgeTool.IsActive = false;
            _removeEdgeTool.IsActive = true;
        }
        
        // Обработчик клика по Canvas
        private void MainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Перемещение узлов
            if (_moveTool.IsActive)
            {
                _moveTool.StartDrag(e);
            }
            
            // Логика для добавления узлов
            if (_addNodeTool.IsActive)
            {
                Point clickPosition = e.GetPosition(MainCanvas);
                _addNodeTool.Add(MainCanvas, clickPosition);
            }

            // Логика для удаления узлов
            if (_removeNodeTool.IsActive)
            {
                _removeNodeTool.Remove(MainCanvas, e);
            }

            // Логика для добавления ребра
            if (_addEdgeTool.IsActive)
            {
                if (e.OriginalSource is FrameworkElement clickedElement)
                {
                    _addEdgeTool.SelectNode(MainCanvas, clickedElement.Parent as UIElement);
                }
            }

            // Логика для удаления ребра
            if (_removeEdgeTool.IsActive)
            {
                _removeEdgeTool.Remove(MainCanvas, e);
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