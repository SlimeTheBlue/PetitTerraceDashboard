# 검증 결과 — 2026-10-04

## 이번 개발본 확인
- .NET 10.0.401 개발 빌드: 오류 0 / 경고 0.
- 실제 WPF 창 생성 및 PNG 렌더: 호실관리/총회·의결관리/준비중 3개 카드, 모두 실행 비활성, 미키 회사명·입찰관리 텍스트 없음. 이미지 시각 확인 완료.
- 4개 합성 카드로 다음/이전 페이지(3개/1개) 확인.
- 실제 프로그램 관리창에서 순서 변경 후 이름·설명·숨김 저장/재읽기 확인.
- JSON 등록 저장/재읽기, 숨김 시 연결정보 보존, 손상 JSON 저장 차단 및 원본 유지 확인.
- 합성 WPF 업무창: 정상0.1초, 지연11초, 실패31초; 대기 안내, 30초 실패, 실패 뒤 늦은 창 연결 차단, 정상 숨김/종료 복귀 및 단일 실행 차단 통과.
- 화면 표시 전 조기 종료 및 부모 실행기 종료 뒤 자식 창 유지/종료 복귀 통과.
- 반복 화면 검증은 별도 임시 설정 디렉터리를 사용. 초기에 검증 디렉터리를 재사용해서 발생한 검증 데이터 오염은 GUID 경로로 수정 후 재검증 통과. 제품 변경 아님.

## 제한
- 실제 Python 호실관리 EXE 위치 및 실행 연동 미확인. 초기 연결 경로 없음, 화면에는 미설치 표시.
- 총회·의결관리 프로그램은 아직 없음. 준비중 카드만 제공.
- 제품별 설치 자동 연결 규칙/Setup 통보 실기는 미구현·미검증. 현재 resolver는 기존 사용자 연결을 그대로 두며 자동 경로를 추측하지 않음.
- 이번 개발본 Windows10, 배율100/125/150 각각 및 다중 모니터 재검증 미실시. 미키에서 기존 확인한 동작을 그대로 재사용했으며 새 제품 통과로 간주하지 않음.
- 이번에는 개발 DLL만 빌드. 배포 EXE/Setup/Git Commit/Release 없음.

## 검증 재실행
.NET10 SDK로 verification/Runner/Verification.csproj을 -p:UseAppHost=true 옵션으로 빌드하고 verification/Fixture/Fixture.csproj도 빌드한다. DOTNET_ROOT를 해당 SDK 폴더로 설정한다.
PetitDashboard.Verification.exe의 인자는 [SDK dotnet.exe 경로] [Fixture.dll 경로] [임시 결과 폴더]이며 마지막에 visual / extra / management를 지정할 수 있다. 기본 실행은 정상·11초·31초 사례를 확인한다. 실제 사용자 설정은 저장하지 않는다.

## 호실관리 실제 연동 — 2026-10-04
- GitHub CommercialUnitManager 최신 Release v1.1, CommercialUnitManagerV1.1_Setup.exe 다운로드. 46,383,294 bytes, SHA256 CCBD284273ADFC48F0289C6E10ED41B114F115403338850E6AA8B3195DE1B900: GitHub digest 일치.
- 기존 사용자 설치 위치 LocalAppData\Programs\상가 호실관리에 적용. 설치 ExitCode0. 설치 중 사용자 연결 설정 해시 동일.
- Services/InstalledProgramResolver.cs: v1.1 전용 uninstall AppId의 InstallLocation 또는 확인한 사용자 기본 설치 위치의 EXE 연결. 다른 프로그램/미키 경로 탐색 없음. 연결 자동 확인은 메모리에서만 처리, 사용자 지정 경로/이름/설명/아이콘/순서/숨김 유지.
- 설치 자동 연결의 설정 보존/사용자 지정 경로 유지/미설치 시 연결 안 함 검증 통과. 빌드 오류0/경고0.
- 실제 MainWindow의 호실관리 Launch 경로로 공식 설치 EXE 실행. 실제 V1.1 호실관리 창을 확인하고 대시보드 숨김 확인. WM_CLOSE 정상 닫기 요청 후 프로세스 종료와 대시보드 복귀 확인. 강제 종료 및 업무 데이터 편집 없음.
- 실제 실행 전후 기존 연결 JSON과 참조된 Excel SHA256 동일.
- 디자인 및 UI 코드 변경 없음. 호실관리 기능 전체/다른 OS·배율/다중 모니터는 이번 연동 검증 범위 밖.
- 이전 '호실관리 미확인/설치 연결 미구현' 기록은 이 범위에서 위 최신 결과로 대체한다. 총회·의결관리는 계속 준비중.

