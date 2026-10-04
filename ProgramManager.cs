using System.IO;
using System.Windows;
using System.Windows.Controls;
using PetitDashboard.Models;
using PetitDashboard.Services;
namespace PetitDashboard;
public sealed class ProgramManager : Window
{
 readonly RegistryStore store;
 readonly List<ProgramEntry> programs;
 readonly ListBox list = new() { DisplayMemberPath = "Name", MinHeight = 260 };
 public ProgramRegistry? Result { get; private set; }
 public ProgramManager(RegistryStore store, ProgramRegistry original)
 {
  Style = (Style)FindResource(typeof(Window)); this.store = store;
  programs = original.Programs.Select(x => x.Copy()).ToList();
  Title = "프로그램 관리"; Width = 650; Height = 460;
  WindowStartupLocation = WindowStartupLocation.CenterOwner; ResizeMode = ResizeMode.NoResize;
  var root = new DockPanel { Margin = new Thickness(22) }; Content = root;
  var info = new TextBlock { Text = "대시보드에 표시할 이름·설명과 순서를 관리합니다.\n등록 해제는 화면에서 숨기며 프로그램과 연결정보는 유지합니다.", Margin = new Thickness(0, 0, 0, 18) };
  DockPanel.SetDock(info, Dock.Top); root.Children.Add(info);
  var bottom = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 16, 0, 0) };
  DockPanel.SetDock(bottom, Dock.Bottom); root.Children.Add(bottom);
  AddButton(bottom, "저장", Save); AddButton(bottom, "취소", () => Close());
  var actions = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 12) };
  DockPanel.SetDock(actions, Dock.Top); root.Children.Add(actions);
  AddButton(actions, "수정", Edit);
  AddButton(actions, "등록 해제", () => {
   if (list.SelectedItem is ProgramEntry item && MessageBox.Show(this, $"'{item.Name}'을 대시보드에서 숨길까요?\n프로그램과 연결정보는 유지됩니다.", "등록 해제", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
   { item.IsHidden = true; Refresh(); }
  });
  AddButton(actions, "위로", () => Move(-1)); AddButton(actions, "아래로", () => Move(1));
  root.Children.Add(list); Refresh();
 }
 static void AddButton(Panel panel, string text, Action action)
 {
  var button = new Button { Content = text, Margin = new Thickness(0, 0, 8, 0) };
  button.Click += (_, _) => action(); panel.Children.Add(button);
 }
 void Refresh() { list.ItemsSource = null; list.ItemsSource = programs.Where(x => !x.IsHidden).ToList(); }
 void Move(int delta)
 {
  if (list.SelectedItem is not ProgramEntry item) return;
  var visible = programs.Where(x => !x.IsHidden).ToList();
  var target = visible.IndexOf(item) + delta;
  if (target < 0 || target >= visible.Count) return;
  var oldIndex = programs.IndexOf(item); var newIndex = programs.IndexOf(visible[target]);
  (programs[oldIndex], programs[newIndex]) = (programs[newIndex], programs[oldIndex]);
  Refresh(); list.SelectedItem = item;
 }
 void Edit()
 {
  if (list.SelectedItem is not ProgramEntry item) return;
  var editor = new ProgramEditor(item.Copy()) { Owner = this };
  if (editor.ShowDialog() != true) return;
  var edited = editor.Result!; programs[programs.IndexOf(item)] = edited;
  Refresh(); list.SelectedItem = edited;
 }
 void Save()
 {
  try {
   var result = new ProgramRegistry { Programs = programs.Select(x => x.Copy()).ToList() };
   store.Save(result); Result = result; DialogResult = true;
  } catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidOperationException)
  { MessageBox.Show(this, ex.Message, "저장 확인"); }
 }
}
public sealed class ProgramEditor : Window
{
 readonly ProgramEntry entry;
 readonly TextBox name = new(), description = new() { AcceptsReturn = true, TextWrapping = TextWrapping.Wrap, Height = 100, VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
 public ProgramEntry? Result { get; private set; }
 public ProgramEditor(ProgramEntry entry)
 {
  Style = (Style)FindResource(typeof(Window)); this.entry = entry;
  Title = "프로그램 정보 수정"; Width = 520; Height = 330;
  ResizeMode = ResizeMode.NoResize; WindowStartupLocation = WindowStartupLocation.CenterOwner;
  var root = new StackPanel { Margin = new Thickness(22) }; Content = root;
  name.Text = entry.Name; description.Text = entry.Description;
  Add(root, "프로그램 이름", name); Add(root, "설명", description);
  var buttons = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 18, 0, 0) };
  var save = new Button { Content = "확인", IsDefault = true, Margin = new Thickness(0, 0, 8, 0) };
  save.Click += (_, _) => {
   if (string.IsNullOrWhiteSpace(name.Text)) { MessageBox.Show(this, "프로그램 이름을 입력해 주세요.", "입력 확인"); return; }
   entry.Name = name.Text.Trim(); entry.Description = description.Text.Trim(); Result = entry; DialogResult = true;
  };
  buttons.Children.Add(save); buttons.Children.Add(new Button { Content = "취소", IsCancel = true }); root.Children.Add(buttons);
 }
 static void Add(Panel root, string label, UIElement field)
 { root.Children.Add(new TextBlock { Text = label, Margin = new Thickness(0, 5, 0, 4) }); root.Children.Add(field); }
}
