﻿using System;
using System.Drawing;
using System.Windows.Forms;

namespace Drawer
{
    internal class FloatForm : Form
    {
        private Point offset;
        private bool isDragging = false;
        private readonly MainTray mainTray;
        private readonly ContextMenuStrip contextMenu;
        private static float dpiScale;

        public FloatForm(MainTray mainTray)
        {
            dpiScale = Graphics.FromHwnd(Handle).DpiX / 96f;
            this.mainTray = mainTray;

            Text = "Drawer";
            TopMost = true;
            ControlBox = false;
            Load += FloatForm_Load;
            FormBorderStyle = FormBorderStyle.None;
            Paint += FloatForm_Paint;
            FormClosing += FloatForm_FormClosing;
            ShowInTaskbar = false;
            MouseMove += FloatForm_MouseMove;
            MouseUp += FloatForm_MouseUp;
            MouseClick += FloatForm_MouseClick;
            Size = new Size((int)(38 * dpiScale), (int)(38 * dpiScale));
            AutoScaleMode = AutoScaleMode.Dpi;
            StartPosition = FormStartPosition.Manual;
            Location = new Point(Screen.PrimaryScreen.WorkingArea.Right - Width - (int)(28 * dpiScale), Screen.PrimaryScreen.WorkingArea.Bottom - Height - (int)(16 * dpiScale));
            ResumeLayout(false);

            // MouseMenu
            ToolStripMenuItem moveItem = new ToolStripMenuItem("移动");
            moveItem.Click += Item_Click;
            ToolStripMenuItem closeItem = new ToolStripMenuItem("关闭");
            closeItem.Click += Item_Click;
            contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add(moveItem);
            contextMenu.Items.Add(closeItem);
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

        private void FloatForm_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, ClientRectangle, Color.DarkGray, ButtonBorderStyle.Solid);
        }

        private void Item_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
            if (menuItem.Text == "移动")
            {
                isDragging = true;
            }
            else if (menuItem.Text == "关闭")
            {
                MainTray.hotKeyItem.Enabled = true;
                MainTray.floatFormItem.Enabled = true;
                MainTray.pauseItem.Enabled = false;
                Hide();
            }
        }

        private void FloatForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            MainTray.hotKeyItem.Enabled = true;
            MainTray.floatFormItem.Enabled = true;
            MainTray.pauseItem.Enabled = false;
            Hide();
        }

        private void FloatForm_Load(object sender, EventArgs e)
        {
            BackgroundImage = Properties.Resources.run;
            BackgroundImageLayout = ImageLayout.Zoom;
        }

        private void FloatForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && isDragging == false)
            {
                Point location = new Point(e.X, e.Y);
                contextMenu.Show(this, location);
            }
            else if (isDragging == false)
            {
                mainTray.mainForm.Run();
            }
        }

        private void FloatForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
            }
        }

        private void FloatForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point newLocation = PointToScreen(new Point(e.X, e.Y));
                newLocation.Offset(-offset.X - (Size.Width / 2), -offset.Y - (Size.Height / 2));
                Location = newLocation;
            }
        }
    }
}