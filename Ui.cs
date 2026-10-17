using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Drawer
{
    internal static class Ui
    {
        internal const string FontName = "微软雅黑";
        internal const int DesignDpi = 96;

        internal static float DpiScale(Control control)
        {
            if (control == null || control.IsDisposed)
            {
                return 1F;
            }
            return control.DeviceDpi / (float)DesignDpi;
        }

        internal static int Px(Control control, int logical)
        {
            return (int)Math.Round(logical * DpiScale(control), MidpointRounding.AwayFromZero);
        }

        internal static Size Px(Control control, int width, int height)
        {
            return new Size(Px(control, width), Px(control, height));
        }

        internal static Font CreateFont(float size, FontStyle style = FontStyle.Regular)
        {
            return new Font(FontName, size, style, GraphicsUnit.Point, 134);
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
