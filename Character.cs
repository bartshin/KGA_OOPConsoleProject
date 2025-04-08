namespace ConsoleProject;

sealed class Character {

  private Character.Playable _character;
  public string Name { get; init; }
  public string Description { get; init; }
  public double Health { get; private set; }
  public double Thirst { get; private set; }
  public double Starving { get; private set; }

  public Dictionary<Stat, double> Stats { get; private set; }
  public List<Status> currentStatus { get; private set; }
  private static Stat[] allStats = (Stat[])Enum.GetValues(typeof(Stat));

  public Character(Character.Playable character, string name, string description, IEnumerable<(Stat statType, double value)> stats) {
    ArgumentNullException.ThrowIfNull(stats);
    this._character = character;
    this.Name = name;
    this.Description = description;
    this.Stats = new Dictionary<Stat, double>();
    this.currentStatus = new List<Status>();
    this.InitStats(stats);
  }

  public double GetConditionLevel() {
    throw (new NotImplementedException());
  }

  private void InitStats(IEnumerable<(Stat, double)> stats) {
    foreach (var (statType, value) in stats) {
      this.Stats[statType] = value;
    }
    int missingStatIndex = Array.FindIndex(Character.allStats,
        (stat) => !this.Stats.ContainsKey(stat));
    if (missingStatIndex != -1) {
      throw (new ApplicationException($"${Character.allStats[missingStatIndex]} is missing for character {this.Name}"));
    }
  }

  public enum Stat {
    MentalToughness,
    DailyFoodIntake,
    DailyWaterIntake,
    DiseaseIncidence,
  }

  public enum Status {
    Hungry,
    Starvation,
    Thirsty,
    Dehydration,
    Sick,
    Crazy,
    Fatigued,
    Hurt,
    Tired,
  }

  public enum Playable {
    Ted,
    Dolores,
    MaryJane,
    Timmy,
    Pancake,
  }
}
