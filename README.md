# Project Status #

| **Current Release** | 1.7 |
|:--------------------|:---------|
| **Date of Release** | January X, 2023 |
| **Realease note** | Upgraded to .NET 6, rewrites, added algorithms |

# About WinHasher #

WinHasher is a free, Open Source cryptographic hash or digest generator written in C# using Microsoft's .NET 2.0 Framework. It can be used to verify file download integrity, compare two or more files for modifications.

[Cryptographic hashing](http://en.wikipedia.org/wiki/Cryptographic_hash_function) is readily available on many computer operating systems. It often comes built-in to the OS or as a (relatively) standard optional package. Mac OS, Linux, Free/OpenBSD, and many other OSes include [OpenSSL](http://www.openssl.org/) as either a pre-installed or easily installable optional component. OpenSSL includes several command-line components for generating cryptographic hashes and there are number of graphical user interface (GUI) applications that allow point-and-click access to its capabilities.

Not so with Microsoft Windows. Windows does not include any built-in utilities for cryptographic hashes, and installing and using OpenSSL on Windows is not a trivial matter. The typical Windows user of today is much less familiar with the Windows Console (i.e. command line) let alone compiling software from source. And while cryptographic hashes are pretty much standard in programming libraries such as Microsoft .NET, the user is required to write and compile their own applications to use them.

This "hashing divide" has annoyed me for some time. While I consider myself to be an operating system agnostic and find myself equally home on both Windows and Linux, there are many times I've downloaded Windows-only software but didn't have the capability to verify the file's hash. Either I've been unable to install and run OpenSSL on a given machine, or I haven't had the time or access to a Linux box to copy the file over, generate the hash, and verify it before install. So I wanted to create a quick, simple, easy-to-use Windows app so I could get the hash of a file without waiting or moving it around. I also thought it would be a nice idea to be able to quickly compare the hashes of multiple files without having to generate each one and manually check every hexadecimal digit, so I added that functionality too. After writing the program, I thought it might be useful to others, so I decided to share.

# Legacy OSes #
Please note that from the 1.7 release and the usage of .NET 6, legacy OSes are being dropped. The only remaining OSes are the [one that can run .NET 6] (https://github.com/dotnet/core/blob/main/release-notes/6.0/supported-os.md) ([for now](https://github.com/dotnet/core/blob/6de37acb50a0d37c4a58dd0ba44dd121cdc8def4/release-notes/6.0/supported-os.md), Windows 7 SP1 and up).

For our full documentation, please see the [project wiki](https://github.com/gpfjeff/winhasher/tree/wiki).
