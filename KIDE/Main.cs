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
using System.Diagnostics;
using System.Text;

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
        private bool isUndoRedoOperation = false;
        private bool isInternalEditorChange = false;
        private Stack<string> undoStack = new Stack<string>();
        private Stack<string> redoStack = new Stack<string>();
        private string lastEditorText = "";
        private string tempSourceFile;
        private string tempExeFile;
        private Process runningProcess;
        private int inputStartPosition = 0;
        private bool consoleRunning = false;      

        public Main()
        {
            InitializeComponent();
            LoadRecentFiles();
            ApplySyntaxHighlighting();

            syntaxHighlightTimer = new Timer();
            syntaxHighlightTimer.Interval = 150;
            syntaxHighlightTimer.Tick += SyntaxHighlightTimer_Tick;
            this.KeyPreview = true;
            ThemeManager.ApplyTheme(this);
        }

        // ===============================
        // Menu Handling
        // ===============================
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
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void lightModeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ThemeManager.SetLightTheme();
            ApplySyntaxHighlighting();
        }
        private void darkModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ThemeManager.SetDarkTheme();
            ApplySyntaxHighlighting();
        }
        private void debugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunDebugger();
        }
        private async void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await RunCode();
        }
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (AboutForm aboutForm = new AboutForm())
            {
                aboutForm.ShowDialog(this);
            }
        }

        // ===============================
        // Syntax Highlighting
        // ===============================
        // Light Theme Colors
        private static readonly Color LightKeywordColor = Color.FromArgb(0, 51, 102);
        private static readonly Color LightDataTypeColor = Color.FromArgb(0, 128, 128);
        private static readonly Color LightFunctionColor = Color.FromArgb(255, 140, 0);
        private static readonly Color LightNumberColor = Color.FromArgb(128, 0, 128);
        private static readonly Color LightStringColor = Color.FromArgb(0, 100, 0);
        private static readonly Color LightCommentColor = Color.FromArgb(128, 128, 128);
        private static readonly Color LightPreprocessorColor = Color.FromArgb(0, 139, 139);
        private static readonly Color LightOperatorColor = Color.FromArgb(128, 0, 0);
        private static readonly Color LightBracketColor = Color.FromArgb(184, 134, 11);
        private static readonly Color LightVariableColor = Color.FromArgb(139, 0, 0);

        // Dark Theme Colors
        private static readonly Color DarkKeywordColor = Color.FromArgb(198, 120, 221);
        private static readonly Color DarkDataTypeColor = Color.FromArgb(224, 108, 117);
        private static readonly Color DarkFunctionColor = Color.FromArgb(97, 175, 254);
        private static readonly Color DarkNumberColor = Color.FromArgb(209, 154, 102);
        private static readonly Color DarkStringColor = Color.FromArgb(152, 195, 121);
        private static readonly Color DarkCommentColor = Color.FromArgb(92, 99, 112);
        private static readonly Color DarkPreprocessorColor = Color.FromArgb(86, 182, 194);
        private static readonly Color DarkOperatorColor = Color.FromArgb(213, 94, 0);
        private static readonly Color DarkBracketColor = Color.FromArgb(171, 178, 191);
        private static readonly Color DarkVariableColor = Color.FromArgb(229, 192, 123);

        private void codeEditor_TextChanged(object sender, EventArgs e)
        {
            if (isUndoRedoOperation)
                return;

            if (isApplyingSyntaxHighlighting)
                return;

            string currentText = codeEditor.Text;

            // اگر واقعاً محتوای متن تغییر نکرده، چیزی ذخیره نکن
            if (currentText == lastEditorText)
                return;

            // وضعیت قبلی متن را برای Undo ذخیره کن
            undoStack.Push(lastEditorText);

            // با یک تغییر جدید، Redo باید پاک شود
            redoStack.Clear();

            // متن فعلی را به عنوان آخرین وضعیت ذخیره کن
            lastEditorText = currentText;

            isModified = true;

            ApplySyntaxHighlighting();
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

                List<SyntaxRule> rules = isDarkTheme ? GetDarkSyntaxRules() : GetLightSyntaxRules();

                codeEditor.SuspendLayout();

                codeEditor.SelectAll();
                codeEditor.SelectionColor = defaultColor;

                bool[] colored = new bool[text.Length];

                foreach (SyntaxRule rule in rules)
                {
                    MatchCollection matches = rule.Pattern.Matches(text);

                    foreach (Match match in matches)
                    {
                        bool canColor = true;

                        for (int i = match.Index; i < match.Index + match.Length; i++)
                        {
                            if (colored[i])
                            {
                                canColor = false;
                                break;
                            }
                        }

                        if (!canColor)
                            continue;

                        codeEditor.Select(match.Index, match.Length);

                        codeEditor.SelectionColor = rule.Color;

                        for (int i = match.Index; i < match.Index + match.Length; i++)
                        {
                            colored[i] = true;
                        }
                    }
                }

                codeEditor.Select(selectionStart, selectionLength);

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
                // Multi-line comments - باید قبل از بقیه باشد
                new SyntaxRule(@"/\*[\s\S]*?\*/", LightCommentColor),

                // Single-line comments
                new SyntaxRule(@"//[^\r\n]*", LightCommentColor),

                // Strings
                new SyntaxRule(@"""(?:\\.|[^""\\])*""", LightStringColor),

                // Characters
                new SyntaxRule(@"'(?:\\.|[^'\\])'", LightStringColor),

                // Preprocessor
                new SyntaxRule(@"^[ \t]*#\s*[A-Za-z_]\w*", LightPreprocessorColor),

                // Keywords
                new SyntaxRule(@"\b(class|if|while|for|return|else|switch|case|break|continue|do|default|const)\b", LightKeywordColor),

                // Data types
                new SyntaxRule(@"\b(int|float|double|char|bool|void|string|long|short|unsigned|signed)\b", LightDataTypeColor),

                // Numbers
                new SyntaxRule(@"\b(?:0[xX][0-9a-fA-F]+|(?:\d+(?:\.\d*)?|\.\d+)(?:[eE][+-]?\d+)?)\b", LightNumberColor),

                // Function name - فقط اسم تابع، نه پرانتزها
                new SyntaxRule(@"\b[A-Za-z_]\w*(?=\s*\()", LightFunctionColor),

                // Operators
                new SyntaxRule(@"(?:==|!=|<=|>=|\+\+|--|&&|\|\||->|<<|>>|\+=|-=|\*=|/=|%=|[+\-*/%=<>!&|^~?:])", LightOperatorColor),

                // Brackets
                new SyntaxRule(@"[\{\}\[\]\(\)]", LightBracketColor),

                // Variables / identifiers
                new SyntaxRule(@"\b[A-Za-z_]\w*\b", LightVariableColor)
            };
        }
        private List<SyntaxRule> GetDarkSyntaxRules()
        {
            return new List<SyntaxRule>
            {
                // Multi-line comments
                new SyntaxRule(@"/\*[\s\S]*?\*/", DarkCommentColor),

                // Single-line comments
                new SyntaxRule(@"//[^\r\n]*", DarkCommentColor),

                // Strings
                new SyntaxRule(@"""(?:\\.|[^""\\])*""", DarkStringColor),

                // Characters
                new SyntaxRule(@"'(?:\\.|[^'\\])'", DarkStringColor),

                // Preprocessor
                new SyntaxRule(@"^[ \t]*#\s*[A-Za-z_]\w*", DarkPreprocessorColor),

                // Keywords
                new SyntaxRule(
                    @"\b(class|if|while|for|return|else|switch|case|break|continue|do|default|const)\b", DarkKeywordColor),

                // Data types
                new SyntaxRule(@"\b(int|float|double|char|bool|void|string|long|short|unsigned|signed)\b", DarkDataTypeColor),

                // Numbers
                new SyntaxRule(@"\b(?:0[xX][0-9a-fA-F]+|(?:\d+(?:\.\d*)?|\.\d+)(?:[eE][+-]?\d+)?)\b", DarkNumberColor),

                // Function name
                new SyntaxRule(@"\b[A-Za-z_]\w*(?=\s*\()", DarkFunctionColor),

                // Operators
                new SyntaxRule(@"(?:==|!=|<=|>=|\+\+|--|&&|\|\||->|<<|>>|\+=|-=|\*=|/=|%=|[+\-*/%=<>!&|^~?:])", DarkOperatorColor),

                // Brackets
                new SyntaxRule(@"[\{\}\[\]\(\)]", DarkBracketColor),

                // Variables / identifiers
                new SyntaxRule(@"\b[A-Za-z_]\w*\b", DarkVariableColor)
            };
        }
        private void SyntaxHighlightTimer_Tick(object sender, EventArgs e)
        {
            syntaxHighlightTimer.Stop();

            if (isApplyingSyntaxHighlighting)
                return;

            ApplySyntaxHighlighting();
        }

        // ===============================
        // Thee View
        // ===============================
        private void RefreshTreeView()
        {
            projectTreeView.Nodes.Clear();

            TreeNode rootNode = new TreeNode("All Projects");

            if (recentFiles.Count == 0)
            {
                TreeNode emptyNode = new TreeNode("No projects saved.");

                emptyNode.ForeColor = Color.Gray;

                rootNode.Nodes.Add(emptyNode);
            }
            else
            {
                foreach (RecentFile file in recentFiles)
                {
                    TreeNode fileNode = new TreeNode(file.FileName);

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

                recentFiles = JsonSerializer.Deserialize<List<RecentFile>>(json) ?? new List<RecentFile>();
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
                string json = JsonSerializer.Serialize(recentFiles, new JsonSerializerOptions
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
                file => string.Equals(file.FilePath, fullPath, StringComparison.OrdinalIgnoreCase));

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
                MessageBox.Show($"The requested file was not found.\n\n{recentFile.FilePath}", "File not found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            OpenFile(recentFile.FilePath);
        }

        // ===============================
        // File Handling
        // ===============================
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
                MessageBox.Show($"File saving failed.\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show($"File saving failed.\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void NewFile()
        {
            if (!ConfirmSaveChanges())
                return;

            isUndoRedoOperation = true;

            try
            {
                codeEditor.Clear();

                currentFilePath = null;

                lastEditorText = "";

                undoStack.Clear();
                redoStack.Clear();

                isModified = false;

                UpdateWindowTitle();
            }
            finally
            {
                isUndoRedoOperation = false;
            }

            ApplySyntaxHighlighting();
        }
        private void OpenFile(string filePath)
        {
            try
            {
                isUndoRedoOperation = true;

                codeEditor.Text = File.ReadAllText(filePath);

                lastEditorText = codeEditor.Text;

                undoStack.Clear();
                redoStack.Clear();

                isModified = false;

                currentFilePath = filePath;

                UpdateWindowTitle();
                AddRecentFile(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to open file.\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                isUndoRedoOperation = false;
            }

            ApplySyntaxHighlighting();
        }
        private void UpdateWindowTitle()
        {
            if (string.IsNullOrEmpty(currentFilePath))
            {
                this.Text = isModified ? "KIDE - Untitled *" : "KIDE - Untitled";

                return;
            }

            string fileName = Path.GetFileName(currentFilePath);

            this.Text = isModified ? $"KIDE - {fileName} *" : $"KIDE - {fileName}";
        }
        private bool ConfirmSaveChanges()
        {
            if (!isModified)
                return true;

            DialogResult result = MessageBox.Show("The current file has unsaved changes. Do you want to save them?", "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

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

        // ===============================
        // Run Code
        // ===============================
        private async Task RunCode()
        {
            if (runningProcess != null)
            {
                MessageBox.Show("A program is already running.", "KIDE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string tempDirectory = Path.Combine(Path.GetTempPath(), "KIDE");

                Directory.CreateDirectory(tempDirectory);

                tempSourceFile = Path.Combine(tempDirectory, "main.cpp");

                tempExeFile = Path.Combine(tempDirectory, "main.exe");

                File.WriteAllText(tempSourceFile, codeEditor.Text, Encoding.UTF8);

                if (File.Exists(tempExeFile))
                    File.Delete(tempExeFile);

                errorBox.ReadOnly = true;
                errorBox.Clear();

                errorBox.AppendText("Compiling...\r\n");

                ProcessStartInfo compileInfo =
                    new ProcessStartInfo
                    {
                        FileName = "g++",

                        Arguments =
                            $"\"{tempSourceFile}\" -o \"{tempExeFile}\"",

                        RedirectStandardOutput = true,
                        RedirectStandardError = true,

                        UseShellExecute = false,
                        CreateNoWindow = true,

                        StandardOutputEncoding = Encoding.UTF8,
                        StandardErrorEncoding = Encoding.UTF8
                    };

                using (Process compileProcess = new Process())
                {
                    compileProcess.StartInfo = compileInfo;

                    compileProcess.Start();

                    string compilerOutput = await compileProcess.StandardOutput.ReadToEndAsync();

                    string compilerError = await compileProcess.StandardError.ReadToEndAsync();

                    await compileProcess.WaitForExitAsync();

                    if (compileProcess.ExitCode != 0)
                    {
                        errorBox.Clear();

                        errorBox.AppendText("Compilation failed.\r\n\r\n");

                        if (!string.IsNullOrWhiteSpace(compilerError))
                        {
                            errorBox.AppendText(compilerError);
                        }
                        return;
                    }
                }
                errorBox.Clear();
                await RunExecutable();
            }
            catch (Exception ex)
            {
                errorBox.ReadOnly = true;

                errorBox.Clear();

                errorBox.AppendText("Error:\r\n\r\n" + ex.Message);
            }
        }
        private async Task RunExecutable()
        {
            try
            {
                ProcessStartInfo runInfo =
                    new ProcessStartInfo
                    {
                        FileName = tempExeFile,

                        UseShellExecute = false,

                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,

                        CreateNoWindow = true,

                        StandardOutputEncoding = Encoding.UTF8,
                        StandardErrorEncoding = Encoding.UTF8
                    };

                runningProcess = new Process();
                runningProcess.StartInfo = runInfo;

                runningProcess.Start();

                consoleRunning = true;

                errorBox.ReadOnly = false;

                AppendConsoleText("Program started.\r\n\r\n");

                inputStartPosition = errorBox.TextLength;
                errorBox.SelectionStart = errorBox.TextLength;
                errorBox.SelectionLength = 0;
                errorBox.Focus();

                Task outputTask = ReadStandardOutput();
                Task errorTask = ReadStandardError();

                await runningProcess.WaitForExitAsync();

                await Task.WhenAll(outputTask, errorTask);

                int exitCode = runningProcess.ExitCode;

                consoleRunning = false;

                errorBox.ReadOnly = true;

                errorBox.SelectionStart = errorBox.TextLength;

                errorBox.SelectionLength = 0;

                if (exitCode == 0)
                {
                    AppendConsoleText("\r\n\r\nProgram finished successfully.\r\n");
                }
                else
                {
                    AppendConsoleText($"\r\n\r\nProgram finished with exit code {exitCode}.\r\n");
                }

                runningProcess.Dispose();
                runningProcess = null;
            }
            catch (Exception ex)
            {
                consoleRunning = false;
                errorBox.ReadOnly = true;
                errorBox.AppendText("\r\nExecution error:\r\n" + ex.Message);
                if (runningProcess != null)
                {
                    runningProcess.Dispose();
                    runningProcess = null;
                }
            }
        }
        private async Task SendInputToProgram()
        {
            if (runningProcess == null)
                return;

            try
            {
                string input = "";

                if (errorBox.TextLength > inputStartPosition)
                {
                    input = errorBox.Text.Substring(inputStartPosition);
                }

                // Enter را در Console نمایش بده
                errorBox.AppendText(Environment.NewLine);

                inputStartPosition = errorBox.TextLength;

                // ارسال Input به cin
                await runningProcess.StandardInput.WriteLineAsync(input);
                await runningProcess.StandardInput.FlushAsync();
                errorBox.SelectionStart = errorBox.TextLength;
                errorBox.SelectionLength = 0;
                errorBox.ScrollToCaret();
            }
            catch
            {
                // ممکن است Process درست در همین لحظه تمام شده باشد.
            }
        }
        private void AppendConsoleText(string text)
        {
            if (!consoleRunning)
                return;

            string currentInput = "";

            if (errorBox.TextLength > inputStartPosition)
            {
                currentInput = errorBox.Text.Substring(inputStartPosition);
            }

            // حذف موقت Input فعلی
            if (errorBox.TextLength > inputStartPosition)
            {
                errorBox.Select(inputStartPosition, errorBox.TextLength - inputStartPosition);
                errorBox.SelectedText = "";
            }

            // اضافه کردن خروجی جدید
            errorBox.AppendText(text);

            // تعیین محل جدید Input
            inputStartPosition = errorBox.TextLength;

            // برگرداندن Input قبلی
            if (!string.IsNullOrEmpty(currentInput))
            {
                errorBox.AppendText(currentInput);
            }

            errorBox.SelectionStart = errorBox.TextLength;
            errorBox.SelectionLength = 0;
            errorBox.ScrollToCaret();
        }
        private async Task ReadOutput()
        {
            char[] buffer = new char[256];

            while (runningProcess != null &&
                   !runningProcess.HasExited)
            {
                int count;

                try
                {
                    count = await runningProcess.StandardOutput.ReadAsync(buffer, 0, buffer.Length);
                }
                catch
                {
                    break;
                }

                if (count == 0)
                    break;

                string text = new string(buffer, 0, count);

                if (errorBox.InvokeRequired)
                {
                    errorBox.Invoke(new Action(() =>
                    {
                        AppendOutputText(text);
                    })
                    );
                }
                else
                {
                    AppendOutputText(text);
                }
            }
        }
        private void AppendOutputText(string text)
        {
            errorBox.AppendText(text);

            errorBox.SelectionStart = errorBox.TextLength;

            errorBox.ScrollToCaret();

            inputStartPosition = errorBox.TextLength;
        }
        private async Task ReadError()
        {
            char[] buffer = new char[256];

            while (runningProcess != null &&
                   !runningProcess.HasExited)
            {
                int count;

                try
                {
                    count = await runningProcess.StandardError.ReadAsync(buffer, 0, buffer.Length);
                }
                catch
                {
                    break;
                }

                if (count == 0)
                    break;

                string text = new string(buffer, 0, count);

                if (errorBox.InvokeRequired)
                {
                    errorBox.Invoke(new Action(() =>
                    {
                        AppendErrorText(text);
                    })
                    );
                }
                else
                {
                    AppendErrorText(text);
                }
            }
        }
        private void AppendErrorText(string text)
        {
            errorBox.AppendText(text);

            errorBox.SelectionStart = errorBox.TextLength;

            errorBox.ScrollToCaret();

            inputStartPosition = errorBox.TextLength;
        }

        private void errorBox_SelectionChanged(object sender, EventArgs e)
        {
            if (!consoleRunning)
                return;

            if (errorBox.SelectionStart < inputStartPosition)
            {
                MoveCaretToInputEnd();
            }
        }

        private async Task ReadStandardOutput()
        {
            char[] buffer = new char[256];

            while (true)
            {
                int count;

                try
                {
                    count = await runningProcess.StandardOutput.ReadAsync(buffer, 0, buffer.Length);
                }
                catch
                {
                    break;
                }

                if (count == 0)
                    break;

                string text =new string(buffer, 0, count);

                AppendConsoleTextSafe(text);
            }
        }
        private async Task ReadStandardError()
        {
            char[] buffer = new char[256];

            while (true)
            {
                int count;

                try
                {
                    count = await runningProcess.StandardError.ReadAsync(buffer, 0, buffer.Length);
                }
                catch
                {
                    break;
                }

                if (count == 0)
                    break;

                string text = new string(buffer, 0, count);

                AppendConsoleTextSafe(text);
            }
        }
        private void MoveCaretToInputEnd()
        {
            errorBox.SelectionStart = errorBox.TextLength;
            errorBox.SelectionLength = 0;
            errorBox.ScrollToCaret();
        }
        private void AppendConsoleTextSafe(string text)
        {
            if (errorBox.InvokeRequired)
            {
                errorBox.BeginInvoke(new Action(() =>
                {
                    AppendConsoleText(text);
                })
                );

                return;
            }

            AppendConsoleText(text);
        }

        // ===============================
        // Theme Handling
        // ===============================
        private void ApplyThemeToControls(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (isDarkTheme)
                {
                    control.BackColor = Color.Black;
                    control.ForeColor = Color.White;
                }
                else
                {
                    control.BackColor = Color.White;
                    control.ForeColor = Color.Black;
                }

                if (control.HasChildren)
                {
                    ApplyThemeToControls(control);
                }
            }
        }

        // ===============================
        // Key Down Handling
        // ===============================
        private void codeEditor_KeyDown(object sender, KeyEventArgs e)
        {
            // Ctrl + Z → Undo
            if (e.Control && e.KeyCode == Keys.Z)
            {
                UndoEditor();

                e.Handled = true;
                e.SuppressKeyPress = true;

                return;
            }

            // Ctrl + Y → Redo
            if (e.Control && e.KeyCode == Keys.Y)
            {
                RedoEditor();

                e.Handled = true;
                e.SuppressKeyPress = true;

                return;
            }
            if (e.Control && e.KeyCode == Keys.G)
            {
                e.SuppressKeyPress = true;
                GoToLine();
            }
        }
        private void UndoEditor()
        {
            if (undoStack.Count == 0)
                return;

            string currentText = codeEditor.Text;

            string previousText = undoStack.Pop();

            redoStack.Push(currentText);

            int oldSelectionStart = codeEditor.SelectionStart;
            int oldSelectionLength = codeEditor.SelectionLength;

            isUndoRedoOperation = true;

            try
            {
                codeEditor.Text = previousText;

                lastEditorText = previousText;

                isModified = true;

                // محل Cursor را تا حد ممکن حفظ کن
                int newSelectionStart = Math.Min(oldSelectionStart, codeEditor.TextLength);

                codeEditor.Select(newSelectionStart, 0);
            }
            finally
            {
                isUndoRedoOperation = false;
            }

            ApplySyntaxHighlighting();
        }
        private void RedoEditor()
        {
            if (redoStack.Count == 0)
                return;

            string currentText = codeEditor.Text;

            string nextText = redoStack.Pop();

            undoStack.Push(currentText);

            int oldSelectionStart = codeEditor.SelectionStart;

            isUndoRedoOperation = true;

            try
            {
                codeEditor.Text = nextText;

                lastEditorText = nextText;

                isModified = true;

                int newSelectionStart = Math.Min(oldSelectionStart, codeEditor.TextLength
                );

                codeEditor.Select(newSelectionStart, 0);
            }
            finally
            {
                isUndoRedoOperation = false;
            }

            ApplySyntaxHighlighting();
        }
        private void GoToLine()
        {
            int totalLines = codeEditor.Lines.Length;
            using (GoToLine form = new GoToLine(totalLines))
            {
                if (form.ShowDialog() != DialogResult.OK)
                    return;
                int lineNumber = form.LineNumber;
                int index = codeEditor.GetFirstCharIndexFromLine(lineNumber - 1);
                if (index < 0)
                    return;
                codeEditor.SelectionStart = index;
                codeEditor.SelectionLength = 0;
                codeEditor.Focus();
            }
        }
        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
            {
                SaveFile();
                e.SuppressKeyPress = true;
            }
        }
        private void codeEditor_KeyPress(object sender, KeyPressEventArgs e)
        {
            char openChar = e.KeyChar;
            char closeChar;
            switch (openChar)
            {
                case '"':
                    closeChar = '"';
                    break;

                case '\'':
                    closeChar = '\'';
                    break;

                case '(':
                    closeChar = ')';
                    break;

                case '[':
                    closeChar = ']';
                    break;

                case '{':
                    closeChar = '}';
                    break;

                default:
                    return;
            }
            int cursorPosition = codeEditor.SelectionStart;
            codeEditor.SelectedText = $"{openChar}{closeChar}";
            codeEditor.SelectionStart = cursorPosition + 1;
            e.Handled = true;
        }
        private async void errorBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (!consoleRunning || runningProcess == null)
                return;

            // جلوگیری از رفتن به بخش خروجی قبلی
            if (errorBox.SelectionStart < inputStartPosition)
            {
                MoveCaretToInputEnd();

                e.SuppressKeyPress = true;
                e.Handled = true;

                return;
            }

            // Ctrl + A
            if (e.Control && e.KeyCode == Keys.A)
            {
                errorBox.SelectionStart = inputStartPosition;

                errorBox.SelectionLength = errorBox.TextLength - inputStartPosition;

                e.SuppressKeyPress = true;
                e.Handled = true;

                return;
            }

            // Home
            if (e.KeyCode == Keys.Home)
            {
                errorBox.SelectionStart = inputStartPosition;
                errorBox.SelectionLength = 0;
                e.SuppressKeyPress = true;
                e.Handled = true;
                return;
            }

            // Backspace
            if (e.KeyCode == Keys.Back)
            {
                if (errorBox.SelectionStart <= inputStartPosition && errorBox.SelectionLength == 0)
                {
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    return;
                }

                return;
            }

            // Delete
            if (e.KeyCode == Keys.Delete)
            {
                if (errorBox.SelectionStart < inputStartPosition)
                {
                    MoveCaretToInputEnd();
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    return;
                }

                return;
            }

            // Ctrl + V
            if (e.Control && e.KeyCode == Keys.V)
            {
                if (errorBox.SelectionStart < inputStartPosition)
                {
                    MoveCaretToInputEnd();
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    return;
                }
            }

            // Ctrl + X
            if (e.Control && e.KeyCode == Keys.X)
            {
                if (errorBox.SelectionStart < inputStartPosition)
                {
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    return;
                }
            }

            // Enter
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                await SendInputToProgram();
                return;
            }
        }

        // ===============================
        // Debugging
        // ===============================
        private void RunDebugger()
        {
            CodeDebugger debugger = new CodeDebugger(codeEditor.Text);

            List<CodeError> errors = debugger.Analyze();

            errorBox.Clear();

            foreach (CodeError error in errors)
            {
                errorBox.AppendText(error + Environment.NewLine);
            }
            if (errors.Count == 0)
            {
                errorBox.AppendText("No errors found.");
            }
        }

        // ===============================
        // Form Closing
        // ===============================
        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!ConfirmSaveChanges())
            {
                e.Cancel = true;
            }
        }
    }
}