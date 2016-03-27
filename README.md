# EVE Online Live packet dumper (EVEOLPD)

#### Requirements
* Windows (Wine version in dev)
* Visual Studio (I use 2013)
* Some knowledge

#### How to use  
1. We need to re-enable local dll loading in windows
  * `$ reg add "HKLM\SOFTWARE\Microsoft\WindowsNT\CurrentVersion\Image File Execution Options" /v DevOverrideEnable /t REG_DWORD /d 1`  
  * Reboot. No seriously.
2.  Download and build for release this repo
3. Create an empty file in the eve/bin folder called `exefile.exe.local`
4. Copy advapi32.dll to eve/bin as advapi32_.dll
  * x86 `C:\Windows\System32\advapi32.dll`
  * x64 `C:\Windows\SysWOW64\advapi32.dll`
5. Copy advapi32.dll from this repo's advapi32/Release folder to eve/bin
6. Build the control 'app' and direct it to the eve/bin folder
7. Run exefile.exe and once loaded fully enable both incomming and outgoing packet logging

Packets will be output to 
    C:\packets_<time_since_epoch>\<bluetime>-<enc/dec>-<count>.everaw
    
