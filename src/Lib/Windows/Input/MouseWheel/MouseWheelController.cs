using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

// ReSharper disable once CheckNamespace
namespace Logitech.Windows.Input
{

    #region IMouseWheelController

    public interface IMouseWheelController
    {
        DependencyObject Element { get; }
        IInputElement InputElement { get; }
        IInputElement ExitElement { get; }

        void AddClient(IMouseWheelClient client);
    }

    #endregion

    #region MouseWheelController

    public partial class MouseWheelController : IMouseWheelController, IDisposable
    {
        #region Initialization

        public MouseWheelController(IInputLevelElement inputLevelElement)
        {
            if (inputLevelElement == null)
                throw new ArgumentNullException(nameof(inputLevelElement));
            InputLevelElement = inputLevelElement;

            InputLevelElement.PreviewMouseWheel += OnPreviewMouseWheel;
            InputLevelElement.AddHandler(PreviewMouseWheelInputEvent, new MouseWheelInputEventHandler(OnPreviewInput));
            InputLevelElement.AddHandler(MouseWheelInputEvent, new MouseWheelInputEventHandler(OnInput));
        }

        #endregion

        #region Queries

        public IInputLevelElement InputLevelElement { get; }

        #endregion

        #region IDisposable

        public virtual void Dispose()
        {
            InputLevelElement.PreviewMouseWheel -= OnPreviewMouseWheel;
            InputLevelElement.RemoveHandler(PreviewMouseWheelInputEvent, new MouseWheelInputEventHandler(OnPreviewInput));
            InputLevelElement.RemoveHandler(MouseWheelInputEvent, new MouseWheelInputEventHandler(OnInput));
            Unload();
        }

        #endregion

        #region Methods

        protected void Unload()
        {
            foreach (var client in _clients)
                client.Unload();
        }

        #endregion

        #region ClientType

        [Flags]
        private enum ClientType
        {
            Patch = 0x01,
            Adapter = 0x02
        }

        #endregion

        #region Constants

        public static readonly RoutedEvent PreviewMouseWheelInputEvent =
            EventManager.RegisterRoutedEvent("PreviewMouseWheelInput", RoutingStrategy.Tunnel,
                typeof(MouseWheelInputEventHandler), typeof(IInputElement));

        public static readonly RoutedEvent MouseWheelInputEvent = EventManager.RegisterRoutedEvent("MouseWheelInput",
            RoutingStrategy.Bubble, typeof(MouseWheelInputEventHandler), typeof(IInputElement));

        #endregion

        #region Fields

        private readonly List<IMouseWheelClient> _clients = new List<IMouseWheelClient>();
        private IInputElement _exitElement;
        private ClientType _clientType;

        #endregion

        #region IMouseWheelController

        public DependencyObject Element => InputLevelElement.Proxied;

        public IInputElement InputElement => Element as IInputElement;

        public IInputElement ExitElement => _exitElement ?? (_exitElement = Element.GetFirstVisualAncestorOfType<IInputElement>());

        public void AddClient(IMouseWheelClient client)
        {
            _clients.Add(client);
        }

        #endregion

        #region Helpers

        #region Queries

        #endregion

        #region Methods

        private IInputElement GetOriginalSource(MouseWheelEventArgs e)
        {
            if (e.OriginalSource is ContentElement)
            {
                var ie = Element as IInputElement;
                var pt = e.GetPosition(ie);
                if (ie is Visual)
                {
                    var result = VisualTreeHelper.HitTest(ie as Visual, pt);
                    return GetOriginalSource(result.VisualHit);
                }
                return null;
            }
            return GetOriginalSource(e.OriginalSource);
        }

        private IInputElement GetOriginalSource(object originalSource)
        {
            var source = originalSource as IInputElement;
            if (source != null) return source;
            return (originalSource as DependencyObject)?.GetFirstVisualAncestorOfType<IInputElement>();
        }

        #endregion

        #region Callbacks

        private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // update wheel motion info
            var wheel = e.MouseDevice.GetWheel();
            var timestamp = e.Timestamp < 0 ? 0 : e.Timestamp;
            var info = wheel.PreTransmit(timestamp, e.Delta);

            // 1. Tunneling event
            // Clients and behaviors use this tunneling event to update the wheel transfer
            // case by dynamically creating / retrieving motion shafts.
            var originalSource = GetOriginalSource(e);
            var inputEventArgs = new MouseWheelInputEventArgs(this, wheel, e)
            {
                RoutedEvent = PreviewMouseWheelInputEvent
            };
            originalSource.RaiseEvent(inputEventArgs);

            // In cooperation with clients and behaviors, if inputEventArgs.Handled is set to true,
            // the controller lets the underlying mouse wheel tunneling event continue its route.
            if (inputEventArgs.Handled)
                return;

            // Fill motion reservoir
            wheel.Transmit(info, e.Delta, null);
            // 2. Bubbling event
            // Clients consume the motion here
            inputEventArgs.RoutedEvent = MouseWheelInputEvent;
            originalSource.RaiseEvent(inputEventArgs);
            // 3. Remaining motion is processed here
            inputEventArgs.EndCommand();
            e.Handled = true;
        }

