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
        private Hashes hashAlgorithm;

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
        /// A boolean flag indicating whether or not all the files match
        /// </summary>
        private bool filesMatch;

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
        /// A boolean flag indicating whether or not all the files match
        /// </summary>
        public bool FilesMatch
        {
            get { return filesMatch; }
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
        public ProgressDialog(string[] fileList, Hashes hashAlgorithm, bool centerInScreen)
        {
            InitializeComponent();
            progressBar1.Value = 0;
            this.files = fileList;
            this.hashAlgorithm = hashAlgorithm;
            resultStatus = ResultStatus.NotStarted;
            filesMatch = false;
            if (centerInScreen) StartPosition = FormStartPosition.CenterScreen;
            else StartPosition = FormStartPosition.CenterParent;
        }

        /// <summary>
        /// Constructs a progress dialog box with the given file path string list and the
        /// specified hashing algorithm
        /// </summary>
        /// <param name="fileList">An array of file path strings to compare</param>
        /// <param name="hashAlgorithm">The hashing algorithm to use in the comparison</param>
        public ProgressDialog(string[] fileList, Hashes hashAlgorithm)
            :
            this(fileList, hashAlgorithm, false)
        {
        }

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
                // Don't do anything if there are no files to compare:
                if (files == null || files.Length <= 0)
                {
                    MessageBox.Show("No files were specified.", "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    resultStatus = ResultStatus.Error;
                }
                // Likewise, if there's no way to compare them, don't do anything:
                else if (hashAlgorithm == null)
                {
                    MessageBox.Show("No hash algorithm was specified.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("Error: The background rendering process is already busy.", "Error",
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
                if (MessageBox.Show("Are you sure you want to cancel this comparison?",
                    "Cancel Comparison", MessageBoxButtons.YesNo,
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
                        MessageBox.Show("Error: The background hashing process cannot be cancelled.",
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
                MessageBox.Show("The comparison has been cancelled at your request.",
                    "Comparison Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                resultStatus = ResultStatus.Cancelled;
            }
            else
            {
                // Try and get the bitmap from the event args:
                try
                {
                    filesMatch = (bool)e.Result;
                    resultStatus = ResultStatus.Success;
                }
                // This could throw a couple exceptions.  This one should only occur if the
                // process was cancelled, but I haven't seen it ever get reached yet:
                catch (InvalidOperationException)
                {
                    MessageBox.Show("The comparison has been cancelled at your request.",
                        "Comparison Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                resultStatus = ResultStatus.Running;
                e.Result = HashEngine.CompareHashes(hashAlgorithm, files, bgWorker, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                resultStatus = ResultStatus.Error;
                e.Cancel = true;
                Close();
            }
        }

        #endregion

    }
}