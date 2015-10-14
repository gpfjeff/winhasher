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
 * UPDATE February 8, 2010 (1.6):  Added "case kludge" for single file hash results.  If the
 * user selects an output type that typically does not have mixed case (currently everything
 * except Base64), pasting a comparison value with the opposite case (i.e. upper-case when
 * lower-case is expected) causes the comparison to fail.  This kludge forces the correct case
 * when a specific case is expected.
 * 
 * UPDATE June 7, 2012 (1.6.1):  Fixes for Issue #4
 * https://github.com/gpfjeff/winhasher/issues/4
 * 
 * UPDATE June 29, 2015 (1.7):  Updates for Bouncy Castle conversion.  Adding GPFUpdateChecker
 * for automatic update checking and downloading.  Added "portable mode" support.
 * 
 * This program is Copyright 2015, Jeffrey T. Darlington.
 * E-mail:  jeff@gpf-comics.com
 * Web:     https://github.com/gpfjeff/winhasher
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
using com.gpfcomics.UpdateChecker;

namespace com.gpfcomics.WinHasher
{
    public partial class MainForm : Form, IUpdateCheckListener
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
        private Hashes hash = HashEngine.DefaultHash;

        /// <summary>
        /// The last selected directory
        /// </summary>
        private string lastDirectory;

        /// <summary>
        /// The ouput encoding for resulting hashes
        /// </summary>
        private OutputType outputType = HashEngine.DefaultOutputType;

        /// <summary>
        /// Whether or not we were launched in "portable" mode.  Portable mode will not write
        /// its settings to the registry when the application closes, and will similarly not
        /// try to read them upon launch.  This defaults to false.
        /// </summary>
        private bool portable = false;

        /// <summary>
        /// The actual <see cref="UpdateChecker"/> object, which will check for WinHasher
        /// updates
        /// </summary>
        private UpdateChecker.UpdateChecker updateChecker = null;

        /// <summary>
        /// This Boolean flag determines whether or not to disable the built-in check for
        /// updates.  This isn't recommended, of course, but a feature nonetheless.
        /// </summary>
        private bool disableUpdateCheck = false;

        /// <summary>
        /// A <see cref="Uri"/> for the official WinHasher updates feed.  The
        /// <see cref="UpdateChecker"/> will use this feed to look for updated versions of
        /// WinHasher.
        /// </summary>
        private Uri updateFeedUri = new Uri(Properties.Resources.UpdateFeedUri);

        /// <summary>
        /// The unique app string for <see cref="UpdateChecker"/> lookups
        /// </summary>
        private string updateFeedAppName = Properties.Resources.UpdateFeedAppName;

        /// <summary>
        /// The last update check timestamp for <see cref="UpdateChecker"/> lookups.  Note
        /// that this defaults to <see cref="DateTime"/>.MinValue, which should force an
        /// update on the first check, but that will be overwritten during initialization.
        /// </summary>
        private DateTime updateFeedLastCheck = DateTime.MinValue;

        /// <summary>
        /// The number of days between update checks, which we will passs to the
        /// <see cref="UpdateChecker"/>.
        /// </summary>
        private int updateInterval = Int32.Parse(Properties.Resources.UpdateIntervalInDays);

        /// <summary>
        /// The alternate download page to download updates.
        /// </summary>
        private string updateAltDownloadPage = Properties.Resources.UpdateAltDownloadPage;

        #endregion

        /// <summary>
        /// Main constructor
        /// </summary>
        public MainForm()
            : this(false)
        { }

        /// <summary>
        /// Main constructor with portable-mode flag
        /// </summary>
        /// <param name="portable">Boolean flag indicating whether or not we should be running in portable mode</param>
        public MainForm(bool portable)
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
            // Build the hash and output type combo boxes:
            hashComboBox.Items.Clear();
            foreach (Hashes hashTemp in Enum.GetValues(typeof(Hashes)))
            {
                hashComboBox.Items.Add(HashEngine.GetHashName(hashTemp));
            }
            outputFormatComboBox.Items.Clear();
            foreach (OutputType otTemp in Enum.GetValues(typeof(OutputType)))
            {
                outputFormatComboBox.Items.Add(HashEngine.GetOutputTypeName(otTemp));
            }
            // Populate the encoding drop-down:
            foreach (EncodingInfo encodingInfo in Encoding.GetEncodings())
            {
                encodingComboBox.Items.Add(encodingInfo.GetEncoding());
            }
            // Set it to show the display name in the dropdown:
            encodingComboBox.DisplayMember = "EncodingName";
            // Take note of whether or not we were called in "portable" mode:
            this.portable = portable;

