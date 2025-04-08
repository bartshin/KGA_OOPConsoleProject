namespace ConsoleProject;

class Renderer {
  const int FrameTime = 20;
  public int MainWindowHeight = 30;

  private static Renderer instance;
  public static Renderer Shared {
    get { 
      if (Renderer.instance == null)
        Renderer.instance = new Renderer();
      return (Renderer.instance);
    }
  }
  public event EventHandler OnRenderFinished;
  public List<Window> Windows { get; } = new List<Window>();
  private Renderer() { }
  private ConsoleColor foregroundColor = Console.ForegroundColor;
  private Dictionary<Window, RenderContent> currentContents = new ();

  public RenderContent? GetRenderContentFor(Window window) {
    return this.currentContents.GetValueOrDefault(window);
  }
  public void SetRenderContentFor(Window window, RenderContent content) {
    this.currentContents[window] = content;
  }

  public void SetWindow(Window window) {
    this.Windows.Add(window);
    window.OnRenderStarted(this);
    this.SetRenderContentFor(window, window.GetRenderContent());
    window.OnContentRefreshed += this.OnWindowRefreshed;
  }

  public void RemoveWindow(Window window) {
    this.Windows.Remove(window);
    this.currentContents.Remove(window);
    window.OnContentRefreshed -= this.OnWindowRefreshed;
  }
  
  private void OnWindowRefreshed(object sender, EventArgs args) {
    Window window = (Window)sender;
    this.SetRenderContentFor(window, window.GetRenderContent());
  } 
  
  public void PreceedRender() { 
    Console.Clear();
    var main = this.Windows.Find(window => window.Type == Window.WindowType.Main);

    var bottom = this.Windows.Find(window => window.Type == Window.WindowType.Bottom);
    if (main != null && this.currentContents.TryGetValue(
          main, out RenderContent mainContent)) {
      Console.SetCursorPosition(0, 0);
      this.PreceedRenderOf(mainContent);
    }
    if (bottom != null && this.currentContents.TryGetValue(
          bottom , out RenderContent bottomContent)) {
      Console.SetCursorPosition(0, this.MainWindowHeight);
      this.PreceedRenderOf(bottomContent);
    }
  }

  public void PreceedRenderOf(RenderContent content) { 
    this.foregroundColor = Console.ForegroundColor;
    switch (content.Animation) {
      case RenderContent.AnimationType.None:
        this.RenderWithoutAnimation(content);
        break;
      case RenderContent.AnimationType.TopToButtom:
        this.RenderTopToBottom(content);
        break;
    }
    Console.ForegroundColor = this.foregroundColor;
    this.OnRenderFinished?.Invoke(this, EventArgs.Empty);
  }

  public void RenderWithoutAnimation(RenderContent content) {
    content.CurrentIndex = content.Contents.Count - 1;
    this.RenderToCurrentIndex(content);
  }

  public void RenderTopToBottom(RenderContent content) {
    while (!content.IsEnd) {
      Console.SetCursorPosition(0, 0);
      Thread.Sleep(Renderer.FrameTime);
      this.RenderToCurrentIndex(content);
      content.CurrentIndex += 1;
    }
  }

  private void RenderToCurrentIndex(RenderContent render) {
    var currentColor = this.foregroundColor;
    for (int i = 0; i < render.CurrentIndex; ++i) {
      var (content, color) = render.Contents[i];
      if (currentColor != color) {
        currentColor = color;
        Console.ForegroundColor = color;
      }
      Console.WriteLine(content);
    }
  }
}


