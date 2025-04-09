using System;
using System.Collections.Generic;

namespace ConsoleProject;

sealed class Character {

  private Character.Playable _character;
  public string Name { get; init; }
  public string Description { get; init; }
  public double Health { get; private set; }
  public double Thirst { get; private set; }
  public double Starving { get; private set; }
  public bool IsAlive { get; private set; }

  public Dictionary<Stat, double> Stats { get; private set; }
  public IEnumerable<Status> CurrentStatus => this.currentStatus.AsEnumerable<Status>();
  private static Stat[] allStats = (Stat[])Enum.GetValues(typeof(Stat));
  private HashSet<Status> currentStatus;

  public Character(Character.Playable character, string name, string description, IEnumerable<(Stat statType, double value)> stats) {
    ArgumentNullException.ThrowIfNull(stats);
    this._character = character;
    this.Name = name;
    this.Description = description;
    this.Stats = new Dictionary<Stat, double>();
    this.currentStatus = new();
    this.InitStats(stats);
    this.IsAlive = true;
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

  public void Consume(double food, double water) {
    var foodIntake = this.Stats[Stat.DailyFoodIntake] * 0.1;
    var waterInake = this.Stats[Stat.DailyWaterIntake] * 0.1;
    this.ModifyHunger(foodIntake - food);
    this.ModifyThirst(waterInake - water);
  }

  private void ModifyHunger(double amount) {
    this.Starving += amount;
    if (this.Starving > 2.5)
      this.IsAlive = false;
    if (this.Starving > 2.0) {
      this.SetStatus(Status.Starvation);
      this.RemoveStatus(Status.Hungry);
    }
    else if (this.Starving > 1.0) {
      this.SetStatus(Status.Hungry);
      this.RemoveStatus(Status.Starvation);
    }
    else {
      this.RemoveStatus(Status.Hungry);
      this.RemoveStatus(Status.Starvation);
    }
  }

  private void ModifyThirst(double amount) {
    this.Thirst += amount;
    if (this.Thirst > 2.5) {
      this.IsAlive = false;
    }
    if (this.Thirst > 2.0) {
      this.SetStatus(Status.Dehydration);
      this.RemoveStatus(Status.Thirsty);
    }
    else if (this.Thirst > 1.0) {
      this.SetStatus(Status.Thirsty);
      this.RemoveStatus(Status.Dehydration);
    }
    else {
      this.RemoveStatus(Status.Thirsty);
      this.RemoveStatus(Status.Dehydration);
    }
  }

  private void RemoveStatus(Status status) {
    this.currentStatus.Remove(status);
  }

  private void SetStatus(Status status) {
    this.currentStatus.Add(status);
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

  public static Playable[] AllPlayables = Enum.GetValues<Character.Playable>();

  public static Character.Playable? GetNext(Character.Playable character) {
    var index = Array.IndexOf(Character.AllPlayables, character);
    if (index == Character.AllPlayables.Length - 1)
      return (null);
    return (Character.AllPlayables[index + 1]);
  }
}
