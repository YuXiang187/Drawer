using Microsoft.Win32.TaskScheduler;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;

namespace Drawer
{
    internal class MainTray
    {
        private readonly KeyValueStore store;
        private readonly MainForm mainForm;
        private readonly FloatForm floatForm;

        public NotifyIcon notifyIcon;
        public static ToolStripMenuItem hotKeyItem;
        public static ToolStripMenuItem floatFormItem;
        public static ToolStripMenuItem pauseItem;
        public MainTray()
        {
            store = new KeyValueStore(Path.Combine(Application.StartupPath, "Drawer.config"));

            if (!File.Exists(Path.Combine(Application.StartupPath, "Drawer.config")))
            {
                store.Add("Mode", "0");
                store.Add("isAutoLaunch", "false");
                store.Add("Key", "wUNAPZUa+PwcYMPifFTPtcJO3i8UKBPHuaHc3caeLos=");
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
            _ = contextMenuStrip.Items.Add("统计", null, CountItem_Click);
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

            string taskName = "Drawer";
            if (menuItem.Checked == true)
            {
                using (TaskService taskService = new TaskService())
                {
                    TaskDefinition taskDefinition = taskService.NewTask();
                    _ = taskDefinition.Triggers.Add(new LogonTrigger { UserId = WindowsIdentity.GetCurrent().Name });
                    _ = taskDefinition.Actions.Add(new ExecAction(Path.Combine(Application.StartupPath, "Drawer.exe"), null));
                    taskDefinition.Principal.RunLevel = TaskRunLevel.Highest;
                    taskDefinition.Settings.Compatibility = TaskCompatibility.V2_3;
                    _ = taskService.RootFolder.RegisterTaskDefinition(taskName, taskDefinition);
                }
                store.Update("isAutoLaunch", "true");
            }
            else
            {
                using (TaskService taskService = new TaskService())
                {
                    taskService.RootFolder.DeleteTask(taskName, false);
                }
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
            _ = MessageBox.Show("YuXiang Drawer：名称随机抽取器\n\n版本 3.3\n作者 YuXiang187\n\n软件支持设置背景图片，请将图片放于本软件的根目录下。\n图片大小推荐为450x250（9:5），名称为以下的任意一种：\n- background.jpg\n- background.jpeg\n- background.png\n- background.bmp", "关于 YuXiang Drawer", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void CountItem_Click(object sender, EventArgs e)
        {
            StringBuilder result = new StringBuilder();
            foreach (string item in StringPool.GetInitList())
            {
                _ = result.Append(item.ToString() + ", ");
            }
            if (result.Length > 0)
            {
                result.Length -= 2;
            }
            _ = MessageBox.Show("统计结果如下。\n\n抽取数量：" + StringPool.GetInitList().Count() + "\n抽取名单：" + result.ToString(), "YuXiang Drawer", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            floatForm.BackgroundImage = isRunIcon == true ? Properties.Resources.run : (Image)Properties.Resources.stop;
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
            if (File.Exists(Path.Combine(Application.StartupPath, "list.txt")))
            {
                _ = new MainTray();
                Application.Run();

            }
            else
            {
                _ = MessageBox.Show("没有找到主列表文件(list.es)，软件启动失败。", "YuXiang Drawer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }
    }
}