namespace ConsoleProject;

class Program {

  static void Main() {

    var titleScene = SceneFactory.Shared.Build(SceneFactory.ImageSN.Title);
    Game game = new Game(new SceneProgressor(titleScene));
    InputForwarder.Shared.FocusedWindow = game.MainWindow;
    while (true) {
      Renderer.Shared.PreceedRender();
      InputForwarder.Shared.GetInput();
    }
  }
}
