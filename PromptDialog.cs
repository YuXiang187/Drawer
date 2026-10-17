using System.Windows.Forms;

namespace Drawer
{
    internal class PromptDialog
    {
        private static Form inputForm;
        private readonly DpiAwareForm form;
        private readonly ToolStripTextBox inputTextBox;
        private Keys hotkey;
        private string text;

        private PromptDialog(string title, string prompt, int width, int boxWidth)
        {
            form = new DpiAwareForm();
            inputForm = form;

            form.Text = title;
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.ShowInTaskbar = false;
            form.MinimizeBox = false;
            form.MaximizeBox = false;

            ToolStrip toolStrip = new ToolStrip();

            ToolStripLabel promptLabel = new ToolStripLabel(prompt)
            {
                Font = Ui.CreateFont(12F)
            };

            inputTextBox = new ToolStripTextBox
            {
                BorderStyle = BorderStyle.FixedSingle,
                Font = Ui.CreateFont(12F)
            };

            ToolStripButton okButton = new ToolStripButton
            {
                Image = Properties.Resources.apply
            };
            okButton.Click += (sender, e) => { form.DialogResult = DialogResult.OK; form.Close(); };

            ToolStripButton cancelButton = new ToolStripButton
            {
                Image = Properties.Resources.cancel
            };
            cancelButton.Click += (sender, e) => { form.DialogResult = DialogResult.Cancel; form.Close(); };

            toolStrip.Items.Add(promptLabel);
            toolStrip.Items.Add(inputTextBox);
            toolStrip.Items.Add(new ToolStripSeparator());
            toolStrip.Items.Add(okButton);
            toolStrip.Items.Add(cancelButton);

            form.Controls.Add(toolStrip);

            form.LayoutRequested += (sender, e) =>
            {
                form.Width = Ui.Px(form, width);
                form.Height = Ui.Px(form, 72);
                toolStrip.ImageScalingSize = Ui.Px(form, 20, 20);
                inputTextBox.Width = Ui.Px(form, boxWidth);
            };
        }

        public static string Input(string title, string prompt, bool isPassword, string initialText = null, int maxLength = 0)
        {
            if (IsOpen())
            {
                return null;
            }

            PromptDialog dialog = new PromptDialog(title, prompt, 278, 120);
            dialog.inputTextBox.TextBox.UseSystemPasswordChar = isPassword;
            if (maxLength > 0)
            {
                dialog.inputTextBox.MaxLength = maxLength;
            }
            if (initialText != null)
            {
                dialog.inputTextBox.Text = initialText;
                dialog.inputTextBox.SelectionStart = initialText.Length;
            }
            dialog.inputTextBox.KeyDown += dialog.InputTextBox_KeyDown;
            dialog.form.Shown += (sender, e) =>
            {
                dialog.inputTextBox.Focus();
                dialog.inputTextBox.TextBox.SelectionStart = dialog.inputTextBox.Text.Length;
            };

            return dialog.Run() ? dialog.text : null;
        }

        public static Keys Hotkey(string title, string prompt, string boxText)
        {
            if (IsOpen())
            {
                return Keys.None;
            }

            PromptDialog dialog = new PromptDialog(title, prompt, 370, 180);
            dialog.inputTextBox.Text = boxText;
            dialog.inputTextBox.Enabled = false;
            dialog.form.KeyDown += dialog.Hotkey_KeyDown;

            return dialog.Run() ? dialog.hotkey : Keys.None;
        }

        private static bool IsOpen()
        {
            if (inputForm == null || inputForm.IsDisposed)
            {
                return false;
            }
            inputForm.Activate();
            return true;
        }

        private bool Run()
        {
            DialogResult result;
            try
            {
                result = form.ShowDialog();
                text = inputTextBox.Text;
            }
            finally
            {
                form.Dispose();
                inputForm = null;
            }
            return result == DialogResult.OK;
        }

        private void Hotkey_KeyDown(object sender, KeyEventArgs e)
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

        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                form.DialogResult = DialogResult.OK;
                form.Close();
                e.SuppressKeyPress = true;
            }
        }
    }
}
