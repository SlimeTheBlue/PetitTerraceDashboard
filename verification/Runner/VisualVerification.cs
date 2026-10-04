using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using PetitDashboard;
using PetitDashboard.Models;
using PetitDashboard.Services;

internal static class VisualVerification
{
 public static void Run(string output)
 {


  // Application XAML is loaded by the runner's App.InitializeComponent instead.
  var window = new MainWindow();
  window.Show();
  window.Dispatcher.Invoke(() => { }, DispatcherPriority.ApplicationIdle);
  window.UpdateLayout();
  var cards = (ItemsControl)window.FindName("Cards");
  if(cards.Items.Count != 3) throw new Exception("Expected three cards");
  var labels = Descendants(window).OfType<TextBlock>().Select(x=>x.Text).ToList();
  foreach(var name in new[]{"호실관리","총회·의결관리","준비중"})
   if(!labels.Contains(name)) throw new Exception("Card missing: "+name);
  if(labels.Any(x=>x.Contains("미키") || x.Contains("입찰"))) throw new Exception("Miki branding remains");
  var buttons = Descendants(cards).OfType<Button>().ToList();
  if(buttons.Count!=3 || buttons.Any(x=>x.IsEnabled)) throw new Exception("Unconnected cards enabled");
  Capture(window,Path.Combine(output,"dashboard-preview.png"));
  var data = new ProgramRegistry { Programs = Enumerable.Range(1,4).Select(i=>new ProgramEntry { Id="test"+i, Name="업무 "+i, IsPlaceholder=true }).ToList() };
  typeof(MainWindow).GetField("registry",BindingFlags.Instance|BindingFlags.NonPublic)!.SetValue(window,data);
  Call(window,"RenderCards");
  Call(window,"NextPage",window,new RoutedEventArgs());
  if(cards.Items.Count!=1 || ((TextBlock)window.FindName("PageLabel")).Text!="2 / 2") throw new Exception("Pagination failed");
  Call(window,"PreviousPage",window,new RoutedEventArgs());
  if(cards.Items.Count!=3) throw new Exception("Previous page failed");
  var store = new RegistryStore(Path.Combine(output,"visual-registry",Guid.NewGuid().ToString("N")));
  var manager = new ProgramManager(store,store.Load()) { Owner=window };
  manager.Show(); manager.UpdateLayout();
  var list = Descendants(manager).OfType<ListBox>().Single();
  list.SelectedIndex=0;
  var item=(ProgramEntry)list.SelectedItem;
  Call(manager,"Move",1);
  if(((ProgramEntry)list.Items[1]).Id!=item.Id) throw new Exception("Card order did not change");
  item.Name="수정된 호실관리"; item.Description="수정 설명"; item.IsHidden=true;
  Call(manager,"Refresh");
  if(list.Items.Count!=2) throw new Exception("Hidden card remains visible");
  var entries=(List<ProgramEntry>)typeof(ProgramManager).GetField("programs",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(manager)!;
  store.Save(new ProgramRegistry{Programs=entries});
  var saved=new RegistryStore(store.DirectoryPath).Load();
  if(saved.Programs[1].Name!=item.Name || saved.Programs[1].Description!=item.Description || !saved.Programs[1].IsHidden) throw new Exception("Management changes not retained");
  Capture(manager,Path.Combine(output,"management-preview.png"));
  manager.Close();window.Close();
  Console.WriteLine("PASS actual WPF cards/status, pagination, management reorder/edit/hide persistence, PNG rendering");
 }
 static void Call(object instance,string name,params object[] args)=>instance.GetType().GetMethod(name,BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(instance,args);
 static IEnumerable<DependencyObject> Descendants(DependencyObject root)
 {
  for(int i=0;i<VisualTreeHelper.GetChildrenCount(root);i++){var child=VisualTreeHelper.GetChild(root,i);yield return child;foreach(var nested in Descendants(child))yield return nested;}
 }
 static void Capture(Window window,string path)
 {
  var image=new RenderTargetBitmap((int)Math.Ceiling(window.ActualWidth),(int)Math.Ceiling(window.ActualHeight),96,96,PixelFormats.Pbgra32);
  image.Render(window);var encoder=new PngBitmapEncoder();encoder.Frames.Add(BitmapFrame.Create(image));using var stream=File.Create(path);encoder.Save(stream);
 }
}
