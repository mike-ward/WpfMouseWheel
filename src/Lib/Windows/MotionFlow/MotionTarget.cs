using System;
using System.Windows.Media;
using Logitech.Diagnostics;

namespace Logitech.Windows.MotionFlow
{

    #region INativeMotionTarget

    public interface INativeMotionTarget
    {
        bool CanMove(IMotionInfo info, object context);
        int Coerce(IMotionInfo info, object context, int nativeDelta);
        void Move(IMotionInfo info, object context, int nativeDelta);
    }

    #endregion

    #region IMotionTarget

    public interface IMotionTarget
    {
        bool CanMove(IMotionInfo info, object context);
        double Coerce(IMotionInfo info, object context, double delta);
        void Move(IMotionInfo info, object context, double delta);
    }

    #endregion

    #region NativeMotionTarget

    public class NativeMotionTarget : INativeMotionTarget
    {
        #region Constants

        public static readonly NativeMotionTarget Terminal = new NativeMotionTarget();

        #endregion

        #region INativeMotionTarget

        public virtual bool CanMove(IMotionInfo info, object context)
        {
            return true;
        }

        public virtual int Coerce(IMotionInfo info, object context, int nativeDelta)
        {
            return nativeDelta;
        }

        public virtual void Move(IMotionInfo info, object context, int nativeDelta)
        {
        }

        #endregion
    }

    #endregion

    #region MotionTarget

    public class MotionTarget : IMotionTarget
    {
        #region IMotionTarget

        public virtual bool CanMove(IMotionInfo info, object context)
        {
            return !DoubleEx.IsZero(Coerce(info, context, info.Direction));
        }

        public virtual double Coerce(IMotionInfo info, object context, double delta)
        {
            return delta;
        }

        public virtual void Move(IMotionInfo info, object context, double delta)
        {
        }

        #endregion
    }

    #endregion

    #region MotionTargetLink

    public class MotionTargetLink : MotionTarget
    {
        #region Properties

        public IMotionTarget Next { get; set; }

        #endregion

        #region MotionInput

        public override bool CanMove(IMotionInfo info, object context)
        {
            return Next.CanMove(info, context);
        }

        public override double Coerce(IMotionInfo info, object context, double delta)
        {
            return Next.Coerce(info, context, delta);
        }

        public override void Move(IMotionInfo info, object context, double delta)
        {
            Next.Move(info, context, delta);
        }

        #endregion
    }

    #endregion

    #region MotionSmoothingTarget

    public class MotionSmoothingTarget : MotionTargetLink, IDisposable
    {
        #region Initialization

        public MotionSmoothingTarget(IMotionFilter filter)
        {
            _filter = filter;
        }

        #endregion

        #region Properties

        public double Precision { get; set; }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            CompositionTarget.Rendering -= OnRendering;
        }

        #endregion

        #region IMotionTarget

        public override void Move(IMotionInfo info, object context, double delta)
        {
            var t = TimeBase.Current.Elapsed;
            _filter.NewInputDelta(t, delta, info);
            CompositionTarget.Rendering -= OnRendering;
            CompositionTarget.Rendering += OnRendering;
        }

        #endregion

        #region Helpers

        private void OnRendering(object sender, EventArgs e)
        {
            var t = TimeBase.Current.Elapsed;
            var delta = _remainder + _filter.NextOutputDelta(t);
            if (DoubleEx.GreaterThanOrClose(Math.Abs(delta), Precision))
            {
                _remainder = 0;
                Next.Move(null, null, delta);
            }
            else if (DoubleEx.IsZero(delta))
                CompositionTarget.Rendering -= OnRendering;
            else
                _remainder += delta;
        }

        #endregion

        #region Fields

        private readonly IMotionFilter _filter;
        private double _remainder;

        #endregion
    }

    #endregion
}