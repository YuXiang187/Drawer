using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Drawer
{
    internal class MainTray
    {
        private readonly KeyValueStore store;
        private readonly FloatForm floatForm;
        public readonly MainForm mainForm;
        private EditForm editForm;

        public NotifyIcon notifyIcon;
        public static ToolStripMenuItem hotKeyItem;
        public static ToolStripMenuItem floatFormItem;
        public static ToolStripMenuItem pauseItem;

        public MainTray()
        {
            store = new KeyValueStore();

            if (!File.Exists(Path.Combine(Application.StartupPath, "Drawer.config")))
            {
                store.Add("Mode", "0");
                store.Add("isAutoLaunch", "false");
                store.Add("Hotkey", "F8");
                store.Add("Key", "Yw5eKi//NQgt69jux/1HfQ==");
                store.Add("initPool", "OMpOcezBBlbG3U4oQTaooNuDCgXUQzz74B7FN6IAzE8=");
                store.Add("pool", "OMpOcezBBlbG3U4oQTaooNuDCgXUQzz74B7FN6IAzE8=");
            }

            mainForm = new MainForm(this);
            floatForm = new FloatForm(this);

            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
            hotKeyItem = new ToolStripMenuItem("热键");
            hotKeyItem.Click += HotKeyItem_Click;
            floatFormItem = new ToolStripMenuItem("浮窗");
            floatFormItem.Click += FloatFormItem_Click;
            pauseItem = new ToolStripMenuItem("暂停");
            pauseItem.Click += PauseItem_Click;

            switch (int.Parse(store.Get("Mode")))
            {
                case 0:
                    hotKeyItem.Enabled = false;
                    mainForm.EnableHotKey((Keys)Enum.Parse(typeof(Keys), new KeyValueStore().Get("Hotkey")));
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

            contextMenuStrip.Items.Add(hotKeyItem);
            contextMenuStrip.Items.Add(floatFormItem);
            contextMenuStrip.Items.Add(pauseItem);
            contextMenuStrip.Items.Add(new ToolStripSeparator());
            contextMenuStrip.Items.Add(isAutoLaunchItem);
            contextMenuStrip.Items.Add("设置", null, SettingItem_Click);
            contextMenuStrip.Items.Add("编辑", null, EditItem_Click);
            contextMenuStrip.Items.Add("统计", null, CountItem_Click);
            contextMenuStrip.Items.Add("关于", null, AboutItem_Click);
            contextMenuStrip.Items.Add(new ToolStripSeparator());
            contextMenuStrip.Items.Add("退出", null, ExitItem_Click);

            notifyIcon = new NotifyIcon
            {
                Text = "YuXiang Drawer",
                Icon = Properties.Resources.tray_run,
                Visible = true,
                ContextMenuStrip = contextMenuStrip
            };
            notifyIcon.MouseClick += NotifyIcon_MouseClick;
        }

        private void SettingItem_Click(object sender, EventArgs e)
        {
            Keys key = HotkeyDialog.GetKeys("设置", "更改热键：", store.Get("Hotkey"));
            if (key != Keys.None)
            {
                if (hotKeyItem.Enabled == false)
                {
                    mainForm.DisableHotkey();
                    mainForm.EnableHotKey(key);
                }
                store.Update("Hotkey", key.ToString());
                MessageBox.Show($"热键已更改为：\n{key}", "更改热键", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void EditItem_Click(object sender, EventArgs e)
        {
            string currectKey = new EncryptString().Decrypt(store.Get("Key"));
            string key = InputDialog.Show("编辑", "密码：", true);
            if (key != null)
            {
                if (key == currectKey)
                {
                    if (editForm == null || editForm.IsDisposed)
                    {
                        if (key == "123456")
                        {
                            MessageBox.Show("检测到您正在使用初始密码。\n为确保列表内容不被恶意篡改，请及时使用“密码”功能更换密码。", "编辑", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        editForm = new EditForm();
                        editForm.Show();
                    }
                    else
                    {
                        editForm.Focus();
                    }
                }
                else
                {
                    MessageBox.Show("密码错误。", "编辑", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mainForm.Run();
            }
        }

        private void IsAutoLaunchItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
            string appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string registryKey = @"Software\Microsoft\Windows\CurrentVersion\Run";

            if (menuItem.Checked == true)
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(registryKey, true))
                {
                    key?.SetValue("Drawer", $"\"{appPath}\"");
                }
                store.Update("isAutoLaunch", "true");
            }
            else
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(registryKey, true))
                {
                    key?.DeleteValue("Drawer", false);
                }
                store.Update("isAutoLaunch", "false");
            }
        }

        private void HotKeyItem_Click(object sender, EventArgs e)
        {
            hotKeyItem.Enabled = false;
            floatFormItem.Enabled = true;
            pauseItem.Enabled = true;
            mainForm.EnableHotKey((Keys)Enum.Parse(typeof(Keys), new KeyValueStore().Get("Hotkey")));
            floatForm.Hide();
            store.Update("Mode", "0");
        }

        private void FloatFormItem_Click(object sender, EventArgs e)
        {
            hotKeyItem.Enabled = true;
            floatFormItem.Enabled = false;
            pauseItem.Enabled = true;
            mainForm.DisableHotkey();
            floatForm.Location = new Point(Screen.PrimaryScreen.WorkingArea.Right - floatForm.Width - 22, Screen.PrimaryScreen.WorkingArea.Bottom - floatForm.Height - 16);
            floatForm.Show();
            store.Update("Mode", "1");
        }

        private void PauseItem_Click(object sender, EventArgs e)
        {
            hotKeyItem.Enabled = true;
            floatFormItem.Enabled = true;
            pauseItem.Enabled = false;
            mainForm.DisableHotkey();
            floatForm.Hide();
        }

        private void AboutItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("YuXiang Drawer：名称随机抽取器\n\n版本 4.0\n作者 YuXiang187\n\n“编辑”功能的初始密码为123456。\n\n软件支持设置背景图片，请将图片放于本软件的根目录下。\n图片大小推荐为450x250（9:5），名称为以下的任意一种：\n- background.jpg\n- background.jpeg\n- background.png\n- background.bmp", "关于 YuXiang Drawer", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void CountItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show($"统计结果如下。\n\n抽取数量：{StringPool.initPool.Count()}\n\n抽取名单：{string.Join(", ", StringPool.initPool)}", "统计", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ExitItem_Click(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            Application.Exit();
        }

        public void FloatFormIcon(bool isRunIcon)
        {
            floatForm.BackgroundImage = isRunIcon == true ? Properties.Resources.run : (Image)Properties.Resources.stop;
        }

        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            new MainTray();
            Application.Run();
        }
    }
}