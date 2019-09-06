using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CsvEditor
{
    public partial class Frm : Form
    {

        private Csv csv;

        private string filePath;

        private Csv displayingCsv;

        private CsvCommand lastSelectCommand;

        private int left, right;


        public Frm()
        {
            InitializeComponent();
        }

        private void Frm_Load(object sender, EventArgs e)
        {


            InitDialogs();
            InitList();

            RegisterCommandEvents();
            
        }

        void InitDialogs()
        {
            openFileDialog.InitialDirectory = Environment.CurrentDirectory;
            saveFileDialog.InitialDirectory = Environment.CurrentDirectory;
            openFileDialog.RestoreDirectory = true;
            saveFileDialog.RestoreDirectory = true;
        }

        void InitList()
        {
            list.LostFocus += List_LostFocus;
        }

        private void List_LostFocus(object sender, EventArgs e)
        {
            list.SelectedIndex = -1;
        }

        void RegisterCommandEvents()
        {
            var runner = CsvCommandRunner.GetInstance();
            runner.OnSuccess += Runner_OnSuccess;
            runner.OnError += Runner_OnError;
        }
        

        private void Runner_OnError(CsvCommand cmd, Exception e)
        {
            MessageBox.Show(e.Message, "Csv Command Error");
        }

        private void Runner_OnSuccess(CsvCommand cmd, CsvCommandQueryResult result)
        {
            switch (cmd.CommandType)
            {
                case CsvCommandType.Select:
                    lastSelectCommand = cmd;
                    break;
            }

            switch (result.Type)
            {
                case CsvCommandQueryResultType.Csv:
                    DisplayCsv(result.Csv);
                    break;

                case CsvCommandQueryResultType.Int:
                    if (lastSelectCommand != null)
                        CsvCommandRunner.GetInstance().Run(csv, lastSelectCommand);
                    UpdateText();
                    break;
            }
        }

        private void DisplayCsv(Csv csv)
        {
            displayingCsv = csv;
            //DisplayCsvWithListView(csv);
            DisplayCsvWithDataGridView(csv);
        }

        DataTable GetDataTable(Csv csv)
        {
            var dt = new DataTable();
            var headers = csv.Headers;
            for (var i = 0; i < headers.Count; i++)
            {
                var header = headers[i];
                dt.Columns.Add(header);
            }
            for (var i = 0; i < csv.VLineCount; i++)
            {
                var vLine = csv.GetVLine(i);
                var dRow = dt.NewRow();
                for (var j = 0; j < headers.Count; j++)
                {
                    var header = headers[j];
                    dRow[header] = vLine.GetValue(header);
                }
                dt.Rows.Add(dRow);
            }
            dt.ColumnChanged += (s, e) =>
            {
                var index = dt.Rows.IndexOf(e.Row);
                if (index == -1) return;
                var vLine = csv.GetVLine(index);
                vLine.SetValue(e.Column.ColumnName, e.ProposedValue.ToString());
                UpdateText();
            };
            return dt;
        }

        void DisplayCsvWithDataGridView(Csv csv)
        {
            dataGridView.DataSource = GetDataTable(csv);
            list.Items.Clear();
            for (var i = 0; i < csv.Headers.Count; i++)
            {
                var header = csv.Headers[i];
                var item = new CsvListBoxItem()
                {
                    DisplayValue = header,
                    Value = header,
                };
                list.Items.Add(item);
            }
        }

        void DisplayCsvWithListView(Csv csv)
        {
            listView.Visible = true;
            listView.Items.Clear();
            listView.Columns.Clear();

            listView.BeginUpdate();

            for (var i = 0; i < csv.Headers.Count; i++)
            {
                var header = csv.Headers[i];
                listView.Columns.Add(header, 100);
            }

            for (var i = 0; i < csv.VLineCount; i++)
            {
                var vLine = csv.GetVLine(i);
                var item = new ListViewItem();
                for (var j = 0; j < csv.Headers.Count; j++)
                {
                    var header = csv.Headers[j];
                    var value = vLine.GetValue(header);
                    if (j == 0)
                    {
                        item.Text = value;
                    }
                    else
                    {
                        item.SubItems.Add(value);
                    }
                }
                listView.Items.Add(item);
            }

            listView.EndUpdate();
        }

        void UpdateText()
        {
            Text = string.Format("{0}{1}", filePath, csv.Changed ? "[未保存]" : "");
        }

        void OpenFile()
        {
            if (csv != null && csv.Changed)
            {
                if (MessageBox.Show("文件还未保存,确定打开新文件?", "提示", MessageBoxButtons.YesNoCancel) != DialogResult.Yes)
                    return;
            }
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var filePath = openFileDialog.FileName;
                this.filePath = filePath;
                csv = Csv.FromFile(filePath);
                DisplayAll();
                openFileDialog.InitialDirectory = new FileInfo(filePath).DirectoryName;
                UpdateText();
            }
        }

        void DisplayAll()
        {
            if (csv == null)
            {
                MessageBox.Show("请先打开文件");
                return;
            }
            CsvCommandRunner.GetInstance().Run(csv, "select * where 1");
        }

        private void menuOpenFile_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        void SaveFile()
        {
            if (csv == null)
                return;
            csv.Save();
            UpdateText();
        }

        private void menuSaveFile_Click(object sender, EventArgs e)
        {
            SaveFile();
        }

        void SaveAs()
        {
            if (csv == null)
                return;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                var filePath = saveFileDialog.FileName;
                csv.SaveAs(filePath);
                saveFileDialog.InitialDirectory = new FileInfo(filePath).DirectoryName;
                UpdateText();
            }
        }

        private void menuSaveAs_Click(object sender, EventArgs e)
        {
            SaveAs();
        }

        void FocusCommandInput()
        {
            txtCommand.Focus();
        }

        private void dataGridView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.R)
            {
                FocusCommandInput();
            }
        }

        bool RunCommand(string s)
        {
            if (csv == null)
            {
                MessageBox.Show("请先打开文件");
                return false;
            }
            var runner = CsvCommandRunner.GetInstance();
            var r = runner.Run(csv, s);
            return r != null;
        }

        private void cCommand_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                var cmdStr = txtCommand.Text;
                var r = RunCommand(cmdStr);
                if (r)
                {
                    txtCommand.Text = string.Empty;
                    if (!txtCommand.Items.Contains(cmdStr))
                        txtCommand.Items.Add(cmdStr);
                }
            }
        }

        private void menuDisplayAll_Click(object sender, EventArgs e)
        {
            DisplayAll();
        }

        string GetTokenNearFocus()
        {
            var s = txtCommand.Text;
            if (s == string.Empty) return "";
            var offset = txtCommand.SelectionStart;
            if (offset >= s.Length || (offset > 0 && s[offset] == ' ')) offset--;
            left = offset;
            while (left > 0 && s[left] != ' ') left--;
            right = offset;
            while (right < s.Length - 1 && s[right] != ' ') right++;
            return s.Substring(left, right - left + 1);
        }

        private void menuRun_Click(object sender, EventArgs e)
        {
            var token = GetTokenNearFocus();
        }

        private void Frm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (csv == null) return;
            if (csv.Changed)
            {
                var r = MessageBox.Show(string.Format("文件:{0}有内容尚未保存,需要保存吗?", filePath), "保存", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                if (r == DialogResult.Yes)
                    csv.Save();
                else if (r == DialogResult.Cancel)
                    e.Cancel = true;
            }
        }

        class CsvListBoxItem
        {
            public string DisplayValue { get; set; }

            public string Value { get; set; }

            public override string ToString()
            {
                return DisplayValue;
            }
        }
    }
}
