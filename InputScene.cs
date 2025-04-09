using System.Collections;
using System.Text;

namespace ConsoleProject;

class InputScene: Scene {

  public ICollection<(InputKey, object)> AllSelection;
  private List<object> Selected = new();
  public int MaxSelection;

  public InputScene(ISceneName name): base(name, SceneState.Rendering) {
    
  }

  public override RenderContent GetRenderContent() {
    List<(string, RenderColor)> contents = new();
    StringBuilder builder = new ();  
    if (this.Selected.Count == this.MaxSelection)  {
      builder.AppendLine("   전부 선택되었습니다");
      builder.AppendLine("   선택된 번호를 다시 누르면 최소됩니다");
      builder.AppendLine("   엔터를 누르면 다음으로 진행합니다");
    }
    else {
      builder.AppendLine("   원하는 것을 선택해 주세요");   
      builder.AppendLine("   선택된 번호를 다시 누르면 최소됩니다");
      builder.AppendLine("   전부 선택하지 않았지만 엔터를 눌러 진행할 수 있습니다");
    }
    string[] lines = builder.ToString().Split(Environment.NewLine);
    foreach (var line in lines) {
      contents.Add((line, RenderColor.Gray)); 
    }
    return (new RenderContent(contents, RenderContent.AnimationType.None));
  }

  public override (Window.WindowCommand, object?) ReceiveMessage(Window.WindowMessage message) {
    switch (message.Type) {
      case Window.WindowMessage.MessageType.Selection:
        this.State = SceneState.Rendering;
        if (this.UpdateSelection((ICollection)message.Value))
          return (Window.WindowCommand.RefreshWindow, null);
        return (Window.WindowCommand.None, null);
      default:
        return (base.ReceiveMessage(message));
    } 
  }

  public override (Window.WindowCommand, object?) ReceiveInput(InputKey input) {
    throw new NotImplementedException();
  }

  public override void OnRenderFinished() {
    this.State = Scene.SceneState.Rendering;
  }

  private bool UpdateSelection(ICollection newSelection) {
    var current = this.Selected.Count == this.MaxSelection;
    this.Selected.Clear();
    foreach (var selected in newSelection) {
      this.Selected.Add(selected);
    }
    var updated = this.Selected.Count == this.MaxSelection;
    return (current != updated);
  }
}
