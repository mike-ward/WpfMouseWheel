using System;

namespace Logitech.Maths
{

    #region LowPassFilter

    /// <summary>
    ///     Filters high frequencies of an input signal
    /// </summary>
    public class LowPassFilter
    {
        #region Fields

        private double _t0 = double.NaN;

        #endregion

        #region Initialization

        /// <summary>
        ///     Initializes a new instance of the LowPassFilter class
        /// </summary>
        public LowPassFilter()
        {
        }

        public LowPassFilter(double lifetime)
        {
            Lifetime = lifetime;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the lifetime of the filter
        /// </summary>
        public double Lifetime { get; set; }

        ///// <summary>
        ///// Gets or sets the gain of the filter
        ///// </summary>
        //public double Gain { get; set; }
        /// <summary>
        ///     Gets the current input signal value or increment
        /// </summary>
        public double Input { get; private set; }

        /// <summary>
        ///     Gets the current output signal value or increment
        /// </summary>
        public double Output { get; private set; }

        #endregion

        #region Overridables

        /// <summary>
        ///     Inputs a new signal increment.
        /// </summary>
        public void NewInputDelta(double delta)
        {
            Input += delta;
        }

        /// <summary>
        ///     Computes next output signal increment
        /// </summary>
        public double NextOutputDelta(double t, double dtMin)
        {
            var dt = double.IsNaN(_t0) ? dtMin : t - _t0;
            _t0 = t;
            var gain = Math.Min(1.0, dt/Lifetime);
            Output += gain*(Input - Output);
            Input = 0;
            return Output;
        }

        #endregion
    }

    #endregion
}