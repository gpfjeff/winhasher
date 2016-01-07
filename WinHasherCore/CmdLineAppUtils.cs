/* CmdLineAppUtils.cs
 * 
 * PROGRAMMER:    Jeffrey T. Darlington
 * DATE:          June 28, 2011
 * PROJECT:       WinHasher Core
 * .NET VERSION:  2.0
 * REQUIRES:      (None)
 * REQUIRED BY:   WinHasher command line utilities (hash, md5, sha1)
 * 
 * Since there is a lot of code reuse among the WinHasher command-line utilities, it made a
 * lot of sense to redesign these programs to consolidate their code into the core library
 * and just have the different utilities reference the common code.  The CmdLineAppUtils
 * class is the result of that effort.  This static class collapses much of what the individual
 * programs did before into a series of common methods, allowing us to reduce those programs
 * down to a few lines.  It also allows us to make changes to all these programs at once by
 * updating the common code.
 * 
 * This file also contains the CmdLineAppArgs class.  CmdLineAppUtils.ParseCmdLineArgs()
 * parsess the command-line arguments to identify the various command switches, input and
 * output files, and so forth.  CmdLineAppArgs provides a wrapper for the parsed data,
 * allowing us to place the different inputs into sorted "buckets" (properties) and pass
 * them around as a single unit.
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
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace com.gpfcomics.WinHasher.Core
{

    /// <summary>
    /// This class encapsulates the parsed command-line arguments used by the various
    /// command-line programs.
    /// </summary>
    public class CmdLineAppArgs
    {
        private Hashes hash = HashEngine.DefaultHash;

        private OutputType outputType = HashEngine.DefaultOutputType;

        private bool compareMode = false;

        private string outputFile = null;

        private bool appendOutput = false;

        private string inputFile = null;

        private string[] files = null;

        /// <summary>
        /// The cryptographic hash algorithm.  Must be a <see cref="Hashes"/> enum value.
        /// The Default is the value of HashEngine.DefaultHash.
        /// </summary>
        public Hashes Hash
        {
            get { return hash; }
            set { hash = value; }
        }

        /// <summary>
        /// The output type. Must  be a <see cref="OutputType"/> enum value.
        /// The Default is the value of HashEngine.DefaultOutputType.
        /// </summary>
        public OutputType OutputType
        {
            get { return outputType; }
            set { outputType = value; }
        }

        /// <summary>
        /// Whether or not we're in compare mode.  Default is false.
        /// </summary>
        public bool CompareMode
        {
            get { return compareMode; }
            set { compareMode = value; }
        }

        /// <summary>
        /// The absolute path to the file where we will write our output.  If this is null
        /// (the default), output will be written to the console rather than to a file.
        /// </summary>
        public string OutputFile
        {
            get { return outputFile; }
            set { outputFile = value; }
        }

        /// <summary>
        /// Whether not to append to an existing output file or overwrite it.  If the output
        /// file does not exist, this flag does nothing, so it's safe to use.  If no output
        /// file has been specified, this flag is meaningless.  Default is false;
        /// </summary>
        public bool AppendOutput
        {
            get { return appendOutput; }
            set { appendOutput = value; }
        }

        /// <summary>
        /// The absolute path to the file where we will read our input.  If this is null
        /// (the default), the file list must be supplied on the command line and will be
        /// stored in the Files property.
        /// </summary>
        public string InputFile
        {
            get { return inputFile; }
            set { inputFile = value; }
        }

        /// <summary>
        /// An array of strings containing the paths to the files we will hash.  If this
        /// value is null (the default), no files were supplied on the command line.
        /// </summary>
        public string[] Files
        {
            get { return files; }
            set { files = value; }
        }

        /// <summary>
        /// Default constructor.  Creates a CmdLineAppArgs object with the default hash and
        /// output type (per HashEngine), compare mode is turned off, and
        /// no input or output files have been specified.
        /// </summary>
        public CmdLineAppArgs() { }

        /// <summary>
        /// Full constructor.  Creates a CmdLineAppArgs object with the specified inputs.
        /// </summary>
        /// <param name="hash">A Hashes enumeration value indicating the hash algorithm to perform</param>
        /// <param name="outputType">An OutputType enumeration value indicating the output encoding</param>
        /// <param name="compareMode">A Boolean flag indicating whether or not we're in comparison mode</param>
        /// <param name="outputFile">A String to an optional output file name</param>
        /// <param name="appendOutput">A Boolean flag indicating whether or not output should be appended
        /// to the output file (true) or if the output file should be overwritten (false)</param>
        /// <param name="inputFile">A String to an optional input file name</param>
        /// <param name="files">A String array containing a list of files to hash.</param>
        public CmdLineAppArgs(Hashes hash, OutputType outputType, bool compareMode,
            string outputFile, bool appendOutput, string inputFile, string[] files)
        {
            this.hash = hash;
            this.outputType = outputType;
            this.compareMode = compareMode;
            this.outputFile = outputFile;
            this.appendOutput = appendOutput;
            this.inputFile = inputFile;
            this.files = files;
        }
    }

    /// <summary>
    /// This static class encapsulates a number of static utility methods used in
    /// common by the various WinHasher command-line utilities.  There is a lot of
    /// code reuse among these programs, so putting chunks of code into the central
    /// library makes a lot of sense.
    /// </summary>
    public static class CmdLineAppUtils
    {
        #region Public Methods

        /// <summary>
        /// Parse the command-line arguments passed into the program and extract out any
        /// options and the file paths to process.
        /// </summary>
        /// <param name="args">A <see cref="String"/> array containing the command-line
        /// arguments passed in to the program's Main() method</param>
        /// <param name="defaultHash">A <see cref="Hashes"/> enum value representing the
        /// default hash algorithm to use if no other hash is specified.</param>
        /// <returns>A <see cref="CmdLineAppArgs"/> object containing the parsed
        /// arguments</returns>
        public static CmdLineAppArgs ParseCmdLineArgs(string[] args, Hashes defaultHash)
        {
            // If no arguments were passed in, there's nothing for us to do.  Return
            // null to signal the caller to print an error or, more likely, the usage
            // statement.
            if (args == null || args.Length == 0) return null;
            // Asbestos underpants:
            try
            {
                // Create a new CmdLineAppArgs object with the default values (SHA-1,
                // lower-case hex output, no compare mode, and no input or output files):
                CmdLineAppArgs parsedArgs = new CmdLineAppArgs();
                // Override the default hash with whatever was given to us in the parse
                // call:
                parsedArgs.Hash = defaultHash;
                // Run through the array and look for option commands.  Note that even
                // though we tested for null above, we need to test for it again because
                // the pop-and-discard step could give us back a null array.
                while (args != null && args.Length > 0 && args[0].StartsWith("-"))
                {
                    // We'll allow upper-case or mixed-case options by forcing the
                    // flag to lower-case:
                    switch (args[0].ToLower())
                    {
                        // Most of these determine which hash to use.  Overwrite the
                        // default when one of these flags are encountered.  Note that
                        // if the program forces a specific hash (like md5 forces MD5),
                        // this could get overridden.
                        case "-md5":
                            parsedArgs.Hash = Hashes.MD5;
                            break;
                        case "-sha1":
                            parsedArgs.Hash = Hashes.SHA1;
                            break;
                        case "-sha224":
                            parsedArgs.Hash = Hashes.SHA224;
                            break;
                        case "-sha256":
                            parsedArgs.Hash = Hashes.SHA256;
                            break;
                        case "-sha384":
                            parsedArgs.Hash = Hashes.SHA384;
                            break;
                        case "-sha512":
                            parsedArgs.Hash = Hashes.SHA512;
                            break;
                        case "-ripemd128":
                            parsedArgs.Hash = Hashes.RIPEMD128;
                            break;
                        case "-ripemd160":
                            parsedArgs.Hash = Hashes.RIPEMD160;
                            break;
                        case "-ripemd256":
                            parsedArgs.Hash = Hashes.RIPEMD256;
                            break;
                        case "-ripemd320":
                            parsedArgs.Hash = Hashes.RIPEMD320;
                            break;
                        case "-whirlpool":
                            parsedArgs.Hash = Hashes.Whirlpool;
                            break;
                        case "-tiger":
                            parsedArgs.Hash = Hashes.Tiger;
                            break;
                        case "-gost3411":
                            parsedArgs.Hash = Hashes.GOST3411;
                            break;
                        case "-sha3-224":
                            parsedArgs.Hash = Hashes.SHA3_224;
                            break;
                        case "-sha3-256":
                            parsedArgs.Hash = Hashes.SHA3_256;
                            break;
                        case "-sha3-384":
                            parsedArgs.Hash = Hashes.SHA3_384;
                            break;
                        case "-sha3-512":
                            parsedArgs.Hash = Hashes.SHA3_512;
                            break;
                        /*
                        case "-shake128":
                            parsedArgs.Hash = Hashes.SHAKE128;
                            break;
                        case "-shake256":
                            parsedArgs.Hash = Hashes.SHAKE256;
                            break;
                         */
                        // But this one puts us in Base64 mode:
                        case "-base64":
                            parsedArgs.OutputType = OutputType.Base64;
                            break;
                        // And this outputs hexadecimal:
                        case "-hex":
                            parsedArgs.OutputType = OutputType.Hex;
                            break;
                        // And this outputs hex with capital letters:
                        case "-hexcaps":
                            parsedArgs.OutputType = OutputType.CapHex;
                            break;
                        // And this outputs Bubble Babble:
                        case "-bubbab":
                            parsedArgs.OutputType = OutputType.BubbleBabble;
                            break;
                        // If we encounter the compare switch, put us in compare mode rather
                        // that hashing each file individually:
                        case "-compare":
                            parsedArgs.CompareMode = true;
                            break;
                        // If the output flag is found, we'll assume the next item on
                        // the list is the name of the file where we'll write our
                        // output.  Note that if no file name is found, this flag
                        // essentially does nothing.
                        case "-out":
                            args = CmdLineAppUtils.PopAndDiscardArray(args);
                            if (args.Length > 0)
                                parsedArgs.OutputFile = args[0];
                            break;
                        // Similarly, if the input switch is encountered, the next item
                        // is assumed to be the input file listing the files to hash:
                        case "-in":
                            args = CmdLineAppUtils.PopAndDiscardArray(args);
                            if (args.Length > 0)
                                parsedArgs.InputFile = args[0];
                            break;
                        // If this flag is set and an output file has been specified, this
                        // will make sure we append to the existing file rather than overwrite
                        // it.  If no output file was specified, this will get silently
                        // ignored.
                        case "-append":
                            parsedArgs.AppendOutput = true;
                            break;
                        // If we didn't get a valid switch, complain:
                        default:
                            Console.Error.WriteLine("ERROR: Invalid switch \"" + args[0] + "\"");
                            break;
                    }
                    // Now shift the array down to the next argument:
                    args = CmdLineAppUtils.PopAndDiscardArray(args);
                }
                // Now that we've processed the switches, make sure there files on the
                // command line to process.  Note that if the "-in" switch was used,
                // it would be valid to have nothing left in this list.
                if (args != null && args.Length > 0)
                {
                    // Loop through the remaining items:
                    for (int i = 0; i < args.Length; i++)
                    {
                        // We're going to officially state that we don't support the
                        // standard redirection symbols (">", ">>", "<", etc.), simply
                        // because they're a pain to work with.  We'll ignore these when
                        // we run through the list of files, but we'll print an error
                        // here if we encounter them.
                        if (IsRedirectionCommand(args[0]))
                            Console.Error.WriteLine("ERROR: Redirection not supported.  Use -input and -output instead.");
                    }
                    // Now add the array as the file list to the return value:
                    parsedArgs.Files = args;
                }
                // Return the parsed arguments:
                return parsedArgs;
            }
            // If anything blew up, return a null value:
            catch { return null; }
        }

        /// <summary>
        /// Parse the command-line arguments passed into the program and extract out any
        /// options and the file paths to process.
        /// </summary>
        /// <param name="args">A <see cref="String"/> array containing the command-line
        /// arguments passed in to the program's Main() method</param>
        /// <returns>A <see cref="CmdLineAppArgs"/> object containing the parsed
        /// arguments</returns>
        public static CmdLineAppArgs ParseCmdLineArgs(string[] args)
        {
            return ParseCmdLineArgs(args, HashEngine.DefaultHash);
        }

        /// <summary>
        /// Perform the actual hash operation
        /// </summary>
        /// <param name="args">A <see cref="CmdLineAppArgs"/> object containing the parsed
        /// input parameters from the command line</param>
        /// <param name="overrideHash">If true, override the hash algorithm specified in
        /// args.Hash with the hash specified by hashToOverrideWith</param>
        /// <param name="hashToOverrideWith">If overrideHash is true, override the hash
        /// algorithm specified in args.Hash with this hash algorithm</param>
        /// <returns></returns>
        public static bool PerformHashes(CmdLineAppArgs args, bool overrideHash,
            Hashes hashToOverrideWith)
        {
            // If the arguments are null, return false to notify the caller that there's
            // nothing to do:
            if (args == null) return false;
            // If we're being told to override the hash, check to make sure that the hash
            // specified in the arguments matches the hash we're supposed to override it
            // with.  If they don't match, print a warning to the user and override the
            // hash.  Otherwise, we'll let this quietly slip by.
            if (overrideHash && args.Hash != hashToOverrideWith)
            {
                Console.Error.WriteLine("WARNING: Can't override hash algorithm, " +
                    GetHashSwitchString(args.Hash) + " switch ignored.");
                args.Hash = hashToOverrideWith;
            }
            // If the file list is empty and the input file parameter was not set, then
            // there's really not much for us to do.  Complain and exit out with a false
            // result.
            if ((args.Files == null || args.Files.Length == 0) &&
                String.IsNullOrEmpty(args.InputFile))
            {
                Console.Error.WriteLine("ERROR: No file list or input file specified. Nothing to do!");
                return false;
            }
            // If the input file was specified but we also got a list of files on the
            // command line, we'll claim the input file overrides the list of files from
            // the arguments and null those out:
            if (!String.IsNullOrEmpty(args.InputFile) && args.Files != null &&
                args.Files.Length > 0)
            {
                Console.Error.WriteLine("WARNING: Input file specified. File list on command line will be ignored.");
                args.Files = null;
            }
            // Now that it looks like we're going to do something, put on our asbestos
            // underpants:
            try
            {
                // If an input file was specified, try to read in its contents:
                if (!String.IsNullOrEmpty(args.InputFile))
                {
                    // Declare a list of strings to hold the intermediate values read
                    // from the file:
                    List<string> fileList = new List<string>();
                    // Now try to open up the file for reading:
                    StreamReader reader = new StreamReader(args.InputFile);
                    // Loop through the lines in the file and examine each one:
                    string line = null;
                    while ((line = reader.ReadLine()) != null)
                    {
                        // For the sake of data integrity, we'll ignore any empty lines
                        // or lines that consist entirely of white space.  As a convenience
                        // to the user, we'll also allow they to put in comments by starting
                        // a line with a hash/pound sign, which we'll also ignore.
                        if (line.Length == 0 || Regex.IsMatch(line, @"^\s*$") ||
                            Regex.IsMatch(line, @"^#"))
                            continue;
                        // If we pass that test, add the contents of the line to the list.
                        // Note that we'll strip any leading or trailing white space while
                        // we're at it.
                        fileList.Add(line.Trim());
                    }
                    // Close the reader:
                    reader.Close();
                    // If we got anything from the file, convert the list into a string
                    // array and put that into the arguments:
                    if (fileList.Count > 0) args.Files = fileList.ToArray();
                }
                // We need to test again whether or not we have anything to do, because
                // reading the input file may have populated the file list where it was
                // empty before.  If no files are listed, bail:
                if (args.Files == null || args.Files.Length == 0)
                {
                    Console.Error.WriteLine("ERROR: No file list specified or input file contained nothing useful. Nothing to do!");
                    return false;
                }
                // Now declare our output stream writer.  If an output file was specified,
                // try to open that file, using the append flag as well.  If no output file
                // was specified, the writer object will remain null, which will be our flag
                // below on where to send our output.
                StreamWriter writer = null;
                if (!String.IsNullOrEmpty(args.OutputFile))
                    writer = new StreamWriter(args.OutputFile, args.AppendOutput);
                // Now to get to work.  First, we need to know what mode we're working in.
                // We'll handle compare mode first:
                if (args.CompareMode)
                {
                    // Compare mode only makes sense if we've got more than one file to
                    // compare.  If not, we need to complain:
                    if (args.Files.Length == 1)
                    {
                        Console.Error.WriteLine("ERROR: Cannot compare files unless two or more files are specified.");
                        return false;
                    }
                    // Print out the initial status message like above:
                    ConsoleStatusUpdater status = null;
                    if (writer == null)
                    {
                        status = new ConsoleStatusUpdater();
                        Console.WriteLine();
                        Console.Write("Comparing " + HashEngine.GetHashName(args.Hash) + " of " +
                            args.Files.Length + " files...   0%");
                    }
                    // Compute the hashes and compare the result.  Note that the
                    // displayed status might be less than 100% if the comparisons
                    // fail.
                    bool isMatch = HashEngine.CompareHashes(args.Hash, args.Files, status);
                    // Print the result to the appropriate place:
                    string result = null;
                    if (isMatch) result = "Congratulations!  All " + args.Files.Length + " files match!";
                    else result = "WARNING! One or more of these " + args.Files.Length + " files do not match!";
                    if (writer == null)
                    {
                        Console.WriteLine();
                        Console.WriteLine(result);
                    }
                    else writer.WriteLine(result);
                }
                // If we're not in compare mode, we must be hashing each file individually:
                else
                {
                    // Loop through the file list:
                    foreach (string file in args.Files)
                    {
                        // We've already warned the user if they tried to use a redirection
                        // command, so ignore those here:
                        if (IsRedirectionCommand(file)) continue;
                        // Try to hash the file.  Note that our actual result here is to
                        // concatenate the result with the file name, giving us a similar
                        // output to md5sum, shasum, and their friends.
                        string result = HashEngine.HashFile(args.Hash, file, args.OutputType,
                                null) + "  " + file;
                        // Write the result to the appropriate output:
                        if (writer != null) writer.WriteLine(result);
                        else Console.WriteLine(result);
                    }
                }
                // Once we're finished, close the file if necessary:
                if (writer != null) writer.Close();
                return true;
            }
            // This needs to be prettied up, but for now just output the message from any
            // exception caught and return false to the caller to indicate an error:
            catch (Exception e)
            {
                Console.Error.WriteLine("ERROR: " + e.Message);
                return false;
            }
        }

        /// <summary>
        /// Parse the command-line arguments passed into the program and extract out any
        /// options and the file paths to process.
        /// </summary>
        /// <param name="args">A <see cref="String"/> array containing the command-line
        /// arguments passed in to the program's Main() method</param>
        public static bool PerformHashes(CmdLineAppArgs args)
        {
            // This is a convenience version of the same method above, only this one does
            // not let us override the user's preferred hash.  No sense reinventing the
            // wheel, so...
            return PerformHashes(args, false, HashEngine.DefaultHash);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Given a <see cref="String"/> array, pop the first element from the front
        /// of the list and discard it, returning the rest of the array.
        /// </summary>
        /// <param name="args">The string array to modify</param>
        /// <returns>The modified string, minus the first element and with the length
        /// reduced by one.  If the array only contains one element, null array is
        /// returned.  If the array is null or empty, it is returned unmodified.</returns>
        private static string[] PopAndDiscardArray(string[] args)
        {
            // If the array is empty or null, there's nothing to do:
            if (args == null || args.Length == 0) return args;
            // If there's only one element in the array, return a null:
            else if (args.Length == 1)
            {
                string[] empty = null;
                return empty;
            }
            // Otherwise, copy all the elements of the original array except the
            // first element into a new array that is one item shorter, then return
            // the new array:
            else
            {
                string[] args2 = new string[args.Length - 1];
                Array.Copy(args, 1, args2, 0, args.Length - 1);
                return args2;
            }
        }


        /// <summary>
        /// Given a <see cref="Hashes"/> item, return a <see cref="String"/> for that item
        /// representing the command line switch used to select that hash.
        /// </summary>
        /// <param name="hash">A Hashes enumeration item</param>
        /// <returns>A string representing the command-line switch used</returns>
        private static string GetHashSwitchString(Hashes hash)
        {
            switch (hash)
            {
                case Hashes.MD5:
                    return "-md5";
                case Hashes.SHA1:
                    return "-sha1";
                case Hashes.SHA224:
                    return "-sha256";
                case Hashes.SHA256:
                    return "-sha256";
                case Hashes.SHA384:
                    return "-sha384";
                case Hashes.SHA512:
                    return "-sha512";
                case Hashes.RIPEMD128:
                    return "-ripemd128";
                case Hashes.RIPEMD160:
                    return "-ripemd160";
                case Hashes.RIPEMD256:
                    return "-ripemd256";
                case Hashes.RIPEMD320:
                    return "-ripemd320";
                case Hashes.Tiger:
                    return "-tiger";
                case Hashes.Whirlpool:
                    return "-whirlpool";
                case Hashes.GOST3411:
                    return "-gost3411";
                case Hashes.SHA3_224:
                    return "-sha3-224";
                case Hashes.SHA3_256:
                    return "-sha3-256";
                case Hashes.SHA3_384:
                    return "-sha3-384";
                case Hashes.SHA3_512:
                    return "-sha3-512";
                /*
                case Hashes.SHAKE128:
                    return "-shake128";
                case Hashes.SHAKE256:
                    return "-shake256";
                 */
                default:
                    return "";
            }
        }

        /// <summary>
        /// Test the specified string and see if it looks like a DOS file redirection
        /// command
        /// </summary>
        /// <param name="arg">The string to test</param>
        /// <returns>True if it looks like a redirection command, false otherwise</returns>
        private static bool IsRedirectionCommand(string arg)
        {
            if (String.IsNullOrEmpty(arg)) return false;
            if (Regex.IsMatch(arg, @"^<") || Regex.IsMatch(arg, @"^\d?>"))
                return true;
            else return false;
        }

        #endregion
    }
}
