using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Drawer
{
    internal static class Ui
    {
        internal const string FontName = "微软雅黑";
        internal const int DesignDpi = 96;

        private const int LogPixelsY = 88;

        [DllImport("user32.dll")]
        private static extern uint GetDpiForWindow(IntPtr hwnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("gdi32.dll")]
        private static extern int GetDeviceCaps(IntPtr hdc, int index);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

        private static readonly Dictionary<string, Font> fontCache = new Dictionary<string, Font>();
        private static bool windowDpiSupported = true;
        private static int screenDpi;

        internal static int Dpi(Control control)
        {
            if (control == null || control.IsDisposed)
            {
                return ScreenDpi;
            }

            Control top = control.TopLevelControl;
            if (top == null || top.IsDisposed)
            {
                top = control;
            }

            if (windowDpiSupported && top.IsHandleCreated)
            {
                try
                {
                    // DeviceDpi stays at 96 for a window that was created on a high DPI monitor,
                    // only the window itself knows the DPI of its monitor
                    uint dpi = GetDpiForWindow(top.Handle);
                    if (dpi >= 48)
                    {
                        return (int)dpi;
                    }
                }
                catch (EntryPointNotFoundException)
                {
                    // Windows before 10 1607 has no GetDpiForWindow, fall back to what WinForms knows
                    windowDpiSupported = false;
                }
            }

            return top.IsHandleCreated ? Math.Max(DesignDpi, top.DeviceDpi) : ScreenDpi;
        }

        // DPI of the primary monitor, used while a window has no handle yet
        private static int ScreenDpi
        {
            get
            {
                if (screenDpi == 0)
                {
                    IntPtr dc = GetDC(IntPtr.Zero);
                    int dpi = dc != IntPtr.Zero ? GetDeviceCaps(dc, LogPixelsY) : DesignDpi;
                    if (dc != IntPtr.Zero)
                    {
                        ReleaseDC(IntPtr.Zero, dc);
                    }
                    screenDpi = dpi >= 48 ? dpi : DesignDpi;
                }
                return screenDpi;
            }
        }

        internal static int Px(Control control, int logical)
        {
            return (int)Math.Round(logical * Dpi(control) / (double)DesignDpi, MidpointRounding.AwayFromZero);
        }

        internal static Size Px(Control control, int width, int height)
        {
            return new Size(Px(control, width), Px(control, height));
        }

        internal static Font CreateFont(Control control, float logicalPoints, FontStyle style = FontStyle.Regular)
        {
            return CreateFont(Dpi(control), logicalPoints, style);
        }

        internal static Font CreateFont(int dpi, float logicalPoints, FontStyle style = FontStyle.Regular)
        {
            string key = string.Format(CultureInfo.InvariantCulture, "{0:0.##}/{1}/{2}", logicalPoints, (int)style, dpi);
            Font font;
            if (!fontCache.TryGetValue(key, out font))
            {
                font = CreateDetachedFont(dpi, logicalPoints, style);
                fontCache[key] = font;
            }
            return font;
        }

        // a font the caller owns, for the one-off sizes used while measuring text
        internal static Font CreateDetachedFont(Control control, float logicalPoints, FontStyle style)
        {
            return CreateDetachedFont(Dpi(control), logicalPoints, style);
        }

        private static Font CreateDetachedFont(int dpi, float logicalPoints, FontStyle style)
        {
            return new Font(FontName, logicalPoints * dpi / 72F, style, GraphicsUnit.Pixel, 134);
        }

        internal static Screen PrimaryScreen()
        {
            return Screen.PrimaryScreen ?? Screen.AllScreens.FirstOrDefault();
        }

        internal static void CenterOnPrimary(Form form)
        {
            Screen screen = PrimaryScreen();
            if (screen == null)
            {
                return;
            }
            Rectangle area = screen.WorkingArea;
            form.Location = new Point(area.Left + (area.Width - form.Width) / 2, area.Top + (area.Height - form.Height) / 2);
        }
    }
}
