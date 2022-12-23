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
 * 1.5:  No changes to the console apps in this version; version number bumped just to keep in
 * step with the GUI app.
 * 
 * UPDATE August 20, 2009 (1.6):  Added usage of WinHasherCore.ConsoleStatusUpdater to update
 * the console with the current percent complete.
 * 
 * UPDATE June 28, 2011 (1.7):  Updates to address Issue 2, "Output redirection".  Used newly
 * rebuilt core library to do the heavy lifting.  Most of the new logic can be found in
 * WinHasherCore.CmdLineAppUtils.
 * 
 * UPDATE January 7, 2015:  Adding SHA3 support.
 * 
 * This program is Copyright 2016, Jeffrey T. Darlington.
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
using System.Reflection;
using com.gpfcomics.WinHasher.Core;

namespace com.gpfcomics.WinHasher.hashconsole
{
    class Program
    {
        // Get our version number from the assembly:
        private static readonly string version = $"WinHasher v.{Assembly.GetExecutingAssembly().GetName().Version.ToString()}";

        // Our main method, which is pretty simple:
        static int Main(string[] args)
        {
            // Set the console title for a little bit of advertising:
            Console.Title = version;
            // If called with no command-line arguments, print out the usage statement:
            if (args == null || args.Length == 0)
            {
                Usage();
            }
            // Otherwise...
            else
            {
                // Parse the command-line arguments and get back a CmdLineAppArgs object,
                // which puts all our bits into the appropriate buckets:
                CmdLineAppArgs parsedArgs = CmdLineAppUtils.ParseCmdLineArgs(args);
                // If we got anything useful...
                if (parsedArgs == null)
                {
                    // If we didn't get any useful arguments after parsing, that's an error.
                    // Print the usage statement and exit with an error code:
                    Usage();
                    return 1;
                }
                else
                {
                    // Use the common code in the core library to do the actual work.  If
                    // we get a true result from this, just return zero for success.
                    if (!CmdLineAppUtils.PerformHashes(parsedArgs))
                    // If we didn't get a true result, something went wrong.  Print the
                    // usage statement and return a one as an error code.
                    {
                        Usage();
                        return 1;
                    }
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
            // Get our copyright information.  It seems a bit silly to do it this way,
            // but this seems to be the only way to do it that I can find.  We'll pull this
            // from the assembly so we only need to change it in one place, and it can be
            // automatically fetched from SVN.
            string copyright = null;
            object[] obj = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            if (obj != null && obj.Length > 0)
                copyright = ((AssemblyCopyrightAttribute)obj[0]).Copyright;
            Console.WriteLine($"\n{version}");
            if (copyright != null)
                Console.WriteLine(copyright);

            Console.WriteLine(
                @"https://github.com/gpfjeff/winhasher

Usage: hash [-md5|-sha1|-sha224|-sha256|-sha384|-sha512|-ripemd128|-ripemd160|
        -ripemd256|-ripemd320|-whirlpool|-tiger|-gost3411|-sha3-224|-sha3-256
        -sha3-384|-sha3-512]
        [-hex|-hexcaps|-base64|-bubbab] [-compare] [-out outfile [-append]]
        [-in infile | filename1 [filename2 ...]]

WinHasher is a command-line cryptographic hash generator for files.  It
runs in one of two modes:  file hashing and multi-file comparison.  If the
""-compare"" switch is supplied, compare mode is active; otherwise, multiple
input files will be hashed individually and the result for each displayed.

In hashing mode, WinHasher computes the cryptographic hash of the given
file(s) and prints it to the screen in mode similar to the UNIX md5sum,
shasum, and related commands.  With no command-line switches, it computes
the " + HashEngine.GetHashName(HashEngine.DefaultHash) + @" hash and displays it in hexadecimal format.  Various switches
allow you to change to other hashing algorithms, such as MD5, the SHA
family, RIPEMD-160, Whirlpool, and Tiger.  The ""-base64"" switch causes
WinHasher to output hashes in MIME Base64 (RFC 2045) format rather than
hexadecimal, ""-hexcaps"" outputs hexadecimal with all capital letters,
and ""-bubbab"" uses Bubble Babble encoding.  If multiple files are
specified, each file is hashed in turn and the result displayed.

In multi-file comparison mode, WinHasher computes the specified hash for
each file given and compares the results.  If the hash of every file
matches, then all files in the batch are declared to be the same.  If
one or more hashes do not match the others, a warning will be displayed
indicating as such.  In this way, you can determine whether two or more
files share the same contents despite file name, path, and modification
time differences.

The result of the either operation can be sent to a file by using the
""-out"" switch followed by a path to the file.  If you also supply the
""-append"" switch, the result will be appended to the end of the file;
otherwise, the file will be overwritten if it already exists.

You may also specify the list of files to hash or compare by listing
them in a simple text file and using the ""-in"" switch, followed by the
path to the file.  Input files should have the path to each file listed
on a separate line.  Leading and trailing white space will be ignored, as
will any blank lines or lines that contain only white space.  You may place
comments in this file by starting a line with the pound or hash (#)
character; any line that starts with this character will also be ignored.");
        }
    }
}
