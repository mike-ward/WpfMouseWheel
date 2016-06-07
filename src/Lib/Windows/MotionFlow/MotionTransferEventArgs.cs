using System;
using System.Windows;

namespace Logitech.Windows.MotionFlow
{

    #region MotionTransferEventHandler

    public delegate void MotionTransferEventHandler(object sender, MotionTransferEventArgs e);

    #endregion

    #region MotionTransferEventArgs

    public class MotionTransferEventArgs : RoutedEventArgs
    {
        #region Initialization

        public MotionTransferEventArgs(IMotionInfo info, double delta)
        {
            RoutedEvent = MotionTransfer.TransferedEvent;
            MotionInfo = info;
            Delta = delta;
        }

        #endregion

        #region RoutedEventArgs

        protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
        {
            var handler = (MotionTransferEventHandler) genericHandler;
            handler(genericTarget, this);
        }

        #endregion

        #region Properties

        public IMotionInfo MotionInfo { get; private set; }
        public double Delta { get; private set; }

        #endregion
    }

    #endregion

    #region NativeMotionTransferEventHandler

    public delegate void NativeMotionTransferEventHandler(object sender, NativeMotionTransferEventArgs e);

    #endregion

    #region NativeMotionTransferEventArgs

    public class NativeMotionTransferEventArgs : RoutedEventArgs
    {
        #region Initialization

        public NativeMotionTransferEventArgs(IMotionInfo info, int nativeDelta)
        {
            MotionInfo = info;
            NativeDelta = nativeDelta;
        }

        #endregion

        #region RoutedEventArgs

        protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
        {
            var handler = (NativeMotionTransferEventHandler) genericHandler;
            handler(genericTarget, this);
        }

        #endregion

        #region Properties

        public IMotionInfo MotionInfo { get; private set; }
        public int NativeDelta { get; private set; }

        #endregion
    }

    #endregion
}