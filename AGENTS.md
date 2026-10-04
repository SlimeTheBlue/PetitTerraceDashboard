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
