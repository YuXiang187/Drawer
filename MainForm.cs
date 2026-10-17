using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Drawer
{
    internal class MainForm : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        internal bool isRun = false;
        private const int HOTKEY_ID = 187;
        private const int MarginX = 12;
        private const int BaseWidth = 450;
        private const float FontSize = 60F;
        private const float MinFontSize = 10F;
        private readonly float dpiScale;

        private readonly StringPool pool;
        private readonly MainTray mainTray;
        private readonly Label mainLabel;
        private readonly ProgressBar progressBar;

        public MainForm(MainTray mainTray)
        {
            dpiScale = Ui.DpiScale(this);
            this.mainTray = mainTray;
            pool = new StringPool();

            // read the name list in the background
            Task.Run(() => GroupStore.Preload(GroupStore.GetActiveGroupId()));

            Text = "YuXiang Drawer";
            ClientSize = new Size((int)(BaseWidth * dpiScale), (int)(250 * dpiScale));
            TopMost = true;
            ControlBox = false;
            FormBorderStyle = FormBorderStyle.None;
            AutoScaleMode = AutoScaleMode.Dpi;
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            Paint += MainForm_Paint;
            FormClosing += MainForm_FormClosing;
            ResumeLayout(false);

            Ui.CenterOnPrimary(this);

            mainLabel = new Label
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.Transparent,
                ForeColor = Color.Black,
                Font = Ui.CreateFont(FontSize, FontStyle.Bold),
                Location = new Point((int)(12 * dpiScale), (int)(49 * dpiScale)),
                Size = new Size((int)(426 * dpiScale), (int)(153 * dpiScale)),
                Text = "LABEL",
                TextAlign = ContentAlignment.MiddleCenter
            };

            progressBar = new ProgressBar
            {
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Location = new Point((int)(12 * dpiScale), (int)(228 * dpiScale)),
                Size = new Size((int)(426 * dpiScale), (int)(10 * dpiScale)),
                Value = 0,
            };

            Controls.Add(mainLabel);
            Controls.Add(progressBar);
        }

        public void EnableHotKey(Keys currentHotkey)
        {
            if (currentHotkey != Keys.None)
            {
                uint fsModifiers = 0;
                if ((currentHotkey & Keys.Control) == Keys.Control)
                {
                    fsModifiers |= 0x0002;
                }
                if ((currentHotkey & Keys.Shift) == Keys.Shift)
                {
                    fsModifiers |= 0x0004;
                }
                if ((currentHotkey & Keys.Alt) == Keys.Alt)
                {
                    fsModifiers |= 0x0001;
                }

                uint vk = (uint)(currentHotkey & ~Keys.Control & ~Keys.Shift & ~Keys.Alt);
                UnregisterHotKey(Handle, HOTKEY_ID);
                if (RegisterHotKey(Handle, HOTKEY_ID, fsModifiers, vk) == 0)
                {
                    MessageBox.Show($"热键 {currentHotkey} 注册失败，可能已被其它程序占用。", "热键", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        public void DisableHotkey()
        {
            UnregisterHotKey(Handle, HOTKEY_ID);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x80;
                return cp;
            }
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_HOTKEY = 0x0312;
            if (m.Msg == WM_HOTKEY)
            {
                Run();
            }
            base.WndProc(ref m);
        }

        public async void Run()
        {
            if (isRun == false)
            {
                FitToNames();
                Show();
                isRun = true;
                progressBar.Value = 100;
                mainLabel.ForeColor = Color.Gray;
                mainTray.notifyIcon.Icon = Properties.Resources.tray_stop;
                mainTray.FloatFormIcon(false);
                for (int i = 0; i < 8 && !IsDisposed; i++)
                {
                    mainLabel.Text = i == 7 ? pool.Draw() : pool.Get();
                    // the drawn name stays on screen, so widen for it when the sample did not cover it
                    if (i == 7)
                    {
                        FitDrawn(mainLabel.Text);
                    }
                    await Task.Delay(60);
                }
                if (IsDisposed)
                {
                    // form was destroyed while animating (e.g. app is exiting)
                    return;
                }
                mainLabel.ForeColor = Color.Black;
                mainTray.notifyIcon.Icon = Properties.Resources.tray_run;
                mainTray.FloatFormIcon(true);
                isRun = false;
                UnVisible();
            }
        }

        // form width adapt the label text
        private void FitToNames()
        {
            FitToNames(pool.Sample(StringPool.SampleCount));
        }

        // widens the form only when the drawn name does not fit, so a short name does not shrink the window
        private void FitDrawn(string name)
        {
            if (mainLabel.Width >= TextRenderer.MeasureText(name, mainLabel.Font).Width)
            {
                return;
            }
            FitToNames(new List<string> { name });
        }

        // form width adapt the given names, a big list is estimated from a sample instead of measuring every entry
        private void FitToNames(List<string> names)
        {
            Screen screen = Ui.PrimaryScreen();
            int margin = (int)(MarginX * dpiScale);
            int limit = screen != null ? screen.WorkingArea.Width - margin * 2 : (int)(426 * dpiScale);

            float size = FontSize;
            int required = WidestName(names, size);
            while (required > limit && size > MinFontSize)
            {
                size = Math.Max(MinFontSize, size * limit / required);
                required = WidestName(names, size);
            }

            if (Math.Abs(size - mainLabel.Font.Size) > 0.1F)
            {
                mainLabel.Font = Ui.CreateFont(size, FontStyle.Bold);
            }

            int centerX = Location.X + ClientSize.Width / 2;
            int width = Math.Max((int)(BaseWidth * dpiScale), required + margin * 2);
            ClientSize = new Size(screen != null ? Math.Min(width, screen.WorkingArea.Width) : width, ClientSize.Height);
            Location = new Point(centerX - ClientSize.Width / 2, Location.Y);
        }

        // width requested for the widest of these names
        private static int WidestName(List<string> sample, float size)
        {
            using (Font probe = Ui.CreateFont(size, FontStyle.Bold))
            {
                int widest = 0;
                foreach (string name in sample)
                {
                    widest = Math.Max(widest, TextRenderer.MeasureText(name, probe).Width);
                }
                return widest;
            }
        }

        private async void UnVisible()
        {
            for (int i = 100; i >= 0 && !IsDisposed; i--)
            {
                progressBar.Value = i;
                if (isRun)
                {
                    progressBar.Value = 100;
                    break;
                }
                if (i == 0)
                {
                    Hide();
                }
                await Task.Delay(14);
            }
        }

        // draw form border
        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, ClientRectangle, Color.DarkGray, ButtonBorderStyle.Solid);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }
    }
}
