/* AboutDialog.cs
 * 
 * PROGRAMMER:    Jeffrey T. Darlington
 * DATE:          September 4, 2007
 * PROJECT:       WinHasher GUI application
 * .NET VERSION:  2.0
 * REQUIRES:      (None)
 * REQUIRED BY:   MainForm
 * 
 * The About dialog for WinHasher.  Nothing fancy here.  It just takes a version string, a URL
 * to link to the official site, and a string containing the license the program is released
 * under (GPL v2).
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace com.gpfcomics.WinHasher
{
    public partial class AboutDialog : Form
    {
        // The URL our link goes to:
        private string URL;
        private string helpFile;

        // Our constructor, which basically just copies our inputs into the appropriate GUI elements:
        public AboutDialog(string version, string url, string license, string helpFile)
        {
            InitializeComponent();
            versionLabel.Text = version;
            gplTextBox.Text = license;
            linkLabel.Text = url;
            URL = url;
            toolTip1.IsBalloon = true;
            // See if the HTML help file is in the same location as the program.  If it isn't,
            // disable the Help button.
            this.helpFile = helpFile;
            if (!File.Exists(helpFile)) helpButton.Enabled = false;
        }

        // When the link button is clicked, take us to the URL using the default browser:
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // The following could actually fail, so we'll need to try & catch:
            try { System.Diagnostics.Process.Start(URL); }
            // If this fails, just tell the user to go there on their own:
            catch
            {
                MessageBox.Show("Error: Could not launch default Web browser. Please type in the " +
                    "URL manually.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // When the OK button is clicked, just close the form:
        private void okButton_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        // If the help button is clicked, try to open the HTML help file in the default browser:
        private void helpButton_Click(object sender, EventArgs e)
        {
            try { System.Diagnostics.Process.Start(helpFile); }
            catch
            {
                MessageBox.Show("Error: Could not launch default Web browser. Please use the " +
                    "Start menu shortcut to load the HTML help file manually.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}