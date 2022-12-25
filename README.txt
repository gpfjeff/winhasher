                                  WinHasher
                                 Version 1.7
                              Source ReadMe File

                        Jeffrey T. Darlington, Mmoi-FR
                                Dec, 22, 2022
                     https://github.com/gpfjeff/winhasher

WinHasher is a free, Open Source cryptographic hash or digest generator written in C# using Microsoft's .NET 6 since the 1.7 version (2.0 Framework before). It can be used to verify file download integrity, compare two or more files for modifications.

BUILDING WINHASHER
==================

Once WinHasher is built, make sure to copy the HTML files in the InnoSetup folder. If WinHasher cannot find these files, the Help button will become disabled.

BUILDING THE WINHASHER INSTALLER
================================

The WinHasher installer is built using Inno Setup 6. You will need to build WinHasher first to generate the executable before executing the script.

Officially, we'll only support OSes that can run .NET 6, so to be, Windows 7 SP1 and higher.
