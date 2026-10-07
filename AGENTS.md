# 프로젝트 작업 정보
작업 전 D:\Dev\AGENTS.md를 읽고 적용한다.

- 프로젝트명: 쁘띠테라스 업무 대시보드
- 프로젝트 루트: D:\Dev\00_슬라임공장\쁘띠테라스 업무 대시보드
- 기술: C# + WPF / .NET 10 / Windows x64
- 기준 소스: MikiEngineering-BusinessDashboard V1.0, e3e52f125029e9131b755fa2f5ef42301d161e01
- 실행/빌드: dev-run.ps1 -DotnetPath '실제 .NET 10 SDK dotnet.exe 경로'. 현재 확인한 경로는 README.md 참조. -SkipBuild로 기존 DLL 실행.
- 검증: VERIFICATION.md 및 verification/Runner. 합성 업무창으로 실행 동작 확인. 전용 Python 호실관리 V1.0 수정본 실행·숨김·종료 복귀 확인.
- 업무 고유 영역: 독립 업무 프로그램의 메인 허브. 기본 카드 호실관리/총회·의결관리/준비중. 의결 업무 규칙은 개별 프로그램에서 관리.
- 디자인 기준: 루트의 배경/로고/프로토타입 PNG. assets에는 실행에 필요한 배경/로고만 포함.
- 확정 범위: 미키 V1.0 동작 유지, 제품명·이미지·기본 카드·제품 식별정보·연결 대상 변경. 신규 등록/경로 편집/숨김 복원 UI 추가 안 함.
- 미키 원본, 기존 호실관리 원본·DB, 사용자 자료 수정 금지.
- 호실관리: 전용 Setup AppId {E7B03965-B03A-4F38-AD14-4F7F5D916C9E}의 HKCU InstallLocation 우선 연결. 미설치 시 확인된 전용 V1.0 개발 EXE D:\Dev\00_슬라임공장\쁘띠테라스_호실관리\dist\쁘띠테라스_호실관리.exe 연결. 범용 V1.1 자동 탐색 제거. 사용자 지정 연결 보존. 총회·의결관리 미구현. 향후 설치 계약은 별도 확인.
- 설정: LocalAppData\SlimeTheBlue\PetitDashboard\programs.json. 미키 설정과 격리. 손상파일 덮어쓰기 금지.
- 이번 승인 범위: 개발 구현 및 실행 검증. Setup/배포 EXE/Git Commit/Release는 별도 요청 시 진행.

## DLib 연결
- 정책: D:\Dev\01_DLib\DEVELOPMENT.md
- 카탈로그: D:\Dev\01_DLib\CATALOG.md
- 정식 소스: D:\Dev\01_DLib
- 연결: 없음. 현재 Python 라이브러리이며 정식 소스에서 C# / XAML 부품은 발견하지 못함. WPF 기반 미키 소스도 DLib 미사용.
- 소스 버전/커밋: 미확인. 이번에는 정책·카탈로그 및 C# 부품 유무 조사만 수행.
- 신규 공용 기능 작성 전 정책/관련 소스 확인. 업무 요구 때문에 Python 런타임 의존성을 임의 도입하지 않음.



## 독립 배포본
- 게시: Publish-Exe.ps1 -DotnetPath '실제 .NET10 SDK 경로'. self-contained win-x64 single-file, trimming 금지.
- 설치: 쁘띠테라스_대시보드.iss, AppId {A18C4A29-644C-4DA3-A41B-480F61A94E5C}. 로컬 보관 02_Releases\쁘띠테라스_대시보드.
- Release 빌드는 전용 호실관리 설치등록만 기본 연결하며 Debug에서만 개발 EXE fallback 허용.
- 검증: verification/InstalledSetup. 실제 설치 EXE 카드 클릭/설치된 호실관리/숨김·복귀 및 제거 검증.
- 이번 요청에서 로컬 EXE/Setup 제작·검증은 승인됨. GitHub Commit/Push/Tag/Release는 별도 승인 대상.

