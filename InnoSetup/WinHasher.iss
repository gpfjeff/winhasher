[Setup]
AppName=WinHasher
AppVersion=1.7
AppCopyright=Copyright 2022, Jeffrey T. Darlington, Mmoi-Fr
SetupIconFile=..\WinHasher\Icon1.ico

ArchitecturesInstallIn64BitMode=x64
DefaultDirName={autopf}\WinHasher
DefaultGroupName=WinHasher

LicenseFile=.\gpl.rtf

PrivilegesRequired=admin
PrivilegesRequiredOverridesAllowed=dialog

ChangesEnvironment=true

OutputDir=.\bin
OutputBaseFilename=WinHasher_Setup

MinVersion=6.1.7601sp1

VersionInfoVersion=1.7.0.0
VersionInfoCompany=GPF Comics
VersionInfoDescription=WinHasher install program. WinHasher is a cryptographic file hashing program using Microsoft .NET 6 and BouncyCastle 2.0.
VersionInfoCopyright=Copyright 2022, Jeffrey T. Darlington, Mmoi-Fr.

AppID=GPFComicsWinHasher
UninstallDisplayIcon={app}\WinHasher.exe

AppPublisher=GPF Comics
AppPublisherURL=http://www.gpf-comics.com/
AppSupportURL=https://github.com/gpfjeff/winhasher
AppUpdatesURL=https://github.com/gpfjeff/winhasher

[Files]
Source: "..\WinHasherCore\bin\Release\netstandard2.1\WinHasherCore.dll"; DestDir: "{app}"; Components: Windows_application Console_applications
Source: "..\WinHasher\bin\Release\net6.0-windows\BouncyCastle.Cryptography.dll"; DestDir: "{app}"; Components: Windows_application Console_applications
Source: "..\WinHasher\bin\Release\net6.0-windows\WinHasher.dll"; DestDir: "{app}"; Components: Windows_application
Source: "..\WinHasher\bin\Release\net6.0-windows\WinHasher.exe"; DestDir: "{app}"; Components: Windows_application
Source: "..\WinHasher\bin\Release\net6.0-windows\WinHasher.runtimeconfig.json"; DestDir: "{app}"; Components: Windows_application
Source: "..\hash\bin\Release\net6.0\hash.dll"; DestDir: "{app}"; Components: Console_applications
Source: "..\hash\bin\Release\net6.0\hash.exe"; DestDir: "{app}"; Components: Console_applications
Source: "..\hash\bin\Release\net6.0\hash.runtimeconfig.json"; DestDir: "{app}"; Components: Console_applications
Source: "cmdhelp.html"; DestDir: "{app}"; Flags: ignoreversion; Components: Console_applications and HTML_help_file
Source: "guihelp.html"; DestDir: "{app}"; Flags: ignoreversion; Components: Windows_application and HTML_help_file
Source: "gpl.html"; DestDir: "{app}"; Flags: ignoreversion; Components: HTML_help_file
Source: "Aliases\md5.bat"; DestDir: "{app}"; Flags: ignoreversion; Components: Console_applications\Alias
Source: "Aliases\sha1.bat"; DestDir: "{app}"; Flags: ignoreversion; Components: Console_applications\Alias
Source: "Aliases\sha224.bat"; DestDir: "{app}"; Flags: ignoreversion; Components: Console_applications\Alias
Source: "Aliases\sha256.bat"; DestDir: "{app}"; Flags: ignoreversion; Components: Console_applications\Alias
Source: "Aliases\sha384.bat"; DestDir: "{app}"; Flags: ignoreversion; Components: Console_applications\Alias
Source: "Aliases\sha512.bat"; DestDir: "{app}"; Flags: ignoreversion; Components: Console_applications\Alias
Source: "Aliases\ripemd128.bat"; DestDir: "{app}"; Flags: ignoreversion; Components: Console_applications\Alias
Source: "Aliases\ripemd160.bat"; DestDir: "{app}"; Flags: ignoreversion; Components: Console_applications\Alias
Source: "Aliases\ripemd256.bat"; DestDir: "{app}"; Flags: ignoreversion; Components: Console_applications\Alias
Source: "Aliases\ripemd320.bat"; DestDir: "{app}"; Flags: ignoreversion; Components: Console_applications\Alias
Source: "Aliases\whirlpool.bat"; DestDir: "{app}"; Flags: ignoreversion; Components: Console_applications\Alias
Source: "Aliases\tiger.bat"; DestDir: "{app}"; Flags: ignoreversion; Components: Console_applications\Alias
Source: "Aliases\gost3411.bat"; DestDir: "{app}"; Flags: ignoreversion; Components: Console_applications\Alias
Source: "Aliases\sha3-224.bat"; DestDir: "{app}"; Flags: ignoreversion; Components: Console_applications\Alias
Source: "Aliases\sha3-256.bat"; DestDir: "{app}"; Flags: ignoreversion; Components: Console_applications\Alias
Source: "Aliases\sha3-384.bat"; DestDir: "{app}"; Flags: ignoreversion; Components: Console_applications\Alias
Source: "Aliases\sha3-512.bat"; DestDir: "{app}"; Flags: ignoreversion; Components: Console_applications\Alias

