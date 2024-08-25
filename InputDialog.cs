using System.Windows.Forms;

namespace Drawer
{
    internal class InputDialog
    {
        public static string Show(string title, string prompt, int width)
        {
            Form inputForm = new Form
            {
                Text = title,
                Width = width,
                Height = 65,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterScreen,
                MinimizeBox = false,
                MaximizeBox = false
            };

            ToolStrip toolStrip = new ToolStrip();

            ToolStripLabel promptLabel = new ToolStripLabel(prompt);
            _ = toolStrip.Items.Add(promptLabel);

            ToolStripTextBox inputTextBox = new ToolStripTextBox { BorderStyle = BorderStyle.FixedSingle };
            _ = toolStrip.Items.Add(inputTextBox);
            _ = toolStrip.Items.Add(new ToolStripSeparator());

            ToolStripButton okButton = new ToolStripButton("确认");
            okButton.Click += (sender, e) => { inputForm.DialogResult = DialogResult.OK; inputForm.Close(); };
            _ = toolStrip.Items.Add(okButton);

            ToolStripButton cancelButton = new ToolStripButton("取消");
            cancelButton.Click += (sender, e) => { inputForm.DialogResult = DialogResult.Cancel; inputForm.Close(); };
            _ = toolStrip.Items.Add(cancelButton);

            inputForm.Controls.Add(toolStrip);

            return inputForm.ShowDialog() == DialogResult.OK ? inputTextBox.Text : null;
        }
    }
}