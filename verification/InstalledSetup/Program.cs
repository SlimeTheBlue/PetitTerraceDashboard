using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Automation;
class Check {
 [DllImport("user32.dll")] static extern bool IsWindowVisible(nint hwnd);
 [DllImport("user32.dll")] static extern bool PostMessage(nint hwnd,uint message,nint a,nint b);
 [STAThread] static int Main(string[] args){try {
  var info=new ProcessStartInfo(args[0]){UseShellExecute=false,WorkingDirectory=Path.GetDirectoryName(args[0])!};
  info.Environment["DOTNET_ROOT"]=Path.Combine(args[1],"absent-dotnet");info.Environment["DOTNET_MULTILEVEL_LOOKUP"]="0";
  info.Environment["LOCALAPPDATA"]=args[1];
  using var app=Process.Start(info)!;
  for(int i=0;i<100;i++){Thread.Sleep(100);app.Refresh();if(app.MainWindowHandle!=0)break;if(app.HasExited)throw new Exception("Dashboard exited early");}
  var handle=app.MainWindowHandle;if(handle==0)throw new Exception("Dashboard window missing");
  var root=AutomationElement.FromHandle(handle);Thread.Sleep(1000);
  var buttons=root.FindAll(TreeScope.Descendants,new PropertyCondition(AutomationElement.ControlTypeProperty,ControlType.Button)).Cast<AutomationElement>().ToArray();
  if(args[2]=="missing") {
   if(!buttons.Any(x=>x.Current.Name=="미설치"&&!x.Current.IsEnabled))throw new Exception("Release used developer room EXE");
   Console.WriteLine("PASS installed standalone dashboard without SDK; missing room installation disabled");
  } else {
   var launch=buttons.Single(x=>x.Current.Name.StartsWith("시스템 실행")&&x.Current.IsEnabled);
   ((InvokePattern)launch.GetCurrentPattern(InvokePattern.Pattern)).Invoke();
   Process room=null;
   for(int i=0;i<150;i++) {Thread.Sleep(100); room=Process.GetProcessesByName("쁘띠테라스_호실관리").FirstOrDefault(x=>{x.Refresh();return x.MainWindowHandle!=0;});if(room!=null)break;}
   if(room==null)throw new Exception("Room window missing");
   Thread.Sleep(600); if(IsWindowVisible(handle))throw new Exception("Dashboard stayed visible");
   if(!room.MainWindowTitle.Contains("호실관리"))throw new Exception("Wrong room window");
   PostMessage(room.MainWindowHandle,0x10,0,0);
   for(int i=0;i<150&&!IsWindowVisible(handle);i++)Thread.Sleep(100);
   if(!IsWindowVisible(handle))throw new Exception("Dashboard did not return");
   Console.WriteLine("PASS installed standalone dashboard card click -> installed room -> hide -> normal close -> dashboard restore");
  }
  PostMessage(handle,0x10,0,0);if(!app.WaitForExit(5000))throw new Exception("Dashboard did not exit");
  return 0;
 }catch(Exception e){Console.WriteLine("FAIL "+e);return 1;}}
}
