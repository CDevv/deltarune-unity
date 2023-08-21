using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Linq;

public class EquipmentPage : MonoBehaviour
{
    CanvasGroup divChars;
    CanvasGroup divStats;
    CanvasGroup divEquipment;
    CanvasGroup divArmors;

    Text charName;
    HorizontalLayoutGroup charsContainer;
    Dictionary<string, Text> statsText = new Dictionary<string, Text>();
    Dictionary<string, Text> equipped = new Dictionary<string, Text>();

    GameObject charIcon;
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
        Dictionary<string, CanvasGroup> objects = transform.GetComponentsInChildren<CanvasGroup>().ToDictionary(x => x.name);
        divChars = objects["div-Char"];
        divStats = objects["div-Stats"];
        divEquipment = objects["div-Equipped"];
        divArmors = objects["div-Armors"];

        charsContainer = divChars.GetComponentInChildren<HorizontalLayoutGroup>();
        charName = divChars.GetComponentInChildren<Text>();

        Dictionary<string, Text> statsDict = divStats.transform.GetComponentsInChildren<Text>().ToDictionary(x => x.transform.parent.gameObject.name);
        statsText = statsDict;

        equipped = divEquipment.transform.GetComponentsInChildren<Text>().ToDictionary(x => x.transform.parent.gameObject.name);

        Refresh();
    }

    public void Refresh()
    {
        List<GameObject> buttons = new List<GameObject>();
        //Clear
        foreach (Transform item in charsContainer.transform)
        {
            Destroy(item.gameObject);
        }
        //Populate
        for (int i = 0; i < Global.characterList.Count; i++)
        {
            Character character = Global.characterList[i];
            Sprite sprite = Resources.Load<Sprite>("Sprites/Ui/equipchar" + (i + 1).ToString());
            GameObject newBtn = Instantiate(Global.gameMenu.charPrefab, charsContainer.transform);
            newBtn.name = character.Name;
            newBtn.GetComponentInChildren<Image>().sprite = sprite;

            UnityAction<BaseEventData> callSelect = new UnityAction<BaseEventData>(EquipCharSelect);
            Global.gameMenu.AddEventToButton(newBtn, callSelect, EventTriggerType.Select);

            buttons.Add(newBtn);
        }
        charIcon = buttons[0];
        

        //Invoke("GetButton", 0.02f);
    }



    //Level
    public void OnPageOpen()
    {
        //Canvas.ForceUpdateCanvases();
        
        //StartCoroutine(GetButton());
        Global.gameMenu.ToggleHeartAnim(true);
        Level1();
    }

    public void Level1()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(charsContainer.GetComponent<RectTransform>());
        divChars.interactable = true;

        //Button firstButton = charsContainer.GetComponentInChildren<Button>();
        //firstButton.Select();
        charIcon.GetComponentInChildren<Button>().Select();
        //EquipCharSelect(charIcon);
    }

    //Button events
    public void EquipCharSelect(BaseEventData baseEvent)
    {
        GameObject button = baseEvent.selectedObject;
        RectTransform rect = Global.gameMenu.heart.GetComponent<RectTransform>();
        RectTransform optionRect = button.GetComponent<RectTransform>();
        Vector3 vector = new Vector3(optionRect.position.x, optionRect.position.y + 30);
        rect.transform.position = vector;

        EquipCharRefresh(button.name);
    }

    public void EquipCharSelect(GameObject gameObject)
    {
        GameObject button = gameObject;
        RectTransform rect = Global.gameMenu.heart.GetComponent<RectTransform>();
        RectTransform optionRect = button.GetComponent<RectTransform>();
        Vector3 vector = new Vector3(optionRect.position.x, optionRect.position.y + 30);
        rect.transform.position = vector;

        EquipCharRefresh(button.name);
    }

    public void EquipCharRefresh(string name)
    {
        Character character = Global.characterList.Find(x => x.Name == name);

        statsText["AttackStat"].text = $"Attack: {character.Stats.Attack}";
        statsText["DefenceStat"].text = $"Defence: {character.Stats.Defense}";
        statsText["MagicStat"].text = $"Magic: {character.Stats.Magic}";

        EquippedItemUpdate(equipped["weapon"].gameObject, character.Equipment.Weapon);
        EquippedItemUpdate(equipped["armor1"].gameObject, character.Equipment.Armor1);
        EquippedItemUpdate(equipped["armor2"].gameObject, character.Equipment.Armor2);

        charName.text = name;
    }

    public void EquippedItemUpdate(GameObject gameObject, string name)
    {
        Button button = gameObject.GetComponent<Button>();
        Text text = gameObject.GetComponentInChildren<Text>();
        Image icon = gameObject.GetComponentInChildren<Image>();

        Color disabledColor = new Color(0.7843137f, 0.7843137f, 0.7843137f);

        if (name == "")
        {
            text.text = "(Nothing.)";
            ColorBlock colorBlock = button.colors;
            colorBlock.normalColor = disabledColor;
            colorBlock.disabledColor = disabledColor;
            button.colors = colorBlock;
            icon.gameObject.SetActive(false);
        }
        else
        {
            text.text = name;
            ColorBlock colorBlock = button.colors;
            colorBlock.normalColor = Color.white;
            colorBlock.disabledColor = Color.white;
            button.colors = colorBlock;
            icon.gameObject.SetActive(true);
        }
    }

    //Extra methods
    private IEnumerator GetButton()
    {
        yield return null;
        Button firstButton = charsContainer.GetComponentInChildren<Button>();
        charIcon = firstButton.gameObject;
        //firstButton.Select();
    }
}
