using System.Drawing;
using System.Windows.Forms;

namespace Drawer
{
    internal class HotkeyDialog
    {
        private static Form inputForm;
        private static readonly string fontName = "微软雅黑";
        private static ToolStripTextBox inputTextBox;
        private static Keys hotkey;

        public static Keys GetKeys(string title, string prompt, string boxText)
        {
            inputForm = new Form
            {
                Text = title,
                Width = 370,
                Height = 72,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterScreen,
                ShowInTaskbar = false,
                MinimizeBox = false,
                MaximizeBox = false
            };
            inputForm.KeyDown += InputForm_KeyDown;

            ToolStrip toolStrip = new ToolStrip
            {
                ImageScalingSize = new Size(20, 20)
            };

            ToolStripLabel promptLabel = new ToolStripLabel(prompt)
            {
                Font = new Font(fontName, 12F, FontStyle.Regular, GraphicsUnit.Point, 134)
            };

            inputTextBox = new ToolStripTextBox
            {
                Width = 180,
                Text = boxText,
                Enabled = false,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font(fontName, 12F, FontStyle.Regular, GraphicsUnit.Point, 134)
            };

            ToolStripButton okButton = new ToolStripButton
            {
                Image = Properties.Resources.apply
            };
            okButton.Click += (sender, e) => { inputForm.DialogResult = DialogResult.OK; inputForm.Close(); };

            ToolStripButton cancelButton = new ToolStripButton
            {
                Image = Properties.Resources.cancel
            };
            cancelButton.Click += (sender, e) => { inputForm.DialogResult = DialogResult.Cancel; inputForm.Close(); };

            toolStrip.Items.Add(promptLabel);
            toolStrip.Items.Add(inputTextBox);
            toolStrip.Items.Add(new ToolStripSeparator());
            toolStrip.Items.Add(okButton);
            toolStrip.Items.Add(cancelButton);

            inputForm.Controls.Add(toolStrip);

            return inputForm.ShowDialog() == DialogResult.OK ? hotkey : Keys.None;
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