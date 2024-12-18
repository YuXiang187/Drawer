using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Drawer
{
    internal class MainForm : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public bool isRun = false;
        private static readonly string fontName = "微软雅黑";
        private const int HOTKEY_ID = 187;
        private static float dpiScale;

        private readonly StringPool pool;
        private readonly MainTray mainTray;
        private readonly Label mainLabel;
        private readonly ProgressBar progressBar;

        public MainForm(MainTray mainTray)
        {
            dpiScale = Graphics.FromHwnd(Handle).DpiX / 96f;
            this.mainTray = mainTray;
            pool = new StringPool();

            Text = "YuXiang Drawer";
            ClientSize = new Size((int)(450 * dpiScale), (int)(250 * dpiScale));
            TopMost = true;
            ControlBox = false;
            Load += MainForm_Load;
            FormBorderStyle = FormBorderStyle.None;
            AutoScaleMode = AutoScaleMode.Dpi;
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            Paint += MainForm_Paint;
            FormClosing += MainForm_FormClosing;
            ResumeLayout(false);

            // set location in center
            Rectangle screenArea = Screen.AllScreens.FirstOrDefault(s => s.Primary).WorkingArea;
            Location = new Point((screenArea.Width - Width) / 2, (screenArea.Height - Height) / 2);

            mainLabel = new Label
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.Transparent,
                ForeColor = Color.Black,
                Font = new Font(fontName, 60F, FontStyle.Bold, GraphicsUnit.Point, 134),
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
                RegisterHotKey(Handle, HOTKEY_ID, fsModifiers, vk);
            }
        }

        public void DisableHotkey()
        {
            UnregisterHotKey(Handle, HOTKEY_ID);
        }

        // set do not appear in Alt+Tab list
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
                Show();
                isRun = true;
                progressBar.Value = 100;
                mainLabel.ForeColor = Color.Gray;
                mainTray.notifyIcon.Icon = Properties.Resources.tray_stop;
                mainTray.FloatFormIcon(false);
                for (int i = 0; i < 8; i++)
                {
                    mainLabel.Text = pool.Get();
                    await Task.Delay(60);
                }
                mainLabel.ForeColor = Color.Black;
                pool.Remove(mainLabel.Text);
                pool.Save();
                mainTray.notifyIcon.Icon = Properties.Resources.tray_run;
                mainTray.FloatFormIcon(true);
                isRun = false;
                UnVisible();
            }
        }

        private async void UnVisible()
        {
            for (int i = 100; i >= 0; i--)
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

        // set background image white mask
        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(178, SystemColors.Control)))
            {
                e.Graphics.FillRectangle(brush, ClientRectangle);
            }
            ControlPaint.DrawBorder(e.Graphics, ClientRectangle, Color.DarkGray, ButtonBorderStyle.Solid);
        }

        // set background image
        private void MainForm_Load(object sender, EventArgs e)
        {
            string[] imageExtensions = { "png", "jpg", "jpeg", "bmp" };
            foreach (string extension in imageExtensions)
            {
                string imagePath = System.IO.Path.Combine(Application.StartupPath, $"background.{extension}");
                if (System.IO.File.Exists(imagePath))
                {
                    BackgroundImage = Image.FromFile(imagePath);
                    BackgroundImageLayout = ImageLayout.Zoom;
                    break;
                }
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
        }
    }
}