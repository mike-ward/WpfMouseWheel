using System.Diagnostics;
using Logitech.Maths;

namespace Logitech.Windows.MotionFlow
{

    #region IMotionTransform

    public interface IMotionTransform : IMotionInput, IMotionOutput
    {
    }

    #endregion

    #region INativeMotionTransform

    public interface INativeMotionTransform : INativeMotionInput, INativeMotionOutput
    {
    }

    #endregion

    #region NativeMotionTransform

    public class NativeMotionTransform : MotionElementLink, INativeMotionTransform
    {
        #region Initialization

        public NativeMotionTransform()
        {
            Next = NativeMotionTerminal.Current;
        }

        #endregion

        #region INativeMotionOutput

        public virtual INativeMotionInput Next
        {
            get { return GetNext() as INativeMotionInput; }
            set { SetNext(value); }
        }

        #endregion

        #region Overridables

        protected virtual int Transform(IMotionInfo info, int nativeDelta)
        {
            return nativeDelta;
        }

        #endregion

        #region INativeMotionInput

        public virtual void Transmit(IMotionInfo info, int nativeDelta, INativeMotionOutput source)
        {
            var nativeDeltaY = Transform(info, nativeDelta);
            //Debug.WriteLine("{0}{1}(delta = {2,4}) --> {3,4}", Name, TransmitMethodSuffix(info, nativeDelta), nativeDelta, nativeDeltaY);
            Next.Transmit(info, nativeDeltaY, this);
        }

        public virtual void OnCoupledTransfer(IMotionInfo info, int nativeDelta, INativeMotionTransferOutput source)
        {
            Next.OnCoupledTransfer(info, nativeDelta, source);
        }

        public virtual void Reset()
        {
            Next.Reset();
        }

        #endregion
    }

    #endregion

    #region NativeDebouncedMotionTransformBase

    public class NativeDebouncedMotionTransformBase : NativeMotionTransform
    {
        #region Fields

        #endregion

        #region Initialization

        public NativeDebouncedMotionTransformBase(Int32DifferentialFunctionPatternModulator debouncingFunction)
        {
            DebouncingFunction = debouncingFunction;
        }

        #endregion

        #region Queries

        public Int32DifferentialFunctionPatternModulator DebouncingFunction { [DebuggerStepThrough] get; }

        #endregion

        #region NativeMotionTransform

        public override void Transmit(IMotionInfo info, int nativeDelta, INativeMotionOutput source)
        {
            var nativeDeltaY = Transform(info, nativeDelta);
            //Debug.WriteLine("{0}{1}(delta = {2,4}) --> {3,4}", Name, TransmitMethodSuffix(info, nativeDelta), nativeDelta, nativeDeltaY);
            Next.Transmit(info, nativeDeltaY, source);
        }

        public override void Reset()
        {
            DebouncingFunction.Reset();
            base.Reset();
        }

        protected override int Transform(IMotionInfo info, int nativeDelta)
        {
            return DebouncingFunction.DF(nativeDelta);
        }

        #endregion
    }

    #endregion

    #region NativeDebouncedMotionTransform

    public class NativeDebouncedMotionTransform : NativeDebouncedMotionTransformBase
    {
        #region Initialization

        public NativeDebouncedMotionTransform(Int32DifferentialFunctionPatternModulator debouncingFunction)
            : base(debouncingFunction)
        {
            Initialize();
        }

        #endregion

        #region CompensationTransform

        public class CompensationTransform : NativeDebouncedMotionTransformBase
        {
            #region Initialization

            public CompensationTransform(Int32DifferentialFunctionPatternModulator debouncingFunction)
                : base(debouncingFunction)
            {
                Name = Id.ToString("'r'00");
            }

            #endregion

            #region NativeMotionTransform

            public override INativeMotionInput Next
            {
                get { return GetNext(false) as INativeMotionInput; }
                set { SetNext(value, false); }
            }

            #endregion
        }

        #endregion

        #region Fields

        private INativeMotionTransform _compensationInput;
        private object _transferSource;

        #endregion

        #region NativeMotionTransform

        public override void OnCoupledTransfer(IMotionInfo info, int nativeDelta, INativeMotionTransferOutput source)
        {
            if (source != _transferSource)
                CompensationInput.Transmit(info, -nativeDelta, this);
        }

        public override void Reset()
        {
            _compensationInput?.Reset();
            base.Reset();
        }

        #endregion

        #region Helpers

        private INativeMotionInput CompensationInput => _compensationInput ?? (_compensationInput = new CompensationTransform(DebouncingFunction.Clone()) {Next = Next});

        private void Initialize()
        {
            Name = Id.ToString("'R'00");
            AddHandler(NativeMotionTransfer.TransferingEvent, new NativeMotionTransferEventHandler(OnMotionTransfering));
        }

        private void OnMotionTransfering(object sender, NativeMotionTransferEventArgs e)
        {
            _transferSource = e.Source;
        }

        #endregion
    }

    #endregion
}