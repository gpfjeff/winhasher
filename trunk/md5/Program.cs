/* Program.cs
 * 
 * PROGRAMMER:    Jeffrey T. Darlington
 * DATE:          August 31, 2007
 * PROJECT:       WinHasher MD5 console application
 * .NET VERSION:  2.0
 * REQUIRES:      com.gpfcomics.WinHasher.Core
 * REQUIRED BY:   (None)
 * 
 * The command-line version of WinHasher MD5.  This console app computes the MD5 hash of one or
 * more files.  In single file mode, it prints out the hash to the screen.  In multi-file mode,
 * it compares the hash of each file to the hashes of the others:  if all match, all the files
 * are said to be the same; if one or more do not match, the entire batch fails.   This is
 * supposed to be roughly compatible to something like OpenSSL's command-line MD5 digest option.
 * See the usage method for details on using this program.  For more hashing algorithms, use the
 * command-line "hash" program or the full WinHasher GUI.
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
 * This program is Copyright 2011, Jeffrey T. Darlington.
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

namespace com.gpfcomics.WinHasher.md5console
{
    class Program
    {
        // Get our version number from the assembly:
        private static string version = "WinHasher MD5 v. " + 
            Assembly.GetExecutingAssembly().GetName().Version.ToString();

        // Our main method, which is pretty simple:
        static int Main(string[] args)
        {
            // Set the console title for a little bit of advertising:
            Console.Title = version;
            // If called with no command-line arguments, print out the usage statement:
            if (args == null || args.Length == 0)
            {
                Usage();
                return 0;
            }
            else
            {
                // Parse the command-line arguments and get back a CmdLineAppArgs object,
                // which puts all our bits into the appropriate buckets:
                CmdLineAppArgs parsedArgs = CmdLineAppUtils.ParseCmdLineArgs(args, Hashes.MD5);
                // If we got anything useful...
                if (parsedArgs != null)
                {
                    // Use the common code in the core library to do the actual work.  If
                    // we get a true result from this, just return zero for success.
                    if (CmdLineAppUtils.PerformHashes(parsedArgs, true, Hashes.MD5)) return 0;
                    // If we didn't get a true result, something went wrong.  Print the
                    // usage statement and return a one as an error code.
                    else
                    {
                        Usage();
                        return 1;
                    }
                }
                // If we didn't get any useful arguments after parsing, that's an error.
                // Print the usage statement and exit with an error code:
                else
                {
                    Usage();
                    return 1;
                }
            }
        }

        // Print out our usage statement. The primary purpose of this is to help the user to learn
        // how to use the program.
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
            Console.WriteLine();
            Console.WriteLine(version);
            if (copyright != null) Console.WriteLine(copyright);
            Console.WriteLine("http://code.google.com/p/winhasher/");
            Console.WriteLine();
            //*****************123456789012345678901234567890123456789012345678901234567890123456789012345
            Console.WriteLine("Usage: md5 [-base64|-hexcaps|-bubbab] [-compare] [-out outfile [-append]]");
            Console.WriteLine("       [-in infile | filename1 [filename2 ...]]");
            Console.WriteLine();
            Console.WriteLine("WinHasher MD5 is a command-line MD5 cryptographic hash generator for files.");
            Console.WriteLine("It runs in one of two modes:  file hashing and multi-file comparison.  If the");
            Console.WriteLine("\"-compare\" switch is supplied, compare mode is active; otherwise, multiple");
            Console.WriteLine("input files will be hashed individually and the result for each displayed.");
            Console.WriteLine();
            Console.WriteLine("In hashing mode, WinHasher computes the MD5 hash of the given file(s) and");
            Console.WriteLine("prints it to the screen.  The \"-base64\" switch causes WinHasher to output");
            Console.WriteLine("hashes in MIME Base64 (RFC 2045) format rather than hexadecimal, \"-hexcaps\"");
            Console.WriteLine("outputs hexadecimal with all capital letters, and \"-bubbab\" uses Bubble");
            Console.WriteLine("Babble encoding.  If multiple files are specified, each file is hashed in");
            Console.WriteLine("turn and the result displayed.");
            Console.WriteLine();
            Console.WriteLine("In multi-file comparison mode, WinHasher computes the MD5 hash for each file");
            Console.WriteLine("given and compares the results.  If the hash of every file matches, then all");
            Console.WriteLine("files in the batch are declared to be the same.  If one or more hashes do not");
            Console.WriteLine("match the others, a warning will be displayed indicating as such.  In this");
            Console.WriteLine("way, you can determine whether two or more files share the same contents");
            Console.WriteLine("despite file name, path, and modification time differences.");
            Console.WriteLine();
            Console.WriteLine("The result of the either operation can be sent to a file by using the");
            Console.WriteLine("\"-out\" switch followed by a path to the file.  If you also supply the");
            Console.WriteLine("\"-append\" switch, the result will be appended to the end of the file;");
            Console.WriteLine("otherwise, the file will be overwritten if it already exists.");
            Console.WriteLine();
            Console.WriteLine("You may also specify the list of files to hash or compare by listing");
            Console.WriteLine("them in a simple text file and using the \"-in\" switch, followed by the");
            Console.WriteLine("path to the file.  Input files should have the path to each file listed");
            Console.WriteLine("on a separate line.  Leading and trailing white space will be ignored, as");
            Console.WriteLine("will any blank lines or lines that contain only white space.  You may place");
            Console.WriteLine("comments in this file by starting a line with the pound or hash (#)");
            Console.WriteLine("character; any line that starts with this character will also be ignored.");
            Console.WriteLine();
            Console.WriteLine("WARNING: MD5 is no longer considered secure by serious cryptographers and");
            Console.WriteLine("should be avoided.  If at all possible, you should consider using a stronger");
            Console.WriteLine("hashing algorithm.");
        }
    }
}
