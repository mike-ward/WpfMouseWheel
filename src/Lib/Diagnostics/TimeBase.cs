using System;
using System.Diagnostics;

namespace Logitech.Diagnostics
{
    public class TimeBase
    {
        #region Constants

        public static readonly TimeBase Current = new TimeBase();

        #endregion

        #region Fields

        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

        #endregion

        #region Queries

        public TimeSpan Elapsed => _stopwatch.Elapsed;

        #endregion

        #region Methods

        public void Start()
        {
        }

        #endregion
    }
}