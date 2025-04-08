using System;
namespace ConsoleProject;

sealed class SceneProgressor {
  private Tree<Scene> sceneTree;
  private Tree<Scene>.Node currentNode;
  public Scene CurrentScene => this.currentNode.Value;

  public List<Scene> GetNextScenes() {
    return this.currentNode.Children.ConvertAll<Scene>(node => node.Value);
  }

  public void ProgressToNextScene(Scene next) {
    var node = this.currentNode.GetChildBy(next);
    if (node == null) {
      throw (new ApplicationException($"${next.SceneName} is not child of current scene"));
    }
    this.currentNode = node;
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

  public void OnEnterScene() {
    var nextScene = this.CurrentScene.NextSceneName == null ?
      this.CreateNextScene(this.CurrentScene.SceneName):
      this.CreateNextScene();
    this.AddNextScene(nextScene);
  }

  private Scene CreateNextScene() {
    var fullName = this.CurrentScene.NextSceneName.Split(':');
    var sceneType = Type.GetType(fullName[0].Trim()); 
    Scene.ISceneName sceneName = (Scene.ISceneName)Activator.CreateInstance(sceneType);
    sceneName.Name = fullName[1].Trim(); 
    return (SceneFactory.Shared.Build(sceneName));
  }

  private Scene CreateNextScene(Scene.ISceneName current) {
    switch (current) {
      case SceneFactory.ImageSN imageScene when imageScene.Name == SceneFactory.ImageSN.TitleScene:
        return (SceneFactory.Shared.Build(
              SceneFactory.ImageSN.CharacterIntro,
              new Dictionary<string, object>() {
              { "characterName", CharacterFactory.CharacterName.Ted }
              }));
      case SceneFactory.ImageSN imageScene when imageScene.Name == SceneFactory.ImageSN.CharacterIntroScene  :
        ImageScene currentScene = (ImageScene)this.CurrentScene;
        string characterName = currentScene.Data["characterName"];
        var character = CharacterFactory.CharacterName.GetPlayable(characterName);
        var nextCharacter = Character.GetNext(character);
        if (nextCharacter != null) {
          return (SceneFactory.Shared.Build(
                SceneFactory.ImageSN.CharacterIntro,
                new Dictionary<string, object>() {
              { "characterName", CharacterFactory.CharacterName.Get(nextCharacter.Value) }
                }
                ));
        }
        else {
          return (SceneFactory.Shared.Build(
                SceneFactory.SelectSN.SelectCharacter
                ));
        }
        throw new NotImplementedException();
      default: 
        throw new NotImplementedException();
    }
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
