using System;
using System.Windows.Forms;

namespace Drawer
{
    internal class DpiAwareForm : Form
    {
        private int laidOutDpi;

        internal event EventHandler LayoutRequested;

        internal DpiAwareForm()
        {
            AutoScaleMode = AutoScaleMode.None;
        }

        protected virtual void LayoutForDpi()
        {
            LayoutRequested?.Invoke(this, EventArgs.Empty);
        }

        private void LayoutAtCurrentDpi()
        {
            laidOutDpi = Ui.Dpi(this);
            LayoutForDpi();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            LayoutAtCurrentDpi();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LayoutAtCurrentDpi();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            if (Ui.Dpi(this) != laidOutDpi)
            {
                LayoutAtCurrentDpi();
            }
        }

        protected override void OnDpiChanged(DpiChangedEventArgs e)
        {
            base.OnDpiChanged(e);
            LayoutAtCurrentDpi();
        }
    }
}
