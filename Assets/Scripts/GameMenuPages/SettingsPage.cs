using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingsPage : MonoBehaviour
{
    VerticalLayoutGroup divMain;
    Dictionary<string, Button> textOptionsMain = new();
    Dictionary<string, ValueOption> valueOptionsMain = new();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Setup()
    {
        Dictionary<string, VerticalLayoutGroup> objects = transform.GetComponentsInChildren<VerticalLayoutGroup>().ToDictionary(x => x.gameObject.name);
        divMain = objects["MainConfig"];

        textOptionsMain = divMain.GetComponentsInChildren<Button>().ToDictionary(x => x.gameObject.name);
        valueOptionsMain = divMain.GetComponentsInChildren<ValueOption>().ToDictionary(x => x.gameObject.name);

        foreach (var item in valueOptionsMain)
        {
            item.Value.Setup();
        }

        Refresh();
    }

    public void Refresh()
    {
        valueOptionsMain["Volume"].SetValue(Global.config.MasterVolume);
        valueOptionsMain["SimplVFX"].SetString(Global.config.SimplifyVFX.ToString());
        valueOptionsMain["Fullscreen"].SetString(Global.config.Fullscreen.ToString());
        valueOptionsMain["AutoRun"].SetString(Global.config.Autorun.ToString());
    }

    //Levels
    public void OnPageOpen()
    {
        Global.gameMenu.ToggleHeartAnim(false);
        Level1();
    }

    public void Level1()
    {
        Global.gameMenu.level = 1;
        valueOptionsMain["Volume"].SelectBtn();
    }

    //Button events
    public void OptionSelect(BaseEventData baseEvent)
    {
        GameObject gameObject = baseEvent.selectedObject;
        RectTransform optionRect = gameObject.GetComponent<RectTransform>();
        RectTransform heartRect = Global.gameMenu.heart.GetComponent<RectTransform>();

        Vector3 vector = new Vector3(optionRect.position.x - (optionRect.sizeDelta.x / 2) - 20, optionRect.position.y);
        heartRect.position = vector;
    }
}
