/* MainForm.cs
 * 
 * PROGRAMMER:    Jeffrey T. Darlington
 * DATE:          August 31, 2007
 * PROJECT:       WinHasher GUI application
 * .NET VERSION:  2.0
 * REQUIRES:      com.gpfcomics.WinHasher.Core
 * REQUIRED BY:   (None)
 * 
 * The main form of the WinHasher GUI application.  This provides a nice point-and-click, drag &
 * drop interface for computing the hashes of files.  I created this primarily because I was sick
 * of downloading files off the Internet and see their MD5 or SHA1 hashes listed for verification,
 * but not having a way to check these hashes in Windows.  Just about every other OS now has hashing
 * either built-in or in standard add-ons; Microsoft, of course, is way behind the times.
 * 
 * This program operates in two modes:  single file hashing and multi-file comparisons.  In single
 * file mode, it takes a single file, reads it, computes its hash, and displays the result.  In
 * multi-file mode, it computes the hash of each file in a user-specified list and compares the
 * results.  If all the hashes match, the whole set is said to be identical; if at least one hash
 * does not match, the whole batch fails.  The user can specify which hash to use in either case.
 * 
 * This program uses the WinHasherCore library for its HashEngine methods.
 * 
 * UPDATE June 18, 2008 (1.3):  Added new Hash Text tab to allow hashing of arbitrary text.  Enter
 * the text in the box, choose a text encoding (defaults to system default), then click the button
 * to get the hash.  Also added the output drop-down to allow choosing between hexadecimal (which
 * is the most common output format) and Base64 (which I personally tend to use more often).
 * 
 * UPDATE February 12, 2009 (1.4):  Added changes to support abstractions of output type in
 * HashEngine so we can expand beyond just hexadecimal and Base64.  Added all-caps hex and
 * Bubble Babble output formats.  Added warning discouraging the use of MD5 once per session.
 * 
 * UPDATE March 26, 2009 (1.4):  Corrected modeTabControl_SelectedIndexChanged() to make the
 * Hash button on the Hash Text tab the default Accept Button when that tab is selected.  Added
 * functionality to save the last set of preferences to the registory so they can be restored
 * when the program is reopened.  Added checkbox to turn tooltips on or off.
 * 
 * UPDATE July 8, 2009 (1.5):  Added Compare To field and comparison result lable to Hash Single
 * File tab.  Now the user can copy a pre-computed hash from somewhere else (such as a Web site)
 * and paste it into the Compare To field.  WinHasher will compare the two hashes and display
 * whether or not the two values match.  This is similar to the new functionality of the
 * ResultDialog added when the program is called in command-line mode.
 * 
 * This program is Copyright 2009, Jeffrey T. Darlington.
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
using Microsoft.Win32;
using com.gpfcomics.WinHasher.Core;

namespace com.gpfcomics.WinHasher
{
    public partial class MainForm : Form
    {
        #region Private Variables

        /// <summary>
        /// The full version number of the assmebly for display:
        /// </summary>
        private static string version = "WinHasher v. " +
            Assembly.GetExecutingAssembly().GetName().Version.ToString();

        /// <summary>
        /// The shortened (major + minor) version number for display:
        /// </summary>
        private static string versionShort = "WinHasher v. " +
            Assembly.GetExecutingAssembly().GetName().Version.Major.ToString() + "." +
            Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString();

        /// <summary>
        /// Our copyright information, fetched from the assembly info:
        /// </summary>
        private static string copyright = "";

        /// <summary>
        /// The currently selected hash algorithm
        /// </summary>
        private Hashes hash;

        /// <summary>
        /// The last selected directory
        /// </summary>
        private string lastDirectory;

        /// <summary>
        /// The ouput encoding for resulting hashes
        /// </summary>
        private OutputType outputType = OutputType.Hex;

        /// <summary>
        /// We want to show a warning the first time MD5 is chosen from the hash drop-down
        /// list.  If this flag is true, the pop-up will appear.  If false, it won't.
        /// </summary>
        private bool showMD5Warning = true;

        #endregion

        /// <summary>
        /// Main constructor
        /// </summary>
        public MainForm()
        {
            // Let .NET do its initialization stuff:
            InitializeComponent();
            // Get our copyright information.  It seems a bit silly to do it this way,
            // but this seems to be the only way to do it that I can find.  We'll pull this
            // from the assembly so we only need to change it in one place, and it can be
            // automatically fetched from SVN.
            object[] obj = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            if (obj != null && obj.Length > 0)
                copyright = ((AssemblyCopyrightAttribute)obj[0]).Copyright;
            // Build the hash combobox.  There's probably a better way to do this, but I couldn't
            // get it to enumerate over the Hashes enumeration.  So we'll do it manually.
            hashComboBox.Items.Add("MD5");
            hashComboBox.Items.Add("SHA-1");
            hashComboBox.Items.Add("SHA-256");
            hashComboBox.Items.Add("SHA-384");
            hashComboBox.Items.Add("SHA-512");
            hashComboBox.Items.Add("RIPEMD-160");
            hashComboBox.Items.Add("Whirlpool");
            hashComboBox.Items.Add("Tiger");
            // Populate the encoding drop-down:
            foreach (EncodingInfo encodingInfo in Encoding.GetEncodings())
            {
                encodingComboBox.Items.Add(encodingInfo.GetEncoding());
            }
            // Set it to show the display name in the dropdown:
            encodingComboBox.DisplayMember = "EncodingName";

            // Attempt to load our settings from the registry:
            try
            {
                // Try to open the HKCU\Software\GPF Comics\WinHasher key:
                RegistryKey winHasherSettings =
                    Registry.CurrentUser.OpenSubKey("Software", false).OpenSubKey("GPF Comics",
                    false).OpenSubKey("WinHasher", false);
                // Try to set the hash combo box:
                hashComboBox.SelectedIndex = (int)winHasherSettings.GetValue("SelectedHash", 1);
                hashComboBox_SelectedIndexChanged(null, null);
                // Try to set the encoding combo box:
                encodingComboBox.SelectedIndex = (int)winHasherSettings.GetValue("TextEncoding",
                    encodingComboBox.Items.IndexOf(Encoding.Default));
                // Try to set the output format combo box:
                outputFormatComboBox.SelectedIndex = (int)winHasherSettings.GetValue("OutputFormat", 0);
                outputFormatComboBox_SelectedIndexChanged(null, null);
                // Try to set the last directory:
                string ldtemp = "";
                try { ldtemp = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); }
                catch { ldtemp = Environment.CurrentDirectory; }
                lastDirectory = (string)winHasherSettings.GetValue("LastDirectory", ldtemp);
                // Try to restore the last used tab:
                modeTabControl.SelectedIndex = (int)winHasherSettings.GetValue("LastTab", 0);
                modeTabControl_SelectedIndexChanged(null, null);
                // Try to restore the tooltips toggle:
                tooltipsCheckbox.Checked = (int)winHasherSettings.GetValue("ShowToolTips", 1)
                    == 1 ? true : false;
                // Close the registry key:
                winHasherSettings.Close();
            }
            // If any of the registry reading code above fails, restore everything to the
            // default settings:
            catch
            {
                // We used to default to MD5, but that's been severely broken.  Default to SHA-1
                // instead.
                hashComboBox.SelectedIndex = 1;
                hash = Hashes.SHA1;
                // Select the system default encoding by default:
                encodingComboBox.SelectedIndex = encodingComboBox.Items.IndexOf(Encoding.Default);
                // Force the output drop-down to the first item, or hex output:
                outputFormatComboBox.SelectedIndex = 0;
                // Set our the last directory to My Documents:
                try { lastDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); }
                catch { lastDirectory = Environment.CurrentDirectory; }
                // Default to the Hash Single File tab:
                modeTabControl.SelectedIndex = 0;
                // Default to tooltips being on:
                tooltipsCheckbox.Checked = true;
            }

            // Set up the file boxes for drag & drop:
            fileSingleTextBox.DragEnter += new DragEventHandler(fileSingleTextBox_DragEnter);
            fileSingleTextBox.DragDrop += new DragEventHandler(fileSingleTextBox_DragDrop);
            compareFilesListBox.DragEnter += new DragEventHandler(fileSingleTextBox_DragEnter);
            compareFilesListBox.DragDrop += new DragEventHandler(compareFilesListBox_DragDrop);
            // Set our title bar to include the version number:
            Text = versionShort;
            // Turn on or off the tooltips depending on whether the checkbox has been
            // checked:
            toolTip1.IsBalloon = true;
            if (tooltipsCheckbox.Checked) toolTip1.Active = true;
            else toolTip1.Active = false;
        }

        #region Button Events

        /// <summary>
        /// What to do when the Close button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeButton_Click(object sender, EventArgs e)
        {
            // By default, this doesn't fire the FormClosing event, so we'll need to call
            // that explicitly:
            MainForm_FormClosing(null, null);
            // Now get rid of the form:
            Dispose();
        }

        /// <summary>
        /// The About button will launch the About dialog box, giving the user information
        /// about our little program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aboutButton_Click(object sender, EventArgs e)
        {
            // Get the full path to the HTML help file:
            string helpFile = Application.StartupPath;
            if (!helpFile.EndsWith("\\")) helpFile += "\\";
            helpFile += Properties.Resources.HelpFile;
            // Now build and show the about dialog:
            AboutDialog ad = new AboutDialog(version, copyright, Properties.Resources.URL,
                Properties.Resources.License, helpFile, tooltipsCheckbox.Checked);
            ad.ShowDialog();
        }

        #region Hash Single File Tab Buttons

        /// <summary>
        /// When the Browse button is clicked, open a file dialog to let the user select a file.
        /// If everything checks out, put the path to that file into the File to Hash text box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                #region Catch Exceptions
                // Most of these are thrown by the FileInfo constructor, although a couple
                // are thrown by its DirectoryName property.  In each case, complain about
                // the problem then restore the GUI to the default state.
                catch (System.Security.SecurityException)
                {
                    MessageBox.Show("Error: You do not have permission to access the file \"" +
                        ofd.FileName + "\".", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    fileSingleTextBox.Text = "";
                    hashSingleButton.Enabled = false;
                    hashSingleTextBox.Text = "";
                }
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show("Error: Access to the file \"" + ofd.FileName +
                        "\" has been denied.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    fileSingleTextBox.Text = "";
                    hashSingleButton.Enabled = false;
                    hashSingleTextBox.Text = "";
                }
                catch (PathTooLongException)
                {
                    MessageBox.Show("Error: The path \"" + ofd.FileName +
                        "\" is too long for the system to handle.", "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    fileSingleTextBox.Text = "";
                    hashSingleButton.Enabled = false;
                    hashSingleTextBox.Text = "";
                }
                catch (NotSupportedException)
                {
                    MessageBox.Show("Error: The path \"" + ofd.FileName +
                        "\" contains invalid characters.", "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    fileSingleTextBox.Text = "";
                    hashSingleButton.Enabled = false;
                    hashSingleTextBox.Text = "";
                }
                catch (ArgumentNullException)
                {
                    MessageBox.Show("Error: The file path specified was empty.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    fileSingleTextBox.Text = "";
                    hashSingleButton.Enabled = false;
                    hashSingleTextBox.Text = "";
                }
                catch (ArgumentException)
                {
                    MessageBox.Show("Error: The file path was empty, contained only white spaces, " +
                        "or contained invalid characters.", "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    fileSingleTextBox.Text = "";
                    hashSingleButton.Enabled = false;
                    hashSingleTextBox.Text = "";
                }
                // A generic catch, just in case:
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.ToString(), "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    fileSingleTextBox.Text = "";
                    hashSingleButton.Enabled = false;
                    hashSingleTextBox.Text = "";
                }
                #endregion
            }
        }

        /// <summary>
        /// When the Compute Hash button is clicked, take the path from the File to Hash text
        /// box, open the file and read it, then compute the specified hash for that file:
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hashSingleButton_Click(object sender, EventArgs e)
        {
            // If the file box is empty or just contains white space, complain:
            if (fileSingleTextBox.Text.Trim() == "")
            {
                MessageBox.Show("Error: No file has been specified, so there's nothing to do.",
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
                    // Make it look like we're busy:
                    UseWaitCursor = true;
                    hashSingleTextBox.Text = "Hashing in progress...";
                    Refresh();
                    // Create the progress dialog and show it.  This kicks off the
                    // actual hashing process.
                    ProgressDialog pd = new ProgressDialog(fileSingleTextBox.Text.Trim(), hash, 
                        false, outputType);
                    pd.ShowDialog();
                    // What we do next depends on the result:
                    switch (pd.Result)
                    {
                        // Success:  Display the hash:
                        case ProgressDialog.ResultStatus.Success:
                            hashSingleTextBox.Text = pd.Hash;
                            break;
                        // Cancelled:  Let the user know they cancelled the hash:
                        case ProgressDialog.ResultStatus.Cancelled:
                            hashSingleTextBox.Text = "The hash of this file was cancelled.";
                            break;
                        // Error:  Warn the user of the error:
                        case ProgressDialog.ResultStatus.Error:
                            hashSingleTextBox.Text = "An error occurred while hashing this file.";
                            break;
                        // Anything else:  Clear out the hash text box:
                        default:
                            hashSingleTextBox.Text = "";
                            break;
                    }
                    // Get ready to do stuff again:
                    UseWaitCursor = false;
                    Refresh();
                }
                #region Catch Exceptions
                // The only thing we really need to catch here are HashEngineExceptions, which
                // are thrown in the HashEngine.HashFile() call.  This should contain the error
                // message already, so just print it out.  But as an extra precaution, we'll catch
                // any other generic exception, just in case.  Note that in each case, we'll clear
                // out the GUI and return it to its default state.
                catch (HashEngineException hee)
                {
                    MessageBox.Show("Error: " + hee.ToString(), "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    fileSingleTextBox.Text = "";
                    hashSingleButton.Enabled = false;
                    hashSingleTextBox.Text = "";
                    UseWaitCursor = false;
                    Refresh();
                }
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
                #endregion
            }
        }

        #endregion

        #region Compare Files Tab Buttons

        /// <summary>
        /// When the Add button is clicked, open a file dialog and allow the user to select one
        /// or more files.  Then add those files to the list box, making sure that each file is
        /// only added once.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                PopulateFileListBox(ofd.FileNames);
            }

        }

        /// <summary>
        /// When the Remove button is clicked, remove any selected files from the file list:
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// When the Compare Hashes button is clicked, compute the hash of each file in the list
        /// and compare them.  If they all match, then all the files are the same.  If at least
        /// one fails to match, then all the files fail the test.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                // Create the progress dialog and show it.  This will start the actual comparison
                // process.
                ProgressDialog pd = new ProgressDialog(fileList, hash);
                pd.ShowDialog();
                // Check the progress dialog and make sure we got a result.  If it says the files
                // match, show the success message:
                if (pd.Result == ProgressDialog.ResultStatus.Success && pd.FilesMatch)
                {
                    // The test passed; all hashes matched:
                    MessageBox.Show("Congratulations! All " + fileList.Length +
                        " files match!", "Hashes Match", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                // If the dialog returns success, but the files didn't match, warn the user.
                // If the dialog returned anything but success, we'll assume they've already
                // seen the error message.
                else if (pd.Result == ProgressDialog.ResultStatus.Success)
                {
                    MessageBox.Show("WARNING! At least one of the " + fileList.Length +
                        " files did not match!", "Hashes Don't Match", MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
                // Get rid of the wait cursor:
                UseWaitCursor = false;
                Refresh();
            }
            #region Catch Exceptions
            // Our primary concern is the HashEngineException.  This should already contain the
            // error message, so just spit it back out:
            catch (HashEngineException hee)
            {
                MessageBox.Show("Error: " + hee.ToString(), "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                UseWaitCursor = false;
                Refresh();
            }
            // Generic exception catch, just in case:
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.ToString(), "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                UseWaitCursor = false;
                Refresh();
            }
            #endregion
        }

        /// <summary>
        /// When the Clear List button is clicked clear the list of all entires.
        /// Also make sure to disable the Remove, Compare, and Clear buttons, as they
        /// now have nothing to do
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearButton_Click(object sender, EventArgs e)
        {
            compareFilesListBox.Items.Clear();
            removeButton.Enabled = false;
            compareButton.Enabled = false;
            clearButton.Enabled = false;
        }

        #endregion

        /// <summary>
        /// When the Hash button on the Hash Text tab is clicked, hash the text in the text box
        /// using the selected text encoding
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hashTextButton_Click(object sender, EventArgs e)
        {
            // Only bother if there's text to hash:
            if (inputTextBox.Text != null && inputTextBox.Text != "")
            {
                // This one's simple enough:  Grab the selected hash, the text, the selected
                // encoding and output formats, and hash it:
                try
                {
                    outputTextBox.Text = HashEngine.HashText(hash, inputTextBox.Text,
                        (Encoding)encodingComboBox.SelectedItem, outputType);
                }
                // This should be more specific:
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            // If no text is in the box, complain:
            else MessageBox.Show("Error: No text has been specified, so there's nothing to do.",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #endregion

        #region Other GUI Events

        /// <summary>
        /// When the hash dropdown changes, change the active hash
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hashComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch ((string)hashComboBox.SelectedItem)
            {
                case "MD5":
                    hash = Hashes.MD5;
                    // MD5 is broken.  I mean *REALLY* broken.  Warn the user once per session
                    // that they shouldn't be using it:
                    if (showMD5Warning)
                    {
                        MessageBox.Show("Please note that the MD5 algorithm is no longer considered" +
                            " secure by most security experts. Therefore, its use should be " +
                            " strongly discouraged. WinHasher will continue to support MD5, but" +
                            " will display this warning the first time it is selected. If at all" +
                            " possible, you should consider using a stronger algorithm.", "Warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        showMD5Warning = false;
                    }
                    break;
                case "SHA-1":
                    hash = Hashes.SHA1;
                    break;
                case "SHA-256":
                    hash = Hashes.SHA256;
                    break;
                case "SHA-384":
                    hash = Hashes.SHA384;
                    break;
                case "SHA-512":
                    hash = Hashes.SHA512;
                    break;
                case "RIPEMD-160":
                    hash = Hashes.RIPEMD160;
                    break;
                case "Whirlpool":
                    hash = Hashes.Whirlpool;
                    break;
                case "Tiger":
                    hash = Hashes.Tiger;
                    break;
                // This should never happen, but default to SHA-1 if we get something
                // invalid:
                default:
                    MessageBox.Show("Error: Invalid hash. Defaulting to SHA-1", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    hash = Hashes.SHA1;
                    break;
            }
        }

        /// <summary>
        /// When the user switch which tab is active, change which button is the
        /// "accept" button, i.e. the default when the Enter button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void modeTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            // For the single hash page, make it the Compute Hash button:
            if (modeTabControl.SelectedIndex == 0) AcceptButton = hashSingleButton;
            // For the compare tab, make it the Compare Hashes button:
            else if (modeTabControl.SelectedIndex == 1) AcceptButton = compareButton;
            // For the hash text tab, make it the Hash Text button:
            else AcceptButton = hashTextButton;
        }

        /// <summary>
        /// When the user changes the value of the file path text box, enable (or disable)
        /// the Compute Hash button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fileSingleTextBox_TextChanged(object sender, EventArgs e)
        {
            if (fileSingleTextBox.Text.Trim() == "") { hashSingleButton.Enabled = false; }
            else { hashSingleButton.Enabled = true; }
        }

        /// <summary>
        /// When the selected index on the file list box changes, enable or disable the
        /// Remove button.  Only enable the button if at least one item is selected;
        /// disable it if no files are selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void compareFilesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (compareFilesListBox.SelectedItems.Count > 0) removeButton.Enabled = true;
            else removeButton.Enabled = false;
        }

        /// <summary>
        /// When the selected index on the output format list box changes, set the flag to
        /// determine the output format of the resulting hash
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void outputFormatComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch ((string)outputFormatComboBox.SelectedItem)
            {
                case "Base64":
                    outputType = OutputType.Base64;
                    break;
                case "Bubble Babble":
                    outputType = OutputType.BubbleBabble;
                    break;
                case "Hex (Caps)":
                    outputType = OutputType.CapHex;
                    break;
                case "Hexadecimal":
                default:
                    outputType = OutputType.Hex;
                    break;
            }
        }

        /// <summary>
        /// Fired on the FormClosing event, this method attempts to store the last used UI
        /// settings to the Windows registry.  If this fails in any way, nothing will be
        /// stored and the defaults will be restored when the program is next opened.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Let's give it the old college try:
            try
            {
                // Try to open HKCU\Software.  This *should* always exist:
                RegistryKey HKCU_Software = Registry.CurrentUser.OpenSubKey("Software", true);
                if (HKCU_Software != null)
                {
                    // Try to open HKCU\Software\GPF Comics.  If this key doesn't exist we'll
                    // try to create it:
                    RegistryKey GPFComics = HKCU_Software.OpenSubKey("GPF Comics", true);
                    if (GPFComics == null) GPFComics = HKCU_Software.CreateSubKey("GPF Comics");
                    if (GPFComics != null)
                    {
                        // Try to open HKCU\Software\GPF Comics\WinHasher.  If this key doesn't
                        // exist we'll try to create it:
                        RegistryKey winHasherSettings = GPFComics.OpenSubKey("WinHasher", true);
                        if (winHasherSettings == null) winHasherSettings =
                            GPFComics.CreateSubKey("WinHasher");
                        if (winHasherSettings != null)
                        {
                            // By now we should be in our own little private paradise.  Start
                            // saving our settings.  For the drop-downs and tabs, we'll just
                            // safe the index value.  For the last directory, we'll save the
                            // path string.  There might be a security question around saving
                            // this path, of course, but we'll go with it as is.
                            winHasherSettings.SetValue("SelectedHash",
                                hashComboBox.SelectedIndex, RegistryValueKind.DWord);
                            winHasherSettings.SetValue("TextEncoding",
                                encodingComboBox.SelectedIndex, RegistryValueKind.DWord);
                            winHasherSettings.SetValue("OutputFormat",
                                outputFormatComboBox.SelectedIndex, RegistryValueKind.DWord);
                            winHasherSettings.SetValue("LastDirectory",
                                lastDirectory, RegistryValueKind.String);
                            winHasherSettings.SetValue("LastTab", modeTabControl.SelectedIndex,
                                RegistryValueKind.DWord);
                            winHasherSettings.SetValue("Version",
                                Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                                RegistryValueKind.String);
                            winHasherSettings.SetValue("ShowToolTips",
                                (tooltipsCheckbox.Checked ? 1 : 0),
                                RegistryValueKind.DWord);
                            // We're done, so close up shop:
                            winHasherSettings.Close();
                        }
                        GPFComics.Close();
                    }
                    HKCU_Software.Close();
                }
            }
            // If we failed for any reason, don't bother with anything else.  The defaults
            // will be restored the next time the program is opened.
            catch { }
        }

        /// <summary>
        /// What to do when the ToolTips checkbox is toggled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tooltipsCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            // Fortunately, the ToolTip.Active property turns tooltips on and off, so
            // this is relatively easy:
            if (tooltipsCheckbox.Checked) toolTip1.Active = true;
            else toolTip1.Active = false;
        }

        /// <summary>
        /// What to do when the Compare To text box on the Hash Single File tab changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void compareToTextBox_TextChanged(object sender, EventArgs e)
        {
            // If the Compare To field is empty (the default) we don't want to show a false
            // error.  In this case, just hide the lable.
            if (String.IsNullOrEmpty(compareToTextBox.Text) ||
                String.IsNullOrEmpty(hashSingleTextBox.Text))
            {
                compareResultLabel.Visible = false;
                compareResultLabel.Text = "";
                compareResultLabel.ForeColor = SystemColors.ControlText;
                compareResultLabel.BackColor = SystemColors.Control;
            }
            // If the two strings match, then the generated hash matches the pre-existing
            // hash and the user can safely say the file is unaltered and intact:
            else if (String.Compare(hashSingleTextBox.Text, compareToTextBox.Text) == 0)
            {
                compareResultLabel.Visible = true;
                compareResultLabel.Text = "Hashes match";
                compareResultLabel.ForeColor = Color.White;
                compareResultLabel.BackColor = Color.Green;
            }
            // Otherwise, the strings don't match, the hashes don't match, and the file is
            // not what it claims to be:
            else
            {
                compareResultLabel.Visible = true;
                compareResultLabel.Text = "Hashes do not match";
                compareResultLabel.ForeColor = Color.Yellow;
                compareResultLabel.BackColor = Color.Red;
            }

        }

        #endregion

        #region Drag and Drop Event Handlers

        /// <summary>
        /// We only accept files as drag and drop data.  Since all GUI elements do the
        /// same basic thing when we drag stuff onto them (copy the values), all the
        /// drag enter events point to this one.  Note that we won't accept any other
        /// form of data other than files.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void fileSingleTextBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else e.Effect = DragDropEffects.None;
        }

        /// <summary>
        /// When we drop files on the compare files tab, add those files paths to the
        /// file list box.  Note that we don't clear out the list first, but add the
        /// files to whatever may already be there.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void compareFilesListBox_DragDrop(object sender, DragEventArgs e)
        {
            // Only accept file drop data:
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // This is similar to the add button.  (In fact, I ought to abstract
                // this and combine this into a single method.)  Given the list of
                // file names as a string array, add them to the file list, making
                // sure no duplicates are added.
                string[] fileList = (string[])e.Data.GetData(DataFormats.FileDrop);
                PopulateFileListBox(fileList);
            }
            // Something other files were dropped:
            else
            {
                MessageBox.Show("Error: Only files can be dropped onto this tab.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// When we drop a file on the single hash tab, add that file's path to the
        /// file text box.  Note that we'll only accept one file here; if multiple
        /// files are dropped, complain.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void fileSingleTextBox_DragDrop(object sender, DragEventArgs e)
        {
            // Make sure we're only getting files dropped and nothing else:
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Get the file list.  We only want one file dropped on this tab, not multiples,
                // so complain if they try to drop more than one.  If we get just one, though,
                // drop its path into the file text box and go ahead and trigger the hash.
                string[] fileList = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (fileList.Length == 1)
                {
                    modeTabControl.SelectedTab = singleTabPage;
                    fileSingleTextBox.Text = fileList[0];
                    hashSingleButton_Click(sender, e);
                }
                // Got too many files:
                else
                {
                    MessageBox.Show("Error: Two many files; only drop one into this box.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            // The data dropped wasn't a file:
            else
            {
                MessageBox.Show("Error: Only files can be dropped onto this tab.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Generic Private Methods

        /// <summary>
        /// Abstracted here for code reuse, this method takes a string array which is assumed to
        /// contain a list of file path strings.  These can come from a drap & drop operation or
        /// from an OpenFileDialog.FileNames property.  Given this list, step through each file.
        /// Compare each one to the items already in the list box and make sure it isn't a duplicate.
        /// If it isn't, add it to the list.  The do any necessary GUI tweaking to make sure the
        /// right buttons are enabled.  Note that this method catches its own exceptions so the
        /// caller doesn't need to worry about them.
        /// </summary>
        /// <param name="fileList">The string array of file names</param>
        private void PopulateFileListBox(string[] fileList)
        {
            try
            {
                // By default, assume no files are really being added:
                bool addedFiles = false;
                // Step through the list of selected files from the dialog:
                foreach (string file in fileList)
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
                    if (compareFilesListBox.Items.Count > 1) { compareButton.Enabled = true; }
                    if (compareFilesListBox.Items.Count > 0) { clearButton.Enabled = true; }
                }
            }
            #region Catch Exceptions
            // Most of these are thrown by the FileInfo constructor, although a couple
            // are thrown by its DirectoryName property.
            catch (System.Security.SecurityException)
            {
                MessageBox.Show("Error: You do not have permission to access one or more of " +
                    "the files.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Error: Access to one or more of the files has been denied.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (PathTooLongException)
            {
                MessageBox.Show("Error: One or more of the file paths is too long for the " +
                    "system to handle.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (NotSupportedException)
            {
                MessageBox.Show("Error: One or more of the file paths contains invalid " +
                    "characters.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("Error: One or more of the file paths specified was empty.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Error: One or more of the file paths was empty, contained " +
                    "only white spaces, or contained invalid characters.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            // This could be thrown by ListBox.Items.Add():
            catch (SystemException)
            {
                MessageBox.Show("Error: The system ran out of memory while adding files to the " +
                    "list. Try removing a few files and comparing them again.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            // A generic catch, just in case:
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.ToString(), "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            #endregion
        }

        #endregion

        
    }
}