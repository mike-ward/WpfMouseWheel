using System;
using System.Windows;
using System.Windows.Input;
using Logitech.Windows.MotionFlow;

// ReSharper disable once CheckNamespace

namespace Logitech.Windows.Input
{

    #region IMouseWheelClient

    public interface IMouseWheelClient : IMouseWheelInputListener, IMotionTarget, IDisposable
    {
        IMouseWheelController Controller { get; }
        bool IsActive { get; }
        double MotionIncrement { get; }
        MouseWheelSmoothing Smoothing { get; }
        IInputElement ExitElement { get; }

        void Unload();
    }

    #endregion

    #region MouseWheelClient

    public abstract class MouseWheelClient : IMouseWheelClient
    {
        #region Initialization

        protected MouseWheelClient(IMouseWheelController controller)
        {
            if (controller == null) throw new ArgumentNullException(nameof(controller));
            Controller = controller;
        }

        #endregion

        #region Queries

        protected IMouseWheelInputListener Behavior
        {
            get
            {
                if (_behavior == null)
                {
                    EnsureLoaded();
                    _behavior = CreateBehavior();
                }
                return _behavior;
            }
        }

        #endregion

        #region Properties

        protected bool Enhanced
        {
            get { return _enhanced; }
            set
            {
                if (_enhanced == value) return;
                _enhanced = value;
                InvalidateBehavior();
            }
        }

        #endregion

        #region IDisposable

        public virtual void Dispose()
        {
            Unload();
        }

        #endregion

        #region Methods

        protected void InvalidateBehavior()
        {
            DisposeBehavior();
            _behavior = null;
        }

        #endregion

        #region Fields

        private IMouseWheelInputListener _behavior;
        private bool _enhanced;
        private bool _loaded;
        private IInputElement _exitElement;

        #endregion

        #region IMouseWheelClient

        public IMouseWheelController Controller { get; }

        public virtual bool IsActive
        {
            get
            {
                EnsureLoaded();
                return Modifiers == Keyboard.Modifiers;
            }
        }

        public virtual double MotionIncrement => 1.0;

        public virtual MouseWheelSmoothing Smoothing
        {
            get { return MouseWheelSmoothing.None; }
            protected set { throw new NotImplementedException(); }
        }

        public abstract ModifierKeys Modifiers { get; }

        public IInputElement ExitElement => _exitElement ?? (_exitElement = GetExitElement());

        public void Unload()
        {
            if (_loaded)
            {
                _loaded = false;
                OnUnloading();
                InvalidateBehavior();
            }
        }

        #endregion

        #region IMouseWheelInputListener

        public void OnPreviewInput(object sender, MouseWheelInputEventArgs e)
        {
            Behavior.OnPreviewInput(sender, e);
        }

        public void OnInput(object sender, MouseWheelInputEventArgs e)
        {
            Behavior.OnInput(sender, e);
        }

        #endregion

        #region IMotionTarget

        public virtual bool CanMove(IMotionInfo info, object context)
        {
            return true;
        }

        public virtual double Coerce(IMotionInfo info, object context, double delta)
        {
            return delta;
        }

        public virtual void Move(IMotionInfo info, object context, double delta)
        {
        }

        #endregion

        #region Overridables

        protected abstract IMouseWheelInputListener CreateBehavior();

        protected virtual IInputElement GetExitElement()
        {
            return Controller.Element.GetFirstVisualAncestorOfType<IInputElement>();
        }

        protected virtual void OnLoading()
        {
            _enhanced = MouseWheel.GetEnhanced(Controller.Element);
            MouseWheel.EnhancedProperty.AddValueChanged(Controller.Element, OnEnhancedChanged);
        }

        protected virtual void OnUnloading()
        {
            MouseWheel.EnhancedProperty.RemoveValueChanged(Controller.Element, OnEnhancedChanged);
        }

        #endregion

        #region Helpers

        private void EnsureLoaded()
        {
            if (!_loaded)
            {
                _loaded = true;
                OnLoading();
            }
        }

        private void DisposeBehavior()
        {
            (_behavior as IDisposable)?.Dispose();
        }

        private void OnEnhancedChanged(object sender, EventArgs e)
        {
            Enhanced = MouseWheel.GetEnhanced(sender as DependencyObject);
        }

        #endregion
    }

    #endregion
}