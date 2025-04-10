using System;
using System.Text;
namespace ConsoleProject;

sealed class SceneProgressor {
  private Tree<Scene> sceneTree;
  private Tree<Scene>.Node currentNode;
  public Scene CurrentScene => this.currentNode.Value;
  public Action<GameStatus> GetGameStatus;
  public Action<GameStatus> ModifyGameStatus;

  public List<Scene> GetNextScenes() {
    if (this.currentNode.Children.Count == 0)
      return ([this.currentNode.Parent.Value]);
    return this.currentNode.Children.ConvertAll<Scene>(node => node.Value);
  }

  public void BackToPreviousSceneFrom(Scene current) {
    if (this.currentNode.Parent == null)
      throw new ApplicationException($"no parent scene: {current.SceneName.Name }");
    if (this.CurrentScene != current) 
      throw new ApplicationException($"BackToPreviousSceneFrom different scene: {current.SceneName.Name }");
    this.currentNode = this.currentNode.Parent;
  }

  public void ProgressToNextScene(Scene next) {
    var node = this.currentNode.GetChildBy(next);
    if (node == null) {
      if (next == this.currentNode.Parent?.Value) {
        this.currentNode = this.currentNode.Parent;
        this.OnEnterScene();
        return;
      }
      throw (new ApplicationException($"${next.SceneName} is not child of current scene"));
    }
    this.currentNode = node;
    if (next is MainScene) {
      sceneTree.ChangeRoot(node);
      this.ProgressToNextDay(); 
    }
    this.OnEnterScene();
  }

  public void ReLocateToScene(Scene scene) {
    var root = new Tree<Scene>.Node(scene);
    this.sceneTree.ChangeRoot(root);
    this.currentNode = root;
  }

  public void AddNextScene(params List<Scene> scenes) {
    if (scenes.Count == 1) 
      this.sceneTree.CreateNodeBelow(this.currentNode, scenes[0]);
    else
      this.sceneTree.CreateNodesBelow(this.currentNode, scenes);
  }

  public SceneProgressor(Scene root) {
    this.currentNode = new Tree<Scene>.Node(root);
    this.sceneTree = new(currentNode);
    this.OnEnterScene();
  }

  private void ProgressToNextDay() {
    GameStatus status = new ([GameStatus.Section.TotalDay]);
    this.GetGameStatus(status);
    status.TryGet<int>(GameStatus.Section.TotalDay, out int day);
    status.Add(GameStatus.Section.TotalDay, day + 1);
    this.ModifyGameStatus(status);
  }

  private void OnEnterScene() {
    var nextScene = this.CurrentScene.NextSceneName == null ?
      this.CreateNextScene(this.CurrentScene.SceneName):
      this.CreateNextScene();
    this.AddNextScene(nextScene);
  }

  private List<Scene> CreateNextScene() {
    var fullName = this.CurrentScene.NextSceneName.Split(':');
    var sceneType = Type.GetType(fullName[0].Trim()); 
    Scene.ISceneName sceneName = (Scene.ISceneName)Activator.CreateInstance(sceneType);
    sceneName.Name = fullName[1].Trim(); 
    return ([SceneFactory.Shared.Build(sceneName)]);
  }

  private List<Scene> CreateNextScene(Scene.ISceneName current) {
    switch (current) {
      case SceneFactory.ImageSN imageScene when imageScene.Name == SceneFactory.ImageSN.TitleScene:
        return ([SceneFactory.Shared.Build(
              SceneFactory.ImageSN.CharacterIntro,
              new Dictionary<string, object>() {
              { "characterName", CharacterFactory.CharacterName.Ted }
              })]);
      case SceneFactory.ImageSN imageScene when imageScene.Name == SceneFactory.ImageSN.CharacterIntroScene  :
        ImageScene currentScene = (ImageScene)this.CurrentScene;
        string characterName = currentScene.Data["characterName"];
        var character = CharacterFactory.CharacterName.GetPlayable(characterName);
        var nextCharacter = Character.GetNext(character);
        if (nextCharacter != null) {
          return ([SceneFactory.Shared.Build(
                SceneFactory.ImageSN.CharacterIntro,
                new Dictionary<string, object>() {
              { "characterName", CharacterFactory.CharacterName.Get(nextCharacter.Value) }
                }
                )]);
        }
        else {
          return ([SceneFactory.Shared.Build(
                SceneFactory.SelectSN.SelectCharacter
                )]);
        }
      case SceneFactory.SelectSN selectScene when selectScene.Name == SceneFactory.SelectSN.SelectItemScene: 
        var mainScene = SceneFactory.PresentingSN.Main;
        Dictionary<string, object> data = new();
        this.FillMainSceneTexts(data);
        var scene = (SceneFactory.Shared.Build(mainScene, data));
        scene.GetGameStatus = this.GetGameStatus;
        return ([scene]);
      case SceneFactory.PresentingSN presentingScene when presentingScene.Name == SceneFactory.PresentingSN.MainScene:
        return (this.GetMainSceneChildren());
      default:
        return ([]);
    }
  }

