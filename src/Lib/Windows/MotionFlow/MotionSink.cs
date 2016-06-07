namespace Logitech.Windows.MotionFlow
{

    #region IMotionSinkConverter

    public interface IMotionSinkConverter
    {
        double SinkToNormalized(double value);
        double NormalizedToSink(double value);
    }

    #endregion

    #region IMotionSink

    public interface IMotionSink : IMotionTarget, IMotionSinkConverter
    {
    }

    #endregion
}