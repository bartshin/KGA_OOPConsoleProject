using System.Collections;

namespace ConsoleProject;

class Window: IInteractable {

  public char? HorizontalEdge = null;
  public char? VertialEdge = null;

  public event EventHandler OnContentRefreshed;
  public bool IsInputDisplayed { get; protected set; } = true;
  public event EventHandler<WindowMessage> OnSendMessage;
  public IInteractable.InputType AcceptType { get; protected set; }
  public IList<InputKey> AcceptKeys { 
    get; protected set; 
  }
  public Func<List<Scene>> GetNextScenes;
  public Func<Scene?, Scene> GoToNextScene;
  public Action<Scene> BackToPreviuosSceneFrom;
  public WindowType Type { get; init; }
  private Scene currentScene;
  public Scene popupScene;
  public Scene PopupScene { 
    get => this.popupScene; 
    set {
      this.popupScene = value;
      this.UpdateAccept(value ?? this.CurrentScene);
    }
  }
  public Scene CurrentScene { 
    get => this.currentScene; 
    private set {
      this.currentScene = value;
      this.UpdateAccept(value);
    }}

  public void OnRenderStarted(Renderer renderer) {
    renderer.OnRenderFinished += this.OnRenderFinished;
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
    this.AcceptKeys = initialScene.AcceptKeys;
  }

  public void ReceiveInput(InputKey input) {
    var (command, obj) = this.PopupScene != null ?
      this.PopupScene.ReceiveInput(input)
      :this.currentScene.ReceiveInput(input); 
    this.HandleCommand(command, obj);
  }

  public void SendSelection(object selection) {
    var type = selection.GetType();
    if(selection is ICollection list) {
      var message = new WindowMessage {
        Type = WindowMessage.MessageType.Selection,
        Value = list
      };
      this.OnSendMessage?.Invoke(this, message);
    }
    else
      Console.Error.WriteLine($"selection is not list: {selection}");
  }

  public void OnReceieveMessage(object sender, Window.WindowMessage arg) {
    var (command, obj) = this.CurrentScene.ReceiveMessage(arg);
    this.HandleCommand(command, obj); 
  }

  public void ChangeScene(Scene scene) {
    this.CurrentScene = scene; 
    this.OnContentRefreshed?.Invoke(this, EventArgs.Empty);
  }

  public RenderContent GetRenderContent() 
    => this.CurrentScene.GetRenderContent();

  private void UpdateAccept(Scene scene) {
    if (scene is IInteractable interactable)  {
      this.AcceptType = interactable.AcceptType;
      this.AcceptKeys = interactable.AcceptKeys;
    }
    else {
      this.AcceptType = IInteractable.InputType.None;
      this.AcceptKeys = scene.AcceptKeys;
    }
  }

  private void HandleCommand(WindowCommand command, object? obj) {
    switch (command) {
      case WindowCommand.NextScene:
        Scene nextScene = this.GoToNextScene(null);
        this.CurrentScene = nextScene;
        break;
      case WindowCommand.SendMessage:
        if (this.OnSendMessage == null) {
          Console.Error.WriteLine($"OnSendMessage is null: {this}");
          break;
        }
        if ((this.CurrentScene is NavigationScene ||
            SelectScene<object>.IsSelectScene(this.CurrentScene))
            && obj != null)
          this.SendSelection(obj);
        else {
          throw new NotImplementedException();
        }
        break;
      case WindowCommand.RefreshWindow:
        break;
      case WindowCommand.CreateScene:
        string sceneName = ((string)obj!).Split(':')[1];
        var scenes = this.GetNextScenes();  
        var scene = scenes.Find(scene => scene.SceneName.Name == sceneName)!;
        scene.IsPopup = true;
        this.PopupScene = this.GoToNextScene(scene); 
        break;
      case WindowCommand.CloseWindow:
        if (this.PopupScene != null) {
          this.BackToPreviuosSceneFrom?.Invoke(this.popupScene);
          this.PopupScene = null;
        }
        else
          this.BackToPreviuosSceneFrom?.Invoke(this.currentScene);
        break;
      case WindowCommand.None:
      default:
        return ;
    }
    // TODO: Check refresh is needed
    this.OnContentRefreshed?.Invoke(this, EventArgs.Empty);
  }

  public enum WindowType {
    Main,
    Bottom,
  }

  public enum WindowCommand {
    None,
    NextScene,
    CreateScene,
    RefreshWindow,
    SendMessage,
    CloseWindow,
  }

  public class WindowMessage: EventArgs {

    public required MessageType Type { get; init; }
    public required object Value { get; init; }

    public enum MessageType {
      Selection,
      Input,
    }
  }
}

