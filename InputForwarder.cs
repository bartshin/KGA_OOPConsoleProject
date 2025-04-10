
global using InputKey = System.ConsoleKey;
namespace ConsoleProject;

class InputForwarder {
  private static InputForwarder instance;
  public static InputForwarder Shared {
    get { 
      if (InputForwarder.instance == null)
        InputForwarder.instance = new InputForwarder();
      return (InputForwarder.instance);
    }
  }
  private InputForwarder() { }

  private Window focusedWindow;
  public Window FocusedWindow {
    get => this.focusedWindow;
    set {
      ArgumentNullException.ThrowIfNull(value);
      this.focusedWindow = value;
  }}
  public IInteractable.InputType AcceptType => this.FocusedWindow.AcceptType;
  
  public void GetInput() {
    if (Console.KeyAvailable) {
      Thread.Sleep(200);
      Console.ReadKey(true);
    }
    var input = Console.ReadKey(true).Key;
    if (this.FocusedWindow.AcceptType == IInteractable.InputType.AnyKey
        || this.FocusedWindow.AcceptKeys.Contains(input))
      this.FocusedWindow.ReceiveInput(input);
  }

  public InputForwarder(Window window) {
    this.focusedWindow = window;
  }
}

