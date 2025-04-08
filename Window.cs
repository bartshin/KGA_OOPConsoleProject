using System.Collections;

namespace ConsoleProject;

class Window: IInteractable {

  public event EventHandler OnContentRefreshed;
  public bool IsInputDisplayed { get; protected set; } = true;
  public event EventHandler<WindowMessage<object>> OnSendMessage;
  public IInteractable.InputType AcceptType { get; protected set; }
  public IEnumerable<InputKey> AcceptKeys { 
    get; protected set; 
  }
  public Func<Scene> GoToNextScene;
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
          Scene nextScene = this.GoToNextScene();
          this.CurrentScene = nextScene;
        }
        break;
      case WindowCommand.SendMessage:
        if (this.OnSendMessage == null) {
          Console.Error.WriteLine($"OnSendMessage is null: {this}");
          break;
        }
        if (SelectScene<object>.IsSelectScene(this.CurrentScene) && obj != null)
          this.SendSelection(obj);
        else {
          throw new NotImplementedException();
        }
          break;
      default:
          break;
    }
    // TODO: Check refresh is needed
    this.OnContentRefreshed?.Invoke(this, EventArgs.Empty);
  }

  public void SendSelection(object selection) {
    var type = selection.GetType();
    if (!type.IsGenericType || 
        type.GetGenericTypeDefinition() != typeof(List<>)) {
      throw (new ApplicationException($"selection is not list: {selection}"));
    }
    if(selection is ICollection list) {
      var message = new WindowMessage<ICollection> {
        Type = WindowMessage<ICollection>.MessageType.Selection,
        Value = list
      };
    }
    else
      Console.Error.WriteLine($"selection is not list: {selection}");
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
    RefreshWindow,
    SendMessage,
    CloseWindow,
  }

  public class WindowMessage<T>: EventArgs {

    public required MessageType Type { get; init; }
    public required T Value { get; init; }

    public enum MessageType {
      Selection,
      Input,
    }
  }
}

