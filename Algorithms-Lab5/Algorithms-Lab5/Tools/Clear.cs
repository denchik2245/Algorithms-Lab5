using System.Windows.Controls;

namespace Algorithms_Lab5.Tools
{
    public class Clear
    {
        private Canvas _canvas;

        public Clear(Canvas canvas)
        {
            _canvas = canvas;
        }
        
        public void ClearCanvas()
        {
            _canvas.Children.Clear();
        }
    }
}