[Components]
Name: "Windows_application"; Description: "Install the Windows GUI version of WinHasher"; Types: custom compact full
Name: "Windows_application\ContextMenu"; Description: "Register WinHasher in the context menu (right clic)"; Types: custom compact full; Check: IsAdmin();
Name: "Windows_application\Desktop_icon"; Description: "Place a shortcut to WinHasher on the desktop."; Types: custom full
Name: "Windows_application\QuickLaunch_icon"; Description: "Place an icon for WinHasher on the Quick Launch toolbar."; Types: full
Name: "Console_applications"; Description: "Install the console (command-line) versions of WinHasher."; Types: custom full
Name: "Console_applications\Path"; Description: "Add the WinHasher program path to your PATH environment variable."; Types: custom full
Name: "Console_applications\Alias"; Description: "Install the aliases for the known algorithm"; Types: custom full
Name: "HTML_help_file"; Description: "Install the HTML help files, which will be accessible through the Start Menu."; Types: full compact custom

[Icons]
Name: "{group}\WinHasher"; Filename: "{app}\WinHasher.exe"; WorkingDir: "{autodocs}"; IconFilename: "{app}\WinHasher.exe"; IconIndex: 0; Comment: "WinHasher allows you to computer cryptographic hashes of files, or compare the hashes of multiple files"; Components: Windows_application
Name: "{group}\Command-line Help"; Filename: "{app}\cmdhelp.html"; WorkingDir: "{app}"; Comment: "Display help information for the command-line version of WinHasher in your default Web browser"; Components: Console_applications and HTML_help_file
Name: "{group}\Windows App Help"; Filename: "{app}\guihelp.html"; WorkingDir: "{app}"; Comment: "Display help information for the Windows version of WinHasher in your default Web browser"; Components: Windows_application and HTML_help_file
Name: "{group}\Uninstall WinHasher"; Filename: "{uninstallexe}"; WorkingDir: "{app}"; Comment: "Uninstall all WinHasher components"
Name: "{autodesktop}\WinHasher"; Filename: "{app}\WinHasher.exe"; WorkingDir: "{autodocs}"; IconFilename: "{app}\WinHasher.exe"; IconIndex: 0; Comment: "WinHasher allows you to computer cryptographic hashes of files, or compare the hashes of multiple files"; Components: Windows_application\Desktop_icon
Name: "{autoappdata}\Microsoft\Internet Explorer\Quick Launch\WinHasher"; Filename: "{app}\WinHasher.exe"; WorkingDir: "{autodocs}"; IconFilename: "{app}\WinHasher.exe"; IconIndex: 0; Comment: "WinHasher allows you to computer cryptographic hashes of files, or compare the hashes of multiple files"; Components: Windows_application\QuickLaunch_icon

