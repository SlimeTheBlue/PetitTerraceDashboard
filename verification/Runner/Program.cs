using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using PetitDashboard.Models;
using PetitDashboard.Services;
using Native=PetitDashboard.Platform.Windows;
class Program {
 [STAThread] static int Main(string[] args){try{
  RoomDiscoveryVerification.Run();

  var app=new PetitDashboard.App(); app.InitializeComponent(); app.ShutdownMode=ShutdownMode.OnExplicitShutdown;

  var directory=Path.Combine(args[2],"registry-verification",Guid.NewGuid().ToString("N"));Directory.CreateDirectory(directory);var store=new RegistryStore(directory);var data=store.Load();if(data.Programs.Count!=3)throw new Exception("Initial cards");
  data.Programs.Add(new ProgramEntry{Name="검증 카드",IsPlaceholder=true});store.Save(data);if(new RegistryStore(directory).Load().Programs.Count!=4)throw new Exception("Registry roundtrip");Console.WriteLine("PASS registry save/reload, four entries");
  var connected=data.Programs[0].Copy();data.Programs[0].IsHidden=true;store.Save(data);
  var reloaded=new RegistryStore(directory).Load();var hidden=reloaded.Programs.Single(x=>x.Id==connected.Id);
  if(!hidden.IsHidden||reloaded.Programs.Count!=4||hidden.Executable!=connected.Executable||hidden.WorkingDirectory!=connected.WorkingDirectory||hidden.IconPath!=connected.IconPath||hidden.IsPlaceholder!=connected.IsPlaceholder||reloaded.Programs.Count(x=>!x.IsHidden)!=3)throw new Exception("Hidden entry or connection lost");
  Console.WriteLine("PASS hidden setting persists without deleting entry or changing connection");
  File.WriteAllText(store.FilePath,"{broken");var bad=new RegistryStore(directory);bad.Load();if(bad.CanSave)throw new Exception("Corrupt registry overwrite");try{bad.Save(data);throw new Exception("Overwrite was allowed");}catch(InvalidOperationException){}if(File.ReadAllText(store.FilePath)!="{broken")throw new Exception("Registry changed");Console.WriteLine("PASS corrupt registry preserved");
  if(args.Length>3&&args[3]=="room"){RoomIntegrationVerification.Run(args[2],args.Length>4?args[4]:null);return 0;}
  if(args.Length>3&&args[3]=="visual"){VisualVerification.Run(args[2]);return 0;}
  if(args.Length>3&&args[3]=="management")return 0;
  if(args.Length>3&&args[3]=="extra"){RunSession(args[0],args[1],2,1,false,false,false);RunSession(args[0],args[1],8,6,false,false,true,"child");}
  else{if(args.Length==3){RunSession(args[0],args[1],0.1,3,false,false);RunSession(args[0],args[1],11,14,true,false);}RunSession(args[0],args[1],31,34,true,true);}
  Console.WriteLine("PASS all verification cases");return 0;
 }catch(Exception ex){Console.WriteLine("FAIL "+ex);return 1;}}
 static void RunSession(string dotnet,string fixture,double delay,double lifetime,bool expectWait,bool expectFail,bool expectReady=true,string extra=""){
  using var session=new LaunchSession();var entry=new ProgramEntry{Executable=dotnet};bool ready=false,waiting=false,failed=false,ended=false;var frame=new DispatcherFrame();var watch=Stopwatch.StartNew();
  var dashboard=new Window{Title="Verification dashboard",Width=480,Height=270};dashboard.Show();
  session.Ready+=hwnd=>{ready=true;Native.MaximizeOnPrimary(hwnd);dashboard.Hide();if(dashboard.IsVisible)throw new Exception("Dashboard did not hide");};
  session.Waiting+=()=>waiting=true;session.Failed+=()=>{failed=true;if(!session.Active||!dashboard.IsVisible)throw new Exception("Failed session lost or dashboard hidden");};
  session.Ended+=()=>{ended=true;Native.Activate(dashboard);if(!dashboard.IsVisible)throw new Exception("Dashboard did not restore");frame.Continue=false;};
  session.Start(entry,$"\"{fixture}\" {delay} {lifetime} {extra}");
  try{session.Start(entry);throw new Exception("Concurrent start allowed");}catch(InvalidOperationException){}
  // Process.Start returns before the process is guaranteed to appear in a name-based enumeration.
  var detection=Stopwatch.StartNew();while(!Native.AlreadyRunning(dotnet)&&detection.Elapsed.TotalSeconds<2)System.Threading.Thread.Sleep(50);
  if(!Native.AlreadyRunning(dotnet))throw new Exception("Existing executable not detected");
  using(var other=new LaunchSession()){try{other.Start(entry);throw new Exception("External process was attached or duplicated");}catch(InvalidOperationException){}}
  var timeout=new DispatcherTimer{Interval=TimeSpan.FromSeconds(lifetime+5)};timeout.Tick+=(_,_)=>{timeout.Stop();frame.Continue=false;};timeout.Start();Dispatcher.PushFrame(frame);timeout.Stop();dashboard.Close();
  if(!ended||session.Active||waiting!=expectWait||failed!=expectFail||ready!=(expectReady&&!expectFail))throw new Exception($"Session mismatch ready={ready} waiting={waiting} failed={failed} ended={ended}");
  Console.WriteLine($"PASS delay={delay}s wait={waiting} timeout={failed} hide/restore={!failed} single-session guard, no late join");
 }
}
