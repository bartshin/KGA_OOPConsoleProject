using System.Text;

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
        PresentingSN scene when scene.Name == PresentingSN.InventoryScene=>
        SceneType.Table,
        PresentingSN scene when scene.Name == PresentingSN.CharacterStatusScene =>
        SceneType.Table,
        PresentingSN scene when scene.Name == PresentingSN.QuataScene=>
        SceneType.Table,
        AssistanceSN scene when scene.Name == AssistanceSN.InputScene => SceneType.Input,
        AssistanceSN scene when scene.Name == AssistanceSN.NavigationScene => SceneType.Navigation,
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
      case SceneType.Navigation:
        return (this.CreateNavigationScene(sceneName));
      case SceneType.Table:
        return (this.CreateTableScene(sceneName));
      default: 
        throw (new NotImplementedException($"{sceneName.Name}"));
    }
  } 

  public Scene Build(Scene.ISceneName sceneName, Dictionary<string, object> data) {
    var sceneType = SceneFactory.GetSceneType(sceneName);
    switch (sceneType) {
      case SceneType.Image:
        return (this.CreateImageScene(sceneName, data));
      case SceneType.Main:
        return (this.CreateMainScene(sceneName, data));
      case SceneType.Select:
        return (this.CreateSelectScene(sceneName, data));
      default: 
        throw (new NotImplementedException());
    }
  } 

  public string GetFullName(Scene.ISceneName sceneName) {
    return (sceneName.GetType().FullName + ":" + sceneName.Name);
  }

  private Scene CreateMainScene(Scene.ISceneName name, Dictionary<string, object> data) {
    return (new MainScene(name, data));
  }

  private Scene CreateSelectScene(Scene.ISceneName scene, Dictionary<string, object>? data = null) {

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
               }));
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
               }));
      case { Name: SelectSN.SelectFarmerScene }:
          var characters = (List<(string, string)>)data!["characters"];
          StringBuilder builder = new("""
              외부 상황이 아직 확인되지 않았다
              나가는 건 위험할 수 있다
              하지만 추가 보급품이 간절히 필요하다


              """);
          List<(InputKey, object)> selections = new();
          for (int i = 0; i < characters.Count; ++i) {
            var (name, descrition) = characters[i];
            builder.AppendLine(string.Format($"{name}: {descrition}"));
            var key = (InputKey)Enum.Parse(typeof(InputKey),
                string.Format($"D{(i + 1)}")
                );
            selections.Add((key, name));
          }
          return (new SelectScene<string>(
            scene,
            new Dictionary<string, object> {
              { "prompt", builder.ToString() },
              { "maximumSelect", 1 },
              { "selections", selections }
            }));
      default: throw (new ArgumentException($"invalid name for SelectScene: {scene.Name}"));
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
          (InputKey.D2, Item.ItemName.Radio.Value),
          (InputKey.D3, Item.ItemName.Axe.Value),
          (InputKey.D4, Item.ItemName.Soup.Value),
          (InputKey.D5, Item.ItemName.Soup.Value),
          (InputKey.D6, Item.ItemName.Soup.Value),
          (InputKey.D7, Item.ItemName.Soup.Value),
          (InputKey.D8, Item.ItemName.Soup.Value),
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
      default: throw (new ArgumentException($"invalid ImageSN: {imageScene}"));
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
              { "textAbove", string.Format($"캐릭터 소개\n    이름: {characterName}")},
              { "textBelow", 
              CharacterFactory.CharacterDescription.Get(character) + "\n계속하려면 아무키나 누르세요"}
              })
            );
      case { Name: ImageSN.EndingScene }:
        bool isSuccess = (bool)data["isSuccess"];
        string endingImage = isSuccess ? Assets.WinLogo: Assets.GameOverLogo;
        int numberOfSurvier = (int)data["numberOfSurviver"];
        string endingAboveText = isSuccess ? "드디어 이 방공호에도 구급대원들이 도착했습니다": "저런 더 이상 버티지 못했네요";
        string endingBelowText = isSuccess ? string.Format($"축하합니다 Ted 가족은 총 {numberOfSurvier}명이 살아남았습니다"):
          "핵폭발로 인해 Ted 가족은 전부 하늘나라로 가게 되었습니다";
        if (isSuccess && data.TryGetValue("winningMessage", out var winningMessage)) {
          endingAboveText = (string)winningMessage;
        }
        return (new ImageScene(SceneFactory.ImageSN.Title,
                new Dictionary<string, string> {
                  {"image", endingImage },
                  { "textAbove", endingAboveText },
                  { "textBelow", endingBelowText }
                  }));
      default:
        throw (new ArgumentException($"invalid ImageSN: {imageScene}"));
    }
  }

  private InputScene CreateInputScene(Scene.ISceneName inputScene) {
    return (new InputScene(inputScene));
  }

  private NavigationScene CreateNavigationScene(Scene.ISceneName scene) {
    return (new NavigationScene(scene));
  }

  private TableScene CreateTableScene(Scene.ISceneName scene) {
    if (scene is PresentingSN presentingSN &&
        presentingSN.Name == PresentingSN.QuataScene) {
      return (new TableControlScene(scene));
    }
    return (new TableScene(scene));
  }

  public struct ImageSN: Scene.ISceneName {
    public string Name { get; set; }

    public const string TitleScene = "Title Scene"; 
    public static readonly ImageSN Title = new () { Name = ImageSN.TitleScene};
    public const string CharacterIntroScene = "Character Intro Scene";
    public static readonly ImageSN CharacterIntro = new () { Name = ImageSN.CharacterIntroScene};
    public const string EndingScene = "EndingScene";
    public static readonly ImageSN Ending = new () { Name = ImageSN.EndingScene};

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
    public const string SelectFarmerScene = "Select Farmer Scene";
    public static readonly SelectSN SelectFarmer = new() {
      Name = SelectFarmerScene
    };
  }

  public struct AssistanceSN: Scene.ISceneName {
    public string Name { get; set; }
    public const string InputScene = "Input Scene";
    public static readonly AssistanceSN Input = new() { Name = AssistanceSN.InputScene };
    public const string NavigationScene = "Navigation Scene";
    public static readonly AssistanceSN Navigation = new() { Name = AssistanceSN.NavigationScene };
  }

  public struct PresentingSN : Scene.ISceneName {
    public string Name { get; set; }
    public const string MainScene = "Main Scene";
    public static readonly PresentingSN Main = new() { Name = PresentingSN.MainScene };
    public const string CharacterStatusScene = "Character Status Scene";
    public static readonly PresentingSN CharacterStatus = new() {
      Name = CharacterStatusScene
    };
    public const string InventoryScene = "Inventory Scene";
    public static readonly PresentingSN Inventory = new() {
      Name = InventoryScene
    };
    public const string QuataScene = "Quata Scene";
    public static readonly PresentingSN Quata = new() {
      Name = QuataScene
    };
  }

  public enum SceneType {
    Select,
    Image,
    Input,
    Main,
    Navigation,
    Table,
  }
}
