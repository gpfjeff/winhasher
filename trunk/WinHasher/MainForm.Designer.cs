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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.modeTabControl = new System.Windows.Forms.TabControl();
            this.singleTabPage = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.hashSingleTextBox = new System.Windows.Forms.TextBox();
            this.hashSingleButton = new System.Windows.Forms.Button();
            this.browseSingleButton = new System.Windows.Forms.Button();
            this.fileSingleTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.compareTabPage = new System.Windows.Forms.TabPage();
            this.clearButton = new System.Windows.Forms.Button();
            this.compareButton = new System.Windows.Forms.Button();
            this.removeButton = new System.Windows.Forms.Button();
            this.addButton = new System.Windows.Forms.Button();
            this.compareFilesListBox = new System.Windows.Forms.ListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textTabPage = new System.Windows.Forms.TabPage();
            this.outputTextBox = new System.Windows.Forms.TextBox();
            this.hashTextButton = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.encodingComboBox = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.inputTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.hashComboBox = new System.Windows.Forms.ComboBox();
            this.closeButton = new System.Windows.Forms.Button();
            this.aboutButton = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label7 = new System.Windows.Forms.Label();
            this.outputFormatComboBox = new System.Windows.Forms.ComboBox();
            this.modeTabControl.SuspendLayout();
            this.singleTabPage.SuspendLayout();
            this.compareTabPage.SuspendLayout();
            this.textTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // modeTabControl
            // 
            this.modeTabControl.Controls.Add(this.singleTabPage);
            this.modeTabControl.Controls.Add(this.compareTabPage);
            this.modeTabControl.Controls.Add(this.textTabPage);
            this.modeTabControl.Location = new System.Drawing.Point(3, 3);
            this.modeTabControl.Name = "modeTabControl";
            this.modeTabControl.SelectedIndex = 0;
            this.modeTabControl.Size = new System.Drawing.Size(284, 228);
            this.modeTabControl.TabIndex = 0;
            this.modeTabControl.SelectedIndexChanged += new System.EventHandler(this.modeTabControl_SelectedIndexChanged);
            // 
            // singleTabPage
            // 
            this.singleTabPage.AllowDrop = true;
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
            this.toolTip1.SetToolTip(this.singleTabPage, resources.GetString("singleTabPage.ToolTip"));
            this.singleTabPage.UseVisualStyleBackColor = true;
            this.singleTabPage.DragDrop += new System.Windows.Forms.DragEventHandler(this.fileSingleTextBox_DragDrop);
            this.singleTabPage.DragEnter += new System.Windows.Forms.DragEventHandler(this.fileSingleTextBox_DragEnter);
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
            // hashSingleTextBox
            // 
            this.hashSingleTextBox.Location = new System.Drawing.Point(6, 94);
            this.hashSingleTextBox.Multiline = true;
            this.hashSingleTextBox.Name = "hashSingleTextBox";
            this.hashSingleTextBox.ReadOnly = true;
            this.hashSingleTextBox.Size = new System.Drawing.Size(264, 102);
            this.hashSingleTextBox.TabIndex = 4;
            this.toolTip1.SetToolTip(this.hashSingleTextBox, resources.GetString("hashSingleTextBox.ToolTip"));
            // 
            // hashSingleButton
            // 
            this.hashSingleButton.Enabled = false;
            this.hashSingleButton.Location = new System.Drawing.Point(176, 45);
            this.hashSingleButton.Name = "hashSingleButton";
            this.hashSingleButton.Size = new System.Drawing.Size(94, 23);
            this.hashSingleButton.TabIndex = 3;
            this.hashSingleButton.Text = "Compute Hash";
            this.toolTip1.SetToolTip(this.hashSingleButton, "This button will activate the selected hashing\r\nalgorithm and compute the hash of" +
                    " the\r\nspecified file.  This button only becomes\r\nactive if a file path is in the" +
                    " text box above.");
            this.hashSingleButton.UseVisualStyleBackColor = true;
            this.hashSingleButton.Click += new System.EventHandler(this.hashSingleButton_Click);
            // 
            // browseSingleButton
            // 
            this.browseSingleButton.Location = new System.Drawing.Point(9, 45);
            this.browseSingleButton.Name = "browseSingleButton";
            this.browseSingleButton.Size = new System.Drawing.Size(75, 23);
            this.browseSingleButton.TabIndex = 2;
            this.browseSingleButton.Text = "Browse...";
            this.toolTip1.SetToolTip(this.browseSingleButton, "Click this button to browse your hard disk\r\nor network shares for a file to hash." +
                    "");
            this.browseSingleButton.UseVisualStyleBackColor = true;
            this.browseSingleButton.Click += new System.EventHandler(this.browseSingleButton_Click);
            // 
            // fileSingleTextBox
            // 
            this.fileSingleTextBox.AllowDrop = true;
            this.fileSingleTextBox.Location = new System.Drawing.Point(6, 19);
            this.fileSingleTextBox.Name = "fileSingleTextBox";
            this.fileSingleTextBox.Size = new System.Drawing.Size(264, 20);
            this.fileSingleTextBox.TabIndex = 1;
            this.toolTip1.SetToolTip(this.fileSingleTextBox, "This box should contain the full path to the file\r\nyou would like to compute the " +
                    "hash of.");
            this.fileSingleTextBox.TextChanged += new System.EventHandler(this.fileSingleTextBox_TextChanged);
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
            // compareTabPage
            // 
            this.compareTabPage.AllowDrop = true;
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
            this.toolTip1.SetToolTip(this.compareTabPage, resources.GetString("compareTabPage.ToolTip"));
            this.compareTabPage.UseVisualStyleBackColor = true;
            this.compareTabPage.DragDrop += new System.Windows.Forms.DragEventHandler(this.compareFilesListBox_DragDrop);
            this.compareTabPage.DragEnter += new System.Windows.Forms.DragEventHandler(this.fileSingleTextBox_DragEnter);
            // 
            // clearButton
            // 
            this.clearButton.Enabled = false;
            this.clearButton.Location = new System.Drawing.Point(101, 146);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(75, 23);
            this.clearButton.TabIndex = 3;
            this.clearButton.Text = "Clear List";
            this.toolTip1.SetToolTip(this.clearButton, "Click this button to clear the\r\nentire file list in one click.");
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // compareButton
            // 
            this.compareButton.Enabled = false;
            this.compareButton.Location = new System.Drawing.Point(72, 175);
            this.compareButton.Name = "compareButton";
            this.compareButton.Size = new System.Drawing.Size(132, 23);
            this.compareButton.TabIndex = 5;
            this.compareButton.Text = "Compare Hashes";
            this.toolTip1.SetToolTip(this.compareButton, "Click this button to compute the hashes of\r\nall the files in the file list and co" +
                    "mpare them.\r\nYou will receive a message indicating whether\r\nor not all the files" +
                    " match.");
            this.compareButton.UseVisualStyleBackColor = true;
            this.compareButton.Click += new System.EventHandler(this.compareButton_Click);
            // 
            // removeButton
            // 
            this.removeButton.Enabled = false;
            this.removeButton.Location = new System.Drawing.Point(195, 146);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(75, 23);
            this.removeButton.TabIndex = 4;
            this.removeButton.Text = "Remove";
            this.toolTip1.SetToolTip(this.removeButton, "Click this button to remove the\r\nselected files from the list.");
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(8, 146);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(75, 23);
            this.addButton.TabIndex = 2;
            this.addButton.Text = "Add...";
            this.toolTip1.SetToolTip(this.addButton, "Click this button to open a file dialog box and\r\nselect files to add to the list." +
                    "");
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
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
            this.toolTip1.SetToolTip(this.compareFilesListBox, resources.GetString("compareFilesListBox.ToolTip"));
            this.compareFilesListBox.SelectedIndexChanged += new System.EventHandler(this.compareFilesListBox_SelectedIndexChanged);
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
            // textTabPage
            // 
            this.textTabPage.Controls.Add(this.outputTextBox);
            this.textTabPage.Controls.Add(this.hashTextButton);
            this.textTabPage.Controls.Add(this.label6);
            this.textTabPage.Controls.Add(this.encodingComboBox);
            this.textTabPage.Controls.Add(this.label5);
            this.textTabPage.Controls.Add(this.inputTextBox);
            this.textTabPage.Location = new System.Drawing.Point(4, 22);
            this.textTabPage.Name = "textTabPage";
            this.textTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.textTabPage.Size = new System.Drawing.Size(276, 202);
            this.textTabPage.TabIndex = 2;
            this.textTabPage.Text = "Hash Text";
            this.toolTip1.SetToolTip(this.textTabPage, resources.GetString("textTabPage.ToolTip"));
            this.textTabPage.UseVisualStyleBackColor = true;
            // 
            // outputTextBox
            // 
            this.outputTextBox.Location = new System.Drawing.Point(6, 127);
            this.outputTextBox.Multiline = true;
            this.outputTextBox.Name = "outputTextBox";
            this.outputTextBox.ReadOnly = true;
            this.outputTextBox.Size = new System.Drawing.Size(264, 69);
            this.outputTextBox.TabIndex = 5;
            this.toolTip1.SetToolTip(this.outputTextBox, "This box contains the result of hashing the\r\ntext in the box above, using the cho" +
                    "sen\r\nencoding and hashing algorithm.");
            // 
            // hashTextButton
            // 
            this.hashTextButton.Location = new System.Drawing.Point(205, 97);
            this.hashTextButton.Name = "hashTextButton";
            this.hashTextButton.Size = new System.Drawing.Size(65, 24);
            this.hashTextButton.TabIndex = 4;
            this.hashTextButton.Text = "Hash";
            this.toolTip1.SetToolTip(this.hashTextButton, "Click this button to compute the hash of the\r\ntext in the box above.");
            this.hashTextButton.UseVisualStyleBackColor = true;
            this.hashTextButton.Click += new System.EventHandler(this.hashTextButton_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 100);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "Encoding:";
            // 
            // encodingComboBox
            // 
            this.encodingComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.encodingComboBox.FormattingEnabled = true;
            this.encodingComboBox.Location = new System.Drawing.Point(67, 97);
            this.encodingComboBox.Name = "encodingComboBox";
            this.encodingComboBox.Size = new System.Drawing.Size(132, 21);
            this.encodingComboBox.TabIndex = 2;
            this.toolTip1.SetToolTip(this.encodingComboBox, "This drop-down box lists all the available text\r\nencodings for your system.  Your" +
                    " system\'s\r\ndefault encoding is chosen by default and\r\nwill be what you want to u" +
                    "se most of the time.");
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 5);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(69, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Text to hash:";
            // 
            // inputTextBox
            // 
            this.inputTextBox.Location = new System.Drawing.Point(6, 21);
            this.inputTextBox.Multiline = true;
            this.inputTextBox.Name = "inputTextBox";
            this.inputTextBox.Size = new System.Drawing.Size(264, 69);
            this.inputTextBox.TabIndex = 0;
            this.toolTip1.SetToolTip(this.inputTextBox, "Enter your text to hash in this text box.");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 243);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Hash algorithm:";
            // 
            // hashComboBox
            // 
            this.hashComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.hashComboBox.FormattingEnabled = true;
            this.hashComboBox.Location = new System.Drawing.Point(90, 240);
            this.hashComboBox.Name = "hashComboBox";
            this.hashComboBox.Size = new System.Drawing.Size(116, 21);
            this.hashComboBox.TabIndex = 2;
            this.toolTip1.SetToolTip(this.hashComboBox, resources.GetString("hashComboBox.ToolTip"));
            this.hashComboBox.SelectedIndexChanged += new System.EventHandler(this.hashComboBox_SelectedIndexChanged);
            // 
            // closeButton
            // 
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.Location = new System.Drawing.Point(212, 269);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 4;
            this.closeButton.Text = "Close";
            this.toolTip1.SetToolTip(this.closeButton, "Click this button to\r\nclose WinHasher.");
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // aboutButton
            // 
            this.aboutButton.Location = new System.Drawing.Point(212, 238);
            this.aboutButton.Name = "aboutButton";
            this.aboutButton.Size = new System.Drawing.Size(75, 23);
            this.aboutButton.TabIndex = 3;
            this.aboutButton.Text = "About...";
            this.toolTip1.SetToolTip(this.aboutButton, "Click this button to see more information about\r\nWinHasher, such as the full vers" +
                    "ion number, the\r\nURL where you can find it online, and the\r\nlicense under which " +
                    "it was released.");
            this.aboutButton.UseVisualStyleBackColor = true;
            this.aboutButton.Click += new System.EventHandler(this.aboutButton_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(4, 272);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(74, 13);
            this.label7.TabIndex = 5;
            this.label7.Text = "Output format:";
            // 
            // outputFormatComboBox
            // 
            this.outputFormatComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.outputFormatComboBox.FormattingEnabled = true;
            this.outputFormatComboBox.Items.AddRange(new object[] {
            "Hexadecimal",
            "Base64"});
            this.outputFormatComboBox.Location = new System.Drawing.Point(90, 269);
            this.outputFormatComboBox.Name = "outputFormatComboBox";
            this.outputFormatComboBox.Size = new System.Drawing.Size(116, 21);
            this.outputFormatComboBox.TabIndex = 6;
            this.toolTip1.SetToolTip(this.outputFormatComboBox, resources.GetString("outputFormatComboBox.ToolTip"));
            this.outputFormatComboBox.SelectedIndexChanged += new System.EventHandler(this.outputFormatComboBox_SelectedIndexChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.closeButton;
            this.ClientSize = new System.Drawing.Size(292, 304);
            this.Controls.Add(this.outputFormatComboBox);
            this.Controls.Add(this.label7);
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
            this.textTabPage.ResumeLayout(false);
            this.textTabPage.PerformLayout();
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
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TabPage textTabPage;
        private System.Windows.Forms.TextBox outputTextBox;
        private System.Windows.Forms.Button hashTextButton;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox encodingComboBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox inputTextBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox outputFormatComboBox;
    }
}

