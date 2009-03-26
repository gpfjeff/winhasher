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
using System.Windows.Forms;
using System.Text;
using System.IO;
using System.Reflection;
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
            if (args.Length == 0) { Application.Run(new MainForm()); }
            // Otherwise, assume we're going to process one or more files, show the results
            // in a dialog box, then exit:
            else
            {
                // Start off by declaring a string array to hold a copy of the command line
                // arguments.  We can't work with the argument array itself because we may
                // need to strip off the first one if it's a hash switch.
                string[] files = null;
                // Default to doing SHA-1 unless otherwise instructed:
                Hashes hash = Hashes.SHA1;
                string hashString = "SHA-1";
                // By default, output hex:
                OutputType outputType = OutputType.Hex;
                // All our command switches come first, so step through them:
                while (args[0].StartsWith("-"))
                {
                    // Examine the switch:
                    switch (args[0].ToLower())
                    {
                        // Most of these determine which hash to use:
                        case "-md5":
                            hash = Hashes.MD5;
                            hashString = "MD5";
                            break;
                        case "-sha1":
                            hash = Hashes.SHA1;
                            hashString = "SHA-1";
                            break;
                        case "-sha256":
                            hash = Hashes.SHA256;
                            hashString = "SHA-256";
                            break;
                        case "-sha512":
                            hash = Hashes.SHA512;
                            hashString = "SHA-512";
                            break;
                        case "-ripemd106":
                            hash = Hashes.RIPEMD160;
                            hashString = "RIPEMD-160";
                            break;
                        case "-whirlpool":
                            hash = Hashes.Whirlpool;
                            hashString = "Whirlpool";
                            break;
                        case "-tiger":
                            hash = Hashes.Tiger;
                            hashString = "Tiger";
                            break;
                        // But this switch enables Base64 hashing:
                        case "-base64":
                            outputType = OutputType.Base64;
                            break;
                        // And this puts us in all-caps hex mode:
                        case "-hexcaps":
                            outputType = OutputType.CapHex;
                            break;
                        // And this puts us in Bubble Babble mode:
                        case "-bubbab":
                            outputType = OutputType.BubbleBabble;
                            break;
                        // If we didn't get a valid hash switch, complain:
                        default:
                            MessageBox.Show("Invalid switch \"" + args[0] + "\"", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                    }
                    // Now shift the array down to the next argument.  I wish there was a better,
                    // more efficient way of doing this (like a Perl or PHP shift()), but this is
                    // all I know of using simple arrays:
                    string[] args2 = new string[args.Length - 1];
                    Array.Copy(args, 1, args2, 0, args.Length - 1);
                    args = args2;
                }
                // By now, all our switches should be exhausted.  We should only have strings
                // not starting with hyphens, which we'll interpret as file path strings.
                // Only proceed if we have files to work with:
                files = args;
                if (files.Length > 0)
                {
                    // If we got one file, compute the hash and print it back:
                    if (files.Length == 1)
                    {
                        // We could throw some exceptions here, so ignore Yoda's advice and give
                        // it a try:
                        try
                        {
                            // Only do this if the file exists:
                            if (File.Exists(files[0]))
                            {
                                // Create a new progress dialog.  This does the actual work:
                                ProgressDialog pd = new ProgressDialog(files[0], hash, true,
                                    outputType);
                                pd.ShowDialog();
                                // If we got back a successful result, show the hash.  Otherwise,
                                // the error message should already be shown so do nothing.
                                if (pd.Result == ProgressDialog.ResultStatus.Success && pd.Hash != null)
                                    MessageBox.Show(hashString + ": " + pd.Hash, hashString + " Hash",
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            // The file didn't exist:
                            else
                            {
                                MessageBox.Show("Error: The specified file does not exist.", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        #region Catch Exceptions
                        // Our hash engine can throw its own exceptions, which usually are just other
                        // exceptions wrapped in our own message format.  We'll look for those first
                        // and foremost:
                        catch (HashEngineException hee)
                        {
                            MessageBox.Show("Error: " + hee.Message, "Error", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                        }
                        // Console.WriteLine() can throw this one:
                        catch (IOException)
                        {
                            MessageBox.Show("Error: An unknown I/O error has occured.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        // A catch-all to handle anything else:
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error: " + ex.ToString(), "Error", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
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
                            ProgressDialog pd = new ProgressDialog(files, hash, true);
                            pd.ShowDialog();
                            // If we got a successful result, keep going.  Anything else should have
                            // already thrown an error message.
                            if (pd.Result == ProgressDialog.ResultStatus.Success)
                            {
                                // If the files matched, congratulate the user:
                                if (pd.FilesMatch)
                                {
                                    MessageBox.Show("Congratulations!  All " + files.Length +
                                        " files match!", hashString + " Hash", MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                                }
                                // Otherwise, warn them:
                                else
                                {
                                    MessageBox.Show("WARNING! One or more of these " + files.Length +
                                        " files do not match!", hashString + " Hash",
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                            }
                        }
                        #region Catch Exceptions
                        // Same as above:
                        catch (HashEngineException hee)
                        {
                            MessageBox.Show("Error: " + hee.Message, "Error", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                        }
                        // Console.WriteLine() can throw this one:
                        catch (IOException)
                        {
                            MessageBox.Show("Error: An unknown I/O error has occured.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        // A catch-all to handle anything else:
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error: " + ex.ToString(), "Error", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                        }
                        #endregion
                    }
                }
                // There were no files left in the list after the switches were exhausted:
                else
                {
                    MessageBox.Show("Error: No files specified", "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }
    }
}