using System;

namespace Logitech.Windows.MotionFlow
{

    #region IMotionFilter

    public interface IMotionFilter
    {
        void NewInputDelta(TimeSpan t, double delta, IMotionInfo info);
        double NextOutputDelta(TimeSpan t);
    }

    #endregion
}