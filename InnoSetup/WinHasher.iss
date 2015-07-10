[Setup]
InternalCompressLevel=ultra
OutputDir=.\bin
OutputBaseFilename=WinHasher_1.7_Setup
VersionInfoVersion=1.7.0.0
VersionInfoCompany=GPF Comics
VersionInfoDescription=This program will install WinHasher on your computer.  WinHasher is a cryptographic file hashing program using the Microsoft .NET 2.0 Framework.
VersionInfoTextVersion=WinHasher Setup 1.7
VersionInfoCopyright=Copyright 2015, Jeffrey T. Darlington.
AppCopyright=Copyright 2015, Jeffrey T. Darlington.
AppName=WinHasher
AppVerName=WinHasher 1.7
LicenseFile=.\gpl.rtf
PrivilegesRequired=poweruser
MinVersion=0,5.0.2195sp3
DefaultDirName={pf}\WinHasher
DefaultGroupName=WinHasher
AppID=GPFComicsWinHasher
UninstallDisplayIcon={app}\WinHasher.exe
Compression=lzma/ultra
ChangesEnvironment=true
AppPublisher=GPF Comics
AppPublisherURL=http://www.gpf-comics.com/
AppSupportURL=https://github.com/gpfjeff/winhasher
AppUpdatesURL=https://github.com/gpfjeff/winhasher
AppVersion=WinHasher 1.7
UninstallDisplayName=WinHasher 1.7
SetupIconFile=..\WinHasher\Icon1.ico

[Files]
Source: "..\WinHasherCore\bin\Release\WinHasherCore.dll"; DestDir: "{app}"; Components: Windows_application Console_applications
Source: "..\BouncyCastle.Crypto.dll"; DestDir: "{app}"; Components: Windows_application Console_applications
Source: "..\GPFUpdateChecker.dll"; DestDir: "{app}"; Components: Windows_application
Source: "..\gpf_update_checker1.xsd"; DestDir: "{app}"; Flags: ignoreversion; Components: Windows_application
Source: "..\WinHasher\bin\Release\WinHasher.exe"; DestDir: "{app}"; Components: Windows_application
Source: "..\hash\bin\Release\hash.exe"; DestDir: "{app}"; Components: Console_applications
Source: "PathTweaker.exe"; DestDir: "{app}"; Components: Console_applications
Source: "cmdhelp.html"; DestDir: "{app}"; Flags: ignoreversion; Components: Console_applications and HTML_help_file
Source: "guihelp.html"; DestDir: "{app}"; Flags: ignoreversion; Components: Windows_application and HTML_help_file
Source: "gpl.html"; DestDir: "{app}"; Flags: ignoreversion; Components: HTML_help_file
Source: "md5.bat"; DestDir: "{app}"; Flags: ignoreversion; Components: Console_applications
Source: "sha1.bat"; DestDir: "{app}"; Flags: ignoreversion; Components: Console_applications
Source: "sha224.bat"; DestDir: "{app}"; Flags: ignoreversion; Components: Console_applications
Source: "sha256.bat"; DestDir: "{app}"; Flags: ignoreversion; Components: Console_applications
Source: "sha384.bat"; DestDir: "{app}"; Flags: ignoreversion; Components: Console_applications
Source: "sha512.bat"; DestDir: "{app}"; Flags: ignoreversion; Components: Console_applications
Source: "ripemd128.bat"; DestDir: "{app}"; Flags: ignoreversion; Components: Console_applications
Source: "ripemd160.bat"; DestDir: "{app}"; Flags: ignoreversion; Components: Console_applications
Source: "ripemd256.bat"; DestDir: "{app}"; Flags: ignoreversion; Components: Console_applications
Source: "ripemd320.bat"; DestDir: "{app}"; Flags: ignoreversion; Components: Console_applications
Source: "whirlpool.bat"; DestDir: "{app}"; Flags: ignoreversion; Components: Console_applications
Source: "tiger.bat"; DestDir: "{app}"; Flags: ignoreversion; Components: Console_applications
Source: "gost3411.bat"; DestDir: "{app}"; Flags: ignoreversion; Components: Console_applications

