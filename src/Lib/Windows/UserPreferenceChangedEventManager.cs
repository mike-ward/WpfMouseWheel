using System;
using System.Windows;
using Microsoft.Win32;

namespace Logitech.Windows
{
    public class UserPreferenceChangedEventManager : WeakEventManager
    {
        #region Static

        #region Queries

        private static UserPreferenceChangedEventManager CurrentManager
        {
            get
            {
                SystemParametersEx.Initialize();
                var managerType = typeof(UserPreferenceChangedEventManager);
                var currentManager = (UserPreferenceChangedEventManager) GetCurrentManager(managerType);
                if (currentManager == null)
                {
                    currentManager = new UserPreferenceChangedEventManager();
                    SetCurrentManager(managerType, currentManager);
                }
                return currentManager;
            }
        }

        #endregion

        #region Methods

        public static void AddListener(IWeakEventListener listener)
        {
            if (listener == null)
                throw new ArgumentNullException(nameof(listener));
            CurrentManager.ProtectedAddListener(null, listener);
        }

        public static void RemoveListener(IWeakEventListener listener)
        {
            if (listener == null)
                throw new ArgumentNullException(nameof(listener));
            CurrentManager.ProtectedRemoveListener(null, listener);
        }

        #endregion

        #endregion

        #region Instance

        #region Initialization

        private UserPreferenceChangedEventManager()
        {
        }

        #endregion

        #region WeakEventManager

        protected override void StartListening(object source)
        {
            SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;
        }

        protected override void StopListening(object source)
        {
            SystemEvents.UserPreferenceChanged -= OnUserPreferenceChanged;
        }

        #endregion

        #region Helpers

        private void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            DeliverEvent(null, e);
        }

        #endregion

        #endregion
    }
}