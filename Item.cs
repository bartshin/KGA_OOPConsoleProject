using System;

namespace ConsoleProject;

abstract class Item {
  public string Name { get; init; }

  protected Item(string name) {
    this.Name = name;
  }

  public readonly struct ItemName: IEquatable<ItemName> {
    public string Value { get; }
    private readonly Type type;
    public bool IsConsumable { get; }
    private ItemName(Type itemType) {
      (this.Value, this.IsConsumable) = itemType switch  {
        Type.Soup => ("수프 통조림", true),
        Type.Water => ("물", true),
        Type.Axe => ("도끼", false),
        Type.FirstAidKit => ("구급상자", false),
        Type.GasMask => ("방독면", false),
        Type.Shovel => ("삽", false),
        _ => throw new ArgumentException("invalid itemType"),
      }; 
      this.type = itemType;
    }
    public static readonly ItemName Soup = new (Type.Soup);
    public static readonly ItemName Water = new (Type.Water);
    public static readonly ItemName Axe = new (Type.Axe);
    public static readonly ItemName GasMask = new (Type.GasMask);
    public static readonly ItemName FirstAidKit = new (Type.FirstAidKit);
    public static readonly ItemName Shovel = new (Type.Shovel);

    private static List<ItemName> All = new List<Type>((Type[])Enum.GetValues(typeof(Type))).ConvertAll(type => new ItemName(type));

    public override string ToString() {
      return (this.Value);
    }

    public static explicit operator string(ItemName item) => item.ToString();
    public static explicit operator ItemName(string name) => ItemName.FromString(name);

    public static ItemName FromString(string name) {
      var index = ItemName.All.FindIndex(item => item.Value == name);
      if (index == -1)
        throw (new ArgumentException($"cannot find item for: {name}"));
      return (Item.ItemName.All[index]);
    }

    bool IEquatable<ItemName>.Equals(ItemName other) {
      return (other.type == this.type);
    }
    
    public static bool operator ==(ItemName lhs, ItemName rhs) => lhs.Equals(rhs);
    public static bool operator !=(ItemName lhs, ItemName rhs) => !(lhs.Equals(rhs));

    public override int GetHashCode() {
      return (this.Value.GetHashCode());
    }

    public override bool Equals(object? obj) {
      if (obj == null)
        return (this == null);
      if (obj is not Item.ItemName) {
        return (false);
      }
      return this.Equals((Item.ItemName)obj);
    }

    enum Type {
      Soup,
      Water,
      GasMask,
      Axe,
      FirstAidKit,
      Shovel,
    }
  }

  public static Item CreateFrom(ItemName item) {
    if (item.IsConsumable) {
      return (new ConsumableItem(item.Value, 1.0));
    }
    else {
      return (new ReusableItem(item.Value));
    }
  }
}


class ConsumableItem: Item {

  public double Quantity;

  public ConsumableItem(string name, double quantity): base(name) {
    this.Quantity = quantity;
  }
}

class ReusableItem: Item {

  public ReusableItem(string name): base(name) {
    this.Name = name;
  }
}

