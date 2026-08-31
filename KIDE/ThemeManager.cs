using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace KIDE
{
    internal class ThemeManager
    {
        public enum Theme
        {
            Light,
            Dark
        }

        public static Theme CurrentTheme { get; private set; } = Theme.Light;

        // =========================
        // Main colors
        // =========================

        public static Color BackgroundColor =>
            CurrentTheme == Theme.Dark
                ? Color.FromArgb(30, 30, 30)
                : Color.FromArgb(250, 250, 250);

        public static Color ForegroundColor =>
            CurrentTheme == Theme.Dark
                ? Color.White
                : Color.Black;

        public static Color PanelColor =>
            CurrentTheme == Theme.Dark
                ? Color.FromArgb(45, 45, 48)
                : Color.FromArgb(240, 240, 240);

        public static Color ButtonColor =>
            CurrentTheme == Theme.Dark
                ? Color.FromArgb(55, 55, 58)
                : Color.FromArgb(230, 230, 230);

        public static Color ButtonHoverColor =>
            CurrentTheme == Theme.Dark
                ? Color.FromArgb(70, 70, 75)
                : Color.FromArgb(215, 215, 215);

        public static Color BorderColor =>
            CurrentTheme == Theme.Dark
                ? Color.FromArgb(80, 80, 80)
                : Color.FromArgb(200, 200, 200);

        public static Color InputColor =>
            CurrentTheme == Theme.Dark
                ? Color.FromArgb(37, 37, 38)
                : Color.White;

        public static Color InputTextColor =>
            CurrentTheme == Theme.Dark
                ? Color.White
                : Color.Black;

        public static Color LinkColor =>
            CurrentTheme == Theme.Dark
                ? Color.DeepSkyBlue
                : Color.Blue;


        // =========================
        // Theme selection
        // =========================

        public static void SetLightTheme()
        {
            CurrentTheme = Theme.Light;

            ApplyThemeToAllForms();
        }

        public static void SetDarkTheme()
        {
            CurrentTheme = Theme.Dark;

            ApplyThemeToAllForms();
        }


        // =========================
        // Apply to all open forms
        // =========================

        public static void ApplyThemeToAllForms()
        {
            foreach (Form form in Application.OpenForms)
            {
                ApplyTheme(form);
            }
        }


        // =========================
        // Apply to a form
        // =========================

        public static void ApplyTheme(Control control)
        {
            ApplyThemeToControl(control);
        }


        // =========================
        // Apply to controls
        // =========================

        private static void ApplyThemeToControl(Control control)
        {
            // -------------------------
            // Form
            // -------------------------

            if (control is Form)
            {
                control.BackColor = BackgroundColor;
                control.ForeColor = ForegroundColor;
            }

            // -------------------------
            // Panel
            // -------------------------

            else if (control is Panel)
            {
                control.BackColor = PanelColor;
                control.ForeColor = ForegroundColor;
            }

            // -------------------------
            // GroupBox
            // -------------------------

            else if (control is GroupBox)
            {
                control.BackColor = PanelColor;
                control.ForeColor = ForegroundColor;
            }

            // -------------------------
            // Button
            // -------------------------

            else if (control is Button button)
            {
                button.BackColor = ButtonColor;
                button.ForeColor = ForegroundColor;
                button.FlatAppearance.BorderColor = BorderColor;
            }

            // -------------------------
            // Label
            // -------------------------

            else if (control is Label label)
            {
                label.BackColor = Color.Transparent;
                label.ForeColor = ForegroundColor;
            }

            // -------------------------
            // TextBox
            // -------------------------

            else if (control is TextBox textBox)
            {
                textBox.BackColor = InputColor;
                textBox.ForeColor = InputTextColor;
            }

            // -------------------------
            // RichTextBox
            // -------------------------

            else if (control is RichTextBox richTextBox)
            {
                richTextBox.BackColor = InputColor;
                richTextBox.ForeColor = InputTextColor;
            }

            // -------------------------
            // LinkLabel
            // -------------------------

            else if (control is LinkLabel linkLabel)
            {
                linkLabel.BackColor = Color.Transparent;
                linkLabel.ForeColor = LinkColor;
                linkLabel.LinkColor = LinkColor;
                linkLabel.ActiveLinkColor = LinkColor;
            }

            // -------------------------
            // CheckBox
            // -------------------------

            else if (control is CheckBox checkBox)
            {
                checkBox.BackColor = BackgroundColor;
                checkBox.ForeColor = ForegroundColor;
            }

            // -------------------------
            // RadioButton
            // -------------------------

            else if (control is RadioButton radioButton)
            {
                radioButton.BackColor = BackgroundColor;
                radioButton.ForeColor = ForegroundColor;
            }

            // -------------------------
            // ComboBox
            // -------------------------

            else if (control is ComboBox comboBox)
            {
                comboBox.BackColor = InputColor;
                comboBox.ForeColor = InputTextColor;
            }

            // -------------------------
            // ListBox
            // -------------------------

            else if (control is ListBox listBox)
            {
                listBox.BackColor = InputColor;
                listBox.ForeColor = InputTextColor;
            }

            // -------------------------
            // TreeView
            // -------------------------

            else if (control is TreeView treeView)
            {
                treeView.BackColor = InputColor;
                treeView.ForeColor = InputTextColor;
            }

            // -------------------------
            // ListView
            // -------------------------

            else if (control is ListView listView)
            {
                listView.BackColor = InputColor;
                listView.ForeColor = InputTextColor;
            }

            // -------------------------
            // TabControl
            // -------------------------

            else if (control is TabControl tabControl)
            {
                tabControl.BackColor = PanelColor;
                tabControl.ForeColor = ForegroundColor;
            }

            // -------------------------
            // StatusStrip
            // -------------------------

            else if (control is StatusStrip statusStrip)
            {
                statusStrip.BackColor = PanelColor;
                statusStrip.ForeColor = ForegroundColor;
            }

            // -------------------------
            // MenuStrip
            // -------------------------

            else if (control is MenuStrip menuStrip)
            {
                ApplyMenuStripTheme(menuStrip);
            }

            // -------------------------
            // ToolStrip
            // -------------------------

            else if (control is ToolStrip toolStrip)
            {
                toolStrip.BackColor = PanelColor;
                toolStrip.ForeColor = ForegroundColor;

                foreach (ToolStripItem item in toolStrip.Items)
                {
                    ApplyToolStripItemTheme(item);
                }
            }


            // =========================
            // Children
            // =========================

            foreach (Control child in control.Controls)
            {
                ApplyThemeToControl(child);
            }
        }


        // =========================
        // MenuStrip
        // =========================

        private static void ApplyMenuStripTheme(MenuStrip menuStrip)
        {
            menuStrip.BackColor = PanelColor;
            menuStrip.ForeColor = ForegroundColor;

            foreach (ToolStripItem item in menuStrip.Items)
            {
                ApplyToolStripItemTheme(item);
            }
        }


        // =========================
        // ToolStrip items
        // =========================

        private static void ApplyToolStripItemTheme(ToolStripItem item)
        {
            item.BackColor = PanelColor;
            item.ForeColor = ForegroundColor;

            if (item is ToolStripMenuItem menuItem)
            {
                foreach (ToolStripItem child in menuItem.DropDownItems)
                {
                    ApplyToolStripItemTheme(child);
                }
            }
        }
    }
}
