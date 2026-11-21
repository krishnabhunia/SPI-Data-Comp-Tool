; 64-bit Windows installer for the SPI Data Comparison Tool (Inno Setup 6).
; CI builds it from the Release|x64 output, e.g.:
;   ISCC /DAppVersion=2.5.0-beta001 /DNumericVersion=2.5.0 /DSourceDir=<bin\x64\Release> /DOutputDir=<dist\windows-x64> SPI_Data_Comp_Tool.iss
; The app is one .exe (its libraries are embedded by Costura) and its .exe.config.

#ifndef AppVersion
  #define AppVersion "0.0.0"
#endif
#ifndef NumericVersion
  #define NumericVersion "0.0.0"
#endif
#ifndef SourceDir
  #define SourceDir "..\..\SPI Data Comp Tool\bin\x64\Release"
#endif
#ifndef OutputDir
  #define OutputDir "..\..\dist\windows-x64"
#endif
#define AppName "SPI Data Comparison Tool"
#define ExeName "SPI Data Comp Tool.exe"

[Setup]
; Never change AppId: it is how a newer setup finds and upgrades the installed copy.
AppId={{DF6AFF7A-0A5A-4F1E-BF54-F5301F101D26}
AppName={#AppName}
AppVersion={#AppVersion}
AppVerName={#AppName} {#AppVersion}
AppPublisherURL=https://github.com/krishnabhunia/SPI-Data-Comp-Tool
VersionInfoVersion={#NumericVersion}
VersionInfoProductVersion={#NumericVersion}
VersionInfoDescription={#AppName} setup
; 64-bit only, installed under C:\Program Files (or %LocalAppData%\Programs when installed just for the current user)
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
PrivilegesRequired=admin
PrivilegesRequiredOverridesAllowed=dialog commandline
DefaultDirName={autopf}\{#AppName}
DefaultGroupName={#AppName}
DisableProgramGroupPage=yes
SetupIconFile=..\..\SPI Data Comp Tool\app.ico
UninstallDisplayIcon={app}\{#ExeName}
UninstallDisplayName={#AppName}
OutputDir={#OutputDir}
OutputBaseFilename=SPI_Data_Comp_Tool_{#AppVersion}
Compression=lzma2/max
SolidCompression=yes
WizardStyle=modern
CloseApplications=yes
RestartApplications=no

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "{#SourceDir}\{#ExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#SourceDir}\{#ExeName}.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#SourceDir}\THIRD_PARTY_NOTICES.md"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{autoprograms}\{#AppName}"; Filename: "{app}\{#ExeName}"
Name: "{autodesktop}\{#AppName}"; Filename: "{app}\{#ExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#ExeName}"; Description: "{cm:LaunchProgram,{#AppName}}"; Flags: nowait postinstall skipifsilent
; The app's own updater runs this installer silently with /RELAUNCH=1 and wants the app started again afterwards.
Filename: "{app}\{#ExeName}"; Flags: nowait skipifnotsilent runasoriginaluser; Check: RelaunchRequested

[Code]
function RelaunchRequested: Boolean;
begin
  Result := ExpandConstant('{param:RELAUNCH|0}') = '1';
end;

// .NET Framework 4.8 or later (minimum Release key 528040), which the app is built for.
function DotNet48Installed(): Boolean;
var
  Release: Cardinal;
begin
  Result := RegQueryDWordValue(HKLM, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full', 'Release', Release)
            and (Release >= 528040);
end;

function InitializeSetup(): Boolean;
begin
  Result := DotNet48Installed();
  if not Result then
    SuppressibleMsgBox('{#AppName} needs Microsoft .NET Framework 4.8 or later.' + #13#10 +
      'Install it from https://dotnet.microsoft.com/download/dotnet-framework and run this setup again.',
      mbCriticalError, MB_OK, IDOK);
end;
