namespace ConsoleProject;

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

  public Scene Build(Scene.ISceneName sceneName) {
    var sceneType = this.GetSceneType(sceneName);
    switch (sceneType) {
      case SceneType.Select:
        return (this.CreateSelectScene(sceneName));
      case SceneType.Image:
        return (this.CreateImageScene(sceneName));
      case SceneType.Input:
        return (this.CreateInputScene(sceneName));
      default: 
        throw (new NotImplementedException());
    }
  } 

  private SceneType GetSceneType(Scene.ISceneName sceneName) {
    return (sceneName switch {
        ImageSN _ => SceneType.Image,
        SelectSN _ => SceneType.Select,
        AssistanceSN scene when scene.Name == AssistanceSN.InputScene => SceneType.Input,
        _ => throw new ArgumentException($"no scene type found for: {sceneName}")
        });

  }

  private Scene CreateSelectScene(Scene.ISceneName scene) {

    switch (scene) {
      case { Name: SelectSN.SelectItemScene }:
                   var selections = GetSelections(scene);
                   return (new SelectScene<string>(
                         scene,
                         new Dictionary<string, object> {
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

  private List<(InputKey, object)> GetSelections(Scene.ISceneName scene) {

    switch (scene.Name) {
      case SelectSN.SelectItemScene:
        return (new List<(InputKey, object)> {
            (InputKey.D1, "방독면"),
            (InputKey.D2, "삽"),
            (InputKey.D3, "구급상자"),
            (InputKey.D4, "도끼"),
            (InputKey.D5, "식량"),
            (InputKey.D6, "물"),
            });
      default:
        throw (new ArgumentException($"no selection for scene: {scene.Name}"));
    }
  }

  private ImageScene CreateImageScene(Scene.ISceneName imageScene) {
    switch (imageScene) {
      case { Name: ImageSN.TitleScene }:
                   return (new ImageScene(
                         imageScene,
                         new Dictionary<string, string> {
                         { "name", imageScene.Name },
                         { "image", Assets.TitleImage },
                         { "nextSceneName", Scene.GetFullSceneName(SelectSN.SelectItem)}
                         }));
      default: 
                   throw (new ArgumentException($"invalid ImageSN: {imageScene}"));
    }
  }

  private InputScene CreateInputScene(Scene.ISceneName inputScene) {
    return (new InputScene(inputScene));
  }

  public struct ImageSN: Scene.ISceneName {
    public string Name { get; set; }

    public const string TitleScene = "Title Scene"; 
    public static readonly ImageSN Title = new () { Name = ImageSN.TitleScene};
  }

  public struct SelectSN: Scene.ISceneName {
    public string Name { get; set; }
    public const string SelectItemScene = "Select Item Scene"; 
    public static readonly SelectSN SelectItem = new() {
      Name = SelectItemScene
    };
    public const string SelectCharacterScene = "Select Character Scene"; 
    public static readonly SelectSN SelectCharacter = new (){
      Name = SelectCharacterScene
    }; 
  }

  public struct AssistanceSN: Scene.ISceneName {
    public string Name { get; set; }
    public const string InputScene = "Input Scene";
    public static readonly AssistanceSN InputSceneName = new() { Name = AssistanceSN.InputScene };
  }

  public enum SceneType {
    Select,
    Image,
    Input,
  }
}
