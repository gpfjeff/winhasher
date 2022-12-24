                                  WinHasher
                                 Version 1.7
                              Source ReadMe File

                        Jeffrey T. Darlington, Mmoi-FR
                                Dec, 22, 2022
                     https://github.com/gpfjeff/winhasher

WinHasher is a free, Open Source cryptographic hash or digest generator written in C# using Microsoft's .NET 6 since the 1.7 version (2.0 Framework before). It can be used to verify file download integrity, compare two or more files for modifications.

BUILDING WINHASHER
==================

This source distribution for WinHasher is a Microsoft Visual Studio 2022 Windows Application project.

If you've downloaded our source and built it yourself in the past, you may remember seeing a couple of Windows batch files that were used in tagging Subversion revision numbers into the application version numbers.  With our move to GitHub and git, these files are no longer necessary and have been removed from the project; you can simply load up the solution and build it as-is.  If this is your first time building WinHasher yourself, you can safely ignore this and move along.  (This is not the disclaimer you were looking for....)

Once WinHasher is built, make sure to copy the HTML files in the InnoSetup folder into the same location as the binaries.  If WinHasher cannot find these files, the Help button will become disabled.

BUILDING THE WINHASHER INSTALLER
================================

The WinHasher installer is built using Inno Setup 6. You will need to build WinHasher first to generate the executable before executing the script.

Officially, we'll only support OSes that can run .NET 6, so to be, Windows 7 SP1 and higher.