[Registry]
// PATH -- https://stackoverflow.com/questions/3304463/how-do-i-modify-the-path-environment-variable-when-running-an-inno-setup-install/3431379#3431379
Root: HKA; Subkey: "Environment"; ValueType: expandsz; ValueName: "Path"; ValueData: "{olddata};{app}"; Check: (not IsAdmin()) and NeedsAddPath(True, '{app}'); Components: Console_applications\Path
Root: HKLM; Subkey: "SYSTEM\CurrentControlSet\Control\Session Manager\Environment"; ValueType: expandsz; ValueName: "Path"; ValueData: "{olddata};{app}"; Check: IsAdmin() and NeedsAddPath(False, '{app}'); Components: Console_applications\Path
// Context menu
Root: HKLM; Subkey: "Software\Classes\*\shell\Hash"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "Software\Classes\*\shell\Hash"; ValueType: string; ValueName: "SubCommands"; ValueData: "WinHasherSHA-1;WinHasherSHA-224;WinHasherSHA-256;WinHasherSHA-384;WinHasherSHA-512;WinHasherRIPEMD-128;WinHasherRIPEMD-160;WinHasherRIPEMD-256;WinHasherRIPEMD-320;WinHasherWHIRLPOOL;WinHasherTIGER;WinHasherGOST;WinHasherSHA3-224;WinHasherSHA3-256;WinHasherSHA3-384;WinHasherSHA3-512"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "Software\Classes\*\shell\Hash"; ValueType: string; ValueName: "Icon"; ValueData: "{app}\WinHasher.exe"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
//
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherSHA-1"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherSHA-1"; ValueType: string; ValueName: "MUIVerb"; ValueData: "SHA-1"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherSHA-1\command"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherSHA-1\command"; ValueType: string; ValueData: "{app}\WinHasher.exe -sha1 ""%1"""; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
//
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherSHA-224"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherSHA-224"; ValueType: string; ValueName: "MUIVerb"; ValueData: "SHA-224"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherSHA-224\command"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherSHA-224\command"; ValueType: string; ValueData: "{app}\WinHasher.exe -sha-224 ""%1"""; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
//
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherSHA-256"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherSHA-256"; ValueType: string; ValueName: "MUIVerb"; ValueData: "SHA-256"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherSHA-256\command"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherSHA-256\command"; ValueType: string; ValueData: "{app}\WinHasher.exe -sha-256 ""%1"""; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
//
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherSHA-384"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherSHA-384"; ValueType: string; ValueName: "MUIVerb"; ValueData: "SHA-384"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherSHA-384\command"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherSHA-384\command"; ValueType: string; ValueData: "{app}\WinHasher.exe -sha-384 ""%1"""; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
//
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherSHA-512"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherSHA-512"; ValueType: string; ValueName: "MUIVerb"; ValueData: "SHA-512"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherSHA-512\command"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherSHA-512\command"; ValueType: string; ValueData: "{app}\WinHasher.exe -sha-512 ""%1"""; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
//
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherRIPEMD-128"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherRIPEMD-128"; ValueType: string; ValueName: "MUIVerb"; ValueData: "RIPEMD-128"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherRIPEMD-128\command"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherRIPEMD-128\command"; ValueType: string; ValueData: "{app}\WinHasher.exe -ripemd128 ""%1"""; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
//
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherRIPEMD-160"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherRIPEMD-160"; ValueType: string; ValueName: "MUIVerb"; ValueData: "RIPEMD-160"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherRIPEMD-160\command"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherRIPEMD-160\command"; ValueType: string; ValueData: "{app}\WinHasher.exe -ripemd160 ""%1"""; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
//
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherRIPEMD-256"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherRIPEMD-256"; ValueType: string; ValueName: "MUIVerb"; ValueData: "RIPEMD-256"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherRIPEMD-256\command"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherRIPEMD-256\command"; ValueType: string; ValueData: "{app}\WinHasher.exe -ripemd256 ""%1"""; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
//
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherRIPEMD-320"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherRIPEMD-320"; ValueType: string; ValueName: "MUIVerb"; ValueData: "RIPEMD-320"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherRIPEMD-320\command"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherRIPEMD-320\command"; ValueType: string; ValueData: "{app}\WinHasher.exe -ripemd320 ""%1"""; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
//
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherWHIRLPOOL"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherWHIRLPOOL"; ValueType: string; ValueName: "MUIVerb"; ValueData: "WHIRLPOOL"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherWHIRLPOOL\command"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherWHIRLPOOL\command"; ValueType: string; ValueData: "{app}\WinHasher.exe -whirlpool ""%1"""; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
//
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherTIGER"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherTIGER"; ValueType: string; ValueName: "MUIVerb"; ValueData: "TIGER"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherTIGER\command"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherTIGER\command"; ValueType: string; ValueData: "{app}\WinHasher.exe -tiger ""%1"""; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
//
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherGOST"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherGOST"; ValueType: string; ValueName: "MUIVerb"; ValueData: "GOST3411"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherGOST\command"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherGOST\command"; ValueType: string; ValueData: "{app}\WinHasher.exe -gost3411 ""%1"""; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
//
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherGOST"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherGOST"; ValueType: string; ValueName: "MUIVerb"; ValueData: "GOST3411"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherGOST\command"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherGOST\command"; ValueType: string; ValueData: "{app}\WinHasher.exe -gost3411 ""%1"""; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
//
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherSHA3-224"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherSHA3-224"; ValueType: string; ValueName: "MUIVerb"; ValueData: "SHA3-224"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherSHA3-224\command"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherSHA3-224\command"; ValueType: string; ValueData: "{app}\WinHasher.exe -sha3-224 ""%1"""; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
//
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherSHA3-256"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherSHA3-256"; ValueType: string; ValueName: "MUIVerb"; ValueData: "SHA3-256"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherSHA3-256\command"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherSHA3-256\command"; ValueType: string; ValueData: "{app}\WinHasher.exe -sha3-256 ""%1"""; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
//
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherSHA3-384"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherSHA3-384"; ValueType: string; ValueName: "MUIVerb"; ValueData: "SHA3-384"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherSHA3-384\command"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherSHA3-384\command"; ValueType: string; ValueData: "{app}\WinHasher.exe -sha3-384 ""%1"""; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
//
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherSHA3-512"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherSHA3-512"; ValueType: string; ValueName: "MUIVerb"; ValueData: "SHA3-512"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherSHA3-512\command"; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WinHasherSHA3-512\command"; ValueType: string; ValueData: "{app}\WinHasher.exe -sha3-512 ""%1"""; Flags: createvalueifdoesntexist uninsdeletekey; Components: Windows_application\ContextMenu

