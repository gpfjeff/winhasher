@rem WinHasher Batch File for Running RIPEMD-160 Hashes
@echo off
if "%OS%" == "Windows_NT" goto WinNT
hash -ripemd160 %0 %1 %2 %3 %4 %5 %6 %7 %8 %9
goto endofbat
:WinNT
hash -ripemd160 %*
:endofbat
