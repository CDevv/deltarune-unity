using Luminosity.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingsPage : MonoBehaviour
{
    public VerticalLayoutGroup divMain;
    public VerticalLayoutGroup divControls;
    Dictionary<string, Button> textOptionsMain = new();
    Dictionary<string, ValueOption> valueOptionsMain = new();
    Dictionary<string, ValueOption> valueOptionsControls = new();

    ValueOption currentOption;
    KeyCode code;
    KeyCode finalCode;
    bool allow = false;
    InputAction inputAction;
    ScanSettings settings = new ScanSettings
    {
        ScanFlags = ScanFlags.Key,
        // If the player presses this key the scan will be canceled.
        CancelScanKey = KeyCode.Escape,
        // If the player doesn't press any key within the specified number
        // of seconds the scan will be canceled.
        Timeout = 10
    };
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //float inputHorizontal = Input.GetAxis("Horizontal");
        if (currentOption != null)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (currentOption.type == "str")
                {
                    divControls.GetComponent<CanvasGroup>().interactable = false;
                    currentOption.ToggleKeyInput();
                }
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                if (currentOption.type == "num")
                {
                    int v = currentOption.value - 1;
                    currentOption.SetValue(v);
                }
                if (currentOption.type == "percent")
                {
                    int v = Mathf.Clamp(currentOption.value - 1, 0, 100);
                    currentOption.SetValue(v);
                }
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                if (currentOption.type == "num")
                {
                    int v = currentOption.value + 1;
                    currentOption.SetValue(v);
                }
                if (currentOption.type == "percent")
                {
                    int v = Mathf.Clamp(currentOption.value + 1, 0, 100);
                    currentOption.SetValue(v);
                }
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (currentOption.type == "bool")
                {
                    bool v = !currentOption.boolValue;
                    currentOption.SetBool(v);
                }
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (currentOption.type == "bool")
                {
                    bool v = !currentOption.boolValue;
                    currentOption.SetBool(v);
                }
            }

            if (Input.anyKeyDown && allow)
            {
                if (currentOption != null)
                {
                    if (currentOption.type == "str" && currentOption.keyPress)
                    {
                        InputManager.StartInputScan(settings, x =>
                        {
                            finalCode = x.Key;
                            currentOption.SetString(finalCode.ToString());
                            divControls.GetComponent<CanvasGroup>().interactable = true;
                            currentOption.ToggleKeyInput();

                            switch (currentOption.gameObject.name)
                            {
                                default:
                                    break;
                                case "Down":
                                    inputAction = InputManager.GetAction("Unity-Imported", "Vertical");
                                    inputAction.Bindings[0].Negative = finalCode;
                                    Global.config.keyDown = finalCode;
                                    break;
                                case "Up":
                                    inputAction = InputManager.GetAction("Unity-Imported", "Vertical");
                                    inputAction.Bindings[0].Positive = finalCode;
                                    Global.config.keyUp = finalCode;
                                    break;
                                case "Left":
                                    inputAction = InputManager.GetAction("Unity-Imported", "Horizontal");
                                    inputAction.Bindings[0].Negative = finalCode;
                                    Global.config.keyLeft = finalCode;
                                    break;
                                case "Right":
                                    inputAction = InputManager.GetAction("Unity-Imported", "Horizontal");
                                    inputAction.Bindings[0].Positive = finalCode;
                                    Global.config.keyRight = finalCode;
                                    break;
                                case "Confirm":
                                    inputAction = InputManager.GetAction("Unity-Imported", "Confirm");
                                    inputAction.Bindings[1].Positive = finalCode;
                                    Global.config.keyConfirm = finalCode;
                                    break;
                                case "Cancel":
                                    inputAction = InputManager.GetAction("Unity-Imported", "Cancel");
                                    inputAction.Bindings[1].Positive = finalCode;
                                    Global.config.keyCancel = finalCode;
                                    break;
                                case "Menu":
                                    inputAction = InputManager.GetAction("Unity-Imported", "Menu");
                                    inputAction.Bindings[1].Positive = finalCode;
                                    Global.config.keyMenu = finalCode;
                                    break;
                            }

                            return true;
                        });
                    }
                }
            }

            switch (currentOption.gameObject.name)
            {
                default:
                    break;
                case "Volume":
                    Global.config.MasterVolume = currentOption.value;
                    Global.bgMusic.volume = (currentOption.value / 100f) * 0.33f;
                    break;
                case "SimplVFX":
                    Global.config.SimplifyVFX = currentOption.boolValue;
                    break;
                case "Fullscreen":
                    Screen.fullScreen = currentOption.boolValue;
                    Global.config.Fullscreen = currentOption.boolValue;
                    break;
                case "AutoRun":
                    //PlayerController
                    Global.config.Autorun = currentOption.boolValue;
                    break;
                
            }
        }

        
    }

    public void Setup()
    {
        //Dictionary<string, VerticalLayoutGroup> objects = transform.GetComponentsInChildren<VerticalLayoutGroup>().ToDictionary(x => x.gameObject.name);
        //divMain = objects["MainConfig"];
        //divControls = objects["ControlsConfig"];

        textOptionsMain = divMain.GetComponentsInChildren<Button>().ToDictionary(x => x.gameObject.name);
        valueOptionsMain = divMain.GetComponentsInChildren<ValueOption>().ToDictionary(x => x.gameObject.name);
        valueOptionsControls = divControls.GetComponentsInChildren<ValueOption>().ToDictionary(x => x.gameObject.name);

        foreach (var item in valueOptionsMain)
        {
            item.Value.Setup();
        }
        foreach (var item in valueOptionsControls)
        {
            item.Value.Setup();
        }

        Refresh();
    }

    public void Refresh()
    {
        valueOptionsMain["Volume"].SetValue(Global.config.MasterVolume);
        valueOptionsMain["SimplVFX"].SetBool(Global.config.SimplifyVFX);
        valueOptionsMain["Fullscreen"].SetBool(Global.config.Fullscreen);
        valueOptionsMain["AutoRun"].SetBool(Global.config.Autorun);

        RefreshControlOptions();
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
        divMain.gameObject.SetActive(true);
        divControls.gameObject.SetActive(false);
        currentOption = valueOptionsMain["Volume"];
        valueOptionsMain["Volume"].SelectBtn();
    }

    public void Level2()
    {
        Global.gameMenu.level = 2;
        divMain.gameObject.SetActive(false);
        divControls.gameObject.SetActive(true);
        currentOption = valueOptionsControls["Down"];
        valueOptionsControls["Down"].SelectBtn();
    }

    //Button events
    public void OptionSelect(BaseEventData baseEvent)
    {
        GameObject gameObject = baseEvent.selectedObject;
        RectTransform optionRect = gameObject.GetComponent<RectTransform>();
        RectTransform heartRect = Global.gameMenu.heart.GetComponent<RectTransform>();

        Vector3 vector = new Vector3(optionRect.position.x - (optionRect.sizeDelta.x / 2) - 20, optionRect.position.y);
        heartRect.position = vector;

        currentOption = valueOptionsMain[gameObject.name];
    }

    public void TextOptionSelect(BaseEventData baseEvent)
    {
        Global.gameMenu.BaseButtonHover(baseEvent);
        currentOption = null;
    }

    public void OptionControls(BaseEventData baseEvent)
    {
        Level2();
        //allow = true;
    }

    public void ControlOptionSelect(BaseEventData baseEvent)
    {
        Global.gameMenu.BaseButtonHover(baseEvent);
        currentOption = valueOptionsControls[baseEvent.selectedObject.name];
        allow = true;
    }

    //Text Options
    public void ReturnTopMenu(BaseEventData baseEvent)
    {
        Global.menuTop.ReturnToTop();
    }

    public void ResetConfig(BaseEventData baseEvent)
    {
        Global.config.ApplyDefaults();
        RefreshControlOptions();
    }

    public void ReturnMenu(BaseEventData baseEvent)
    {
        Level1();
        allow = false;
    }

    //Extra
    void RefreshControlOptions()
    {
        valueOptionsControls["Down"].SetString(Global.config.keyDown.ToString());
        valueOptionsControls["Up"].SetString(Global.config.keyUp.ToString());
        valueOptionsControls["Left"].SetString(Global.config.keyLeft.ToString());
        valueOptionsControls["Right"].SetString(Global.config.keyRight.ToString());
        valueOptionsControls["Confirm"].SetString(Global.config.keyConfirm.ToString());
        valueOptionsControls["Cancel"].SetString(Global.config.keyCancel.ToString());
        valueOptionsControls["Menu"].SetString(Global.config.keyMenu.ToString());
    }
}