[Code]
// To prevent double path add
function NeedsAddPath(IsUser: boolean; PathToCheck: string): boolean;
var
  OrigPath: string;
begin
  // ADMIN && key read KO
  if not IsUser and not RegQueryStringValue(HKEY_LOCAL_MACHINE, 'SYSTEM\CurrentControlSet\Control\Session Manager\Environment', 'Path', OrigPath) then
  begin
    Result := True;
    exit;
  end;

  // USER && key read KO
  if IsUser and not RegQueryStringValue(HKEY_CURRENT_USER, 'Environment', 'Path', OrigPath) then
  begin
    Result := True;
    exit;
  end;

  { look for the path with leading and trailing semicolon }
  { Pos() returns 0 if not found }
  Result := Pos(';' + Uppercase(PathToCheck) + ';', ';' + Uppercase(OrigPath) + ';') = 0;
end;

// https://stackoverflow.com/questions/72479992/check-if-net-5-0-is-installed-in-inno-setup/72526428#72526428
function IsDotNetInstalled(DotNetName: string): Boolean;
var
  Cmd, Args: string;
  FileName: string;
  Output: AnsiString;
  Command: string;
  ResultCode: Integer;
begin
  FileName := ExpandConstant('{tmp}\dotnet.txt');
  Cmd := ExpandConstant('{cmd}');
  Command := 'dotnet --list-runtimes';
  Args := '/C ' + Command + ' > "' + FileName + '" 2>&1';
  if Exec(Cmd, Args, '', SW_HIDE, ewWaitUntilTerminated, ResultCode) and (ResultCode = 0) then
  begin
    if LoadStringFromFile(FileName, Output) then
    begin
      if Pos(DotNetName, Output) > 0 then
      begin
        Log('"' + DotNetName + '" found in output of "' + Command + '"');
        Result := True;
      end
        else
      begin
        Log('"' + DotNetName + '" not found in output of "' + Command + '"');
        Result := False;
      end;
    end
      else
    begin
      Log('Failed to read output of "' + Command + '"');
    end;
  end
    else
  begin
    Log('Failed to execute "' + Command + '"');
    Result := False;
  end;
  DeleteFile(FileName);
