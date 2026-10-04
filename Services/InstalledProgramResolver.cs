using System.IO;
using Microsoft.Win32;
using PetitDashboard.Models;
namespace PetitDashboard.Services;

// Dedicated PetitBuildingManager installer contract; development executable is fallback.
public sealed class InstalledProgramResolver
{
 readonly string standard, installed;
 readonly Func<string,bool> exists;
 public InstalledProgramResolver() : this(
  DevelopmentPath(),
  InstalledPath(),File.Exists) {}
 internal InstalledProgramResolver(string standard,string installed,Func<string,bool> exists)
 {this.standard=standard;this.installed=installed;this.exists=exists;}
 static string DevelopmentPath()
 {
#if DEBUG
  return @"D:\Dev\00_슬라임공장\쁘띠테라스_호실관리\dist\쁘띠테라스_호실관리.exe";
#else
  return "";
#endif
 }
 static string InstalledPath()
 {
  const string keyPath = @"Software\Microsoft\Windows\CurrentVersion\Uninstall\{E7B03965-B03A-4F38-AD14-4F7F5D916C9E}_is1";
  try {
   using var key=Registry.CurrentUser.OpenSubKey(keyPath);
   var directory=key?.GetValue("InstallLocation") as string;
   return string.IsNullOrWhiteSpace(directory)?"":Path.Combine(directory,"쁘띠테라스_호실관리.exe");
  } catch(Exception ex) when(ex is System.Security.SecurityException or UnauthorizedAccessException or IOException){return "";}
 }
 static bool SamePath(string a,string b)
 {try{return !string.IsNullOrWhiteSpace(a)&&!string.IsNullOrWhiteSpace(b)&&string.Equals(Path.GetFullPath(a),Path.GetFullPath(b),StringComparison.OrdinalIgnoreCase);}catch(Exception ex) when(ex is ArgumentException or NotSupportedException or PathTooLongException){return false;}}
 public bool Refresh(ProgramRegistry registry)
 {
  var entry=registry.Programs.FirstOrDefault(x=>x.Id=="rooms");
  if(entry==null||entry.IsPlaceholder)return false;
  if(!string.IsNullOrWhiteSpace(entry.Executable)&&!SamePath(entry.Executable,standard)&&!SamePath(entry.Executable,installed))return false;
  var target=!string.IsNullOrWhiteSpace(installed)&&exists(installed)?installed:standard;
  if(!exists(target)||SamePath(entry.Executable,target))return false;
  var previousDirectory=string.IsNullOrWhiteSpace(entry.Executable)?"":Path.GetDirectoryName(entry.Executable)??"";
  if(string.IsNullOrWhiteSpace(entry.WorkingDirectory)||SamePath(entry.WorkingDirectory,previousDirectory))entry.WorkingDirectory=Path.GetDirectoryName(target)??"";
  entry.Executable=target;
  // Discovery stays in memory; preserve names, order, hidden state and the JSON.
  return true;
 }
}
