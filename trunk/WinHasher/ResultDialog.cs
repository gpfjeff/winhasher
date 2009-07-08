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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using com.gpfcomics.WinHasher.Core;

namespace com.gpfcomics.WinHasher
{
    public partial class ResultDialog : Form
    {
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
            txtResult.Text = result;
            // Build the hash type label.  For this, we'll do switches on the hash and
            // output type to build a string that includes both, such as "SHA-1 / Base64".
            string labelText = "";
            switch (hash)
            {
                case Hashes.MD5:
                    labelText += "MD5";
                    break;
                case Hashes.SHA1:
                    labelText += "SHA-1";
                    break;
                case Hashes.SHA256:
                    labelText += "SHA-256";
                    break;
                case Hashes.SHA384:
                    labelText += "SHA-384";
                    break;
                case Hashes.SHA512:
                    labelText += "SHA-512";
                    break;
                case Hashes.Tiger:
                    labelText += "Tiger";
                    break;
                case Hashes.Whirlpool:
                    labelText += "Whirlpool";
                    break;
                case Hashes.RIPEMD160:
                    labelText += "RIPEMD-160";
                    break;
                default:
                    labelText += "Invalid Hash";
                    break;
            }
            labelText += " / ";
            switch (outputType)
            {
                case OutputType.Base64:
                    labelText += "Base64";
                    break;
                case OutputType.BubbleBabble:
                    labelText += "Bubble Babble";
                    break;
                case OutputType.CapHex:
                    labelText += "Hex (Caps)";
                    break;
                case OutputType.Hex:
                    labelText += "Hexadecimal";
                    break;
                default:
                    labelText += "Invalid Encoding";
                    break;
            }
            lblResult.Text = labelText + ":";
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
                lblCompareResult.Text = "Please enter a pre-computed hash value in the " +
                    "Compare To field to compare values.";
                lblCompareResult.ForeColor = SystemColors.ControlText;
                lblCompareResult.BackColor = SystemColors.Control;
            }
            // If the two strings match, then the generated hash matches the pre-existing
            // hash and the user can safely say the file is unaltered and intact:
            else if (String.Compare(txtResult.Text, txtCompare.Text) == 0)
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