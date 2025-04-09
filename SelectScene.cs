using System.Collections;

namespace ConsoleProject;

interface ISelectScene {
  public ICollection<(InputKey, object)> AllSelections { get; }
  public int MaxSelection { get; }
  public ICollection<object> CurrentSelection { get; }
}

sealed class SelectScene<T>: Scene, ISelectScene {

  public static bool IsSelectScene(Scene scene) {
    var type = scene.GetType();
    return  (type.IsGenericType && 
        type.GetGenericTypeDefinition() == typeof (SelectScene<>));
  }

  private string prompt;
  public Dictionary<InputKey, T> Selections { get; } 
  public List<InputKey> Selected { get; } = new();
  public int MaximumSelect { get; }

  public ICollection<(InputKey, object)> AllSelections { get {
    List<(InputKey, object)> list = new();
    foreach (var (key, value) in this.Selections) {
      list.Add((key, (object)value!)); 
    }
    return (list);
  }}
  public int MaxSelection => this.MaximumSelect;

  public ICollection<object> CurrentSelection => this.Selected.ConvertAll<object>(key => (object)this.Selections[key]!);

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
      if (maximumSelect > 1 )
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
    if (input == InputKey.Enter) {
      if (this.MaximumSelect > 1 && this.Selected.Count > 0)
        return (Window.WindowCommand.NextScene, this.NextSceneName);
      return (Window.WindowCommand.None, null);
    }
    var inputValue = this.Selections[input];
    if (inputValue == null)
      return (Window.WindowCommand.None, null);
    if (this.Selected.Contains(input) &&
      this.MaximumSelect > 1) {
      this.Selected.Remove(input);
    }
    else if (this.Selected.Count < this.MaxSelection) {
      this.Selected.Add(input);
    }
    if (this.MaximumSelect == 1)
      return (Window.WindowCommand.SendMessage, new T[]{ inputValue });
    return (Window.WindowCommand.SendMessage, this.Selected);
  }

  public override void OnRenderFinished() {
    this.State = SceneState.WaitingInput;
  }

  public override RenderContent GetRenderContent() {
    var prompts = this.prompt.Split('\n');
    List<(string, RenderColor)> lists = new();
    this.AddMargin(lists, 1);
    foreach (var line in prompts) {
      lists.Add(("   " + line, RenderColor.Gray)); 
    }
    this.AddMargin(lists, 1);
    string maximum = this.MaximumSelect.ToString() + (this.MaximumSelect > 1 ? " 항목까지": " 항목만");
    lists.Add(("      " + maximum + " 고를 수 있습니다.", RenderColor.Red));
    lists.Add((string.Format($"{this.Selected.Count} 항목 선택됨"), RenderColor.Gray));
    this.AddMargin(lists, 1);
    foreach (var (key, value) in this.Selections) {
       lists.Add((string.Format(
               $"   [{this.InputKeyToString(key)}]: {value}"), 
             this.IsSelected(key) ? RenderColor.Blue: RenderColor.Green)); 
    }
    this.AddMargin(lists, 1);
    return (new RenderContent(lists, RenderContent.AnimationType.None));
  }

  private string InputKeyToString(InputKey input) {
    string inputString = input.ToString();
    if (inputString[0] == 'D' && inputString.Length > 1) {
      return (inputString.Remove(0, 1));
    }
    return (inputString);
  }

  private bool IsSelected(InputKey key) => this.Selected.IndexOf(key) != -1;
}

