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

        private static readonly string fontName = "微软雅黑";
        private static float dpiScale;
        private readonly EncryptString es;
        private readonly KeyValueStore store;
        private List<string> list;
        private int foundIndex = 0;
        public EditForm()
        {
            dpiScale = Graphics.FromHwnd(Handle).DpiX / 96f;
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
            totalStripStatusLabel.Text = "总数：0";

            selectStripStatusLabel.BorderSides = ToolStripStatusLabelBorderSides.Left
            | ToolStripStatusLabelBorderSides.Top
            | ToolStripStatusLabelBorderSides.Right
            | ToolStripStatusLabelBorderSides.Bottom;
            selectStripStatusLabel.Text = "选中：0";

            importStripButton.Image = Properties.Resources.import;
            importStripButton.Text = "导入";
            importStripButton.TextImageRelation = TextImageRelation.ImageAboveText;
            importStripButton.Click += ImportStripButton_Click;

            exportStripButton.Image = Properties.Resources.export;
            exportStripButton.Text = "导出";
            exportStripButton.TextImageRelation = TextImageRelation.ImageAboveText;
            exportStripButton.Click += ExportStripButton_Click;

            searchStripTextBox.BorderStyle = BorderStyle.FixedSingle;
            searchStripTextBox.Font = new Font(fontName, 12F, FontStyle.Regular, GraphicsUnit.Point, 134);

            searchStripButton.Image = Properties.Resources.find;
            searchStripButton.Text = "查找";
            searchStripButton.TextImageRelation = TextImageRelation.ImageAboveText;
            searchStripButton.Click += SearchStripButton_Click;

            clearStripButton.Image = Properties.Resources.clear;
            clearStripButton.Text = "清空";
            clearStripButton.TextImageRelation = TextImageRelation.ImageAboveText;
            clearStripButton.Click += ClearStripButton_Click;

            keyStripButton.Image = Properties.Resources.key;
            keyStripButton.Text = "密码";
            keyStripButton.TextImageRelation = TextImageRelation.ImageAboveText;
            keyStripButton.Click += KeyStripButton_Click;

            helpStripButton.Image = Properties.Resources.help;
            helpStripButton.Text = "帮助";
            helpStripButton.TextImageRelation = TextImageRelation.ImageAboveText;
            helpStripButton.Click += HelpStripButton_Click;

            cancelStripButton.Image = Properties.Resources.cancel;
            cancelStripButton.Text = "取消";
            cancelStripButton.TextImageRelation = TextImageRelation.ImageAboveText;
            cancelStripButton.Click += CancelStripButton_Click;

            applyStripButton.Image = Properties.Resources.apply;
            applyStripButton.Text = "应用";
            applyStripButton.TextImageRelation = TextImageRelation.ImageAboveText;
            applyStripButton.Click += ApplyStripButton_Click;

            toolStrip.ImageScalingSize = new Size((int)(20 * dpiScale), (int)(20 * dpiScale));
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
            listBox.Font = new Font(fontName, 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            listBox.ItemHeight = 27;
            listBox.Location = new Point((int)(12 * dpiScale), (int)(50 * dpiScale));
            listBox.ScrollAlwaysVisible = true;
            listBox.SelectionMode = SelectionMode.MultiExtended;
            listBox.Size = new Size((int)(236 * dpiScale), (int)(353 * dpiScale));
            listBox.SelectedIndexChanged += ListBox_SelectedIndexChanged;
            listBox.DoubleClick += ListBox_DoubleClick;

            textBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.Font = new Font(fontName, 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            textBox.Location = new Point((int)(254 * dpiScale), (int)(50 * dpiScale));
            textBox.Multiline = true;
            textBox.ScrollBars = ScrollBars.Vertical;
            textBox.Size = new Size((int)(516 * dpiScale), (int)(314 * dpiScale));
            textBox.TextChanged += TextBox_TextChanged;
            textBox.KeyPress += TextBox_KeyPress;

            addTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            addTextBox.BorderStyle = BorderStyle.FixedSingle;
            addTextBox.Font = new Font(fontName, 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            addTextBox.Location = new Point((int)(254 * dpiScale), (int)(372 * dpiScale));
            addTextBox.Size = new Size((int)(436 * dpiScale), (int)(34 * dpiScale));
            addTextBox.KeyDown += AddTextBox_KeyDown;

            addButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            addButton.BackgroundImage = Properties.Resources.add;
            addButton.BackgroundImageLayout = ImageLayout.Zoom;
            addButton.Location = new Point((int)(696 * dpiScale), (int)(370 * dpiScale));
            addButton.Size = new Size((int)(34 * dpiScale), (int)(34 * dpiScale));
            addButton.UseVisualStyleBackColor = true;
            addButton.Click += AddButton_Click;

            removeButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            removeButton.BackgroundImage = Properties.Resources.remove;
            removeButton.BackgroundImageLayout = ImageLayout.Zoom;
            removeButton.Location = new Point((int)(736 * dpiScale), (int)(370 * dpiScale));
            removeButton.Size = new Size((int)(34 * dpiScale), (int)(34 * dpiScale));
            removeButton.UseVisualStyleBackColor = true;
            removeButton.Click += RemoveButton_Click;

            Text = "YuXiang Drawer";
            ClientSize = new Size((int)(782 * dpiScale), (int)(433 * dpiScale));
            Controls.Add(statusStrip);
            Controls.Add(toolStrip);
            Controls.Add(removeButton);
            Controls.Add(addButton);
            Controls.Add(textBox);
            Controls.Add(listBox);
            Controls.Add(addTextBox);
            Icon = Properties.Resources.tray_run;
            MinimumSize = new Size((int)(600 * dpiScale), (int)(400 * dpiScale));
            AutoScaleMode = AutoScaleMode.Dpi;
            StartPosition = FormStartPosition.Manual;
            FormClosing += MainForm_FormClosing;
            ResumeLayout(false);

            // set location in center
            Rectangle screenArea = Screen.AllScreens.FirstOrDefault(s => s.Primary).WorkingArea;
            Location = new Point((screenArea.Width - Width) / 2, (screenArea.Height - Height) / 2);

            textBox.Text = string.Join(",", StringPool.initPool);
        }

        private void ListBox_DoubleClick(object sender, EventArgs e)
        {
            if (listBox.SelectedItems.Count > 0)
            {
                searchStripTextBox.Text = listBox.SelectedItems[0].ToString();
            }
        }

        private void ListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectStripStatusLabel.Text = $"选中：{listBox.SelectedIndices.Count}";
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult result = MessageBox.Show("是否丢弃所有内容？", "退出", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
                    MessageBox.Show("请不要包含英文逗号“,”。", "添加", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            totalStripStatusLabel.Text = $"总数：{listBox.Items.Count}";

            // ban whitespace when paste
            if (textBox != null)
            {
                string originalText = textBox.Text;
                string newText = originalText
                    .Replace(" ", "")
                    .Replace("\t", "")
                    .Replace("\n", ",")
                    .Replace("\r", "");

                if (originalText != newText)
                {
                    textBox.Text = newText;
                    textBox.SelectionStart = Math.Min(textBox.SelectionStart, textBox.Text.Length);
                }
            }
        }

        private void ImportStripButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "文本文档|*.txt",
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
                        MessageBox.Show("文本文件的内容为空。", "导入", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    Filter = "文本文档|*.txt"
                };
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    list.RemoveAll(s => string.IsNullOrEmpty(s));
                    File.WriteAllText(saveFileDialog.FileName, string.Join(",", list));
                }
            }
            else
            {
                MessageBox.Show("内容不能为空。", "导出", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SearchStripButton_Click(object sender, EventArgs e)
        {
            string searchText = searchStripTextBox.Text;

            if (!string.IsNullOrEmpty(searchText))
            {
                int index = textBox.Text.IndexOf(searchText, foundIndex, StringComparison.OrdinalIgnoreCase);

                if (!textBox.Focused)
                {
                    textBox.Focus();
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
                    MessageBox.Show("查找完毕。", "查找", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void ClearStripButton_Click(object sender, EventArgs e)
        {
            if (textBox.Text.Length > 0)
            {
                if (MessageBox.Show("是否清空内容？", "清空", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
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
            EncryptString es = new EncryptString();

            string currectKey = es.Decrypt(store.Get("Key"));
            string key = InputDialog.Show("更改密码", "原密码：", true);
            if (key != null)
            {
                if (key == currectKey)
                {
                    string changeKey = InputDialog.Show("更改密码", "新密码：", false);
                    if (!string.IsNullOrEmpty(changeKey))
                    {
                        store.Update("Key", es.Encrypt(changeKey));
                        MessageBox.Show($"密码已更改为：\n{changeKey}", "更改密码", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("密码不能为空。", "更改密码", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("密码错误。", "更改密码", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void HelpStripButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("编辑列表的语法为：\n\n名称1,名称2,名称3,名称4,名称5,...\n\n注意：分割符为英文逗号，不是中文逗号！", "帮助", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void CancelStripButton_Click(object sender, EventArgs e)
        {
            if (textBox.Text.Length > 0)
            {
                DialogResult result = MessageBox.Show("是否丢弃所有内容？", "退出", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
                list.RemoveAll(s => string.IsNullOrEmpty(s));
                StringPool.initPool = new List<string>(list);
                StringPool.pool = new List<string>(list);
                store.Update("initPool", es.Encrypt(string.Join(",", list)));
                store.Update("pool", es.Encrypt(string.Join(",", list)));
                Dispose();

                MessageBox.Show($"统计结果如下。\n\n抽取数量：{StringPool.initPool.Count()}\n\n抽取名单：{string.Join(", ", StringPool.initPool)}", "统计", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("内容不能为空。", "应用", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}