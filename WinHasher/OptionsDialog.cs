/* OptionsDialog.cs
 * 
 * PROGRAMMER:    Jeffrey T. Darlington
 * DATE:          July 10, 2015
 * PROJECT:       WinHasher GUI application
 * .NET VERSION:  2.0
 * REQUIRES:      com.gpfcomics.WinHasher.Core
 * REQUIRED BY:   MainForm.cs
 * 
 * This is a new dialog box added for version 1.7.  When WinHasher is not run in portable mode,
 * this dialog allows the user to set a number of options and preferences.  If WinHasher is launched
 * in portable mode, the Options button on the main form is disabled and this dialog becomes inaccessible.
 * 
 * The first option displayed here is the Disable Update Checks checkbox.  By default, update checks are
 * always performed, so this box should be cleared.  If the user checks this box and clicks OK, a setting
 * will be saved to the registry that prevents WinHasher from automatically searching for updates in the
 * future.
 * 
 * Since we had to remove the option to create Send To shortcuts from the installer, we've moved that
 * functionality into the Options Dialog.  Send To shortcuts are always per user, so the user can elect
 * to create these shortcuts for him or herself.  The user is presented with a list of checkboxes, one for
 * each hash; if a shortcut already exists for that hash, the box will be initially checked.  When the user
 * clicks OK, we'll loop through the list and remove any shortcuts whose hashes are unchecked and create new
 * shortcuts for hashes which are checked.  We'll also provide the user with the ability to select the output
 * type for these shortcuts.  On the off-chance that the user gets WinHasher to run on a non-Windows box (say
 * running under Mono), we'll disable the option to create Send To shortcuts, since that's not available on
 * other platforms.
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
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using IWshRuntimeLibrary;
using com.gpfcomics.WinHasher.Core;
using com.gpfcomics.UpdateChecker;

namespace com.gpfcomics.WinHasher
{
    /// <summary>
    /// The options dialog allows the user to tweak some settings that (hopefully) make WinHasher easier to use.  This assumes
    /// the user has installed the application; these options do not apply when the program is executed in portable mode.
    /// </summary>
    public partial class OptionsDialog : Form, IUpdateCheckListener
    {
        /// <summary>
        /// A reference to the main form, so we can ask it to close itself if we successfully download an update
        /// </summary>
        private MainForm parent = null;

        /// <summary>
        /// The path to the user's Send To folder as a string
        /// </summary>
        private string sendToPath = null;

        /// <summary>
        /// Our very own <see cref="UpdateChecker"/> object, through which we'll look for updates.
        /// </summary>
        private UpdateChecker.UpdateChecker updateChecker = null;

        /// <summary>
        /// The date we last checked for updates
        /// </summary>
        private DateTime lastUpdateCheck = DateTime.MinValue;

        /// <summary>
        /// A <see cref="Uri"/> for the official WinHasher updates feed.  The
        /// <see cref="UpdateChecker"/> will use this feed to look for updated versions of
        /// WinHasher.
        /// </summary>
        private Uri updateFeedUri = null;

        /// <summary>
        /// The unique app string for <see cref="UpdateChecker"/> lookups
        /// </summary>
        private string updateFeedAppName = "";

        /// <summary>
        /// The alternate download page to download updates.
        /// </summary>
        private string updateAltDownloadPage = "";

        /// <summary>
        /// Our current app version, used for checking updates
        /// </summary>
        private Version version = null;

        /// <summary>
        /// A reference to the main form, so we can ask it to close itself if we successfully download an update
        /// </summary>
        public MainForm ParentDialog
        {
            get
            {
                return parent;
            }
            set
            {
                parent = value;
            }
        }

        /// <summary>
        /// A Boolean flag indicating whether or not update checks should be disabled.  Setting this value determines the state
        /// of the checkbox in the Options Dialog; getting this value returns the state of that checkbox.
        /// </summary>
        public bool DisableUpdateCheck
        {
            get
            {
                return chkDisableUpdateCheck.Checked;
            }
            set
            {
                chkDisableUpdateCheck.Checked = value;
            }
        }

        /// <summary>
        /// A Boolean flag indicating whether or not tool tips should be displayed.
        /// </summary>
        public bool EnableTooltips
        {
            get
            {
                return toolTip1.Active;
            }
            set
            {
                toolTip1.Active = value;
            }
        }

        /// <summary>
        /// The date we last checked for updates
        /// </summary>
        public DateTime LastUpdateCheck
        {
            get
            {
                return lastUpdateCheck;
            }
            set
            {
                lastUpdateCheck = value;
                lblUpdateCheck.Text = "Last update check:  " + lastUpdateCheck.ToString();
            }
        }

        /// <summary>
        /// A <see cref="Uri"/> for the official WinHasher updates feed.  The
        /// <see cref="UpdateChecker"/> will use this feed to look for updated versions of
        /// WinHasher.
        /// </summary>
        public Uri UpdateFeedUri
        {
            get
            {
                return updateFeedUri;
            }
            set
            {
                updateFeedUri = value;
            }
        }

        /// <summary>
        /// The unique app string for <see cref="UpdateChecker"/> lookups
        /// </summary>
        public string UpdateFeedAppName
        {
            get
            {
                return updateFeedAppName;
            }
            set
            {
                updateFeedAppName = value;
            }
        }

        /// <summary>
        /// The alternate download page to download updates.
        /// </summary>
        public string UpdateAltDownloadPage
        {
            get
            {
                return updateAltDownloadPage;
            }
            set
            {
                updateAltDownloadPage = value;
            }
        }

        /// <summary>
        /// Our current app version, used for checking updates
        /// </summary>
        public Version Version
        {
            get
            {
                return version;
            }
            set
            {
                version = value;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public OptionsDialog()
        {
            try
            {
                InitializeComponent();
                // The state of the Disable Update Checks checkbox should get set by setting the DisableUpdateCheck property.
                // For the rest, we'll need to do some digging.  The Send To shortcuts only make sense on Windows, so just in
                // case, make sure we're currently running on a Windows box before proceeding here.
                if (Environment.OSVersion.Platform == PlatformID.Win32NT
                    || Environment.OSVersion.Platform == PlatformID.Win32Windows)
                {
                    // Convert the path to the Send To folder to a string for our convenience:
                    sendToPath = Environment.GetFolderPath(Environment.SpecialFolder.SendTo);
                    // Set up the output types drop-down.  This is pretty simple, and mimics what we did on the main form.
                    // However, we don't know the current state of any existing shortcuts, so we'll default the output type
                    // drop-down to the default value.
                    comboOutputTypes.Items.Clear();
                    foreach (OutputType otTemp in Enum.GetValues(typeof(OutputType)))
                    {
                        comboOutputTypes.Items.Add(HashEngine.GetOutputTypeName(otTemp));
                    }
                    comboOutputTypes.SelectedItem = HashEngine.GetOutputTypeName(HashEngine.DefaultOutputType);
                    // Set up the list of shortcuts as a checkbox list box.  What we'll do is loop through the available hash
                    // algorithms and, for each one, determine whether or not a shortcut already exists for it.  That will decide
                    // whether or not the checkbox will be checked initially.  Once we know that, create an item in the list for
                    // that hash and set its initial checked state.
                    listSentToShortcuts.Items.Clear();
                    foreach (Hashes hashTemp in Enum.GetValues(typeof(Hashes)))
                    {
                        string shortcutFile = sendToPath + Path.DirectorySeparatorChar.ToString() + HashEngine.GetHashName(hashTemp) + ".lnk";
                        bool isChecked = System.IO.File.Exists(shortcutFile);
                        listSentToShortcuts.Items.Add(HashEngine.GetHashName(hashTemp), isChecked);
                    }
                }
                // If we're not running on Windows, disable the Send To shortcut controls so the user can't do anything
                // with them:
                else
                {
                    listSentToShortcuts.Enabled = false;
                    comboOutputTypes.Enabled = false;
                    toolTip1.SetToolTip(listSentToShortcuts, "Send To shortcuts are not supported on your operating system.");
                    toolTip1.SetToolTip(comboOutputTypes, "Send To shortcuts are not supported on your operating system.");
                }
                // If enabled, make our tooltips look like nifty balloons:
                toolTip1.IsBalloon = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was a problem trying to build the options dialog box.\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// What to do when the OK button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void okButton_Click(object sender, EventArgs e)
        {
            try
            {
                // The disable update checks setting will be passed through the DisableUpdateCheck property.  For the Send To
                // shortcuts, those only make sense if we're running on Windows:
                if (Environment.OSVersion.Platform == PlatformID.Win32NT
                    || Environment.OSVersion.Platform == PlatformID.Win32Windows)
                {
                    // Determine the command-line flag for output types.  Get the current value of the drop-down and switch on
                    // it to determine which flag to add.
                    string outputType = null;
                    switch (HashEngine.GetOutputTypeFromName((string)comboOutputTypes.SelectedItem))
                    {
                        case OutputType.CapHex:
                            outputType = " -hexcaps";
                            break;
                        case OutputType.Base64:
                            outputType = " -base64";
                            break;
                        case OutputType.BubbleBabble:
                            outputType = " -bubbab";
                            break;
                        case OutputType.Hex:
                        default:
                            outputType = "";
                            break;
                    }
                    // Here's where things get funky.  For creating shortcuts, we'll need to dip into the Windows Shell,
                    // so create our interface to it first:
                    WshShellClass wsh = new WshShellClass();
                    // We're going have to look at every hash in the list, so we might as well do a for loop here:
                    for (int i = 0; i < listSentToShortcuts.Items.Count; i++)
                    {
                        // Get the name of the hash as a string, as well as the full path to the shortcut file we plan to
                        // create or delete:
                        string hash = (string)listSentToShortcuts.Items[i];
                        string shortcutFile = sendToPath + Path.DirectorySeparatorChar.ToString() + hash + ".lnk";
                        // Always delete the existing shortcut if it exists.  The user may have changed the output type,
                        // and there's no good way to check for that.
                        if (System.IO.File.Exists(shortcutFile)) System.IO.File.Delete(shortcutFile);
                        // If the hash's checkbox was checked, we'll create the shortcut for that hash.  Most of this is
                        // pretty straightforward.  The funkiest part is probably the shortcut arguments, which is where we
                        // tell WinHasher which hash to use.  The command-line argument is usually the hash name in lower case
                        // minus any non-alphanumeric characters and with a UNIX-style dash ("-") on the front of it.  We'll tack
                        // the output type onto the end of that before we assign it to the Arguments property.  The WorkingDirectory
                        // is not really necessary as the Send To shortcut will send WinHasher the full path to the file, but we'll
                        // default it to My Documents, just in case.  One gotcha on the TargetPath:  If WinHasher is run through
                        // Visual Studio, this will point to the Visual Studio Hosting Process version of the file, not the final
                        // EXE.  This should work as intended whenever the program is run by itself.  The icon path should point
                        // to the WinHasher executable; in the Visual Studio Hosting Process scenario mentioned above, this means
                        // we won't get a nice icon on our shortcut, but it'll work otherwise.
                        if (listSentToShortcuts.GetItemChecked(i))
                        {
                            IWshShortcut shortcut = (IWshShortcut)(wsh.CreateShortcut(shortcutFile));
                            shortcut.TargetPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                            string hashArg = "-" + Regex.Replace(hash.ToLower(), @"\W", "");
                            shortcut.Arguments = hashArg + outputType;
                            shortcut.WindowStyle = 1;
                            shortcut.Description = "Compute the " + hash + " hash for the selected file(s)";
                            shortcut.WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                            shortcut.IconLocation = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                            shortcut.Save();
                        }
                    }
                }
                // Let the caller know we clicked the OK button:
                DialogResult = System.Windows.Forms.DialogResult.OK;
            }
            // We should probably pretty up this erorr message, but for now we'll just include the exception information:
            catch (Exception ex)
            {
                MessageBox.Show("The Send To shortcuts could not be created.\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            // Dispose of the form:
            Dispose();
        }

        /// <summary>
        /// What to do if the Cancel button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            // The main thing we want to do here is notify the caller that the user clicked Cancel, and dispose of the
            // form.  We won't do anything with any changes the user may have made to the shortcuts.  It'll be up to the
            // caller to make sure that the disable update checks option is ignored.
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Dispose();
        }

        /// <summary>
        /// What to do when the Check for Updates Now button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCheckForUpdates_Click(object sender, EventArgs e)
        {
            try
            {
                // Disable the Check for Updates button so it can't be clicked multiple times:
                btnCheckForUpdates.Enabled = false;
                // Launch the update checker.  Since this time we're calling it on demand, the last update check and
                // update interval are set to values that ensure that we'll always get the update checker to run.
                updateChecker = new UpdateChecker.UpdateChecker(updateFeedUri, updateFeedAppName, version, this,
                    DateTime.MinValue, 1, false);
                updateChecker.CheckForNewVersion();
            }
            catch (Exception)
            {
                MessageBox.Show("I was unable to check for updates.  Please try again later.", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                btnCheckForUpdates.Enabled = true;
            }
        }

        #region IUpdateCheckListner Implementation

        // These methods implement the IUpdateCheckListener interface, which is used to check
        // for updates for WinHasher.  They shouldn't have to actually do much, as the update
        // checker does most of the work.

        /// <summary>
        /// What to do when the download has been cancelled
        /// </summary>
        void IUpdateCheckListener.OnDownloadCanceled()
        {
            // Re-enable the Check for Updates button:
            btnCheckForUpdates.Enabled = true;
        }

        /// <summary>
        /// What to do if a new update is found.
        /// </summary>
        void IUpdateCheckListener.OnFoundNewerVersion()
        {
            try
            {
                // Pure and simple:  Tell the updater to get the new version:
                if (updateChecker != null) updateChecker.GetNewerVersion();
            }
            catch (Exception)
            {
                MessageBox.Show("I was unable to download the latest update.  Please try again later.", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// What to do if no update was found
        /// </summary>
        void IUpdateCheckListener.OnNoUpdateFound()
        {
            // I can't remember if the UpdateChecker displays its own dialog box here, or if
            // we need to do so.  I'll leave this in for now until I can test the update
            // process thoroughly.
            MessageBox.Show("You appear to have the latest version of WinHasher.  Please try again later.", "Update Check",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            // Re-enable the Check for Updates button:
            btnCheckForUpdates.Enabled = true;
        }

        /// <summary>
        /// What to do if the update checker says to record a new last update check date.
        /// This gets called by the update checker whenever a check is started, whether it
        /// is successful or not.
        /// </summary>
        /// <param name="lastCheck">The new date of the last update check</param>
        void IUpdateCheckListener.OnRecordLastUpdateCheck(DateTime lastCheck)
        {
            try
            {
                // Take note of the new update check date.  Unfortunately, we can't really
                // update the label that displays the date, since this code is actually called
                // by the BackgroundWorker thread in the UpdateChecker, so we can't directly
                // touch the UI.
                lastUpdateCheck = lastCheck;
                //lblUpdateCheck.Text = "Last update check:  " + lastUpdateCheck.ToString();
            }
            catch (Exception)
            {
                MessageBox.Show("I was unable to check for updates.  Please try again later.", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// What to do if the update checker wants us to close.  This gets called if the
        /// update check has successfully download the file and now wants to install the
        /// new version.
        /// </summary>
        void IUpdateCheckListener.OnRequestGracefulClose()
        {
            // I'm not sure if this will actually work, but try to get our parent form
            // to close itself:
            if (parent != null)
            {
                parent.Close();
                parent.Dispose();
            }
        }

        // We don't really care what happens when an error occurs.  The update checker displays
        // its own error dialog, so adding another one here would be redundant.

        void IUpdateCheckListener.OnUpdateCheckError() { }

        #endregion
    }
}
