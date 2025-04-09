using System;

namespace ConsoleProject;

class TableScene : Scene, INavigatable {

  public TableScene(Scene.ISceneName name): base(name, Scene.SceneState.Rendering) {
  }


  public IList<string> Menu => new List<string>() {
    "A",
    "B",
  };

  public override RenderContent GetRenderContent() {
    List<(string, RenderColor)> list = new();

    list.Add((("Text"), RenderColor.White));
    list.Add((("Text"), RenderColor.White));
    list.Add((("Text"), RenderColor.White));
    list.Add((("Text"), RenderColor.White));

    return (new RenderContent(list, RenderContent.AnimationType.None));
  }

  public override (Window.WindowCommand, object?) ReceiveInput(InputKey input) {
    throw new NotImplementedException();
  }
}
