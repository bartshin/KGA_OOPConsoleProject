using ConsoleProject;

public interface IInteractable {
  
  InputType AcceptType { get; }
  IList<InputKey> AcceptKeys { get; }

  enum InputType {
    None,
    AnyKey,
    SpecificKeys
  };
}

public interface INavigatable: IInteractable {
  IList<string> Menu { get; }  
  InputType IInteractable.AcceptType => InputType.SpecificKeys;
  IList<InputKey> IInteractable.AcceptKeys => new InputKey[] {
    InputKey.LeftArrow,
    InputKey.RightArrow,
    InputKey.UpArrow,
    InputKey.DownArrow,
    InputKey.Enter,
  };
}
