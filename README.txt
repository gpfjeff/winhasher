                                  WinHasher
                                 Version 1.7
                              Source ReadMe File

                            Jeffrey T. Darlington
                                July, 9, 2015
                     https://github.com/gpfjeff/winhasher

WinHasher is a free, Open Source cryptographic hash or digest generator written in C# using Microsoft's .NET 2.0 Framework. It can be used to verify file download integrity, compare two or more files for modifications, and to some degree generate strong, unique passwords. 

BUILDING WINHASHER
==================

This source distribution for WinHasher is a Microsoft Visual Studio 2012 Windows Application project.  Although it was originally built in Visual Studio 2012, you should be able to open and compile it in compatible Express versions without any problems.

If you've downloaded our source and built it yourself in the past, you may remember seeing a couple of Windows batch files that were used in tagging Subversion revision numbers into the application version numbers.  With our move to GitHub and git, these files are no longer necessary and have been removed from the project; you can simply load up the solution and build it as-is.  If this is your first time building WinHasher yourself, you can safely ignore this and move along.  (This is not the disclaimer you were looking for....)

Once WinHasher is built, make sure to copy the HTML files in the InnoSetup folder into the same location as the binaries.  If WinHasher cannot find these files, the Help button will become disabled.

Note that I'm planning to eventually do away with the separate MD5 and SHA-1 console applications (md5.exe and sha1.exe respectively) and replace them with Windows batch files that are "aliases" for the WinHahser console app (hash.exe) that "shortcut" your hash selection to the specified algorithm (i.e. sha1.bat calls hash.exe and defaults it to using SHA-1).  These batch files are currently in the InnoSetup folder and are installed by the installer if the user selects to install the console application.  For now, the md5 and sha1 projects are still part of the overall solution, so you can still build and use them if you wish.  However, they will not be distributed with official binary distributions of WinHasher nor will they be installed by the official installer.  Both console applications will be completely removed from the solution at a later date.

Once WinHasher is built, make sure to copy the XML schema files (the .XSD files) in the root of the source tree into the same location as the binaries.  If WinHasher cannot find these files, the update checker will fail because WinHasher will not be able to validate the XML used in these files.


BUILDING THE WINHASHER INSTALLER
================================

The WinHasher installer is built using Inno Setup 5.  I tend to use Inno Script Studio, which comes as an optional install with Inno Setup, to make writing the installer code a bit easier.  However, Inno Script Studio is not necessary for building the installer; the script should run in Inno Setup just fine.  You will need to build WinHasher first to generate the executable before executing the script.  Make sure to check the paths within the script and modify them to fit your building environment; I believe I have removed all the absolute paths and replaced them with relative paths, but you should double-check this before attempting to build the installer.

Note that Inno Setup no longer supports older versions of Windows.  Technically, WinHasher should build and run on any system that supports .NET 2.0; however, Inno Setup restricts us to "every Windows release since 2000".  Officially, we'll only support Windows XP and higher.