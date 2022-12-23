/* ConsoleStatusUpdater.cs
 * 
 * PROGRAMMER:    Jeffrey T. Darlington
 * DATE:          August 20, 2009
 * PROJECT:       WinHasher core library
 * .NET VERSION:  2.0
 * REQUIRES:      See using statements
 * REQUIRED BY:   All WinHasher programs
 * 
 * This very simple class provides us with a means of passing updates to the Windows console
 * for command-line versions of the WinHasher programs (hash, md5, and sha1).  An object of this
 * class can be passed into certain methods of the HashEngine class.  When present, the HashEngine
 * will print out the percentage complete of its progress to the Console, similar to the progress
 * bar used in the GUI app but as a textual number ("95%").
 * 
 * It is assumed that when this is called, an initial status will be displayed such as:
 * 
 *    Computing SHA-1 of C:\some_file.zip...   0%
 * 
 * The ConsoleStatusUpdater replace in place the "  0%" string with the updated percentage
 * complete, ensuring that no extra lines are needed.  Note the extra spaces in front of the
 * zero in the sample; this object assumes it will overwrite the last four characters, meaning
 * three characters for the number (up to 100) plus the percent sign.
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

namespace com.gpfcomics.WinHasher.Core
{
    /// <summary>
    /// A class to update the Windows <see cref="Console"/> with the percentage complete
    /// </summary>
    public class ConsoleStatusUpdater
    {
        /// <summary>
        /// Set the percentage complete
        /// </summary>
        /// <param name="percent">An integer representing the current percentage complete</param>
        public void SetPercentDone(int percent)
        {
            // Assume the percent can be up to three digits plus a percent sign, go back
            // four spaces and overwrite them with the current percent value, including
            // a new percent sign:
            Console.CursorLeft = Console.CursorLeft - 4;
            Console.Write(percent.ToString().PadLeft(3, ' ') + "%");
        }
    }
}
