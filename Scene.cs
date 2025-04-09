using System;
using System.Text.RegularExpressions;

namespace ConsoleProject;

abstract class Scene: IEquatable<Scene> {

  protected const int MarginVertical = 2;
  private const string Delimiters = "(?<=[.!\n])";
  public ISceneName SceneName { get; init; }
  public string? NextSceneName { get; init; }
  public SceneState State { get; protected set; }
  public List<InputKey> AcceptKeys { get; protected set; } = new();
  public Action<GameStatus> GetGameStatus;

  public abstract (Window.WindowCommand, object?) ReceiveInput(InputKey input);
  public virtual (Window.WindowCommand, object?) ReceiveMessage(Window.WindowMessage message) {
    return (Window.WindowCommand.None, null);
  }
  public virtual void OnRenderFinished() {}
  public abstract RenderContent GetRenderContent();

  protected Scene(ISceneName name, SceneState state) {
    this.SceneName = name;
    this.State = state;
  }

  protected string[] SplitText(string text) {
    return (Regex.Split(text, Scene.Delimiters));
  }
  protected void AddMargin(List<(string, RenderColor)> content, int value= Scene.MarginVertical) {
    for (int i = 0; i < Scene.MarginVertical; i++) {
      content.Add(("\n", RenderColor.White));
    }
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
