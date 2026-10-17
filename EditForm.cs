using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
        private readonly ToolStrip groupToolStrip;
        private readonly ToolStripComboBox groupComboBox;
        private readonly ToolStripButton groupAddButton;
        private readonly ToolStripButton groupRenameButton;
        private readonly ToolStripButton groupDeleteButton;

        private readonly KeyValueStore store;
        private readonly List<string> groupIds;
        private string currentGroupId;
        private string loadedContent = string.Empty;
        private string pendingContent = string.Empty;
        private List<string> list;
        private int foundIndex = 0;

        public EditForm()
        {
            float dpiScale = Ui.DpiScale(this);
            store = new KeyValueStore();
            groupIds = new List<string>();
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
            groupToolStrip = new ToolStrip();
            groupComboBox = new ToolStripComboBox();
            groupAddButton = new ToolStripButton();
            groupRenameButton = new ToolStripButton();
            groupDeleteButton = new ToolStripButton();
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

            searchStripTextBox.Width = (int)(120 * dpiScale);
            searchStripTextBox.BorderStyle = BorderStyle.FixedSingle;
            searchStripTextBox.Font = Ui.CreateFont(12F);

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

            groupComboBox.AutoSize = false;
            groupComboBox.Width = (int)(200 * dpiScale);
            groupComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            groupComboBox.Font = Ui.CreateFont(12F);
            groupComboBox.ComboBox.SelectionChangeCommitted += GroupComboBox_SelectionChangeCommitted;

            groupAddButton.Image = Properties.Resources.add;
            groupAddButton.Text = "添加";
            groupAddButton.ToolTipText = "添加分组";
            groupAddButton.Click += GroupAddButton_Click;

            groupRenameButton.Image = Properties.Resources.edit;
            groupRenameButton.Text = "重命名";
            groupRenameButton.ToolTipText = "重命名分组";
            groupRenameButton.Click += GroupRenameButton_Click;

            groupDeleteButton.Image = Properties.Resources.remove;
            groupDeleteButton.Text = "删除";
            groupDeleteButton.ToolTipText = "删除分组";
            groupDeleteButton.Click += GroupDeleteButton_Click;

            ToolStripLabel groupStripLabel = new ToolStripLabel("分组")
            {
                Image = Properties.Resources.group,
                TextImageRelation = TextImageRelation.ImageAboveText
            };

            groupToolStrip.ImageScalingSize = new Size((int)(20 * dpiScale), (int)(20 * dpiScale));
            groupToolStrip.Items.AddRange(new ToolStripItem[] {
            groupStripLabel,
            groupComboBox,
            groupAddButton,
            groupRenameButton,
            groupDeleteButton});

            listBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            listBox.BorderStyle = BorderStyle.FixedSingle;
            listBox.Font = Ui.CreateFont(12F);
            listBox.ItemHeight = 27;
            listBox.Location = new Point((int)(12 * dpiScale), (int)(91 * dpiScale));
            listBox.ScrollAlwaysVisible = true;
            listBox.SelectionMode = SelectionMode.MultiExtended;
            listBox.Size = new Size((int)(236 * dpiScale), (int)(353 * dpiScale));
            listBox.SelectedIndexChanged += ListBox_SelectedIndexChanged;
            listBox.DoubleClick += ListBox_DoubleClick;

            textBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.Font = Ui.CreateFont(12F);
            textBox.Location = new Point((int)(254 * dpiScale), (int)(91 * dpiScale));
            textBox.Multiline = true;
            textBox.ScrollBars = ScrollBars.Vertical;
            textBox.Size = new Size((int)(516 * dpiScale), (int)(314 * dpiScale));
            textBox.TextChanged += TextBox_TextChanged;
            textBox.KeyPress += TextBox_KeyPress;

            addTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            addTextBox.BorderStyle = BorderStyle.FixedSingle;
            addTextBox.Font = Ui.CreateFont(12F);
            addTextBox.Location = new Point((int)(254 * dpiScale), (int)(413 * dpiScale));
            addTextBox.Size = new Size((int)(436 * dpiScale), (int)(34 * dpiScale));
            addTextBox.KeyDown += AddTextBox_KeyDown;

            addButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            addButton.BackgroundImage = Properties.Resources.add;
            addButton.BackgroundImageLayout = ImageLayout.Zoom;
            addButton.Location = new Point((int)(696 * dpiScale), (int)(411 * dpiScale));
            addButton.Size = new Size((int)(34 * dpiScale), (int)(34 * dpiScale));
            addButton.UseVisualStyleBackColor = true;
            addButton.Click += AddButton_Click;

            removeButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            removeButton.BackgroundImage = Properties.Resources.remove;
            removeButton.BackgroundImageLayout = ImageLayout.Zoom;
            removeButton.Location = new Point((int)(736 * dpiScale), (int)(411 * dpiScale));
            removeButton.Size = new Size((int)(34 * dpiScale), (int)(34 * dpiScale));
            removeButton.UseVisualStyleBackColor = true;
            removeButton.Click += RemoveButton_Click;

            Text = "YuXiang Drawer";
            ClientSize = new Size((int)(782 * dpiScale), (int)(474 * dpiScale));
            Controls.Add(statusStrip);
            Controls.Add(groupToolStrip);
            Controls.Add(toolStrip);
            Controls.Add(removeButton);
            Controls.Add(addButton);
            Controls.Add(textBox);
            Controls.Add(listBox);
            Controls.Add(addTextBox);
            Icon = Properties.Resources.tray_run;
            MinimumSize = new Size((int)(600 * dpiScale), (int)(441 * dpiScale));
            AutoScaleMode = AutoScaleMode.Dpi;
            StartPosition = FormStartPosition.Manual;
            FormClosing += EditForm_FormClosing;
            ResumeLayout(false);

            Ui.CenterOnPrimary(this);

            LoadGroup(GroupStore.GetActiveGroupId());

            // a big list is filled in after the window is up, otherwise Show() keeps measuring the long text
            Shown += (sender, e) => BeginInvoke(new Action(FillContent));
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

        private void EditForm_FormClosing(object sender, FormClosingEventArgs e)
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

        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r' || e.KeyChar == '\n')
            {
                e.Handled = true;
                return;
            }
            if (char.IsWhiteSpace(e.KeyChar) && TouchesComma())
            {
                e.Handled = true;
            }
        }

        // whether the caret sits right before or right after a comma
        private bool TouchesComma()
        {
            int start = textBox.SelectionStart;
            int end = start + textBox.SelectionLength;
            string text = textBox.Text;
            return (start > 0 && text[start - 1] == ',') || (end < text.Length && text[end] == ',');
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            list = textBox.Text.Length == 0 ? new List<string>() : new List<string>(textBox.Text.Split(','));
            listBox.DataSource = list;
            listBox.ClearSelected();
            listBox.SelectedIndex = listBox.Items.Count - 1;
            textBox.ScrollToCaret();
            totalStripStatusLabel.Text = $"总数：{listBox.Items.Count}";

            string originalText = textBox.Text;
            string newText = NormalizeCommas(originalText);

            if (originalText != newText)
            {
                textBox.Text = newText;
                textBox.SelectionStart = Math.Min(textBox.SelectionStart, textBox.Text.Length);
            }
        }

        // turns line breaks into commas and strips spaces next to a comma
        internal static string NormalizeCommas(string text)
        {
            string normalized = text.Replace("\r\n", ",").Replace('\n', ',').Replace("\r", "");
            return Regex.Replace(normalized, @"\s*,\s*", ",");
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
                    // the whole file is imported, line breaks are turned into commas later
                    string text = reader.ReadToEnd().Trim();
                    if (text.Length > 0)
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
                if (foundIndex > textBox.Text.Length)
                {
                    foundIndex = 0;
                }

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
                }
            }
        }

        private void KeyStripButton_Click(object sender, EventArgs e)
        {
            EncryptString es = new EncryptString();

            string currectKey = es.Decrypt(store.Get("Key"));
            string key = PromptDialog.Input("更改密码", "原密码：", true, null, 20);
            if (key != null)
            {
                if (key == currectKey)
                {
                    string changeKey = PromptDialog.Input("更改密码", "新密码：", false, null, 20);
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

        private void LoadGroup(string groupId)
        {
            if (string.IsNullOrEmpty(groupId))
            {
                groupId = GroupStore.GetActiveGroupId();
            }
            if (string.IsNullOrEmpty(groupId))
            {
                groupId = GroupStore.AddGroup(GroupStore.DefaultGroupName);
            }
            currentGroupId = groupId;
            pendingContent = string.Join(",", GroupStore.GetInitReference(groupId));
            if (Visible)
            {
                FillContent();
            }
            Text = $"YuXiang Drawer - {GroupStore.GetGroupName(groupId)}";
            RefreshGroupComboBox();
        }

        private void FillContent()
        {
            if (IsDisposed || Disposing)
            {
                return;
            }
            if (textBox.Text != pendingContent)
            {
                textBox.Text = pendingContent;
            }
            loadedContent = textBox.Text;
        }

        private void RefreshGroupComboBox()
        {
            // the edited group can disappear when it is removed, fall back to the active one
            if (!GroupStore.GetGroupIds().Contains(currentGroupId))
            {
                LoadGroup(GroupStore.GetActiveGroupId());
                return;
            }

            groupIds.Clear();
            groupIds.AddRange(GroupStore.GetGroupIds());
            groupComboBox.Items.Clear();
            foreach (string id in groupIds)
            {
                groupComboBox.Items.Add(GroupStore.GetGroupName(id));
            }
            int index = groupIds.IndexOf(currentGroupId);
            if (index >= 0)
            {
                groupComboBox.SelectedIndex = index;
            }
        }

        private bool IsListModified()
        {
            return textBox.Text != loadedContent;
        }

        private void WithDiscardConfirmation(Action action)
        {
            if (!IsListModified())
            {
                action();
                return;
            }
            DialogResult result = MessageBox.Show($"是否丢弃对“{GroupStore.GetGroupName(currentGroupId)}”的修改？", "分组", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                action();
            }
            RefreshGroupComboBox();
        }

        private void GroupComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            int index = groupComboBox.SelectedIndex;
            if (index < 0 || index >= groupIds.Count)
            {
                return;
            }
            string selected = groupIds[index];
            if (selected == currentGroupId)
            {
                return;
            }
            WithDiscardConfirmation(() => LoadGroup(selected));
        }

        private bool ValidateGroupName(string name, string excludeId)
        {
            if (name.Length == 0)
            {
                MessageBox.Show("组名不能为空。", "分组", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (name.Length > 30)
            {
                MessageBox.Show("组名不能超过30个字符。", "分组", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            string existing = GroupStore.FindGroupIdByName(name);
            if (existing != null && existing != excludeId)
            {
                MessageBox.Show($"分组“{name}”已存在。", "分组", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void GroupAddButton_Click(object sender, EventArgs e)
        {
            string name = PromptDialog.Input("添加分组", "组名：", false, null, 30);
            if (name == null)
            {
                return;
            }
            name = name.Trim();
            if (!ValidateGroupName(name, null))
            {
                return;
            }

            if (IsListModified())
            {
                string currentName = GroupStore.GetGroupName(currentGroupId);
                DialogResult result = MessageBox.Show(
                    $"是否保存对“{currentName}”的修改并切换到新分组？",
                    "添加分组",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);
                if (result == DialogResult.Cancel)
                {
                    return;
                }
                if (result == DialogResult.Yes)
                {
                    list.RemoveAll(s => string.IsNullOrEmpty(s));
                    GroupStore.SetInit(currentGroupId, list);
                }
            }

            LoadGroup(GroupStore.AddGroup(name));
        }

        private void GroupRenameButton_Click(object sender, EventArgs e)
        {
            string currentName = GroupStore.GetGroupName(currentGroupId);
            string name = PromptDialog.Input("重命名分组", "新组名：", false, currentName, 30);
            if (name == null)
            {
                return;
            }
            name = name.Trim();
            if (name == currentName)
            {
                return;
            }
            if (!ValidateGroupName(name, currentGroupId))
            {
                return;
            }
            GroupStore.RenameGroup(currentGroupId, name);
            Text = $"YuXiang Drawer - {name}";
            RefreshGroupComboBox();
        }

        private void GroupDeleteButton_Click(object sender, EventArgs e)
        {
            WithDiscardConfirmation(() =>
            {
                if (GroupStore.GetGroupCount() <= 1)
                {
                    MessageBox.Show("至少需要保留一个分组。", "删除分组", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                string name = GroupStore.GetGroupName(currentGroupId);
                if (MessageBox.Show($"是否删除分组“{name}”？\n该分组内的名单将一并删除。", "删除分组", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    GroupStore.DeleteGroup(currentGroupId);
                    LoadGroup(GroupStore.GetActiveGroupId());
                }
            });
        }

        private void ApplyStripButton_Click(object sender, EventArgs e)
        {
            if (textBox.Text.Length > 0)
            {
                // spaces at the very start or end of an entry are dropped as well
                list = list.Select(s => s.Trim()).Where(s => s.Length > 0).ToList();
                GroupStore.SetInit(currentGroupId, list);
                GroupStore.SetActiveGroup(currentGroupId);
                StatsForm.Show(GroupStore.GetGroupName(currentGroupId), list);
                Dispose();
            }
            else
            {
                MessageBox.Show("内容不能为空。", "应用", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
