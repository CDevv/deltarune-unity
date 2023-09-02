using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Luminosity.IO;
using System;

public class GameConfig
{
    [JsonProperty("volume")] public int MasterVolume;
    [JsonProperty("vfx")] public bool SimplifyVFX;
    [JsonProperty("fullscreen")] public bool Fullscreen;
    [JsonProperty("autorun")] public bool Autorun;
    [JsonProperty("keyDOWN")] public KeyCode keyDown;
    [JsonProperty("keyUP")] public KeyCode keyUp;
    [JsonProperty("keyLEFT")] public KeyCode keyLeft;
    [JsonProperty("keyRIGHT")] public KeyCode keyRight;
    [JsonProperty("keyCONFIRM")] public KeyCode keyConfirm;
    [JsonProperty("keyCANCEL")] public KeyCode keyCancel;
    [JsonProperty("keyMENU")] public KeyCode keyMenu;

    public static Dictionary<string, KeyCode> defaults = new()
    {
        ["Up"] = KeyCode.UpArrow,
        ["Down"] = KeyCode.DownArrow,
        ["Left"] = KeyCode.LeftArrow,
        ["Right"] = KeyCode.RightArrow,
        ["Confirm"] = KeyCode.Z,
        ["Cancel"] = KeyCode.X,
        ["Menu"] = KeyCode.C,
    };

    public void ApplyDefaults()
    {
        InputAction inputAction;
        //InputManager
        inputAction = InputManager.GetAction("Unity-Imported", "Vertical");
        inputAction.Bindings[0].Positive = defaults["Up"];
        inputAction.Bindings[0].Negative = defaults["Down"];

        inputAction = InputManager.GetAction("Unity-Imported", "Horizontal");
        inputAction.Bindings[0].Positive = defaults["Right"];
        inputAction.Bindings[0].Negative = defaults["Left"];

        inputAction = InputManager.GetAction("Unity-Imported", "Confirm");
        inputAction.Bindings[1].Positive = defaults["Confirm"];
        inputAction = InputManager.GetAction("Unity-Imported", "Cancel");
        inputAction.Bindings[1].Positive = defaults["Cancel"];
        inputAction = InputManager.GetAction("Unity-Imported", "Menu");
        inputAction.Bindings[1].Positive = defaults["Menu"];
        //Fields
        keyDown = defaults["Down"];
        keyUp = defaults["Up"];
        keyLeft = defaults["Left"];
        keyRight = defaults["Right"];
        keyConfirm = defaults["Confirm"];
        keyCancel = defaults["Cancel"];
        keyMenu = defaults["Menu"];
    }
}
