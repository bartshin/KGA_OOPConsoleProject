namespace ConsoleProject;

class Game {
  public int TotalDays = 1;
  public bool IsEnded { get; private set; } = false;
  public SceneProgressor Scenes { get; private set; }
  public List<Window> Windows { get; } = new List<Window>();
  public Window MainWindow => this.Windows.Find(w => w.Type == Window.WindowType.Main)!;
  public Window? BottomWindow { 
    get => this.Windows.Find(w => w.Type == Window.WindowType.Bottom);
    set {
      if (value == null) {
        this.RemoveBottomWindow();
        return;
      }
      this.CreateBottomWindow(value);
    }
  }
  readonly public GameConfig Config = new GameConfig();
  private GameData data = new GameData();

  public Game(SceneProgressor scenes) {
    this.Scenes = scenes;
    this.Scenes.GetGameStatus = this.ServeGameStatus;
    this.SetMainWindow();
  }

  public void ServeGameStatus(GameStatus status) {
    foreach (var section in status.Sections) {
      switch (section) {
        case GameStatus.Section.TotalDay:
          status.Add(section, this.TotalDays);
          break;
        case GameStatus.Section.RemainingWater:
          status.Add(section, this.data.Inven.GetTotalWater());
          break;
        case GameStatus.Section.RemainingSoup:
          status.Add(section, this.data.Inven.GetTotalSoup());
          break;
        case GameStatus.Section.CharacterStatus:
          throw new NotImplementedException();
          break;
      } 
    }
  }

  public Scene GoToNextScene(Window window) {
    switch (window.Type) {
      case Window.WindowType.Main:
        if (window.CurrentScene is ISelectScene selectScene) 
          this.GetSelection(selectScene);
        var nextScene = this.SelectSceneFrom(this.Scenes.GetNextScenes());
        if (SelectScene<object>.IsSelectScene(nextScene))
          this.SetBottomWindow(nextScene);
        else
          this.BottomWindow = null;
        this.Scenes.ProgressToNextScene(nextScene);
        return (nextScene);
      default: throw new NotImplementedException();
    }
  }

  private void GetSelection(ISelectScene selectScene) {
    var selection = selectScene.CurrentSelection;
    Scene scene = (Scene)selectScene;
    switch (scene.SceneName) {
      case { Name: SceneFactory.SelectSN.SelectCharacterScene }:
        foreach (string name in selection) {
          var character = CharacterFactory.CharacterName.GetPlayable(name);
          this.PickCharacters(CharacterFactory.Shared.Create(character));
        }
        break;
      case { Name: SceneFactory.SelectSN.SelectItemScene }:
        foreach (string item in selection) {
          this.data.Inven.Add((Item.ItemName)item);     
        }
        break;
      default: throw new NotImplementedException();
    }
  }

  private void SetMainWindow() {
    var mainWindow = new Window(this.Scenes.CurrentScene, 
        Window.WindowType.Main);
    mainWindow.GoToNextScene = () => this.GoToNextScene(mainWindow);
    this.Windows.Add(mainWindow);
  }

  private void RemoveBottomWindow() {
    if (this.BottomWindow == null)
      return ;
    var window = this.BottomWindow;
    if (window.OnReceieveMessage != null)
      this.MainWindow.OnSendMessage -= window.OnReceieveMessage!;
    this.Windows.Remove(window);
    Renderer.Shared.RemoveWindow(window);
  }

  private void SetBottomWindow(Scene mainScene) {
    if (SelectScene<object>.IsSelectScene(mainScene)) {
        var selectScene = (ISelectScene)mainScene;
      if (this.BottomWindow == null) {
        InputScene scene = (InputScene)SceneFactory.Shared.Build(
            SceneFactory.AssistanceSN.Input
            );
        scene.AllSelection = selectScene.AllSelections;
        scene.MaxSelection = selectScene.MaxSelection;
        var window = new Window(scene, Window.WindowType.Bottom);
        this.MainWindow.OnSendMessage += window.OnReceieveMessage;
        this.BottomWindow = window;
      }
      else if (this.BottomWindow.CurrentScene is InputScene inputScene) {
        inputScene.AllSelection = selectScene.AllSelections;
        inputScene.MaxSelection = selectScene.MaxSelection;
        this.BottomWindow.Refresh();
      }
      else {
        throw new NotImplementedException();
      }
    }
    else {
      throw new NotImplementedException();
    }
  }

  private void CreateBottomWindow(Window window) {
    this.Windows.Add(window);
    Renderer.Shared.SetWindow(window);
  }

  public Scene SelectSceneFrom(List<Scene> scenes) {
    // TODO: select next scene
    return (scenes[0]);
  }

  readonly public struct GameConfig {
    readonly int numberOfTotalDays = 30;

    public GameConfig() {}
  }

  private void PickCharacters(params Character[] characters) {
    foreach (Character character in characters) {
       this.data.AddCharacter(character); 
    }
  }

  private class GameData {
    public List<Character> Characters { get; private set; }
    public Inventory Inven { get; private set; }

    public GameData() {
      this.Characters = new List<Character>();
      this.Inven = new Inventory();
    }

    public void AddCharacter(Character character) {
      this.Characters.Add(character); 
    }
  }
}

