using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using PetitDashboard;
using PetitDashboard.Models;
using PetitDashboard.Services;

internal static class RoomIntegrationVerification
{
 [DllImport("user32.dll",CharSet=CharSet.Unicode)] static extern int GetWindowText(nint hwnd,StringBuilder text,int count);
 [DllImport("user32.dll")] static extern bool PostMessage(nint hwnd,uint message,nint wParam,nint lParam);
 [StructLayout(LayoutKind.Sequential)] struct Rect { public int Left,Top,Right,Bottom; }
 [DllImport("user32.dll")] static extern bool GetWindowRect(nint hwnd,out Rect rect);
 [DllImport("user32.dll")] static extern bool IsZoomed(nint hwnd);
 public static void Run(string output,string? executable=null)
 {
  var window=new MainWindow(); window.Show();window.UpdateLayout();
  var registry=(ProgramRegistry)typeof(MainWindow).GetField("registry",BindingFlags.NonPublic|BindingFlags.Instance)!.GetValue(window)!;
  var entry=registry.Programs.Single(x=>x.Id=="rooms");
  if(executable!=null){entry.Executable=Path.GetFullPath(executable);entry.WorkingDirectory=Path.GetDirectoryName(entry.Executable)!;}
  if(!File.Exists(entry.Executable))throw new Exception("Installed room manager not found");
  var session=(LaunchSession)typeof(MainWindow).GetField("session",BindingFlags.NonPublic|BindingFlags.Instance)!.GetValue(window)!;
  var frame=new DispatcherFrame();bool hidden=false,restored=false;string title="";
  var close=new DispatcherTimer{Interval=TimeSpan.FromSeconds(2)};
  nint roomWindow=0;
  close.Tick+=(_,_)=>{close.Stop();if(IsZoomed(roomWindow))throw new Exception("Room window was maximized");GetWindowRect(roomWindow,out var roomRect);GetWindowRect(new System.Windows.Interop.WindowInteropHelper(window).Handle,out var dashboardRect);if(roomRect.Right-roomRect.Left!=dashboardRect.Right-dashboardRect.Left||roomRect.Bottom-roomRect.Top!=dashboardRect.Bottom-dashboardRect.Top)throw new Exception("Default window sizes differ");Console.WriteLine($"PASS normal equal window size {roomRect.Right-roomRect.Left}x{roomRect.Bottom-roomRect.Top}");if(!PostMessage(roomWindow,0x0010,0,0))throw new Exception("Normal close request failed");};
  session.Ready+=hwnd=>{
   roomWindow=hwnd;var text=new StringBuilder(512);GetWindowText(hwnd,text,text.Capacity);title=text.ToString();
   hidden=!window.IsVisible;
   if(!hidden)throw new Exception("Dashboard did not hide");
   close.Start();
  };
  session.Ended+=()=>{restored=window.IsVisible;frame.Continue=false;};
  session.Failed+=()=>frame.Continue=false;
  var timeout=new DispatcherTimer{Interval=TimeSpan.FromSeconds(45)};
  timeout.Tick+=(_,_)=>{timeout.Stop();frame.Continue=false;};timeout.Start();
  typeof(MainWindow).GetMethod("Launch",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(window,[entry]);
  Dispatcher.PushFrame(frame);timeout.Stop();close.Stop();
  if(!hidden||!restored||session.Active)throw new Exception($"Integration incomplete hidden={hidden} restored={restored} active={session.Active}");
  if(!title.Contains("호실"))throw new Exception("Unexpected actual window: "+title);
  window.UpdateLayout();
  var image=new RenderTargetBitmap((int)Math.Ceiling(window.ActualWidth),(int)Math.Ceiling(window.ActualHeight),96,96,PixelFormats.Pbgra32);image.Render(window);
  var encoder=new PngBitmapEncoder();encoder.Frames.Add(BitmapFrame.Create(image));using(var stream=File.Create(Path.Combine(output,"room-connected-preview.png")))encoder.Save(stream);
  window.Close();Console.WriteLine("PASS actual room EXE/card launch, actual room window, dashboard hide, normal WM_CLOSE, process exit and dashboard restore");
  Console.WriteLine("Actual window title: "+title);
 }
}