            // If the user has elected to load the app in portable mode, populate the default settings
            // and disable the Options button (since we can't save any options if we can't write to
            // the registry):
            if (portable)
            {
                PopulateDefaultSettings();
                optionsButton.Enabled = false;
            }
            // If we're not running in portable mode, try to read the settings from the registry and
            // restore them, falling back to the defaults if something fails:
            else
            {
                try
                {
                    // Upgrade any existing settings if necessary:
                    UpgradeSettings();
                    // Try to open the HKCU\Software\GPF Comics\WinHasher key:
                    RegistryKey winHasherSettings =
                        Registry.CurrentUser.OpenSubKey("Software", false).OpenSubKey("GPF Comics",
                        false).OpenSubKey("WinHasher", false);
                    // Try to set the hash combo box:
                    hashComboBox.SelectedItem = (string)winHasherSettings.GetValue("CurrentHash", HashEngine.GetHashName(HashEngine.DefaultHash));
                    hashComboBox_SelectedIndexChanged(null, null);
                    // Try to set the encoding combo box:
                    encodingComboBox.SelectedIndex = (int)winHasherSettings.GetValue("TextEncoding",
                        encodingComboBox.Items.IndexOf(Encoding.Default));
                    // Try to set the output format combo box:
                    outputFormatComboBox.SelectedItem = (string)winHasherSettings.GetValue("OutputType", HashEngine.GetOutputTypeName(HashEngine.DefaultOutputType));
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
                    // Try to restore the disable update check setting:
                    disableUpdateCheck = (int)winHasherSettings.GetValue("DisableUpdateCheck", 0)
                        == 1 ? true : false;
                    // Get the last update check date.  I'm not sure if the try/catch block is
                    // really necessary, but I'm a belt-and-suspenders guy.  Also note that
                    // the default, whether the parse fails for the registry value isn't set,
                    // is DateTime.MinValue, which pretty much guarantees an update check on
                    // the first go-around.
                    try
                    {
                        updateFeedLastCheck =
                            DateTime.Parse((string)winHasherSettings.GetValue("LastUpdateCheck",
                            DateTime.MinValue.ToString()));
                    }
                    catch { updateFeedLastCheck = DateTime.MinValue; }
                    // Close the registry key:
                    winHasherSettings.Close();
                }
                // If any of the registry reading code above fails, restore everything to the
                // default settings:
                catch
                {
                    PopulateDefaultSettings();
                }
            }

            // Set up the file boxes for drag & drop:
            fileSingleTextBox.DragEnter += new DragEventHandler(fileSingleTextBox_DragEnter);
            fileSingleTextBox.DragDrop += new DragEventHandler(fileSingleTextBox_DragDrop);
            compareFilesListBox.DragEnter += new DragEventHandler(fileSingleTextBox_DragEnter);
            compareFilesListBox.DragDrop += new DragEventHandler(compareFilesListBox_DragDrop);
            // Set our title bar to include the version number:
            if (portable) Text = versionShort + " (Portable)";
            else Text = versionShort;
            // Turn on or off the tooltips depending on whether the checkbox has been
            // checked:
            toolTip1.IsBalloon = true;
            if (tooltipsCheckbox.Checked) toolTip1.Active = true;
            else toolTip1.Active = false;

