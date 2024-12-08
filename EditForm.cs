using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Drawer
{
    internal class EditForm : Form
    {
        private readonly StatusStrip statusStrip;
        private readonly ToolStripStatusLabel totalStripStatusLabel;
        private readonly ToolStripStatusLabel selectStripStatusLabel;
        private readonly ToolStrip toolStrip;
        private readonly ToolStripButton importStripButton;
        private readonly ToolStripButton exportStripButton;
        private readonly ToolStripTextBox searchStripTextBox;
        private readonly ToolStripButton searchStripButton;
        private readonly ToolStripButton clearStripButton;
        private readonly ToolStripButton keyStripButton;
        private readonly ToolStripButton helpStripButton;
        private readonly ToolStripButton cancelStripButton;
        private readonly ToolStripButton applyStripButton;
        private readonly ListBox listBox;
        private readonly TextBox textBox;
        private readonly TextBox addTextBox;
        private readonly Button addButton;
        private readonly Button removeButton;

        private readonly EncryptString es;
        private readonly KeyValueStore store;
        private List<string> list;
        private int foundIndex = 0;
        public EditForm()
        {
            es = new EncryptString();
            store = new KeyValueStore();
            list = new List<string>();
            statusStrip = new StatusStrip();
            totalStripStatusLabel = new ToolStripStatusLabel();
            selectStripStatusLabel = new ToolStripStatusLabel();
            toolStrip = new ToolStrip();
            importStripButton = new ToolStripButton();
            exportStripButton = new ToolStripButton();
            searchStripTextBox = new ToolStripTextBox();
            searchStripButton = new ToolStripButton();
            clearStripButton = new ToolStripButton();
            keyStripButton = new ToolStripButton();
            helpStripButton = new ToolStripButton();
            cancelStripButton = new ToolStripButton();
            applyStripButton = new ToolStripButton();
            listBox = new ListBox();
            textBox = new TextBox();
            addTextBox = new TextBox();
            addButton = new Button();
            removeButton = new Button();
            SuspendLayout();

            statusStrip.Items.AddRange(new ToolStripItem[] {
                totalStripStatusLabel,
                selectStripStatusLabel });

            totalStripStatusLabel.BorderSides = ToolStripStatusLabelBorderSides.Left
            | ToolStripStatusLabelBorderSides.Top
            | ToolStripStatusLabelBorderSides.Right
            | ToolStripStatusLabelBorderSides.Bottom;
            totalStripStatusLabel.Text = string.Format(Properties.Resources.bar_total, 0);

            selectStripStatusLabel.BorderSides = ToolStripStatusLabelBorderSides.Left
            | ToolStripStatusLabelBorderSides.Top
            | ToolStripStatusLabelBorderSides.Right
            | ToolStripStatusLabelBorderSides.Bottom;
            selectStripStatusLabel.Text = string.Format(Properties.Resources.bar_selected, 0);

            importStripButton.Image = Properties.Resources.import;
            importStripButton.Text = Properties.Resources.button_import;
            importStripButton.TextImageRelation = TextImageRelation.ImageAboveText;
            importStripButton.Click += ImportStripButton_Click;

            exportStripButton.Image = Properties.Resources.export;
            exportStripButton.Text = Properties.Resources.button_export;
            exportStripButton.TextImageRelation = TextImageRelation.ImageAboveText;
            exportStripButton.Click += ExportStripButton_Click;

            searchStripTextBox.BorderStyle = BorderStyle.FixedSingle;
            searchStripTextBox.Font = new Font(Properties.Resources.font_name, 12F, FontStyle.Regular, GraphicsUnit.Point, 134);

            searchStripButton.Image = Properties.Resources.find;
            searchStripButton.Text = Properties.Resources.button_find;
            searchStripButton.TextImageRelation = TextImageRelation.ImageAboveText;
            searchStripButton.Click += SearchStripButton_Click;

            clearStripButton.Image = Properties.Resources.clear;
            clearStripButton.Text = Properties.Resources.button_clear;
            clearStripButton.TextImageRelation = TextImageRelation.ImageAboveText;
            clearStripButton.Click += ClearStripButton_Click;

            keyStripButton.Image = Properties.Resources.key;
            keyStripButton.Text = Properties.Resources.button_password;
            keyStripButton.TextImageRelation = TextImageRelation.ImageAboveText;
            keyStripButton.Click += KeyStripButton_Click;

            helpStripButton.Image = Properties.Resources.help;
            helpStripButton.Text = Properties.Resources.button_help;
            helpStripButton.TextImageRelation = TextImageRelation.ImageAboveText;
            helpStripButton.Click += HelpStripButton_Click;

            cancelStripButton.Image = Properties.Resources.cancel;
            cancelStripButton.Text = Properties.Resources.button_cancel;
            cancelStripButton.TextImageRelation = TextImageRelation.ImageAboveText;
            cancelStripButton.Click += CancelStripButton_Click;

            applyStripButton.Image = Properties.Resources.apply;
            applyStripButton.Text = Properties.Resources.button_apply;
            applyStripButton.TextImageRelation = TextImageRelation.ImageAboveText;
            applyStripButton.Click += ApplyStripButton_Click;

            toolStrip.ImageScalingSize = new Size(20, 20);
            toolStrip.Items.AddRange(new ToolStripItem[] {
            importStripButton,
            exportStripButton,
            new ToolStripSeparator(),
            searchStripTextBox,
            searchStripButton,
            new ToolStripSeparator(),
            clearStripButton,
            keyStripButton,
            helpStripButton,
            new ToolStripSeparator(),
            cancelStripButton,
            applyStripButton});

            listBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            listBox.BorderStyle = BorderStyle.FixedSingle;
            listBox.Font = new Font(Properties.Resources.font_name, 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            listBox.ItemHeight = 27;
            listBox.Location = new Point(12, 50);
            listBox.ScrollAlwaysVisible = true;
            listBox.SelectionMode = SelectionMode.MultiExtended;
            listBox.Size = new Size(236, 353);
            listBox.SelectedIndexChanged += ListBox_SelectedIndexChanged;

            textBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.Font = new Font(Properties.Resources.font_name, 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            textBox.Location = new Point(254, 50);
            textBox.Multiline = true;
            textBox.ScrollBars = ScrollBars.Vertical;
            textBox.Size = new Size(516, 314);
            textBox.TextChanged += TextBox_TextChanged;
            textBox.KeyPress += TextBox_KeyPress;

            addTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            addTextBox.BorderStyle = BorderStyle.FixedSingle;
            addTextBox.Font = new Font(Properties.Resources.font_name, 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            addTextBox.Location = new Point(254, 372);
            addTextBox.Size = new Size(436, 34);
            addTextBox.KeyDown += AddTextBox_KeyDown;

            addButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            addButton.BackgroundImage = Properties.Resources.add;
            addButton.BackgroundImageLayout = ImageLayout.Zoom;
            addButton.Location = new Point(696, 370);
            addButton.Size = new Size(34, 34);
            addButton.UseVisualStyleBackColor = true;
            addButton.Click += AddButton_Click;

            removeButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            removeButton.BackgroundImage = Properties.Resources.remove;
            removeButton.BackgroundImageLayout = ImageLayout.Zoom;
            removeButton.Location = new Point(736, 370);
            removeButton.Size = new Size(34, 34);
            removeButton.UseVisualStyleBackColor = true;
            removeButton.Click += RemoveButton_Click;

            Text = Properties.Resources.app_whole_name;
            ClientSize = new Size(782, 433);
            Controls.Add(statusStrip);
            Controls.Add(toolStrip);
            Controls.Add(removeButton);
            Controls.Add(addButton);
            Controls.Add(textBox);
            Controls.Add(listBox);
            Controls.Add(addTextBox);
            Icon = Properties.Resources.tray_run;
            MinimumSize = new Size(600, 400);
            StartPosition = FormStartPosition.CenterScreen;
            FormClosing += MainForm_FormClosing;
            ResumeLayout(false);

            textBox.Text = string.Join(",", StringPool.initPool);
        }

        private void ListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectStripStatusLabel.Text = string.Format(Properties.Resources.bar_selected, listBox.SelectedIndices.Count);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult result = MessageBox.Show(Properties.Resources.dialog_discard_content, Properties.Resources.menu_exit, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        private void AddItem()
        {
            if (!string.IsNullOrWhiteSpace(addTextBox.Text))
            {
                if (!addTextBox.Text.Contains(","))
                {
                    list.Add(addTextBox.Text);
                    textBox.Text = string.Join(",", list);
                    addTextBox.Text = "";
                    textBox.SelectionStart = textBox.Text.Length;
                    textBox.ScrollToCaret();
                    if (textBox.Text.StartsWith(","))
                    {
                        textBox.Text = textBox.Text.Substring(1);
                    }
                }
                else
                {
                    _ = MessageBox.Show(Properties.Resources.dialog_no_english_comma, Properties.Resources.button_add, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                addTextBox.Text = "";
            }
        }

        private void AddTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                AddItem();
                e.SuppressKeyPress = true;
            }
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            ListBox.SelectedIndexCollection selectedIndices = listBox.SelectedIndices;
            for (int i = selectedIndices.Count - 1; i >= 0; i--)
            {
                int selectedIndex = selectedIndices[i];
                if (selectedIndex < list.Count)
                {
                    list.RemoveAt(selectedIndex);
                }
            }
            textBox.Text = string.Join(",", list);
            textBox.SelectionStart = textBox.Text.Length;
            textBox.ScrollToCaret();
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            AddItem();
        }

        // ban whitespace when input text
        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsWhiteSpace(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            list = new List<string>(textBox.Text.Split(','));
            listBox.DataSource = null;
            listBox.DataSource = list;
            listBox.ClearSelected();
            listBox.SelectedIndex = listBox.Items.Count - 1;
            textBox.ScrollToCaret();
            totalStripStatusLabel.Text = string.Format(Properties.Resources.bar_total, listBox.Items.Count);

            // ban whitespace when paste
            if (textBox != null)
            {
                string originalText = textBox.Text;
                string newText = originalText
                    .Replace(" ", "")
                    .Replace("\t", "")
                    .Replace("\n", "")
                    .Replace("\r", "");

                // update cursor location
                if (originalText != newText)
                {
                    int cursorPosition = textBox.SelectionStart;
                    textBox.Text = newText;
                    textBox.SelectionStart = Math.Min(cursorPosition, textBox.Text.Length);
                }
            }
        }

        private void ImportStripButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = string.Format(Properties.Resources.dialog_File_filter, "|*.txt"),
                Multiselect = false
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (StreamReader reader = new StreamReader(openFileDialog.FileName))
                {
                    string text = reader.ReadLine();
                    if (text != null)
                    {
                        textBox.Text = text;
                    }
                    else
                    {
                        _ = MessageBox.Show(Properties.Resources.dialog_document_is_empty, Properties.Resources.button_import, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private void ExportStripButton_Click(object sender, EventArgs e)
        {
            if (textBox.Text.Length > 0)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    FileName = "*.txt",
                    Filter = string.Format(Properties.Resources.dialog_File_filter, "|*.txt")
                };
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(saveFileDialog.FileName, textBox.Text);
                }
            }
            else
            {
                _ = MessageBox.Show(Properties.Resources.dialog_content_is_empty, Properties.Resources.button_export, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SearchStripButton_Click(object sender, EventArgs e)
        {
            string searchText = searchStripTextBox.Text;

            if (!string.IsNullOrEmpty(searchText) && !string.IsNullOrEmpty(textBox.Text))
            {
                int index = textBox.Text.IndexOf(searchText, foundIndex, StringComparison.OrdinalIgnoreCase);

                if (!textBox.Focused)
                {
                    _ = textBox.Focus();
                }

                if (index != -1)
                {
                    textBox.Select(index, searchText.Length);
                    textBox.ScrollToCaret();
                    foundIndex = index + searchText.Length;
                }
                else
                {
                    foundIndex = 0;
                    _ = MessageBox.Show(Properties.Resources.dialog_find_complete, Properties.Resources.button_find, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void ClearStripButton_Click(object sender, EventArgs e)
        {
            if (textBox.Text.Length > 0)
            {
                if (MessageBox.Show(Properties.Resources.dialog_is_clear_content, Properties.Resources.button_clear, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    textBox.Clear();
                    searchStripTextBox.Clear();
                    listBox.ClearSelected();
                    if (list.Count == 1)
                    {
                        list.RemoveAt(0);
                    }
                }
            }
        }

        private void KeyStripButton_Click(object sender, EventArgs e)
        {
            string key = InputDialog.Show(Properties.Resources.title_change_password, Properties.Resources.dialog_enter_password, false);
            if (key != null)
            {
                if (!string.IsNullOrEmpty(key))
                {
                    store.Update("Key", es.Encrypt(key));
                    _ = MessageBox.Show(string.Format(Properties.Resources.dialog_password_changed, "\n", key), Properties.Resources.title_change_password, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    _ = MessageBox.Show(Properties.Resources.dialog_password_empty_warn, Properties.Resources.title_change_password, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void HelpStripButton_Click(object sender, EventArgs e)
        {
            _ = MessageBox.Show(string.Format(Properties.Resources.dialog_editor_help, "\n\n", "\n\n"), Properties.Resources.button_help, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void CancelStripButton_Click(object sender, EventArgs e)
        {
            if (textBox.Text.Length > 0)
            {
                DialogResult result = MessageBox.Show(Properties.Resources.dialog_discard_content, Properties.Resources.menu_exit, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    Dispose();
                }
            }
            else
            {
                Dispose();
            }
        }

        private void ApplyStripButton_Click(object sender, EventArgs e)
        {
            if (textBox.Text.Length > 0)
            {
                store.Update("initPool", es.Encrypt(textBox.Text));
                store.Update("pool", es.Encrypt(textBox.Text));
                StringPool.initPool = new List<string>(list);
                StringPool.pool = new List<string>(list);
                Dispose();

                _ = MessageBox.Show(string.Format(Properties.Resources.dialog_statistics, "\n\n", StringPool.initPool.Count(), "\n", string.Join(", ", StringPool.initPool)), Properties.Resources.menu_statistics, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                _ = MessageBox.Show(Properties.Resources.dialog_content_is_empty, Properties.Resources.button_apply, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}