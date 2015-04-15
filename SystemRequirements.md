# System Requirements #

  * [Microsoft .NET 2.0 Framework](http://msdn2.microsoft.com/en-us/netframework/aa731542.aspx); only the runtime or redistributable package is required. .NET 2.0 requires:
    * Windows XP SP3, Vista, 7, 8, Server 2003 and above
    * Windows Installer 3.0 (3.1 recommended)
    * Internet Explorer 5.01 or higher
    * 280MB disc space for the x86 version, 610MB for the x64 version
  * About 1MB of free disk space
  * No hard memory or processor limits are know beyond the OS and .NET's requirements, but obviously the more the merrier

Note: WinHasher may work on non-Windows platforms with a .NET 2.0 compatible framework like [Mono](http://www.mono-project.com/). However, this is untested beyond running it through [MoMA](http://www.mono-project.com/Moma) (which it passed). Use on a non-Windows platform is considered unsupported; there are far better hashing options available outside of Windows anyway, such as [OpenSSL](http://www.openssl.org/) or [Jacksum](http://www.jonelo.de/java/jacksum/index.html).

Note #2: Although .NET 2.0 technically works with some versions of Windows prior to XP SP3, Microsoft has dropped support for these older versions, as has InnoSetup, our installation file builder. If you need to use WinHasher on an older version of Windows, please try the raw binary archive release instead of the installer.