## 전용 V1.0 연결 수정 — 2026-10-04 (현재 적용)
- 사용자가 지정한 petitbuildingmanager 전용 V1.0 수정본으로 연결 대상 변경. 범용 V1.1 설치 경로/레지스트리 자동 탐색 제거.
- 기본 개발 실행본: D:\Dev\00_슬라임공장\쁘띠테라스_호실관리\dist\쁘띠테라스_호실관리.exe. 사용자 지정 연결은 유지.
- 격리된 LOCALAPPDATA와 합성 XLSX로 실제 새 EXE 실행. 창 제목 쁘띠테라스상가 호실관리 V1.0 확인. 대시보드 숨김 → 정상 WM_CLOSE → 프로세스 종료 → 대시보드 복귀 통과.
- 빌드 오류0/경고0. 등록정보 보존/사용자 지정 경로 보존/손상 JSON 보호 확인.
- 앞의 범용 V1.1 연결은 조사 이력이며 현재 연결 대상이 아님. 운영 XLSM 수정/변환 없음. 총회·의결관리 준비중.

## Setup 검증 완료 — 2026-10-04
- 전용 AppId {E7B03965-B03A-4F38-AD14-4F7F5D916C9E}. 기본 설치경로 LOCALAPPDATA\Programs\SlimeTheBlue\쁘띠테라스 호실관리. 범용 V1.1과 분리.
- Setup: D:/Dev/02_Releases/쁘띠테라스_호실관리/쁘띠테라스_호실관리_Setup.exe. SHA256 1AFCCF2BCC9840C8739916A8426D2B5F3AB50B8971992B0FF50FF492E58C189D. 업무 XLSX 포함 안 함.
- 설치0/제거0. 실제 첫 실행 XLSX 선택/기억 및 재실행 자동 복원/대시보드 숨김·복귀 통과.
- 제거 후 설치 파일·전용 레지스트리·바탕화면/시작메뉴 바로가기 제거. 사용자 파일 및 기억 설정 보존. 실제 운영자료·범용 V1.1 전후 보존.
- 테스트 설치 제거 완료. Git Commit/Push/Tag/Release 없음.

## 독립 EXE/Setup 완료
쁘띠테라스 업무 대시보드 EXE/Setup 검증 완료 — 2026-10-04
보관: D:\Dev\02_Releases\쁘띠테라스_대시보드
파일: 쁘띠테라스_대시보드_Setup.exe / 쁘띠테라스_대시보드.exe
Setup: 46,342,379 bytes, SHA256 9A97D920242F07FDC7D4A7EA04778CBDA2DA5AE11785272716CA72966F34B900
EXE: 141,448,274 bytes, SHA256 C71F35520D2616974E81A3646A129AA70E5AD22A92416FADC82577559D484BA8
구성: .NET10 Windows x64 self-contained single-file, WPF trimming 없음. UI 변경 없음.
전용 AppId {A18C4A29-644C-4DA3-A41B-480F61A94E5C}
기본 설치경로 LOCALAPPDATA\Programs\SlimeTheBlue\쁘띠테라스 업무 대시보드
호실관리 및 사용자 데이터는 설치본에 포함하지 않음. 호실관리 Setup은 별도 설치.
Release 연결은 전용 호실관리 AppId의 InstallLocation 사용. 개발 폴더 fallback은 Debug 빌드에서만 사용.

검증:
- 독립 EXE 및 Setup 생성. 설치 ExitCode0, EXE/시작 메뉴/선택 바탕화면 바로가기 생성.
- 실제 설치 EXE를 직접 실행하고 DOTNET_ROOT를 없는 경로로 지정해 외부 SDK 의존 없는 실행 확인.
- 호실관리 미설치 때 실행 비활성/미설치 표시 확인.
- 전용 호실관리 설치 후 실제 UI Automation 카드 클릭→설치된 호실관리 실행→대시보드 숨김→정상 종료→복귀 확인.
- 대시보드 정상 종료, 두 테스트 설치 제거 ExitCode0. 설치 폴더/전용 레지스트리/새 바로가기 제거 확인.
- 실제 XLSX/사용자 연결 설정/범용 V1.1 전후 해시 보존.
- 테스트 설치는 제거되어 있음. 실제 사용하려면 호실관리와 대시보드 각각 Setup 설치.
- 사용자 설정은 제거 대상으로 지정하지 않았음. .NET 단일파일 런타임의 표준 임시 추출 캐시는 설치 폴더 잔여물 검사 범위에 포함하지 않음.
- Git Commit/Push/Tag/GitHub Release 작업 없음. 로컬 Releases 폴더에 보관 완료.

수정/추가 파일:
Services/InstalledProgramResolver.cs (Release의 개발 경로 의존 제거)
Publish-Exe.ps1, 쁘띠테라스_대시보드.iss
verification/InstalledSetup 및 운영 문서
