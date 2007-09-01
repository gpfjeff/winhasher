/* Program.cs
 * 
 * PROGRAMMER:    Jeffrey T. Darlington
 * DATE:          August 31, 2007
 * PROJECT:       WinHasher SHA1 console application
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
using System.Text;
using System.IO;
using System.Reflection;
using com.gpfcomics.WinHasher.Core;

namespace com.gpfcomics.WinHasher.sha1console
{
    class Program
    {
        // Get our version number from the assembly:
        private static string version = "WinHasher SHA1 v. " +
            Assembly.GetExecutingAssembly().GetName().Version.ToString();

        // Our main method, which is pretty simple:
        static void Main(string[] args)
        {
            // Set the console title for a little bit of advertising:
            Console.Title = version;
            // If called with no command-line arguments, print out the usage statement:
            if (args.Length == 0) { Usage(); }
            // Treat all arguments as file paths.  If only one argument is specified, assume
            // we are to read in that file, compute the SHA1 hash, and spit out the hex dump
            // to the screen.
            else if (args.Length == 1)
            {
                // We could throw some exceptions here, so ignore Yoda's advice and give
                // it a try:
                try
                {
                    // This should be simple enough:
                    Console.WriteLine();
                    Console.WriteLine(HashEngine.SHA1HashFile(args[0]));
                }
                #region Catch Exceptions
                // Our hash engine can throw its own exceptions, which usually are just other
                // exceptions wrapped in our own message format.  We'll look for those first
                // and foremost:
                catch (HashEngineException hee)
                {
                    Console.WriteLine();
                    Console.WriteLine("ERROR: " + hee.Message);
                    Usage();
                }
                // Console.WriteLine() can throw this one:
                catch (IOException)
                {
                    Console.WriteLine();
                    Console.WriteLine("ERROR: An unknown I/O error has occured.");
                    Usage();
                }
                // A catch-all to handle anything else:
                catch (Exception ex)
                {
                    Console.WriteLine();
                    Console.WriteLine("ERROR: " + ex.ToString());
                    Usage();
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
                    if (HashEngine.CompareSHA1Hashes(args))
                    {
                        Console.WriteLine();
                        Console.WriteLine("Congratulations!  All " + args.Length + " files match!");
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine("WARNING! One or more of these " + args.Length + " files do not match!");
                    }
                }
                #region Catch Exceptions
                // Same as above:
                catch (HashEngineException hee)
                {
                    Console.WriteLine();
                    Console.WriteLine("ERROR: " + hee.Message);
                    Usage();
                }
                catch (IOException)
                {
                    Console.WriteLine();
                    Console.WriteLine("ERROR: An unknown I/O error has occured.");
                    Usage();
                }
                catch (Exception ex)
                {
                    Console.WriteLine();
                    Console.WriteLine("ERROR: " + ex.ToString());
                    Usage();
                }
                #endregion
            }
        }

        // Print out our usage statement. The primary purpose of this is to help the user to learn
        // how to use the program.
        static void Usage()
        {
            Console.WriteLine();
            Console.WriteLine(version);
            Console.WriteLine("Copyright 2007, Jeffrey T. Darlington.  All rights reserved.");
            Console.WriteLine("http://www.gpf-comics.com/dl/winhasher/");
            Console.WriteLine();
            //*****************123456789012345678901234567890123456789012345678901234567890123456789012345
            Console.WriteLine("To use WinHasher SHA1, feed it one more more paths to files.  If the file");
            Console.WriteLine("path contains spaces, make sure to enclose the entire path in quotes.");
            Console.WriteLine("If only one file is specified, its SHA1 hash will be returned.  If more");
            Console.WriteLine("than one file is specified, the SHA1 hash of each file will be computed");
            Console.WriteLine("and then compared.  If all the hashes of all the files match, you will");
            Console.WriteLine("receive a happy notification as such.  If one or more of the hashes do");
            Console.WriteLine("not match the others, a warning will be displayed.");
        }
    }
}
