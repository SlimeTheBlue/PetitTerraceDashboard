using System.IO;
using PetitDashboard.Models;
using PetitDashboard.Services;
internal static class RoomDiscoveryVerification
{
 public static void Run()
 {
  var standard=Path.GetFullPath("room-default/상가호실관리.exe");
  var installed=Path.GetFullPath("room-installed/상가호실관리.exe");
  var registry=new ProgramRegistry{Programs=[new ProgramEntry{Id="rooms",Name="사용자 이름",Description="사용자 설명",IsHidden=true,IconPath="keep.png"},new ProgramEntry{Id="assembly",Name="총회·의결관리",IsPlaceholder=true}]};
  var resolver=new InstalledProgramResolver(standard,installed,path=>path==installed);
  if(!resolver.Refresh(registry))throw new Exception("Installed connection not found");
  var entry=registry.Programs[0];
  if(entry.Executable!=installed||entry.WorkingDirectory!=Path.GetDirectoryName(installed)||!entry.IsHidden||entry.Name!="사용자 이름"||entry.Description!="사용자 설명"||entry.IconPath!="keep.png")throw new Exception("User settings changed during discovery");
  if(resolver.Refresh(registry))throw new Exception("Unchanged connection refreshed");
  entry.Executable=Path.GetFullPath("user-selected.exe");entry.WorkingDirectory="custom-directory";
  if(resolver.Refresh(registry)||entry.WorkingDirectory!="custom-directory")throw new Exception("Custom connection overwritten");
  entry.Executable="";entry.WorkingDirectory="";
  if(new InstalledProgramResolver(standard,installed,_=>false).Refresh(registry)||entry.Executable!="")throw new Exception("Missing EXE connected");
  Console.WriteLine("PASS room discovery, user settings/hidden state/custom path preservation and missing EXE");
 }
}
