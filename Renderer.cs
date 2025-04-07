namespace ConsoleProject;

class Renderer {

  private static Renderer instance;
  public static Renderer Shared {
    get { 
      if (Renderer.instance == null)
        Renderer.instance = new Renderer();
      return (Renderer.instance);
    }
  }
  private Renderer() { }
  const int FrameTime = 50;
  public event EventHandler OnRenderFinished;
  public List<Window> Windows { get; } = new List<Window>();
  private RenderContent currentContent;
  public RenderContent CurrentContent {
    get => this.currentContent;
    set {
      ArgumentNullException.ThrowIfNull(value);
      this.currentContent = value;
    }
  }

  public void SetWindow(Window window) {
    this.Windows.Add(window);
    window.OnRenderStarted(this);
    if (this.currentContent == null) {
      this.currentContent = window.GetRenderContent();
    }
    window.OnContentRefreshed += (object sender, EventArgs args) => {
      Window window = (Window)sender;
      this.currentContent = window.GetRenderContent();
    };
  }

  private ConsoleColor foregroundColor = Console.ForegroundColor;

  public void PreceedRender() { 
    this.foregroundColor = Console.ForegroundColor;
    while (!this.CurrentContent.IsEnd) {
      Thread.Sleep(Renderer.FrameTime);
      this.Render();
      this.currentContent.CurrentIndex += 1;
    }
    Console.ForegroundColor = this.foregroundColor;
    this.OnRenderFinished?.Invoke(this, EventArgs.Empty);
  }

  private void Render() {
    Console.Clear();
    var currentColor = this.foregroundColor;
    for (int i = 0; i < this.CurrentContent.CurrentIndex; ++i) {
      var (content, color) = this.CurrentContent.Contents[i];
      if (currentColor != color) {
        currentColor = color;
        Console.ForegroundColor = color;
      }
      Console.WriteLine(content);
    }
  }
}


