using System;
using System.Text;

namespace ConsoleProject;

class NavigationScene: Scene {

  public IList<string> Menu;
  public Action<string> OnSelect;
  private int currentIndex = 0;
  private const int MenusForRow = 3;

  public NavigationScene(Scene.ISceneName name): base(name, Scene.SceneState.Rendering) { }

  public override RenderContent GetRenderContent() {
    List<(string, RenderColor)> list = new();
    list.Add(("화살표와 엔터키로 메뉴를 선택하세요", RenderColor.Gray)); 
    int index = 0;
    while (index < this.Menu.Count) {
      list.Add((this.CreateMenuString(
              index, index + NavigationScene.MenusForRow),
            RenderColor.Yellow));
      index += NavigationScene.MenusForRow;
    }
    this.AddMargin(list, 2);
    return (new RenderContent(list, RenderContent.AnimationType.None));
  }

  private string CreateMenuString(int start, int end) {
    StringBuilder builder = new();
    for (int i = start; i < Math.Min(end, this.Menu.Count); ++i) {
      if (this.currentIndex == i)
      builder.Append(">" + this.Menu[i] + "<    "); 
      else
      builder.Append(this.Menu[i] + "    "); 
    }
    return (builder.ToString());
  }

  public override void OnRenderFinished() {
    this.State = SceneState.WaitingInput;
  }

  public override (Window.WindowCommand, object?) ReceiveInput(InputKey input) {
    switch (input) {
      case InputKey.UpArrow:
        this.currentIndex = Math.Max(this.currentIndex - NavigationScene.MenusForRow, 0);
        break;
      case InputKey.LeftArrow:
        if (this.currentIndex % NavigationScene.MenusForRow != 0)
          this.currentIndex = Math.Max(this.currentIndex - 1, 0);
        break;
      case InputKey.RightArrow:
        if ((this.currentIndex % NavigationScene.MenusForRow)
            < NavigationScene.MenusForRow - 1)
          this.currentIndex = Math.Min(this.currentIndex + 1, this.Menu.Count - 1);
        break;
      case InputKey.DownArrow:
        this.currentIndex = Math.Min(this.currentIndex + NavigationScene.MenusForRow, this.Menu.Count - 1);
        break;
      case InputKey.Enter:
        this.OnSelect(this.Menu[this.currentIndex]);
        return (Window.WindowCommand.SendMessage,
           new string[]{ this.Menu[this.currentIndex] });
      default:
        break;
    }
    return (Window.WindowCommand.RefreshWindow, null);
  }

  public override (Window.WindowCommand, object?) ReceiveMessage(Window.WindowMessage message) {
    return base.ReceiveMessage(message);
  }
}
