using System;
using System.Text;

namespace ConsoleProject;

class MainScene : Scene {
  public const string MainText = "Main text";

  public Dictionary<string, object> data;

  public MainScene(Scene.ISceneName name,
      Dictionary<string, object> data)
    : base(name, Scene.SceneState.Rendering) => this.data = data;

  public override RenderContent GetRenderContent() {
     
    List<(string, RenderColor)> lists = new();
    this.AddMargin(lists);
    if (data.TryGetValue(MainScene.MainText, out object text) &&
        text is string mainText
        )  {
      var lines = this.SplitText(mainText);
      foreach (var line in lines) {
        lists.Add(("\t" + line, ConsoleColor.Gray));
      }
    }
    this.AddMargin(lists);
    var status = this.GetStatusText();
    foreach (var line in status) {
      lists.Add(("\t" + line, ConsoleColor.DarkGreen));
    }
    this.AddMargin(lists);
    return (new RenderContent(lists, RenderContent.AnimationType.None));
  }

  private List<string> GetStatusText() {
    List<string> texts = new();
    GameStatus status = new([GameStatus.Section.RemainingSoup, GameStatus.Section.RemainingWater]); 
    this.GetGameStatus(status);
    if (status.TryGet<double>(GameStatus.Section.RemainingSoup, out double soup)) {
      texts.Add(
          GameText.AddCosumableItemText(Item.ItemName.Soup.Value, soup)
          );
    }
    if (status.TryGet<double>(GameStatus.Section.RemainingWater, out double water)) {
      texts.Add(
          GameText.AddCosumableItemText(Item.ItemName.Water.Value, water)
          );
    }
    return (texts);
  }

  public override void OnRenderFinished() {
    base.OnRenderFinished();
  }

  public override (Window.WindowCommand, object?) ReceiveInput(InputKey input) {
    throw new NotImplementedException();
  }

  public override (Window.WindowCommand, object?) ReceiveMessage(Window.WindowMessage message) {
    return base.ReceiveMessage(message);
  }
}

