using System.Windows;
using System.Windows.Threading;
internal static class Program {
 [STAThread] static void Main(string[] args){
  var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};var delay=double.Parse(args[0]);var lifetime=double.Parse(args[1]);
  if(args.Length>2&&args[2]=="child"){using var child=System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(System.Diagnostics.Process.GetCurrentProcess().MainModule!.FileName!){UseShellExecute=false,Arguments=$"\"{typeof(Program).Assembly.Location}\" 2 5"});lifetime=.8;delay=8;}
  var window=new Window{Title="Dashboard verification fixture",Width=500,Height=300,Content=new System.Windows.Controls.TextBlock{Text="대시보드 실행 검증용 창",Margin=new Thickness(25)}};
  var show=new DispatcherTimer{Interval=TimeSpan.FromSeconds(Math.Max(.01,delay))};show.Tick+=(_,_)=>{show.Stop();window.Show();};show.Start();
  var stop=new DispatcherTimer{Interval=TimeSpan.FromSeconds(lifetime)};stop.Tick+=(_,_)=>{stop.Stop();app.Shutdown();};stop.Start();app.Run();
 }
}
