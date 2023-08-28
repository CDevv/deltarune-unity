using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class GameConfig
{
    [JsonProperty("volume")] public int MasterVolume;
    [JsonProperty("vfx")] public bool SimplifyVFX;
    [JsonProperty("fullscreen")] public bool Fullscreen;
    [JsonProperty("autorun")] public bool Autorun;
}
