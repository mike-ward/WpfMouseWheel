using System;

namespace Logitech.Windows.MotionFlow
{

    #region IMotionInfo

    public interface IMotionInfo
    {
        IMotionInfo Source { get; }
        TimeSpan Time { get; }
        TimeSpan Delay { get; }
        double Velocity { get; }
        double Speed { get; }
        int NativeDirection { get; }
        int Direction { get; }
        bool DirectionChanged { get; }
    }

    #endregion
}