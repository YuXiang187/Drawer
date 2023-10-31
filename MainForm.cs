using NHotkey;
using NHotkey.WindowsForms;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Drawer
{
    internal class MainForm : Form
    {
        public bool isRun = false;
        public static bool isHotKey = false;

        private readonly StringPool pool;
        private readonly MainTray mainTray;
        private readonly Label mainLabel;
        private readonly ProgressBar progressBar;

        public MainForm(MainTray mainTray)
        {
            this.mainTray = mainTray;
            pool = new StringPool();

            HotkeyManager.Current.AddOrReplace("Default", Keys.Control, OnHotKey);

            Name = "MainForm";
            Text = "YuXiang Drawer";
            ClientSize = new Size(450, 250);
            TopMost = true;
            ControlBox = false;
            Load += MainForm_Load;
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;
            ShowInTaskbar = false;
            Click += MainForm_Click;
            Paint += MainForm_Paint;
            FormClosing += MainForm_FormClosing;
            ResumeLayout(false);

            mainLabel = new Label
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.Transparent,
                ForeColor = Color.Black,
                Font = new Font("微软雅黑", 60F, FontStyle.Bold, GraphicsUnit.Point, 134),
                Location = new Point(12, 49),
                Name = "mainLabel",
                Size = new Size(426, 153),
                Text = "LABEL",
                TextAlign = ContentAlignment.MiddleCenter
            };
            mainLabel.Click += MainForm_Click;

            progressBar = new ProgressBar
            {
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Location = new Point(12, 228),
                Name = "progressBar",
                Size = new Size(426, 10),
                Value = 0,
            };
            progressBar.Click += MainForm_Click;

            Controls.Add(mainLabel);
            Controls.Add(progressBar);
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

        private void MainForm_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
        }

        private void OnHotKey(object sender, HotkeyEventArgs e)
        {
            if (isHotKey == true)
            {
                Run();
            }
            e.Handled = true;
        }
    }
}