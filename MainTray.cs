using Microsoft.Win32.TaskScheduler;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Windows.Forms;

namespace Drawer
{
    internal class MainTray
    {
        private readonly KeyValueStore store;
        private readonly MainForm mainForm;
        private EditForm editForm;
        private readonly FloatForm floatForm;

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
                store.Add("Key", "Yw5eKi//NQgt69jux/1HfQ==");
                store.Add("initPool", "OMpOcezBBlbG3U4oQTaooNuDCgXUQzz74B7FN6IAzE8=");
                store.Add("pool", "OMpOcezBBlbG3U4oQTaooNuDCgXUQzz74B7FN6IAzE8=");
            }

            mainForm = new MainForm(this);
            floatForm = new FloatForm(this);

            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
            hotKeyItem = new ToolStripMenuItem(Properties.Resources.menu_ctrl);
            hotKeyItem.Click += HotKeyItem_Click;
            floatFormItem = new ToolStripMenuItem(Properties.Resources.menu_float);
            floatFormItem.Click += FloatFormItem_Click;
            pauseItem = new ToolStripMenuItem(Properties.Resources.menu_pause);
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

            ToolStripMenuItem isAutoLaunchItem = new ToolStripMenuItem(Properties.Resources.menu_start_on_boot)
            {
                CheckOnClick = true,
                Checked = bool.Parse(store.Get("isAutoLaunch"))
            };
            isAutoLaunchItem.Click += IsAutoLaunchItem_Click;

            ToolStripMenuItem editItem = new ToolStripMenuItem(Properties.Resources.menu_edit);
            editItem.Click += EditItem_Click;

            _ = contextMenuStrip.Items.Add(hotKeyItem);
            _ = contextMenuStrip.Items.Add(floatFormItem);
            _ = contextMenuStrip.Items.Add(pauseItem);
            _ = contextMenuStrip.Items.Add(new ToolStripSeparator());
            _ = contextMenuStrip.Items.Add(isAutoLaunchItem);
            _ = contextMenuStrip.Items.Add(editItem);
            _ = contextMenuStrip.Items.Add(Properties.Resources.menu_statistics, null, CountItem_Click);
            _ = contextMenuStrip.Items.Add(Properties.Resources.menu_about, null, AboutItem_Click);
            _ = contextMenuStrip.Items.Add(new ToolStripSeparator());
            _ = contextMenuStrip.Items.Add(Properties.Resources.menu_exit, null, ExitItem_Click);

            notifyIcon = new NotifyIcon
            {
                Text = Properties.Resources.app_whole_name,
                Icon = Properties.Resources.tray_run,
                Visible = true,
                ContextMenuStrip = contextMenuStrip
            };
            notifyIcon.MouseClick += NotifyIcon_MouseClick;
        }

        private void EditItem_Click(object sender, EventArgs e)
        {
            string key = InputDialog.Show(Properties.Resources.menu_exit, Properties.Resources.dialog_enter_password, true);
            if (key != null)
            {
                EncryptString es = new EncryptString();
                if (key == es.Decrypt(store.Get("Key")))
                {
                    if (editForm == null || editForm.IsDisposed)
                    {
                        if (key == "123456")
                        {
                            _ = MessageBox.Show(string.Format(Properties.Resources.dialog_password_low_warning,"\n"), Properties.Resources.menu_edit, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        editForm = new EditForm();
                        editForm.Show();
                    }
                }
                else
                {
                    _ = MessageBox.Show(Properties.Resources.dialog_password_wrong, Properties.Resources.menu_edit, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
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

            string taskName = Properties.Resources.app_name;
            if (menuItem.Checked == true)
            {
                try
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
                catch (Exception ex)
                {
                    _ = MessageBox.Show(string.Format(Properties.Resources.dialog_permission_error, "\n", ex.Message), Properties.Resources.title_error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    menuItem.Checked = false;
                }
            }
            else
            {
                try
                {
                    using (TaskService taskService = new TaskService())
                    {
                        taskService.RootFolder.DeleteTask(taskName, false);
                    }
                    store.Update("isAutoLaunch", "false");
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show(string.Format(Properties.Resources.dialog_permission_error, "\n", ex.Message), Properties.Resources.title_error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    menuItem.Checked = false;
                }
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
            _ = MessageBox.Show(string.Format(Properties.Resources.dialog_about, "\n\n", "\n", "\n\n", "\n\n", "\n", "\n", "\n", "\n", "\n"), Properties.Resources.title_about, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void CountItem_Click(object sender, EventArgs e)
        {
            _ = MessageBox.Show(string.Format(Properties.Resources.dialog_statistics, "\n\n", StringPool.initPool.Count(), "\n", string.Join(", ", StringPool.initPool)), Properties.Resources.menu_statistics, MessageBoxButtons.OK, MessageBoxIcon.Information);
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