            // Finally, initialize the update checker and set it to work.  The update check
            // should occur in a separate thread, which will allow the main UI thread to
            // continue without any problems.  The entire process *should* be transparent to
            // the user unless an update is actually found.
            if (!disableUpdateCheck)
            {
                try
                {
                    updateChecker = new UpdateChecker.UpdateChecker(updateFeedUri, updateFeedAppName,
                        Assembly.GetExecutingAssembly().GetName().Version, this, updateFeedLastCheck,
                        updateInterval, false);
                    updateChecker.CheckForNewVersion();
                }
                catch
                {
                    MessageBox.Show("An error occurred while trying to perform the update check.  Please try another check later.",
                        "Update Check Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
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
            if (!helpFile.EndsWith(Path.DirectorySeparatorChar.ToString()))
                helpFile += Path.DirectorySeparatorChar.ToString();
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
            ofd.CheckFileExists = true;
            ofd.Multiselect = false;
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
                    // Clear out comparison hash text and the result label.  We could
                    // force the comparison to be rerun, but it would be safer to
                    // clear out the comparison here instead.
                    compareResultLabel.Visible = false;
                    compareToTextBox.Text = "";
                }
                #region Catch Exceptions
                // Most of these are thrown by the FileInfo constructor, although a couple
                // are thrown by its DirectoryName property.  In each case, complain about
                // the problem then restore the GUI to the default state.
                catch (System.Security.SecurityException)
                {
                    MessageBox.Show("You do not have permission to access the file \"" +
                        ofd.FileName + "\".", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    fileSingleTextBox.Text = "";
                    hashSingleButton.Enabled = false;
                    hashSingleTextBox.Text = "";
                }
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show("Access to the file \"" + ofd.FileName +
                        "\" has been denied.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    fileSingleTextBox.Text = "";
                    hashSingleButton.Enabled = false;
                    hashSingleTextBox.Text = "";
                }
                catch (PathTooLongException)
                {
                    MessageBox.Show("The path \"" + ofd.FileName +
                        "\" is too long for the system to handle.", "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    fileSingleTextBox.Text = "";
                    hashSingleButton.Enabled = false;
                    hashSingleTextBox.Text = "";
                }
                catch (NotSupportedException)
                {
                    MessageBox.Show("The path \"" + ofd.FileName +
                        "\" contains invalid characters.", "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    fileSingleTextBox.Text = "";
                    hashSingleButton.Enabled = false;
                    hashSingleTextBox.Text = "";
                }
                catch (ArgumentNullException)
                {
                    MessageBox.Show("The file path specified was empty.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    fileSingleTextBox.Text = "";
                    hashSingleButton.Enabled = false;
                    hashSingleTextBox.Text = "";
                }
                catch (ArgumentException)
                {
                    MessageBox.Show("The file path was empty, contained only white spaces, " +
                        "or contained invalid characters.", "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    fileSingleTextBox.Text = "";
                    hashSingleButton.Enabled = false;
                    hashSingleTextBox.Text = "";
                }
                // A generic catch, just in case:
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK,
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
            // If the file box is empty or doesn't point to a vald fiel name, complain:
            if (String.IsNullOrEmpty(fileSingleTextBox.Text) || !File.Exists(fileSingleTextBox.Text.Trim()))
            {
                MessageBox.Show("Please specify a valid file name in the File to Hash text box.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                hashSingleButton.Enabled = false;
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
                    fileSingleTextBox.Text = fileSingleTextBox.Text.Trim();
                    Refresh();
                    // Create the progress dialog and show it.  This kicks off the
                    // actual hashing process.
                    ProgressDialog pd = new ProgressDialog(fileSingleTextBox.Text, hash, 
                        false, outputType);
                    pd.ShowDialog();
                    // What we do next depends on the result:
                    switch (pd.Result)
                    {
                        // Success:  Display the hash:
                        case ProgressDialog.ResultStatus.Success:
                            hashSingleTextBox.Text = pd.Hash;
                            // If the comparison text box is not empty, rerun the comparison:
                            if (!String.IsNullOrEmpty(compareToTextBox.Text))
                                compareToTextBox_TextChanged(null, null);
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
                    MessageBox.Show(hee.ToString(), "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    fileSingleTextBox.Text = "";
                    hashSingleButton.Enabled = false;
                    hashSingleTextBox.Text = "";
                    UseWaitCursor = false;
                    Refresh();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK,
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
            ofd.CheckFileExists = true;
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
                MessageBox.Show("You must select at least one file before you can " +
                    "remove anything from the list.", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                removeButton.Enabled = false;
                clearButton.Enabled = false;
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
                MessageBox.Show(hee.ToString(), "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                UseWaitCursor = false;
                Refresh();
            }
            // Generic exception catch, just in case:
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK,
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
            if (!String.IsNullOrEmpty(inputTextBox.Text))
            {
                // This one's simple enough:  Grab the selected hash, the text, the selected
                // encoding and output formats, and hash it.  Note that we do *NOT* want to use
                // a Trim() on this, because we're going to consider all text, including all
                // whitespace, as significant.
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
            else MessageBox.Show("No text has been specified, so there's nothing to do.",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// What to do when the Options button has been clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void optionsButton_Click(object sender, EventArgs e)
        {
            // This is pretty simple:  Build the options dialog, pass it a couple values, and show it.
            // If the user clicks OK, grab the user's preference on disabling update checks.  (Everything
            // else is handled within the dialog itself.)
            OptionsDialog od = new OptionsDialog();
            od.ParentDialog = this;
            od.DisableUpdateCheck = disableUpdateCheck;
            od.EnableTooltips = toolTip1.Active;
            od.LastUpdateCheck = updateFeedLastCheck;
            od.UpdateFeedUri = updateFeedUri;
            od.UpdateFeedAppName = updateFeedAppName;
            od.UpdateAltDownloadPage = updateAltDownloadPage;
            od.Version = Assembly.GetExecutingAssembly().GetName().Version;
            if (od.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                disableUpdateCheck = od.DisableUpdateCheck;
                updateFeedLastCheck = od.LastUpdateCheck;
            }
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
            hash = HashEngine.GetHashFromName((string)hashComboBox.SelectedItem);
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
            // A minor improvement on the previous version of this logic.  Rather than simply test
            // to see if the file path text box is empty or not, we now test to see if the file
            // specified in the box actually exists.  If it does, we'll enable the Compute Hash
            // button; otherwise, we'll disable it.  Thus, the button should only be enabled if
            // there's something in the path box worth hashing.
            if (!String.IsNullOrEmpty(fileSingleTextBox.Text) && File.Exists(fileSingleTextBox.Text.Trim()))
                hashSingleButton.Enabled = true;
            else
                hashSingleButton.Enabled = false;
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
            outputType = HashEngine.GetOutputTypeFromName((string)outputFormatComboBox.SelectedItem);
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
            // Only try to write our settings to the registry if we weren't opened in
            // portable mode:
            if (!portable)
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
                                winHasherSettings.SetValue("CurrentHash",
                                    (string)hashComboBox.SelectedItem, RegistryValueKind.String);
                                winHasherSettings.SetValue("TextEncoding",
                                    encodingComboBox.SelectedIndex, RegistryValueKind.DWord);
                                winHasherSettings.SetValue("OutputType",
                                    (string)outputFormatComboBox.SelectedItem, RegistryValueKind.String);
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
                                winHasherSettings.SetValue("DisableUpdateCheck",
                                    (disableUpdateCheck ? 1 : 0),
                                    RegistryValueKind.DWord);
                                winHasherSettings.SetValue("LastUpdateCheck",
                                    updateFeedLastCheck.ToString(), RegistryValueKind.String);
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
            else
            {
                // This is a convenience kludge.  Most websites that post hashes tend to use
                // lower-case hexadecimal, which is why we've set that to our default everywhere.
                // That said, there are sites out there that post hashes in upper-case, which
                // makes it a pain to compare against if our default is lower-case.  Originally,
                // the only way to change the behavior of the Send To shortcuts was to change
                // the command line of the shortcut, which isn't something every user knows how
                // to do.  So this kludge tweaks the comparison string (if set) to force the
                // value to match the case we've specified.  In the default case (lower-case
                // hex), that means forcing the hash to be lower-case, even if pasted in as
                // upper-case.  The same goes for Bubble Babble, which is almost always lower-
                // case, and the inverse is true for our "CapHex" setting (force it to be
                // upper-case).  For *all* instances, we'll tack on a Trim() to remove any
                // excess whitespace on either end; I've lost count of how many times I've copied
                // a hash from a website and it was marked as "no match" just because some
                // extra whitespace was accidentally tacked onto the end.
                if (!String.IsNullOrEmpty(compareToTextBox.Text))
                {
                    switch (outputType)
                    {
                        case OutputType.Hex:
                        case OutputType.BubbleBabble:
                            compareToTextBox.Text = compareToTextBox.Text.Trim().ToLower();
                            break;
                        case OutputType.CapHex:
                            compareToTextBox.Text = compareToTextBox.Text.Trim().ToUpper();
                            break;
                        default:
                            compareToTextBox.Text = compareToTextBox.Text.Trim();
                            break;
                    }
                }
                // If the two strings match, then the generated hash matches the pre-existing
                // hash and the user can safely say the file is unaltered and intact:
                if (String.Compare(hashSingleTextBox.Text, compareToTextBox.Text) == 0)
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
                MessageBox.Show("Only files can be dropped onto this tab.",
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
                    MessageBox.Show("Two many files; only drop one into this box.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            // The data dropped wasn't a file:
            else
            {
                MessageBox.Show("Only files can be dropped onto this tab.",
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
                MessageBox.Show("You do not have permission to access one or more of " +
                    "the files.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Access to one or more of the files has been denied.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (PathTooLongException)
            {
                MessageBox.Show("One or more of the file paths is too long for the " +
                    "system to handle.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (NotSupportedException)
            {
                MessageBox.Show("One or more of the file paths contains invalid " +
                    "characters.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("One or more of the file paths specified was empty.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ArgumentException)
            {
                MessageBox.Show("One or more of the file paths was empty, contained " +
                    "only white spaces, or contained invalid characters.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            // This could be thrown by ListBox.Items.Add():
            catch (SystemException)
            {
                MessageBox.Show("The system ran out of memory while adding files to the " +
                    "list. Try removing a few files and comparing them again.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            // A generic catch, just in case:
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            #endregion
        }

        /// <summary>
        /// Reset all settings to their defaults
        /// </summary>
        private void PopulateDefaultSettings()
        {
            // We used to default to MD5 then to SHA-1 explicitly.  Now we'll drive the default value
            // of the hash drop-down by whatever the default we set in the HashEngine:
            hash = HashEngine.DefaultHash;
            hashComboBox.SelectedItem = HashEngine.GetHashName(hash);
            // Select the system default encoding by default:
            encodingComboBox.SelectedIndex = encodingComboBox.Items.IndexOf(Encoding.Default);
            // Force the output drop-down to the HashEngine default:
            outputType = HashEngine.DefaultOutputType;
            outputFormatComboBox.SelectedItem = HashEngine.GetOutputTypeName(outputType);
            // Set our the last directory to My Documents:
            try { lastDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); }
            catch { lastDirectory = Environment.CurrentDirectory; }
            // Default to the Hash Single File tab:
            modeTabControl.SelectedIndex = 0;
            // Default to tooltips being on:
            tooltipsCheckbox.Checked = true;
            // Always check for updates:
            disableUpdateCheck = false;
            updateFeedLastCheck = DateTime.MinValue;
        }

        /// <summary>
        /// Upgrade old settings values in the Registry from previous versions of WinHasher if necessary.
        /// </summary>
        private void UpgradeSettings()
        {
            // Asbestos underpants:
            try
            {
                // Below are version numbers where major changes have occurred to our settings.  We want
                // to check versions prior to these versions and upgrade the settings accordingly.  To do
                // that, however, we'll need to look at the value of the Version registry entry to see
                // which version last updated the registry settings.  Note that we don *NOT* look at the
                // current version of the running code, which won't help us in this case.
                Version v1_7 = new Version("1.7.0.0");

                // Get the version of the app that last wrote to the registry.  To do that, we'll need to
                // open our registry values first.  Note that unlike the MainForm_FormClosing() method above,
                // we won't try to create our registry keys if they don't exist; all of our tests will then
                // silently fail.  By default, we'll set the "current" version to something ridiculously
                // high so the other tests will fail if we can't read it from the registry.
                Version current = new Version("9999.9999.9999.9999");
                RegistryKey HKCU_Software, GPFComics, winHasherSettings = null;
                HKCU_Software = Registry.CurrentUser.OpenSubKey("Software", false);
                if (HKCU_Software != null)
                {
                    GPFComics = HKCU_Software.OpenSubKey("GPF Comics", false);
                    if (GPFComics != null)
                    {
                        winHasherSettings = GPFComics.OpenSubKey("WinHasher", false);
                        if (winHasherSettings != null)
                        {
                            current = new Version((string)winHasherSettings.GetValue("Version", "9999.9999.9999.9999"));
                        }
                        winHasherSettings.Close();

                        // For version 1.7, we added a number of new hash values, causing the order of those
                        // hashes to change in the drop-down box.  For that, we'll need to tweak the user's
                        // last used hash preference to keep the value they had before.  While we're at it,
                        // we'll convert the index-based registry keys for both the hash and output type to
                        // a string, which will make things easier to deal with if the order of the hashes
                        // moves around in the future.
                        if (current < v1_7)
                        {
                            winHasherSettings = GPFComics.OpenSubKey("WinHasher", true);
                            if (winHasherSettings != null)
                            {
                                // Get the current value of the Selected Hash register setting.  This
                                // will be the old value of the selected index of the hash drop-down box,
                                // which we'll need to convert.  We'll give -1 as the default if we can't
                                // read the value, which will be our cue to skip the conversion as a
                                // failure.
                                int currentHash = (int)winHasherSettings.GetValue("SelectedHash", -1);
                                if (currentHash >= 0)
                                {
                                    /* The mapping from old index values:
                                        MD5         0
                                        SHA-1       1
                                        SHA-256     2
                                        SHA-384     3
                                        SHA-512     4
                                        RIPEMD-160  5
                                        Whirlpool   6
                                        Tiger       7 */

                                    // Based on the old index value, set the new string-based value using
                                    // the hash name:
                                    switch (currentHash)
                                    {
                                        case 0:
                                            winHasherSettings.SetValue("CurrentHash", HashEngine.GetHashName(Hashes.MD5), RegistryValueKind.String);
                                            break;
                                        case 1:
                                            winHasherSettings.SetValue("CurrentHash", HashEngine.GetHashName(Hashes.SHA1), RegistryValueKind.String);
                                            break;
                                        case 2:
                                            winHasherSettings.SetValue("CurrentHash", HashEngine.GetHashName(Hashes.SHA256), RegistryValueKind.String);
                                            break;
                                        case 3:
                                            winHasherSettings.SetValue("CurrentHash", HashEngine.GetHashName(Hashes.SHA384), RegistryValueKind.String);
                                            break;
                                        case 4:
                                            winHasherSettings.SetValue("CurrentHash", HashEngine.GetHashName(Hashes.SHA512), RegistryValueKind.String);
                                            break;
                                        case 5:
                                            winHasherSettings.SetValue("CurrentHash", HashEngine.GetHashName(Hashes.RIPEMD160), RegistryValueKind.String);
                                            break;
                                        case 6:
                                            winHasherSettings.SetValue("CurrentHash", HashEngine.GetHashName(Hashes.Whirlpool), RegistryValueKind.String);
                                            break;
                                        case 7:
                                            winHasherSettings.SetValue("CurrentHash", HashEngine.GetHashName(Hashes.Tiger), RegistryValueKind.String);
                                            break;
                                        default:
                                            winHasherSettings.SetValue("CurrentHash", HashEngine.GetHashName(HashEngine.DefaultHash), RegistryValueKind.String);
                                            break;
                                    }
                                    // Delete the old index-based hash setting value:
                                    winHasherSettings.DeleteValue("SelectedHash");
                                }

                                // For the output type, we'll pretty much to the same thing.  Get the old value,
                                // map it to the string, save that as a new string registry value, then delete
                                // the old key.
                                currentHash = (int)winHasherSettings.GetValue("OutputFormat", -1);
                                if (currentHash >= 0)
                                {
                                    switch (currentHash)
                                    {
                                        case 0:
                                            winHasherSettings.SetValue("OutputType", HashEngine.GetOutputTypeName(OutputType.Hex), RegistryValueKind.String);
                                            break;
                                        case 1:
                                            winHasherSettings.SetValue("OutputType", HashEngine.GetOutputTypeName(OutputType.CapHex), RegistryValueKind.String);
                                            break;
                                        case 2:
                                            winHasherSettings.SetValue("OutputType", HashEngine.GetOutputTypeName(OutputType.Base64), RegistryValueKind.String);
                                            break;
                                        case 3:
                                            winHasherSettings.SetValue("OutputType", HashEngine.GetOutputTypeName(OutputType.BubbleBabble), RegistryValueKind.String);
                                            break;
                                        default:
                                            winHasherSettings.SetValue("OutputType", HashEngine.GetOutputTypeName(HashEngine.DefaultOutputType), RegistryValueKind.String);
                                            break;
                                    }
                                    winHasherSettings.DeleteValue("OutputFormat");
                                }

                                // Create the new disable update check registry flag and assign it the default
                                // value of false (zero).  This is a new registry setting, so it doesn't already exist.
                                winHasherSettings.SetValue("DisableUpdateCheck", 0, RegistryValueKind.DWord);

                                // Create the new last update check registry and assign it the default minimum
                                // value of DateTime, ensuring that the first time this runs, we'll do an update
                                // check.  This is a new registry setting, so it doesn't already exist.
                                winHasherSettings.SetValue("LastUpdateCheck", DateTime.MinValue.ToString(), RegistryValueKind.String);

                                // Close the registry:
                                winHasherSettings.Close();
                            }
                        }

                        // Now that we've done all our tests, close the upper level registry keys and exit:
                        GPFComics.Close();
                    }
                    HKCU_Software.Close();
                }
            }
            // If anything blows up, silently ignore it.  This may not be the best course of action, but
            // we'll change that later if necessary.
            catch { }
        }

        #endregion

        #region IUpdateCheckListener Implementations

        // These methods implement the IUpdateCheckListener interface, which is used to check
        // for updates for WinHasher.  They shouldn't have to actually do much, as the update
        // checker does most of the work.

        /// <summary>
        /// What to do if a new update is found.
        /// </summary>
        void IUpdateCheckListener.OnFoundNewerVersion()
        {
            // This is pretty simple.  If the update check found a new version, tell it to
            // go ahead and download it.  Note that the update checker will handle any user
            // notifications, which includes a prompt on whether or not they'd like to
            // upgrade.  The null check is probably redudant--this method should never be
            // called if the update checker is null--but it's a belt-and-suspenders thing.
            try
            {
                if (updateChecker != null) updateChecker.GetNewerVersion();
            }
            catch
            {
                MessageBox.Show("An error occurred while attempting to download the new version. " +
                    "Please try downloading it again later.", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// What to do if the update checker says to record a new last update check date.
        /// This gets called by the update checker whenever a check is started, whether it
        /// is successful or not.
        /// </summary>
        /// <param name="lastCheck">The new date of the last update check</param>
        void IUpdateCheckListener.OnRecordLastUpdateCheck(DateTime lastCheck)
        {
            // Don't bother recording anything if we're in portable mode:
            if (!portable)
            {
                // Cache the last update check date locally, then try to open the registry and write
                // the date to our registry key.  Note that if this fails, we'll silently ignore the
                // error and the date simply won't be recorded.
                updateFeedLastCheck = lastCheck;
                try
                {
                    RegistryKey winHasherSettings =
                        Registry.CurrentUser.OpenSubKey("Software", false).OpenSubKey("GPF Comics",
                        false).OpenSubKey("WinHasher", true);
                    if (winHasherSettings != null)
                    {
                        winHasherSettings.SetValue("LastUpdateCheck", updateFeedLastCheck.ToString(),
                            RegistryValueKind.String);
                        winHasherSettings.Close();
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// What to do if the update checker wants us to close.  This gets called if the
        /// update check has successfully download the file and now wants to install the
        /// new version.
        /// </summary>
        void IUpdateCheckListener.OnRequestGracefulClose()
        {
            // We don't have a lot to do to close up shop.  Fortunately, we already have
            // a method to do all that stuff, so call it:
            MainForm_FormClosing(null, null);
            Dispose();
        }

        // We don't really care to do anything in the case when no update is found or when an
        // update check ends in an error.  The update checker does well enough on its own in
        // both cases.  Thus, we'll provide empty implementations for both of these call-backs
        // and let the update check handle these items itself.

        void IUpdateCheckListener.OnNoUpdateFound() { }
        void IUpdateCheckListener.OnUpdateCheckError() { }
        void IUpdateCheckListener.OnDownloadCanceled() { }

        #endregion

    }
}