/* HashEngine.cs
 * 
 * PROGRAMMER:    Jeffrey T. Darlington
 * DATE:          August 31, 2007
 * PROJECT:       WinHasher core library
 * .NET VERSION:  2.0
 * REQUIRES:      See using statements
 * REQUIRED BY:   All WinHasher programs
 * 
 * This is the core library for the WinHasher family of programs.  It provides the core functionality,
 * including all shared methods and enumerations.  In this fashion, all the programs will operate
 * the same regardless of how they are called.  In fact, this implementation is actually pretty
 * simple; it would have been just as easy to put this into each of the programs individually, as it
 * just wraps around some pretty standard .NET built-in stuff.  However, this also promotes code
 * reuse, and if something needs to be changed all the programs change together.
 * 
 * This is implemented primarily as a single static class that is called like a library.  There's
 * no need to instantiate it; just call the static method and go.  There are three primary public-
 * facing parts--an enumeration and two methods--and a series of convenience methods for common
 * hashing tasks.  The three big ones are:
 * 
 *      o   The Hashes enumeration, which lets callers know which hashes the engine supports.
 *      o   The HashFile() method, which takes a Hashes item and a string representing the path to
 *          the file to hash.  The method opens the specified file and computes its hash using the
 *          algorithm specified.  It returns a string containing the hexadecimal representation of
 *          the computed hash.
 *      o   The CompareHashes() method, which takes a Hashes item and a string array.  Again, the
 *          Hashes item specifies the algorithm, while the string array lists the paths to the files
 *          to hash.  The method reads each file, hashes it, and compares all the hashes to see if
 *          they match.  This is an all or nothing deal; either all match or all fail.  This returns
 *          a Boolean value:  true = pass, false = fail.
 * 
 * The other methods contained in here provide convenience access to the MD5 and SHA1 hashes, as
 * they are by far the most popular.  It's likely that I'll eventually provide convenience methods
 * to all the hashes, but there's really no need.
 * 
 * Note that all methods have a chance to throw a HashEngineException.  Essentially, this just
 * wraps any other exception that could be thrown and gives us a relatively friendly error message
 * we can display.  When calling one of these, make sure to catch this exception.
 * 
 * Also note that the two methods of each group that take the most arguments include a
 * System.ComponentModel.BackgroundWorker object and a System.ComponentModel.DoWorkEventArgs object.
 * This allows us to report status when this is called from a BackgroundWorker process.  This way,
 * the GUI can be more responsive instead of appearing to lock up on large files or comparing
 * numerous files.  If both of these objects are null (the default for methods that don't specify
 * them), no kind of reporting is performed.  Note, however, that no real progress gets reported
 * during HashFile() calls, and CompareHashes() only reports how many files have been fully processed.
 * This is because System.Security.Cryptography.HashAlgorithm.ComputeHash() does not provide any
 * kind of progress itself, so there's no way to tell how far we are into a given file.  We only
 * know the file is currently being checked, or when comparing files how many files we've finished
 * out of the list.
 *  
 * UPDATE June 18, 2008 (1.3):  Abstracted the byte-array-to-hex-string code into a new private
 * method, then added a byte-array-to-Base64-string method as well.  Then added a flag to all
 * the hashing methods to allow the choice between hex output and Base64.  Also added the HashText()
 * methods to take a string and a text encoding and return the associated hash.
 * 
 * UPDATE February 12, 2009 (1.4):  Added OutputType enumeration to (hopefully) abstract the output
 * type beyond just hexadecimal and Base64.  Old methods with which used a Boolean flag to
 * differentiate should continue to function in the same capacity but should be considered
 * depreciated.  Also added Bubble Babble output type, ported from Perl's Digest::BubbleBabble
 * by Benjamin Trott from CPAN.
 * 
 * UPDATE July 8-9, 2009 (1.5):  Changed internal variable bytesSoFar in primary HashFile() method
 * from an 32-bit int to a 64-bit long.  I noticed that files greater than 2GB in size (specifically
 * 2,147,483,647 bytes, or System.Int32.MaxValue) would cause the hashing engine to crash when
 * the progress through the file exceeded the 2GB barrier.  This was because this variable would
 * overflow and exceed its boundaries. totalBytesSoFar, the variable holding the sum of file
 * lengths in a multi-file comparison, was already a long, but for some reason I didn't think to
 * do this for single files.  Of course, this means that a total of 8EB (exabytes) can be compared,
 * either as a single file or as a sum of compared files, but for now that's better than blowing
 * up around 2GB.  Also added error checks to make sure these byte lengths (both total for multi-
 * file hashes or any given single file) does not exceed the 8EB barrier.  If it does, we'll
 * throw a HashEngineException that the calling code must catch.
 * 
 * UPDATE August 20, 2009 (1.6):  Added overloads that include new ConsoleStatusUpdater object so
 * we can so the progress of long hashes performed on the command line.
 * 
 * UPDATE June 29, 2015:  Switching from Microsoft's crypto API code to Legion of the Bouncy
 * Castle's so we can support a larger number of hashes, which will hopefully open us up to
 * even more hashes (such as SHA-3) once the BC updates in the future.  Also added new Default
 * Hash and Default Output Type properties and some public static methods for translating hashes
 * and output types to display-ready strings and back again.  (These should eliminate a large number
 * of duplicated switch cases where display values and enumerations gets swapped, and again helps
 * with the future addition of new hashes.)
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
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;

namespace com.gpfcomics.WinHasher.Core
{
    /// <summary>
    /// An enumeration of the hashes this library supports.
    /// </summary>
    public enum Hashes
    {
        MD5,
        SHA1,
        SHA224,
        SHA256,
        SHA384,
        SHA512,
        RIPEMD128,
        RIPEMD160,
        RIPEMD256,
        RIPEMD320,
        Whirlpool,
        Tiger,
        GOST3411
    }

    /// <summary>
    /// An enumeration of the output types this library supports.
    /// </summary>
    public enum OutputType
    {
        Hex,
        CapHex,
        Base64,
        BubbleBabble
    }

    /// <summary>
    /// The hash engine core.  Note that this is a static class, and it should be used like a
    /// utility library.  There's no need to create instances; just call the static methods.
    /// </summary>
    public static class HashEngine
    {
        /// <summary>
        /// A <see cref="FileStream"/> object, pulled out into the class for potential reuse
        /// </summary>
        private static FileStream fs;

        /// <summary>
        /// The buffer size for file reads.  Adjust this value if it is found to be too
        /// inefficient.
        /// </summary>
        private static int bufferSize = 1024;

        /// <summary>
        /// The default hash algorithm used by the HashEngine if no other hash has been specified.
        /// </summary>
        public static Hashes DefaultHash
        {
            get
            {
                return Hashes.SHA1;
            }
        }

        /// <summary>
        /// The default output type used by the HashEngine if no other output type has been specified.
        /// </summary>
        public static OutputType DefaultOutputType
        {
            get
            {
                return OutputType.Hex;
            }
        }

        #region Public Methods

        #region Single File Hash Methods

        /// <summary>
        /// The core hashing method.  Given a hash algorithm and a file name, compute the hash of
        /// the file and return that hash as a string of hexadecimal characters.
        /// </summary>
        /// <param name="hash">The Hashes enumeration representing the hash algorithm to use</param>
        /// <param name="filename">The path to the file to compute the hash for</param>
        /// <param name="bgWorker">A BackgroundWorker to report any progress to</param>
        /// <param name="dwe">A DoWorkEventArgs object to allow us to cancel the work in progerss
        /// if necessary</param>
        /// <param name="consoleStatus">A <see cref="ConsoleStatusUpdater"/> that, if not null,
        /// will allow us to update the percent complete to the console rather than to the GUI.</param>
        /// <param name="totalByteLength">The total length in bytes of all items being hashed.
        /// This is primarily for multi-file hash comparison, where this value would be the the sum
        /// of all the lengths of all the files being hashed.  This is used to compute the progress
        /// passed to the progress dialog box.  If set to anything less than or equal to zero, it
        /// derives this value from the specified file's length.</param>
        /// <param name="totalBytesSoFar">The total number of bytes already hashed in a multi-file
        /// hash comparison.  This should be the sum of the lengths of all the files hashed before
        /// this file.  This should be zero for the first file in a comparison or for a single file
        /// hash.  If set to anything less than zero, this value is reset to zero.</param>
        /// <param name="outputType">An enum specifying what format the output string should be
        /// in (hexadecimal, Base64, etc.)</param>
        /// <returns>A string representing the computed hash</returns>
        /// <exception cref="HashEngineException">Thrown whenever anything bad happens when the
        /// hash is computed</exception>
        public static string HashFile(Hashes hash, string filename, BackgroundWorker bgWorker,
            DoWorkEventArgs dwe, ConsoleStatusUpdater consoleStatus, long totalByteLength,
            long totalBytesSoFar, OutputType outputType)
        {
            // Lots of things can go wrong here, so ignore Yoda's advice and give it a try:
            try
            {
                // On the off chance we're using a BackgroundWorker and the user cancels the
                // process, formalize the cancel, close the file, and return false:
                if (bgWorker != null && dwe != null && bgWorker.CancellationPending)
                {
                    dwe.Cancel = true;
                    if (fs != null)
                    {
                        fs.Unlock(0, fs.Length);
                        fs.Close();
                    }
                    return null;
                }
                // Find out which hash algorithm to use:
                IDigest hasher = GetHashAlgorithm(hash);
                hasher.Reset();
                // Read in the contents of the file as bytes, then compute the hash.
                // There's no need to worry about encodings and such, as the hash algorithms
                // work on raw bytes.  I switched this from using File.ReadAllBytes() to
                // File.Open() so the file doesn't have to loaded in its entirety into memory
                // before the hash is computed.  This should be a bit more memory efficient.
                // Also note that we need to open and close the FileStream manually; I originally
                // passed this into the HashAlgorithm.ComputeHash() call, but then it wouldn't
                // release the file when it was done.  Also note that we lock the file during
                // the read to prevent other processes from changing it.  If for any reason
                // the file length is less than zero, that means we've overflowed the 64-bit
                // integer length and the file is too large to hash.
                fs = File.Open(filename, FileMode.Open, FileAccess.Read);
                if (fs.Length < 0L)
                    throw new HashEngineException("The total amount of data to hash is " +
                        "too large. The total amount of data hashed cannot " +
                        "exceed 8.05EB (exabytes).");
                fs.Lock(0, fs.Length);
                // For multi-file comparisons, we want to know the total number of bytes in all the
                // files that need to be hashed.  That is passed in here as the total byte length.
                // But this doesn't make sense for single file hashes.  So if we get anything less
                // than or equal to zero, set the total byte length to the length of this particular
                // file.  This is used to calculate the percent complete for the process, so this way
                // it should get the progress for a single hash when hashing a single file, or for
                // the entire batch if comparing multiple.
                if (totalByteLength <= 0) totalByteLength = fs.Length;
                // Similarly, this is the total number of bytes already hashed.  For a multi-file
                // comparison, this would be the sum of the lengths of the files previously hashed
                // in this batch.  If we get anything less than zero, assume there were no other
                // files and reset this to zero, so when it gets added to the progress calculations
                // below, we'll only calculate the progress of the single hash.
                if (totalBytesSoFar < 0) totalBytesSoFar = 0;
                // We'll need a buffer to read in our data:
                byte[] buffer = new byte[bufferSize];
                // And we'll need to know how many bytes we've read so far in this file:
                long bytesSoFar = 0;
                // We'll use these to keep track of our current percentage completion.  "percent"
                // will be recalculated on each pass, while "last_percent" will store the value
                // of the last reported percentage.  The difference between the two will be
                // explained in the relevant code below.
                int last_percent = 0;
                int percent = 0;
                // Keep going until there's nothing left to read:
                while (true)
                {
                    // Read in the next batch of bytes into the buffer:
                    int bytesRead = fs.Read(buffer, 0, bufferSize);
                    // Increment our so-far count:
                    bytesSoFar += (long)bytesRead;
                    // If we didn't read anything this pass, break out of the loop:
                    if (bytesRead == 0) break;
                    // Update the hash with however many bytes are currently in the buffer.
                    // In the previous versions where we used Microsoft's APIs, we had to
                    // carefully choose between the normal update (transform) method and the
                    // final one based on how many bytes were read.  With the Bouncy Castle
                    // API, however, we don't need to worry about that here, so we only have
                    // to worry about updating the hasher with what we've gone.  (We'll do
                    // the finalizing step later.)
                    hasher.BlockUpdate(buffer, 0, bytesRead);
                    // Report our progress to the whoever is listening.  This could be a background
                    // worker (for the GUI) or a special console reporting class (for console apps).
                    // Note that we're also including the total values.  Thus, for a multi-file
                    // comparison, this should give us the progress through the entire batch, every
                    // byte of every file.  For a single file hash, the "bytes so far" value is zero
                    // and the total byte length is the length of the file, so we'll just get the
                    // progress through this single file.
                    percent = (int)(((double)(bytesSoFar + totalBytesSoFar) /
                            (double)totalByteLength) * 100.0);
                    // Only report the percentage if it's different from what we last reported.
                    // The reason for this is that it reduces flicker in the console version.
                    if (percent > last_percent)
                    {
                        // Report the status to the background worker or console updater, which-
                        // ever is present.  Then take not of the update for the next pass.
                        if (bgWorker != null) bgWorker.ReportProgress(percent);
                        if (consoleStatus != null) consoleStatus.SetPercentDone(percent);
                        last_percent = percent;
                    }
                }
                // If we broke out of the loop, grab the final hash value and unlock and close
                // the file:
                byte[] theHash = new byte[hasher.GetDigestSize()];
                hasher.DoFinal(theHash, 0);
                fs.Unlock(0, fs.Length);
                fs.Close();
                // Return the output string in the chosen format:
                return EncodeBytes(theHash, outputType);
            }
            #region Catch Exceptions
            // Unfortunately, I don't know what new and exciting exceptions are thrown by the
            // Bouncy Castle code; the API isn't self-documenting, so none of the IDigest methods
            // mention what exceptions they might throw.  Of course, I *always* catch the general
            // Exception as a last resort, but this may need to be expanded if other exceptions
            // are discovered later.  For now, we'll leave the exceptions thrown by the Microsoft
            // classes in place for now.
            //
            // The following exceptions can be thrown by File.ReadAllBytes():
            catch (PathTooLongException)
            {
                throw new HashEngineException("The path \"" + filename + "\" was too long for " +
                    "the operating system to handle.");
            }
            catch (DirectoryNotFoundException)
            {
                throw new HashEngineException("The path \"" + filename + "\" was invalid. If the " +
                    "file is on a mapped network drive, make sure that drive is actually mapped.");
            }
            catch (UnauthorizedAccessException)
            {
                throw new HashEngineException("The file \"" + filename + "\" is either a directory, " +
                    "is not supported by this platform, you don't have permission to read it, or it " +
                    "otherwise couldn't be read for some reason.");
            }
            catch (FileNotFoundException)
            {
                throw new HashEngineException("The file \"" + filename + "\" could not be found.");
            }
            catch (NotSupportedException)
            {
                throw new HashEngineException("The path \"" + filename + "\" is invalid.");
            }
            catch (System.Security.SecurityException)
            {
                throw new HashEngineException("You do not have the necessary permissions to read " +
                    "the file \"" + filename + "\".");
            }
            catch (ArgumentNullException)
            {
                throw new HashEngineException("No file was specified (the file name was null).");
            }
            //catch (ArgumentOutOfRangeException)
            //{
            //    throw new HashEngineException("ArgumentOutOfRangeException");
            //}
            catch (ArgumentException)
            {
                throw new HashEngineException("No file was specified (the file path was empty, " +
                    "contained only white space, or contained invalid characters).");
            }
            catch (IOException)
            {
                throw new HashEngineException("An unknown I/O error occurred while trying to read " +
                    "the file \"" + filename + "\".");
            }
            //catch (ObjectDisposedException)
            //{
            //    throw new HashEngineException("ObjectDisposedExcpetion");
            //}
            // This can be thrown by SHA256Managed and SHA512Managed constructors, where are
            // called down in GetHashAlgorithm():
            catch (InvalidOperationException)
            {
                throw new HashEngineException("The Federal Information Processing Standards (FIPS) " +
                    "security setting is enabled on your system. The hash algorithm you selected " +
                    "is not available.");
            }
            // Re-throw any HashEngineExceptions that may have been thrown upstream:
            catch (HashEngineException hee) { throw hee; }
            // Finally, a catch-all to catch all exceptions that haven't already been caught:
            catch (CryptographicUnexpectedOperationException)
            {
                throw new HashEngineException("The hash failed to return a useful result. (The " +
                    "returned value was empty.)");
            }
            catch (Exception)
            {
                throw new HashEngineException();
            }
            #endregion
            // Just in case we threw an exception, check to see if the file stream is still there,
            // and unlock and close it if we haven't already.  Note the catch block doesn't do
            // anything; we just silently give up if it didn't work.
            finally
            {
                if (fs != null)
                {
                    try
                    {
                        fs.Unlock(0, fs.Length);
                        fs.Close();
                    }
                    catch { }
                }
            }
        }

        /// <summary>
        /// Given a hash algorithm and a file name, compute the hash of
        /// the file and return that hash as a string of hexadecimal characters.
        /// </summary>
        /// <param name="hash">The Hashes enumeration representing the hash algorithm to use</param>
        /// <param name="filename">The path to the file to compute the hash for</param>
        /// <param name="bgWorker">A BackgroundWorker to report any progress to</param>
        /// <param name="dwe">A DoWorkEventArgs object to allow us to cancel the work in progerss
        /// if necessary</param>
        /// <param name="totalByteLength">The total length in bytes of all items being hashed.
        /// This is primarily for multi-file hash comparison, where this value would be the the sum
        /// of all the lengths of all the files being hashed.  This is used to compute the progress
        /// passed to the progress dialog box.  If set to anything less than or equal to zero, it
        /// derives this value from the specified file's length.</param>
        /// <param name="totalBytesSoFar">The total number of bytes already hashed in a multi-file
        /// hash comparison.  This should be the sum of the lengths of all the files hashed before
        /// this file.  This should be zero for the first file in a comparison or for a single file
        /// hash.  If set to anything less than zero, this value is reset to zero.</param>
        /// <param name="outputType">An enum specifying what format the output string should be
        /// in (hexadecimal, Base64, etc.)</param>
        /// <returns>A string representing the computed hash</returns>
        /// <exception cref="HashEngineException">Thrown whenever anything bad happens when the
        /// hash is computed</exception>
        public static string HashFile(Hashes hash, string filename, BackgroundWorker bgWorker,
            DoWorkEventArgs dwe, long totalByteLength, long totalBytesSoFar, OutputType outputType)
        {
            return HashFile(hash, filename, bgWorker, dwe, null, totalByteLength,
                totalBytesSoFar, outputType);
        }

        /// <summary>
        /// The core hashing method.  Given a hash algorithm and a file name, compute the hash of
        /// the file and return that hash as a string of hexadecimal characters.
        /// </summary>
        /// <param name="hash">The Hashes enumeration representing the hash algorithm to use</param>
        /// <param name="filename">The path to the file to compute the hash for</param>
        /// <param name="bgWorker">A BackgroundWorker to report any progress to</param>
        /// <param name="dwe">A DoWorkEventArgs object to allow us to cancel the work in progerss
        /// if necessary</param>
        /// <param name="totalByteLength">The total length in bytes of all items being hashed.
        /// This is primarily for multi-file hash comparison, where this value would be the the sum
        /// of all the lengths of all the files being hashed.  This is used to compute the progress
        /// passed to the progress dialog box.  If set to anything less than or equal to zero, it
        /// derives this value from the specified file's length.</param>
        /// <param name="totalBytesSoFar">The total number of bytes already hashed in a multi-file
        /// hash comparison.  This should be the sum of the lengths of all the files hashed before
        /// this file.  This should be zero for the first file in a comparison or for a single file
        /// hash.  If set to anything less than zero, this value is reset to zero.</param>
        /// <param name="base64">True to return a Base64 encoded string, false for hexadeciaml</param>
        /// <returns>A string representing the computed hash</returns>
        /// <exception cref="HashEngineException">Thrown whenever anything bad happens when the
        /// hash is computed</exception>
        public static string HashFile(Hashes hash, string filename, BackgroundWorker bgWorker,
            DoWorkEventArgs dwe, long totalByteLength, long totalBytesSoFar, bool base64)
        {
            if (base64) return HashFile(hash, filename, bgWorker, dwe, totalByteLength,
                totalBytesSoFar, OutputType.Base64);
            else return HashFile(hash, filename, bgWorker, dwe, totalByteLength, totalBytesSoFar,
                OutputType.Hex);
        }


        /// <summary>
        /// The core hashing method.  Given a hash algorithm and a file name, compute the hash of
        /// the file and return that hash as a string of hexadecimal characters.
        /// </summary>
        /// <param name="hash">The Hashes enumeration representing the hash algorithm to use</param>
        /// <param name="filename">The path to the file to compute the hash for</param>
        /// <param name="bgWorker">A BackgroundWorker to report any progress to</param>
        /// <param name="dwe">A DoWorkEventArgs object to allow us to cancel the work in progerss
        /// if necessary</param>
        /// <param name="totalByteLength">The total length in bytes of all items being hashed.
        /// This is primarily for multi-file hash comparison, where this value would be the the sum
        /// of all the lengths of all the files being hashed.  This is used to compute the progress
        /// passed to the progress dialog box.  If set to anything less than or equal to zero, it
        /// derives this value from the specified file's length.</param>
        /// <param name="totalBytesSoFar">The total number of bytes already hashed in a multi-file
        /// hash comparison.  This should be the sum of the lengths of all the files hashed before
        /// this file.  This should be zero for the first file in a comparison or for a single file
        /// hash.  If set to anything less than zero, this value is reset to zero.</param>
        /// <returns>A hexadecimal string representing the computed hash</returns>
        /// <exception cref="HashEngineException">Thrown whenever anything bad happens when the
        /// hash is computed</exception>
        public static string HashFile(Hashes hash, string filename, BackgroundWorker bgWorker,
            DoWorkEventArgs dwe, long totalByteLength, long totalBytesSoFar)
        {
            return HashFile(hash, filename, bgWorker, dwe, totalByteLength, totalBytesSoFar,
                DefaultOutputType);
        }

        /// <summary>
        /// The core hashing method.  Given a hash algorithm and a file name, compute the hash of
        /// the file and return that hash as a string of hexadecimal characters.
        /// </summary>
        /// <param name="hash">The Hashes enumeration representing the hash algorithm to use</param>
        /// <param name="filename">The path to the file to compute the hash for</param>
        /// <param name="bgWorker">A BackgroundWorker to report any progress to</param>
        /// <param name="dwe">A DoWorkEventArgs object to allow us to cancel the work in progerss
        /// if necessary</param>
        /// <param name="outputType">An enum specifying what format the output string should be
        /// in (hexadecimal, Base64, etc.)</param>
        /// <returns>A string representing the computed hash</returns>
        /// <exception cref="HashEngineException">Thrown whenever anything bad happens when the
        /// hash is computed</exception>
        public static string HashFile(Hashes hash, string filename, BackgroundWorker bgWorker,
            DoWorkEventArgs dwe, OutputType outputType)
        {
            return HashFile(hash, filename, bgWorker, dwe, -1, -1, outputType);
        }

        /// <summary>
        /// The core hashing method.  Given a hash algorithm and a file name, compute the hash of
        /// the file and return that hash as a string of hexadecimal characters.
        /// </summary>
        /// <param name="hash">The Hashes enumeration representing the hash algorithm to use</param>
        /// <param name="filename">The path to the file to compute the hash for</param>
        /// <param name="bgWorker">A BackgroundWorker to report any progress to</param>
        /// <param name="dwe">A DoWorkEventArgs object to allow us to cancel the work in progerss
        /// if necessary</param>
        /// <param name="base64">True to return a Base64 encoded string, false for hexadeciaml</param>
        /// <returns>A string representing the computed hash</returns>
        /// <exception cref="HashEngineException">Thrown whenever anything bad happens when the
        /// hash is computed</exception>
        public static string HashFile(Hashes hash, string filename, BackgroundWorker bgWorker,
            DoWorkEventArgs dwe, bool base64)
        {
            return HashFile(hash, filename, bgWorker, dwe, -1, -1, base64);
        }

        /// <summary>
        /// The core hashing method.  Given a hash algorithm and a file name, compute the hash of
        /// the file and return that hash as a string of hexadecimal characters.
        /// </summary>
        /// <param name="hash">The Hashes enumeration representing the hash algorithm to use</param>
        /// <param name="filename">The path to the file to compute the hash for</param>
        /// <param name="bgWorker">A BackgroundWorker to report any progress to</param>
        /// <param name="dwe">A DoWorkEventArgs object to allow us to cancel the work in progerss
        /// if necessary</param>
        /// <returns>A hexadecimal string representing the computed hash</returns>
        /// <exception cref="HashEngineException">Thrown whenever anything bad happens when the
        /// hash is computed</exception>
        public static string HashFile(Hashes hash, string filename, BackgroundWorker bgWorker,
            DoWorkEventArgs dwe)
        {
            return HashFile(hash, filename, bgWorker, dwe, -1, -1, DefaultOutputType);
        }

        /// <summary>
        /// The core hashing method.  Given a hash algorithm and a file name, compute the hash of
        /// the file and return that hash as a string of hexadecimal characters.
        /// </summary>
        /// <param name="hash">The Hashes enumeration representing the hash algorithm to use</param>
        /// <param name="filename">The path to the file to compute the hash for</param>
        /// <param name="outputType">An enum specifying what format the output string should be
        /// in (hexadecimal, Base64, etc.)</param>
        /// <returns>A string representing the computed hash</returns>
        /// <exception cref="HashEngineException">Thrown whenever anything bad happens when the
        /// hash is computed</exception>
        public static string HashFile(Hashes hash, string filename, OutputType outputType)
        {
            return HashFile(hash, filename, null, null, -1, -1, outputType);
        }

        /// <summary>
        /// The core hashing method.  Given a hash algorithm and a file name, compute the hash of
        /// the file and return that hash as a string of hexadecimal characters.
        /// </summary>
        /// <param name="hash">The Hashes enumeration representing the hash algorithm to use</param>
        /// <param name="filename">The path to the file to compute the hash for</param>
        /// <param name="outputType">An enum specifying what format the output string should be
        /// in (hexadecimal, Base64, etc.)</param>
        /// <param name="consoleStatus">A <see cref="ConsoleStatusUpdater"/> that, if not null,
        /// will allow us to update the percent complete to the console rather than to the GUI.</param>
        /// <returns>A string representing the computed hash</returns>
        /// <exception cref="HashEngineException">Thrown whenever anything bad happens when the
        /// hash is computed</exception>
        public static string HashFile(Hashes hash, string filename, OutputType outputType,
            ConsoleStatusUpdater consoleStatus)
        {
            return HashFile(hash, filename, null, null, consoleStatus, -1, -1, outputType);
        }


        /// <summary>
        /// The core hashing method.  Given a hash algorithm and a file name, compute the hash of
        /// the file and return that hash as a string of hexadecimal characters.
        /// </summary>
        /// <param name="hash">The Hashes enumeration representing the hash algorithm to use</param>
        /// <param name="filename">The path to the file to compute the hash for</param>
        /// <param name="base64">True to return a Base64 encoded string, false for hexadeciaml</param>
        /// <returns>A string representing the computed hash</returns>
        /// <exception cref="HashEngineException">Thrown whenever anything bad happens when the
        /// hash is computed</exception>
        public static string HashFile(Hashes hash, string filename, bool base64)
        {
            return HashFile(hash, filename, null, null, -1, -1, base64);
        }

        /// <summary>
        /// The core hashing method.  Given a hash algorithm and a file name, compute the hash of
        /// the file and return that hash as a string of hexadecimal characters.
        /// </summary>
        /// <param name="hash">The Hashes enumeration representing the hash algorithm to use</param>
        /// <param name="filename">The path to the file to compute the hash for</param>
        /// <returns>A hexadecimal string representing the computed hash</returns>
        /// <exception cref="HashEngineException">Thrown whenever anything bad happens when the
        /// hash is computed</exception>
        public static string HashFile(Hashes hash, string filename)
        {
            return HashFile(hash, filename, null, null, -1, -1, DefaultOutputType);
        }

        /// <summary>
        /// A convenience method that computes the MD5 hash of a given file.
        /// </summary>
        /// <param name="filename">The path to the file to compute the hash of</param>
        /// <param name="outputType">An enum specifying what format the output string should be
        /// in (hexadecimal, Base64, etc.)</param>
        /// <returns>A string representing the MD5 hash of the file</returns>
        /// <exception cref="HashEngineException">Thrown whenever anything bad happens when the
        /// hash is computed</exception>
        public static string MD5HashFile(string filename, OutputType outputType)
        {
            return HashFile(Hashes.MD5, filename, null, null, -1, -1, outputType);
        }

        /// <summary>
        /// A convenience method that computes the MD5 hash of a given file.
        /// </summary>
        /// <param name="filename">The path to the file to compute the hash of</param>
        /// <param name="base64">True to return a Base64 encoded string, false for hexadeciaml</param>
        /// <returns>A string representing the MD5 hash of the file</returns>
        /// <exception cref="HashEngineException">Thrown whenever anything bad happens when the
        /// hash is computed</exception>
        public static string MD5HashFile(string filename, bool base64)
        {
            return HashFile(Hashes.MD5, filename, null, null, -1, -1, base64);
        }

        /// <summary>
        /// A convenience method that computes the MD5 hash of a given file.
        /// </summary>
        /// <param name="filename">The path to the file to compute the hash of</param>
        /// <returns>A hexadecimal string representing the MD5 hash of the file</returns>
        /// <exception cref="HashEngineException">Thrown whenever anything bad happens when the
        /// hash is computed</exception>
        public static string MD5HashFile(string filename)
        {
            return HashFile(Hashes.MD5, filename, null, null, -1, -1, DefaultOutputType);
        }

        /// <summary>
        /// A convenience method that computes the SHA1 hash of a given file.
        /// </summary>
        /// <param name="filename">The path to the file to compute the hash of</param>
        /// <param name="outputType">An enum specifying what format the output string should be
        /// in (hexadecimal, Base64, etc.)</param>
        /// <returns>A string representing the MD5 hash of the file</returns>
        /// <exception cref="HashEngineException">Thrown whenever anything bad happens when the
        /// hash is computed</exception>
        public static string SHA1HashFile(string filename, OutputType outputType)
        {
            return HashFile(Hashes.SHA1, filename, null, null, -1, -1, outputType);
        }

        /// <summary>
        /// A convenience method that computes the SHA1 hash of a given file.
        /// </summary>
        /// <param name="filename">The path to the file to compute the hash of</param>
        /// <param name="base64">True to return a Base64 encoded string, false for hexadeciaml</param>
        /// <returns>A string representing the MD5 hash of the file</returns>
        /// <exception cref="HashEngineException">Thrown whenever anything bad happens when the
        /// hash is computed</exception>
        public static string SHA1HashFile(string filename, bool base64)
        {
            return HashFile(Hashes.SHA1, filename, null, null, -1, -1, base64);
        }

        /// <summary>
        /// A convenience method that computes the SHA1 hash of a given file.
        /// </summary>
        /// <param name="filename">The path to the file to compute the hash of</param>
        /// <returns>A hexadecimal string representing the MD5 hash of the file</returns>
        /// <exception cref="HashEngineException">Thrown whenever anything bad happens when the
        /// hash is computed</exception>
        public static string SHA1HashFile(string filename)
        {
            return HashFile(Hashes.SHA1, filename, null, null, -1, -1, DefaultOutputType);
        }

        #endregion

        #region Compare File Hash Methods

        /// <summary>
        /// Compares the hashes of all the files in the file list and determine whether or not
        /// they all match.  Note that this is an all-or-nothing deal; if just one hash does
        /// not match, the entire batch failes.  If no files or only one are passed in, the
        /// method also fails, as there's nothing to compare.
        /// </summary>
        /// <param name="hash">The Hashes enumeration representing the hash algorithm to use</param>
        /// <param name="fileList">A string array representing the path to each file to
        /// process</param>
        /// <param name="bgWorker">A BackgroundWorker task to notify of our progress</param>
        /// <param name="dwe">A DoWorkEventArgs object to let us know if the user has cancelled
        /// the comparison</param>
        /// <param name="consoleStatus">A <see cref="ConsoleStatusUpdater"/> that, if not null,
        /// will allow us to update the percent complete to the console rather than to the GUI.</param>
        /// <returns>A boolean flag representing whether or not all the hashes matched or not</returns>
        /// <exception cref="HashEngineException">Thrown whenever anything bad happens when the
        /// hash is computed</exception>
        public static bool CompareHashes(Hashes hash, string[] fileList, BackgroundWorker bgWorker,
            DoWorkEventArgs dwe, ConsoleStatusUpdater consoleStatus)
        {
            // On the off chance we're using a BackgroundWorker and the user cancels the
            // process, formalize the cancel and return false:
            if (bgWorker != null && dwe != null && bgWorker.CancellationPending)
            {
                dwe.Cancel = true;
                return false;
            }
            // If we only got one file path or none at all, go ahead and fail, as there's
            // nothing to compare:
            if (fileList.Length <= 1) return false;
            // We need to keep track of the first file's hash so we can compare the other hashes
            // to it:
            string firstHash = String.Empty;
            // In order to report our progress, we need to know how many total bytes we have to
            // hash and how many bytes we've hashed so far.  This needs to be across all files in
            // the batch.  So declare these two variables and we'll keep track of that information
            // as we go.
            long totalByteLength = 0;
            long totalBytesSoFar = 0;
            // Only bother calculating the total number of bytes if we'll be reporting it back
            // to a GUI background worker:
            if (bgWorker != null || consoleStatus != null)
            {
                // Step through the files, get their lengths, and add them to the total.  I'd
                // rather not step through the file list more than once, but we need this total
                // before we process the first file.
                foreach (string file in fileList)
                {
                    FileInfo fi = new FileInfo(file);
                    totalByteLength += fi.Length;
                    // If at any point the total byte length is less than zero, that means we've
                    // overflowed the 64-bit integer on the length.  We can't handle this
                    // situation, so throw an error.  This is pretty unlikely to happen, as
                    // this means the total byte length is in excess of 8.05EB (exabytes),
                    // but in today's world of ever growing media types, who knows what the
                    // future will bring?
                    if (totalByteLength < 0L)
                        throw new HashEngineException("The total amount of data to hash is " +
                            "too large. The sum of the size of all files being hashed cannot " +
                            "exceed 8.05EB (exabytes).");
                }
            }
            // We don't care about the total if we're not reporting progress:
            else totalByteLength = -1;
            // Now step through the entire file list:
            foreach (string file in fileList)
            {
                // If this is the first file in the list, compute its hash and store it.  Note
                // that we just call the HashFile() method above to do the dirty work.
                if (firstHash.CompareTo(String.Empty) == 0)
                    firstHash = HashFile(hash, file, bgWorker, dwe, consoleStatus,
                        totalByteLength, totalBytesSoFar, OutputType.Base64);
                // Otherwise, compare the current file's hash to the first file's hash.  If
                // the hashes do not match, fail the entire batch.  Note that this kind of
                // short-ciruits, so if we're comparing a bunch of files but the first two
                // don't match, we won't bother with the rest.
                else
                    if (firstHash.CompareTo(HashFile(hash, file, bgWorker, dwe, consoleStatus,
                        totalByteLength, totalBytesSoFar, OutputType.Base64)) != 0)
                        return false;
                // If we didn't return there, add the length of the file we just processed to the
                // total number of bytes hashed, so we can add that to the progress in the next
                // pass:
                if (bgWorker != null || consoleStatus != null)
                {
                    FileInfo fi = new FileInfo(file);
                    totalBytesSoFar += fi.Length;
                }
            }
            // If we get to this point, all of the hashes match:
            return true;
        }

        /// <summary>
        /// Compares the hashes of all the files in the file list and determine whether or not
        /// they all match.  Note that this is an all-or-nothing deal; if just one hash does
        /// not match, the entire batch failes.  If no files or only one are passed in, the
        /// method also fails, as there's nothing to compare.
        /// </summary>
        /// <param name="hash">The Hashes enumeration representing the hash algorithm to use</param>
        /// <param name="fileList">A string array representing the path to each file to
        /// process</param>
        /// <param name="bgWorker">A BackgroundWorker task to notify of our progress</param>
        /// <param name="dwe">A DoWorkEventArgs object to let us know if the user has cancelled
        /// the comparison</param>
        /// <returns>A boolean flag representing whether or not all the hashes matched or not</returns>
        /// <exception cref="HashEngineException">Thrown whenever anything bad happens when the
        /// hash is computed</exception>
        public static bool CompareHashes(Hashes hash, string[] fileList, BackgroundWorker bgWorker,
            DoWorkEventArgs dwe)
        {
            return CompareHashes(hash, fileList, bgWorker, dwe, null);
        }

        /// <summary>
        /// Compares the hashes of all the files in the file list and determine whether or not
        /// they all match.  Note that this is an all-or-nothing deal; if just one hash does
        /// not match, the entire batch failes.  If no files or only one are passed in, the
        /// method also fails, as there's nothing to compare.
        /// </summary>
        /// <param name="hash">The Hashes enumeration representing the hash algorithm to use</param>
        /// <param name="fileList">A string array representing the path to each file to
        /// process</param>
        /// <param name="consoleStatus">A <see cref="ConsoleStatusUpdater"/> that, if not null,
        /// will allow us to update the percent complete to the console rather than to the GUI.</param>
        /// <returns>A boolean flag representing whether or not all the hashes matched or not</returns>
        /// <exception cref="HashEngineException">Thrown whenever anything bad happens when the
        /// hash is computed</exception>
        public static bool CompareHashes(Hashes hash, string[] fileList,
            ConsoleStatusUpdater consoleStatus)
        {
            return CompareHashes(hash, fileList, null, null, consoleStatus);
        }
        
        /// <summary>
        /// Compares the hashes of all the files in the file list and determine whether or not
        /// they all match.  Note that this is an all-or-nothing deal; if just one hash does
        /// not match, the entire batch failes.  If no files or only one are passed in, the
        /// method also fails, as there's nothing to compare.
        /// </summary>
        /// <param name="hash">The Hashes enumeration representing the hash algorithm to use</param>
        /// <param name="fileList">A string array representing the path to each file to
        /// process</param>
        /// <returns>A boolean flag representing whether or not all the hashes matched or not</returns>
        /// <exception cref="HashEngineException">Thrown whenever anything bad happens when the
        /// hash is computed</exception>
        public static bool CompareHashes(Hashes hash, string[] fileList)
        {
            return CompareHashes(hash, fileList, null, null);
        }

        /// <summary>
        /// A convenience method that compares the MD5 hashes of each file in the list.
        /// </summary>
        /// <param name="fileList">A string array representing the path to each file to
        /// process</param>
        /// <returns>A boolean flag representing whether or not all the hashes matched or not</returns>
        /// <exception cref="HashEngineException">Thrown whenever anything bad happens when the
        /// hash is computed</exception>
        public static bool CompareMD5Hashes(string[] fileList)
        {
            return CompareHashes(Hashes.MD5, fileList);
        }

        /// <summary>
        /// A convenience method that compares the SHA1 hashes of each file in the list.
        /// </summary>
        /// <param name="fileList">A string array representing the path to each file to
        /// process</param>
        /// <returns>A boolean flag representing whether or not all the hashes matched or not</returns>
        /// <exception cref="HashEngineException">Thrown whenever anything bad happens when the
        /// hash is computed</exception>
        public static bool CompareSHA1Hashes(string[] fileList)
        {
            return CompareHashes(Hashes.SHA1, fileList);
        }

        #endregion

        #region Hash Text Methods

        /// <summary>
        /// Given a hash algorithm, a text string, and a text encoding (presumably in which the
        /// string was encoded), compute the specified hash
        /// </summary>
        /// <param name="hash">The hash to perform</param>
        /// <param name="text">The input text</param>
        /// <param name="encoding">The text encoding of the input text</param>
        /// <param name="outputType">An enum specifying what format the output string should be
        /// in (hexadecimal, Base64, etc.)</param>
        /// <returns>A string representing the computed hash</returns>
        /// <exception cref="HashEngineException">Thrown whenever anything bad happens when the
        /// hash is computed</exception>
        public static string HashText(Hashes hash, string text, Encoding encoding,
            OutputType outputType)
        {
            // This one is pretty simple:
            try
            {
                // Get the hash algorithm:
                IDigest hasher = GetHashAlgorithm(hash);
                // Convert the input string to raw bytes given the encoding:
                byte[] textBytes = encoding.GetBytes(text);
                // Hash it and get the hash:
                hasher.BlockUpdate(textBytes, 0, textBytes.Length);
                byte[] theHash = new byte[hasher.GetDigestSize()];
                hasher.DoFinal(theHash, 0);
                // And build the output:
                return EncodeBytes(theHash, outputType);
            }
            // Pretty this up by catching specific exceptions:
            catch (Exception ex)
            {
                throw new HashEngineException(ex.Message);
            }
        }

                /// <summary>
        /// Given a hash algorithm, a text string, and a text encoding (presumably in which the
        /// string was encoded), compute the specified hash
        /// </summary>
        /// <param name="hash">The hash to perform</param>
        /// <param name="text">The input text</param>
        /// <param name="encoding">The text encoding of the input text</param>
        /// <param name="base64">True to return a Base64 encoded string, false for hexadeciaml</param>
        /// <returns>A string representing the computed hash</returns>
        /// <exception cref="HashEngineException">Thrown whenever anything bad happens when the
        /// hash is computed</exception>
        public static string HashText(Hashes hash, string text, Encoding encoding, bool base64)
        {
            if (base64) return HashText(hash, text, encoding, OutputType.Base64);
            else return HashText(hash, text, encoding, OutputType.Hex);
        }

        /// <summary>
        /// Given a hash algorithm, a text string, and a text encoding (presumably in which the
        /// string was encoded), compute the specified hash and return the hexadecimal output
        /// </summary>
        /// <param name="hash">The hash to perform</param>
        /// <param name="text">The input text</param>
        /// <param name="encoding">The text encoding of the input text</param>
        /// <returns>A hexadecimal output of the computed hash</returns>
        /// <exception cref="HashEngineException">Thrown whenever anything bad happens when the
        /// hash is computed</exception>
        public static string HashText(Hashes hash, string text, Encoding encoding)
        {
            return HashText(hash, text, encoding, DefaultOutputType);
        }

        #endregion

        #region Static Convenience Methods

        /// <summary>
        /// Returns a display name <see cref="String"/> for the given Hashes enumeration
        /// </summary>
        /// <param name="hash">A Hashes enumeration item</param>
        /// <returns>A <see cref="String"/> suitable for display</returns>
        public static string GetHashName(Hashes hash)
        {
            // Pure and simple:  switch on the input Hashes enumeration.  The default
            // should never be hit here, but if for some reason we hit it, we need to
            // print something intelligent anyway.
            switch (hash)
            {
                case Hashes.MD5:
                    return "MD5";
                case Hashes.SHA1:
                    return "SHA-1";
                case Hashes.SHA224:
                    return "SHA-224";
                case Hashes.SHA256:
                    return "SHA-256";
                case Hashes.SHA384:
                    return "SHA-384";
                case Hashes.SHA512:
                    return "SHA-512";
                case Hashes.RIPEMD128:
                    return "RIPEMD128";
                case Hashes.RIPEMD160:
                    return "RIPEMD160";
                case Hashes.RIPEMD256:
                    return "RIPEMD256";
                case Hashes.RIPEMD320:
                    return "RIPEMD320";
                case Hashes.Whirlpool:
                    return "Whirlpool";
                case Hashes.Tiger:
                    return "Tiger";
                case Hashes.GOST3411:
                    return "GOST3411";
                default:
                    return "Invalid hash";
            }
        }

        /// <summary>
        /// Returns the Hashes enumeration item for the specified hash displays string.
        /// The test on the string is case insensitive.
        /// </summary>
        /// <param name="hashName">A <see cref="String"/> containing the hash display name</param>
        /// <returns>A Hashes enumeration represented by the input <see cref="String"/>. If the
        /// input String does not represent a valid Hashes option, the default hash is returned.</returns>
        public static Hashes GetHashFromName(string hashName)
        {
            // If we get a null or empty string, just return the default hash:
            if (String.IsNullOrEmpty(hashName))
                return DefaultHash;
            // To normalize things, force all upper case and strip out any non-alphanumeric characters:
            hashName = Regex.Replace(hashName.ToUpper(), @"\W+", "").Replace("_", "");
            // Now switch based on the modified input:
            switch (hashName)
            {
                case "MD5":
                    return Hashes.MD5;
                case "SHA1":
                    return Hashes.SHA1;
                case "SHA224":
                    return Hashes.SHA224;
                case "SHA256":
                    return Hashes.SHA256;
                case "SHA384":
                    return Hashes.SHA384;
                case "SHA512":
                    return Hashes.SHA512;
                case "RIPEMD128":
                    return Hashes.RIPEMD128;
                case "RIPEMD160":
                    return Hashes.RIPEMD160;
                case "RIPEMD256":
                    return Hashes.RIPEMD256;
                case "RIPEMD320":
                    return Hashes.RIPEMD320;
                case "WHIRLPOOL":
                    return Hashes.Whirlpool;
                case "TIGER":
                    return Hashes.Tiger;
                case "GOST3411":
                    return Hashes.GOST3411;
                // If we didn't get a match on anything, return the default hash:
                default:
                    return DefaultHash;
            }
        }

        /// <summary>
        /// Returns a display name <see cref="String"/> for the given OutputType enumeration
        /// </summary>
        /// <param name="hash">An OutputType enumeration item</param>
        /// <returns>A <see cref="String"/> suitable for display</returns>
        public static string GetOutputTypeName(OutputType outputType)
        {
            switch (outputType)
            {
                case OutputType.Base64:
                    return "Base64";
                case OutputType.BubbleBabble:
                    return "Bubble Babble";
                case OutputType.CapHex:
                    return "Hex (Caps)";
                case OutputType.Hex:
                    return "Hexadecimal";
                default:
                    return GetOutputTypeName(DefaultOutputType);
            }
        }

        /// <summary>
        /// Returns the OutputType enumeration item for the specified output type display string.
        /// The test on the string is case insensitive.
        /// </summary>
        /// <param name="hashName">A <see cref="String"/> containing the output type display name</param>
        /// <returns>An OutputType enumeration represented by the input <see cref="String"/>. If the
        /// input String does not represent a valid OutputType option, the default hash is returned.</returns>
        public static OutputType GetOutputTypeFromName(string outputType)
        {
            // If we get a null or empty string, just return the default hash:
            if (String.IsNullOrEmpty(outputType))
                return OutputType.Hex;
            // To normalize things, force all upper case and strip out any non-alphanumeric characters:
            outputType = Regex.Replace(outputType.ToUpper(), @"\W+", "").Replace("_", "");
            // Now switch based on the modified input:
            switch (outputType)
            {
                case "BASE64":
                    return OutputType.Base64;
                case "BUBBLEBABBLE":
                    return OutputType.BubbleBabble;
                case "HEXCAPS":
                    return OutputType.CapHex;
                case "HEXADECIMAL":
                    return OutputType.Hex;
                default:
                    return DefaultOutputType;
            }
        }

        /// <summary>
        /// Encode the specified byte array in the specified output type.
        /// </summary>
        /// <param name="theHash">The byte array to encode</param>
        /// <param name="outputType">The output type</param>
        /// <returns>A string containing the encoded data</returns>
        public static string EncodeBytes(byte[] theHash, OutputType outputType)
        {
            // This has mainly been pulled out into its own function because I keep reusing
            // the same code over and over again.  So there's no sense copying and pasting
            // everywhere, and changing it in one place means it now gets changed everywhere.
            switch (outputType)
            {
                // Encode with Base64:
                case OutputType.Base64:
                    return BytesToBase64String(theHash);
                // Encode with Bubble Babble:
                case OutputType.BubbleBabble:
                    return BytesToBubbleBabbleString(theHash);
                // Encode with hexadecimal, but with upper-case letters:
                case OutputType.CapHex:
                    return BytesToHexString(theHash).ToUpper();
                // Encode with hexadecimal with lower-case letters:
                case OutputType.Hex:
                    return BytesToHexString(theHash);
                // If for some bizarre reason we don't end up with one of the proper output
                // type values, drop back to the default:
                default:
                    return EncodeBytes(theHash, DefaultOutputType);
            }
        }

        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// Given an item from the Hashes enumeration, get the <see cref="IDigest"/> associated with it.
        /// </summary>
        /// <param name="hash">A Hashes enumeration item</param>
        /// <returns>The <see cref="IDigest"/> that matches the enumerated hash</returns>
        private static IDigest GetHashAlgorithm(Hashes hash)
        {
            IDigest hasher = null;
            switch (hash)
            {
                case Hashes.MD5:
                    hasher = new MD5Digest();
                    break;
                case Hashes.SHA1:
                    hasher = new Sha1Digest();
                    break;
                case Hashes.SHA224:
                    hasher = new Sha224Digest();
                    break;
                case Hashes.SHA256:
                    hasher = new Sha256Digest();
                    break;
                case Hashes.SHA384:
                    hasher = new Sha384Digest();
                    break;
                case Hashes.SHA512:
                    hasher = new Sha512Digest();
                    break;
                case Hashes.RIPEMD128:
                    hasher = new RipeMD128Digest();
                    break;
                case Hashes.RIPEMD160:
                    hasher = new RipeMD160Digest();
                    break;
                case Hashes.RIPEMD256:
                    hasher = new RipeMD256Digest();
                    break;
                case Hashes.RIPEMD320:
                    hasher = new RipeMD320Digest();
                    break;
                case Hashes.Whirlpool:
                    hasher = new WhirlpoolDigest();
                    break;
                case Hashes.Tiger:
                    hasher = new TigerDigest();
                    break;
                case Hashes.GOST3411:
                    hasher = new Gost3411Digest();
                    break;
                // If we didn't get something we expected, return the default:
                default:
                    hasher = GetHashAlgorithm(DefaultHash);
                    break;
            }
            return hasher;
        }

        /// <summary>
        /// Convert a byte array into a string of hexadecimal digits
        /// </summary>
        /// <param name="bytes">The input byte array</param>
        /// <returns>A string of hexadecimal digits representing the input array</returns>
        private static string BytesToHexString(byte[] bytes)
        {
            // There's probably a more efficient way to do this, but this is all I've found
            // so far:
            StringBuilder sOutput = new StringBuilder(bytes.Length);
            for (int i = 0; i < bytes.Length; i++)
            {
                sOutput.Append(bytes[i].ToString("x2"));
            }
            return sOutput.ToString();
        }

        /// <summary>
        /// Convert a byte array into a Base64-encoded string
        /// </summary>
        /// <param name="bytes">The input byte array</param>
        /// <returns>A Base64-encoded string representing the input array</returns>
        private static string BytesToBase64String(byte[] bytes)
        {
            // This is simple enough:
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Convert a byte array into a Bubble-Babble-encoded string
        /// </summary>
        /// <param name="bytes">The input byte array</param>
        /// <returns>A Bubble-Babble-encoded string representing the input array</returns>
        private static string BytesToBubbleBabbleString(byte[] bytes)
        {
            // The official Bubble Babble implemention seems to come from here:
            // http://wiki.yak.net/589
            //
            // HOWEVER... as hard as I tried to implement that as described, I could never
            // get it to work.  THIS, however, is a port of the Perl Digest::BubbleBabble
            // module from CPAN by Benjamin Trott, found here:
            // http://search.cpan.org/~btrott/Digest-BubbleBabble-0.01/BubbleBabble.pm
            // and it works flawlessly.  The biggest change is using .NET's StringBuilder to
            // build the output string rather than just concatenating strings together as
            // Perl can do so easily.
            //
            // This isn't well documented because, well, quite frankly, I can't describe
            // how it works very well.  As mentioned, the *official* description is clear
            // as mud to me and the Perl port "just works".  This passes all the test vectors
            // for Bubble Babble, so I'm declaring it done.
            char[] vowels = { 'a', 'e', 'i', 'o', 'u', 'y' };
            char[] consonants = { 'b', 'c', 'd', 'f', 'g', 'h', 'k', 'l', 'm', 'n', 'p', 'r',
                's', 't', 'v', 'z', 'x' };
            int seed = 1;
            int rounds = (bytes.Length / 2) + 1;
            StringBuilder sOutput = new StringBuilder();
            sOutput.Append('x');
            for (int i = 0; i < rounds; i++)
            {
                if (i + 1 < rounds || bytes.Length % 2 != 0)
                {
                    int idx0 = (((bytes[2 * i] >> 6) & 3) + seed) % 6;
                    int idx1 = (bytes[2 * i] >> 2) & 15;
                    int idx2 = ((bytes[2 * i] & 3) + seed / 6) % 6;
                    sOutput.Append(vowels[idx0]);
                    sOutput.Append(consonants[idx1]);
                    sOutput.Append(vowels[idx2]);
                    if (i + 1 < rounds)
                    {
                        int idx3 = (bytes[2 * i + 1] >> 4) & 15;
                        int idx4 = bytes[2 * i + 1] & 15;
                        sOutput.Append(consonants[idx3]);
                        sOutput.Append('-');
                        sOutput.Append(consonants[idx4]);
                        seed = (seed * 5 + bytes[2 * i] * 7 + bytes[2 * i + 1]) % 36;
                    }
                }
                else
                {
                    int idx0 = seed % 6;
                    int idx1 = 16;
                    int idx2 = seed / 6;
                    sOutput.Append(vowels[idx0]);
                    sOutput.Append(consonants[idx1]);
                    sOutput.Append(vowels[idx2]);
                }
            }
            sOutput.Append('x');
            return sOutput.ToString();
        }

        #endregion
    }
}
