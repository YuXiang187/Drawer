using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Drawer
{
    internal class MainTray
    {
        private readonly KeyValueStore store;
        private readonly FloatForm floatForm;
        private readonly MainForm mainForm;

        public NotifyIcon notifyIcon;
        public static ToolStripMenuItem hotKeyItem;
        public static ToolStripMenuItem floatFormItem;
        public static ToolStripMenuItem pauseItem;
        public MainTray()
        {
            store = new KeyValueStore("Drawer.config");

            if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Drawer.config")))
            {
                store.Add("Mode", "0");
                store.Add("isAutoLaunch", "false");
            }

            mainForm = new MainForm(this);
            floatForm = new FloatForm(this);

            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
            hotKeyItem = new ToolStripMenuItem("Ctrl键");
            hotKeyItem.Click += HotKeyItem_Click;
            floatFormItem = new ToolStripMenuItem("浮窗");
            floatFormItem.Click += FloatFormItem_Click;
            pauseItem = new ToolStripMenuItem("暂停");
            pauseItem.Click += PauseItem_Click;

            switch (int.Parse(store.Get("Mode")))
            {
                case 0:
                    hotKeyItem.Enabled = false;
                    MainForm.isHotKey = true;
                    break;
                case 1:
                    floatFormItem.Enabled = false;
                    floatForm.Show();
                    break;
            }

            ToolStripMenuItem isAutoLaunchItem = new ToolStripMenuItem("自启")
            {
                CheckOnClick = true,
                Checked = bool.Parse(store.Get("isAutoLaunch"))
            };
            isAutoLaunchItem.Click += IsAutoLaunchItem_Click;

            _ = contextMenuStrip.Items.Add(hotKeyItem);
            _ = contextMenuStrip.Items.Add(floatFormItem);
            _ = contextMenuStrip.Items.Add(pauseItem);
            _ = contextMenuStrip.Items.Add(new ToolStripSeparator());
            _ = contextMenuStrip.Items.Add(isAutoLaunchItem);
            _ = contextMenuStrip.Items.Add("关于", null, AboutItem_Click);
            _ = contextMenuStrip.Items.Add(new ToolStripSeparator());
            _ = contextMenuStrip.Items.Add("退出", null, ExitItem_Click);

            notifyIcon = new NotifyIcon
            {
                Text = "YuXiang Drawer",
                Icon = Properties.Resources.tray_run,
                Visible = true,
                ContextMenuStrip = contextMenuStrip
            };
            notifyIcon.MouseClick += NotifyIcon_MouseClick;
        }

        private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Run();
            }
        }

        private void IsAutoLaunchItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
            if (menuItem.Checked == true)
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                key.SetValue("Drawer", Process.GetCurrentProcess().MainModule.FileName);
                key.Close();
                store.Update("isAutoLaunch", "true");
            }
            else
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                key.DeleteValue("Drawer", false);
                key.Close();
                store.Update("isAutoLaunch", "false");
            }
        }

        private void HotKeyItem_Click(object sender, EventArgs e)
        {
            hotKeyItem.Enabled = false;
            floatFormItem.Enabled = true;
            pauseItem.Enabled = true;
            MainForm.isHotKey = true;
            floatForm.Hide();
            store.Update("Mode", "0");
        }

        private void FloatFormItem_Click(object sender, EventArgs e)
        {
            hotKeyItem.Enabled = true;
            floatFormItem.Enabled = false;
            pauseItem.Enabled = true;
            MainForm.isHotKey = false;
            floatForm.Location = new Point(Screen.PrimaryScreen.WorkingArea.Right - floatForm.Width - 22, Screen.PrimaryScreen.WorkingArea.Bottom - floatForm.Height - 16);
            floatForm.Show();
            store.Update("Mode", "1");
        }
        private void PauseItem_Click(object sender, EventArgs e)
        {
            hotKeyItem.Enabled = true;
            floatFormItem.Enabled = true;
            pauseItem.Enabled = false;
            MainForm.isHotKey = false;
            floatForm.Hide();
        }

        private void AboutItem_Click(object sender, EventArgs e)
        {
            _ = MessageBox.Show("版本 3.0 (2023.10.29)\n作者 YuXiang187\n\nYuXiang Drawer 是免费软件，如果你是花钱买的说明你被骗了。\n背景图片会自动缩放，推荐大小为 (450, 250) ，推荐比例为 9:5 。\n\n软件根目录下的文件结构：\nDrawer.exe - 软件本体\nNHotkey.dll - 软件运行库\nDrawer.config - 配置文件\nlist.txt - 加密名称列表\nbackground.png - 背景图片（可选）",
                "关于 YuXiang Drawer", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ExitItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("是否退出本软件？", "YuXiang Drawer", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                notifyIcon.Visible = false;
                Application.Exit();
            }
        }

        public void FloatFormIcon(bool isRunIcon)
        {
            floatForm.BackgroundImage = isRunIcon == true ? Properties.Resources.run : (System.Drawing.Image)Properties.Resources.stop;
        }

        public void Run()
        {
            mainForm.Run();
        }

        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            _ = new MainTray();
            Application.Run();
        }
    }
}