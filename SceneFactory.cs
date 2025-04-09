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

  private static SceneType GetSceneType(Scene.ISceneName sceneName) {
    return (sceneName switch {
        ImageSN _ => SceneType.Image,
        SelectSN _ => SceneType.Select,
        PresentingSN scene when scene.Name == PresentingSN.MainScene =>
        SceneType.Main,
        AssistanceSN scene when scene.Name == AssistanceSN.InputScene => SceneType.Input,
        _ => throw new ArgumentException($"no scene type found for: {sceneName}")
        });

  }

  public Scene Build(Scene.ISceneName sceneName) {
    var sceneType = SceneFactory.GetSceneType(sceneName);
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

  public Scene Build(Scene.ISceneName sceneName, Dictionary<string, object> data) {
    var sceneType = SceneFactory.GetSceneType(sceneName);
    switch (sceneType) {
      case SceneType.Image:
        return (this.CreateImageScene(sceneName, data));
      case SceneType.Main:
        return (this.CreateMainScene(sceneName, data));
      default: 
        throw (new NotImplementedException());
    }
  } 

  private Scene CreateMainScene(Scene.ISceneName name, Dictionary<string, object> data) {
    return (new MainScene(name, data));
  }

  private Scene CreateSelectScene(Scene.ISceneName scene) {

    switch (scene) {
      case { Name: SelectSN.SelectCharacterScene }:
                   var characterSelection = GetSelections(scene);
                   return (new SelectScene<string>(
                         scene,
                         new Dictionary<string, object> {
                         {"prompt", 
                         """
                         핵전쟁이 일어났습니다!!!
                         모두를 데려갈 수는 없습니다 테드
                         당신은 함께 대피할 가족을 선택해야 합니다
                         """},
                         {"maximumSelect", 2 },
                         {"selections",characterSelection },
                         {"nextSceneName", GetFullName(SceneFactory.SelectSN.SelectItem)}
                         })
                       );
      case { Name: SelectSN.SelectItemScene }:
                   var itemSelection = GetSelections(scene);
                   return (new SelectScene<string>(
                         scene,
                         new Dictionary<string, object> {
                         {"prompt", 
                         """
                         멍하니 서 있지 마세요, 테드! 이제 60초밖에 안 남았으니까요!
                         꼭 필요한 아이템을 고르세요
                         """},
                         {"maximumSelect", 8},
                         {"selections",itemSelection },
                         })
                       );
      default: 
                   throw (new ArgumentException($"invalid name for SelectScene: {scene.Name}"));
    }
  }

  private List<(InputKey, object)> GetSelections(Scene.ISceneName scene) {

    switch (scene.Name) {
      case SelectSN.SelectCharacterScene:
        return (new List<(InputKey, object)> {
            (InputKey.D1, CharacterFactory.CharacterName.Get(Character.Playable.Dolores)),
            (InputKey.D2, CharacterFactory.CharacterName.Get(Character.Playable.MaryJane)),
            (InputKey.D3, CharacterFactory.CharacterName.Get(Character.Playable.Timmy)),
            (InputKey.D4, CharacterFactory.CharacterName.Get(Character.Playable.Pancake)),
            });
      case SelectSN.SelectItemScene:
        return (new List<(InputKey, object)> {
            (InputKey.D1, Item.ItemName.GasMask.Value),
            (InputKey.D3, Item.ItemName.Shovel.Value),
            (InputKey.D4, Item.ItemName.Axe.Value),
            (InputKey.D5, Item.ItemName.Soup.Value),
            (InputKey.D6, Item.ItemName.Soup.Value),
            (InputKey.D7, Item.ItemName.Soup.Value),
            (InputKey.D8, Item.ItemName.Soup.Value),
            (InputKey.D9, Item.ItemName.Soup.Value),
            (InputKey.A, Item.ItemName.Water.Value),
            (InputKey.S, Item.ItemName.Water.Value),
            (InputKey.D, Item.ItemName.Water.Value),
            (InputKey.F, Item.ItemName.Water.Value),
            (InputKey.G, Item.ItemName.Water.Value),
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
                         { "textBelow", "아무키나 눌러주세요" },
                         }));

      default: 
                   throw (new ArgumentException($"invalid ImageSN: {imageScene}"));
    }
  }

  private ImageScene CreateImageScene(Scene.ISceneName imageScene, Dictionary<string, object> data) {
    switch (imageScene) {
      case { Name: ImageSN.CharacterIntroScene }:
        string characterName = (string)data["characterName"]; 
        Character.Playable character = CharacterFactory.CharacterName.GetPlayable(characterName);
        string image = Assets.CharacterImages[characterName];
        return (new ImageScene(
              imageScene,
              new Dictionary<string, string> {
              { "name", imageScene.Name },
              { "characterName", characterName },
              { "image", image },
              { "textAbove", string.Format($"캐릭터 소개\n\t이름: {characterName}")},
              { "textBelow", 
              CharacterFactory.CharacterDescription.Get(character) + "\n계속하려면 아무키나 누르세요"}
              })
            );
      default:
        throw (new ArgumentException($"invalid ImageSN: {imageScene}"));
    }
  }

  private InputScene CreateInputScene(Scene.ISceneName inputScene) {
    return (new InputScene(inputScene));
  }

  private string GetFullName(Scene.ISceneName sceneName) {
    return (sceneName.GetType().FullName + ":" + sceneName.Name);
  }

  public struct ImageSN: Scene.ISceneName {
    public string Name { get; set; }

    public const string TitleScene = "Title Scene"; 
    public static readonly ImageSN Title = new () { Name = ImageSN.TitleScene};
    public const string CharacterIntroScene = "Character Intro Scene";
    public static readonly ImageSN CharacterIntro = new () { Name = ImageSN.CharacterIntroScene};
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
    public static readonly AssistanceSN Input = new() { Name = AssistanceSN.InputScene };
  }

  public struct PresentingSN : Scene.ISceneName {
    public string Name { get; set; }
    public const string MainScene = "Main Scene";
    public static readonly PresentingSN Main = new() { Name = PresentingSN.MainScene };
  }

  public enum SceneType {
    Select,
    Image,
    Input,
    Main,
  }
}
