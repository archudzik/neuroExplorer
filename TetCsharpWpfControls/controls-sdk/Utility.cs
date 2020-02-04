/*
 * Copyright (c) 2013-present, The Eye Tribe. 
 * All rights reserved.
 *
 * This source code is licensed under the BSD-style license found in the LICENSE file in the root directory of this source tree. 
 *
 */
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;


namespace EyeTribe.Controls
{
    public class Utility
    {
        #region Variabels

        private const float DPI_DEFAULT = 96f; // default system DIP setting
        private const int S_OK = 0;
        private const int MONITOR_DEFAULTTONEAREST = 2;
        private const int E_INVALIDARG = -2147024809;

        private enum DpiType
        {
            Effective = 0,
            Angular = 1,
            Raw = 2,
        }

        #endregion

        #region Enums

        private enum DeviceCap
        {
            /// <summary>
            /// Logical pixels inch in X
            /// </summary>
            LOGPIXELSX = 88,
            /// <summary>
            /// Logical pixels inch in Y
            /// </summary>
            LOGPIXELSY = 90
        }

        #endregion

        #region Public methods

        [Obsolete("GetSystemDpi has been deprecated, please use GetMonitorDpi(Screen screen) instead as monitors may have individual DPI settings from Windows 8.1 and forward.")]
        public static Point GetSystemDpi()
        {
            Point result = new Point();
            IntPtr hDc = GetDC(IntPtr.Zero);
            result.X = GetDeviceCaps(hDc, (int)DeviceCap.LOGPIXELSX);
            result.Y = GetDeviceCaps(hDc, (int)DeviceCap.LOGPIXELSY);
            ReleaseDC(IntPtr.Zero, hDc);
            return result;
        }

        public static double GetMonitorScaleDpi(Screen screen)
        {
            Point dpi = GetMonitorDpi(screen);
            return DPI_DEFAULT / dpi.X;
        }

        public static bool IsWindows81OrNewer()
        {
            var reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            string productName = (string)reg.GetValue("ProductName");
            return productName.Contains("Windows 8.1") || productName.Contains("Windows 10");
        }

        public static Point GetMonitorDpi(Screen screen)
        {
            if (IsWindows81OrNewer())
            {
                var point = new Point(screen.Bounds.Left + 1, screen.Bounds.Top + 1);
                var hmonitor = MonitorFromPoint(point, MONITOR_DEFAULTTONEAREST);

                uint dpiX, dpiY;
                DpiType type = DpiType.Effective; // see https://msdn.microsoft.com/en-us/library/windows/desktop/dn280511.aspx

                try
                {
                    switch (GetDpiForMonitor(hmonitor, type, out dpiX, out dpiY).ToInt32())
                    {
                        case S_OK:
                            return new Point(Convert.ToInt32(dpiX), Convert.ToInt32(dpiX));

                        case E_INVALIDARG:
                            Console.Out.WriteLine(
                                "Unable to fetch monitor DPI in Utiliy.cs, Invalid argument used to call Win32 function. See https://msdn.microsoft.com/en-us/library/windows/desktop/dn280510.aspx for more information.");
                            break;

                        default:
                            Console.Out.WriteLine(
                                "Unable to fetch monitor DPI in Utility.cs, unknown error. See https://msdn.microsoft.com/en-us/library/windows/desktop/dn280510.aspx for more information.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.Out.WriteLine(
                        "An exception occurred in Utility.cs, method GetMonitorDpi(Screen screen). Message: " +
                        ex.Message);
                }
            }
            // Fall back and general system-wide DPI for Windows 8 and earlier versions
            return GetSystemDpi();
        }

        #endregion

        #region Private methods (DLL Imports)

        [DllImport("gdi32.dll")]
        private static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDc);

        // Per monitor DPI awareness, see https://msdn.microsoft.com/en-us/library/windows/desktop/dn280510.aspx
        [DllImport("Shcore.dll")]
        private static extern IntPtr GetDpiForMonitor([In]IntPtr hmonitor, [In]DpiType dpiType, [Out]out uint dpiX, [Out]out uint dpiY);

        [DllImport("User32.dll")]
        private static extern IntPtr MonitorFromPoint([In]Point pt, [In]uint dwFlags);

        #endregion
    }
}
