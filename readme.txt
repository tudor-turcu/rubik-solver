C# / .NET Core port of Herbert Kociemba (http://kociemba.org/cube.htm) original implementaton (in Java): http://kociemba.org/downloads/twophase.jar
-------------
Get the latest Cake build.ps1 by opening a powershell script, change the working directory to this one, and run:

Invoke-WebRequest http://cakebuild.net/download/bootstrapper/windows -OutFile build.ps1

Now open the build.cake file, and replace the placeholders with values for your Raspberry Pi's IP address, the destination directory, and the username you use to log into your Pi.

Prerequisites
- development workstation/laptop with Windows 10
- Visual Studio 2017
- git installed and configured: https://git-scm.com/book/en/v2/Getting-Started-Installing-Git
- a working and configured GitHub account: https://git-scm.com/book/en/v2/GitHub-Account-Setup-and-Configuration

Rapsberry PI 3
- keyboard
- mouse
- ethernet cable
- HDMI cable
- micro SD card >= 8 GB
- SD card reader (external or included on the laptop)
- external TV/monitor with HDMI input

Environment setup
- Install Rapsbian on the PI device as decribed at https://blogs.msdn.microsoft.com/david/2017/07/20/setting_up_raspian_and_dotnet_core_2_0_on_a_raspberry_pi/ or https://www.raspberrypi.org/documentation/installation/installing-images/README.md

- Install .NET Core on Rapsbian as described above or at https://jeremylindsayni.wordpress.com/2017/07/23/running-a-net-core-2-app-on-raspbian-jessie-and-deploying-to-the-pi-with-cake/

- Install .NET Core 2.0 SDK on Windows: https://www.microsoft.com/net/download/windows

