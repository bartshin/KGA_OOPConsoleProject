using System;
using System.Text;

namespace ConsoleProject;

class MainScene : Scene, INavigatable {
  public const string MainText = "Main text";

  public Dictionary<string, object> data;
  public IList<string> Menu => this.menu;

  private List<string> menu =
    new List<string>() {
      MainScene.MenuType.CharacterStatus,
      MainScene.MenuType.Inventory,
      MainScene.MenuType.Quata,
      MainScene.MenuType.Next
    };


  public MainScene(Scene.ISceneName name,
      Dictionary<string, object> data)
    : base(name, Scene.SceneState.Rendering) {
      this.data = data;
      this.NextSceneName = null;
    }

  public void UpdateMenu() {

    GameStatus status = new([GameStatus.Section.IsAnyCharacterFarming]);
    this.GetGameStatus?.Invoke(status);
    if (status.TryGet<bool>(GameStatus.Section.IsAnyCharacterFarming, out bool isFarming)) {
      if (!isFarming && !this.menu.Contains(MainScene.MenuType.Farming))
        this.menu.Add(MainScene.MenuType.Farming);
      else if (isFarming) 
        this.menu.Remove(MainScene.MenuType.Farming);
    }
  }

  public override RenderContent GetRenderContent() {
    List<(string, RenderColor)> lists = new();
    this.AddMargin(lists);
    if (data.TryGetValue(MainScene.MainText, out object text) &&
        text is string mainText
        )  {
      var lines = this.SplitText(mainText);
      foreach (var line in lines) {
        lists.Add(("    "+ line, ConsoleColor.Gray));
      }
    }

    this.AddMargin(lists);
    var status = this.GetStatusText();
    foreach (var line in status) {
      lists.Add(("    " + line, ConsoleColor.DarkGreen));
    }
    this.AddMargin(lists);
    return (new RenderContent(lists, RenderContent.AnimationType.None));
  }

  private List<string> GetStatusText() {
    List<string> texts = new();
    GameStatus status = new([
        GameStatus.Section.RemainingSoup,
        GameStatus.Section.RemainingWater,
        GameStatus.Section.TodayQuota,
        GameStatus.Section.TodayDead,
        GameStatus.Section.TodayFarming,
        GameStatus.Section.YesterDayFarming,
    ]); 
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
    if (status.TryGet<(double, double)>(GameStatus.Section.TodayQuota, out var quata)) {
      texts.Add("");
      texts.Add(
          string.Format($"오늘의 할당량 수프: {quata.Item1}, 물: {quata.Item2}")
          );
    }
    if (status.TryGet<IList<string>>(GameStatus.Section.TodayDead, out var deadCharacters)) {
      foreach (var character in deadCharacters) {
         texts.Add(string.Format($"오늘 {character}가 하늘나라로 갔습니다.")); 
      }
    }
    if (status.TryGet<string>(GameStatus.Section.YesterDayFarming, out var farmingResult)) {
      texts.Add((string.Format($"어제의 탐험 결과: {farmingResult}")));
    }
    else
      texts.Add((string.Format($"어제의 탐험 결과가 없습니다")));
    if (status.TryGet<string>(GameStatus.Section.TodayFarming, out var farmingCharacter)) {
      texts.Add(string.Format($"오늘의 탐험: {farmingCharacter}"));
    }
    return (texts);
  }

  public override void OnRenderFinished() {
    this.State = SceneState.WaitingInput;
  }

  public override (Window.WindowCommand, object?) ReceiveInput(InputKey input) {
    this.State = SceneState.Rendering;
    return (Window.WindowCommand.None, null);
  }

  public override (Window.WindowCommand, object?) ReceiveMessage(Window.WindowMessage message) {
    this.State = SceneState.Rendering;
    if (message.Type == Window.WindowMessage.MessageType.Selection) {
      string menu = ((IList<string>)message.Value)[0];
      switch (menu) {
        case MainScene.MenuType.CharacterStatus:
          return (Window.WindowCommand.CreateScene, SceneFactory.Shared.GetFullName(SceneFactory.PresentingSN.CharacterStatus));
        case MainScene.MenuType.Inventory:
          return (Window.WindowCommand.CreateScene, SceneFactory.Shared.GetFullName(SceneFactory.PresentingSN.Inventory));
        case MainScene.MenuType.Quata:
          return ( Window.WindowCommand.CreateScene,
              SceneFactory.Shared.GetFullName(SceneFactory.PresentingSN.Quata));
        case MainScene.MenuType.Next:
          return (Window.WindowCommand.NextScene, null);
        case MainScene.MenuType.Farming:
          return (Window.WindowCommand.NextScene, SceneFactory.Shared.GetFullName(SceneFactory.SelectSN.SelectFarmer));
      }
    }
    return base.ReceiveMessage(message);
  }

  readonly struct MenuType {
    public const string CharacterStatus = "캐릭터 상태";
    public const string Inventory = "인벤토리";
    public const string Quata = "할당량";
    public const string Farming = "탐험";
    public const string Next = "다음날로";
  }
}

