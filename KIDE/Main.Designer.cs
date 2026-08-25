using System.IO;

namespace KIDE
{
    partial class Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            newProjectToolStripMenuItem = new ToolStripMenuItem();
            openToolStripMenuItem = new ToolStripMenuItem();
            saveProjectToolStripMenuItem = new ToolStripMenuItem();
            saveToolStripMenuItem = new ToolStripMenuItem();
            exitToolStripMenuItem = new ToolStripMenuItem();
            editToolStripMenuItem = new ToolStripMenuItem();
            undoToolStripMenuItem = new ToolStripMenuItem();
            redoToolStripMenuItem = new ToolStripMenuItem();
            viewToolStripMenuItem = new ToolStripMenuItem();
            themeToolStripMenuItem = new ToolStripMenuItem();
            lightModeToolStripMenuItem1 = new ToolStripMenuItem();
            darkModeToolStripMenuItem = new ToolStripMenuItem();
            debugCompileToolStripMenuItem = new ToolStripMenuItem();
            buildToolStripMenuItem = new ToolStripMenuItem();
            rebuildToolStripMenuItem = new ToolStripMenuItem();
            cleanToolStripMenuItem = new ToolStripMenuItem();
            runToolStripMenuItem = new ToolStripMenuItem();
            runToolStripMenuItem1 = new ToolStripMenuItem();
            debugToolStripMenuItem = new ToolStripMenuItem();
            compileToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            splitContainer1 = new SplitContainer();
            splitContainer2 = new SplitContainer();
            projectTreeView = new TreeView();
            codeEditor = new RichTextBox();
            errorBox = new RichTextBox();
            saveFileDialog = new SaveFileDialog();
            openFileDialog = new OpenFileDialog();
            menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, editToolStripMenuItem, viewToolStripMenuItem, debugCompileToolStripMenuItem, runToolStripMenuItem, compileToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1054, 28);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { newProjectToolStripMenuItem, openToolStripMenuItem, saveProjectToolStripMenuItem, saveToolStripMenuItem, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(46, 24);
            fileToolStripMenuItem.Text = "File";
            // 
            // newProjectToolStripMenuItem
            // 
            newProjectToolStripMenuItem.Name = "newProjectToolStripMenuItem";
            newProjectToolStripMenuItem.Size = new Size(143, 26);
            newProjectToolStripMenuItem.Text = "New";
            newProjectToolStripMenuItem.Click += newProjectToolStripMenuItem_Click;
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.Size = new Size(143, 26);
            openToolStripMenuItem.Text = "Open";
            openToolStripMenuItem.Click += openToolStripMenuItem_Click;
            // 
            // saveProjectToolStripMenuItem
            // 
            saveProjectToolStripMenuItem.Name = "saveProjectToolStripMenuItem";
            saveProjectToolStripMenuItem.Size = new Size(143, 26);
            saveProjectToolStripMenuItem.Text = "Save";
            saveProjectToolStripMenuItem.Click += saveProjectToolStripMenuItem_Click;
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.Size = new Size(143, 26);
            saveToolStripMenuItem.Text = "Save As";
            saveToolStripMenuItem.Click += saveToolStripMenuItem_Click;
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(143, 26);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // editToolStripMenuItem
            // 
            editToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { undoToolStripMenuItem, redoToolStripMenuItem });
            editToolStripMenuItem.Name = "editToolStripMenuItem";
            editToolStripMenuItem.Size = new Size(49, 24);
            editToolStripMenuItem.Text = "Edit";
            // 
            // undoToolStripMenuItem
            // 
            undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            undoToolStripMenuItem.Size = new Size(128, 26);
            undoToolStripMenuItem.Text = "Undo";
            // 
            // redoToolStripMenuItem
            // 
            redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            redoToolStripMenuItem.Size = new Size(128, 26);
            redoToolStripMenuItem.Text = "Redo";
            // 
            // viewToolStripMenuItem
            // 
            viewToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { themeToolStripMenuItem });
            viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            viewToolStripMenuItem.Size = new Size(55, 24);
            viewToolStripMenuItem.Text = "View";
            // 
            // themeToolStripMenuItem
            // 
            themeToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { lightModeToolStripMenuItem1, darkModeToolStripMenuItem });
            themeToolStripMenuItem.Name = "themeToolStripMenuItem";
            themeToolStripMenuItem.Size = new Size(137, 26);
            themeToolStripMenuItem.Text = "Theme";
            // 
            // lightModeToolStripMenuItem1
            // 
            lightModeToolStripMenuItem1.Name = "lightModeToolStripMenuItem1";
            lightModeToolStripMenuItem1.Size = new Size(168, 26);
            lightModeToolStripMenuItem1.Text = "Light Mode";
            lightModeToolStripMenuItem1.Click += lightModeToolStripMenuItem1_Click;
            // 
            // darkModeToolStripMenuItem
            // 
            darkModeToolStripMenuItem.Name = "darkModeToolStripMenuItem";
            darkModeToolStripMenuItem.Size = new Size(168, 26);
            darkModeToolStripMenuItem.Text = "Dark Mode";
            darkModeToolStripMenuItem.Click += darkModeToolStripMenuItem_Click;
            // 
            // debugCompileToolStripMenuItem
            // 
            debugCompileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { buildToolStripMenuItem, rebuildToolStripMenuItem, cleanToolStripMenuItem });
            debugCompileToolStripMenuItem.Name = "debugCompileToolStripMenuItem";
            debugCompileToolStripMenuItem.Size = new Size(57, 24);
            debugCompileToolStripMenuItem.Text = "Build";
            // 
            // buildToolStripMenuItem
            // 
            buildToolStripMenuItem.Name = "buildToolStripMenuItem";
            buildToolStripMenuItem.Size = new Size(143, 26);
            buildToolStripMenuItem.Text = "Build";
            // 
            // rebuildToolStripMenuItem
            // 
            rebuildToolStripMenuItem.Name = "rebuildToolStripMenuItem";
            rebuildToolStripMenuItem.Size = new Size(143, 26);
            rebuildToolStripMenuItem.Text = "Rebuild";
            // 
            // cleanToolStripMenuItem
            // 
            cleanToolStripMenuItem.Name = "cleanToolStripMenuItem";
            cleanToolStripMenuItem.Size = new Size(143, 26);
            cleanToolStripMenuItem.Text = "Clean";
            // 
            // runToolStripMenuItem
            // 
            runToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { runToolStripMenuItem1, debugToolStripMenuItem });
            runToolStripMenuItem.Name = "runToolStripMenuItem";
            runToolStripMenuItem.Size = new Size(48, 24);
            runToolStripMenuItem.Text = "Run";
            // 
            // runToolStripMenuItem1
            // 
            runToolStripMenuItem1.Name = "runToolStripMenuItem1";
            runToolStripMenuItem1.Size = new Size(137, 26);
            runToolStripMenuItem1.Text = "Run";
            // 
            // debugToolStripMenuItem
            // 
            debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            debugToolStripMenuItem.Size = new Size(137, 26);
            debugToolStripMenuItem.Text = "Debug";
            debugToolStripMenuItem.Click += debugToolStripMenuItem_Click;
            // 
            // compileToolStripMenuItem
            // 
            compileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { aboutToolStripMenuItem });
            compileToolStripMenuItem.Name = "compileToolStripMenuItem";
            compileToolStripMenuItem.Size = new Size(55, 24);
            compileToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(133, 26);
            aboutToolStripMenuItem.Text = "About";
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 28);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(errorBox);
            splitContainer1.Size = new Size(1054, 544);
            splitContainer1.SplitterDistance = 351;
            splitContainer1.TabIndex = 1;
            // 
            // splitContainer2
            // 
            splitContainer2.Dock = DockStyle.Fill;
            splitContainer2.Location = new Point(0, 0);
            splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(projectTreeView);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(codeEditor);
            splitContainer2.Size = new Size(1054, 351);
            splitContainer2.SplitterDistance = 290;
            splitContainer2.TabIndex = 0;
            // 
            // projectTreeView
            // 
            projectTreeView.Dock = DockStyle.Fill;
            projectTreeView.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            projectTreeView.Location = new Point(0, 0);
            projectTreeView.Name = "projectTreeView";
            projectTreeView.Size = new Size(290, 351);
            projectTreeView.TabIndex = 0;
            projectTreeView.NodeMouseDoubleClick += projectTreeView_NodeMouseDoubleClick_1;
            // 
            // codeEditor
            // 
            codeEditor.AcceptsTab = true;
            codeEditor.DetectUrls = false;
            codeEditor.Dock = DockStyle.Fill;
            codeEditor.Font = new Font("Consolas", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            codeEditor.Location = new Point(0, 0);
            codeEditor.Name = "codeEditor";
            codeEditor.Size = new Size(760, 351);
            codeEditor.TabIndex = 0;
            codeEditor.Text = "";
            codeEditor.WordWrap = false;
            codeEditor.TextChanged += codeEditor_TextChanged;
            codeEditor.KeyDown += codeEditor_KeyDown;
            codeEditor.KeyPress += codeEditor_KeyPress;
            // 
            // errorBox
            // 
            errorBox.Dock = DockStyle.Fill;
            errorBox.Font = new Font("Consolas", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            errorBox.Location = new Point(0, 0);
            errorBox.Name = "errorBox";
            errorBox.ReadOnly = true;
            errorBox.Size = new Size(1054, 189);
            errorBox.TabIndex = 0;
            errorBox.Text = "";
            errorBox.WordWrap = false;
            // 
            // saveFileDialog
            // 
            saveFileDialog.Filter = "C++ Files (*.cpp;*.h;*.hpp)|*.cpp;*.h;*.hpp|All Files (*.*)|*.*";
            // 
            // openFileDialog
            // 
            openFileDialog.FileName = "openFileDialog1";
            openFileDialog.Filter = "C++ Files (*.cpp;*.h;*.hpp)|*.cpp;*.h;*.hpp|All Files (*.*)|*.*";
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1054, 572);
            Controls.Add(splitContainer1);
            Controls.Add(menuStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            Name = "Main";
            Text = "KIDE";
            FormClosing += Main_FormClosing;
            KeyDown += Main_KeyDown;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem viewToolStripMenuItem;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem debugCompileToolStripMenuItem;
        private ToolStripMenuItem newProjectToolStripMenuItem;
        private ToolStripMenuItem saveProjectToolStripMenuItem;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem undoToolStripMenuItem;
        private ToolStripMenuItem redoToolStripMenuItem;
        private ToolStripMenuItem compileToolStripMenuItem;
        private ToolStripMenuItem runToolStripMenuItem;
        private ToolStripMenuItem themeToolStripMenuItem;
        private ToolStripMenuItem lightModeToolStripMenuItem1;
        private ToolStripMenuItem darkModeToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripMenuItem buildToolStripMenuItem;
        private ToolStripMenuItem rebuildToolStripMenuItem;
        private ToolStripMenuItem cleanToolStripMenuItem;
        private ToolStripMenuItem runToolStripMenuItem1;
        private ToolStripMenuItem debugToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private SplitContainer splitContainer1;
        private SplitContainer splitContainer2;
        private TreeView projectTreeView;
        private RichTextBox codeEditor;
        private RichTextBox errorBox;
        private SaveFileDialog saveFileDialog;
        private OpenFileDialog openFileDialog;
    }
}