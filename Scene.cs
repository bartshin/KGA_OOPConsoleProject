namespace ConsoleProject;

abstract class Scene {

  public string Name { get; init; }
  public string? NextSceneName { get; init; }
  public SceneState State { get; protected set; }
  public List<InputKey> AcceptKeys { get; protected set; } = new();

  public abstract (Window.WindowCommand, object?) ReceiveInput(InputKey input);
  public abstract void OnRenderFinished();
  public abstract RenderContent GetRenderContent();

  protected Scene(string name, SceneState state) {
    this.Name = name;
    this.State = state;
  }

  public enum SceneState {
    WaitingInput,
    Paused,
    Rendering,
  }
}

sealed class SceneFactory {

  private static SceneFactory instance;
  public static SceneFactory Shared {
    get { 
      if (SceneFactory.instance == null)
        SceneFactory.instance = new SceneFactory();
      return (SceneFactory.instance);
    }
  }
  private SceneFactory() { }

  public Scene Build(SceneType sceneType, ISceneName sceneName) {
    switch (sceneType) {
      case SceneType.Select:
        return (this.CreateSelectScene(sceneName));
      case SceneType.Image:
        return (this.CreateImageScene(sceneName));
      default: 
        throw (new NotImplementedException());
    }
  } 

  private Scene CreateSelectScene(ISceneName scene) {

    switch (scene) {
      case { Name: SelectSceneName.SelectItemScene }:
        var selections = GetSelections<string>(scene);
        return (new SelectScene<string>(
              new Dictionary<string, object> {
              {"name", "SelectItemScene"},
              {"prompt", 
              """
              멍하니 서 있지 마세요, 테드! 이제 60초밖에 안 남았으니까요!
              꼭 필요한 아이템을 고르세요
              """},
              {"maximumSelect", 3},
              {"selections", selections},
              {"nextSceneName", ""}
              })
            );
      default: 
        throw (new ArgumentException($"invalid name for SelectScene: {scene.Name}"));
    }
  }

  private IList<(InputKey, T)> GetSelections<T>(ISceneName scene) {

    switch (scene.Name) {
        
        default:
          throw (new ArgumentException($"no selection for scene: ${scene.Name}"));
    }
  }

  private ImageScene CreateImageScene(ISceneName imageScene) {
    switch (imageScene) {
      case { Name: ImageSceneName.TitleScene }:
        return (new ImageScene(
              new Dictionary<string, string> {
              { "name", imageScene.Name },
              { "image", Assets.TitleImage },
              { "nextSceneName", SelectSceneName.SelectItemScene }
              }));
      default: 
        throw (new ArgumentException($"invalid ImageSceneName: {imageScene}"));
    }
  }

  public interface ISceneName {
    public string Name { get; }
  }

  public readonly struct ImageSceneName: ISceneName {
    public readonly string Name => this.name;
    private readonly string name;

    public const string TitleScene = "Title Scene"; 
    public static readonly ImageSceneName Title = new(TitleScene);

    private ImageSceneName(string name) => this.name = name;
  }

  public readonly struct SelectSceneName: ISceneName {
    public readonly string Name => this.name;
    private readonly string name;
    public const string SelectItemScene = "Select Item Scene"; 
    public static readonly SelectSceneName SelectItem = new(SelectItemScene);
    public const string SelectCharacterScene = "Selec Character Scene"; 
    public static readonly SelectSceneName SelectCharacter = new(SelectCharacterScene); 

    private SelectSceneName(string name) => this.name = name;
  }

  public enum SceneType {
    Select,
    Image,
  }
}
