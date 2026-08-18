using System.Windows.Forms;

namespace KIDE
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void viewToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void debugCompileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!ConfirmSaveChanges())
                return;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                OpenFile(openFileDialog.FileName);
            }
        }

        private void saveProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile();
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileAs();
        }

        private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewFile();
        }

        private void codeEditor_TextChanged(object sender, EventArgs e)
        {
            isModified = true;

            UpdateWindowTitle();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!ConfirmSaveChanges())
            {
                e.Cancel = true;
            }
        }

        private void Main_Load(object sender, EventArgs e)
        {
            TreeNode fileNode = new TreeNode("main.cpp");

            fileNode.Tag = @"D:\University\Term 1\Basic programming\Project\Project\main.cpp";

            projectTreeView.Nodes.Add(fileNode);
        }
    }
}
