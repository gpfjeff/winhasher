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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsDialog));
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.chkDisableUpdateCheck = new System.Windows.Forms.CheckBox();
            this.lblSendToShortcuts = new System.Windows.Forms.Label();
            this.comboOutputTypes = new System.Windows.Forms.ComboBox();
            this.lblOutputType = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.listSentToShortcuts = new System.Windows.Forms.CheckedListBox();
            this.lblUpdateCheck = new System.Windows.Forms.Label();
            this.btnCheckForUpdates = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(68, 333);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 7;
            this.okButton.Text = "OK";
            this.toolTip1.SetToolTip(this.okButton, "Click here to save all your settings\r\nspecified above. Any Send To shortcuts\r\nwil" +
        "l be created or deleted, depending on\r\nthe state of their checkboxes.  You will\r" +
        "\nthen be returned to the main window.");
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(149, 333);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 8;
            this.cancelButton.Text = "Cancel";
            this.toolTip1.SetToolTip(this.cancelButton, "Click this button to close this dialog\r\nwithout saving any changed settings.\r\nYou" +
        " will then be returned to the\r\nmain window.");
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // chkDisableUpdateCheck
            // 
            this.chkDisableUpdateCheck.AutoSize = true;
            this.chkDisableUpdateCheck.Location = new System.Drawing.Point(12, 12);
            this.chkDisableUpdateCheck.Name = "chkDisableUpdateCheck";
            this.chkDisableUpdateCheck.Size = new System.Drawing.Size(184, 17);
            this.chkDisableUpdateCheck.TabIndex = 0;
            this.chkDisableUpdateCheck.Text = "Disable automatic update checks";
            this.toolTip1.SetToolTip(this.chkDisableUpdateCheck, "Check this box to disable automatic\r\nchecks for updates.  We strongly\r\nrecommend " +
        "leaving this option\r\nenabled.  If disabled, you will need\r\nto manually check for" +
        " updates\r\nperiodically.");
            this.chkDisableUpdateCheck.UseVisualStyleBackColor = true;
            // 
            // lblSendToShortcuts
            // 
            this.lblSendToShortcuts.Location = new System.Drawing.Point(9, 87);
            this.lblSendToShortcuts.Name = "lblSendToShortcuts";
            this.lblSendToShortcuts.Size = new System.Drawing.Size(268, 52);
            this.lblSendToShortcuts.TabIndex = 3;
            this.lblSendToShortcuts.Text = "Add shortcuts for the following hashes in your Send To folder.  This allows you t" +
    "o right-click a single file to instantly get the hash of that file, or right-cli" +
    "ck multiple files to compare them.";
            // 
            // comboOutputTypes
            // 
            this.comboOutputTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboOutputTypes.FormattingEnabled = true;
            this.comboOutputTypes.Location = new System.Drawing.Point(12, 301);
            this.comboOutputTypes.Name = "comboOutputTypes";
            this.comboOutputTypes.Size = new System.Drawing.Size(116, 21);
            this.comboOutputTypes.TabIndex = 6;
            this.toolTip1.SetToolTip(this.comboOutputTypes, "Select the output type for all Send To\r\nshortcuts.  In most cases, you should\r\nle" +
        "ave this at the default (lower-case\r\nhexadecimal), since that is the de facto\r\ns" +
        "tandard for hashes on most websites.");
            // 
            // lblOutputType
            // 
            this.lblOutputType.AutoSize = true;
            this.lblOutputType.Location = new System.Drawing.Point(9, 285);
            this.lblOutputType.Name = "lblOutputType";
            this.lblOutputType.Size = new System.Drawing.Size(170, 13);
            this.lblOutputType.TabIndex = 5;
            this.lblOutputType.Text = "Output type for Send To shortcuts:";
            // 
            // listSentToShortcuts
            // 
            this.listSentToShortcuts.FormattingEnabled = true;
            this.listSentToShortcuts.Location = new System.Drawing.Point(12, 142);
            this.listSentToShortcuts.Name = "listSentToShortcuts";
            this.listSentToShortcuts.Size = new System.Drawing.Size(265, 139);
            this.listSentToShortcuts.TabIndex = 4;
            this.toolTip1.SetToolTip(this.listSentToShortcuts, resources.GetString("listSentToShortcuts.ToolTip"));
            // 
            // lblUpdateCheck
            // 
            this.lblUpdateCheck.AutoSize = true;
            this.lblUpdateCheck.Location = new System.Drawing.Point(12, 36);
            this.lblUpdateCheck.Name = "lblUpdateCheck";
            this.lblUpdateCheck.Size = new System.Drawing.Size(134, 13);
            this.lblUpdateCheck.TabIndex = 1;
            this.lblUpdateCheck.Text = "Last update check:  Never";
            // 
            // btnCheckForUpdates
            // 
            this.btnCheckForUpdates.Location = new System.Drawing.Point(54, 52);
            this.btnCheckForUpdates.Name = "btnCheckForUpdates";
            this.btnCheckForUpdates.Size = new System.Drawing.Size(184, 23);
            this.btnCheckForUpdates.TabIndex = 2;
            this.btnCheckForUpdates.Text = "Check for Updates Now...";
            this.toolTip1.SetToolTip(this.btnCheckForUpdates, "Click this button to manually check\r\nfor WinHasher updates.  If a newer\r\nversion " +
        "is found, you\'ll be prompted\r\nto download it.  Obviously, this\r\nrequires an acti" +
        "ve Internet connection.");
            this.btnCheckForUpdates.UseVisualStyleBackColor = true;
            this.btnCheckForUpdates.Click += new System.EventHandler(this.btnCheckForUpdates_Click);
            // 
            // OptionsDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(292, 364);
            this.Controls.Add(this.btnCheckForUpdates);
            this.Controls.Add(this.lblUpdateCheck);
            this.Controls.Add(this.listSentToShortcuts);
            this.Controls.Add(this.lblOutputType);
            this.Controls.Add(this.comboOutputTypes);
            this.Controls.Add(this.lblSendToShortcuts);
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
        private System.Windows.Forms.Label lblSendToShortcuts;
        private System.Windows.Forms.ComboBox comboOutputTypes;
        private System.Windows.Forms.Label lblOutputType;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckedListBox listSentToShortcuts;
        private System.Windows.Forms.Label lblUpdateCheck;
        private System.Windows.Forms.Button btnCheckForUpdates;
    }
}