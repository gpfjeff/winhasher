/* MainForm.cs
 * 
 * PROGRAMMER:    Jeffrey T. Darlington
 * DATE:          August 31, 2007
 * PROJECT:       WinHasher GUI application
 * .NET VERSION:  2.0
 * REQUIRES:      com.gpfcomics.WinHasher.Core
 * REQUIRED BY:   (None)
 * 
 * (header comments go here)
 *  
 * This program is Copyright 2007, Jeffrey T. Darlington.
 * E-mail:  jeff@gpf-comics.com
 * Web:     http://www.gpf-comics.com/
 * 
 * This program is free software; you can redistribute it and/or modify it under the terms of
 * the GNU General Public License as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
 * without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * See theGNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with this program;
 * if not, write to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
 * Boston, MA  02110-1301, USA.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using com.gpfcomics.WinHasher.Core;

namespace com.gpfcomics.WinHasher
{
    public partial class MainForm : Form
    {
        #region Private Variables

        // Get our version number from the assembly:
        private static string version = "WinHasher v. " +
            Assembly.GetExecutingAssembly().GetName().Version.ToString();

        // The currently selected hash algorithm:
        private Hashes hash;

        // The last selected directory:
        private string lastDirectory;

        #endregion

        // Our main constructor:
        public MainForm()
        {
            // Let .NET do its initialization stuff:
            InitializeComponent();
            // Build the hash combobox.  There's probably a better way to do this, but I couldn't
            // get it to enumerate over the Hashes enumeration.  So we'll do it manually.
            hashComboBox.Items.Add("MD5");
            hashComboBox.Items.Add("SHA1");
            hashComboBox.Items.Add("SHA256");
            hashComboBox.Items.Add("SHA512");
            // Default to MD5:
            hashComboBox.SelectedIndex = 0;
            hash = Hashes.MD5;
            // Set our the last directory to My Documents:
            try { lastDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); }
            catch { lastDirectory = Environment.CurrentDirectory; }
            // Set up the file boxes for drag & drop:
            fileSingleTextBox.DragEnter += new DragEventHandler(fileSingleTextBox_DragEnter);
            fileSingleTextBox.DragDrop += new DragEventHandler(fileSingleTextBox_DragDrop);
            compareFilesListBox.DragEnter += new DragEventHandler(fileSingleTextBox_DragEnter);
            compareFilesListBox.DragDrop += new DragEventHandler(compareFilesListBox_DragDrop);
            // Set our title bar to include the version number:
            Text = version;
        }

        #region Button Events

        // What to do when the Close button is clicked.  This one's really simple....
        private void closeButton_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        // The About button will launch the About dialog box, giving the user information
        // about our little program:
        private void aboutButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Show about stuff here", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #region Hash Single File Tab Buttons

        // When the Browse button is clicked, open a file dialog to let the user select a file.
        // If everything checks out, put the path to that file into the File to Hash text box.
        private void browseSingleButton_Click(object sender, EventArgs e)
        {
            // Create our open file dialog box and show it:
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = lastDirectory;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Get the file name of the file and put it into the text box:
                    fileSingleTextBox.Text = ofd.FileName;
                    // Derive the directory information from the file's path:
                    FileInfo fi = new FileInfo(ofd.FileName);
                    lastDirectory = fi.DirectoryName;
                    // Enable the hash button:
                    hashSingleButton.Enabled = true;
                    // Clear out any existing hash in the hash text box:
                    hashSingleTextBox.Text = "";
                }
                catch (Exception ex)
                {
                    // If something goes wrong, complain then restore us to our default state:
                    MessageBox.Show("Error: " + ex.ToString(), "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    fileSingleTextBox.Text = "";
                    hashSingleButton.Enabled = false;
                    hashSingleTextBox.Text = "";
                }
            }
        }

        // When the Computer Hash button is clicked, take the path from the File to Hash text
        // box, open the file and read it, then compute the specified hash for that file:
        private void hashSingleButton_Click(object sender, EventArgs e)
        {
            // If the file box is empty or just contains white space, complain:
            if (fileSingleTextBox.Text.Trim() == "")
            {
                MessageBox.Show("Error: Now file has been specified, so there's nothing to do.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                // This should be pretty simple:  Feed the value of the file text box to the
                // hash engine and let it compute the hash, putting the result into the hash
                // text box.  Of course, the try block should imply that you don't always get
                // what you want....
                try
                {
                    UseWaitCursor = true;
                    Refresh();
                    hashSingleTextBox.Text = HashEngine.HashFile(hash, fileSingleTextBox.Text.Trim());
                    UseWaitCursor = false;
                    Refresh();
                }
                // Catch any exceptions.  This should be prettied up later by catching individual
                // exceptions and printing more useful error messages.  Either way, clear out
                // the text boxes and disable the button.
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.ToString(), "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    fileSingleTextBox.Text = "";
                    hashSingleButton.Enabled = false;
                    hashSingleTextBox.Text = "";
                    UseWaitCursor = false;
                    Refresh();
                }
            }
        }

        #endregion

        #region Compare Files Tab Buttons

        // When the Add button is clicked, open a file dialog and allow the user to select one
        // or more files.  Then add those files to the list box, making sure that each file is
        // only added once.
        private void addButton_Click(object sender, EventArgs e)
        {
            // Open and display the file dialog.  Note that multi-select is on, so the user
            // can grab any number of files.
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = lastDirectory;
            ofd.Multiselect = true;
            // If they clicked OK:
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                // Sorry, Yoda, but I can only do my best:
                try
                {
                    // By default, assume no files are really being added:
                    bool addedFiles = false;
                    // Step through the list of selected files from the dialog:
                    foreach (string file in ofd.FileNames)
                    {
                        // Assume that this particular file isn't already in the list:
                        bool inList = false;
                        // Step through the files currently in the list box:
                        foreach (object listFile in compareFilesListBox.Items)
                        {
                            // If the selected file is already in the list, we don't want to
                            // add it twice.  So mark that we found it and stop looping:
                            if (file.CompareTo((string)listFile) == 0)
                            {
                                inList = true;
                                break;
                            }
                        }
                        // If the file isn't in the list, add it, noting that we've added at
                        // least one file.  Also take note of the directory the file came
                        // from.
                        if (!inList)
                        {
                            compareFilesListBox.Items.Add(file);
                            addedFiles = true;
                            FileInfo fi = new FileInfo(file);
                            lastDirectory = fi.DirectoryName;
                        }
                    }
                    // If we added any files, enable the Compare Hashes and Clear List buttons:
                    if (addedFiles)
                    {
                        if (compareFilesListBox.Items.Count > 1)
                        {
                            compareButton.Enabled = true;
                            clearButton.Enabled = true;
                        }
                    }
                }
                // Expand this with more useful error messages later:
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.ToString(), "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    fileSingleTextBox.Text = "";
                    hashSingleButton.Enabled = false;
                    hashSingleTextBox.Text = "";
                }
            }

        }

        // When the Remove button is clicked, remove any selected files from the file list:
        private void removeButton_Click(object sender, EventArgs e)
        {
            // Make sure at least one file is selected.  This shouldn't be problem, but it's
            // always safer to check:
            if (compareFilesListBox.SelectedItems.Count > 0)
            {
                // We can't iterate through the list and change it at the same time.  So
                // create a separate array to hold the strings of the selected files' paths.
                string[] fileList = new string[compareFilesListBox.Items.Count];
                int counter = 0;
                foreach (object file in compareFilesListBox.SelectedItems)
                {
                    fileList[counter] = (string)file;
                    counter++;
                }
                // The step through the new array and remove each file from the list box:
                foreach (string s in fileList)
                {
                    compareFilesListBox.Items.Remove(s);
                }
                // Check the count of the files and disable the Compare, Remove, and Clear
                // buttons as appropriate:
                if (compareFilesListBox.Items.Count <= 1) compareButton.Enabled = false;
                if (compareFilesListBox.Items.Count < 1)
                {
                    removeButton.Enabled = false;
                    clearButton.Enabled = false;
                }
            }
            // This should never happen:
            else
            {
                MessageBox.Show("Error: You must select at least one file before you can " +
                    "remove anything from the list.", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // When the Compare Hashes button is clicked, compute the hash of each file in the list
        // and compare them.  If they all match, then all the files are the same.  If at least
        // one fails to match, then all the files fail the test.
        private void compareButton_Click(object sender, EventArgs e)
        {
            // Lots of places where this could fail:
            try
            {
                // The hash engine compare function takes an array of file path strings.
                // There's probably a better way to do this, but we'll step through each
                // element in the list box and put it into a proper string array first:
                string[] fileList = new string[compareFilesListBox.Items.Count];
                for (int i = 0; i < compareFilesListBox.Items.Count; i++)
                    fileList[i] = (string)compareFilesListBox.Items[i];
                // Make sure the user knows we're busy:
                UseWaitCursor = true;
                Refresh();
                // Do the comparison.  This returns either true or false:
                if (HashEngine.CompareHashes(hash, fileList))
                {
                    // The test passed; all hashes matched:
                    MessageBox.Show("Congratulations! All " + fileList.Length +
                        " files match!", "Hashes Match", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    // The test failed; one of the hashes didn't match:
                    MessageBox.Show("WARNING! At least one of the " + fileList.Length +
                        " files did not match!", "Hashes Don't Match", MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
                // Get rid of the wait cursor:
                UseWaitCursor = false;
                Refresh();
            }
            // Catch the exceptions.  We need to make some nicer error messages, though:
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.ToString(), "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                UseWaitCursor = false;
                Refresh();
            }
        }

        // When the Clear List button is clicked clear the list of all entires.
        // Also make sure to disable the Remove, Compare, and Clear buttons, as they
        // now have nothing to do:
        private void clearButton_Click(object sender, EventArgs e)
        {
            compareFilesListBox.Items.Clear();
            removeButton.Enabled = false;
            compareButton.Enabled = false;
            clearButton.Enabled = false;
        }

        #endregion

        #endregion

        #region Other GUI Events

        // When the hash dropdown changes, change the active hash:
        private void hashComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch ((string)hashComboBox.SelectedItem)
            {
                case "MD5":
                    hash = Hashes.MD5;
                    break;
                case "SHA1":
                    hash = Hashes.SHA1;
                    break;
                case "SHA256":
                    hash = Hashes.SHA256;
                    break;
                case "SHA512":
                    hash = Hashes.SHA512;
                    break;
                // This should never happen, but default to MD5 if we get something
                // invalid:
                default:
                    MessageBox.Show("Error: Invalid hash. Defaulting to MD5", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    hash = Hashes.MD5;
                    break;
            }
        }

        // When the user switch which tab is active, change which button is the
        // "accept" button, i.e. the default when the Enter button is pressed:
        private void modeTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            // For the single hash page, make it the Compute Hash button:
            if (modeTabControl.SelectedIndex == 0)
            {
                AcceptButton = hashSingleButton;
            }
            // For the compare tab, make it the Compare Hashes button:
            else
            {
                AcceptButton = compareButton;
            }
        }

        // When the user changes the value of the file path text box, enable (or disable)
        // the Compute Hash button:
        private void fileSingleTextBox_TextChanged(object sender, EventArgs e)
        {
            if (fileSingleTextBox.Text.Trim() == "") { hashSingleButton.Enabled = false; }
            else { hashSingleButton.Enabled = true; }
        }

        // When the selected index on the file list box changes, enable or disable the
        // Remove button.  Only enable the button if at least one item is selected;
        // disable it if no files are selected.
        private void compareFilesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (compareFilesListBox.SelectedItems.Count > 0) removeButton.Enabled = true;
            else removeButton.Enabled = false;
        }

        #endregion

        #region Drag and Drop Event Handlers

        // We only accept files as drag and drop data.  Since all GUI elements do the
        // same basic thing when we drag stuff onto them (copy the values), all the
        // drag enter events point to this one.  Note that we won't accept any other
        // form of data other than files.
        void fileSingleTextBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else e.Effect = DragDropEffects.None;
        }

        // When we drop files on the compare files tab, add those files paths to the
        // file list box.  Note that we don't clear out the list first, but add the
        // files to whatever may already be there.
        void compareFilesListBox_DragDrop(object sender, DragEventArgs e)
        {
            // Stuff could barf here:
            try
            {
                // Only accept file drop data:
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    // This is similar to the add button.  (In fact, I ought to abstract
                    // this and combine this into a single method.)  Given the list of
                    // file names as a string array, add them to the file list, making
                    // sure no duplicates are added.
                    string[] fileList = (string[])e.Data.GetData(DataFormats.FileDrop);
                    bool addedFiles = false;
                    foreach (string file in fileList)
                    {
                        bool inList = false;
                        foreach (object listFile in compareFilesListBox.Items)
                        {
                            if (file.CompareTo((string)listFile) == 0)
                            {
                                inList = true;
                                break;
                            }
                        }
                        if (!inList)
                        {
                            compareFilesListBox.Items.Add(file);
                            addedFiles = true;
                            FileInfo fi = new FileInfo(file);
                            lastDirectory = fi.DirectoryName;
                        }
                    }
                    // If any files were added, enable the compare and clear buttons
                    // as appropriate:
                    if (addedFiles)
                    {
                        if (compareFilesListBox.Items.Count > 1)
                        {
                            compareButton.Enabled = true;
                        }
                        if (compareFilesListBox.Items.Count >= 1)
                        {
                            clearButton.Enabled = true;
                        }
                    }
                }
                // Something other files were dropped:
                else
                {
                    MessageBox.Show("Error: Only files can be dropped onto this applet.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            // Expand this with more meaningful error messages:
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.ToString(), "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // When we drop a file on the single hash tab, add that file's path to the
        // file text box.  Note that we'll only accept one file here; if multiple
        // files are dropped, complain.
        void fileSingleTextBox_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] fileList = (string[])e.Data.GetData(DataFormats.FileDrop);
                    if (fileList.Length == 1)
                    {
                        modeTabControl.SelectedTab = singleTabPage;
                        fileSingleTextBox.Text = fileList[0];
                    }
                    else
                    {
                        MessageBox.Show("Error: Two many files; only drop one into this box.",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Error: Only files can be dropped onto this applet.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.ToString(), "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        #endregion

    }
}