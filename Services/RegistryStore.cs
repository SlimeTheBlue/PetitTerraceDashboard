using System.IO;
using System.Text.Json;
using PetitDashboard.Models;
namespace PetitDashboard.Services;
public sealed class RegistryStore
{
 public string DirectoryPath { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"SlimeTheBlue","PetitDashboard");
 public RegistryStore(){}
 internal RegistryStore(string directoryPath){DirectoryPath=directoryPath;}
 public string FilePath => Path.Combine(DirectoryPath,"programs.json");
 public bool CanSave { get; private set; } = true;
 public string? LoadWarning { get; private set; }
 static readonly JsonSerializerOptions Options = new() { WriteIndented = true };
 public ProgramRegistry Load()
 {
  if (!File.Exists(FilePath)) return Defaults();
  try { var data = JsonSerializer.Deserialize<ProgramRegistry>(File.ReadAllText(FilePath)) ?? throw new InvalidDataException("등록정보가 비어 있습니다.");
   if(data.Version!=1 || data.Programs==null || data.Programs.Any(x=>x==null || string.IsNullOrWhiteSpace(x.Id) || string.IsNullOrWhiteSpace(x.Name)) || data.Programs.Select(x=>x.Id).Distinct().Count()!=data.Programs.Count) throw new InvalidDataException("등록정보 형식 또는 버전을 확인하세요.");
   return data;
  } catch(Exception ex) when(ex is IOException or UnauthorizedAccessException or JsonException or InvalidDataException) {
   CanSave=false; LoadWarning=$"등록정보를 읽지 못했습니다. 원본은 유지되며 덮어쓰지 않습니다.\n{FilePath}\n{ex.Message}"; return Defaults();
  }
 }
 public void Save(ProgramRegistry registry)
 {
  if(!CanSave) throw new InvalidOperationException("기존 등록정보 확인 전에는 저장할 수 없습니다.");
  Directory.CreateDirectory(DirectoryPath); var temp=FilePath+"."+Guid.NewGuid().ToString("N")+".tmp";
  try { using(var stream=new FileStream(temp,FileMode.CreateNew,FileAccess.Write,FileShare.None)) { JsonSerializer.Serialize(stream,registry,Options); stream.Flush(true); }
   if(File.Exists(FilePath)) File.Replace(temp,FilePath,null); else File.Move(temp,FilePath);
  } finally { if(File.Exists(temp)) File.Delete(temp); }
 }
 static ProgramRegistry Defaults() => new() { Programs = [
  new() { Id="rooms",Name="호실관리",Description="상가 호실 현황 및 입점 상태를\n관리할 수 있습니다.",BuiltInIcon="rooms" },
  new() { Id="assembly",Name="총회·의결관리",Description="총회 일정, 의결 안건 및\n결과를 관리할 수 있습니다.",IsPlaceholder=true,BuiltInIcon="assembly" },
  new() { Id="pending",Name="준비중",Description="새로운 업무 프로그램을\n준비하고 있습니다.",IsPlaceholder=true,BuiltInIcon="generic" }
 ] };
}
