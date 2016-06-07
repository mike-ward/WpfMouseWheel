using System;
using System.Diagnostics;

namespace Logitech.Windows.MotionFlow
{

    #region INativeMotionSourceInput

    public interface INativeMotionSourceInput : INativeMotionInput
    {
        void Transmit(int timeStamp, int nativeDelta);
    }

    #endregion

    #region INativeMotionConverter

    public interface INativeMotionConverter
    {
        int NativeResolutionFrequency { get; }

        double NativeToNormalized(int value);
        int NormalizedToNative(double value);
    }

    #endregion

    #region INativeMotionSource

    public interface INativeMotionSource : IMotionInfo, INativeMotionSourceInput, INativeMotionOutput,
        INativeMotionConverter
    {
    }

    #endregion

    #region NativeMotionSource

    public abstract class NativeMotionSource : MotionElementLink, INativeMotionSource
    {
        #region Initialization

        protected NativeMotionSource()
        {
            Next = NativeMotionTerminal.Current;
        }

        #endregion

        #region INativeMotionSourceInput

        public void Transmit(int timeStamp, int nativeDelta)
        {
            var info = PreTransmit(timeStamp, nativeDelta);
            Transmit(info, nativeDelta, null);
        }

        #endregion

        #region INativeMotionOutput

        public INativeMotionInput Next
        {
            [DebuggerStepThrough] get { return GetNext() as INativeMotionInput; }
            [DebuggerStepThrough] set { SetNext(value); }
        }

        #endregion

        #region Methods

        public virtual IMotionInfo PreTransmit(int timeStamp, int nativeDelta)
        {
            if (nativeDelta != 0)
            {
                NativeDirection = Math.Sign(nativeDelta);

                UpdateTimings(timeStamp);

                var delta = NativeToNormalized(nativeDelta);
                UpdateVelocity(delta);
            }
            return this;
        }

        #endregion

        #region Fields

        private long _timeStamp = -1;
        private int _previousNativeDirection;
        private int _nativeDirection;

        #endregion

        #region IMotionInfo

        public IMotionInfo Source => this;

        public TimeSpan Time => TimeSpan.FromMilliseconds(_timeStamp);

        public TimeSpan Delay { get; internal set; } = TimeSpan.Zero;
        public double Velocity { get; private set; }

        public double Speed => Math.Abs(Velocity);

        public int Direction => -_nativeDirection;

        public bool DirectionChanged => _previousNativeDirection != _nativeDirection;

        public int NativeDirection
        {
            get { return _nativeDirection; }
            private set
            {
                if (_nativeDirection == value) return;
                _previousNativeDirection = _nativeDirection;
                _nativeDirection = value;
            }
        }

        #endregion

        #region INativeMotionInput

        public void Transmit(IMotionInfo info, int nativeDelta, INativeMotionOutput source)
        {
            Next.Transmit(info, nativeDelta, this);
        }

        public void OnCoupledTransfer(IMotionInfo info, int nativeDelta, INativeMotionTransferOutput source)
        {
            Next.OnCoupledTransfer(info, nativeDelta, source);
        }

        public void Reset()
        {
            Next.Reset();
        }

        #endregion

        #region INativeMotionConverter

        public abstract int NativeResolutionFrequency { get; }

        public abstract double NativeToNormalized(int value);
        public abstract int NormalizedToNative(double value);

        #endregion

        #region Helpers

        private void UpdateTimings(long timeStamp)
        {
            // for information on timestamp seee http://msdn.microsoft.com/en-us/library/ms644939(VS.85).aspx

            if (timeStamp < 0)
                throw new ArgumentOutOfRangeException(nameof(timeStamp));

            if (_timeStamp >= 0)
            {
                // fix timestamp wrapping issue
                if (timeStamp < _timeStamp) timeStamp += int.MaxValue;
                Delay = TimeSpan.FromMilliseconds(timeStamp - _timeStamp);
            }
            _timeStamp = timeStamp;
        }

        private void UpdateVelocity(double delta)
        {
            if (Delay != TimeSpan.Zero)
                Velocity = delta/Delay.TotalSeconds;
        }

        #endregion
    }

    #endregion
}