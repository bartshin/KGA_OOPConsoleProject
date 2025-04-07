namespace ConsoleProject;

class Program {

  static void Main() {

    var titleScene = SceneFactory.Shared.Build(SceneFactory.SceneType.Image, SceneFactory.ImageSceneName.Title);
    var mainWindow = new Window(titleScene, Window.WindowType.Main);
    Renderer.Shared.SetWindow(mainWindow);
    InputForwarder.Shared.FocusedWindow = mainWindow;
    Game game = new Game();
    while (!game.IsEnded) {
      Renderer.Shared.PreceedRender();
      if (InputForwarder.Shared.IsWaitingInput)
        InputForwarder.Shared.GetInput();
    }
  }
}
