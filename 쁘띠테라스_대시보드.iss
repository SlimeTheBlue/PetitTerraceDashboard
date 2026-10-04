[Setup]
AppId={{A18C4A29-644C-4DA3-A41B-480F61A94E5C}
AppName=쁘띠테라스 업무 대시보드
AppVersion=1.0
AppPublisher=SlimeTheBlue
DefaultDirName={localappdata}\Programs\SlimeTheBlue\쁘띠테라스 업무 대시보드
DefaultGroupName=SlimeTheBlue\쁘띠테라스 업무 대시보드
OutputDir=..\..\02_Releases\쁘띠테라스_대시보드
OutputBaseFilename=쁘띠테라스_대시보드_Setup
SetupIconFile=assets\petit_dash_compact_v2.ico
UninstallDisplayIcon={app}\쁘띠테라스_대시보드.exe
PrivilegesRequired=lowest
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
AppMutex=Local\SlimeTheBlue.PetitDashboard
VersionInfoCompany=SlimeTheBlue
VersionInfoDescription=쁘띠테라스 업무 대시보드 설치 프로그램
VersionInfoProductName=쁘띠테라스 업무 대시보드
VersionInfoProductVersion=1.0.0
VersionInfoVersion=1.0.0
[Languages]
Name: "korean"; MessagesFile: "compiler:Languages\Korean.isl"
Name: "english"; MessagesFile: "compiler:Default.isl"
[Tasks]
Name: "desktopicon"; Description: "바탕화면 바로가기 만들기"; Flags: unchecked
[Files]
Source: "artifacts\publish\PetitDashboard.exe"; DestDir: "{app}"; DestName: "쁘띠테라스_대시보드.exe"; Flags: ignoreversion
[Icons]
Name: "{group}\쁘띠테라스 업무 대시보드"; Filename: "{app}\쁘띠테라스_대시보드.exe"; AppUserModelID: "SlimeTheBlue.PetitDashboard.1"
Name: "{autodesktop}\쁘띠테라스 업무 대시보드"; Filename: "{app}\쁘띠테라스_대시보드.exe"; AppUserModelID: "SlimeTheBlue.PetitDashboard.1"; Tasks: desktopicon
[Run]
Filename: "{app}\쁘띠테라스_대시보드.exe"; Description: "쁘띠테라스 업무 대시보드 실행"; Flags: nowait postinstall skipifsilent
