using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Drawer
{
    internal class StatsForm
    {
        private static Form current;

        // shows the whole list in a scrollable window, a MessageBox cuts it off with an ellipsis
        public static void Show(string groupName, List<string> names)
        {
            // a click while the window is already up brings it back instead of opening a second one
            if (current != null && !current.IsDisposed)
            {
                current.Activate();
                return;
            }

            using (Form form = new Form())
            {
                current = form;
                float dpiScale = Ui.DpiScale(form);

                form.Text = "统计";
                form.Icon = Properties.Resources.tray_run;
                form.ClientSize = new Size((int)(430 * dpiScale), (int)(400 * dpiScale));
                form.MinimumSize = new Size((int)(330 * dpiScale), (int)(240 * dpiScale));
                form.AutoScaleMode = AutoScaleMode.Dpi;
                form.StartPosition = FormStartPosition.CenterScreen;
                form.ShowInTaskbar = false;
                form.MinimizeBox = false;

                // the header lives in the same box, so a long group name wraps instead of overflowing
                string content = string.Join(Environment.NewLine,
                    $"抽取分组：{groupName}",
                    $"抽取数量：{names.Count}",
                    string.Empty,
                    string.Join(", ", names));

                TextBox namesTextBox = new TextBox
                {
                    Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                    BackColor = Color.White,
                    Font = Ui.CreateFont(10F),
                    Location = new Point((int)(14 * dpiScale), (int)(12 * dpiScale)),
                    Multiline = true,
                    ReadOnly = true,
                    ScrollBars = ScrollBars.Vertical,
                    Size = new Size((int)(402 * dpiScale), (int)(332 * dpiScale)),
                    WordWrap = true
                };

                form.Controls.Add(namesTextBox);
                // create the handle first, so the wrap layout runs once at the final width instead of twice
                _ = namesTextBox.Handle;
                namesTextBox.Text = content;

                Button closeButton = new Button
                {
                    Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                    DialogResult = DialogResult.OK,
                    Location = new Point((int)(328 * dpiScale), (int)(356 * dpiScale)),
                    Size = new Size((int)(88 * dpiScale), (int)(30 * dpiScale)),
                    Text = "关闭"
                };

                form.Controls.Add(closeButton);
                form.AcceptButton = closeButton;
                form.CancelButton = closeButton;
                form.Shown += (sender, e) =>
                {
                    // a tray app has no foreground window, push the dialog in front once like a message box
                    form.TopMost = true;
                    form.TopMost = false;
                    // the box takes focus for wheel scrolling, but a read only box would start fully selected
                    namesTextBox.Select(0, 0);
                };
                form.ShowDialog();
            }
            current = null;
        }
    }
}
