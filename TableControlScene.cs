using System;

namespace ConsoleProject;

class TableControlScene: TableScene {

  private int currentIndex = 0;
  private Dictionary<string, double> MaxValues = new();

  public TableControlScene(Scene.ISceneName name): base(name) { 
    this.Title = name switch {
      { Value: SceneFactory.PresentingSN.QuataScene } => "할당량 조절",
      _ => "",
    };
  }

  public override RenderContent GetRenderContent() {
    if (this.Items == null)
      this.GetStatus();
    List<(string, RenderColor)> list = new();
    string padding = new string(' ', 8);
    list.Add((this.Title, RenderColor.DarkGreen));
    list.Add(("        (방향키로 조절, 엔터키를 눌러서 닫기)", RenderColor.Yellow));
    this.AddMargin(list, 1);
    for (int i = 0; i < this.Items!.Count; ++i) {
      var (item, description) = this.Items[i];
      var content = string.Format($"{item}: {description}");
      bool isSelected = i == this.currentIndex;
      list.Add((isSelected ? string.Format($"> {content} <")
            :content, isSelected ? RenderColor.Blue: RenderColor.Gray ));
    }
    this.AddMargin(list, 4);
    return (new RenderContent(list, RenderContent.AnimationType.None));
  }

  protected override void GetStatus() {
    this.Items = new();
    var scene  = this.SceneName.Value;
    List<GameStatus.Section> sections = new();
    switch(scene ) {

      case SceneFactory.PresentingSN.QuataScene:
        sections.Add(GameStatus.Section.TodayQuota);
        sections.Add(GameStatus.Section.MaxQuota);
          break;
    };
    GameStatus status = new(sections);
    this.GetGameStatus(status);
    if (status.TryGet<(double, double)>(GameStatus.Section.TodayQuota, out var quata)) {
      this.Items.Add((Item.ItemName.Soup.Value, string.Format($"{quata.Item1}")));
      this.Items.Add((Item.ItemName.Water.Value, string.Format($"{quata.Item2}")));
    }

    if (status.TryGet<(double, double)>(GameStatus.Section.MaxQuota, out var maxQuata)) {
      this.MaxValues.Add(Item.ItemName.Soup.Value, maxQuata.Item1);
      this.MaxValues.Add(Item.ItemName.Water.Value, maxQuata.Item2);
    }
  }

  public override void OnRenderFinished() {
    this.State = SceneState.WaitingInput;
  }

  public override (Window.WindowCommand, object?) ReceiveInput(InputKey input) {
    this.State = SceneState.Rendering;
    switch (input) {
      case InputKey.UpArrow:
        this.currentIndex = Math.Max(this.currentIndex - 1, 0);
        break;
      case InputKey.DownArrow:
        this.currentIndex = Math.Min(this.currentIndex + 1, 
            this.Items.Count - 1);
        break;
      case InputKey.LeftArrow:
        this.HandleModify(false);
        break;
      case InputKey.RightArrow:
        this.HandleModify(true);
        break;
      case InputKey.Enter:
        this.HandleConfirm();
        return (Window.WindowCommand.CloseWindow, null);
    }
    return (Window.WindowCommand.RefreshWindow, null);
  }
  
  private void HandleModify(bool isIncerese) {
    switch (this.SceneName) {
      case { Value: SceneFactory.PresentingSN.QuataScene }:
        double value = double.Parse(this.Items[this.currentIndex].Item2);
        var key = this.Items[this.currentIndex].Item1;
        if (!isIncerese && value < 0.5)
          return;
        if (isIncerese && this.MaxValues[key] < value + 0.5)
          return ;
        value += isIncerese ? 0.5: - 0.5;
        this.Items[this.currentIndex] = (this.Items[this.currentIndex].Item1, value.ToString());
        break;
      default: throw new NotImplementedException();
    }
  }

  private void HandleConfirm() {
    switch (this.SceneName) {
      case { Value: SceneFactory.PresentingSN.QuataScene }:
        GameStatus status = new ([GameStatus.Section.TodayQuota]);
        double food = double.Parse(this.Items.Find(
              e => e.Item1 == Item.ItemName.Soup.Value).Item2);
        double water = double.Parse(this.Items.Find(
              e => e.Item1 == Item.ItemName.Water.Value).Item2);
        status.Add(GameStatus.Section.TodayQuota, (food, water));
        this.ModifyGameStatus(status);
        break;
    }
  }

  public override (Window.WindowCommand, object?) ReceiveMessage(Window.WindowMessage message) {
    return base.ReceiveMessage(message);
  }
}
