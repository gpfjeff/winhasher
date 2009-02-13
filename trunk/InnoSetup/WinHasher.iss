[Setup]
InternalCompressLevel=ultra
OutputDir=C:\Documents and Settings\Jeff\My Documents\Visual Studio 2005\Projects\WinHasher\InnoSetup\bin
OutputBaseFilename=WinHasher_1.4_Setup
VersionInfoVersion=1.4.0.0
VersionInfoCompany=GPF Comics
VersionInfoDescription=This program will install WinHasher on your computer.  WinHasher is a cryptographic file hashing program using the Microsoft .NET 2.0 Framework.
VersionInfoTextVersion=WinHasher Setup 1.4
VersionInfoCopyright=Copyright 2009, Jeffrey T. Darlington.
AppCopyright=Copyright 2009, Jeffrey T. Darlington.
AppName=WinHasher
AppVerName=WinHasher 1.4
LicenseFile=C:\Documents and Settings\Jeff\My Documents\Visual Studio 2005\Projects\WinHasher\InnoSetup\gpl.rtf
PrivilegesRequired=poweruser
MinVersion=4.1.1998,5.0.2195sp3
DefaultDirName={pf}\WinHasher
DefaultGroupName=WinHasher
AppID=GPFComicsWinHasher
UninstallDisplayIcon={app}\WinHasher.exe
Compression=lzma/ultra
ChangesEnvironment=true
AppPublisher=GPF Comics
AppPublisherURL=http://www.gpf-comics.com/
AppSupportURL=http://www.gpf-comics.com/dl/winhasher/
AppUpdatesURL=http://www.gpf-comics.com/dl/winhasher/
AppVersion=WinHasher 1.4
UninstallDisplayName=WinHasher 1.4
SetupIconFile=C:\Documents and Settings\Jeff\My Documents\Visual Studio 2005\Projects\WinHasher\WinHasher\Icon1.ico
[Files]
Source: ..\WinHasherCore\bin\Release\WinHasherCore.dll; DestDir: {app}; Components: Windows_application Console_applications
Source: ..\WinHasher\bin\Release\WinHasher.exe; DestDir: {app}; Components: Windows_application
Source: ..\hash\bin\Release\hash.exe; DestDir: {app}; Components: Console_applications
Source: ..\md5\bin\Release\md5.exe; DestDir: {app}; Components: Console_applications
Source: ..\sha1\bin\Release\sha1.exe; DestDir: {app}; Components: Console_applications
Source: ..\..\Mandelbrot_Madness\PathTweaker\bin\Release\PathTweaker.exe; DestDir: {app}; Components: Console_applications
Source: cmdhelp.html; DestDir: {app}; Flags: ignoreversion; Components: Console_applications and HTML_help_file
Source: guihelp.html; DestDir: {app}; Flags: ignoreversion; Components: Windows_application and HTML_help_file
Source: gpl.html; DestDir: {app}; Flags: ignoreversion; Components: HTML_help_file
[Components]
Name: Windows_application; Description: Check this box to install the Windows GUI version of WinHasher; Types: custom compact full
Name: Windows_application\Desktop_icon; Description: Place an icon for WinHasher on your desktop.  If you are an administrator, this will make the icon available to all users.; Types: custom full
Name: Windows_application\QuickLaunch_icon; Description: Place an icon for WinHasher on your Quick Launch toolbar.  Note that if you are an administrator, this will only install the icon for yourself.; Types: full
Name: Windows_application\MD5_SendTo; Description: Add an MD5 shortcut to the Send To menu; Types: custom full
Name: Windows_application\SHA1_SendTo; Description: Add an SHA-1 shortcut to the Send To menu; Types: custom full
Name: Windows_application\SHA256_SendTo; Description: Add an SHA-256 shortcut to the Send To menu; Types: full
Name: Windows_application\SHA384_SendTo; Description: Add an SHA-384 shortcut to the Send To menu; Types: full
Name: Windows_application\SHA512_SendTo; Description: Add an SHA-512 shortcut to the Send To menu; Types: full
Name: Windows_application\RIPEMD160_SendTo; Description: Add an RIPEMD-160 shortcut to the Send To menu; Types: full
Name: Windows_application\Whirlpool_SendTo; Description: Add a Whirlpool shortcut to the Send To menu; Types: full
Name: Windows_application\Tiger_SendTo; Description: Add a Tiger shortcut to the Send To menu; Types: full
Name: Console_applications; Description: Check this box to install the console (command-line) versions of WinHasher.  This will also add the WinHasher program path to your PATH environment variable.; Types: custom full
Name: HTML_help_file; Description: Check this box to install the HTML help files, which will be accessible through the Start Menu.; Types: full compact custom
[Icons]
Name: {group}\WinHasher; Filename: {app}\WinHasher.exe; WorkingDir: {userdocs}; IconFilename: {app}\WinHasher.exe; IconIndex: 0; Components: Windows_application; Comment: WinHasher allows you to computer cryptographic hashes of files, or compare the hashes of multiple files
Name: {group}\Command-line Help; Filename: {app}\cmdhelp.html; WorkingDir: {app}; Components: Console_applications and HTML_help_file; Comment: Display help information for the command-line version of WinHasher in your default Web browser
Name: {group}\Windows App Help; Filename: {app}\guihelp.html; WorkingDir: {app}; Components: Windows_application and HTML_help_file; Comment: Display help information for the Windows version of WinHasher in your default Web browser
Name: {group}\Uninstall WinHasher; Filename: {uninstallexe}; WorkingDir: {app}; Comment: Uninstall all WinHasher components
Name: {commondesktop}\WinHasher; Filename: {app}\WinHasher.exe; WorkingDir: {userdocs}; IconFilename: {app}\WinHasher.exe; IconIndex: 0; Components: Windows_application\Desktop_icon; Comment: WinHasher allows you to computer cryptographic hashes of files, or compare the hashes of multiple files
Name: {userappdata}\Microsoft\Internet Explorer\Quick Launch\WinHasher; Filename: {app}\WinHasher.exe; WorkingDir: {userdocs}; IconFilename: {app}\WinHasher.exe; IconIndex: 0; Components: Windows_application\QuickLaunch_icon; Comment: WinHasher allows you to computer cryptographic hashes of files, or compare the hashes of multiple files
Name: {sendto}\WinHasher\MD5; Filename: {app}\WinHasher.exe; Parameters: -md5; WorkingDir: {userdocs}; IconFilename: {app}\WinHasher.exe; IconIndex: 0; Components: Windows_application\MD5_SendTo
Name: {sendto}\WinHasher\SHA-1; Filename: {app}\WinHasher.exe; Parameters: -sha1; WorkingDir: {userdocs}; IconFilename: {app}\WinHasher.exe; IconIndex: 0; Components: Windows_application\SHA1_SendTo
Name: {sendto}\WinHasher\SHA-256; Filename: {app}\WinHasher.exe; Parameters: -sha256; WorkingDir: {userdocs}; IconFilename: {app}\WinHasher.exe; IconIndex: 0; Components: Windows_application\SHA256_SendTo
Name: {sendto}\WinHasher\SHA-384; Filename: {app}\WinHasher.exe; Parameters: -sha384; WorkingDir: {userdocs}; IconFilename: {app}\WinHasher.exe; IconIndex: 0; Components: Windows_application\SHA384_SendTo
Name: {sendto}\WinHasher\SHA-512; Filename: {app}\WinHasher.exe; Parameters: -sha512; WorkingDir: {userdocs}; IconFilename: {app}\WinHasher.exe; IconIndex: 0; Components: Windows_application\SHA512_SendTo
Name: {sendto}\WinHasher\RIPEMD-160; Filename: {app}\WinHasher.exe; Parameters: -ripemd160; WorkingDir: {userdocs}; IconFilename: {app}\WinHasher.exe; IconIndex: 0; Components: Windows_application\RIPEMD160_SendTo
Name: {sendto}\WinHasher\Whirlpool; Filename: {app}\WinHasher.exe; Parameters: -whirlpool; WorkingDir: {userdocs}; IconFilename: {app}\WinHasher.exe; IconIndex: 0; Components: Windows_application\Whirlpool_SendTo
Name: {sendto}\WinHasher\Tiger; Filename: {app}\WinHasher.exe; Parameters: -tiger; WorkingDir: {userdocs}; IconFilename: {app}\WinHasher.exe; IconIndex: 0; Components: Windows_application\Tiger_SendTo
[Run]
Filename: {app}\PathTweaker.exe; Parameters: "-add ""{app}"""; WorkingDir: {app}; Flags: runminimized runhidden; Components: Console_applications; StatusMsg: "Adding ""{app}"" to the PATH..."
[UninstallRun]
Filename: {app}\PathTweaker.exe; Parameters: "-remove ""{app}"""; WorkingDir: {app}; Flags: runhidden; Components: Console_applications
[Code]
function InitializeSetup(): Boolean;
var
   DotNetRegKey: String;
   DotNetDlURL:  String;
   ErrorCode:    Integer;
