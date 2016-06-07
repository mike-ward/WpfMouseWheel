using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Win32;

namespace Logitech.Windows
{
    public static class SystemParametersEx
    {
        #region Fields

        private static int _wheelScrollChars = -1;

        #endregion

        #region Queries

        public static int WheelScrollChars
        {
            get
            {
                if (_wheelScrollChars < 0)
                {
                    lock (SyncMouse)
                    {
                        if (_wheelScrollChars < 0)
                        {
                            if (!SystemParametersInfo(0x006C, 0, ref _wheelScrollChars, 0))
                                throw new Win32Exception();
                        }
                    }
                }
                return _wheelScrollChars;
            }
        }

        #endregion

        #region Initialization

        static SystemParametersEx()
        {
            SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;
        }

        internal static void Initialize()
        {
        }

        #endregion

        #region Helpers

        private static object SyncMouse => typeof(SystemParametersEx);

        private static void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            if (e.Category == UserPreferenceCategory.Mouse)
                lock (SyncMouse)
                    _wheelScrollChars = -1;
        }

        [SuppressUnmanagedCodeSecurity, SecurityCritical,
         DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SystemParametersInfo(int nAction, int nParam, ref int value, int ignore);

        #endregion
    }
}