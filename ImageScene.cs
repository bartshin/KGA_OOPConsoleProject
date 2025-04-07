namespace ConsoleProject;

class ImageScene: Scene {

  private RenderContent renderContent;
  public override (Window.WindowCommand, object?) ReceiveInput(InputKey input) {
    return (Window.WindowCommand.NextScene, this.NextSceneName);
  }

  public override RenderContent GetRenderContent() => this.renderContent;

  public ImageScene(Dictionary<string, string> param): this(
      param["name"],
      param["image"],
      param.ContainsKey("nextSceneName") ?
      param["nextSceneName"]: null
      ) {}

  public ImageScene(
      string name, 
      string image,
      string? nextSceneName
      ): base(name, SceneState.Rendering) { 
    string[] lines = image.Split('\n');
    List<(string, RenderColor)> contents = new (lines.Length);
    foreach (string line in lines) {
      contents.Add((line, RenderColor.White));
    }
    contents.Add(("", RenderColor.White));
    contents.Add(("\t\t아무 키나 누르세요", RenderColor.Green));
    contents.Add(("", RenderColor.White));
    contents.Add(("", RenderColor.White));
    this.renderContent = new RenderContent(contents);
    this.NextSceneName = nextSceneName;
    if (nextSceneName == null)
      throw (new NotImplementedException());
  }

  public override void OnRenderFinished() {
    this.State = Scene.SceneState.WaitingInput;
  }
}

