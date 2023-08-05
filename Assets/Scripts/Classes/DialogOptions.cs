using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]

public class DialogOptions
{
    [JsonProperty("title")] public string Title;
    [JsonProperty("dialogue_id")] public string DialogID;

    public DialogOptions(string title, string id)
    {
        this.Title = title;
        this.DialogID = id;
    }

    public override string ToString(){
        return $"Option {this.Title} with dialog id: {this.DialogID}";
    }
}
