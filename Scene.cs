namespace ConsoleProject;

abstract class Scene: IEquatable<Scene> {

  public ISceneName SceneName { get; init; }
  public string? NextSceneName { get; init; }
  public SceneState State { get; protected set; }
  public List<InputKey> AcceptKeys { get; protected set; } = new();

  public abstract (Window.WindowCommand, object?) ReceiveInput(InputKey input);
  public abstract void OnRenderFinished();
  public abstract RenderContent GetRenderContent();

  protected Scene(ISceneName name, SceneState state) {
    this.SceneName = name;
    this.State = state;
  }

  public enum SceneState {
    WaitingInput,
    Paused,
    Rendering,
  }

  public interface ISceneName {
    public string Name { get; set; }
  }

  public static string GetFullSceneName(ISceneName scene) => 
        scene.GetType().FullName + ":" + scene.Name;

  public override int GetHashCode() {
    return (this.SceneName.Name.GetHashCode());
  }

  public override bool Equals(object? obj) {
    if (obj == null || !(obj is Scene))
      return (false);
    return (this.Equals((Scene)obj));
  }

  public bool Equals(Scene? other)
  {
    if (other == null)
      return (this == null);
    return (this.SceneName == ((Scene)other).SceneName);
  }
}
