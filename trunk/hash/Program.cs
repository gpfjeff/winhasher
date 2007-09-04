/* Program.cs
 * 
 * PROGRAMMER:    Jeffrey T. Darlington
 * DATE:          August 31, 2007
 * PROJECT:       WinHasher console application
 * .NET VERSION:  2.0
 * REQUIRES:      com.gpfcomics.WinHasher.Core
 * REQUIRED BY:   (None)
 * 
 * The command-line version of WinHasher.  This console app computes the hash of one or more files.
 * In single file mode, it prints out the hash to the screen.  In multi-file mode, it compares the
 * hash of each file to the hashes of the others:  if all match, all the files are said to be the
 * same; if one or more do not match, the entire batch fails.  Which hash algorithm to use is
 * specified as the first argument switch; if no algorithm is specified, MD5 is the default.
 * This is supposed to be roughly compatible to something like OpenSSL's command-line digest
 * options.  See the usage method for details on using this program.
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

namespace com.gpfcomics.WinHasher.hashconsole
{
    class Program
    {
        // Get our version number from the assembly:
        private static string version = "WinHasher v. " +
            Assembly.GetExecutingAssembly().GetName().Version.ToString();

        // Our main method, which is pretty simple:
        static int Main(string[] args)
        {
            // Set the console title for a little bit of advertising:
            Console.Title = version;
            // If called with no command-line arguments, print out the usage statement:
            if (args.Length == 0) { Usage(); }
            // Otherwise...
            else
            {
                // Start off by declaring a string array to hold a copy of the command line
                // arguments.  We can't work with the argument array itself because we may
                // need to strip off the first one if it's a hash switch.
                string[] files = null;
                // Default to doing MD5 unless otherwise instructed:
                Hashes hash = Hashes.MD5;
                // Look at the first argument.  If it starts with a hyphen, we'll take it to
                // be a switch telling us which hash to use.
                if (args[0].StartsWith("-"))
                {
                    // Check again to see if there are no other arguments and print the usage
                    // statement if that's the case:
                    if (args.Length == 1) { Usage(); return 1; }

                    // Examine the switch and pick which hash to use:
                    switch (args[0].ToLower())
                    {
                        case "-md5":
                            hash = Hashes.MD5;
                            break;
                        case "-sha1":
                            hash = Hashes.SHA1;
                            break;
                        case "-sha256":
                            hash = Hashes.SHA256;
                            break;
                        case "-sha384":
                            hash = Hashes.SHA384;
                            break;
                        case "-sha512":
                            hash = Hashes.SHA512;
                            break;
                        case "-ripemd160":
                            hash = Hashes.RIPEMD160;
                            break;
                        // If we didn't get a valid hash switch, complain, but proceed using
                        // the MD5 default:
                        default:
                            Console.WriteLine();
                            Console.WriteLine("ERROR: Invalid hash switch. I don't know about \"" +
                                args[0] + "\". Doing MD5 instead.");
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
                        Console.WriteLine();
                        Console.WriteLine(HashEngine.HashFile(hash, files[0]));
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
                        return 1;
                    }
                    // Console.WriteLine() can throw this one:
                    catch (IOException)
                    {
                        Console.WriteLine();
                        Console.WriteLine("ERROR: An unknown I/O error has occured.");
                        Usage();
                        return 1;
                    }
                    // A catch-all to handle anything else:
                    catch (Exception ex)
                    {
                        Console.WriteLine();
                        Console.WriteLine("ERROR: " + ex.ToString());
                        Usage();
                        return 1;
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
                            Console.WriteLine();
                            Console.WriteLine("Congratulations!  All " + files.Length + " files match!");
                        }
                        else
                        {
                            Console.WriteLine();
                            Console.WriteLine("WARNING! One or more of these " + files.Length + " files do not match!");
                        }
                    }
                    #region Catch Exceptions
                    // Same as above:
                    catch (HashEngineException hee)
                    {
                        Console.WriteLine();
                        Console.WriteLine("ERROR: " + hee.Message);
                        Usage();
                        return 1;
                    }
                    catch (IOException)
                    {
                        Console.WriteLine();
                        Console.WriteLine("ERROR: An unknown I/O error has occured.");
                        Usage();
                        return 1;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine();
                        Console.WriteLine("ERROR: " + ex.ToString());
                        Usage();
                        return 1;
                    }
                    #endregion
                }
            }
            return 0;
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
            Console.WriteLine("Usage: hash [-md5|-sha1|-sha256|-sha384|-sha512|-ripemd160] filename1");
            Console.WriteLine("       [filename2 ...]");
            Console.WriteLine();
            Console.WriteLine("WinHasher first looks at the first argument to see if it is a switch that");
            Console.WriteLine("indicates which hash to use.  If found, it will use that hash algorithm;");
            Console.WriteLine("if a switch is not found or is otherwise invalid, it will default to MD5.");
            Console.WriteLine("It will then treat the rest of the inputs as paths to files.  If the file");
            Console.WriteLine("path contains spaces, make sure to enclose the entire path in quotes.");
            Console.WriteLine("If only one file is specified, its hash will be returned.  If more");
            Console.WriteLine("than one file is specified, the hash of each file will be computed");
            Console.WriteLine("and then compared.  If all the hashes of all the files match, you will");
            Console.WriteLine("receive a happy notification as such.  If one or more of the hashes do");
            Console.WriteLine("not match the others, a warning will be displayed.");
        }
    }
}
