using System.Windows;

// ReSharper disable once CheckNamespace
namespace Logitech.Windows.Input
{
    public class MouseWheelFrameworkLevelController : MouseWheelController
    {
        #region Initialization

        public MouseWheelFrameworkLevelController(IFrameworkLevelElement frameworkLevelElement)
            : base(frameworkLevelElement)
        {
            frameworkLevelElement.Unloaded += OnElementUnloaded;
        }

        #endregion

        #region Queries

        public IFrameworkLevelElement FrameworkLevelElement => InputLevelElement as IFrameworkLevelElement;

        #endregion

        #region IDisposable

        public override void Dispose()
        {
            FrameworkLevelElement.Unloaded -= OnElementUnloaded;
            base.Dispose();
        }

        #endregion

        #region Helpers

        #region Callbacks

        private void OnElementUnloaded(object sender, RoutedEventArgs e)
        {
            Unload();
        }

        #endregion

        #endregion
    }
}