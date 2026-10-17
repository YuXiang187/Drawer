using System.Drawing;
using System.Windows.Forms;

namespace Drawer
{
    internal class InputDialog
    {
        private static Form inputForm;
        private static readonly string fontName = "微软雅黑";
        private static float dpiScale;

        public static string Show(string title, string prompt, bool isPassword, string initialText = null, int maxLength = 0)
        {
            inputForm = new Form();
            dpiScale = Graphics.FromHwnd(inputForm.Handle).DpiX / 96f;
            inputForm.Text = title;
            inputForm.Width = (int)(278 * dpiScale);
            inputForm.Height = (int)(72 * dpiScale);
            inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            inputForm.AutoScaleMode = AutoScaleMode.Dpi;
            inputForm.StartPosition = FormStartPosition.CenterScreen;
            inputForm.ShowInTaskbar = false;
            inputForm.MinimizeBox = false;
            inputForm.MaximizeBox = false;

            ToolStrip toolStrip = new ToolStrip
            {
                ImageScalingSize = new Size((int)(20 * dpiScale), (int)(20 * dpiScale))
            };

            ToolStripLabel promptLabel = new ToolStripLabel(prompt)
            {
                Font = new Font(fontName, 12F, FontStyle.Regular, GraphicsUnit.Point, 134)
            };

            ToolStripTextBox inputTextBox = new ToolStripTextBox
            {
                Width = (int)(120 * dpiScale),
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font(fontName, 12F, FontStyle.Regular, GraphicsUnit.Point, 134)
            };
            inputTextBox.TextBox.UseSystemPasswordChar = isPassword;
            if (maxLength > 0)
            {
                inputTextBox.MaxLength = maxLength;
            }
            if (initialText != null)
            {
                inputTextBox.Text = initialText;
                inputTextBox.SelectionStart = initialText.Length;
            }
            inputTextBox.KeyDown += InputTextBox_KeyDown;

            inputForm.Shown += (sender, e) =>
            {
                inputTextBox.Focus();
                inputTextBox.TextBox.SelectionStart = inputTextBox.Text.Length;
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

            DialogResult dialogResult = inputForm.ShowDialog();
            string inputText = dialogResult == DialogResult.OK ? inputTextBox.Text : null;
            inputForm.Dispose();
            return inputText;
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