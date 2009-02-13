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
 * UPDATE June 19, 2008 (1.3):  Added -base64 switch and Base64 output option.
 *
 * UPDATE February 12, 2009 (1.4):  Added -hexcaps switch and all-caps hexadcimal output, as well
 * as -bubbab and Bubble Babble.
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
                // Default to doing SHA-1 unless otherwise instructed:
                Hashes hash = Hashes.SHA1;
                // Default to hexadecimal output:
                OutputType outputType = OutputType.Hex;
                // All command line arguments come first, so step through those:
                while (args[0].StartsWith("-"))
                {
                    switch (args[0].ToLower())
                    {
                        // Most of these determine which hash to use:
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
                        case "-whirlpool":
                            hash = Hashes.Whirlpool;
                            break;
                        case "-tiger":
                            hash = Hashes.Tiger;
                            break;
                        // But this one puts us in Base64 mode:
                        case "-base64":
                            outputType = OutputType.Base64;
                            break;
                        // And this outputs hex with capital letters:
                        case "-hexcaps":
                            outputType = OutputType.CapHex;
                            break;
                        // And this outputs Bubble Babble:
                        case "-bubbab":
                            outputType = OutputType.BubbleBabble;
                            break;
                        // If we didn't get a valid switch, complain:
                        default:
                            Console.WriteLine("ERROR: Invalid switch \"" + args[0] + "\"");
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
                            // Display which hash we're running:
                            string hashString = "SHA-1";
                            switch (hash)
                            {
                                case Hashes.MD5:
                                    hashString = "MD5";
                                    break;
                                case Hashes.RIPEMD160:
                                    hashString = "RIPEMD-160";
                                    break;
                                case Hashes.SHA256:
                                    hashString = "SHA-256";
                                    break;
                                case Hashes.SHA384:
                                    hashString = "SHA-384";
                                    break;
                                case Hashes.SHA512:
                                    hashString = "SHA-512";
                                    break;
                                case Hashes.Tiger:
                                    hashString = "Tiger";
                                    break;
                                case Hashes.Whirlpool:
                                    hashString = "Whirlpool";
                                    break;
                                default:
                                    hashString = "SHA-1";
                                    break;
                            }

                            // Print out a warning if they chose MD5:
                            if (hash == Hashes.MD5)
                            {
                                Console.WriteLine();
                                Console.WriteLine("WARNING: MD5 is no longer considered a secure hashing algorithm.  You");
                                Console.WriteLine("may want to consider using a stronger algorithm whenever possible.");
                            }
                            // This should be simple enough:
                            Console.WriteLine();
                            Console.WriteLine(hashString + ": " + HashEngine.HashFile(hash, files[0], outputType));
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
                // If no files were specified after the switches were exhausted, complain:
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("ERROR: No files specified, nothing to do");
                    Usage();
                    return 1;
                }
            }
            return 0;
        }

        /// <summary>
        /// Print out our usage statement. The primary purpose of this is to help the user to learn
        /// how to use the program.
        /// </summary>
        static void Usage()
        {
            Console.WriteLine();
            Console.WriteLine(version);
            Console.WriteLine("Copyright 2009, Jeffrey T. Darlington.  All rights reserved.");
            Console.WriteLine("http://www.gpf-comics.com/dl/winhasher/");
            Console.WriteLine();
            //*****************123456789012345678901234567890123456789012345678901234567890123456789012345
            Console.WriteLine("Usage: hash [-md5|-sha1|-sha256|-sha384|-sha512|-ripemd160|-whirlpool|");
            Console.WriteLine("       -tiger] [-base64|-hexcaps|-bubbab] filename1 [filename2 ...]");
            Console.WriteLine();
            Console.WriteLine("WinHasher is a command-line cryptographic hash generator for files.  It");
            Console.WriteLine("runs in one of two modes:  single file hashing and multi-file comparison.");
            Console.WriteLine();
            Console.WriteLine("In single file mode, WinHasher computes the cryptographic hash of the");
            Console.WriteLine("given file and prints it to the screen.  With no command-line switches,");
            Console.WriteLine("it computes the SHA-1 hash and displays it in hexadecimal format.  Various");
            Console.WriteLine("switches allow you to change to other hashing algorithms, such as MD5,");
            Console.WriteLine("the SHA family, RIPEMD-160, Whirlpool, and Tiger.  The \"-base64\" switch");
            Console.WriteLine("causes WinHasher to output hashes in MIME Base64 (RFC 2045) format rather");
            Console.WriteLine("than hexadecimal, \"-hexcaps\" outputs hexadecimal with all capital letters,");
            Console.WriteLine("and \"-bubbab\" uses Bubble Babble encoding.");
            Console.WriteLine();
            Console.WriteLine("In multi-file comparison mode, WinHasher computes the specified hash for");
            Console.WriteLine("each file given and compares the results.  If the hash of every file");
            Console.WriteLine("matches, then all files in the batch are declared to be the same.  If");
            Console.WriteLine("one or more hashes do not match the others, a warning will be displayed");
            Console.WriteLine("indicating as such.  In this way, you can determine whether two or more");
            Console.WriteLine("files share the same contents despite file name, path, and modification");
            Console.WriteLine("time differences.");
        }
    }
}
