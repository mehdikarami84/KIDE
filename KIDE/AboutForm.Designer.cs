namespace KIDE
{
    partial class AboutForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            pictureBoxLogo = new PictureBox();
            lblProductName = new Label();
            lblVersion = new Label();
            lblDescription = new Label();
            lblDeveloper = new Label();
            lblProjectInfo = new Label();
            btnClose = new Button();
            lblCopyright = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBoxLogo).BeginInit();
            SuspendLayout();
            // 
            // pictureBoxLogo
            // 
            pictureBoxLogo.Image = (Image)resources.GetObject("pictureBoxLogo.Image");
            pictureBoxLogo.Location = new Point(25, 25);
            pictureBoxLogo.Name = "pictureBoxLogo";
            pictureBoxLogo.Size = new Size(100, 100);
            pictureBoxLogo.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxLogo.TabIndex = 0;
            pictureBoxLogo.TabStop = false;
            // 
            // lblProductName
            // 
            lblProductName.AutoSize = true;
            lblProductName.Font = new Font("Calibri", 25.8000011F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            lblProductName.Location = new Point(131, 25);
            lblProductName.Name = "lblProductName";
            lblProductName.Size = new Size(108, 54);
            lblProductName.TabIndex = 1;
            lblProductName.Text = "KIDE";
            // 
            // lblVersion
            // 
            lblVersion.AutoSize = true;
            lblVersion.Font = new Font("Calibri", 10.8F, FontStyle.Italic, GraphicsUnit.Point, 0);
            lblVersion.ForeColor = SystemColors.GrayText;
            lblVersion.Location = new Point(131, 79);
            lblVersion.Name = "lblVersion";
            lblVersion.Size = new Size(128, 22);
            lblVersion.TabIndex = 2;
            lblVersion.Text = "KIDE Version 1.0";
            // 
            // lblDescription
            // 
            lblDescription.Font = new Font("Microsoft Sans Serif", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblDescription.Location = new Point(25, 130);
            lblDescription.Name = "lblDescription";
            lblDescription.Size = new Size(423, 50);
            lblDescription.TabIndex = 4;
            lblDescription.Text = "KIDE is a lightweight C++ Integrated Development Environment developed as an educational project.";
            // 
            // lblDeveloper
            // 
            lblDeveloper.AutoSize = true;
            lblDeveloper.Font = new Font("Microsoft Sans Serif", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblDeveloper.Location = new Point(25, 190);
            lblDeveloper.Name = "lblDeveloper";
            lblDeveloper.Size = new Size(234, 44);
            lblDeveloper.TabIndex = 5;
            lblDeveloper.Text = "Developed by Mehdi Karami\r\n2005m.karami[at]gmail.com\r\n";
            // 
            // lblProjectInfo
            // 
            lblProjectInfo.AutoSize = true;
            lblProjectInfo.Font = new Font("Microsoft Sans Serif", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblProjectInfo.Location = new Point(25, 240);
            lblProjectInfo.Name = "lblProjectInfo";
            lblProjectInfo.Size = new Size(278, 22);
            lblProjectInfo.TabIndex = 6;
            lblProjectInfo.Text = "Built with C# and Windows Forms";
            // 
            // btnClose
            // 
            btnClose.Location = new Point(328, 290);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(120, 30);
            btnClose.TabIndex = 8;
            btnClose.Text = "Close";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;
            // 
            // lblCopyright
            // 
            lblCopyright.AutoSize = true;
            lblCopyright.Font = new Font("Microsoft Sans Serif", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblCopyright.Location = new Point(25, 270);
            lblCopyright.Name = "lblCopyright";
            lblCopyright.Size = new Size(163, 22);
            lblCopyright.TabIndex = 7;
            lblCopyright.Text = "© 2026 Karami IDE";
            // 
            // AboutForm
            // 
            AutoScaleDimensions = new SizeF(8F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(462, 333);
            Controls.Add(pictureBoxLogo);
            Controls.Add(lblProductName);
            Controls.Add(btnClose);
            Controls.Add(lblCopyright);
            Controls.Add(lblProjectInfo);
            Controls.Add(lblDeveloper);
            Controls.Add(lblDescription);
            Controls.Add(lblVersion);
            Font = new Font("Microsoft Sans Serif", 8.25F);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AboutForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "About KIDE";
            ((System.ComponentModel.ISupportInitialize)pictureBoxLogo).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBoxLogo;
        private Label lblProductName;
        private Label lblVersion;
        private Label lblDescription;
        private Label lblDeveloper;
        private Label lblProjectInfo;
        private Button btnClose;
        private Label lblCopyright;
    }
}