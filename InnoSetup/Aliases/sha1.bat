@rem WinHasher Batch File for Running SHA-1 Hashes
@echo off
if "%OS%" == "Windows_NT" goto WinNT
hash -sha1 %0 %1 %2 %3 %4 %5 %6 %7 %8 %9
goto endofbat
:WinNT
hash -sha1 %*
:endofbat
