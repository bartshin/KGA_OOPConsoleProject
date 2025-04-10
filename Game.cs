using System.Text;

namespace ConsoleProject;

class Game {
  private int totalDays = 0;
  private (double food, double water) TodayQuota = (0.0, 0.0);
  private Character? todayFarming = null;
  public string YesterDayFarmingResult;
  public List<string> TodayDeadChracter = new();
  public bool IsEnded { get; private set; } = false;
  public bool IsSuccess { get; private set; } = false;
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
    }
  }
  readonly public GameConfig Config = new GameConfig();
  private GameData data = new GameData();
  public int NumberOfSurviver => this.data.Characters.FindAll(c => c.IsAlive).Count;

  public Game(SceneProgressor scenes) {
    this.Scenes = scenes;
    this.Scenes.GetGameStatus = this.ServeGameStatus;
    this.Scenes.ModifyGameStatus = this.HandleGameModification;
    this.SetMainWindow(this.Scenes.CurrentScene);
  }

  public void HandleGameModification(GameStatus status) {
    foreach (var section in status.Sections ) {
       switch (section) {
         case GameStatus.Section.TodayQuota:
           if (status.TryGet<(double, double)>(section, out var quota)) {
            this.TodayQuota = quota;
           }
         break;
         case GameStatus.Section.TotalDay:
         if (status.TryGet<int>(section, out var day) &&
             day == this.totalDays + 1) 
            this.GoToNextDay(); 
         else 
           throw new ApplicationException("invalid modify totalDays");
         break;
         default: throw new NotImplementedException();
       } 
    } 
  }

  public void ServeGameStatus(GameStatus status) {
    foreach (var section in status.Sections) {
      switch (section) {
        case GameStatus.Section.TotalDay:
          status.Add(section, this.totalDays);
          break;
        case GameStatus.Section.RemainingWater:
          status.Add(section, this.data.Inven.GetTotalWater());
          break;
        case GameStatus.Section.RemainingSoup:
          status.Add(section, this.data.Inven.GetTotalSoup());
          break;
        case GameStatus.Section.CharacterStatus:
          status.Add(section, this.data.GetChracterStatus());
          break;
        case GameStatus.Section.Items:
          status.Add(section, this.data.GetItems());
          break;
        case GameStatus.Section.TodayQuota:
          status.Add(section, this.TodayQuota);
          break;
        case GameStatus.Section.MaxQuota:
          status.Add(section, (this.data.Inven.GetTotalSoup(), 
                this.data.Inven.GetTotalWater()));
          break;
        case GameStatus.Section.TodayDead:
          status.Add(section, this.TodayDeadChracter);
          this.TodayDeadChracter = new(); 
          break;
        case GameStatus.Section.TodayFarming:
          status.Add(section, this.todayFarming?.Name ?? "없음");
          break;
        case GameStatus.Section.YesterDayFarming:
          status.Add(section, this.YesterDayFarmingResult);
          break;
        case GameStatus.Section.IsAnyCharacterFarming:
          status.Add(section, this.data.IsAnyCharacterFarming());
          break;
      } 
    }
  }

  private Scene SelectScene(Scene scene, Window window) {
    if (window.Type != Window.WindowType.Main)
      throw (new NotImplementedException());
    if (scene is TableScene tableScene ||
       scene is SelectScene<string> selectScene ) 
      this.BottomWindow = null;
    this.Scenes.ProgressToNextScene(scene);
    return (scene);
  }

  private Scene GoToNextScene(Window window, Scene? scene = null) {
    switch (window.Type) {
      case Window.WindowType.Main:
        if (window.CurrentScene is ISelectScene selectScene &&
            window.PopupScene == null) 
          this.GetSelection(selectScene);
        var nextScene = scene ?? this.SelectSceneFrom(
            this.Scenes.GetNextScenes(), window);
        if ((nextScene is MainScene  && window.PopupScene == null )|| 
            SelectScene<object>.IsSelectScene(nextScene))
          this.SetBottomWindow(nextScene);
        else
          this.BottomWindow = null;
        this.Scenes.ProgressToNextScene(nextScene);
        if (nextScene is MainScene) 
          this.BottomWindow?.RefreshWindow();
        return (nextScene);
      default: throw new NotImplementedException();
    }
  }

  private void GetSelection(ISelectScene selectScene) {
    var selection = selectScene.CurrentSelection;
    Scene scene = (Scene)selectScene;
    switch (scene.SceneName) {
      case { Name: SceneFactory.SelectSN.SelectCharacterScene }:
       this.PickCharacters(CharacterFactory.Shared.Create(Character.Playable.Ted));
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
      case { Name: SceneFactory.SelectSN.SelectFarmerScene }:
        var farmer = (string)selection.FirstOrDefault();
        this.todayFarming = this.data.Characters.Find(c => c.Name == farmer); 
        break;
      default: throw new NotImplementedException();
    }
  }

  private bool CheckGameEnded() {
    if (this.IsEnded)
      return (true);
    var winningDays = Config.NumberOfTotalDays;
    if (this.data.Inven.IsContain(Item.ItemName.Radio)) 
      winningDays -= 5;
    if (this.totalDays >= winningDays) {
      this.IsEnded = true;
      this.IsSuccess = true;
      return (true);
    }
    return (false);
  }

  private string GetWinnngMessage() {
    StringBuilder builder = new();
    if (this.data.Inven.IsContain(Item.ItemName.Radio)) 
      builder.AppendLine("라디오가 있어서 빠른 구출을 도움이 되었습니다"); 
    return (builder.ToString());
  }

  private void RenderEnding() {
    this.BottomWindow = null;
    foreach (var window in this.Windows)
      Renderer.Shared.RemoveWindow(window);
    ImageScene endingScene = (ImageScene) SceneFactory.Shared.Build(
        SceneFactory.ImageSN.Ending,
          new() {
          { "isSuccess", this.IsSuccess },
          { "numberOfSurviver", this.NumberOfSurviver },
          { "winningMessage", this.GetWinnngMessage() }
          });
    Renderer.Shared.OnRenderFinished += ((_, _) => {
        Console.CursorVisible = true;
        Environment.Exit(0);
        });
    this.SetMainWindow(endingScene);
  }

  private void GoToNextDay() {
    if (this.CheckGameEnded()) {
      this.RenderEnding();
      return;
    }
    this.YesterDayFarmingResult = this.data.GetFarmingResult();
    if (this.todayFarming != null)
      this.data.GoFarming(this.todayFarming);
    bool isSurvived = this.data.GoToNextDay(this.TodayQuota);
    if (!isSurvived) {
      foreach (Character character in this.data.Characters.FindAll(c => !c.IsAlive)) {
         this.TodayDeadChracter.Add(character.Name); 
      }
      this.data.RemoveDeadCharacter();
      if (this.data.Characters.FindIndex(character => character.IsAlive) == -1)  {
        this.IsEnded = true;
      }
    }
    this.data.TakeRest();
    this.totalDays += 1;
    this.TodayQuota = (0.0, 0.0);
    this.todayFarming = null;
  }

  private void SetMainWindow(Scene scene) {
    var mainWindow = new Window(
        scene, Window.WindowType.Main);
    mainWindow.GoToNextScene = (scene) => 
      this.GoToNextScene(mainWindow, scene);
    mainWindow.GetNextScenes = this.Scenes.GetNextScenes;
    mainWindow.BackToPreviuosSceneFrom = (scene) => {
      if (scene == mainWindow.PopupScene)
        this.SetBottomWindow(mainWindow.CurrentScene);
      this.Scenes.BackToPreviousSceneFrom(scene);
    };
    this.Windows.Add(mainWindow);
    Renderer.Shared.SetWindow(mainWindow);
  }

  private void RemoveBottomWindow() {
    if (this.BottomWindow == null)
      return ;
    var window = this.BottomWindow;
    if (window.OnReceieveMessage != null)
      this.MainWindow.OnSendMessage -= window.OnReceieveMessage!;
    InputForwarder.Shared.FocusedWindow = this.MainWindow;
    this.Windows.Remove(window);
    Renderer.Shared.RemoveWindow(window);
  }

  private void SetBottomWindow(Scene topScene) {
    if (topScene is MainScene mainScene) {
      var navigationScene = (NavigationScene)SceneFactory.Shared.Build(SceneFactory.AssistanceSN.Navigation);
      navigationScene.Menu = mainScene.Menu;
      navigationScene.OnSelect = (selected) => {
        InputForwarder.Shared.FocusedWindow = this.MainWindow;
      };
      if (this.BottomWindow == null) 
        this.CreateBottomWindow(navigationScene);
      else  
        this.BottomWindow.ChangeScene(navigationScene);
      InputForwarder.Shared.FocusedWindow = this.BottomWindow!;
    }
    else if (SelectScene<object>.IsSelectScene(topScene)) {
      var selectScene = (ISelectScene)topScene;
      InputScene scene;
      if (this.BottomWindow?.CurrentScene is InputScene inputScene) 
        scene = inputScene;
      else
        scene = (InputScene)SceneFactory.Shared.Build(
            SceneFactory.AssistanceSN.Input);
      scene.AllSelection = selectScene.AllSelections;
      scene.MaxSelection = selectScene.MaxSelection;
      bool isBottomWindowExist = this.BottomWindow != null;
      if (!isBottomWindowExist) {
        this.CreateBottomWindow(scene);
      }
      if (isBottomWindowExist) {
        this.BottomWindow!.ChangeScene(scene);
      }
    }
  }

  private void CreateBottomWindow(Scene scene) {
    var window = new Window(scene, Window.WindowType.Bottom);
    this.MainWindow.OnSendMessage += window.OnReceieveMessage;
    window.OnSendMessage += this.MainWindow.OnReceieveMessage;
    window.VertialEdge = '|';
    window.HorizontalEdge = '-';
    this.Windows.Add(window);
    Renderer.Shared.SetWindow(window);
    this.BottomWindow = window;
  }

  public Scene SelectSceneFrom(List<Scene> scenes, Window window) {
    if (window == this.MainWindow &&
        window.CurrentScene.SceneName.Name== SceneFactory.PresentingSN.MainScene) {
      var mainIndex = scenes.FindIndex(scene => scene.SceneName.Name == SceneFactory.PresentingSN.MainScene );
      if (mainIndex != -1)
        return (scenes[mainIndex]);
    }
    // TODO: select next scene
    return (scenes[0]);
  }

  readonly public struct GameConfig {
    public readonly int NumberOfTotalDays = 20;

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
    private Random random = new();

    public GameData() {
      this.Characters = new List<Character>();
      this.Inven = new Inventory();
    }

    public void AddCharacter(Character character) {
      this.Characters.Add(character); 
    }

    public List<(string, string)> GetChracterStatus() {
      List<(string, string)> list = new();
      foreach (var character in this.Characters) {
        if (!character.IsAlive)
          continue;
        if (character.IsFarming) {
          list.Add((character.Name, Character.FarmingText));
          continue;
        }
        int count = 0;
        foreach (var status in character.CurrentStatus) {
          list.Add((character.Name, GameText.GetCharacterComment(status))); 
          ++count;
        }
        if (count == 0) 
          list.Add((character.Name, GameText.GetCharacterComment(null)));
      }
      return (list);
    }

    public void GoFarming(Character character) {
      character.IsFarming = true;
      if (this.Inven.IsContain(Item.ItemName.FirstAidKit)) 
        character.DoWork(Item.ItemName.FirstAidKit);
      else if(this.Inven.IsContain(Item.ItemName.GasMask)) 
        character.DoWork(Item.ItemName.GasMask);
      else
        character.DoWork();
    }

    public string GetFarmingResult() {
      var farmer = this.Characters.Find(c => c.IsFarming);
      if (farmer == null)
        return (null);
      farmer.IsFarming = false;
      var chance = 70;
      if (this.Inven.IsContain(Item.ItemName.Axe))
        chance -= 20;
      bool food = this.random.Next(0, 100) > chance;
      bool water = this.random.Next(0, 100) > chance;
      if (food)
        this.Inven.Add(Item.ItemName.Soup);
      if (water)
        this.Inven.Add(Item.ItemName.Water);
      StringBuilder builder = new();
      if (food && water)
        builder.AppendLine(string.Format($"{farmer.Name}은 물과 수프를 모두 발견했다!"));
      else if (food)
        builder.AppendLine(string.Format($"{farmer.Name}은 수프를 발견했다!"));
      else if (water)
        builder.AppendLine(string.Format($"{farmer.Name}은 물을 발견했다!"));
      else
        builder.AppendLine(string.Format($"{farmer.Name}은 아무 소득도 없이 돌아왔다"));
      return (builder.ToString());
    }
    
    public void TakeRest() {
      foreach (var character in this.Characters) {
         if (!character.IsFarming && character.IsAlive) {
            character.TakeRest();
         }
      }
    }

    public List<(string, string)> GetItems() {
      List<(string, string)> list = new();
      foreach (var item in this.Inven) {
        string description = string.Format($"{item.Description}");
        if (item is ConsumableItem consumableItem) {
          description += string.Format($" 보유량:{double.Round(consumableItem.Quantity, 1)}".Replace('.', ',')); 
        }
        list.Add((item.Name, description)); 
      }
      return (list);
    }

    public bool GoToNextDay((double, double)quata) {
      double food = quata.Item1 / this.Characters.Count;
      double water = quata.Item2 / this.Characters.Count;
      foreach (Character character in this.Characters) {
        character.Consume(food, water); 
      } 
      this.Inven.ModifyAmount(Item.ItemName.Soup, - quata.Item1); 
      this.Inven.ModifyAmount(Item.ItemName.Water,- quata.Item2);
      return (this.Characters.FindIndex(
            character => !character.IsAlive) == -1);
    }

    public void RemoveDeadCharacter() {
      for (int i = this.Characters.Count - 1; i > 0; --i) {
        if (!this.Characters[i].IsAlive) 
          this.Characters.RemoveAt(i);
      }
    }
    public bool IsAnyCharacterFarming() {
      return (this.Characters.FindIndex(c => c.IsFarming) != -1);
    }
  }
}

