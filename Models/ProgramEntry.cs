namespace PetitDashboard.Models;
public sealed class ProgramEntry
{
 public string Id { get; set; } = Guid.NewGuid().ToString("N");
 public string Name { get; set; } = "";
 public string Description { get; set; } = "";
 public string Executable { get; set; } = "";
 public string WorkingDirectory { get; set; } = "";
 public string IconPath { get; set; } = "";
 public string BuiltInIcon { get; set; } = "generic";
 public bool IsPlaceholder { get; set; }
 public bool IsHidden { get; set; }
 public ProgramEntry Copy() => (ProgramEntry)MemberwiseClone();
}
public sealed class ProgramRegistry { public int Version { get; set; } = 1; public List<ProgramEntry> Programs { get; set; } = []; }
