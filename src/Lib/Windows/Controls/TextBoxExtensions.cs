using System.Windows.Controls;

namespace Logitech.Windows.Controls
{
    public static class TextBoxExtensions
    {
        public static int GetScrollableLines(this TextBox source, int direction)
        {
            return direction < 0
                ? -source.GetFirstVisibleLineIndex()
                : source.LineCount - 1 - source.GetLastVisibleLineIndex();
        }
    }
}