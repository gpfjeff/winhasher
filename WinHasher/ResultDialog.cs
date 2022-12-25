/* ResultDialog.cs
 * 
 * PROGRAMMER:    Jeffrey T. Darlington
 * DATE:          July 8, 2009
 * PROJECT:       WinHasher GUI application
 * .NET VERSION:  2.0
 * REQUIRES:      (None)
 * REQUIRED BY:   Program
 * 
 * The Result Dialog for WinHasher.  This is used primary by the command-line execution mode
 * of the core WinHasher program, which in turn is used by the Send To shortcuts added by the
 * installer.  When called in this mode, WinHasher originally displayed a standard MessageBox
 * dialog, which had disadvantages in that you could not select the hash value and copy/paste
 * it elsewhere.  This meant you had to visually compare values from somewhere else with the
 * contents of the dialog, which wasn't pleasant.
 * 
 * By contrast, this dedicated dialog does a number of unique things.  First, it explicitly
 * identifies both the hash used and the output type (hex, Base64, etc.); the original method
 * only identified the hash.  Second, the hash value is displayed in a read-only text box,
 * which can subsequently be selected, copied, and pasted for external comparison.  Third, a
 * second text box allows you to copy a hash from another location (say a Web site) and paste
 * it into the box.  If the two hashes match, a label below the box turns green and displays
 * a success message; if the hashes do not match, the label turns red and displays an
 * unsuccessful message.  All three of these additions make the result much more robust and
 * easier to work with.
 * 
 * UPDATE February 8, 2010 (1.6):  Added "case kludge" for single file hash results.  If the
 * user selects an output type that typically does not have mixed case (currently everything
 * except Base64), pasting a comparison value with the opposite case (i.e. upper-case when
 * lower-case is expected) causes the comparison to fail.  This kludge forces the correct case
 * when a specific case is expected.
 * 
 * UPDATE June 29, 2015 (1.7):  Updates for Bouncy Castle conversion.  Minor enhancement to the
 * comparison text box validation to strip whitespace off the ends.
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
using System.Drawing;
using System.Windows.Forms;
using com.gpfcomics.WinHasher.Core;

namespace com.gpfcomics.WinHasher
{
    public partial class ResultDialog : Form
    {
        /// <summary>
        /// The <see cref="OutputType"/> of the result hash
        /// </summary>
        private OutputType outputType { get; set; }

        /// <summary>
        /// The ResultDialog constructor
        /// </summary>
        /// <param name="result">A <see cref="String"/> containing the hash produced by the
        /// <see cref="HashEngine"/></param>
        /// <param name="hash">A <see cref="Hashes"/> enum value indicating which hashing
        /// algorithm was used</param>
        /// <param name="outputType">A <see cref="OutputType"/> enum value indicating which
        /// output encoding type was used</param>
        public ResultDialog(string result, Hashes hash, OutputType outputType)
        {
            // Do the usual initialization:
            InitializeComponent();

            // Put the hash text in the result box:
            this.txtResult.Text = result;

            // Hold onto the output type for comparison later:
            this.outputType = outputType;
        }

        /// <summary>
        /// What to do when the OK button is clicked.  For our purposes, we'll just close
        /// the dialog and exit the program.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        /// <summary>
        /// What to do when the Compare To field is populated.  This compares the contents
        /// of that field to the generated hash text and sees if they match.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtCompare_TextChanged(object sender, EventArgs e)
        {
            // If the Compare To field is empty (the default) we don't want to show a false
            // error.  In this case, just tell the user they need to enter a value if they
            // want us to compare hashes for them.  Note that we'll use the SystemColors
            // object to set the colors here, so it will match the user interface.
            if (String.IsNullOrEmpty(txtCompare.Text))
            {
                lblCompareResult.Text = "Please enter a pre-computed hash value in the \"Compare To\" field to compare values.";
                lblCompareResult.ForeColor = SystemColors.ControlText;
                lblCompareResult.BackColor = SystemColors.Control;
            }
            else
            {
                // This is a convenience kludge.  Most websites that post hashes tend to use
                // lower-case hexadecimal, which is why we've set that to our default everywhere.
                // That said, there are sites out there that post hashes in upper-case, which
                // makes it a pain to compare against if our default is lower-case.  Originally,
                // the only way to change the behavior of the Send To shortcuts was to change
                // the command line of the shortcut, which isn't something every user knows how
                // to do.  So this kludge tweaks the comparison string (if set) to force the
                // value to match the case we've specified.  In the default case (lower-case
                // hex), that means forcing the hash to be lower-case, even if pasted in as
                // upper-case.  The same goes for Bubble Babble, which is almost always lower-
                // case, and the inverse is true for our "CapHex" setting (force it to be
                // upper-case).  For *all* instances, we'll tack on a Trim() to remove any
                // excess whitespace on either end; I've lost count of how many times I've copied
                // a hash from a website and it was marked as "no match" just because some
                // extra whitespace was accidentally tacked onto the end.
                switch (outputType)
                {
                    case OutputType.Hex:
                    case OutputType.BubbleBabble:
                        txtCompare.Text = txtCompare.Text.Trim().ToLower();
                        break;
                    case OutputType.CapHex:
                        txtCompare.Text = txtCompare.Text.Trim().ToUpper();
                        break;
                    default: // OutputType.Base64:
                        txtCompare.Text = txtCompare.Text.Trim();
                        break;
                }

                // If the two strings match, then the generated hash matches the pre-existing
                // hash and the user can safely say the file is unaltered and intact:
                if (String.Compare(txtResult.Text, txtCompare.Text) == 0)
                {
                    lblCompareResult.Text = "The two hashes match.";
                    lblCompareResult.ForeColor = Color.White;
                    lblCompareResult.BackColor = Color.Green;
                }
                // Otherwise, the strings don't match, the hashes don't match, and the file is
                // not what it claims to be:
                else
                {
                    lblCompareResult.Text = "The two hashes do not match.";
                    lblCompareResult.ForeColor = Color.Yellow;
                    lblCompareResult.BackColor = Color.Red;
                }
            }
        }
    }
}