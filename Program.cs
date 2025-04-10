namespace ConsoleProject;

class Program {

  static void Main() {

    var titleScene = SceneFactory.Shared.Build(SceneFactory.ImageSN.Title);
    Game game = new Game(new SceneProgressor(titleScene));
    InputForwarder.Shared.FocusedWindow = game.MainWindow;
    Renderer.Shared.SetWindow(game.MainWindow);
    while (!game.IsEnded) {
      Renderer.Shared.PreceedRender();
      InputForwarder.Shared.GetInput();
    }
    Console.WriteLine("Game Over!");
  }
}
