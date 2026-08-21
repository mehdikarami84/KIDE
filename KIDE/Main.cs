using KIDE;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Timer = System.Windows.Forms.Timer;

namespace KIDE
{
    public partial class Main : Form
    {
        private class SyntaxRule
        {
            public Regex Pattern { get; }
            public Color Color { get; }

            public SyntaxRule(string pattern, Color color)
            {
                Pattern = new Regex(
                    pattern,
                    RegexOptions.Compiled
                );

                Color = color;
            }
        }

        private string currentFilePath = null;
        private bool isModified = false;
        private List<RecentFile> recentFiles = new List<RecentFile>();
        private readonly string recentFilesPath = Path.Combine(Application.StartupPath, "recentFiles.json");
        private bool isDarkTheme = false;
        private bool isApplyingSyntaxHighlighting = false;
        private Timer syntaxHighlightTimer;

        // ===============================
        // Syntax Highlighting Colors
        // ===============================

        private static readonly Color LightKeywordColor =
            Color.FromArgb(0, 51, 102);

        private static readonly Color LightDataTypeColor =
            Color.FromArgb(0, 128, 128);

        private static readonly Color LightFunctionColor =
            Color.FromArgb(255, 140, 0);

        private static readonly Color LightNumberColor =
            Color.FromArgb(128, 0, 128);

        private static readonly Color LightStringColor =
            Color.FromArgb(0, 100, 0);

        private static readonly Color LightCommentColor =
            Color.FromArgb(128, 128, 128);

        private static readonly Color LightPreprocessorColor =
            Color.FromArgb(0, 139, 139);

        private static readonly Color LightOperatorColor =
            Color.FromArgb(128, 0, 0);

        private static readonly Color LightBracketColor =
            Color.FromArgb(184, 134, 11);

        private static readonly Color LightVariableColor =
            Color.FromArgb(139, 0, 0);


        // ===============================
        // Dark Theme
        // ===============================

        private static readonly Color DarkKeywordColor =
            Color.FromArgb(198, 120, 221);

        private static readonly Color DarkDataTypeColor =
            Color.FromArgb(224, 108, 117);

        private static readonly Color DarkFunctionColor =
            Color.FromArgb(97, 175, 254);

        private static readonly Color DarkNumberColor =
            Color.FromArgb(209, 154, 102);

        private static readonly Color DarkStringColor =
            Color.FromArgb(152, 195, 121);

        private static readonly Color DarkCommentColor =
            Color.FromArgb(92, 99, 112);

        private static readonly Color DarkPreprocessorColor =
            Color.FromArgb(86, 182, 194);

        private static readonly Color DarkOperatorColor =
            Color.FromArgb(213, 94, 0);

        private static readonly Color DarkBracketColor =
            Color.FromArgb(171, 178, 191);

        private static readonly Color DarkVariableColor =
            Color.FromArgb(229, 192, 123);

        public Main()
        {
            InitializeComponent();
            LoadRecentFiles();
            ApplySyntaxHighlighting();

            syntaxHighlightTimer = new Timer();
            syntaxHighlightTimer.Interval = 150;
            syntaxHighlightTimer.Tick += SyntaxHighlightTimer_Tick;
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
            if (isApplyingSyntaxHighlighting)
                return;

            syntaxHighlightTimer.Stop();
            syntaxHighlightTimer.Start();
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
        private void RefreshTreeView()
        {
            projectTreeView.Nodes.Clear();

            TreeNode rootNode = new TreeNode("All Projects");

            if (recentFiles.Count == 0)
            {
                TreeNode emptyNode =
                    new TreeNode("No projects saved.");

                emptyNode.ForeColor = Color.Gray;

                rootNode.Nodes.Add(emptyNode);
            }
            else
            {
                foreach (RecentFile file in recentFiles)
                {
                    TreeNode fileNode =
                        new TreeNode(file.FileName);

                    fileNode.Tag = file;

                    rootNode.Nodes.Add(fileNode);
                }
            }

            projectTreeView.Nodes.Add(rootNode);

            rootNode.Expand();
        }
        private void LoadRecentFiles()
        {
            recentFiles.Clear();

            if (!File.Exists(recentFilesPath))
            {
                RefreshTreeView();
                return;
            }

            try
            {
                string json = File.ReadAllText(recentFilesPath);

                recentFiles = JsonSerializer.Deserialize<List<RecentFile>>(json)
                              ?? new List<RecentFile>();
            }
            catch
            {
                recentFiles = new List<RecentFile>();
            }

            RefreshTreeView();
        }
        private void SaveRecentFiles()
        {
            try
            {
                string json = JsonSerializer.Serialize(
                    recentFiles,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });

                File.WriteAllText(recentFilesPath, json);
            }
            catch
            {

            }
        }
        private void AddRecentFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return;

