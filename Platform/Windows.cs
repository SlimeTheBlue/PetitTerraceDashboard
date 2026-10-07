using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;
namespace PetitDashboard.Platform;
public static class Windows
{
 [DllImport("user32.dll",CharSet=CharSet.Unicode,SetLastError=true)] public static extern uint RegisterWindowMessage(string name);
 [StructLayout(LayoutKind.Sequential)] public struct Rect { public int Left,Top,Right,Bottom; }
 [StructLayout(LayoutKind.Sequential)] struct MonitorInfo { public int Size; public Rect Monitor,Work; public uint Flags; }
 [StructLayout(LayoutKind.Sequential,CharSet=CharSet.Unicode)] struct ProcessEntry { public uint Size,Usage,Pid; public UIntPtr Heap; public uint Module,Threads,Parent; public int Priority; public uint Flags; [MarshalAs(UnmanagedType.ByValTStr,SizeConst=260)] public string Exe; }
 delegate bool MonitorCallback(nint monitor,nint dc,nint rect,nint data);
 delegate bool WindowCallback(nint hwnd,nint data);
 [DllImport("user32.dll")] static extern bool EnumDisplayMonitors(nint dc,nint clip,MonitorCallback cb,nint data);
 [DllImport("user32.dll",CharSet=CharSet.Unicode)] static extern bool GetMonitorInfo(nint monitor,ref MonitorInfo info);
 [DllImport("shcore.dll")] static extern int GetDpiForMonitor(nint monitor,int type,out uint x,out uint y);
 [DllImport("user32.dll")] static extern bool EnumWindows(WindowCallback cb,nint data);
 [DllImport("user32.dll")] static extern bool IsWindowVisible(nint hwnd);
 [DllImport("user32.dll")] static extern nint GetWindow(nint hwnd,uint command);
 [DllImport("user32.dll")] static extern uint GetWindowThreadProcessId(nint hwnd,out uint pid);
 [DllImport("user32.dll",CharSet=CharSet.Unicode)] static extern int GetWindowText(nint hwnd,StringBuilder text,int count);
 [DllImport("user32.dll")] static extern bool SetWindowPos(nint hwnd,nint after,int x,int y,int width,int height,uint flags);
 [DllImport("user32.dll")] static extern bool ShowWindow(nint hwnd,int command);
 [DllImport("user32.dll")] static extern bool SetForegroundWindow(nint hwnd);
 [DllImport("user32.dll")] static extern nint GetForegroundWindow();
 [DllImport("kernel32.dll")] static extern uint GetCurrentThreadId();
 [DllImport("user32.dll")] static extern bool AttachThreadInput(uint from,uint to,bool attach);
 [DllImport("user32.dll",SetLastError=true)] static extern bool SetProcessDpiAwarenessContext(nint context);
 [DllImport("user32.dll",SetLastError=true)] static extern nint SetThreadDpiAwarenessContext(nint context);
 public static void EnablePerMonitorDpi()
 {
  SetProcessDpiAwarenessContext(-4);
  if(SetThreadDpiAwarenessContext(-4)==0) throw new InvalidOperationException("모니터별 화면 배율을 설정하지 못했습니다.");
 }
 [DllImport("kernel32.dll")] static extern nint CreateToolhelp32Snapshot(uint flags,uint pid);
 [DllImport("kernel32.dll",CharSet=CharSet.Unicode)] static extern bool Process32First(nint snapshot,ref ProcessEntry entry);
 [DllImport("kernel32.dll",CharSet=CharSet.Unicode)] static extern bool Process32Next(nint snapshot,ref ProcessEntry entry);
 [DllImport("kernel32.dll")] static extern bool CloseHandle(nint handle);
 public static (Rect Area,uint Dpi) Primary()
 {
  Rect area=default; uint dpi=96;
  EnumDisplayMonitors(0,0,(monitor,dc,rect,data)=>{var info=new MonitorInfo{Size=Marshal.SizeOf<MonitorInfo>()};if(GetMonitorInfo(monitor,ref info)&&(info.Flags&1)!=0){area=info.Work;try{if(GetDpiForMonitor(monitor,0,out var x,out _) ==0)dpi=x;}catch(DllNotFoundException){}return false;}return true;},0);
  if(area.Right<=area.Left) throw new InvalidOperationException("주 모니터 작업영역을 확인할 수 없습니다.");
  return(area,dpi);
 }
 public static void PlaceDashboard(Window window)
 {
  var (area,dpi)=Primary();var scale=dpi/96.0;var width=Math.Min(area.Right-area.Left,1844);var height=Math.Min(area.Bottom-area.Top,1110);
  window.Width=width/scale;window.Height=height/scale;
  SetWindowPos(new WindowInteropHelper(window).Handle,0,area.Left+(int)((area.Right-area.Left-width)/2),area.Top+(int)((area.Bottom-area.Top-height)/2),(int)width,(int)height,0x14);
 }
 public static void MaximizeOnPrimary(nint hwnd)
 { var(a,_)=Primary();ShowWindow(hwnd,9);SetWindowPos(hwnd,0,a.Left,a.Top,a.Right-a.Left,a.Bottom-a.Top,0x14);ShowWindow(hwnd,3); }
 public static void Activate(Window window)
 {
  window.ShowInTaskbar=true;
  window.Show();
  if(window.WindowState==WindowState.Minimized)window.WindowState=WindowState.Normal;
  PlaceDashboard(window);
  var hwnd=new WindowInteropHelper(window).Handle;
  window.Activate();window.Focus();SetForegroundWindow(hwnd);
  if(GetForegroundWindow()==hwnd)return;
  // Pulse only on failed activation; never leave the dashboard always on top.
  var wasTopmost=window.Topmost;
  try {window.Topmost=true;window.Activate();SetForegroundWindow(hwnd);}
  finally {window.Topmost=wasTopmost;}
  if(GetForegroundWindow()==hwnd)return;
  var foreground=GetForegroundWindow();var foregroundThread=GetWindowThreadProcessId(foreground,out _);var currentThread=GetCurrentThreadId();
  var attached=foregroundThread!=0&&foregroundThread!=currentThread&&AttachThreadInput(currentThread,foregroundThread,true);
  try {window.Activate();window.Focus();SetForegroundWindow(hwnd);}
  finally {if(attached)AttachThreadInput(currentThread,foregroundThread,false);}
 }
 public static Dictionary<int,int>? Parents()
 {
  var result=new Dictionary<int,int>();var snapshot=CreateToolhelp32Snapshot(2,0);if(snapshot==0||snapshot==-1)return null;
  try {var entry=new ProcessEntry{Size=(uint)Marshal.SizeOf<ProcessEntry>(),Exe=""};if(!Process32First(snapshot,ref entry))return null;do{result[(int)entry.Pid]=(int)entry.Parent;}while(Process32Next(snapshot,ref entry));} finally{CloseHandle(snapshot);}return result;
 }
 public static nint FindWindow(HashSet<int> pids)
 {
  nint found=0;EnumWindows((hwnd,data)=>{GetWindowThreadProcessId(hwnd,out var pid);if(pids.Contains((int)pid)&&IsWindowVisible(hwnd)&&GetWindow(hwnd,4)==0){var text=new StringBuilder(512);GetWindowText(hwnd,text,512);if(text.Length>0&&!text.ToString().Contains("실행 확인")){found=hwnd;return false;}}return true;},0);return found;
 }
 public static bool AlreadyRunning(string exe)
 {
  foreach(var p in Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(exe)))using(p){try {if(string.Equals(p.MainModule?.FileName,exe,StringComparison.OrdinalIgnoreCase))return true;}catch(System.ComponentModel.Win32Exception){throw new InvalidOperationException("같은 이름의 프로그램 실행 상태를 확인할 수 없습니다. 실행 상태를 확인한 뒤 다시 시도해 주세요.");}catch(InvalidOperationException){}}
  return false;
 }
}
