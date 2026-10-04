using System.Windows;
using System.Runtime.InteropServices;
namespace PetitDashboard;
internal static class Program
{
 [DllImport("shell32.dll",CharSet=CharSet.Unicode)] static extern int SetCurrentProcessExplicitAppUserModelID(string id);
 [STAThread]
 public static void Main()
 {
  // A development DLL is hosted by dotnet.exe, whose manifest is not this app's manifest.
  SetCurrentProcessExplicitAppUserModelID("SlimeTheBlue.PetitDashboard.1");
  Platform.Windows.EnablePerMonitorDpi();
  var app = new App(); app.InitializeComponent(); app.Run();
 }
}
