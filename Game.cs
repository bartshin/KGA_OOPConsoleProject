namespace ConsoleProject;

class Game {
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
      this.CreateBottonWindow(value);
    }
  }
  readonly public GameConfig Config = new GameConfig();
  private GameData data = new GameData();

  public Game(SceneProgressor scenes) {
    this.Scenes = scenes;
    this.SetMainWindow();
  }

  public Scene GoToNextScene(Window window) {
    switch (window.Type) {
      case Window.WindowType.Main:
        if (window.CurrentScene is ISelectScene selectScene) 
          this.GetSelection(selectScene);
        var nextScene = this.SelectSceneFrom(this.Scenes.GetNextScenes());
        if (SelectScene<object>.IsSelectScene(nextScene))
          this.SetBottomWindow(nextScene);
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
    throw new NotImplementedException();
  }

  private void SetBottomWindow(Scene mainScene) {
    if (SelectScene<object>.IsSelectScene(mainScene)) {
        var selectScene = (ISelectScene)mainScene;
      if (this.BottomWindow == null) {
        InputScene scene = (InputScene)SceneFactory.Shared.Build(
            SceneFactory.AssistanceSN.InputSceneName
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

  private void CreateBottonWindow(Window window) {
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
    public List<Character> Characters { get; }

    public GameData() {
      this.Characters = new List<Character>();
    }

    public void AddCharacter(Character character) {
      this.Characters.Add(character); 
    }
  }
}

