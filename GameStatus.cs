namespace ConsoleProject;

class GameStatus {
  Dictionary<Section, object> dict;
  public Section[] Sections { get; init; }

  public enum Section {
    TotalDay,
    RemainingSoup,
    RemainingWater,
    CharacterStatus,
    Items,
  }

  public void Add(Section section, object value) {
    this.dict.Add(section, value);
  }

  public bool TryGet<T>(Section section, out T value) {
    if (!this.dict.TryGetValue(section, out object? o) 
        || o == null) {
      value = default(T);
      return (false);
    }
    value = (T)o;
    return (true);
  }

  public GameStatus(params Section[] sections) {
    this.dict = new ();
    this.Sections = sections;
  }
}
