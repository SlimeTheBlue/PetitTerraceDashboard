# 쁘띠테라스 업무 대시보드 V1.0

독립 업무 프로그램을 실행하는 Windows WPF 대시보드입니다.
호실관리 카드와 총회·의결관리/준비중 카드, 이름·설명·순서·숨김 관리 기능을 제공합니다.
미키 대시보드 V1.0(e3e52f125029e9131b755fa2f5ef42301d161e01)을 기준으로 제작했습니다.

## 사용과 설치
호실관리 Setup과 대시보드 Setup을 각각 설치합니다.
.NET 런타임 포함 Windows x64 EXE이므로 실행 PC에는 SDK가 필요하지 않습니다.
호실관리 설치본은 전용 AppId {E7B03965-B03A-4F38-AD14-4F7F5D916C9E}의 사용자 설치등록으로 연결합니다.
Release는 개발 폴더에 의존하지 않으며 호실관리 미설치 때 실행을 비활성화합니다.
업무 XLSX와 호실관리 실행파일은 대시보드에 포함하지 않습니다.
총회·의결관리 프로그램은 아직 준비중입니다.

## 데이터와 식별정보
설정은 LOCALAPPDATA\SlimeTheBlue\PetitDashboard\programs.json에 저장합니다.
손상된 설정은 덮어쓰지 않으며 제거 시 사용자 설정을 삭제하지 않습니다.
대시보드 AppId: {A18C4A29-644C-4DA3-A41B-480F61A94E5C}
설치경로: LOCALAPPDATA\Programs\SlimeTheBlue\쁘띠테라스 업무 대시보드
작업표시줄/바로가기 AppUserModelID: SlimeTheBlue.PetitDashboard.1
아이콘: assets/petit_dash_compact_v2.ico. 확정한 투명 아이콘의 비율을 유지하고 표시 여백만 축소했습니다.

## 개발과 빌드
.NET10 Windows SDK로 dev-run.ps1 -DotnetPath 'SDK dotnet.exe 경로'를 실행합니다.
Publish-Exe.ps1 -DotnetPath 'SDK dotnet.exe 경로'는 런타임 포함 단일 EXE를 artifacts/publish에 생성합니다.
Inno Setup6의 ISCC.exe에 쁘띠테라스_대시보드.iss를 전달하여 Setup을 만듭니다.
로컬 배포본은 D:\Dev\02_Releases\쁘띠테라스_대시보드에 보관합니다.
Debug만 개발 호실관리 경로 fallback을 사용합니다. UI는 승인된 1844x1110 기본 창을 유지합니다.

## 검증
verification/Runner: 설정 보존, 실행 상태, 실제 호실관리 연동 및 화면 검증.
verification/InstalledSetup: 설치된 독립 EXE의 미설치 상태 및 실제 카드 클릭/숨김/복귀 검증.
검증 자료는 합성 XLSX와 격리된 사용자 설정을 사용합니다. 세부 결과는 VERIFICATION.md 참조.
GitHub에는 소스·선택된 리소스·빌드 정의를 올리고 업무 데이터·바이너리·개인 바로가기는 제외합니다.
