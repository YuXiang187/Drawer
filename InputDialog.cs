using System.Drawing;
using System.Windows.Forms;

namespace Drawer
{
    internal class InputDialog
    {
        private static Form inputForm;
        private static readonly string fontName = "微软雅黑";
        public static string Show(string title, string prompt, bool isPassword)
        {
            inputForm = new Form
            {
                Text = title,
                Width = 268,
                Height = 72,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterScreen,
                ShowInTaskbar = false,
                MinimizeBox = false,
                MaximizeBox = false
            };

            ToolStrip toolStrip = new ToolStrip
            {
                ImageScalingSize = new Size(20, 20)
            };

            ToolStripLabel promptLabel = new ToolStripLabel(prompt) { Font = new Font(fontName, 12F, FontStyle.Regular, GraphicsUnit.Point, 134) };

            ToolStripTextBox inputTextBox = new ToolStripTextBox
            {
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font(fontName, 12F, FontStyle.Regular, GraphicsUnit.Point, 134)
            };
            inputTextBox.TextBox.UseSystemPasswordChar = isPassword;
            inputTextBox.KeyDown += InputTextBox_KeyDown;

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

            _ = toolStrip.Items.Add(promptLabel);
            _ = toolStrip.Items.Add(inputTextBox);
            _ = toolStrip.Items.Add(new ToolStripSeparator());
            _ = toolStrip.Items.Add(okButton);
            _ = toolStrip.Items.Add(cancelButton);

            inputForm.Controls.Add(toolStrip);

            return inputForm.ShowDialog() == DialogResult.OK ? inputTextBox.Text : null;
        }

        private static void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                inputForm.DialogResult = DialogResult.OK; inputForm.Close();
                e.SuppressKeyPress = true;
            }
        }
    }
}