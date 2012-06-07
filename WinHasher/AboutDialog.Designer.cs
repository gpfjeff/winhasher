namespace com.gpfcomics.WinHasher
{
    partial class AboutDialog
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutDialog));
            this.versionLabel = new System.Windows.Forms.Label();
            this.copyrightLabel = new System.Windows.Forms.Label();
            this.linkLabel = new System.Windows.Forms.LinkLabel();
            this.gplTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.helpButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // versionLabel
            // 
            this.versionLabel.Location = new System.Drawing.Point(12, 9);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(340, 23);
            this.versionLabel.TabIndex = 0;
            this.versionLabel.Text = "WinHasher #.#.#.#";
            this.versionLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.toolTip1.SetToolTip(this.versionLabel, "The full version number for\r\nthis version of WinHasher");
            // 
            // copyrightLabel
            // 
            this.copyrightLabel.Location = new System.Drawing.Point(12, 32);
            this.copyrightLabel.Name = "copyrightLabel";
            this.copyrightLabel.Size = new System.Drawing.Size(340, 23);
            this.copyrightLabel.TabIndex = 1;
            this.copyrightLabel.Text = "© Copyright ####, Jeffrey T. Darlington.";
            this.copyrightLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.toolTip1.SetToolTip(this.copyrightLabel, "The person to blame\r\nfor this mess.");
            // 
            // linkLabel
            // 
            this.linkLabel.Location = new System.Drawing.Point(12, 55);
            this.linkLabel.Name = "linkLabel";
            this.linkLabel.Size = new System.Drawing.Size(340, 23);
            this.linkLabel.TabIndex = 2;
            this.linkLabel.TabStop = true;
            this.linkLabel.Text = "https://code.google.com/p/winhasher/";
            this.linkLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.toolTip1.SetToolTip(this.linkLabel, "Click this link to find\r\nWinHasher online.");
            this.linkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // gplTextBox
            // 
            this.gplTextBox.Location = new System.Drawing.Point(12, 101);
            this.gplTextBox.Multiline = true;
            this.gplTextBox.Name = "gplTextBox";
            this.gplTextBox.ReadOnly = true;
            this.gplTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.gplTextBox.Size = new System.Drawing.Size(340, 125);
            this.gplTextBox.TabIndex = 3;
            this.toolTip1.SetToolTip(this.gplTextBox, "This license under which WinHasher has been\r\nreleased.  This describes what you c" +
        "an and\r\ncannot do with this program.");
            this.gplTextBox.WordWrap = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 85);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "License:";
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.okButton.Location = new System.Drawing.Point(185, 238);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 5;
            this.okButton.Text = "OK";
            this.toolTip1.SetToolTip(this.okButton, "Click this button to go back to\r\nusing WinHasher.  What fun!");
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // helpButton
            // 
            this.helpButton.Location = new System.Drawing.Point(104, 238);
            this.helpButton.Name = "helpButton";
            this.helpButton.Size = new System.Drawing.Size(75, 23);
            this.helpButton.TabIndex = 6;
            this.helpButton.Text = "Help...";
            this.toolTip1.SetToolTip(this.helpButton, "Click this button to launch the HTML file\r\nif your default Web browser");
            this.helpButton.UseVisualStyleBackColor = true;
            this.helpButton.Click += new System.EventHandler(this.helpButton_Click);
            // 
            // AboutDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.okButton;
            this.ClientSize = new System.Drawing.Size(364, 273);
            this.Controls.Add(this.helpButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.gplTextBox);
            this.Controls.Add(this.linkLabel);
            this.Controls.Add(this.copyrightLabel);
            this.Controls.Add(this.versionLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About WinHasher";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label versionLabel;
        private System.Windows.Forms.Label copyrightLabel;
        private System.Windows.Forms.LinkLabel linkLabel;
        private System.Windows.Forms.TextBox gplTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button helpButton;
    }
}