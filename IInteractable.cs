using ConsoleProject;

public interface IInteractable {
  
  bool IsInputDisplayed { get; }
  InputType AcceptType { get; }
  IEnumerable<InputKey> AcceptKeys { get; }

  enum InputType {
    None,
    AnyKey,
    SpecificKeys
  };
}

