@rem = '--*-Perl-*--
@echo off
if "%OS%" == "Windows_NT" goto WinNT
perl -x -S "%0" %1 %2 %3 %4 %5 %6 %7 %8 %9
goto endofperl
:WinNT
perl -x -S %0 %*
if NOT "%COMSPEC%" == "%SystemRoot%\system32\cmd.exe" goto endofperl
if %errorlevel% == 9009 echo You do not have Perl in your PATH.
if errorlevel 1 goto script_failed_so_exit_with_non_zero_val 2>nul
goto endofperl
@rem ';
#!perl
#line 15
# SubWCRev SVN Hook Script

my $workingpath = "C:\\Users\\Jeff\\Documents\\Visual Studio 2010\\Projects\\WinHasher";

my @templates = (
		"WinHasherCore\\Properties\\AssemblyInfo.cs.template",
		"WinHasher\\Properties\\AssemblyInfo.cs.template",
		"sha1\\Properties\\AssemblyInfo.cs.template",
		"md5\\Properties\\AssemblyInfo.cs.template",
		"hash\\Properties\\AssemblyInfo.cs.template",
		"InnoSetup\\WinHasher.iss.template"
	);

foreach $template (@templates) {
	my $target = join('.', (split(/\./, $template))[0,1]);
	my $cmd = "SubWCRev \"$workingpath\" \"$template\" \"$target\"";
	print("Executing: $cmd\n");
	system($cmd);
}
__END__
:endofperl
