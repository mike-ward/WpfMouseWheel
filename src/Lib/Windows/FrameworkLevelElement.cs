using System;
using System.Windows;

namespace Logitech.Windows
{

    #region IFrameworkLevelElement

    /// <summary>
    ///     Represent common interface for FrameworkElement and FrameworkContentElement classes
    /// </summary>
    public interface IFrameworkLevelElement : IInputLevelElement
    {
        bool IsInitialized { get; }
        bool IsLoaded { get; }
        DependencyObject TemplatedParent { get; }
        event EventHandler Initialized;
        event RoutedEventHandler Loaded;
        event RoutedEventHandler Unloaded;
    }

    #endregion

    #region FrameworkLevelElementFactory

    /// <summary>
    ///     Acts as a factory for FrameworkElementProxy and FrameworkContentElementProxy
    /// </summary>
    public abstract class FrameworkLevelElementFactory : InputLevelElementProxy
    {
        #region Instance

        #region Initialization

        protected FrameworkLevelElementFactory(DependencyObject proxiedObject) : base(proxiedObject)
        {
        }

        #endregion

        #endregion

        #region Static

        public new static IFrameworkLevelElement FromElement(DependencyObject element)
        {
            IFrameworkLevelElement frameworkLevelElement = ZoomElementFactory.FromElement(element);
            if (frameworkLevelElement != null)
                return frameworkLevelElement;
            var proxied = element as FrameworkElement;
            if (proxied != null) return new FrameworkElementProxy(proxied);
            var contentElement = element as FrameworkContentElement;
            return contentElement != null ? new FrameworkContentElementProxy(contentElement) : null;
        }

        #endregion

        #region FrameworkElementProxy

        protected class FrameworkElementProxy : UiElementProxy, IFrameworkLevelElement
        {
            #region Initialization

            public FrameworkElementProxy(UIElement proxied) : base(proxied)
            {
            }

            #endregion

            #region Helpers

            private FrameworkElement Handle => Proxied as FrameworkElement;

            #endregion

            #region IFrameworkLevelElement

            #region Events

            public event EventHandler Initialized
            {
                add { Handle.Initialized += value; }
                remove { Handle.Initialized -= value; }
            }

            public event RoutedEventHandler Loaded
            {
                add { Handle.Loaded += value; }
                remove { Handle.Loaded -= value; }
            }

            public event RoutedEventHandler Unloaded
            {
                add { Handle.Unloaded += value; }
                remove { Handle.Unloaded -= value; }
            }

            #endregion

            #region Queries

            public bool IsInitialized => Handle.IsInitialized;

            public bool IsLoaded => Handle.IsLoaded;

            public DependencyObject TemplatedParent => Handle.TemplatedParent;

            #endregion

            #endregion
        }

        #endregion

        #region FrameworkContentElementProxy

        protected class FrameworkContentElementProxy : ContentElementProxy, IFrameworkLevelElement
        {
            #region Initialization

            public FrameworkContentElementProxy(FrameworkContentElement proxied) : base(proxied)
            {
            }

            #endregion

            #region Helpers

            private FrameworkContentElement Handle => Proxied as FrameworkContentElement;

            #endregion

            #region IFrameworkLevelElement

            #region Events

            public event EventHandler Initialized
            {
                add { Handle.Initialized += value; }
                remove { Handle.Initialized -= value; }
            }

            public event RoutedEventHandler Loaded
            {
                add { Handle.Loaded += value; }
                remove { Handle.Loaded -= value; }
            }

            public event RoutedEventHandler Unloaded
            {
                add { Handle.Unloaded += value; }
                remove { Handle.Unloaded -= value; }
            }

            #endregion

            #region Queries

            public bool IsInitialized => Handle.IsInitialized;

            public bool IsLoaded => Handle.IsLoaded;

            public DependencyObject TemplatedParent => Handle.TemplatedParent;

            #endregion

            #endregion
        }

        #endregion
    }

    #endregion
}