        private void OnPreviewInput(object sender, MouseWheelInputEventArgs e)
        {
            Debug.Assert(Equals(sender, Element));
            var client = _clients.FirstOrDefault(c => c.IsActive);
            client?.OnPreviewInput(sender, e);
        }

        private void OnInput(object sender, MouseWheelInputEventArgs e)
        {
            Debug.Assert(Equals(sender, Element));
            e.Controller = this;
            var client = _clients.FirstOrDefault(c => c.IsActive);
            if (client != null)
            {
                _exitElement = client.ExitElement;
                client.OnInput(sender, e);
            }
        }

        #endregion

        #endregion
    }

    #endregion

    #region MouseWheelController - repository

    public partial class MouseWheelController
    {
        #region Initialization

        static MouseWheelController()
        {
            InitializePatchClientFactories();
        }

        #endregion

        #region Constants

        private static readonly Func<IMouseWheelController, IMouseWheelClient> AdaptationClientFactory =
            controller => new MouseWheelAdaptationClient(controller);

        private static readonly Dictionary<Type, IEnumerable<Func<IMouseWheelController, IMouseWheelClient>>>
            PatchClientFactories = new Dictionary<Type, IEnumerable<Func<IMouseWheelController, IMouseWheelClient>>>();

        private static readonly Dictionary<DependencyObject, IMouseWheelController> Controllers =
            new Dictionary<DependencyObject, IMouseWheelController>();

        #endregion

        #region Methods

        internal static void BeginEnsurePatchController(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            BeginEnsureController(element, e, ClientType.Patch);
        }

        internal static void BeginEnsureMapController(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            BeginEnsureController(element, e, ClientType.Adapter);
        }

        #endregion

        #region Helpers

        #region Controller Repository

        private static void BeginEnsureController(DependencyObject element, DependencyPropertyChangedEventArgs e,
            ClientType clientType)
        {
            var inputLevelElement = InputLevelElementProxy.FromElement(element);
            if (inputLevelElement == null) return;

            element.Dispatcher.BeginInvoke(
                new PropertyChangedCallback((element1, e1) => EnsureController(inputLevelElement, element1, clientType)),
                DispatcherPriority.Loaded, element, e);
        }

        private static void EnsureController(IInputLevelElement inputLevelElement, DependencyObject element,
            ClientType clientType)
        {
            var clientFactories = GetClientFactories(element, clientType);
            var enumerable = clientFactories as Func<IMouseWheelController, IMouseWheelClient>[] ?? clientFactories.ToArray();
            if (enumerable.FirstOrDefault() == null) return;
            IMouseWheelController controller;
            if (Controllers.TryGetValue(element, out controller))
            {
                var mouseWheelController = controller as MouseWheelController;
                if (mouseWheelController != null && 0 == (mouseWheelController._clientType & clientType))
                {
                    foreach (var clientFactory in enumerable) controller.AddClient(clientFactory(controller));
                    ((MouseWheelController)controller)._clientType |= clientType;
                }
            }
            else
            {
                var controllerFactory = GetControllerFactory(inputLevelElement);
                Controllers[element] = controller = controllerFactory(element);
                var mouseWheelController = controller as MouseWheelController;
                if (mouseWheelController != null) mouseWheelController._clientType |= clientType;
                foreach (var clientFactory in enumerable) controller.AddClient(clientFactory(controller));
            }
        }

        #endregion

        #region Controller Factory

        private static Func<DependencyObject, IMouseWheelController> GetControllerFactory(
            IInputLevelElement inputLevelElement)
        {
            if (inputLevelElement is IFrameworkLevelElement)
                return s => new MouseWheelFrameworkLevelController(inputLevelElement as IFrameworkLevelElement);
            return s => new MouseWheelController(inputLevelElement);
        }

        #endregion

        #region Client Factories

        private static void InitializePatchClientFactories()
        {
            PatchClientFactories.Add(typeof(ScrollViewer), new Func<IMouseWheelController, IMouseWheelClient>[]
            {
                controller => new MouseWheelScrollClient(controller, Orientation.Vertical),
                controller => new MouseWheelScrollClient(controller, Orientation.Horizontal)
            });
            PatchClientFactories.Add(typeof(FlowDocumentPageViewer),
                new Func<IMouseWheelController, IMouseWheelClient>[]
                {
                    controller => new MouseWheelFlowDocumentPageViewerScrollClient(controller),
                    controller => new MouseWheelZoomClient(controller)
                });
            PatchClientFactories.Add(typeof(FlowDocumentScrollViewer),
                new Func<IMouseWheelController, IMouseWheelClient>[]
                {
                    controller => new MouseWheelZoomClient(controller)
                });
        }

        private static IEnumerable<Func<IMouseWheelController, IMouseWheelClient>> GetClientFactories(
             DependencyObject element, ClientType clientType)
        {
            if (0 != (clientType & ClientType.Patch))
            {
                var elementType = element.GetType();
                var key = PatchClientFactories.Keys.FirstOrDefault(t => t.IsAssignableFrom(elementType));
                if (key != null)
                {
                    foreach (var factory in PatchClientFactories[key])
                        yield return factory;
                }
            }
            if (0 != (clientType & ClientType.Adapter))
                yield return AdaptationClientFactory;
        }

        #endregion

        #endregion
    }

    #endregion
}