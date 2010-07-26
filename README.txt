                                  WinHasher
                                 Version 1.6
                              Source ReadMe File

                            Jeffrey T. Darlington
                                July 26, 2010
                     http://code.google.com/p/winhasher/

WinHasher is a free, Open Source cryptographic hash or digest generator written in C# using Microsoft's .NET 2.0 Framework. It can be used to verify file download integrity, compare two or more files for modifications, and to some degree generate strong, unique passwords. 

BUILDING WINHASHER
==================

This source distribution for WinHasher is a Microsoft Visual Studio 2005 Windows Application project.  Although it was originally built in Visual Studio 2005, you should be able to open and compile it in Visual C# 2005 Express without any problem.  (In fact, I tend to use Visual C# 2005 Express for official builds.)  Note, however, that there are a few local modifications you may need to make to the files before building.

If you checked out the source from the source repository, you will find two Windows batch files, "new_revision_tag.bat" and "SubWCRev_batch.bat".  These scripts are actually Perl scripts (written for ActiveState's Active Perl) encased in a batch wrapper and are used to add the SVN revision number and copyright date to the officially builds.  "new_revision_tag.bat" is intended to be run as a pre-commit hook script and updates a random "tag" in a comment inside the "template" files to force SVN to always update the templates before a commit.  "SubWCRev_batch.bat", its companion script, is run as a post-commit and post-update script which runs SubWCRev.exe, which parses the templates and adds the revision and copyright date information.  If you wish to take advantage of these scripts, replace the $workingpath variable value with the path to the root of your working copy.  If you're running this on Windows, make sure to escape your back-slashes; if you're running it on a *NIX setup, remove the Windows batch information, replace the "shebang" line with the correct path to your Perl executable, and tweak the path strings in the @templates array with the correct path separators (forward slashes instead of back-slashes).  You will also need to configure your local SVN setup to execute these scripts on the appropriate hooks.

If you do not wish to take advantage of the hook scripts, look for the *.template files throughout the source tree.  Rename them to remove the ".template" extension, then edit them to replace the $WC*$ variables.

If you downloaded an "official" ZIP archive of the source rather than checking the code out of the repository, you can ignore the above comments about the hook scripts.  The "official" source archives have the scripts and templates removed and the templated files will already have the correct revision and copyright information in place.  It should be ready to build as-is.

Once WinHasher is built, make sure to copy the HTML files in the InnoSetup folder into the same location as the binaries.  If WinHasher cannot find these files, the Help button will become disabled.


BUILDING THE WINHASHER INSTALLER
================================

The WinHasher installer is built using Inno Setup 5.  I tend to use ISTool, which comes as an optional install with Inno Setup, to make writing the installer code a bit easier.  However, ISTool is not necessary for building the installer; the script should run in Inno Setup just fine.  You will need to build WinHasher first to generate the executable before executing the script.  Make sure to check the paths within the script and modify them to fit your building environment; I believe I have removed all the absolute paths and replaced them with relative paths, but you should double-check this before attempting to build the installer.

Note that the installer script is templated using the same commit hook scripts mentioned in the "Building WinHasher" section.  If you checked out the source from the repository, you may need to set up the hook scripts or otherwise manually tweak the files.

If you downloaded an "official" source archive, you can ignore this step, as the installer source is not distributed with the source archive.

