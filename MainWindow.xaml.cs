using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PetitDashboard.Models;
using PetitDashboard.Services;
using Native = PetitDashboard.Platform.Windows;
namespace PetitDashboard;
public partial class MainWindow : Window
{
 readonly RegistryStore store=new(); ProgramRegistry registry; int page; readonly LaunchSession session=new(); bool placementPending; string installationStamp=""; readonly InstalledProgramResolver installedPrograms=new(); bool installationRefreshPending; readonly uint productsChangedMessage=Native.RegisterWindowMessage("SlimeTheBlue.PetitDashboard.ProductsChanged.v1");
 public MainWindow() {
  InitializeComponent(); registry=store.Load(); installedPrograms.Refresh(registry); SourceInitialized+=(_,_)=>{
   var source=System.Windows.Interop.HwndSource.FromHwnd(new System.Windows.Interop.WindowInteropHelper(this).Handle);
   source?.AddHook((nint hwnd,int message,nint wParam,nint lParam,ref bool handled)=>{if(message==0x007E||message==0x001A)RefreshPlacement();if(productsChangedMessage!=0&&(uint)message==productsChangedMessage){handled=true;Dispatcher.BeginInvoke(new Action(RequestInstallationRefresh));}return 0;});
   Native.PlaceDashboard(this);
  RefreshInstallation();
  }; DpiChanged+=(_,_)=>RefreshPlacement(); RenderCards();
  Loaded+=(_,_)=>{if(store.LoadWarning!=null) MessageBox.Show(this,store.LoadWarning,"등록정보 확인");};
  session.Waiting+=()=>SetStatus("프로그램 실행을 기다리고 있습니다.");
  session.Ready+=hwnd=>{Hide();};
  session.Failed+=()=>{SetStatus("실행 화면을 확인하지 못했습니다. 프로그램이 종료되면 다시 실행할 수 있습니다."); MessageBox.Show(this,"30초 안에 프로그램 화면을 확인하지 못했습니다. 실행 중인 프로그램은 강제 종료하지 않습니다.","실행 확인");};
  session.Ended+=()=>{if(installationRefreshPending)RequestInstallationRefresh();SetStatus(null);ManageButton.IsEnabled=store.CanSave;RenderCards();Native.Activate(this);};
  session.ExitedWithoutWindow+=()=>MessageBox.Show(this,"실행한 프로그램이 화면을 표시하기 전에 종료되었습니다. 실행 파일과 작업 폴더를 확인해 주세요.","실행 확인");

  Activated+=(_,_)=>RefreshInstallation();
  Closed+=(_,_)=>{session.Dispose();};
 }
 void RefreshPlacement(){
  if(placementPending)return;placementPending=true;
  Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Loaded,new Action(()=>{
   placementPending=false;if(!IsVisible||WindowState==WindowState.Minimized)return;
   Native.PlaceDashboard(this);
   foreach(Window child in OwnedWindows)if(child.IsVisible){child.Left=Left+(ActualWidth-child.ActualWidth)/2;child.Top=Top+(ActualHeight-child.ActualHeight)/2;}
  }));
 }
 void SetStatus(string? text){StatusText.Text=text??"";StatusBox.Visibility=text==null?Visibility.Collapsed:Visibility.Visible;}
 string InstallationStamp()=>string.Join("|",registry.Programs.Select(x=>$"{x.Id}:{x.Executable}:{File.Exists(x.Executable)}"));
 void RequestInstallationRefresh(){if(session.Active){installationRefreshPending=true;return;}installationRefreshPending=false;RefreshInstallation();}
 void RefreshInstallation(){if(session.Active)return;var changed=installedPrograms.Refresh(registry);if(changed||installationStamp!=InstallationStamp())RenderCards();}
 void RenderCards() {
  if(!session.Active)installedPrograms.Refresh(registry);
  installationStamp=InstallationStamp();
  var visible=registry.Programs.Where(x=>!x.IsHidden).ToList();
  page=Math.Clamp(page,0,Math.Max(0,(visible.Count-1)/3));Cards.Items.Clear();
  foreach(var entry in visible.Skip(page*3).Take(3)) Cards.Items.Add(CreateCard(entry));
  PageLabel.Text=$"{page+1} / {Math.Max(1,(visible.Count+2)/3)}";Previous.IsEnabled=page>0;Next.IsEnabled=(page+1)*3<visible.Count;ManageButton.IsEnabled=store.CanSave&&!session.Active;
 }
 Border CreateCard(ProgramEntry entry) {
  var stack=new StackPanel();stack.Children.Add(CreateIcon(entry));
  stack.Children.Add(new TextBlock{Text=entry.Name,FontSize=25,FontWeight=FontWeights.Bold,TextAlignment=TextAlignment.Center,TextWrapping=TextWrapping.Wrap,Height=43,Margin=new Thickness(0,22,0,0)});
  stack.Children.Add(new TextBlock{Text=entry.Description,FontSize=14,TextAlignment=TextAlignment.Center,TextWrapping=TextWrapping.Wrap,Height=65,Margin=new Thickness(0,12,0,0)});
  var button=new Button{Content=entry.IsPlaceholder?"준비중":File.Exists(entry.Executable)?"시스템 실행  →":"미설치",IsEnabled=!entry.IsPlaceholder&&File.Exists(entry.Executable)&&!session.Active,Height=42,Margin=new Thickness(0,15,0,0),Background=new SolidColorBrush(entry.IsPlaceholder?Color.FromRgb(224,233,245):Color.FromRgb(0,99,220)),Foreground=entry.IsPlaceholder?Brushes.SlateGray:Brushes.White};
  button.Click+=(_,_)=>Launch(entry);stack.Children.Add(button);
  return new Border{Width=280,Height=310,Padding=new Thickness(24,22,24,16),Margin=new Thickness(10,0,10,0),CornerRadius=new CornerRadius(18),BorderThickness=new Thickness(entry.IsPlaceholder?1:2),BorderBrush=new SolidColorBrush(Color.FromRgb(139,183,233)),Background=new SolidColorBrush(Color.FromArgb(242,245,250,255)),Child=stack};
 }
 UIElement CreateIcon(ProgramEntry entry) {
  if(!string.IsNullOrEmpty(entry.IconPath))try{var image=new BitmapImage();image.BeginInit();image.CacheOption=BitmapCacheOption.OnLoad;image.UriSource=new Uri(entry.IconPath,UriKind.Absolute);image.EndInit();return new Image{Source=image,Width=70,Height=70};}catch(Exception ex) when(ex is IOException or NotSupportedException or UriFormatException){}
  string geometry=entry.BuiltInIcon switch {
   "rooms"=>"M 7,18 L 7,43 41,43 41,18 M 3,18 L 9,6 39,6 45,18 Z M 15,43 L 15,29 25,29 25,43 M 31,28 L 37,28 37,35 31,35 Z M 13,6 L 11,18 M 22,6 L 22,18 M 31,6 L 33,18",
   "assembly"=>"M 18,11 A 6,6 0 1 0 30,11 A 6,6 0 1 0 18,11 M 4,18 A 5,5 0 1 0 14,18 A 5,5 0 1 0 4,18 M 34,18 A 5,5 0 1 0 44,18 A 5,5 0 1 0 34,18 M 14,35 L 14,29 C 14,19 34,19 34,29 L 34,35 M 1,35 L 1,31 C 1,23 12,23 12,31 M 36,31 C 36,23 47,23 47,31 L 47,35 M 2,38 L 46,38 M 7,38 L 7,45 41,45 41,38",
   "schedule"=>"M 7,10 L 43,10 43,43 7,43 Z M 7,19 L 43,19 M 16,4 L 16,14 M 34,4 L 34,14 M 14,27 L 18,27 M 24,27 L 28,27 M 34,27 L 38,27 M 14,35 L 18,35 M 24,35 L 28,35",
   _=>"M 6,7 L 44,7 44,43 6,43 Z M 6,17 L 44,17 M 14,27 L 36,27 M 14,35 L 29,35"};
  return new Viewbox{Width=70,Height=70,Child=new System.Windows.Shapes.Path{Data=Geometry.Parse(geometry),Stroke=new SolidColorBrush(entry.IsPlaceholder?Color.FromRgb(130,154,188):Color.FromRgb(0,91,211)),StrokeThickness=2.5,StrokeLineJoin=PenLineJoin.Round,StrokeStartLineCap=PenLineCap.Round,StrokeEndLineCap=PenLineCap.Round}};
 }
 void Launch(ProgramEntry entry){try{installedPrograms.Refresh(registry);session.Start(entry);SetStatus("프로그램을 실행하고 있습니다.");RenderCards();}catch(Exception ex){MessageBox.Show(this,ex.Message,"실행 확인");}}
 void Manage(object sender,RoutedEventArgs e){if(session.Active)return;var manager=new ProgramManager(store,registry){Owner=this};if(manager.ShowDialog()==true){registry=manager.Result!;RenderCards();}}
 void PreviousPage(object sender,RoutedEventArgs e){page--;RenderCards();} void NextPage(object sender,RoutedEventArgs e){page++;RenderCards();}
 void DragHeader(object sender,MouseButtonEventArgs e){if(e.ChangedButton==MouseButton.Left&&e.OriginalSource is not Button)DragMove();}
 void Minimize(object sender,RoutedEventArgs e)=>WindowState=WindowState.Minimized;
 void CloseDashboard(object sender,RoutedEventArgs e)=>Close();
}
