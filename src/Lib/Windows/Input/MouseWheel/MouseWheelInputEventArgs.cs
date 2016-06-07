using System;
using System.Windows;
using System.Windows.Input;
using Logitech.Windows.MotionFlow;

// ReSharper disable once CheckNamespace
namespace Logitech.Windows.Input
{

    #region MouseWheelInputEventArgs

    public class MouseWheelInputEventArgs : MouseEventArgs
    {
        #region Initialization

        public MouseWheelInputEventArgs(IMouseWheelController controller, MouseWheel wheel,
            MouseWheelEventArgs nativeEventArgs)
            : base(nativeEventArgs.MouseDevice, nativeEventArgs.Timestamp)
        {
            Controller = controller;
            Wheel = wheel;
            NativeEventArgs = nativeEventArgs;
        }

        #endregion

        #region MouseEventArgs

        protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
        {
            var handler = (MouseWheelInputEventHandler) genericHandler;
            handler(genericTarget, this);
        }

        #endregion

        #region MouseWheelNativeMotionTarget

        private class MouseWheelNativeMotionTarget : INativeMotionTarget
        {
            #region Singleton

            public static INativeMotionTarget Current { get; } = new MouseWheelNativeMotionTarget();

            #endregion

            #region INativeMotionTarget

            public bool CanMove(IMotionInfo info, object context)
            {
                return true;
            }

            public int Coerce(IMotionInfo info, object context, int nativeDelta)
            {
                return nativeDelta;
            }

            public void Move(IMotionInfo info, object context, int nativeDelta)
            {
                var e = (MouseWheelInputEventArgs)context;
                var args = e?.NativeDeltaToNativeEventArgs(nativeDelta);
                if (args != null) e.Controller.ExitElement.RaiseEvent(args);
            }

            #endregion
        }

        #endregion

        #region Properties

        public MouseWheelEventArgs NativeEventArgs { get; }
        public MouseWheel Wheel { get; }
        public IMouseWheelController Controller { get; set; }

        #endregion

        #region Methods

        public void RaiseNativeEvent(int nativeDelta)
        {
            Controller.InputElement.RaiseEvent(NativeDeltaToNativeEventArgs(nativeDelta));
        }

        public void EndCommand()
        {
            if (Handled)
                Controller.ExitElement.RaiseEvent(CreateNativeEventArgs(Timestamp, NativeEventArgs.Delta));
            else
                Handled = Wheel.Transfer(MouseWheelNativeMotionTarget.Current, this);
        }

        #endregion

        #region Helpers

        private MouseWheelEventArgs CreateNativeEventArgs(int timestamp, int delta)
        {
            return new MouseWheelEventArgs(Wheel.MouseDevice, timestamp, delta)
            {
                RoutedEvent = UIElement.MouseWheelEvent,
                Handled = Handled
            };
        }

        private MouseWheelEventArgs NativeDeltaToNativeEventArgs(int nativeDelta)
        {
            return nativeDelta == 0 ? null : CreateNativeEventArgs(Timestamp, nativeDelta);
        }

        #endregion
    }

    #endregion

    #region MouseWheelInputEventHandler

    public delegate void MouseWheelInputEventHandler(object sender, MouseWheelInputEventArgs e);

    #endregion
}