## 2026-10-08 두 프로그램 카드 연결
사용자 최신 요청: 우선 기존 설치대시보드 카드연결/실행검증만, 빌드SetupGit 별도. LocalAppData\SlimeTheBlue\PetitDashboard\programs.json의 rooms/assembly 이름·설명·EXE·작업폴더·IconPath·준비중상태만 변경. 기존순서/숨김상태/다른카드 보존. rooms는 확인된 설치EXE C:\Users\iljung\AppData\Local\Programs\SlimeTheBlue\쁘띠테라스 호실관리\쁘띠테라스_호실관리.exe. assembly는 확인된 최종빌드EXE D:\Dev\00_슬라임공장\쁘띠테라스_총회관리\dist\PetitMeeting\PetitMeeting.exe(현재 총회관리 설치본은없음). 각각 최종ICO를 설정폴더icons에 취득, 최대프레임PNG로변환해카드선명도확보. 실제설치대시보드 카드클릭으로각EXE실행/대시보드숨김/AltF4정상종료/복원 통과. 기존 LaunchSession 중복방지·숨김복원 소스수정없음. 직접사용자가 검증중2026-10-05_총회/참여의결.json수정했다고확인, 그대로보존. 그외운영자료·마스터불변. 각업무프로그램소스 변경없음. outputs 쁘띠대시보드_두프로그램_연결.png 및 연결검증.json. 다음단계 총회관리 실제설치경로전환 및 대시보드 빌드SetupGit은 별도요청.

## 2026-10-08 X 종료 후 전면 복원 수정
사용자 요청: 소스수정/개발실행 검증만, Setup/Git 보류. Platform/Windows.cs Activate에서 ShowInTaskbar=true/Show/최소화면Normal/주모니터작업영역중앙/Activate·Focus·SetForegroundWindow 순서. 전면활성화 실패때만 Topmost pulse(원래상태finally복구), 추가fallback AttachThreadInput(반드시finally해제). FlashWindow로작업표시줄만알림하던처리제거. LaunchSession 중복방지/실행중숨김/종료감지 변경없음. .NET10기존SDK Debug빌드 경고0오류0. 실제개발대시보드→호실관리설치EXE→숨김→실제제목줄닫기X→자동복원/총회관리최종EXE도동일검증통과. 관찰자는 입력없이 실제hwnd foreground/visible/IsIconic/primaryrect/APPWINDOW/TOPMOST측정, 두프로그램 주모니터중앙(358,129,2202,1239),최소화아님,taskbar=true,topmost=false. 표시후foreground시간 각각0.034초/0.023초. 별도숨김최소화창도Show/Normal/taskbar/foreground/Topmostfalse통과. outputs 호실관리_X종료_복원검증.json 및 총회관리_X종료_복원검증.json. 변경소스Windows.cs와AGENTS기록만, 설치대시보드EXE/Setup/Git/업무프로그램소스수정없음. 현재설치본에는아직미반영, 개발대시보드실행중.

## 2026-10-08 복원 수정 최종Setup 설치 검증 및 Git 반영
최신 사용자 새대화진행 요청으로 Release publish/기존InnoSetup 재생성/기존사용자설치갱신/납품본갱신/Commit-mainPush승인. 이전Setup·설치EXE 및 SHA work/dashboard_final_before보존. 게시EXE=설치EXE 전파일SHA동일. 최종설치대시보드에서호실관리설치본/총회관리최종빌드 각각 실제카드클릭/숨김/제목줄X종료/주모니터중앙전면복원통과. visible·foreground·taskbar=true,IsIconic·TOPMOST=false,primaryrect(358,129,2202,1239),표시후포커스 각각0.029초/0.024초. 운영자료·마스터·사용자카드설정 SHA변경없음. 납품 최종전달본/대시보드 Setup+사용안내2파일. 개인등록경로/운영자료는Setup과Git미포함. 총회관리카드현재개발최종EXE연결 유지(설치경로자동등록후속). Tag/Release/PR별도지시없어미실행. 최종SetupSHA·GitCommit은현재 outputs 대시보드_최종Setup_검증결과.json참조.
