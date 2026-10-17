using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Drawer
{
    internal static class Ui
    {
        internal const string FontName = "微软雅黑";

        internal static float DpiScale(Control control)
        {
            return Graphics.FromHwnd(control.Handle).DpiX / 96f;
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