begin
   // Set up our constants, abstracted here to make changing them
   // later easier.  The first is the .NET 2.0 registry key.
   // Actually, it's the setup program's regkey, but a Google
   // search said this was the best place to look.  .NET setup will
   // not install if it finds this key, so if it's good enough for
   // Microsoft, it's good enough for us.
   DotNetRegKey := 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v2.0.50727';
   // The URL of where to download .NET:
   DotNetDlURL := 'http://msdn2.microsoft.com/en-us/netframework/aa731542.aspx';
   // Check the registry to see if the .NET registry key exists.
   // If not, then we go to work:
   if not RegKeyExists(HKLM, DotNetRegKey) then begin
      // Ask the user if they want to download .NET:
      if MsgBox('The Microsoft .NET Framework version 2.0 or higher is required to run this application, but I couldn''t find it installed on your system.  Would you like to download it now?', mbConfirmation, MB_YESNO) = IDYES then begin
         // Open the download URL in the default browser:
         ShellExec('open', DotNetDlURL, '', '', SW_SHOW, ewNoWait, ErrorCode);
      end
      // If they decided not to download .NET now, tell them they
      // can always get it from Windows Update:
      else begin
         MsgBox('Ok, but you can also install the framework through Windows Update.  This installer will now exit.', mbInformation, MB_OK);
      end;
      // In all cases above, we want to stop the installation here:
      Result := False;
   end
   // If .NET was found, everything is rosy.  Proceed with the
   // installation:
   else begin
      //MsgBox('Found .NET Framework 2.0 or higher', mbInformation, MB_OK);
      Result := True;
   end;
end;
