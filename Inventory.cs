using System;

namespace ConsoleProject;

sealed class Inventory {
  private Dictionary<Item.ItemName, Item> Items;

  public void Add(Item.ItemName name) {
    var item = Item.CreateFrom(name);
    if (item is ConsumableItem newItem &&
        this.Items.TryGetValue(name, out Item current) &&
        current is ConsumableItem consumableItem) {
      consumableItem.Quantity += newItem.Quantity;
    }
    else 
      this.Items.Add(name, item); 
  }

  public double GetTotalWater() {
    if (this.Items.TryGetValue(
          Item.ItemName.Water, out Item water
          )) {
      return (((ConsumableItem)water).Quantity);
    }
    return (0.0);
  }

  public double GetTotalSoup() {

    if (this.Items.TryGetValue(
          Item.ItemName.Soup, out Item soup
          )) {
      return (((ConsumableItem)soup).Quantity);
    }
    return (0.0);
  }

  public bool IsContain(Item.ItemName name) {
    return (this.Items.ContainsKey(name));
  }

  public Inventory() {
    this.Items = new();
  }
}
