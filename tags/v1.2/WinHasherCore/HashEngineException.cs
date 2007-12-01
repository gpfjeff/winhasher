/* HashEngineException.cs
 * 
 * PROGRAMMER:    Jeffrey T. Darlington
 * DATE:          August 31, 2007
 * PROJECT:       WinHasher MD5 core library
 * .NET VERSION:  2.0
 * REQUIRES:      See using statements
 * REQUIRED BY:   All WinHasher programs
 * 
 * This exception class is throw by any of the methods in the HashEngine class.  This isn't very
 * elaborate.  It mostly consists of a message property that lets you specify a friendly error
 * message to display back.  In general, catch all HashEngineExceptions thrown by HashEngine
 * methods and echo their Message properites for the user to see.  That's all there is to it.
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

namespace com.gpfcomics.WinHasher.Core
{
    public class HashEngineException : Exception
    {
        private string message;
        
        public HashEngineException(string message)
        {
            this.message = message;
        }

        public HashEngineException()
        {
            message = "An unknown exception occured in the hashing engine.";
        }

        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        public override string ToString()
        {
            return "Hash Engine Exception: " + message;
        }
    }
}
