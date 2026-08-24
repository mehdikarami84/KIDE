namespace KIDE
{
    partial class GoToLine
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GoToLine));
            lineLabel = new Label();
            lineNumberInput = new NumericUpDown();
            okButton = new Button();
            cancelButton = new Button();
            ((System.ComponentModel.ISupportInitialize)lineNumberInput).BeginInit();
            SuspendLayout();
            // 
            // lineLabel
            // 
            lineLabel.AutoSize = true;
            lineLabel.Location = new Point(12, 9);
            lineLabel.Name = "lineLabel";
            lineLabel.Size = new Size(97, 20);
            lineLabel.TabIndex = 0;
            lineLabel.Text = "Line Number:";
            // 
            // lineNumberInput
            // 
            lineNumberInput.Location = new Point(12, 32);
            lineNumberInput.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            lineNumberInput.Name = "lineNumberInput";
            lineNumberInput.Size = new Size(286, 27);
            lineNumberInput.TabIndex = 1;
            // 
            // okButton
            // 
            okButton.Location = new Point(12, 65);
            okButton.Name = "okButton";
            okButton.Size = new Size(140, 30);
            okButton.TabIndex = 2;
            okButton.Text = "OK";
            okButton.UseVisualStyleBackColor = true;
            okButton.Click += okButton_Click;
            // 
            // cancelButton
            // 
            cancelButton.Location = new Point(158, 65);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(140, 30);
            cancelButton.TabIndex = 3;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += cancelButton_Click;
            // 
            // GoToLine
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(312, 110);
            Controls.Add(cancelButton);
            Controls.Add(okButton);
            Controls.Add(lineNumberInput);
            Controls.Add(lineLabel);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "GoToLine";
            Text = "Go To Line";
            ((System.ComponentModel.ISupportInitialize)lineNumberInput).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lineLabel;
        private NumericUpDown lineNumberInput;
        private Button okButton;
        private Button cancelButton;
    }
}