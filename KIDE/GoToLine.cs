using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KIDE
{
    public partial class GoToLine : Form
    {
        public int LineNumber
        {
            get
            {
                return (int)lineNumberInput.Value;
            }
        }

        public GoToLine(int maxLine)
        {
            InitializeComponent();

            lineNumberInput.Minimum = 1;
            lineNumberInput.Maximum = Math.Max(1, maxLine);

            lineNumberInput.Value = 1;

            this.AcceptButton = okButton;
            this.CancelButton = cancelButton;

            ThemeManager.ApplyTheme(this);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
