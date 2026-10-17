using System;
using System.Windows.Forms;

namespace Drawer
{
    internal class DpiAwareForm : Form
    {
        private const int WM_DPICHANGED = 0x02E0;

        internal event EventHandler LayoutRequested;

        internal DpiAwareForm()
        {
            AutoScaleMode = AutoScaleMode.None;
        }

        internal void ApplyDpiLayout()
        {
            LayoutForDpi();
        }

        protected virtual void LayoutForDpi()
        {
            LayoutRequested?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            LayoutForDpi();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LayoutForDpi();
        }

        protected override void OnDpiChanged(DpiChangedEventArgs e)
        {
            base.OnDpiChanged(e);
            LayoutForDpi();
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_DPICHANGED)
            {
                LayoutForDpi();
            }
        }
    }
}
