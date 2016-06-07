using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Controls;
using Logitech.Windows.MotionFlow;
using System.Windows;

namespace Logitech.Windows.Input
{
  public class MouseWheelFlowDocumentPageViewerScrollClient : MouseWheelClient
  {
    #region Fields
    private ModifierKeys _modifiersKeys;
    #endregion

    #region Initialization
    public MouseWheelFlowDocumentPageViewerScrollClient(IMouseWheelController controller) : base(controller) { }
    #endregion

    #region MouseWheelClient
    protected override void OnLoading()
    {
      base.OnLoading();
      var element = Controller.Element;
      _modifiersKeys = MouseWheel.GetVScrollModifiers(element);
      MouseWheel.VScrollModifiersProperty.AddValueChanged(element, OnModifierKeysYChanged);
    }
    protected override void OnUnloading()
    {
      var element = Controller.Element;
      MouseWheel.VScrollModifiersProperty.RemoveValueChanged(element, OnModifierKeysYChanged);
      base.OnUnloading();
    }
    protected override IMouseWheelInputListener CreateBehavior() { return new MouseWheelFlowDocumentPageViewerScrollBehavior(this); }
    #endregion

    #region IMouseWheelClient
    public override ModifierKeys Modifiers { get { return _modifiersKeys; } }
    #endregion

    #region IMotionTarget
    public override bool CanMove(IMotionInfo info, object context)
    {
      return info.Direction < 0 ? PageViewer.CanGoToPreviousPage : PageViewer.CanGoToNextPage;
    }
    public override double Coerce(IMotionInfo info, object context, double delta)
    {
      var p = PageViewer;
      if (info.Direction < 0)
      {
        var scrollable = -(p.MasterPageNumber - 1);
        return Math.Max(scrollable, delta);
      }
      else
      {
        var scrollable = p.PageCount - p.MasterPageNumber;
        return Math.Min(scrollable, delta);
      }
    }
    public override void Move(IMotionInfo info, object context, double delta)
    {
      if (info.NativeDirection > 0)
        PageViewer.PreviousPage();
      else
        PageViewer.NextPage();
    }
    #endregion

    #region Helpers
    private FlowDocumentPageViewer PageViewer { get { return Controller.Element as FlowDocumentPageViewer; } }

    private void OnModifierKeysYChanged(object sender, EventArgs e) { _modifiersKeys = MouseWheel.GetVScrollModifiers(sender as DependencyObject); }
    #endregion
  }
}
