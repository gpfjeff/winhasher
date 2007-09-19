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
using System.Security.Cryptography;
using System.IO;

namespace com.gpfcomics.WinHasher.Core
{
    /// <summary>
    /// An enumeration of the hashes this library supports.
    /// </summary>
    public enum Hashes
    {
        MD5,
        SHA1,
        SHA256,
        SHA384,
        SHA512,
        RIPEMD160/*,
        Whirlpool*/
    }

    /// <summary>
    /// The hash engine core.  Note that this is a static class, and it should be used like a
    /// utility library.  There's no need to create instances; just call the static methods.
    /// </summary>
    public static class HashEngine
    {
        #region Public Methods

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
            // Find out which hash algorithm to use:
            HashAlgorithm hasher = GetHashAlgorithm(hash);
            // Lots of things can go wrong here, so ignore Yoda's advice and give it a try:
            try
            {
                // Read in the contents of the file as bytes, then compute the hash.
                // There's no need to worry about encodings and such, as the hash algorithms
                // work on raw bytes.  I switched this from using File.ReadAllBytes() to
                // File.Open() so the file doesn't have to loaded in its entirety into memory
                // before the hash is computed.  This should be a bit more memory efficient.
                // Also note that we need to open and close the FileStream manually; I originally
                // passed this into the HashAlgorithm.ComputeHash() call, but then it wouldn't
                // release the file when it was done.
                FileStream fs = File.Open(filename, FileMode.Open, FileAccess.Read);
                byte[] theHash = hasher.ComputeHash(fs);
                fs.Close();
                // Begin to build the output string.  There's probably a more efficient way
                // of doing this, but this method has worked for me in the past.
                StringBuilder sOutput = new StringBuilder(theHash.Length);
                for (int i = 0; i < theHash.Length; i++)
                {
                    sOutput.Append(theHash[i].ToString("x2"));
                }
                return sOutput.ToString();
            }
            #region Catch Exceptions
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
            // This can be thrown by SHA256Managed and SHA512Managed constructors, where are
            // called down in GetHashAlgorithm():
            catch (InvalidOperationException)
            {
                throw new HashEngineException("The Federal Information Processing Standards (FIPS) " +
                    "security setting is enabled on your system. The hash algorithm you selected " +
                    "is not available.");
            }
            // Finally, a catch-all to catch all exceptions that haven't already been caught:
            catch (Exception)
            {
                throw new HashEngineException();
            }
            #endregion
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
            return HashFile(Hashes.MD5, filename);
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
            return HashFile(Hashes.SHA1, filename);
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
            // If we only got one file path or none at all, go ahead and fail, as there's
            // nothing to compare:
            if (fileList.Length <= 1) return false;
            // We need to keep track of the first file's hash so we can compare the other hashes
            // to it:
            string firstHash = String.Empty;
            // Step through the entire file list:
            foreach (string file in fileList)
            {
                // If this is the first file in the list, compute its hash and store it.  Note
                // that we just call the HashFile() method above to do the dirty work.
                if (firstHash.CompareTo(String.Empty) == 0)
                {
                    firstHash = HashFile(hash, file).ToUpper();
                }
                // Otherwise, compare the current file's hash to the first file's hash.  If
                // the hashes do not match, fail the entire batch.  Note that this kind of
                // short-ciruits, so if we're comparing a bunch of files but the first two
                // don't match, we won't bother with the rest.
                else
                {
                    if (firstHash.CompareTo(HashFile(hash, file).ToUpper()) != 0)
                    {
                        return false;
                    }
                }
            }
            // If we get to this point, all of the hashes match:
            return true;
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

        #region Private Methods

        /// <summary>
        /// Given an item from the Hashes enumeration, get the HashAlgorithm associated with it.
        /// </summary>
        /// <param name="hash">A Hashes enumeration item</param>
        /// <returns>The HashAlgorithm that matches the enumerated hash</returns>
        private static HashAlgorithm GetHashAlgorithm(Hashes hash)
        {
            HashAlgorithm hasher = null;
            switch (hash)
            {
                case Hashes.MD5:
                    hasher = new MD5CryptoServiceProvider();
                    break;
                case Hashes.SHA1:
                    hasher = new SHA1CryptoServiceProvider();
                    break;
                case Hashes.SHA256:
                    hasher = new SHA256Managed();
                    break;
                case Hashes.SHA384:
                    hasher = new SHA384Managed();
                    break;
                case Hashes.SHA512:
                    hasher = new SHA512Managed();
                    break;
                case Hashes.RIPEMD160:
                    hasher = new RIPEMD160Managed();
                    break;
                //case Hashes.Whirlpool:
                //    hasher = new WhirlpoolManaged();
                //    break;
                // If we didn't get something we expected, default to MD5:
                default:
                    hasher = new MD5CryptoServiceProvider();
                    break;
            }
            return hasher;
        }

        #endregion
    }
}
