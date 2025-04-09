using System;

namespace ConsoleProject;

abstract class Item {
  public string Name { get; init; }
  public string Description { get; init; }

  protected Item(string name, string description) {
    this.Name = name;
    this.Description = description;
  }

  public readonly struct ItemName: IEquatable<ItemName> {
    public string Value { get; }
    public readonly ItemType Type;
    public bool IsConsumable { get; }
    private ItemName(ItemType itemType) {
      (this.Value, this.IsConsumable) = itemType switch  {
        ItemType.Soup => ("수프 통조림", true),
        ItemType.Water => ("물", true),
        ItemType.Axe => ("도끼", false),
        ItemType.FirstAidKit => ("구급상자", false),
        ItemType.GasMask => ("방독면", false),
        ItemType.Radio => ("라디오", false),
        _ => throw new ArgumentException("invalid itemType"),
      }; 
      this.Type = itemType;
    }
    public static readonly ItemName Soup = new (ItemType.Soup);
    public static readonly ItemName Water = new (ItemType.Water);
    public static readonly ItemName Axe = new (ItemType.Axe);
    public static readonly ItemName GasMask = new (ItemType.GasMask);
    public static readonly ItemName FirstAidKit = new (ItemType.FirstAidKit);
    public static readonly ItemName Radio = new (ItemType.Radio);

    private static List<ItemName> All = new List<ItemType>((ItemType[])Enum.GetValues(typeof(ItemType))).ConvertAll(type => new ItemName(type));

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
      return (other.Type == this.Type);
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

    public enum ItemType {
      Soup,
      Water,
      GasMask,
      Axe,
      FirstAidKit,
      Radio,
    }
  }

  public static Item CreateFrom(ItemName item) {
    if (item.IsConsumable) {
      return (new ConsumableItem(item.Value, Item.GetDescription(item), 1.0));
    }
    else {
      return (new ReusableItem(item.Value, Item.GetDescription(item)));
    }
  }
  private static string GetDescription(ItemName item) {
    switch (item.Type) {
      case ItemName.ItemType.Soup:
        return ("737년 동안 보존 가능한 평범한 토마토 수프 통조림으로,\t 게임 내의 주 식량이다. 미국 수프 브랜드인 캠벨 수프를 패러디했다.");
      case ItemName.ItemType.Water:
        return ("수프와 같이 삶을 연명하는 데 반드시 필요하다. 역시 한 병당 4인 가족이 하루의 갈증을 채울 수 있다.");
      case ItemName.ItemType.GasMask:
        return ("외부 방사능 수치가 높아도 매우 높은 확률로\t 방사능에 피폭되지 않고 오게 해준다.");
      case ItemName.ItemType.Axe:
        return ("강도와 싸우거나 내부 조사,\t 침투한 생물(쥐, 설치류, 벌레 등)을 죽일 때 사용하는 무기이다.");
      case ItemName.ItemType.FirstAidKit:
        return ("종이에 베이는 사소한 경상에서부터 목이 잘리는\t 치명상까지 어떤 상처도 치료 가능하다는 마법의 구급상자.");
      case ItemName.ItemType.Radio:
        return (" 음악이나 밖의 소식을 들어서 정신을 안정시킬 수 있고,\t 이게 있어야 정부의 보급품을 가져가거나\t 군대에게 구조받는 이벤트를 볼 수 있다. ");
      default:
        return ("");
    }
  }
}


class ConsumableItem: Item {

  public double Quantity;

  public ConsumableItem(string name, string description, double quantity): base(name, description) {
    this.Quantity = quantity;
  }
}

class ReusableItem: Item {

  public ReusableItem(string name, string description): base(name, description) {
    this.Name = name;
  }
}

