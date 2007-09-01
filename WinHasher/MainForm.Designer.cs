namespace com.gpfcomics.WinHasher
{
    partial class MainForm
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
            this.modeTabControl = new System.Windows.Forms.TabControl();
            this.singleTabPage = new System.Windows.Forms.TabPage();
            this.compareTabPage = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.hashComboBox = new System.Windows.Forms.ComboBox();
            this.closeButton = new System.Windows.Forms.Button();
            this.aboutButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.fileSingleTextBox = new System.Windows.Forms.TextBox();
            this.browseSingleButton = new System.Windows.Forms.Button();
            this.hashSingleButton = new System.Windows.Forms.Button();
            this.hashSingleTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.compareFilesListBox = new System.Windows.Forms.ListBox();
            this.addButton = new System.Windows.Forms.Button();
            this.removeButton = new System.Windows.Forms.Button();
            this.compareButton = new System.Windows.Forms.Button();
            this.clearButton = new System.Windows.Forms.Button();
            this.modeTabControl.SuspendLayout();
            this.singleTabPage.SuspendLayout();
            this.compareTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // modeTabControl
            // 
            this.modeTabControl.Controls.Add(this.singleTabPage);
            this.modeTabControl.Controls.Add(this.compareTabPage);
            this.modeTabControl.Location = new System.Drawing.Point(3, 3);
            this.modeTabControl.Name = "modeTabControl";
            this.modeTabControl.SelectedIndex = 0;
            this.modeTabControl.Size = new System.Drawing.Size(284, 228);
            this.modeTabControl.TabIndex = 0;
            this.modeTabControl.SelectedIndexChanged += new System.EventHandler(this.modeTabControl_SelectedIndexChanged);
            // 
            // singleTabPage
            // 
            this.singleTabPage.Controls.Add(this.label3);
            this.singleTabPage.Controls.Add(this.hashSingleTextBox);
            this.singleTabPage.Controls.Add(this.hashSingleButton);
            this.singleTabPage.Controls.Add(this.browseSingleButton);
            this.singleTabPage.Controls.Add(this.fileSingleTextBox);
            this.singleTabPage.Controls.Add(this.label2);
            this.singleTabPage.Location = new System.Drawing.Point(4, 22);
            this.singleTabPage.Name = "singleTabPage";
            this.singleTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.singleTabPage.Size = new System.Drawing.Size(276, 202);
            this.singleTabPage.TabIndex = 0;
            this.singleTabPage.Text = "Hash Single File";
            this.singleTabPage.UseVisualStyleBackColor = true;
            // 
            // compareTabPage
            // 
            this.compareTabPage.Controls.Add(this.clearButton);
            this.compareTabPage.Controls.Add(this.compareButton);
            this.compareTabPage.Controls.Add(this.removeButton);
            this.compareTabPage.Controls.Add(this.addButton);
            this.compareTabPage.Controls.Add(this.compareFilesListBox);
            this.compareTabPage.Controls.Add(this.label4);
            this.compareTabPage.Location = new System.Drawing.Point(4, 22);
            this.compareTabPage.Name = "compareTabPage";
            this.compareTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.compareTabPage.Size = new System.Drawing.Size(276, 202);
            this.compareTabPage.TabIndex = 1;
            this.compareTabPage.Text = "Compare Files";
            this.compareTabPage.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 243);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Hash:";
            // 
            // hashComboBox
            // 
            this.hashComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.hashComboBox.FormattingEnabled = true;
            this.hashComboBox.Location = new System.Drawing.Point(36, 240);
            this.hashComboBox.Name = "hashComboBox";
            this.hashComboBox.Size = new System.Drawing.Size(71, 21);
            this.hashComboBox.TabIndex = 2;
            this.hashComboBox.SelectedIndexChanged += new System.EventHandler(this.hashComboBox_SelectedIndexChanged);
            // 
            // closeButton
            // 
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.Location = new System.Drawing.Point(212, 238);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 4;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // aboutButton
            // 
            this.aboutButton.Location = new System.Drawing.Point(123, 238);
            this.aboutButton.Name = "aboutButton";
            this.aboutButton.Size = new System.Drawing.Size(75, 23);
            this.aboutButton.TabIndex = 3;
            this.aboutButton.Text = "About...";
            this.aboutButton.UseVisualStyleBackColor = true;
            this.aboutButton.Click += new System.EventHandler(this.aboutButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "File to hash:";
            // 
            // fileSingleTextBox
            // 
            this.fileSingleTextBox.AllowDrop = true;
            this.fileSingleTextBox.Location = new System.Drawing.Point(6, 19);
            this.fileSingleTextBox.Name = "fileSingleTextBox";
            this.fileSingleTextBox.Size = new System.Drawing.Size(264, 20);
            this.fileSingleTextBox.TabIndex = 1;
            this.fileSingleTextBox.TextChanged += new System.EventHandler(this.fileSingleTextBox_TextChanged);
            // 
            // browseSingleButton
            // 
            this.browseSingleButton.Location = new System.Drawing.Point(9, 45);
            this.browseSingleButton.Name = "browseSingleButton";
            this.browseSingleButton.Size = new System.Drawing.Size(75, 23);
            this.browseSingleButton.TabIndex = 2;
            this.browseSingleButton.Text = "Browse...";
            this.browseSingleButton.UseVisualStyleBackColor = true;
            this.browseSingleButton.Click += new System.EventHandler(this.browseSingleButton_Click);
            // 
            // hashSingleButton
            // 
            this.hashSingleButton.Enabled = false;
            this.hashSingleButton.Location = new System.Drawing.Point(176, 45);
            this.hashSingleButton.Name = "hashSingleButton";
            this.hashSingleButton.Size = new System.Drawing.Size(94, 23);
            this.hashSingleButton.TabIndex = 3;
            this.hashSingleButton.Text = "Compute Hash";
            this.hashSingleButton.UseVisualStyleBackColor = true;
            this.hashSingleButton.Click += new System.EventHandler(this.hashSingleButton_Click);
            // 
            // hashSingleTextBox
            // 
            this.hashSingleTextBox.Location = new System.Drawing.Point(6, 94);
            this.hashSingleTextBox.Multiline = true;
            this.hashSingleTextBox.Name = "hashSingleTextBox";
            this.hashSingleTextBox.ReadOnly = true;
            this.hashSingleTextBox.Size = new System.Drawing.Size(264, 52);
            this.hashSingleTextBox.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 78);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Hash:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Files to compare:";
            // 
            // compareFilesListBox
            // 
            this.compareFilesListBox.AllowDrop = true;
            this.compareFilesListBox.FormattingEnabled = true;
            this.compareFilesListBox.HorizontalScrollbar = true;
            this.compareFilesListBox.Location = new System.Drawing.Point(8, 19);
            this.compareFilesListBox.Name = "compareFilesListBox";
            this.compareFilesListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.compareFilesListBox.Size = new System.Drawing.Size(262, 121);
            this.compareFilesListBox.TabIndex = 1;
            this.compareFilesListBox.SelectedIndexChanged += new System.EventHandler(this.compareFilesListBox_SelectedIndexChanged);
            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(8, 146);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(75, 23);
            this.addButton.TabIndex = 2;
            this.addButton.Text = "Add...";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // removeButton
            // 
            this.removeButton.Enabled = false;
            this.removeButton.Location = new System.Drawing.Point(195, 146);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(75, 23);
            this.removeButton.TabIndex = 4;
            this.removeButton.Text = "Remove";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // compareButton
            // 
            this.compareButton.Enabled = false;
            this.compareButton.Location = new System.Drawing.Point(72, 175);
            this.compareButton.Name = "compareButton";
            this.compareButton.Size = new System.Drawing.Size(132, 23);
            this.compareButton.TabIndex = 5;
            this.compareButton.Text = "Compare Hashes";
            this.compareButton.UseVisualStyleBackColor = true;
            this.compareButton.Click += new System.EventHandler(this.compareButton_Click);
            // 
            // clearButton
            // 
            this.clearButton.Enabled = false;
            this.clearButton.Location = new System.Drawing.Point(101, 146);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(75, 23);
            this.clearButton.TabIndex = 3;
            this.clearButton.Text = "Clear List";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.closeButton;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Controls.Add(this.aboutButton);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.hashComboBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.modeTabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "WinHasher";
            this.modeTabControl.ResumeLayout(false);
            this.singleTabPage.ResumeLayout(false);
            this.singleTabPage.PerformLayout();
            this.compareTabPage.ResumeLayout(false);
            this.compareTabPage.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl modeTabControl;
        private System.Windows.Forms.TabPage singleTabPage;
        private System.Windows.Forms.TabPage compareTabPage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox hashComboBox;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button aboutButton;
        private System.Windows.Forms.Button hashSingleButton;
        private System.Windows.Forms.Button browseSingleButton;
        private System.Windows.Forms.TextBox fileSingleTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox hashSingleTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox compareFilesListBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button compareButton;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button clearButton;
    }
}

