using System.Windows;
namespace PetitDashboard;
public partial class App : Application {
 System.Threading.Mutex? instance;
 protected override void OnStartup(StartupEventArgs e){instance=new System.Threading.Mutex(true,"Local\\SlimeTheBlue.PetitDashboard",out var acquired);if(!acquired){MessageBox.Show("대시보드가 이미 실행 중입니다. 실행 중인 창을 확인해 주세요.","대시보드");Shutdown();return;}base.OnStartup(e);}
 protected override void OnExit(ExitEventArgs e){if(instance!=null){try{instance.ReleaseMutex();}catch(ApplicationException){}instance.Dispose();}base.OnExit(e);}
}
