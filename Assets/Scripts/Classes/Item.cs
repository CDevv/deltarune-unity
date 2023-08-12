using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    [JsonProperty("name")] public string Name;
    [JsonProperty("description")] public string Description;
    [JsonProperty("type")] public string Type;
    [JsonProperty("effects")] public int Effect;
}
