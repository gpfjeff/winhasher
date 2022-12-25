@rem WinHasher Batch File for Running MD5 Hashes
@echo off
if "%OS%" == "Windows_NT" goto WinNT
hash -md5 %0 %1 %2 %3 %4 %5 %6 %7 %8 %9
goto endofbat
:WinNT
hash -md5 %*
:endofbat
