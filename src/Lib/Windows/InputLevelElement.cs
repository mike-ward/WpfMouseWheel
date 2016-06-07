using System;
using System.Windows;
using System.Windows.Input;

namespace Logitech.Windows
{

    #region IInputLevelElement

    /// <summary>
    ///     Represent common interface for ContentElement and UIElement and UIElement3D classes
    /// </summary>
    public interface IInputLevelElement : IEquatable<IInputLevelElement>, IEquatable<DependencyObject>
    {
        DependencyObject Proxied { get; }
        InputBindingCollection InputBindings { get; }
        event MouseWheelEventHandler PreviewMouseWheel;
        event MouseWheelEventHandler MouseWheel;

        void RaiseEvent(RoutedEventArgs e);
        void AddHandler(RoutedEvent routedEvent, Delegate handler);
        void RemoveHandler(RoutedEvent routedEvent, Delegate handler);
    }

    #endregion

    #region InputLevelElementProxy

    /// <summary>
    ///     Acts as a base class and factory for ContentElementProxy, UIElementProxy and UIElement3DProxy
    /// </summary>
    public abstract class InputLevelElementProxy : IEquatable<IInputLevelElement>, IEquatable<DependencyObject>
    {
        #region Static

        public static IInputLevelElement FromElement(DependencyObject element)
        {
            IInputLevelElement inputLevelElement = FrameworkLevelElementFactory.FromElement(element);
            if (inputLevelElement != null)
                return inputLevelElement;

            var proxied = element as UIElement;
            if (proxied != null) return new UiElementProxy(proxied);
            var contentElement = element as ContentElement;
            if (contentElement != null) return new ContentElementProxy(contentElement);
            var d = element as UIElement3D;
            return d != null ? new UIElement3DProxy(d) : null;
        }

        #endregion

        #region UIElementProxy

        protected class UiElementProxy : InputLevelElementProxy, IInputLevelElement
        {
            #region Initialization

            public UiElementProxy(UIElement proxied) : base(proxied)
            {
            }

            #endregion

            #region Helpers

            private UIElement Handle => Proxied as UIElement;

            #endregion

            #region IInputLevelElement

            #region Events

            public event MouseWheelEventHandler PreviewMouseWheel
            {
                add { Handle.PreviewMouseWheel += value; }
                remove { Handle.PreviewMouseWheel -= value; }
            }

            public event MouseWheelEventHandler MouseWheel
            {
                add { Handle.MouseWheel += value; }
                remove { Handle.MouseWheel -= value; }
            }

            #endregion

            #region Queries

            public InputBindingCollection InputBindings => Handle.InputBindings;

            #endregion

            #region Methods

            public void RaiseEvent(RoutedEventArgs e)
            {
                Handle.RaiseEvent(e);
            }

            public void AddHandler(RoutedEvent routedEvent, Delegate handler)
            {
                Handle.AddHandler(routedEvent, handler);
            }

            public void RemoveHandler(RoutedEvent routedEvent, Delegate handler)
            {
                Handle.RemoveHandler(routedEvent, handler);
            }

            #endregion

            #endregion
        }

        #endregion

        #region ContentElementProxy

        protected class ContentElementProxy : InputLevelElementProxy, IInputLevelElement
        {
            #region Initialization

            public ContentElementProxy(ContentElement proxied) : base(proxied)
            {
            }

            #endregion

            #region Helpers

            private ContentElement Handle => Proxied as ContentElement;

            #endregion

            #region IInputLevelElement

            #region Events

            public event MouseWheelEventHandler PreviewMouseWheel
            {
                add { Handle.PreviewMouseWheel += value; }
                remove { Handle.PreviewMouseWheel -= value; }
            }

            public event MouseWheelEventHandler MouseWheel
            {
                add { Handle.MouseWheel += value; }
                remove { Handle.MouseWheel -= value; }
            }

            #endregion

            #region Queries

            public InputBindingCollection InputBindings => Handle.InputBindings;

            #endregion

            #region Methods

            public void RaiseEvent(RoutedEventArgs e)
            {
                Handle.RaiseEvent(e);
            }

            public void AddHandler(RoutedEvent routedEvent, Delegate handler)
            {
                Handle.AddHandler(routedEvent, handler);
            }

            public void RemoveHandler(RoutedEvent routedEvent, Delegate handler)
            {
                Handle.RemoveHandler(routedEvent, handler);
            }

            #endregion

            #endregion
        }

        #endregion

        #region UIElement3DProxy

        protected class UIElement3DProxy : InputLevelElementProxy, IInputLevelElement
        {
            #region Initialization

            public UIElement3DProxy(UIElement3D proxied) : base(proxied)
            {
            }

            #endregion

            #region Helpers

            private UIElement3D Handle => Proxied as UIElement3D;

            #endregion

            #region IInputLevelElement

            #region Events

            public event MouseWheelEventHandler PreviewMouseWheel
            {
                add { Handle.PreviewMouseWheel += value; }
                remove { Handle.PreviewMouseWheel -= value; }
            }

            public event MouseWheelEventHandler MouseWheel
            {
                add { Handle.MouseWheel += value; }
                remove { Handle.MouseWheel -= value; }
            }

            #endregion

            #region Queries

            public InputBindingCollection InputBindings => Handle.InputBindings;

            #endregion

            #region Methods

            public void RaiseEvent(RoutedEventArgs e)
            {
                Handle.RaiseEvent(e);
            }

            public void AddHandler(RoutedEvent routedEvent, Delegate handler)
            {
                Handle.AddHandler(routedEvent, handler);
            }

            public void RemoveHandler(RoutedEvent routedEvent, Delegate handler)
            {
                Handle.RemoveHandler(routedEvent, handler);
            }

            #endregion

            #endregion
        }

        #endregion

        #region Instance

        #region Initialization

        protected InputLevelElementProxy(DependencyObject proxiedObject)
        {
            Proxied = proxiedObject;
        }

        #endregion

        #region Object

        public override string ToString()
        {
            return Proxied.ToString();
        }

        public override int GetHashCode()
        {
            return Proxied.GetHashCode();
        }

        public sealed override bool Equals(object obj)
        {
            var a = obj as IInputLevelElement;
            if (a != null) return Equals(a);
            return Equals(obj as DependencyObject);
        }

        #endregion

        #region IEquatable<IInputLevelElement>

        public bool Equals(IInputLevelElement other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (ReferenceEquals(other, null)) return false;
            return ReferenceEquals(Proxied, other.Proxied);
        }

        #endregion

        #region IEquatable<DependencyObject>

        public bool Equals(DependencyObject other)
        {
            return ReferenceEquals(Proxied, other);
        }

        #endregion

        #region Properties

        public DependencyObject Proxied { get; }

        #endregion

        #endregion
    }

    #endregion
}