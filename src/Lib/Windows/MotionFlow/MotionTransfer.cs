using System;
using System.Windows;

namespace Logitech.Windows.MotionFlow
{

    #region IMotionTransferOutput

    public interface IMotionTransferOutput
    {
        IMotionInfo MotionInfo { get; }
        double Remainder { get; }

        bool Transfer(IMotionTarget target, object context);
    }

    #endregion

    #region INativeMotionTransferOutput

    public interface INativeMotionTransferOutput : IMotionTransferOutput
    {
        int NativeRemainder { get; }

        bool Transfer(INativeMotionTarget target, object context);
    }

    #endregion

    #region IMotionTransfer

    public interface IMotionTransfer : IMotionInput, IMotionTransferOutput
    {
    }

    #endregion

    #region INativeMotionTransfer

    public interface INativeMotionTransfer : INativeMotionInput, INativeMotionTransferOutput
    {
    }

    #endregion

    #region MotionTransfer

    public class MotionTransfer : MotionElement, IMotionTransfer
    {
        #region Initialization

        public MotionTransfer()
        {
            Name = Id.ToString("'T'00");
        }

        #endregion

        #region Helpers

        #endregion

        #region Object

        public override string ToString() => $"{Name} : Remainder={Remainder}";

        #endregion

        #region Constants

        public static readonly RoutedEvent TransferingEvent = EventManager.RegisterRoutedEvent("Transfering",
            RoutingStrategy.Bubble, typeof(MotionTransferEventHandler), typeof(MotionTransfer));

        public static readonly RoutedEvent TransferedEvent = EventManager.RegisterRoutedEvent("Transfered",
            RoutingStrategy.Bubble, typeof(MotionTransferEventHandler), typeof(MotionTransfer));

        #endregion

        #region Fields

        #endregion

        #region IMotionInput

        public void Transmit(IMotionInfo info, double delta, IMotionOutput source)
        {
            if (info != null)
                MotionInfo = info;
            Remainder += delta;
        }

        public void OnCoupledTransfer(IMotionInfo info, double delta, IMotionTransferOutput source)
        {
            if (!Equals(source, this)) Remainder -= delta;
        }

        public void Reset()
        {
            Remainder = 0;
        }

        #endregion

        #region IMotionTransferOutput

        public IMotionInfo MotionInfo { get; private set; }
        public double Remainder { get; private set; }

        public bool Transfer(IMotionTarget target, object context)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            RaiseEvent(new MotionTransferEventArgs(MotionInfo, Remainder) { RoutedEvent = TransferingEvent });

            if (Math.Sign(Remainder) == MotionInfo.Direction)
            {
                var targetDelta = target.Coerce(MotionInfo, context, Remainder);
                if (!DoubleEx.IsZero(targetDelta))
                {
                    target.Move(MotionInfo, context, targetDelta);
                    Remainder -= targetDelta;
                    RaiseEvent(new MotionTransferEventArgs(MotionInfo, targetDelta) { RoutedEvent = TransferedEvent });
                }
            }
            return target.CanMove(MotionInfo, context);
        }

