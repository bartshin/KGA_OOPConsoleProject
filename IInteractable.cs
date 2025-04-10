using ConsoleProject;

public interface IInteractable {
  
  InputType AcceptType { get; }
  public IList<InputKey> AcceptKeys { get; }

  enum InputType {
    AnyKey,
    SpecificKeys
  };
}

public interface INavigatable {
  IList<string> Menu { get; }  
  IList<InputKey> AcceptKeys => new InputKey[] {
    InputKey.LeftArrow,
    InputKey.RightArrow,
    InputKey.UpArrow,
    InputKey.DownArrow,
    InputKey.Enter,
  };
}
