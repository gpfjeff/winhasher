@rem WinHasher Batch File for Running Whirlpool Hashes
@echo off
if "%OS%" == "Windows_NT" goto WinNT
hash -whirlpool %0 %1 %2 %3 %4 %5 %6 %7 %8 %9
goto endofbat
:WinNT
hash -whirlpool %*
:endofbat
