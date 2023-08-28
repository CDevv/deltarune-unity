using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;

[JsonObject(MemberSerialization.OptIn)]
public class Character
{
    [JsonProperty("inParty")]     public bool InParty;
    [JsonProperty("name")]        public string Name;
    [JsonProperty("color")]       public float[] Color;
    [JsonProperty("description")] public string Description;
    [JsonProperty("hp")]          public int Hp;
    [JsonProperty("maxhp")]       public int Maxhp;
    [JsonProperty("equipment")]   public PlayerEquipment Equipment;
    [JsonProperty("inventory")]   public string[] Inventory;
    [JsonProperty("weaponinv")]   public string[] WeaponInventory;
    [JsonProperty("armorinv")]    public string[] ArmorInventory;
    [JsonProperty("stats")]       public PlayerStats Stats;
    [JsonProperty("basestats")] public PlayerStats BaseStats;

    /*
    public Character(string name, string description, int hp, int maxhp, Equipment equipment, string[] inventory, Stats stats)
    {
        this.Name        = name;
        this.Description = description;
        this.Hp          = hp;
        this.Maxhp       = maxhp;
        this.Equipment   = equipment;
        this.Inventory   = inventory;
        this.Stats       = stats;
    }

    public override string ToString()
    {

    }
    */

    public void AddHP(int value)
    {
        if (Hp + value <= Maxhp)
        {
            Hp += value;
        }
        else
        {
            Hp += value - (Mathf.Abs((Hp + value) - Maxhp));
        }
    }

    public void RemoveItem(string name)
    {
        for (int i = 0; i < Inventory.Length; i++)
        {
            if (Inventory[i] == name)
            {
                Inventory[i] = "";
                break;
            }
        }
        List<string> list = Inventory.ToList();
        list = list.OrderBy(x => x == "").ToList();
        Inventory = list.ToArray();
    }

    public void EquipItem(string name)
    {
        Item item = Global.items.Find(x => x.Name == name);
        if (item == null)
        {
            Stats.Attack = BaseStats.Attack;
            Stats.Defense = BaseStats.Defense;
        }
        else
        {
            if (item.Type == "weapon")
            {
                for (int i = 0; i < WeaponInventory.Length; i++)
                {
                    if (WeaponInventory[i] == name)
                    {
                        WeaponInventory[i] = "";
                        break;
                    }
                }
                Stats.Attack = BaseStats.Attack + item.Attack;
            }
            else
            {
                for (int i = 0; i < ArmorInventory.Length; i++)
                {
                    if (ArmorInventory[i] == name)
                    {
                        ArmorInventory[i] = "";
                        break;
                    }
                }
                Stats.Defense = BaseStats.Defense + item.Defence;
            }
        }
    }
}