  private List<Scene> GetMainSceneChildren() {
    List<Scene> scenes = new (); 
    TableScene inventory = (TableScene)SceneFactory.Shared.Build(
        SceneFactory.PresentingSN.Inventory
        );
    scenes.Add(inventory);
    TableScene characterStatus = (TableScene)SceneFactory.Shared.Build(
        SceneFactory.PresentingSN.CharacterStatus
        );
    scenes.Add(characterStatus);
    TableControlScene quata = (TableControlScene)SceneFactory.Shared.Build(SceneFactory.PresentingSN.Quata);
    quata.ModifyGameStatus = this.ModifyGameStatus;
    scenes.Add(quata);
    var characters = this.GetCharacterStatus().FindAll(
          status => status.Item2 != Character.FarmingText);
    if (characters.Count > 1) {
      var farmer = (SelectScene<string>)SceneFactory.Shared.Build(
          SceneFactory.SelectSN.SelectFarmer,
          new () {{"characters", characters }});
      //farmer.ModifyGameStatus = this.ModifyGameStatus;
      scenes.Add(farmer);
    }
    var mainScene = SceneFactory.PresentingSN.Main;
    Dictionary<string, object> data = new();
    this.FillMainSceneTexts(data);
    scenes.Add(SceneFactory.Shared.Build(mainScene, data));
    foreach (var scene in scenes) 
      scene.GetGameStatus = this.GetGameStatus;
    return (scenes);
  }

  private void FillMainSceneTexts(Dictionary<string, object> dict) {
    StringBuilder builder = new();
    GameStatus status = new (GameStatus.Section.TotalDay);
    this.GetGameStatus(status);
    if (status.TryGet<int>(GameStatus.Section.TotalDay, out int day))
      builder.Append(GameText.AddDayText(day + 1)); 
    dict.Add(MainScene.MainText, builder.ToString());
  }

  private List<(string, string)> GetCharacterStatus() {
    GameStatus status = new([ GameStatus.Section.CharacterStatus ]);
    this.GetGameStatus(status);
    if (status.TryGet<List<(string, string)>>(
        GameStatus.Section.CharacterStatus, 
        out var characterStatus
        )) {
      return (characterStatus);
    }
    throw (new ApplicationException("can not get character status"));
  }
}

sealed class Tree<T> where T: IEquatable<T> {

  public Node Root { get; private set; }

  public Tree(Node root) {
    this.Root = root;
  }

  public void ChangeRoot(Node node) {
    node.Parent = null;
    this.Root = node;
  }

  public Node CreateNodeBelow(Node parent, T value) {
    var node = new Node(value, parent); 
    parent.AddChild(node);
    return (node);
  }

  public List<Node> CreateNodesBelow(Node parent, params List<T> values) {
    var nodes = values.ConvertAll<Node>(value => new Node(value, parent));
    parent.AddChild(nodes);
    return (nodes);
  }

  public void ConnectNode(T from, T to) {
    if (from.Equals(to))
      throw (new ArgumentException("connect same node is not valid"));
  }

  public class Node {
    public T Value;
    public Node? Parent;
    public List<Node> Children { get; }

    public Node(T value): this(value, null) {}
    
    public Node(T value, Node? parent, params IEnumerable<Node> children) {
      this.Value = value;
      this.Parent = parent;
      this.Children = new ();
      this.AddChild(children);
    }

    public void AddChild(params IEnumerable<Node> children) {
      foreach (Node child in children) {
        this.Children.Add(child); 
      }
    }

    public void RemoveChild(params IEnumerable<Node> children) {
      foreach (Node child in children) {
        this.Children.Remove(child);
      }
    }

    public Node? GetChildBy(T value) {
      return (this.Children.Find(node => node.Value.Equals(value)));
    }
  }
}
