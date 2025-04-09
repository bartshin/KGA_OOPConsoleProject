using System;
using System.Text;

namespace ConsoleProject;

class Renderer {
  const int FrameTime = 20;
  const int Width = 100;
  public static int MainWindowHeight = 30;
  public static int PopupStartHeight = 10;
  public static int PopupMargin = 8;

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
  private Renderer() { 
    Console.CursorVisible = false;
    Console.Clear();
  }
  private ConsoleColor foregroundColor = Console.ForegroundColor;
  private Dictionary<Window, RenderContent> currentContents = new ();
  private RenderContent popUpContent;
  private bool isRenderingPopup = false;

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
    if (window.Type == Window.WindowType.Main &&
        window.PopupScene != null) {
      this.popUpContent = window.PopupScene.GetRenderContent(); 
    }
    this.SetRenderContentFor(window, window.GetRenderContent());
  } 
  
  public void PreceedRender() { 
    Console.Clear();
    var main = this.Windows.Find(window => window.Type == Window.WindowType.Main);

    var bottom = this.Windows.Find(window => window.Type == Window.WindowType.Bottom);
    if (main != null && this.currentContents.TryGetValue(
          main, out RenderContent mainContent)) {
      Console.SetCursorPosition(0, 0);
      this.PreceedRenderOf(mainContent, horizontalEdge: main.HorizontalEdge, verticalEdge: main.VertialEdge, isFocused: InputForwarder.Shared.FocusedWindow == main);
    }
    if (bottom != null && this.currentContents.TryGetValue(
          bottom , out RenderContent bottomContent)) {
      Console.SetCursorPosition(0, Renderer.MainWindowHeight);
      this.PreceedRenderOf(bottomContent, horizontalEdge: bottom.HorizontalEdge, verticalEdge: bottom.VertialEdge, isFocused: InputForwarder.Shared.FocusedWindow == bottom);
    }
    if (this.popUpContent != null) {
      this.isRenderingPopup = true;
      Console.SetCursorPosition(0, Renderer.PopupStartHeight);
      this.PreceedRenderOf(this.popUpContent, horizontalEdge: '*',
          verticalEdge: '*', isFocused: true);
      this.isRenderingPopup = false;
    }
  }

  public void PreceedRenderOf(RenderContent content, bool isFocused = false, char? verticalEdge = null, char? horizontalEdge = null) { 
    string edgeLine = null;
    if (horizontalEdge != null) {
      int edgeWidth = Renderer.Width - (this.isRenderingPopup ? Renderer.PopupMargin * 2 + 10: 0);
      int marginWidth = this.isRenderingPopup ? Renderer.PopupMargin + 2: 1;
      string margin = new string(' ', marginWidth);
      edgeLine = string.Format($"{margin}{
          new string(horizontalEdge.Value, edgeWidth)}{margin}");
    }

    this.foregroundColor = Console.ForegroundColor;
    if (horizontalEdge != null) {
      Console.ForegroundColor = isFocused ? RenderColor.Blue: this.foregroundColor;
      Console.WriteLine(edgeLine);
      Console.ForegroundColor = this.foregroundColor;
    }
    switch (content.Animation) {
      case RenderContent.AnimationType.None:
        (char, RenderColor)? edge = verticalEdge != null ?
          (verticalEdge.Value,
           isFocused ? RenderColor.Blue: RenderColor.White): null;
        this.RenderWithoutAnimation(content, edge);
        break;
      case RenderContent.AnimationType.TopToButtom:
         edge = verticalEdge != null ?
          (verticalEdge.Value,
           isFocused ? RenderColor.Blue: RenderColor.White): null;
        this.RenderTopToBottom(content, edge);
        break;
    }
    if (horizontalEdge != null) {
      Console.ForegroundColor = isFocused ? RenderColor.Blue: this.foregroundColor;
      Console.WriteLine(edgeLine);
    }
    Console.ForegroundColor = this.foregroundColor;
    this.OnRenderFinished?.Invoke(this, EventArgs.Empty);
  }

  public void RenderWithoutAnimation(RenderContent content, (char, RenderColor)? verticalEdge = null) {
    content.CurrentIndex = content.Contents.Count - 1;
    this.RenderToCurrentIndex(content, verticalEdge);
  }

  public void RenderTopToBottom(RenderContent content,
      (char, RenderColor)? verticalEdge = null) {
    while (!content.IsEnd) {
      Console.SetCursorPosition(0, 0);
      Thread.Sleep(Renderer.FrameTime);
      this.RenderToCurrentIndex(content, verticalEdge);
      content.CurrentIndex += 1;
    }
  }

  private void RenderToCurrentIndex(RenderContent render, (char, RenderColor)? verticalEdge) {
    var currentColor = this.foregroundColor ;
    bool isBackground = this.popUpContent != null && !this.isRenderingPopup;
    if (isBackground) {
      currentColor = RenderColor.DarkGray;
      Console.ForegroundColor = RenderColor.DarkGray;
    }
    for (int i = 0; i < render.CurrentIndex; ++i) {
      var (content, color) = render.Contents[i];
      if (currentColor != color && !isBackground) {
        currentColor = color;
        Console.ForegroundColor = color;
      }
      var length = Math.Max(Encoding.Unicode.GetByteCount(content), content.Length);
      var paddingLength = this.isRenderingPopup ? 3:  ( Renderer.Width - length) / 3;
      var padding = length < Renderer.Width ? new string(' ', paddingLength): "";
      Console.WriteLine($"{padding}{content}");
      if (verticalEdge != null) {
        var (left, top) = Console.GetCursorPosition();
        Console.ForegroundColor = verticalEdge.Value.Item2;
        if (this.isRenderingPopup) 
          Console.SetCursorPosition(Renderer.PopupMargin, top - 1);
        else 
          Console.SetCursorPosition(0, top - 1);
        Console.Write(verticalEdge.Value.Item1);
        if (this.isRenderingPopup)
          Console.SetCursorPosition(Renderer.Width - Renderer.PopupMargin * 2, top - 1);
        else
          Console.SetCursorPosition(Renderer.Width + 1, top - 1);
        Console.Write(verticalEdge.Value.Item1);
        Console.SetCursorPosition(left, top);
        Console.ForegroundColor = currentColor;
      }
    }
  }
}


