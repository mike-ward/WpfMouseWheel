namespace Logitech.Windows.MotionFlow
{
    public class NativeMotionTerminal : MotionElement, INativeMotionInput, IMotionInput
    {
        #region Constants

        public static readonly NativeMotionTerminal Current = new NativeMotionTerminal();

        #endregion

        #region INativeMotionInput

        public void Transmit(IMotionInfo info, int nativeDelta, INativeMotionOutput source)
        {
        }

        public void OnCoupledTransfer(IMotionInfo info, int nativeDelta, INativeMotionTransferOutput source)
        {
        }

        public void Reset()
        {
        }

        #endregion

        #region IMotionInput Members

        public void Transmit(IMotionInfo info, double delta, IMotionOutput source)
        {
        }

        public void OnCoupledTransfer(IMotionInfo info, double delta, IMotionTransferOutput source)
        {
        }

        #endregion
    }
}