            string fullPath = Path.GetFullPath(filePath);

            recentFiles.RemoveAll(
                file => string.Equals(
                    file.FilePath,
                    fullPath,
                    StringComparison.OrdinalIgnoreCase));

            recentFiles.Insert(0, new RecentFile
            {
                FileName = Path.GetFileName(fullPath),
                FilePath = fullPath
            });

            SaveRecentFiles();

            RefreshTreeView();
        }
        private void projectTreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node == null)
                return;

            if (e.Node.Tag is not RecentFile recentFile)
                return;

            if (!File.Exists(recentFile.FilePath))
            {
                MessageBox.Show(
                    $"The requested file was not found.\n\n{recentFile.FilePath}",
                    "File not found",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            OpenFile(recentFile.FilePath);
        }
        private void SaveFile()
        {
            if (string.IsNullOrEmpty(currentFilePath))
            {
                SaveFileAs();
                return;
            }

            try
            {
                File.WriteAllText(currentFilePath, codeEditor.Text);

                isModified = false;
                UpdateWindowTitle();
                AddRecentFile(currentFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"File saving failed.\n\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        private void SaveFileAs()
        {
            if (saveFileDialog.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                string filePath = saveFileDialog.FileName;

                File.WriteAllText(filePath, codeEditor.Text);

                currentFilePath = filePath;
                isModified = false;

                UpdateWindowTitle();
                AddRecentFile(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"File saving failed.\n\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        private void NewFile()
        {
            if (!ConfirmSaveChanges())
                return;

            codeEditor.Clear();

            currentFilePath = null;

            isModified = false;

            UpdateWindowTitle();
        }
        private void OpenFile(string filePath)
        {
            try
            {
                codeEditor.Text = File.ReadAllText(filePath);

                currentFilePath = filePath;
                isModified = false;

                UpdateWindowTitle();
                AddRecentFile(filePath);
                ApplySyntaxHighlighting();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Unable to open file.\n\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        private void UpdateWindowTitle()
        {
            if (string.IsNullOrEmpty(currentFilePath))
            {
                this.Text = isModified
                    ? "KIDE - Untitled *"
                    : "KIDE - Untitled";

                return;
            }

            string fileName = Path.GetFileName(currentFilePath);

            this.Text = isModified
                ? $"KIDE - {fileName} *"
                : $"KIDE - {fileName}";
        }
        private bool ConfirmSaveChanges()
        {
            if (!isModified)
                return true;

            DialogResult result = MessageBox.Show(
                "The current file has unsaved changes. Do you want to save them?",
                "Unsaved Changes",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                SaveFile();
                return !isModified;
            }

            if (result == DialogResult.No)
            {
                return true;
            }

            return false;
        }
        private Color GetEditorBackColor()
        {
            return isDarkTheme ? Color.FromArgb(30, 30, 35) : Color.White;
        }
        private Color GetEditorDefaultTextColor()
        {
            return isDarkTheme ? Color.White : Color.Black;
        }
        private void ApplySyntaxHighlighting()
        {
            if (codeEditor == null)
                return;

            if (isApplyingSyntaxHighlighting)
                return;

            isApplyingSyntaxHighlighting = true;

            try
            {
                int selectionStart = codeEditor.SelectionStart;
                int selectionLength = codeEditor.SelectionLength;

                string text = codeEditor.Text;

                Color defaultColor = GetEditorDefaultTextColor();

                List<SyntaxRule> rules = isDarkTheme
                    ? GetDarkSyntaxRules()
                    : GetLightSyntaxRules();

                codeEditor.SuspendLayout();

                // ابتدا کل متن را به رنگ پیش‌فرض برمی‌گردانیم
                codeEditor.SelectAll();
                codeEditor.SelectionColor = defaultColor;

                // مشخص می‌کند هر کاراکتر قبلاً توسط یک Rule رنگ شده یا نه
                bool[] colored = new bool[text.Length];

                foreach (SyntaxRule rule in rules)
                {
                    MatchCollection matches = rule.Pattern.Matches(text);

                    foreach (Match match in matches)
                    {
                        bool canColor = true;

                        // بررسی می‌کنیم این قسمت قبلاً رنگ نشده باشد
                        for (int i = match.Index;
                             i < match.Index + match.Length;
                             i++)
                        {
                            if (colored[i])
                            {
                                canColor = false;
                                break;
                            }
                        }

                        if (!canColor)
                            continue;

                        // اعمال رنگ
                        codeEditor.Select(
                            match.Index,
                            match.Length
                        );

                        codeEditor.SelectionColor = rule.Color;

                        // علامت‌گذاری این قسمت به عنوان رنگ‌شده
                        for (int i = match.Index;
                             i < match.Index + match.Length;
                             i++)
                        {
                            colored[i] = true;
                        }
                    }
                }

                // برگرداندن Cursor / Selection
                codeEditor.Select(
                    selectionStart,
                    selectionLength
                );

                codeEditor.ResumeLayout();
            }
            finally
            {
                isApplyingSyntaxHighlighting = false;
            }
        }
        private List<SyntaxRule> GetLightSyntaxRules()
        {
            return new List<SyntaxRule>
                {
                    // Keywords
                    new SyntaxRule(
                        @"\b(class|if|while|for|return|else|switch|case|break|continue|do|default|const)\b",
                        LightKeywordColor
                    ),

                    // Data Types
                    new SyntaxRule(
                        @"\b(int|float|double|char|bool|void|string|long|short|unsigned|signed)\b",
                        LightDataTypeColor
                    ),

                    // Function names
                    new SyntaxRule(
                        @"\b(\w+)\s*\(\s*\)",
                        LightFunctionColor
                    ),

                    // Numbers
                    new SyntaxRule(
                        @"\b\d+(\.\d+)?\b",
                        LightNumberColor
                    ),

                    // Strings
                    new SyntaxRule(
                        @"""([^""\\]|\\.)*""",
                        LightStringColor
                    ),

                    // Characters
                    new SyntaxRule(
                        @"'([^'\\]|\\.)'",
                        LightStringColor
                    ),

                    // Single-line comments
                    new SyntaxRule(
                        @"//.*",
                        LightCommentColor
                    ),

                    // Multi-line comments
                    new SyntaxRule(
                        @"/\*[\s\S]*?\*/",
                        LightCommentColor
                    ),

                    // Preprocessor
                    new SyntaxRule(
                        @"#\s*\w+",
                        LightPreprocessorColor
                    ),

                    // Operators
                    new SyntaxRule(
                        @"[-+*/=<>!&|]+",
                        LightOperatorColor
                    ),

                    // Brackets
                    new SyntaxRule(
                        @"[\{\}\[\]\(\)]",
                        LightBracketColor
                    ),

                    // Variables / identifiers
                    new SyntaxRule(
                        @"\b\w+\b",
                        LightVariableColor
                    )
                };
        }
        private List<SyntaxRule> GetDarkSyntaxRules()
        {
            return new List<SyntaxRule>
                {
                    // Keywords
                    new SyntaxRule(
                        @"\b(class|if|while|for|return|else|switch|case|break|continue|do|default|const)\b",
                        DarkKeywordColor
                    ),

                    // Data Types
                    new SyntaxRule(
                        @"\b(int|float|double|char|bool|void|string|long|short|unsigned|signed)\b",
                        DarkDataTypeColor
                    ),

                    // Function names
                    new SyntaxRule(
                        @"\b(\w+)\s*\(\s*\)",
                        DarkFunctionColor
                    ),

                    // Numbers
                    new SyntaxRule(
                        @"\b\d+(\.\d+)?\b",
                        DarkNumberColor
                    ),

                    // Strings
                    new SyntaxRule(
                        @"""([^""\\]|\\.)*""",
                        DarkStringColor
                    ),

                    // Characters
                    new SyntaxRule(
                        @"'([^'\\]|\\.)'",
                        DarkStringColor
                    ),

                    // Single-line comments
                    new SyntaxRule(
                        @"//.*",
                        DarkCommentColor
                    ),

                    // Multi-line comments
                    new SyntaxRule(
                        @"/\*[\s\S]*?\*/",
                        DarkCommentColor
                    ),

                    // Preprocessor
                    new SyntaxRule(
                        @"#\s*\w+",
                        DarkPreprocessorColor
                    ),

                    // Operators
                    new SyntaxRule(
                        @"[-+*/=<>!&|]+",
                        DarkOperatorColor
                    ),

                    // Brackets
                    new SyntaxRule(
                        @"[\{\}\[\]\(\)]",
                        DarkBracketColor
                    ),

                    // Variables / identifiers
                    new SyntaxRule(
                        @"\b\w+\b",
                        DarkVariableColor
                    )
                };
        }

        private void lightModeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            isDarkTheme = false;
        }

        private void darkModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isDarkTheme = true;
        }
        private void SyntaxHighlightTimer_Tick(object sender, EventArgs e)
        {
            syntaxHighlightTimer.Stop();

            if (isApplyingSyntaxHighlighting)
                return;

            ApplySyntaxHighlighting();
        }

        private void projectTreeView_NodeMouseDoubleClick_1(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node == null)
                return;

            if (e.Node.Tag is not RecentFile recentFile)
                return;

            if (!File.Exists(recentFile.FilePath))
            {
                MessageBox.Show(
                    $"The requested file was not found.\n\n{recentFile.FilePath}",
                    "File not found",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            OpenFile(recentFile.FilePath);
        }
    }
}