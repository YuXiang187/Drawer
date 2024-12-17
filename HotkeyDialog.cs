using System.Drawing;
using System.Windows.Forms;

namespace Drawer
{
    internal class HotkeyDialog
    {
        private static Form hotkeyForm;
        private static readonly string fontName = "微软雅黑";
        private static float dpiScale;
        private static ToolStripTextBox inputTextBox;
        private static Keys hotkey;

        public static Keys GetKeys(string title, string prompt, string boxText)
        {
            hotkeyForm = new Form();
            dpiScale = Graphics.FromHwnd(hotkeyForm.Handle).DpiX / 96f;
            hotkeyForm.Text = title;
            hotkeyForm.Width = (int)(370 * dpiScale);
            hotkeyForm.Height = (int)(72 * dpiScale);
            hotkeyForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            hotkeyForm.AutoScaleMode = AutoScaleMode.Dpi;
            hotkeyForm.StartPosition = FormStartPosition.CenterScreen;
            hotkeyForm.ShowInTaskbar = false;
            hotkeyForm.MinimizeBox = false;
            hotkeyForm.MaximizeBox = false;
            hotkeyForm.KeyDown += InputForm_KeyDown;

            ToolStrip toolStrip = new ToolStrip
            {
                ImageScalingSize = new Size((int)(20 * dpiScale), (int)(20 * dpiScale))
            };

            ToolStripLabel promptLabel = new ToolStripLabel(prompt)
            {
                Font = new Font(fontName, 12F, FontStyle.Regular, GraphicsUnit.Point, 134)
            };

            inputTextBox = new ToolStripTextBox
            {
                Width = (int)(180 * dpiScale),
                Text = boxText,
                Enabled = false,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font(fontName, 12F, FontStyle.Regular, GraphicsUnit.Point, 134)
            };

            ToolStripButton okButton = new ToolStripButton
            {
                Image = Properties.Resources.apply
            };
            okButton.Click += (sender, e) => { hotkeyForm.DialogResult = DialogResult.OK; hotkeyForm.Close(); };

            ToolStripButton cancelButton = new ToolStripButton
            {
                Image = Properties.Resources.cancel
            };
            cancelButton.Click += (sender, e) => { hotkeyForm.DialogResult = DialogResult.Cancel; hotkeyForm.Close(); };

            toolStrip.Items.Add(promptLabel);
            toolStrip.Items.Add(inputTextBox);
            toolStrip.Items.Add(new ToolStripSeparator());
            toolStrip.Items.Add(okButton);
            toolStrip.Items.Add(cancelButton);

            hotkeyForm.Controls.Add(toolStrip);

            return hotkeyForm.ShowDialog() == DialogResult.OK ? hotkey : Keys.None;
        }

        private static void InputForm_KeyDown(object sender, KeyEventArgs e)
        {
            Keys modifiers = Keys.None;

            if (e.Modifiers.HasFlag(Keys.Control))
            {
                modifiers |= Keys.Control;
            }

            if (e.Modifiers.HasFlag(Keys.Shift))
            {
                modifiers |= Keys.Shift;
            }

            if (e.Modifiers.HasFlag(Keys.Alt))
            {
                modifiers |= Keys.Alt;
            }

            hotkey = modifiers | e.KeyCode;

            inputTextBox.Text = hotkey.ToString();
            e.Handled = true;
        }
    }
}