end;

// InitializeSetup:  Check to see if .NET 6 is installed to warn the user.
function InitializeSetup(): Boolean;
var
   ErrorCode:    Integer;
begin
   if IsDotNetInstalled('Microsoft.NETCore.App 6.') then
   begin
      Result := True;
    end
      else
    begin
      if MsgBox('The .NET 6 runtime needs to be installed to run WinHasher, which appears not to be installed on the system. Do you want to open the browser to the download page of the runtime?', mbInformation, MB_YESNO) = IDYES then
      begin
         // Open the download URL in the default browser:
         ShellExec('open', 'https://dotnet.microsoft.com/en-us/download/dotnet/6.0', '', '', SW_SHOW, ewNoWait, ErrorCode);
      end;

      Result := MsgBox('Do you want to install WinHasher anyway (the runtime can be installed later on)?', mbInformation, MB_YESNO) = IDYES;
    end;
end;

// https://stackoverflow.com/questions/3304463/how-do-i-modify-the-path-environment-variable-when-running-an-inno-setup-install/46609047#46609047
procedure EnvRemovePath(IsUser: boolean; PathToRemove: string);
var
    OrigPath: string;
    P: Integer;
begin
    { Skip if registry entry not exists }
    // ADMIN and key read KO
    if not IsUser and not RegQueryStringValue(HKEY_LOCAL_MACHINE, 'SYSTEM\CurrentControlSet\Control\Session Manager\Environment', 'Path', OrigPath) then exit;
    // USER && key read KO
    if IsUser and not RegQueryStringValue(HKEY_CURRENT_USER, 'Environment', 'Path', OrigPath) then exit;

    { Skip if string not found in path }
    P := Pos(';' + Uppercase(PathToRemove) + ';', ';' + Uppercase(OrigPath) + ';');
    if P = 0 then exit;

    { Update path variable }
    Delete(OrigPath, P - 1, Length(PathToRemove) + 1);

    { Overwrite path environment variable }
    // ADMIN
    if not IsUser and not RegWriteStringValue(HKEY_LOCAL_MACHINE, 'SYSTEM\CurrentControlSet\Control\Session Manager\Environment', 'Path', OrigPath)
    then Log(Format('Error while removing the [%s] from PATH: [%s]', [PathToRemove, OrigPath]));
    // USER
    if IsUser and not RegWriteStringValue(HKEY_CURRENT_USER, 'Environment', 'Path', OrigPath)
    then Log(Format('Error while removing the [%s] from PATH: [%s]', [PathToRemove, OrigPath]));
end;

// DeinitializeUninstall:  Offer to remove our registry keys upon uninstall if the user
// doesn't want to keep them.
procedure DeinitializeUninstall();
var
	WinHasherRegKey: String;
	GPFComicsRegKey: String;
begin
  // First, remove the PATH (if any)
  EnvRemovePath(IsAdmin(), '{app}'); 

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
