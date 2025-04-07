namespace ConsoleProject;

class Window: IInteractable {

  private Dictionary<string, Func<Scene>> sceneConstructors = new() ;
  public event EventHandler OnContentRefreshed;
  public bool IsInputDisplayed { get; protected set; } = true;
  public IInteractable.InputType AcceptType { get; protected set; }
  public IEnumerable<InputKey> AcceptKeys { 
    get; protected set; 
  }
  public WindowType Type { get; init; }
  private Scene currentScene;
  public Scene CurrentScene { 
    get => this.currentScene; 
    private set {
      this.currentScene = value;
      this.AcceptKeys = value.AcceptKeys;
    }}

  public void OnRenderStarted(Renderer renderer) {
    renderer.OnRenderFinished += this.OnRenderFinished;
  }

  public void AddScene(string name, Func<Scene> constructor) {
    this.sceneConstructors.Add(name, constructor);
  }

  public void OnRenderFinished(object? sender, EventArgs args) {
    this.CurrentScene.OnRenderFinished();
    switch (this.CurrentScene.State) {
      case Scene.SceneState.WaitingInput:
        if (this.AcceptKeys.Count() == 0) 
          this.AcceptType = IInteractable.InputType.AnyKey;
        else
          this.AcceptType = IInteractable.InputType.SpecificKeys;
        break;
    }
  }

  public Window(Scene initialScene, WindowType type) {
    this.CurrentScene = initialScene;
    this.Type = type;
    this.AcceptType = IInteractable.InputType.None;
    this.AcceptKeys = new List<InputKey>();
  }

  public void ReceiveInput(InputKey input) {
    var (command, obj) = this.currentScene.ReceiveInput(input); 
    switch (command) {
      case WindowCommand.NextScene:
        if (obj is string nextSceneName) {
          Console.Clear();
          if (this.sceneConstructors.ContainsKey(nextSceneName)) {
            Scene nextScene = this.sceneConstructors[nextSceneName]();
            this.CurrentScene = nextScene;
            this.OnContentRefreshed?.Invoke(this, EventArgs.Empty);
          }
        }
        break;
        default:
          break;
    }
  }

  public RenderContent GetRenderContent() 
    => this.CurrentScene.GetRenderContent();

  public enum WindowType {
    Main,
    Bottom
  }

  public enum WindowCommand {
    None,
    NextScene,
    CloseWindow,
  }
}

