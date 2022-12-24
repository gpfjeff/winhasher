@rem WinHasher Batch File for Running SHA3-512 Hashes
@echo off
if "%OS%" == "Windows_NT" goto WinNT
hash -sha3-512 %0 %1 %2 %3 %4 %5 %6 %7 %8 %9
goto endofbat
:WinNT
hash -sha3-512 %*
:endofbat
