 // ReSharper disable once CheckNamespace
namespace Logitech.Windows.Input
{

    #region IMouseWheelInputListener

    public interface IMouseWheelInputListener
    {
        void OnPreviewInput(object sender, MouseWheelInputEventArgs e);
        void OnInput(object sender, MouseWheelInputEventArgs e);
    }

    #endregion
}