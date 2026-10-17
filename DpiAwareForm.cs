using System;
using System.Windows.Forms;

namespace Drawer
{
    internal class DpiAwareForm : Form
    {
        internal event EventHandler LayoutRequested;

        internal DpiAwareForm()
        {
            AutoScaleMode = AutoScaleMode.None;
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
    }
}
