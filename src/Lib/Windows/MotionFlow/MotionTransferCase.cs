using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace Logitech.Windows.MotionFlow
{

    #region IMotionTransferCase

    public interface IMotionTransferCase : IMotionInput, IEnumerable<IMotionTransfer>, IMotionTransferOutput
    {
    }

    #endregion

    #region INativeMotionTransferCase

    public interface INativeMotionTransferCase : INativeMotionInput, IEnumerable<INativeMotionTransfer>,
        INativeMotionTransferOutput
    {
    }

    #endregion

    #region MotionTransferCaseBase

    public abstract class MotionTransferCaseBase : MotionElement, IMotionTransferCase
    {
        #region IEnumerable<INativeMotionTransfer>

        public abstract IEnumerator<IMotionTransfer> GetEnumerator();

        #endregion

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IMotionInput

        public void Transmit(IMotionInfo info, double delta, IMotionOutput source)
        {
            foreach (var item in this)
                item.Transmit(info, delta, source);
        }

        public void OnCoupledTransfer(IMotionInfo info, double delta, IMotionTransferOutput source)
        {
            foreach (var item in this)
                item.OnCoupledTransfer(info, delta, source);
        }

        public void Reset()
        {
            foreach (var item in this)
                item.Reset();
        }

        #endregion

        #region IMotionTransferOutput

        public IMotionInfo MotionInfo => this.First().MotionInfo;

        public double Remainder
        {
            get { return this.Sum(item => item.Remainder); }
        }

        public bool Transfer(IMotionTarget target, object context)
        {
            return this.All(item => item.Transfer(target, context));
        }

        #endregion
    }

    #endregion

    #region NativeMotionTransferCaseBase

    public abstract class NativeMotionTransferCaseBase : ContentElement, INativeMotionTransferCase
    {
        #region IEnumerable<INativeMotionTransfer>

        public abstract IEnumerator<INativeMotionTransfer> GetEnumerator();

        #endregion

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region INativeMotionInput

        public void Transmit(IMotionInfo info, int nativeDelta, INativeMotionOutput source)
        {
            foreach (var item in this)
                item.Transmit(info, nativeDelta, source);
        }

        public void OnCoupledTransfer(IMotionInfo info, int nativeDelta, INativeMotionTransferOutput source)
        {
            foreach (var item in this)
                item.OnCoupledTransfer(info, nativeDelta, source);
        }

        public void Reset()
        {
            foreach (var item in this)
                item.Reset();
        }

        #endregion

        #region IMotionTransferOutput

        public IMotionInfo MotionInfo => this.First().MotionInfo;

        public double Remainder
        {
            get { return this.Sum(item => item.Remainder); }
        }

        public bool Transfer(IMotionTarget target, object context)
        {
            return this.All(item => item.Transfer(target, context));
        }

        #endregion

        #region INativeMotionTransferOutput

        public int NativeRemainder
        {
            get { return this.Sum(item => item.NativeRemainder); }
        }

        public bool Transfer(INativeMotionTarget target, object context)
        {
            return this.All(item => item.Transfer(target, context));
        }

        #endregion
    }

    #endregion

    #region MotionTransferCase

    public class MotionTransferCase : MotionTransferCaseBase
    {
        #region Fields

        private readonly List<IMotionTransfer> _items = new List<IMotionTransfer>();

        #endregion

        #region IEnumerable<INativeMotionTransfer>

        public override IEnumerator<IMotionTransfer> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        #endregion

        #region Methods

        public void Add(IMotionTransfer item)
        {
            _items.Add(item);
            item.SetParent(this);
        }

        public bool Remove(IMotionTransfer item)
        {
            var removed = _items.Remove(item);
            if (removed)
                item.SetParent(null);
            return removed;
        }

        #endregion
    }

    #endregion

    #region NativeMotionTransferCase

    public class NativeMotionTransferCase : NativeMotionTransferCaseBase
    {
        #region Fields

        private readonly List<INativeMotionTransfer> _items = new List<INativeMotionTransfer>();

        #endregion

        #region IEnumerable<INativeMotionTransfer>

        public override IEnumerator<INativeMotionTransfer> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        #endregion

        #region Methods

        public void Add(INativeMotionTransfer item)
        {
            _items.Add(item);
            item.SetParent(this);
        }

        public bool Remove(INativeMotionTransfer item)
        {
            var removed = _items.Remove(item);
            if (removed)
                item.SetParent(null);
            return removed;
        }

        #endregion
    }

    #endregion

    #region NativeCoupledMotionTransferCase

    public class NativeCoupledMotionTransferCase : NativeMotionTransferCase
    {
        #region Initialization

        public NativeCoupledMotionTransferCase()
        {
            AddHandler(NativeMotionTransfer.TransferedEvent, new NativeMotionTransferEventHandler(OnMotionTransfered));
        }

        #endregion

        #region Helpers

        private void OnMotionTransfered(object sender, NativeMotionTransferEventArgs e)
        {
            var source = e.Source as INativeMotionTransferOutput;
            Debug.Assert(source != null);
            foreach (var item in this)
                item.OnCoupledTransfer(e.MotionInfo, e.NativeDelta, source);
        }

        #endregion
    }

    #endregion
}