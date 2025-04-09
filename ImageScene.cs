using System;

namespace ConsoleProject;

class ImageScene: Scene {
  const int PadLeft = 15;
  private RenderContent renderContent;
  public override (Window.WindowCommand, object?) ReceiveInput(InputKey input) {
    return (Window.WindowCommand.NextScene, this.NextSceneName);
  }
  public string Image { get; private set; }
  public string[] TextAbove { get; private set; }
  public string[] TextBelow { get; private set; }
  public Dictionary<string, string> Data;

  public override RenderContent GetRenderContent() => this.renderContent;

  public ImageScene(Scene.ISceneName name, Dictionary<string, string> param): this(
      name,
      param["image"],
      nextSceneName: param.GetValueOrDefault("nextSceneName"),
      textAbove: param.GetValueOrDefault("textAbove"),
      textBelow: param.GetValueOrDefault("textBelow")
      ) {
    this.Data = param;
  }

  public ImageScene(
      Scene.ISceneName name, 
      string image,
      string? nextSceneName,
      string? textAbove = null,
      string? textBelow = null
      ): base(name, SceneState.Rendering) { 
    this.Image = image;
    this.NextSceneName = nextSceneName;
    this.TextAbove = textAbove != null ? this.SplitText(textAbove): [];
    this.TextBelow = textBelow != null ? this.SplitText(textBelow): [];
    this.renderContent = this.CreateRenderContent();
  }

  public override void OnRenderFinished() {
    this.State = Scene.SceneState.WaitingInput;
  }

  private RenderContent CreateRenderContent() {
    string[] lines = this.Image.Split('\n');
    int numberOfLines = lines.Length;
    if (this.TextAbove.Length > 0) 
      numberOfLines += this.TextAbove.Length + Scene.MarginVertical;
    if (this.TextBelow.Length > 0) 
      numberOfLines += this.TextBelow.Length + Scene.MarginVertical;
    List<(string, RenderColor)> contents = new (numberOfLines);
    if (this.TextAbove.Length > 0) {
      foreach (var line in this.TextAbove) {
        contents.Add((line.PadLeft(ImageScene.PadLeft, ' '), 
              RenderColor.DarkYellow));
      }
      this.AddMargin(contents);
    }
    foreach (string line in lines) {
      contents.Add((line, RenderColor.White));
    }
    if (this.TextBelow.Length > 0) {
      this.AddMargin(contents);
      foreach (var line in this.TextBelow) {
        contents.Add(("\t" + line.PadLeft(ImageScene.PadLeft, ' '), 
              RenderColor.Green));
      }
      this.AddMargin(contents);
    }
    return (new RenderContent(contents, RenderContent.AnimationType.TopToButtom));
  }
}

