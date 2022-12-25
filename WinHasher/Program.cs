/* Program.cs
 * 
 * PROGRAMMER:    Jeffrey T. Darlington
 * DATE:          August 31, 2007
 * PROJECT:       WinHasher GUI application
 * .NET VERSION:  2.0
 * REQUIRES:      com.gpfcomics.WinHasher.Core
 * REQUIRED BY:   (None)
 * 
 * The main program for WinHasher.  Unlike some of my other projects, this is a bit more advanced,
 * and for good reason.  The WinHasher GUI app works in one of two ways.
 * 
 * When called with no command-line arguments, WinHasher works like any other Windows app.  It loads
 * its GUI and lets the user work interactively.  This is handy if you're not used to command-line
 * work, or if you want to do multiple drag & drop comparisons.
 * 
 * However, if called from the command line with a set of arguments, the main GUI (usually) doesn't
 * load.  The idea is to put this in the user's "Send To" folder in their Windows Explorer context
 * menu (or even better, find a way to add a dedicated WinHasher submenu; still working on this
 * part).  The first argument should be which hash to use; if none is specified, MD5 is the default.
 * Then all the other arguments are assumed to be file paths.  If only one file is provided, its
 * hash is computed and a dialog box appears showing the hash.  If more than one file is provided,
 * the hash of each file is computed and then all the hashes are compared.  If all the hashes match,
 * all the files are said to be the same; if at least one hash does not match the others, the whole
 * batch is said to be different.  This method of operation is similar to calling the command-line
 * versions of the programs, but with the benefit of just right-clicking one or more files and
 * getting instant results through dialog boxes without going through the main app.
 * 
 * UPDATED June 19, 2008 (1.3):  Added Base64 output and "-base64" command line switch.  Hash
 * switches and the Base64 switch can be entered in either order, but all switches must come
 * before file arguments.  GUI functionality is unaffected.
 * 
 * UPDATED February 12, 2009 (1.4):  Added all-caps hex and Bubble Babble command line options.
 * Default hash is now SHA-1.
 * 
 * UPDATED July 8, 2009 (1.5):  Switched command-line mode single-file hash to using the new
 * ResultDialog class rather than MessageBox to give the user easier access to the hash value
 * and to allow them to compare the hash with a pre-computed value more easily.
 * 
 * UPDATED June 29, 2015 (1.7):  Updates for Bouncy Castle conversion as well as to enable
 * "portable" mode
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
using System.Windows.Forms;
using System.IO;
using com.gpfcomics.WinHasher.Core;

namespace com.gpfcomics.WinHasher
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // Set up the usual GUI stuff first:
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // If no command-line arguments are given, go ahead and run the program in
            // interactive GUI mode:
            if (args.Length == 0)
            {
                Application.Run(new MainForm());
            }
            // If we have one and only one command-line argument and it's the portable
            // flag, load the main application in portable mode (i.e. no writes to the
            // registry):
            else if (args.Length == 1 && args[0].ToLower() == "-portable")
            {
                Application.Run(new MainForm(true));
            }
            // Otherwise, assume we're going to process one or more files, show the results
            // in a dialog box, then exit:
            else
            {
                CmdLineAppArgs parsedArgs = CmdLineAppUtils.ParseCmdLineArgs(args);
                if (parsedArgs != null)
                {
                    if (parsedArgs.Files.Length > 0)
                    {
                        // If we got one file, compute the hash and print it back:
                        if (parsedArgs.Files.Length == 1)
                        {
                            // We could throw some exceptions here, so ignore Yoda's advice and give
                            // it a try:
                            try
                            {
                                // Only do this if the file exists:
                                // TODO: check that in the underlying function ComputeHash
                                if (File.Exists(parsedArgs.Files[0]))
                                {
                                    // Create a new progress dialog.  This does the actual work:
                                    ProgressDialog pd = new ProgressDialog(parsedArgs.Files, parsedArgs.Hash, true, parsedArgs.OutputType);
                                    pd.ShowDialog();
                                    // If we got back a successful result, show the hash.  Otherwise,
                                    // the error message should already be shown so do nothing.
                                    if (pd.Result == ProgressDialog.ResultStatus.Success && pd.Hash != null)
                                    {
                                        ResultDialog rd = new ResultDialog(pd.Hash, parsedArgs.Hash, parsedArgs.OutputType);
                                        rd.ShowDialog();
                                    }
                                }
                                // The file didn't exist:
                                else
                                {
                                    MessageBox.Show("Error: The specified file does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            #region Catch Exceptions
                            // Our hash engine can throw its own exceptions, which usually are just other
                            // exceptions wrapped in our own message format.  We'll look for those first
                            // and foremost:
                            catch (HashEngineException hee)
                            {
                                MessageBox.Show("Error: " + hee.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            // Console.WriteLine() can throw this one:
                            catch (IOException)
                            {
                                MessageBox.Show("Error: An unknown I/O error has occured.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            // A catch-all to handle anything else:
                            catch (Exception ex)
                            {
                                MessageBox.Show("Error: " + ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            #endregion
                        }
                        // If we receive more than one argument, will treat each one as a file path and
                        // reach each one in turn, computing its hash.  The hash of each file is compared
                        // against the hash of the first file.  If all the hashes match, we'll print out
                        // a happy congratulatory message.  If just one of the hashes doesn't match the
                        // others, we say the whole batch fails.
                        else
                        {
                            try
                            {
                                // Create a new progress dialog and show it.  This is where the actual
                                // work will be done.  Note we don't care about the Base64 flag, since
                                // the actual hashes won't be displayed to the user.  (Whether we compare
                                // hex strings or Base64 strings doesn't really matter.)
                                ProgressDialog pd = new ProgressDialog(parsedArgs.Files, parsedArgs.Hash, true);
                                pd.ShowDialog();
                                // If we got a successful result, keep going.  Anything else should have
                                // already thrown an error message.
                                if (pd.Result == ProgressDialog.ResultStatus.Success)
                                {
                                    // If the files matched, congratulate the user:
                                    if (pd.FilesMatch)
                                    {
                                        MessageBox.Show("Congratulations!  All " + parsedArgs.Files.Length + " files match!",
                                            HashEngine.GetHashName(parsedArgs.Hash) + " Hash",
                                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }
                                    // Otherwise, warn them:
                                    else
                                    {
                                        MessageBox.Show("WARNING! One or more of these " + parsedArgs.Files.Length + " files do not match!",
                                            HashEngine.GetHashName(parsedArgs.Hash) + " Hash",
                                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }
                                }
                            }
                            #region Catch Exceptions
                            // Same as above:
                            catch (HashEngineException hee)
                            {
                                MessageBox.Show("Error: " + hee.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            // Console.WriteLine() can throw this one:
                            catch (IOException)
                            {
                                MessageBox.Show("Error: An unknown I/O error has occured.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            // A catch-all to handle anything else:
                            catch (Exception ex)
                            {
                                MessageBox.Show("Error: " + ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            #endregion
                        }
                    }
                    // There were no files left in the list after the switches were exhausted:
                    else
                    {
                        MessageBox.Show("Error: No files specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Error: Unexpected error while parsing the arguments.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}