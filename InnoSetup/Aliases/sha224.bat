@rem WinHasher Batch File for Running SHA-224 Hashes
@echo off
if "%OS%" == "Windows_NT" goto WinNT
hash -sha224 %0 %1 %2 %3 %4 %5 %6 %7 %8 %9
goto endofbat
:WinNT
hash -sha224 %*
:endofbat
