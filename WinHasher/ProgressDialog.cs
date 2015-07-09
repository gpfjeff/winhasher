/* ProgressDialog.cs
 * 
 * PROGRAMMER:    Jeffrey T. Darlington
 * DATE:          November 9, 2007
 * PROJECT:       WinHasher GUI application
 * .NET VERSION:  2.0
 * REQUIRES:      com.gpfcomics.WinHasher.Core
 * REQUIRED BY:   (None)
 * 
 * This dialog box appears when a hashing process is initiated by the GUI somehow.  It displays a
 * very simple box indicating that the hash is in progress and presents a progress bar and a Cancel
 * button to allow the user to cancel the process.
 * 
 * Note that when we hash a single file, there's no way (that I know of) to get the progress of
 * the actual hashing process.  System.Security.Cryptography.HashAlgorithm.ComputeHash() does not
 * report any sort of status.  So for hashing a single file, use HashInProgressDialog as a generic
 * status message to the user that the hash is in progress and that they need to be patient.  If you
 * are going to hash multiple files, use this dialog instead instead; with it, you can at least find
 * out how many files have been processed so far.
 * 
 * UPDATED June 18, 2008 (1.3):  Added necessary flags and members to introduce Base64 output as
 * an option along with hexadecimal.
 * 
 * UPDATED February 12, 2009 (1.4):  Added necessary flags and members to introduce abstracted
 * output type methods in HashEngine.
 * 
 * UPDATED June 29, 2015 (1.7):  Changes to default hash and output type values
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
using com.gpfcomics.WinHasher.Core;

namespace com.gpfcomics.WinHasher
{
    /// <summary>
    /// A dialog box for reporting the progress of a multi-file hash comparison and to allow the
    /// user to cancel the process
    /// </summary>
    public partial class ProgressDialog : Form
    {

        /// <summary>
        /// This enumeration allows us to report the status of the hash currently in progress
        /// that is working in the background.
        /// </summary>
        public enum ResultStatus
        {
            NotStarted,
            Running,
            Error,
            Cancelled,
            Success
        }

        #region Private Variables

        /// <summary>
        /// An array of file path strings listing the files to compare
        /// </summary>
        private string[] files;

        /// <summary>
        /// The hash algorithm being used
        /// </summary>
        private Hashes hashAlgorithm = HashEngine.DefaultHash;

        /// <summary>
        /// A System.ComponentModel.BackgroundWorker object which will perform the hashing
        /// operation in the background
        /// </summary>
        private BackgroundWorker bgWorker;

        /// <summary>
        /// The current status of the hashing process
        /// </summary>
        private ResultStatus resultStatus;

        /// <summary>
        /// A boolean flag indicating whether or not all the files match in a comparison
        /// </summary>
        private bool filesMatch;

        /// <summary>
        /// Are we in single-file or multi-file mode?
        /// </summary>
        private bool singleMode;

        /// <summary>
        /// A string containing the hexadecimal hash in single file mode
        /// </summary>
        private string singleHash;

        /// <summary>
        /// The output encoding for the resulting hash
        /// </summary>
        private OutputType outputType = HashEngine.DefaultOutputType;

        #endregion

        #region Public Properties

        /// <summary>
        /// An array of file path strings listing the files to compare
        /// </summary>
        public string[] FileList
        {
            get { return files; }
            set { files = value; }
        }

        /// <summary>
        /// The hash algorithm being used
        /// </summary>
        public Hashes HashAlgorithm
        {
            get { return hashAlgorithm; }
            set { value = hashAlgorithm; }
        }

        /// <summary>
        /// The current status of the hashing process
        /// </summary>
        public ResultStatus Result
        {
            get { return resultStatus; }
        }

        /// <summary>
        /// A boolean flag indicating whether or not all the files match in a comparison
        /// </summary>
        public bool FilesMatch
        {
            get { return filesMatch; }
        }

        /// <summary>
        /// A string containing the hexadecimal hash a the file in single file mode
        /// </summary>
        public string Hash
        {
            get { return singleHash; }
        }

        /// <summary>
        /// The output encoding for the resulting hash. The default is hexadecimal.
        /// </summary>
        public OutputType OutputType
        {
            get { return outputType; }
            set { outputType = value; }
        }

        #endregion

        /// <summary>
        /// Constructs a progress dialog box with the given file path string list and the
        /// specified hashing algorithm
        /// </summary>
        /// <param name="fileList">An array of file path strings to compare</param>
        /// <param name="hashAlgorithm">The hashing algorithm to use in the comparison</param>
        /// <param name="centerInScreen">True to center in the middle of the screen, false to
        /// center around the parent window</param>
        /// <param name="outputType">An enum specifying what format the output string should be
        /// in (hexadecimal, Base64, etc.)</param>
        public ProgressDialog(string[] fileList, Hashes hashAlgorithm, bool centerInScreen,
            OutputType outputType)
        {
            InitializeComponent();
            progressBar1.Value = 0;
            this.files = fileList;
            this.hashAlgorithm = hashAlgorithm;
            resultStatus = ResultStatus.NotStarted;
            filesMatch = false;
            if (centerInScreen) StartPosition = FormStartPosition.CenterScreen;
            else StartPosition = FormStartPosition.CenterParent;
            singleMode = false;
            this.outputType = outputType;
        }

        /// <summary>
        /// Constructs a progress dialog box with the given file path string list and the
        /// specified hashing algorithm
        /// </summary>
        /// <param name="fileList">An array of file path strings to compare</param>
        /// <param name="hashAlgorithm">The hashing algorithm to use in the comparison</param>
        /// <param name="centerInScreen">True to center in the middle of the screen, false to
        /// center around the parent window</param>
        /// <param name="base64">True to return a Base64 encoded string, false for hexadeciaml</param>
        public ProgressDialog(string[] fileList, Hashes hashAlgorithm, bool centerInScreen,
            bool base64)
        {
            InitializeComponent();
            progressBar1.Value = 0;
            this.files = fileList;
            this.hashAlgorithm = hashAlgorithm;
            resultStatus = ResultStatus.NotStarted;
            filesMatch = false;
            if (centerInScreen) StartPosition = FormStartPosition.CenterScreen;
            else StartPosition = FormStartPosition.CenterParent;
            singleMode = false;
            if (base64) outputType = OutputType.Base64;
            else outputType = OutputType.Hex;
        }

        /// <summary>
        /// Constructs a progress dialog box with the given file path string list and the
        /// specified hashing algorithm
        /// </summary>
        /// <param name="fileList">An array of file path strings to compare</param>
        /// <param name="hashAlgorithm">The hashing algorithm to use in the comparison</param>
        /// <param name="centerInScreen">True to center in the middle of the screen, false to
        /// center around the parent window</param>
        public ProgressDialog(string[] fileList, Hashes hashAlgorithm, bool centerInScreen)
            : this(fileList, hashAlgorithm, centerInScreen, HashEngine.DefaultOutputType)
        { }


        /// <summary>
        /// Constructs a progress dialog box with the given file path string list and the
        /// specified hashing algorithm.  The dialog by default is centered in the parent window.
        /// </summary>
        /// <param name="fileList">An array of file path strings to compare</param>
        /// <param name="hashAlgorithm">The hashing algorithm to use in the comparison</param>
        public ProgressDialog(string[] fileList, Hashes hashAlgorithm)
            : this(fileList, hashAlgorithm, false, HashEngine.DefaultOutputType)
        { }

        /// <summary>
        /// Constructs a progress dialog box with the given file path string and the
        /// specified hashing algorithm
        /// </summary>
        /// <param name="filename">A string containing the path to the file to be hashed</param>
        /// <param name="hashAlgorithm">The hashing algorithm to use</param>
        /// <param name="centerInScreen">True to center in the middle of the screen, false to
        /// center around the parent window</param>
        /// <param name="outputType">An enum specifying what format the output string should be
        /// in (hexadecimal, Base64, etc.)</param>
        public ProgressDialog(string filename, Hashes hashAlgorithm, bool centerInScreen,
            OutputType outputType)
        {
            InitializeComponent();
            progressBar1.Value = 0;
            files = new string[1];
            files[0] = filename;
            this.hashAlgorithm = hashAlgorithm;
            resultStatus = ResultStatus.NotStarted;
            filesMatch = false;
            if (centerInScreen) StartPosition = FormStartPosition.CenterScreen;
            else StartPosition = FormStartPosition.CenterParent;
            singleMode = true;
            this.outputType = outputType;
        }

        /// <summary>
        /// Constructs a progress dialog box with the given file path string and the
        /// specified hashing algorithm
        /// </summary>
        /// <param name="filename">A string containing the path to the file to be hashed</param>
        /// <param name="hashAlgorithm">The hashing algorithm to use</param>
        /// <param name="centerInScreen">True to center in the middle of the screen, false to
        /// center around the parent window</param>
        /// <param name="base64">True to return a Base64 encoded string, false for hexadeciaml</param>
        public ProgressDialog(string filename, Hashes hashAlgorithm, bool centerInScreen,
            bool base64)
        {
            InitializeComponent();
            progressBar1.Value = 0;
            files = new string[1];
            files[0] = filename;
            this.hashAlgorithm = hashAlgorithm;
            resultStatus = ResultStatus.NotStarted;
            filesMatch = false;
            if (centerInScreen) StartPosition = FormStartPosition.CenterScreen;
            else StartPosition = FormStartPosition.CenterParent;
            singleMode = true;
            if (base64) outputType = OutputType.Base64;
            else outputType = OutputType.Hex;
        }

        /// <summary>
        /// Constructs a progress dialog box with the given file path string and the
        /// specified hashing algorithm
        /// </summary>
        /// <param name="filename">A string containing the path to the file to be hashed</param>
        /// <param name="hashAlgorithm">The hashing algorithm to use</param>
        /// <param name="centerInScreen">True to center in the middle of the screen, false to
        /// center around the parent window</param>
        public ProgressDialog(string filename, Hashes hashAlgorithm, bool centerInScreen)
            : this(filename, hashAlgorithm, centerInScreen, HashEngine.DefaultOutputType)
        { }

        /// <summary>
        /// Constructs a progress dialog box with the given file path string and the
        /// specified hashing algorithm.  The dialog by default is centered in the parent window.
        /// </summary>
        /// <param name="filename">A string containing the path to the file to be hashed</param>
        /// <param name="hashAlgorithm">The hashing algorithm to use</param>
        public ProgressDialog(string filename, Hashes hashAlgorithm)
            : this(filename, hashAlgorithm, false, HashEngine.DefaultOutputType)
        { }

        #region Event Handlers

        /// <summary>
        /// This event handler is called with the dialog box is shown.  Initiate the hashing
        /// process in the background
        /// </summary>
        /// <param name="sender">The calling object</param>
        /// <param name="e">Any event arguements</param>
        private void ProgressDialog_Shown(object sender, EventArgs e)
        {
            // Lots of things can go wrong:
            try
            {
                // Don't do anything if there are no files to compare in comparison mode:
                if (!singleMode && (files == null || files.Length <= 0))
                {
                    MessageBox.Show("No files were specified.", "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    resultStatus = ResultStatus.Error;
                }
                // Similarly, if in single-file mode, don't do anything if there's no file:
                else if (singleMode && (files == null || files.Length != 1))
                {
                    MessageBox.Show("No file was specified.", "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    resultStatus = ResultStatus.Error;
                }
                // Now let's get to work:
                else
                {
                    // Create the background worker:
                    bgWorker = new BackgroundWorker();
                    // Allow it to report its progress and to be cancelled:
                    bgWorker.WorkerReportsProgress = true;
                    bgWorker.WorkerSupportsCancellation = true;
                    // Set up its event handlers:
                    bgWorker.DoWork += new DoWorkEventHandler(bgWorker_DoWork);
                    bgWorker.RunWorkerCompleted +=
                        new RunWorkerCompletedEventHandler(bgWorker_RunWorkerCompleted);
                    bgWorker.ProgressChanged +=
                        new ProgressChangedEventHandler(bgWorker_ProgressChanged);
                    // Indicate that we're now working and kick off the background job:
                    resultStatus = ResultStatus.Running;
                    bgWorker.RunWorkerAsync();
                }
            }
            // This should be the only exception that could get thrown, and that should only occur
            // if the worker was already busy.  We should never encounter this, but we'll catch
            // it anyway:
            catch (InvalidOperationException)
            {
                MessageBox.Show("The background hashing process is already busy.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Dispose();
            }
            // And just in case there's some other exception, catch it here:
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Dispose();
            }
        }

        /// <summary>
        /// This event handler is called if the cancel button is clicked.  Confirm with the user
        /// that they want to cancel the operation and interrupt it if necessary.
        /// </summary>
        /// <param name="sender">The calling object</param>
        /// <param name="e">Any event arguements</param>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            // If the worker doesn't exist, we obviously shouldn't be doing this:
            if (bgWorker != null)
            {
                // Confirm with the user that they really want to cancel.  Note that the worker
                // will continue to work even while this is being shown.
                if (MessageBox.Show("Are you sure you want to cancel this process?",
                    "Cancel Hash", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    // Send the worker the cancel signal:
                    try
                    {
                        bgWorker.CancelAsync();
                        cancelButton.Enabled = false;
                        resultStatus = ResultStatus.Cancelled;
                    }
                    // If the worker throws an exception, the process can't be cancelled for some
                    // reason.  Make sure to let the user know.
                    catch
                    {
                        MessageBox.Show("The background hashing process cannot be cancelled.",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        #endregion

        #region BackgroundWorker Event Methods

        /// <summary>
        /// When we receive a status report, update the display:
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            Text = "WinHasher Progress: " + e.ProgressPercentage.ToString() + "%";
        }

        /// <summary>
        /// When the worker finishes, grab the result and let the main window know
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // If an error occurred, we need to notify the user:
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                resultStatus = ResultStatus.Error;
            }
            // I haven't seen the cancel button actually reach this, but it's here for
            // completeness:
            else if (e.Cancelled)
            {
                MessageBox.Show("The hash operation has been cancelled.",
                    "Hash Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                resultStatus = ResultStatus.Cancelled;
            }
            else
            {
                // Try and get the result from the event args:
                try
                {
                    // In single file mode, it's the hash string:
                    if (singleMode) singleHash = (string)e.Result;
                    // In comparison mode, it's a boolean flag:
                    else filesMatch = (bool)e.Result;
                    // Let everyone know we're done:
                    resultStatus = ResultStatus.Success;
                }
                // This could throw a couple exceptions.  This one should only occur if the
                // process was cancelled, but I haven't seen it ever get reached yet:
                catch (InvalidOperationException)
                {
                    MessageBox.Show("The hash operation has been cancelled.",
                        "Hash Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    resultStatus = ResultStatus.Cancelled;
                }
                // The other exception is rather obscure and theoretically shouldn't happen.
                // However, we'll catch a generic exception here just in case.
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    resultStatus = ResultStatus.Error;
                }
            }
            Close();
        }

        /// <summary>
        /// Put the background worker to work
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                // We're off and running:
                resultStatus = ResultStatus.Running;
                // What we do depends on what mode we're in.  In single-file mode, start hashing
                // the specified file:
                if (singleMode)
                    e.Result = HashEngine.HashFile(hashAlgorithm, files[0], bgWorker, e, outputType);
                // Otherwise, start comparing the hashes of the files in the list:
                else
                    e.Result = HashEngine.CompareHashes(hashAlgorithm, files, bgWorker, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                resultStatus = ResultStatus.Error;
                e.Cancel = true;
                // Close is commented out here, because if this occurs, we'll be closing the dialog
                // from the background worker thread, not the GUI thread, and that throws an
                // exception.
                //Close();
            }
        }

        #endregion

    }
}