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

sealed class CharacterFactory {

  private static CharacterFactory instance;
  public static CharacterFactory Shared {
    get { 
      if (CharacterFactory.instance == null)
        CharacterFactory.instance = new CharacterFactory();
      return (CharacterFactory.instance);
    }
  }
  private CharacterFactory() { }
  private Random random = new Random();

  public Character Create(Character.Playable character) {


    return (new Character(
        character,
        CharacterName.Get(character),
        CharacterDescription.Get(character), 
        this.GetStat(character))
        );
  }

  private List<(Character.Stat, double)> GetStat(Character.Playable character) {
    List<(Character.Stat, double)> stats;

    switch (character) {
      case Character.Playable.Ted:
        stats = new() {
          (Character.Stat.MentalToughness, 2.0),
          (Character.Stat.DailyFoodIntake, 2.5),
          (Character.Stat.DailyWaterIntake, 2.2),
          (Character.Stat.DiseaseIncidence, 2.0)
        };
        break;
      case Character.Playable.Dolores:
        stats = new() {
          (Character.Stat.MentalToughness, 5.0),
          (Character.Stat.DailyFoodIntake, 1.8),
          (Character.Stat.DailyWaterIntake, 1.9),
          (Character.Stat.DiseaseIncidence, 4.2)
        };
        break;
      case Character.Playable.MaryJane:
        stats = new() {
          (Character.Stat.MentalToughness, 2.0),
          (Character.Stat.DailyFoodIntake, 2.5),
          (Character.Stat.DailyWaterIntake, 2.0),
          (Character.Stat.DiseaseIncidence, 0.8)
        };
        break;
      case Character.Playable.Timmy: 
        stats = new() {
          (Character.Stat.MentalToughness, 5.0),
          (Character.Stat.DailyFoodIntake, 1.5),
          (Character.Stat.DailyWaterIntake, 1.7),
          (Character.Stat.DiseaseIncidence, 7.2)
        };
        break;
      default: 
        throw (new ApplicationException($"{character} has no stat"));
    }

    return (stats);
  }


  /// Constants
   
  readonly public struct CharacterName  {
    public const string FamilyName = "McDoodle";
    public const string Ted = "Ted";
    public const string Dolores = "Dolores";
    public const string MaryJane = "Mary Jane"; 
    public const string Timmy = "Timmy";
    public const string Pancake = "Pancake";

    public static string Get(Character.Playable character) {
      return (character switch {
          Character.Playable.Ted => CharacterName.Ted,
          Character.Playable.Dolores => CharacterName.Dolores,
          Character.Playable.MaryJane => CharacterName.MaryJane,
          Character.Playable.Timmy => CharacterName.Timmy,
          Character.Playable.Pancake => CharacterName.Pancake,
          _ => "" });
    }
  }

  readonly public struct CharacterDescription() {
    public const string Ted = "Ted is one of the main family members in 60 Seconds! and 60 Seconds!: Reatomized, he is the husband of Dolores and the father of Mary Jane and Timmy";
    public const string Dolores = "Dolores is the wife of Ted and mother of Mary Jane and Timmy. She is the 2nd best in expeditions as she will usually obtain many items, but she is most commonly captured by raiders whilst searching for supplies.";
    public const string MaryJane = "Mary Jane is the daughter of Ted and Dolores and older sister of Timmy. She is overall a liability as she is the least resistant to insanity, she takes up three slots during scavenging." ;
    public const string Timmy = "Timmy is the youngest child of Ted and Dolores and younger brother of Mary Jane. Timmy is the best at scavenging and takes the least amount of time to come home from the wastelands";
    public const string Pancake = "Pancake is a stray dog that comes to the shelter. After a series of events involving the dog, (if done correctly), he will join the family. He, unlike anyone else, will never leave, unless a certain event is done in a certain way.";

    public static string Get(Character.Playable character) {
      return (character switch {
          Character.Playable.Ted => CharacterDescription.Ted,
          Character.Playable.Dolores => CharacterDescription.Dolores,
          Character.Playable.MaryJane => CharacterDescription.MaryJane,
          Character.Playable.Timmy => CharacterDescription.Timmy,
          Character.Playable.Pancake => CharacterDescription.Pancake,
          _ => "",
          });
    }
  }
}

