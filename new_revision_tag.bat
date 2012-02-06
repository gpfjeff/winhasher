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
# Random SVN Revision Tagger

my $workingpath = "C:\\Users\\Jeff\\Documents\\Visual Studio 2010\\Projects\\WinHasher\\";

my @templates = (
		"WinHasherCore\\Properties\\AssemblyInfo.cs.template",
		"WinHasher\\Properties\\AssemblyInfo.cs.template",
		"sha1\\Properties\\AssemblyInfo.cs.template",
		"md5\\Properties\\AssemblyInfo.cs.template",
		"hash\\Properties\\AssemblyInfo.cs.template",
		"InnoSetup\\WinHasher.iss.template"
	);

srand(time ^ ($$ + ($$ << 15)));
$tag = unpack('H24', rand());
$tempfile = $workingpath . 'temp' . $$ . '.tmp';

foreach $template (@templates) {
	$infile = $workingpath . $template;
	unlink($tempfile);
	open(IN, "<$infile") or die("Cannot open $infile [$1]");
	open(OUT, ">$tempfile") or die("Cannot open $tempfile [$1]");
	while ($line = <IN>) {
		chomp($line);
		$line =~ s/^\/\/ Revision Tag: \w+$/\/\/ Revision Tag: $tag/;
		print(OUT "$line\n");
	}
	close(IN);
	close(OUT);
	unlink($infile);
	rename($tempfile, $infile);
}
unlink($tempfile);

__END__
:endofperl
