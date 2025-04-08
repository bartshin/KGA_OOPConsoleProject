
namespace ConsoleProject;

class InputScene: Scene {

  public InputScene(ISceneName name): base(name, SceneState.Paused) {

  }

  public override RenderContent GetRenderContent() {
    throw new NotImplementedException();
  }

  public override void OnRenderFinished() {
    throw new NotImplementedException();
  }

  public override (Window.WindowCommand, object?) ReceiveInput(InputKey input) {
    throw new NotImplementedException();
  }
}