        #endregion
    }

    #endregion

    #region NativeMotionTransfer

    public class NativeMotionTransfer : MotionElement, INativeMotionTransfer
    {
        #region Initialization

        public NativeMotionTransfer()
        {
            Name = Id.ToString("'T'00");
        }

        #endregion

        #region Object

        public override string ToString()
        {
            return $"{Name}: Remaining={Remainder:F3}";
        }

        #endregion

        #region Helpers

        private void EndTransfer(int nativeTargetDelta, int resolutionFrequency)
        {
            NativeRemainder -= nativeTargetDelta;

            var nativeTransferDelta = nativeTargetDelta - _nativeTransferCreditDelta;
            var nativeTransferSnappedDelta = MathEx.Snap(nativeTransferDelta, resolutionFrequency);
            _nativeTransferCreditDelta = nativeTransferSnappedDelta - nativeTransferDelta;

            if (nativeTransferSnappedDelta != 0)
                RaiseEvent(new NativeMotionTransferEventArgs(MotionInfo, nativeTransferSnappedDelta)
                {
                    RoutedEvent = TransferedEvent
                });
        }

        #endregion

        #region Constants

        public static readonly RoutedEvent TransferingEvent = EventManager.RegisterRoutedEvent("Transfering",
            RoutingStrategy.Bubble, typeof(NativeMotionTransferEventHandler), typeof(NativeMotionTransfer));

        public static readonly RoutedEvent TransferedEvent = EventManager.RegisterRoutedEvent("Transfered",
            RoutingStrategy.Bubble, typeof(NativeMotionTransferEventHandler), typeof(NativeMotionTransfer));

        #endregion

        #region Fields

        private int _nativeTransferCreditDelta;

        #endregion

        #region IMotionInput

        public virtual void Transmit(IMotionInfo info, int nativeDelta, INativeMotionOutput source)
        {
            if (info != null)
                MotionInfo = info;
            NativeRemainder += nativeDelta;
            //if (nativeDelta != 0)
            //  Debug.WriteLine("{0}{1}(delta = {2,4}) --> {3,4}{4}",
            //    Name, TransmitMethodSuffix(info, nativeDelta),
            //    nativeDelta, _nativeSourceDelta,
            //    _nativeSourceDelta != 0 && Math.Sign(_nativeSourceDelta) != info.NativeDirection ? " (##)" : "");
        }

        public void OnCoupledTransfer(IMotionInfo info, int nativeDelta, INativeMotionTransferOutput source)
        {
            if (!Equals(source, this))
            {
                NativeRemainder -= nativeDelta;
                //if (nativeDelta != 0)
                //  Debug.WriteLine("{0}{1}(delta = {2,4}) --> {3,4}{4}",
                //    Name, TransmitMethodSuffix(info, nativeDelta),
                //    nativeDelta, _nativeSourceDelta,
                //    _nativeSourceDelta != 0 && Math.Sign(_nativeSourceDelta) != info.NativeDirection ? " (##)" : "");
            }
        }

        public void Reset()
        {
            NativeRemainder = _nativeTransferCreditDelta = 0;
        }

        #endregion

        #region IMotionTransferOutput

        public IMotionInfo MotionInfo { get; private set; }

        public double Remainder => ((INativeMotionConverter)MotionInfo?.Source)?.NativeToNormalized(NativeRemainder) ?? 0;

        public bool Transfer(IMotionTarget target, object context)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            RaiseEvent(new NativeMotionTransferEventArgs(MotionInfo, NativeRemainder) { RoutedEvent = TransferingEvent });

            if (Math.Sign(NativeRemainder) == MotionInfo.NativeDirection)
            {
                var converter = (INativeMotionConverter)MotionInfo.Source;
                var sourceDelta = converter.NativeToNormalized(NativeRemainder);
                var targetDelta = target.Coerce(MotionInfo, context, sourceDelta);
                if (!DoubleEx.IsZero(targetDelta))
                {
                    target.Move(MotionInfo, context, targetDelta);
                    EndTransfer(converter.NormalizedToNative(targetDelta), converter.NativeResolutionFrequency);
                }
            }
            return target.CanMove(MotionInfo, context);
        }

        #endregion

        #region INativeMotionTransferOutput

        public int NativeRemainder { get; private set; }

        public bool Transfer(INativeMotionTarget target, object context)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            RaiseEvent(new NativeMotionTransferEventArgs(MotionInfo, NativeRemainder) { RoutedEvent = TransferingEvent });

            if (Math.Sign(NativeRemainder) == MotionInfo.NativeDirection)
            {
                var converter = (INativeMotionConverter)MotionInfo.Source;
                var nativeTargetDelta = target.Coerce(MotionInfo, context, NativeRemainder);
                if (!DoubleEx.IsZero(nativeTargetDelta))
                {
                    target.Move(MotionInfo, context, nativeTargetDelta);
                    EndTransfer(nativeTargetDelta, converter.NativeResolutionFrequency);
                }
            }
            return target.CanMove(MotionInfo, context);
        }

        #endregion
    }

    #endregion
}