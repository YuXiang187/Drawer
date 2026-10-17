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

            using (DpiAwareForm form = new DpiAwareForm())
            {
                current = form;

                form.Text = "统计";
                form.Icon = Properties.Resources.tray_run;
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
                    Multiline = true,
                    ReadOnly = true,
                    ScrollBars = ScrollBars.Vertical,
                    WordWrap = true
                };

                Button closeButton = new Button
                {
                    Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                    DialogResult = DialogResult.OK,
                    Text = "关闭"
                };

                form.Controls.Add(namesTextBox);
                form.Controls.Add(closeButton);

                form.LayoutRequested += (sender, e) =>
                {
                    form.MinimumSize = Size.Empty;
                    form.ClientSize = Ui.Px(form, 430, 400);
                    form.MinimumSize = Ui.Px(form, 330, 240);
                    namesTextBox.Font = Ui.CreateFont(form, 10F);
                    closeButton.Font = Ui.CreateFont(form, 12F);
                    namesTextBox.Location = new Point(Ui.Px(form, 14), Ui.Px(form, 12));
                    namesTextBox.Size = Ui.Px(form, 402, 332);
                    closeButton.Location = new Point(Ui.Px(form, 328), Ui.Px(form, 356));
                    closeButton.Size = Ui.Px(form, 88, 30);
                };

                // create the handle first, so the wrap layout runs once at the final width instead of twice
                _ = namesTextBox.Handle;
                namesTextBox.Text = content;

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
