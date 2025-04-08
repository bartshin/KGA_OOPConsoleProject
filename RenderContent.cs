global using RenderColor = System.ConsoleColor;

namespace ConsoleProject;

class RenderContent {
  public List<(string, RenderColor)> Contents { get; init; }
  private int currentIndex; 
  public AnimationType Animation { get; private set; } 

  public bool IsEnd => this.Contents.Count - 1 == this.currentIndex;
  public int CurrentIndex { 
    get => this.currentIndex;
    set {
      if (value >= 0 && value < this.Contents.Count) {
        this.currentIndex = value;
      }
    }
  }

  public RenderContent(): this(new(), AnimationType.None) { }

  public RenderContent(List<(string, RenderColor)> contents, AnimationType animation) {
    this.Contents = contents;
    this.currentIndex = 0;
    this.Animation = animation;
  }
  
  public enum AnimationType {
    None,
    TopToButtom,
  }
}
