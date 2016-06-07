using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Logitech.Maths;
using Logitech.Windows.MotionFlow;

// ReSharper disable once CheckNamespace
namespace Logitech.Windows.Input
{

    #region IMouseWheelTransferCase

    public interface IMouseWheelTransferCase : INativeMotionTransferCase
    {
        IMouseWheelShaft this[double debouncingCellCount] { get; }
        IMouseWheelShaft this[int debouncingCellCount] { get; }
        IMouseWheelShaft ActiveShaft { get; set; }
    }

    #endregion

    #region MouseWheelSingleShaftTransferCase

    public class MouseWheelSingleShaftTransferCase : MouseWheelShaft, IMouseWheelTransferCase
    {
        #region Initialization

        public MouseWheelSingleShaftTransferCase(int resolution) : base(resolution)
        {
        }

        #endregion

        #region IEnumerable<INativeMotionTransfer>

        public IEnumerator<INativeMotionTransfer> GetEnumerator()
        {
            yield return this;
        }

        #endregion

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IMouseWheelTransferCase

        public IMouseWheelShaft ActiveShaft
        {
            get { return this; }
            set
            {
                if (!Equals(value, this)) throw new InvalidOperationException();
            }
        }

        public IMouseWheelShaft this[double resolution] => this;

        public IMouseWheelShaft this[int resolution] => this;

        #endregion
    }

    #endregion

    #region MouseWheelMultiShaftTransferCase

    public class MouseWheelMultiShaftTransferCase : NativeCoupledMotionTransferCase, IMouseWheelTransferCase
    {
        #region Initialization

        public MouseWheelMultiShaftTransferCase(MouseWheel wheel)
        {
            _wheel = wheel;
            InitializeDebouncingFunctions();
        }

        #endregion

        #region Fields

        private readonly MouseWheel _wheel;

        /// <summary>
        ///     Identifies a dictionary of periodic debouncing functions where the key represents the count of debouncing cells per
        ///     period
        /// </summary>
        private readonly Dictionary<int, Int32DifferentialFunctionPatternModulator> _debouncingFunctions =
            new Dictionary<int, Int32DifferentialFunctionPatternModulator>();

        private readonly Dictionary<int, IMouseWheelShaft> _shafts = new Dictionary<int, IMouseWheelShaft>();

        #endregion

        #region IMouseWheelTransferCase

        public IMouseWheelShaft this[double debouncingCellCount] => GetOrAddShaft(MatchDebouncingCellCount(debouncingCellCount));

        public IMouseWheelShaft this[int debouncingCellCount] => GetOrAddShaft(MatchDebouncingCellCount(debouncingCellCount));

        public IMouseWheelShaft ActiveShaft { get; set; }

        #endregion

        #region Helpers

        private IEnumerable<int> DebouncingCellCountsOrderByDescending => _debouncingFunctions.Keys.OrderByDescending(i => i);

        private int MatchDebouncingCellCount(double desiredCount) => MatchDebouncingCellCount((int) Math.Round(desiredCount));

        private int MatchDebouncingCellCount(int desiredCount)
        {
            if (desiredCount < 0)
            {
                // auto debouncing
                return _wheel.NativeNotchFrequency%_wheel.NativeResolutionFrequency != 0 
                    ? 0 
                    : DebouncingCellCountsOrderByDescending.FirstOrDefault();
            }
            return desiredCount <= 1 
                ? desiredCount 
                : DebouncingCellCountsOrderByDescending.FirstOrDefault(key => desiredCount%key == 0);
        }

        private Int32DifferentialFunctionPatternModulator CreateDebouncingFunction(int hysteronCount)
        {
            var hysterons = Int32HysteronGenerator.CreateFunctions(hysteronCount,
                _wheel.NativeNotchFrequency, _wheel.NativeResolutionFrequency);

            var pattern = Int32FunctionSummator.CreateFunction(hysterons);
            return pattern == null 
                ? null 
                : new Int32DifferentialFunctionPatternModulator(x => x/hysteronCount, pattern, _wheel.NativeNotchFrequency);
        }

        private void InitializeDebouncingFunctions()
        {
            for (var hysteronCount = 1;
                hysteronCount <= _wheel.NativeNotchFrequency/_wheel.NativeResolutionFrequency;
                ++hysteronCount)
            {
                var f = CreateDebouncingFunction(hysteronCount);
                if (f == null) break;
                _debouncingFunctions.Add(hysteronCount, f);
            }
        }

        private IMouseWheelShaft GetOrAddShaft(int resolution)
        {
            IMouseWheelShaft shaft;
            if (!_shafts.TryGetValue(resolution, out shaft))
            {
                shaft = resolution == 0 ? CreateDirectShaft() : CreateDebouncedShaft(resolution);
                _shafts[resolution] = shaft;
            }
            return shaft;
        }

        private IMouseWheelShaft CreateDirectShaft()
        {
            var transfer = new MouseWheelShaft(0);
            transfer.Name = transfer.Id.ToString("'D'00");
            Add(transfer);
            return transfer;
        }

        private IMouseWheelShaft CreateDebouncedShaft(int resolution)
        {
            var debouncing = new NativeDebouncedMotionTransform(_debouncingFunctions[resolution]);
            var transfer = new MouseWheelShaft(resolution);
            debouncing.Next = transfer;
            var debouncedTransfer = new NativeMotionTransferGroup(debouncing, transfer);
            Add(debouncedTransfer);
            return transfer;
        }

        #endregion
    }

    #endregion
}