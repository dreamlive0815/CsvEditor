using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CsvEditor
{
    class DoubleBufferedDataGridView : DataGridView
    {
        public DoubleBufferedDataGridView() : base()
        {
            SetStyle(
               ControlStyles.DoubleBuffer |
               ControlStyles.OptimizedDoubleBuffer |
               ControlStyles.AllPaintingInWmPaint,
               true
            );
            UpdateStyles();
        }
    }
}
