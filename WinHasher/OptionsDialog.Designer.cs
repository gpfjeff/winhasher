namespace com.gpfcomics.WinHasher
{
    partial class OptionsDialog
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
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.chkDisableUpdateCheck = new System.Windows.Forms.CheckBox();
            this.listSentToShortcuts = new System.Windows.Forms.ListBox();
            this.lblSendToShortcuts = new System.Windows.Forms.Label();
            this.comboOutputTypes = new System.Windows.Forms.ComboBox();
            this.lblOutputType = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(68, 285);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(149, 285);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // chkDisableUpdateCheck
            // 
            this.chkDisableUpdateCheck.AutoSize = true;
            this.chkDisableUpdateCheck.Location = new System.Drawing.Point(12, 12);
            this.chkDisableUpdateCheck.Name = "chkDisableUpdateCheck";
            this.chkDisableUpdateCheck.Size = new System.Drawing.Size(184, 17);
            this.chkDisableUpdateCheck.TabIndex = 2;
            this.chkDisableUpdateCheck.Text = "Disable automatic update checks";
            this.chkDisableUpdateCheck.UseVisualStyleBackColor = true;
            // 
            // listSentToShortcuts
            // 
            this.listSentToShortcuts.FormattingEnabled = true;
            this.listSentToShortcuts.Location = new System.Drawing.Point(12, 87);
            this.listSentToShortcuts.Name = "listSentToShortcuts";
            this.listSentToShortcuts.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.listSentToShortcuts.Size = new System.Drawing.Size(268, 134);
            this.listSentToShortcuts.TabIndex = 3;
            // 
            // lblSendToShortcuts
            // 
            this.lblSendToShortcuts.Location = new System.Drawing.Point(12, 32);
            this.lblSendToShortcuts.Name = "lblSendToShortcuts";
            this.lblSendToShortcuts.Size = new System.Drawing.Size(268, 52);
            this.lblSendToShortcuts.TabIndex = 4;
            this.lblSendToShortcuts.Text = "Add shortcuts for the following hashes in your Send To folder.  This allows you t" +
    "o right-click a single file to instantly get the hash of that file, or right-cli" +
    "ck multiple files to compare them.";
            // 
            // comboOutputTypes
            // 
            this.comboOutputTypes.FormattingEnabled = true;
            this.comboOutputTypes.Location = new System.Drawing.Point(15, 246);
            this.comboOutputTypes.Name = "comboOutputTypes";
            this.comboOutputTypes.Size = new System.Drawing.Size(116, 21);
            this.comboOutputTypes.TabIndex = 5;
            // 
            // lblOutputType
            // 
            this.lblOutputType.AutoSize = true;
            this.lblOutputType.Location = new System.Drawing.Point(12, 230);
            this.lblOutputType.Name = "lblOutputType";
            this.lblOutputType.Size = new System.Drawing.Size(170, 13);
            this.lblOutputType.TabIndex = 6;
            this.lblOutputType.Text = "Output type for Send To shortcuts:";
            // 
            // OptionsDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(292, 320);
            this.Controls.Add(this.lblOutputType);
            this.Controls.Add(this.comboOutputTypes);
            this.Controls.Add(this.lblSendToShortcuts);
            this.Controls.Add(this.listSentToShortcuts);
            this.Controls.Add(this.chkDisableUpdateCheck);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionsDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "WinHasher Options";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.CheckBox chkDisableUpdateCheck;
        private System.Windows.Forms.ListBox listSentToShortcuts;
        private System.Windows.Forms.Label lblSendToShortcuts;
        private System.Windows.Forms.ComboBox comboOutputTypes;
        private System.Windows.Forms.Label lblOutputType;
    }
}