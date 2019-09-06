namespace CsvEditor
{
    partial class Frm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuOpenFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSaveFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDisplay = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDisplayAll = new System.Windows.Forms.ToolStripMenuItem();
            this.txtCommand = new System.Windows.Forms.ToolStripComboBox();
            this.menuCommand = new System.Windows.Forms.ToolStripMenuItem();
            this.menuRun = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.list = new System.Windows.Forms.ListBox();
            this.dataGridView = new CsvEditor.DoubleBufferedDataGridView();
            this.listView = new CsvEditor.DoubleBufferedListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFile,
            this.menuDisplay,
            this.txtCommand,
            this.menuCommand});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1272, 29);
            this.menuStrip.TabIndex = 2;
            this.menuStrip.Text = "menuStrip1";
            // 
            // menuFile
            // 
            this.menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuOpenFile,
            this.menuSaveFile,
            this.menuSaveAs});
            this.menuFile.Name = "menuFile";
            this.menuFile.Size = new System.Drawing.Size(58, 25);
            this.menuFile.Text = "文件(&F)";
            // 
            // menuOpenFile
            // 
            this.menuOpenFile.Name = "menuOpenFile";
            this.menuOpenFile.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.menuOpenFile.Size = new System.Drawing.Size(191, 22);
            this.menuOpenFile.Text = "打开";
            this.menuOpenFile.Click += new System.EventHandler(this.menuOpenFile_Click);
            // 
            // menuSaveFile
            // 
            this.menuSaveFile.Name = "menuSaveFile";
            this.menuSaveFile.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.menuSaveFile.Size = new System.Drawing.Size(191, 22);
            this.menuSaveFile.Text = "保存";
            this.menuSaveFile.Click += new System.EventHandler(this.menuSaveFile_Click);
            // 
            // menuSaveAs
            // 
            this.menuSaveAs.Name = "menuSaveAs";
            this.menuSaveAs.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.A)));
            this.menuSaveAs.Size = new System.Drawing.Size(191, 22);
            this.menuSaveAs.Text = "另存为";
            this.menuSaveAs.Click += new System.EventHandler(this.menuSaveAs_Click);
            // 
            // menuDisplay
            // 
            this.menuDisplay.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuDisplayAll});
            this.menuDisplay.Name = "menuDisplay";
            this.menuDisplay.Size = new System.Drawing.Size(61, 25);
            this.menuDisplay.Text = "显示(&D)";
            // 
            // menuDisplayAll
            // 
            this.menuDisplayAll.Name = "menuDisplayAll";
            this.menuDisplayAll.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.menuDisplayAll.Size = new System.Drawing.Size(170, 22);
            this.menuDisplayAll.Text = "显示全部";
            this.menuDisplayAll.Click += new System.EventHandler(this.menuDisplayAll_Click);
            // 
            // txtCommand
            // 
            this.txtCommand.Name = "txtCommand";
            this.txtCommand.Size = new System.Drawing.Size(400, 25);
            this.txtCommand.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cCommand_KeyUp);
            // 
            // menuCommand
            // 
            this.menuCommand.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuRun});
            this.menuCommand.Name = "menuCommand";
            this.menuCommand.Size = new System.Drawing.Size(60, 25);
            this.menuCommand.Text = "命令(&C)";
            // 
            // menuRun
            // 
            this.menuRun.Name = "menuRun";
            this.menuRun.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.menuRun.Size = new System.Drawing.Size(145, 22);
            this.menuRun.Text = "执行";
            this.menuRun.Click += new System.EventHandler(this.menuRun_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "Csv文件|*.csv";
            this.openFileDialog.Title = "打开";
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "csv";
            this.saveFileDialog.Filter = "Csv文件|*.csv";
            // 
            // list
            // 
            this.list.FormattingEnabled = true;
            this.list.ItemHeight = 17;
            this.list.Location = new System.Drawing.Point(0, 29);
            this.list.Name = "list";
            this.list.Size = new System.Drawing.Size(303, 89);
            this.list.TabIndex = 3;
            this.list.Visible = false;
            // 
            // dataGridView
            // 
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(0, 29);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowTemplate.Height = 23;
            this.dataGridView.Size = new System.Drawing.Size(1272, 621);
            this.dataGridView.TabIndex = 1;
            this.dataGridView.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dataGridView_KeyUp);
            // 
            // listView
            // 
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView.Location = new System.Drawing.Point(0, 29);
            this.listView.Margin = new System.Windows.Forms.Padding(4);
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(1272, 621);
            this.listView.TabIndex = 0;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.Visible = false;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "dasd";
            // 
            // Frm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1272, 650);
            this.Controls.Add(this.list);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.listView);
            this.Controls.Add(this.menuStrip);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Consolas", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainMenuStrip = this.menuStrip;
            this.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.Name = "Frm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Frm_FormClosing);
            this.Load += new System.EventHandler(this.Frm_Load);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DoubleBufferedListView listView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private DoubleBufferedDataGridView dataGridView;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem menuFile;
        private System.Windows.Forms.ToolStripMenuItem menuOpenFile;
        private System.Windows.Forms.ToolStripMenuItem menuSaveFile;
        private System.Windows.Forms.ToolStripMenuItem menuSaveAs;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.ToolStripComboBox txtCommand;
        private System.Windows.Forms.ToolStripMenuItem menuDisplay;
        private System.Windows.Forms.ToolStripMenuItem menuDisplayAll;
        private System.Windows.Forms.ListBox list;
        private System.Windows.Forms.ToolStripMenuItem menuCommand;
        private System.Windows.Forms.ToolStripMenuItem menuRun;
    }
}

