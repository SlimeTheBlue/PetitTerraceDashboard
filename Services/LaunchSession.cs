using System.Diagnostics;
using System.IO;
using System.Windows.Threading;
using PetitDashboard.Models;
using Native=PetitDashboard.Platform.Windows;
namespace PetitDashboard.Services;
public sealed class LaunchSession : IDisposable
{
 readonly DispatcherTimer timer=new(){Interval=TimeSpan.FromMilliseconds(200)}; readonly Dictionary<int,long> known=new(); readonly Stopwatch elapsed=new(); bool ready,failed,waiting;
 public bool Active{get;private set;} public event Action? Waiting,Failed,Ended,ExitedWithoutWindow; public event Action<nint>? Ready;
 public LaunchSession(){timer.Tick+=Tick;}
 public void Start(ProgramEntry entry)=>Start(entry,null);
 internal void Start(ProgramEntry entry,string? arguments){
  if(Active)throw new InvalidOperationException("한 번에 한 프로그램만 실행할 수 있습니다.");
  if(entry.IsPlaceholder)throw new InvalidOperationException("준비 중인 프로그램입니다.");
  var exe=Path.GetFullPath(entry.Executable);if(!File.Exists(exe)||!string.Equals(Path.GetExtension(exe),".exe",StringComparison.OrdinalIgnoreCase))throw new InvalidOperationException("연결된 EXE를 찾을 수 없습니다. 설치 상태와 연결 정보를 확인해 주세요.");
  if(Native.AlreadyRunning(exe))throw new InvalidOperationException("이미 실행 중인 프로그램입니다. 기존 프로그램을 종료한 뒤 다시 실행해 주세요.");
  var cwd=string.IsNullOrWhiteSpace(entry.WorkingDirectory)?Path.GetDirectoryName(exe)!:entry.WorkingDirectory;if(!Directory.Exists(cwd))throw new InvalidOperationException("작업 폴더가 없습니다. 연결 정보를 확인해 주세요.");
  using var process=Process.Start(new ProcessStartInfo(exe){UseShellExecute=false,WorkingDirectory=cwd,Arguments=arguments??""})??throw new InvalidOperationException("프로그램을 시작하지 못했습니다.");
  known.Clear();try{known[process.Id]=process.StartTime.ToUniversalTime().Ticks;}catch(InvalidOperationException){throw new InvalidOperationException("프로그램이 시작 직후 종료되었습니다.");}
  ready=failed=waiting=false;Active=true;elapsed.Restart();timer.Start();
 }
 void Tick(object? sender,EventArgs e){
  var parents=Native.Parents();if(parents==null)return;
  var live=new HashSet<int>();foreach(var pair in known.ToArray())if(Alive(pair.Key,pair.Value))live.Add(pair.Key);
  // Retain the original parent identity while enumerating descendants of a launcher that has just exited.
  var ancestry=new HashSet<int>(known.Keys);bool changed;
  do {changed=false;foreach(var pair in parents)if(ancestry.Contains(pair.Value)&&!ancestry.Contains(pair.Key)&&TryIdentity(pair.Key,out var stamp)){known[pair.Key]=stamp;ancestry.Add(pair.Key);live.Add(pair.Key);changed=true;}}while(changed);
  foreach(var pid in known.Keys.ToArray())if(!live.Contains(pid))known.Remove(pid);
  if(live.Count==0){var noWindow=!ready&&!failed;Stop();Ended?.Invoke();if(noWindow)ExitedWithoutWindow?.Invoke();return;}
  if(!ready&&!failed){var hwnd=Native.FindWindow(live);if(hwnd!=0){ready=true;Ready?.Invoke(hwnd);return;}
   if(elapsed.Elapsed.TotalSeconds>=30){failed=true;Failed?.Invoke();}else if(!waiting&&elapsed.Elapsed.TotalSeconds>=10){waiting=true;Waiting?.Invoke();}}
 }
 static bool TryIdentity(int pid,out long stamp){stamp=0;try{using var p=Process.GetProcessById(pid);if(p.HasExited)return false;stamp=p.StartTime.ToUniversalTime().Ticks;return true;}catch(ArgumentException){return false;}catch(InvalidOperationException){return false;}catch(System.ComponentModel.Win32Exception){return false;}}
 static bool Alive(int pid,long stamp)=>TryIdentity(pid,out var current)&&stamp==current;
 void Stop(){timer.Stop();Active=false;known.Clear();elapsed.Stop();} public void Dispose()=>Stop();
}
