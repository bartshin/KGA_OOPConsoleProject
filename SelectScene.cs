namespace ConsoleProject;

sealed class SelectScene<T>: Scene {

  private string prompt;
  public Dictionary<InputKey, T> Selections { get; } 
  public List<InputKey> Selected { get; } = new();
  public int MaximumSelect { get; } 

  public SelectScene(Dictionary<string, object> param)
    : base((string)param["name"], SceneState.Rendering) {
    if (param["prompt"] is string prompt)
      this.prompt = prompt;
    else
      throw (new ArgumentException());

    if (param["selections"] is (InputKey, T)[] selections) {
      this.Selections = new();
      foreach (var (input, value) in selections) {
        this.Selections[input] = value;
        this.AcceptKeys.Add(input);
      }
    }
    else 
      throw (new ArgumentException());

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
    if (this.Selected.Contains(input) &&
      this.MaximumSelect > 1) {
      this.Selected.Remove(input);
    }
    else
      this.Selected.Add(input);
    if (this.MaximumSelect == 1)
      return (Window.WindowCommand.NextScene, this.NextSceneName);
    else if (this.MaximumSelect > 1 && input == InputKey.Enter)
      return (Window.WindowCommand.NextScene, this.NextSceneName);
    return (Window.WindowCommand.None, null);
  }

  public override void OnRenderFinished() {
    this.State = SceneState.WaitingInput;
  }

  private string InputKeyToString(InputKey input) {
    string inputString = input.ToString();
    if (inputString[0] == 'D') {
      return (inputString.Remove(0, 1));
    }
    return (inputString);
  }

  public override RenderContent GetRenderContent() {
    var prompts = this.prompt.Split('\n');
    List<(string, RenderColor)> lists = new();
    foreach (var line in prompts) {
      lists.Add((line, RenderColor.White)); 
    }
    string maximum = this.MaximumSelect.ToString() + (this.MaximumSelect > 1 ? "개까지": "개만");
    lists.Add((maximum + " 고를 수 있습니다.", RenderColor.Red));
    lists.Add(("", RenderColor.White));
    foreach (var (key, value) in this.Selections) {
       lists.Add((string.Format(
               $"[{this.InputKeyToString(key)}]: {value}"), RenderColor.Green)); 
    }
    lists.Add(("", RenderColor.White));
    return (new RenderContent(lists));
  }
}

