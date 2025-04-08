global using RenderColor = System.ConsoleColor;

namespace ConsoleProject;

class RenderContent {
  const int MinWidth = 90;
  public List<(string, RenderColor)> Contents { get; init; }
  private int currentIndex; 
  public AnimationType Animation { get; private set; } 

  public bool IsEnd => this.currentIndex >= this.Contents.Count - 1;
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
    for (int i = 0; i < this.Contents.Count; ++i) {
      if (this.Contents[i].Item2 == RenderColor.White &&
          this.Contents[i].Item1.Length < RenderContent.MinWidth) {
        int margin = RenderContent.MinWidth - this.Contents[i].Item1.Length;
        string line = new string(' ', margin / 2 ) + this.Contents[i].Item1 + new string(' ', margin / 2);
        this.Contents[i] = (line, this.Contents[i].Item2);
      } 
    }
  }
  
  public enum AnimationType {
    None,
    TopToButtom,
  }
}
