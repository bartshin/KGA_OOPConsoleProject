using System;

namespace ConsoleProject;

class TableScene : Scene, INavigatable {

  protected string Title { get; init; }
  protected List<(string, string)> Items;

  public TableScene(Scene.ISceneName name): base(name, Scene.SceneState.Rendering) {
    switch (name) {
      case { Value: SceneFactory.PresentingSN.InventoryScene }:
        this.Title = "인벤토리";
        break;
      case { Value: SceneFactory.PresentingSN.CharacterStatusScene }:
        this.Title = "캐릭터";
        break;
    }
  }

  protected virtual void GetStatus() {
    this.Items = new();
    var scene  = this.SceneName.Value;
    GameStatus.Section section = scene switch {
      SceneFactory.PresentingSN.CharacterStatusScene => GameStatus.Section.CharacterStatus ,
      SceneFactory.PresentingSN.InventoryScene => GameStatus.Section.Items
    };
    GameStatus status = new([section]);
    this.GetGameStatus(status);
    status.TryGet<List<(string ,string)>>(section, out var list);
    list.ForEach((item) => this.Items.Add(item));
  } 

  public IList<string> Menu => new List<string>() { };

  public override RenderContent GetRenderContent() {
    if (this.Items == null)
      this.GetStatus();
    List<(string, RenderColor)> list = new();
    string padding = new string(' ', 8);
    list.Add((this.Title, RenderColor.DarkGreen));
    list.Add(("(엔터키를 눌러서 닫기)", RenderColor.Yellow));
    this.AddMargin(list, 1);
    foreach (var (item, description) in this.Items!) {
      list.Add((item, RenderColor.Gray));
      var lines = this.SplitText(description);
      foreach (var line in lines) 
        list.Add((padding + line, RenderColor.DarkBlue));
    }
    this.AddMargin(list, 4);
    return (new RenderContent(list, RenderContent.AnimationType.None));
  }

  public override (Window.WindowCommand, object?) ReceiveInput(InputKey input) {
    if (input == InputKey.Enter) {
      return (Window.WindowCommand.CloseWindow, null);
    }
    return (Window.WindowCommand.None, null);
  }
}
