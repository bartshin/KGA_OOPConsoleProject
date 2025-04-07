global using RenderColor = System.ConsoleColor;

namespace ConsoleProject;

class RenderContent {
  public List<(string, RenderColor)> Contents { get; init; }
  private int currentIndex; 

  public bool IsEnd => this.Contents.Count - 1 == this.currentIndex;
  public int CurrentIndex { 
    get => this.currentIndex;
    set {
      if (value >= 0 && value < this.Contents.Count) {
        this.currentIndex = value;
      }
    }
  }

  public RenderContent(): this(new()) { }

  public RenderContent(List<(string, RenderColor)> contents) {
    this.Contents = contents;
    this.currentIndex = 0;
  }
}
