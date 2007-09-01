/* Program.cs
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
                // Default to doing MD5 unless otherwise instructed:
                Hashes hash = Hashes.MD5;
                string hashString = "MD5";
                // Look at the first argument.  If it starts with a hyphen, we'll take it to
                // be a switch telling us which hash to use.
                if (args[0].StartsWith("-"))
                {
                    // Check again to see if there are no other arguments and print the usage
                    // statement if that's the case:
                    if (args.Length == 1) 
                    {
                        MessageBox.Show("Error: If you specify a hash switch on the command line, " +
                            "you must also specify at least one file to hash.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Application.Run(new MainForm());
                    }

                    // Examine the switch and pick which hash to use:
                    switch (args[0].ToLower())
                    {
                        case "-md5":
                            hash = Hashes.MD5;
                            hashString = "MD5";
                            break;
                        case "-sha1":
                            hash = Hashes.SHA1;
                            hashString = "SHA1";
                            break;
                        case "-sha256":
                            hash = Hashes.SHA256;
                            hashString = "SHA256";
                            break;
                        case "-sha512":
                            hash = Hashes.SHA512;
                            hashString = "SHA512";
                            break;
                        // If we didn't get a valid hash switch, complain, but proceed using
                        // the MD5 default:
                        default:
                            MessageBox.Show("Error: Invalid hash switch. I don't know about \"" +
                                args[0] + "\". Doing MD5 instead.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                    }
                    // Copy the rest of the array into the files array:
                    files = new string[args.Length - 1];
                    Array.Copy(args, 1, files, 0, args.Length - 1);
                }
                // If there was no switch, then we'll assume the rest of the arguments are
                // file paths.  Just make the files array a reference to the args array:
                else
                {
                    files = args;
                }
                // If we got one file, compute the hash and print it back:
                if (files.Length == 1)
                {
                    // We could throw some exceptions here, so ignore Yoda's advice and give
                    // it a try:
                    try
                    {
                        // This should be simple enough:
                        MessageBox.Show(hashString + ": " + HashEngine.HashFile(hash, files[0]),
                            hashString + " Hash", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                        if (HashEngine.CompareHashes(hash, files))
                        {
                            MessageBox.Show("Congratulations!  All " + files.Length + " files match!",
                                hashString + " Hash", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("WARNING! One or more of these " + files.Length + 
                                " files do not match!", hashString + " Hash", MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
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
        }
    }
}