[Components]
Name: "Windows_application"; Description: "Check this box to install the Windows GUI version of WinHasher"; Types: custom compact full
Name: "Windows_application\Desktop_icon"; Description: "Place an icon for WinHasher on your desktop.  (If you are an administrator, this will make the icon available to all users.)"; Types: custom full
Name: "Windows_application\QuickLaunch_icon"; Description: "Place an icon for WinHasher on your Quick Launch toolbar.  (This will only install the icon for yourself, not other users.)"; Types: full
Name: "Console_applications"; Description: "Check this box to install the console (command-line) versions of WinHasher.  This will also add the WinHasher program path to your PATH environment variable."; Types: custom full
Name: "HTML_help_file"; Description: "Check this box to install the HTML help files, which will be accessible through the Start Menu."; Types: full compact custom

[Icons]
Name: "{group}\WinHasher"; Filename: "{app}\WinHasher.exe"; WorkingDir: "{userdocs}"; IconFilename: "{app}\WinHasher.exe"; IconIndex: 0; Comment: "WinHasher allows you to computer cryptographic hashes of files, or compare the hashes of multiple files"; Components: Windows_application
Name: "{group}\Command-line Help"; Filename: "{app}\cmdhelp.html"; WorkingDir: "{app}"; Comment: "Display help information for the command-line version of WinHasher in your default Web browser"; Components: Console_applications and HTML_help_file
Name: "{group}\Windows App Help"; Filename: "{app}\guihelp.html"; WorkingDir: "{app}"; Comment: "Display help information for the Windows version of WinHasher in your default Web browser"; Components: Windows_application and HTML_help_file
Name: "{group}\Uninstall WinHasher"; Filename: "{uninstallexe}"; WorkingDir: "{app}"; Comment: "Uninstall all WinHasher components"
Name: "{commondesktop}\WinHasher"; Filename: "{app}\WinHasher.exe"; WorkingDir: "{userdocs}"; IconFilename: "{app}\WinHasher.exe"; IconIndex: 0; Comment: "WinHasher allows you to computer cryptographic hashes of files, or compare the hashes of multiple files"; Components: Windows_application\Desktop_icon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\WinHasher"; Filename: "{app}\WinHasher.exe"; WorkingDir: "{userdocs}"; IconFilename: "{app}\WinHasher.exe"; IconIndex: 0; Comment: "WinHasher allows you to computer cryptographic hashes of files, or compare the hashes of multiple files"; Components: Windows_application\QuickLaunch_icon

[Run]
Filename: {app}\PathTweaker.exe; Parameters: "-add ""{app}"""; WorkingDir: {app}; Flags: runminimized runhidden; Components: Console_applications; StatusMsg: "Adding ""{app}"" to the PATH..."
[UninstallRun]
Filename: {app}\PathTweaker.exe; Parameters: "-remove ""{app}"""; WorkingDir: {app}; Flags: runhidden; Components: Console_applications
[Code]
// InitializeSetup:  Check to see if .NET 2.0 is installed and abort if it isn't.
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
         MsgBox('OK, but you can also install the framework through Windows Update.  This installer will now exit.', mbInformation, MB_OK);
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
// DeinitializeUninstall:  Offer to remove our registry keys upon uninstall if the user
// doesn't want to keep them.
procedure DeinitializeUninstall();
var
	WinHasherRegKey: String;
	GPFComicsRegKey: String;
begin
	// Define both the GPF Comics key and the WinHasher key.  We'll definitely delete the
	// WinHasher key if present and also the GPF Comics key if no other subkeys exists.
	GPFComicsRegKey := 'SOFTWARE\GPF Comics';
	WinHasherRegKey := GPFComicsRegKey + '\WinHasher';
	// If the WinHasher key exists...
	if RegKeyExists(HKCU, WinHasherRegKey) then begin
		// Confirm with the user that we'll delete their settings:
		if MsgBox('Would you like to remove your saved preferences from the registry?  If you plan to reinstall WinHasher, you should probably say no.', mbConfirmation, MB_YESNO) = IDYES then begin
			// They said OK, so try to delete it:
			if (RegDeleteKeyIncludingSubkeys(HKCU, WinHasherRegKey)) then begin
				// If that worked, also delete the GPF Comics key if it's empty:
				RegDeleteKeyIfEmpty(HKCU, GPFComicsRegKey);
			end
			// The delete didn't work.  We should probably warn the user that we tried and
			// failed.  Note that we don't really care to inform them if (a) the keys don't
			// exist (why bother?) and (b) if they said no to removing them.
			else begin
				MsgBox('Your preferences could not be deleted for some reason.  Sorry...', mbInformation, MB_OK);
			end;
		end;
	end;
end;
