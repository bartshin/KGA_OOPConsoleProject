namespace ConsoleProject;

class Game {
  public bool IsEnded { get; private set; } = false;
  readonly public GameConfig Config = new GameConfig();
  private GameData data = new GameData();

  public Game() {}

  readonly public struct GameConfig {
    readonly int numberOfTotalDays = 30;

    public GameConfig() {}
  }

  public void PickCharacters(params Character[] characters) {
    foreach (Character character in characters) {
       this.data.AddCharacter(character); 
    }
  }

  private class GameData {
    public List<Character> Characters { get; }

    public GameData() {
      this.Characters = new List<Character>();
    }

    public void AddCharacter(Character character) {
      this.Characters.Add(character); 
    }
  }
}

