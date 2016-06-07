using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Logitech.Windows.Controls
{
    public static class ScrollViewerExtensions
    {
        public static void Scroll(this ScrollViewer source, Orientation orientation, double scrollLength)
        {
            if (orientation == Orientation.Vertical)
                source.ScrollToVerticalOffset(source.VerticalOffset + scrollLength);
            else
                source.ScrollToHorizontalOffset(source.HorizontalOffset + scrollLength);
        }

        public static double GetScrollableDisplacement(this ScrollViewer source, Orientation orientation, int direction)
        {
            if (orientation == Orientation.Vertical)
            {
                if (!DoubleEx.IsZero(source.ScrollableHeight))
                {
                    if (direction < 0)
                        return -source.VerticalOffset;
                    return source.ExtentHeight - source.ViewportHeight - source.VerticalOffset;
                }
            }
            else if (!DoubleEx.IsZero(source.ScrollableWidth))
            {
                if (direction < 0)
                    return -source.HorizontalOffset;
                return source.ExtentWidth - source.ViewportWidth - source.HorizontalOffset;
            }
            return 0;
        }

        public static Orientation GetScrollAreaOrientation(this ScrollViewer source)
        {
            var vsp = source.GetFirstVisualDescendantOfType<VirtualizingStackPanel>();
            if (vsp != null)
                return vsp.Orientation;
            var sp = source.GetFirstVisualDescendantOfType<StackPanel>();
            if (sp != null)
                return sp.Orientation;
            return Orientation.Vertical;
        }

        public static bool HasNestedScrollFrames(this ScrollViewer scrollViewer)
        {
            var nsv = scrollViewer.GetFirstVisualDescendantOfType<ScrollViewer>();
            return nsv != null && !Equals(nsv.TemplatedParent, scrollViewer);
        }

        public static bool ObjectIsDescendantOfNestedScrollViewer(this ScrollViewer source, DependencyObject obj)
        {
            if (obj != null && !Equals(obj, source))
            {
                if (obj is Visual || obj is Visual3D)
                {
                    var scrollFrame = obj.GetFirstVisualAncestorOfType<ScrollViewer>();
                    if (scrollFrame != null && !Equals(scrollFrame, source)) return true;
                }
                else
                {
                    var scrollFrame = obj.GetFirstLogicalAncestorOfType<ScrollViewer>();
                    if (scrollFrame != null && !Equals(scrollFrame, source))
                        return true;
                }
            }
            return false;
        }
    }
}