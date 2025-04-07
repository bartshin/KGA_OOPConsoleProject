
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
      this.OnSetWindow(value);
  }}
  private HashSet<InputKey> waitingKeys = new();
  public bool IsWaitingInput => this.FocusedWindow.AcceptType != IInteractable.InputType.None;
  public IInteractable.InputType AcceptType => this.FocusedWindow.AcceptType;
  
  public void GetInput() {
    if (!this.IsWaitingInput)
      return ;
    var input = Console.ReadKey(
        this.FocusedWindow.IsInputDisplayed
        ).Key;
    this.FocusedWindow.ReceiveInput(input);
  }

  public InputForwarder(Window window) {
    this.focusedWindow = window;
    this.OnSetWindow(window);
  }

  public void OnSetWindow(Window window) {
    this.waitingKeys.Clear();
    foreach (var key in window.AcceptKeys) {
      this.waitingKeys.Add(key);
    }
  }
}

