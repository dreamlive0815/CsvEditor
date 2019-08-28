using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CsvEditor
{
    public partial class Frm : Form
    {
        public Frm()
        {
            InitializeComponent();
        }

        private void Frm_Load(object sender, EventArgs e)
        {
            
            

            Test();
            
        }

        private void DisplayCsv(Csv csv)
        {
            DisplayCsvWithListView(csv);
        }

        void DisplayCsvWithListView(Csv csv)
        {
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

        void Test()
        {
            /*
            var csv = Csv.FromFile("../../csv/equip.csv");
            var cmd = new CsvSelectCommand("select Name, Category where (Name = 树枝 and Category = 零件) or Name has 灵魂石");
            //var cmd = new CsvSelectCommand("select * where true");
            var select = cmd.DoSelect(csv);
            DisplayCsv(select);
            */

            CsvTest.Test(null);
        }
    }
}
