using System.Collections;

namespace ConsoleProject;

interface ISelectScene {
  public ICollection<(InputKey, object)> AllSelections { get; }
  public int MaxSelection { get; }
}

sealed class SelectScene<T>: Scene, ISelectScene {

  public static bool IsSelectScene(Scene scene) {
    var type = scene.GetType();
    return  (type.IsGenericType && 
        type.GetGenericTypeDefinition() == typeof (SelectScene<>));
  }

  private string prompt;
  public Dictionary<InputKey, T> Selections { get; } 
  public List<T> Selected { get; } = new();
  public int MaximumSelect { get; }

  public ICollection<(InputKey, object)> AllSelections { get {
    List<(InputKey, object)> list = new();
    foreach (var (key, value) in this.Selections) {
      list.Add((key, (object)value!)); 
    }
    return (list);
  }}
  public int MaxSelection => this.MaximumSelect;

  public SelectScene(ISceneName name, Dictionary<string, object> param)
    : base(name, SceneState.Rendering) {
    if (param["prompt"] is string prompt)
      this.prompt = prompt;
    else
      throw (new ArgumentException());
    if (param["selections"] is List<(InputKey, object)> selections) {
      this.Selections = new();
      foreach (var (input, value) in selections) {
        this.Selections[input] = (T)value;
        this.AcceptKeys.Add(input);
      }
    }
    else 
      throw (new ArgumentException("fail to init selections"));

    if (param.ContainsKey("maximumSelect") && 
        param["maximumSelect"] is int maximumSelect
        ) {
      this.MaximumSelect = maximumSelect;
      if (maximumSelect > 1)
        this.AcceptKeys.Add(InputKey.Enter);
    }
    else
      this.MaximumSelect = 1;
    if (param.ContainsKey("nextSceneName") &&
        param["nextSceneName"] is string nextSceneName) {
      this.NextSceneName = nextSceneName;
    }
    else
      this.NextSceneName = null;
  }

  public override (Window.WindowCommand, object?) ReceiveInput(InputKey input) {
    if (this.MaximumSelect > 1 && input == InputKey.Enter)
      return (Window.WindowCommand.NextScene, this.NextSceneName);
    var inputValue = this.Selections[input];
    if (inputValue == null)
      return (Window.WindowCommand.None, null);
    if (this.Selected.Contains(inputValue) &&
      this.MaximumSelect > 1) {
      this.Selected.Remove(inputValue);
    }
    else if (this.Selected.Count < this.MaxSelection) {
      this.Selected.Add(inputValue);
    }
    if (this.MaximumSelect == 1)
      return (Window.WindowCommand.SendMessage, inputValue);
    return (Window.WindowCommand.SendMessage, this.Selected);
  }

  public override void OnRenderFinished() {
    this.State = SceneState.WaitingInput;
  }

  public override RenderContent GetRenderContent() {
    var prompts = this.prompt.Split('\n');
    List<(string, RenderColor)> lists = new();
    lists.Add(("", RenderColor.White));
    foreach (var line in prompts) {
      lists.Add(("\t" + line, RenderColor.White)); 
    }
    string maximum = this.MaximumSelect.ToString() + (this.MaximumSelect > 1 ? "개까지": "개만");
    lists.Add(("\t" + maximum + " 고를 수 있습니다.", RenderColor.Red));
    lists.Add(("", RenderColor.White));
    foreach (var (key, value) in this.Selections) {
       lists.Add((string.Format(
               $"\t\t[{this.InputKeyToString(key)}]: {value}"), 
             this.IsSelected(value) ? RenderColor.Blue: RenderColor.Green)); 
    }
    lists.Add(("", RenderColor.White));
    return (new RenderContent(lists, RenderContent.AnimationType.None));
  }

  private string InputKeyToString(InputKey input) {
    string inputString = input.ToString();
    if (inputString[0] == 'D') {
      return (inputString.Remove(0, 1));
    }
    return (inputString);
  }

  private bool IsSelected(T value) => this.Selected.IndexOf(value) != -1;
}

