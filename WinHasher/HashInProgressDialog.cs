/* HashInProgress.cs
 * 
 * PROGRAMMER:    Jeffrey T. Darlington
 * DATE:          November 9, 2007
 * PROJECT:       WinHasher GUI application
 * .NET VERSION:  2.0
 * REQUIRES:      com.gpfcomics.WinHasher.Core
 * REQUIRED BY:   (None)
 * 
 * This dialog box appears when a hashing process is initiated by the GUI somehow.  It displays a
 * very simple box indicating that the hash is in progress and presents a Cancel button to allow
 * the user to cancel the process.
 * 
 * Note that when we hash a single file, there's no way (that I know of) to get the progress of
 * the actual hashing process.  System.Security.Cryptography.HashAlgorithm.ComputeHash() does not
 * report any sort of status.  So for hashing a single file, use this dialog as a generic status
 * message to the user that the hash is in progress and that they need to be patient.  If you are
 * going to hash multiple files, use ProgressDialog instead; with it, you can at least find out
 * how many files have been processed so far.
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
    /// A dialog for inform the user that a hash of a single file is in progress, and to allow
    /// them to cancel the process
    /// </summary>
    public partial class HashInProgressDialog : Form
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
        /// The file path to the file being hashed
        /// </summary>
        private string filename;

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
        /// The final resulting hash in hexidecimal format
        /// </summary>
        private string finalHash;

        #endregion

        #region Public Properties

        /// <summary>
        /// The file path to the file being hashed
        /// </summary>
        public string Filename
        {
            get { return filename; }
            set { filename = value; }
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
        /// The final resulting hash in hexidecimal format
        /// </summary>
        public string Hash
        {
            get { return finalHash; }
        }

        #endregion

        /// <summary>
        /// Create a hash-in-progress dialog box to perform the cryptographic hash specified on
        /// a given file
        /// </summary>
        /// <param name="filename">The path to the file to hash</param>
        /// <param name="hashAlgorithm">The hashing algorithm to use</param>
        /// <param name="centerInScreen">True to center in the middle of the screen, false to
        /// center around the parent window</param>
        public HashInProgressDialog(string filename, Hashes hashAlgorithm, bool centerInScreen)
        {
            InitializeComponent();
            this.filename = filename;
            this.hashAlgorithm = hashAlgorithm;
            resultStatus = ResultStatus.NotStarted;
            finalHash = null;
            if (centerInScreen) StartPosition = FormStartPosition.CenterScreen;
            else StartPosition = FormStartPosition.CenterParent;
        }

        /// <summary>
        /// Create a hash-in-progress dialog box to perform the cryptographic hash specified on
        /// a given file
        /// </summary>
        /// <param name="filename">The path to the file to hash</param>
        /// <param name="hashAlgorithm">The hashing algorithm to use</param>
        public HashInProgressDialog(string filename, Hashes hashAlgorithm)
            :
            this(filename, hashAlgorithm, false)
        {
        }

        #region Event Handlers

        /// <summary>
        /// This event handler is called with the dialog box is shown.  Initiate the hashing
        /// process in the background
        /// </summary>
        /// <param name="sender">The calling object</param>
        /// <param name="e">Any event arguements</param>
        private void HashInProgressDialog_Shown(object sender, EventArgs e)
        {
            // Catch anything that might blow up:
            try
            {
                // If the file path is empty or the file does not exist, complain:
                if (filename == null || filename == string.Empty || filename == "" ||
                    !File.Exists(filename))
                {
                    MessageBox.Show("No filename was specified or the specified file did not " +
                        "exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    resultStatus = ResultStatus.Error;
                }
                // This shouldn't happen, but if the hashing algorithm is not set, complain:
                else if (hashAlgorithm == null)
                {
                    MessageBox.Show("No hash algorithm was specified.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    resultStatus = ResultStatus.Error;
                }
                // Otherwise, put the background worker to work:
                else
                {
                    bgWorker = new BackgroundWorker();
                    // Allow the process to be cancelled:
                    bgWorker.WorkerSupportsCancellation = true;
                    // Specify our background worker event handlers:
                    bgWorker.DoWork += new DoWorkEventHandler(bgWorker_DoWork);
                    bgWorker.RunWorkerCompleted +=
                        new RunWorkerCompletedEventHandler(bgWorker_RunWorkerCompleted);
                    // Specify that we're currently running:
                    resultStatus = ResultStatus.Running;
                    // And get to work:
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
                if (MessageBox.Show("Are you sure you want to cancel this hash?", "Cancel Hash",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
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
                        MessageBox.Show("Error: The background hasing process cannot be cancelled.",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        #endregion

        #region BackgroundWorker Event Handlers

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
                MessageBox.Show("Hashing has been cancelled at your request.", "Hash Cancelled",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                resultStatus = ResultStatus.Cancelled;
            }
            else
            {
                // Try and get the hash string from the event args:
                try { finalHash = (string)e.Result; }
                // This could throw a couple exceptions.  This one should only occur if the
                // process was cancelled, but I haven't seen it ever get reached yet:
                catch (InvalidOperationException)
                {
                    MessageBox.Show("Hashing has been cancelled at your request.", "Hash Cancelled",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            // If the hash is good, send it back to the main window and gracefully die:
            if (finalHash != null)
            {
                resultStatus = ResultStatus.Success;
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
                e.Result = HashEngine.HashFile(hashAlgorithm, filename);
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