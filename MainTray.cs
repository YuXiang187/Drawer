using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Drawer
{
    internal class MainTray
    {
        private static readonly Dictionary<string, string> DefaultConfig = new Dictionary<string, string>
        {
            { "Mode", "0" },
            { "isAutoLaunch", "false" },
            { "Hotkey", "F8" },
            { "Key", "Yw5eKi//NQgt69jux/1HfQ==" },
            { "initPool", "OMpOcezBBlbG3U4oQTaooNuDCgXUQzz74B7FN6IAzE8=" },
            { "pool", "OMpOcezBBlbG3U4oQTaooNuDCgXUQzz74B7FN6IAzE8=" },
        };

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

            // fill missing keys with defaults (covers first run and incomplete/corrupted config)
            foreach (KeyValuePair<string, string> entry in DefaultConfig)
            {
                if (store.Get(entry.Key) == null)
                {
                    store.Update(entry.Key, entry.Value);
                }
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

            if (!int.TryParse(store.Get("Mode"), out int mode))
            {
                mode = 0;
                store.Update("Mode", "0");
            }

            switch (mode)
            {
                case 0:
                    hotKeyItem.Enabled = false;
                    mainForm.EnableHotKey(GetStoredHotkey());
                    break;
                case 1:
                    floatFormItem.Enabled = false;
                    floatForm.Show();
                    break;
            }

            if (!bool.TryParse(store.Get("isAutoLaunch"), out bool isAutoLaunch))
            {
                isAutoLaunch = false;
                store.Update("isAutoLaunch", "false");
            }

            ToolStripMenuItem isAutoLaunchItem = new ToolStripMenuItem("自启")
            {
                CheckOnClick = true,
                Checked = isAutoLaunch
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
                try
                {
                    using (RegistryKey key = Registry.CurrentUser.OpenSubKey(registryKey, true))
                    {
                        key?.SetValue("Drawer", $"\"{appPath}\"");
                    }
                    store.Update("isAutoLaunch", "true");
                }
                catch (UnauthorizedAccessException ex)
                {
                    MessageBox.Show($"注册表修改失败。\n{ex.Message}", "自启", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    menuItem.Checked = false;
                }
            }
            else
            {
                try
                {
                    using (RegistryKey key = Registry.CurrentUser.OpenSubKey(registryKey, true))
                    {
                        key?.DeleteValue("Drawer", false);
                    }
                    store.Update("isAutoLaunch", "false");
                }
                catch (UnauthorizedAccessException ex)
                {
                    MessageBox.Show($"注册表修改失败。\n{ex.Message}", "自启", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    menuItem.Checked = false;
                }
            }
        }

        private void HotKeyItem_Click(object sender, EventArgs e)
        {
            hotKeyItem.Enabled = false;
            floatFormItem.Enabled = true;
            pauseItem.Enabled = true;
            mainForm.EnableHotKey(GetStoredHotkey());
            floatForm.Hide();
            store.Update("Mode", "0");
        }

        // parse the stored hotkey, falling back to F8 (and repairing the config) when missing or invalid
        private Keys GetStoredHotkey()
        {
            string value = store.Get("Hotkey");
            if (!string.IsNullOrEmpty(value))
            {
                try
                {
                    return (Keys)Enum.Parse(typeof(Keys), value);
                }
                catch (ArgumentException)
                {
                }
            }
            store.Update("Hotkey", "F8");
            return Keys.F8;
        }

        private void FloatFormItem_Click(object sender, EventArgs e)
        {
            hotKeyItem.Enabled = true;
            floatFormItem.Enabled = false;
            pauseItem.Enabled = true;
            mainForm.DisableHotkey();
            Screen screen = Screen.PrimaryScreen ?? Screen.AllScreens.FirstOrDefault();
            if (screen != null)
            {
                floatForm.Location = new Point(screen.WorkingArea.Right - floatForm.Width - 22, screen.WorkingArea.Bottom - floatForm.Height - 16);
            }
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
            MessageBox.Show("YuXiang Drawer：名称随机抽取器\n\n版本 4.1\n作者 YuXiang187\n\n“编辑”功能的初始密码为123456。", "关于 YuXiang Drawer", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            using (Mutex mutex = new Mutex(true, "Drawer", out bool createdNew))
            {
                if (createdNew)
                {
                    try
                    {
                        new MainTray();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"程序启动失败：\n{ex.Message}", "YuXiang Drawer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    Application.Run();
                }
                else
                {
                    MessageBox.Show("软件已经在运行！", "YuXiang Drawer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // last-resort handler for UI thread exceptions, keeps the app alive instead of crashing
        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            MessageBox.Show($"程序发生未处理的异常：\n{e.Exception.Message}", "YuXiang Drawer", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"程序发生致命异常，即将退出：\n{e.ExceptionObject}", "YuXiang Drawer", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}