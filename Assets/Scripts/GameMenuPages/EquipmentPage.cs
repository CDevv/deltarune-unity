using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.TextCore.Text;

public class EquipmentPage : MonoBehaviour
{
    CanvasGroup divChars;
    CanvasGroup divStats;
    CanvasGroup divEquipment;
    CanvasGroup divArmors;

    Text charName;
    HorizontalLayoutGroup charsContainer;
    Dictionary<string, Text> statsText = new Dictionary<string, Text>();
    Dictionary<string, EquipSlotOption> equipped = new Dictionary<string, EquipSlotOption>();
    ScrollableContext scrollable;

    GameObject charIcon;
    int childrenCount = 0;
    float[] charIconPos = { -140.75f, -170.75f, -200.75f };

    Character selectedChar;
    string selectedEquipSlot;
    string selectedArmorSlot;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        //float inputHorizontal = Input.GetAxisRaw("Horizontal");
        float inputVertical = Input.GetAxisRaw("Vertical");
        GameObject gameObject = EventSystem.current.currentSelectedGameObject;

        if (Input.GetKeyDown(KeyCode.UpArrow)) //up
        {

            if (gameObject.name == "Option_1")
            {
                scrollable.ScrollUp();
                ArmorSlotSelect(new BaseEventData(EventSystem.current));
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow)) // down
        {
            if (gameObject.name == "Option_5")
            {
                scrollable.ScrollDown();
                ArmorSlotSelect(new BaseEventData(EventSystem.current));
            }
        }
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

        equipped = divEquipment.transform.GetComponentsInChildren<EquipSlotOption>().ToDictionary(x => x.gameObject.name);

        foreach (var item in equipped)
        {
            item.Value.SetupElement();
        }

        scrollable = divArmors.GetComponentInChildren<ScrollableContext>();
        scrollable.SetupElement();

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
            UnityAction<BaseEventData> callSubmit = new UnityAction<BaseEventData>(EquipCharClick);
            Global.gameMenu.AddEventToButton(newBtn, callSelect, EventTriggerType.Select);
            Global.gameMenu.AddEventToButton(newBtn, callSubmit, EventTriggerType.Submit);

            buttons.Add(newBtn);
        }

        childrenCount = buttons.Count;
        charIcon = buttons[0];
    }

    //Level
    public void OnPageOpen()
    {
        Global.gameMenu.ToggleHeartAnim(true);  
        Level1();
        HeartPos();
    }

    public void Level1()
    {
        Global.gameMenu.level = 1;

        divChars.interactable = true;
        divEquipment.interactable = false;
        charIcon.GetComponentInChildren<Button>().Select();
    }

    public void Level2()
    {
        Global.gameMenu.level = 2;

        divChars.interactable = false;
        divEquipment.interactable = true;
        divArmors.interactable = false;
        Button firstButton = divEquipment.GetComponentInChildren<Button>();
        firstButton.Select();
    }

    public void Level3()
    {
        Global.gameMenu.level = 3;

        divEquipment.interactable = false;
        divArmors.interactable = true;

        IconTextOption button = scrollable.GetFirst();
        button.SelectBtn();
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

    public void EquipCharClick(BaseEventData baseEvent)
    {
        Global.gameMenu.ToggleHeartAnim(false);
        Level2();
    }

    public void EquipCharRefresh(string name)
    {
        Character character = Global.characterList.Find(x => x.Name == name);

        charName.text = name;

        statsText["AttackStat"].text = $"Attack: {character.Stats.Attack}";
        statsText["DefenceStat"].text = $"Defence: {character.Stats.Defense}";
        statsText["MagicStat"].text = $"Magic: {character.Stats.Magic}";

        equipped["weapon"].UpdateElement(character.Equipment.Weapon);
        equipped["armor1"].UpdateElement(character.Equipment.Armor1);
        equipped["armor2"].UpdateElement(character.Equipment.Armor2);

        scrollable.Refresh(8, character.WeaponInventory.ToList());

        selectedChar = character;
    }

    public void EquipSlotSelect(BaseEventData baseEvent)
    {
        GameObject gameObject = baseEvent.selectedObject;
        EquipSlotOption option = gameObject.GetComponent<EquipSlotOption>();
        RectTransform iconRect = option.imgSecondary.gameObject.GetComponent<RectTransform>();
        RectTransform heartRect = Global.gameMenu.heart.GetComponent<RectTransform>();
        heartRect.position = iconRect.position;
        option.imgSecondary.gameObject.SetActive(false);

        if (gameObject.name == "weapon")
        {
            scrollable.Refresh(8, selectedChar.WeaponInventory.ToList());
        }
        else
        {
            scrollable.Refresh(8, selectedChar.ArmorInventory.ToList());
        }

        selectedEquipSlot = gameObject.name;
    }

    public void EquipSlotDeselect(BaseEventData baseEvent)
    {
        GameObject gameObject = baseEvent.selectedObject;
        EquipSlotOption option = gameObject.GetComponent<EquipSlotOption>();
        option.imgSecondary.gameObject.SetActive(true);
    }

    public void EquipSlotClick(BaseEventData baseEvent)
    {
        Level3();
    }

    public void ArmorSlotSelect(BaseEventData baseEvent)
    {

        GameObject gameObject = baseEvent.selectedObject;
        IconTextOption option = gameObject.GetComponent<IconTextOption>();
        RectTransform imgRect = option.img.GetComponent<RectTransform>();

        RectTransform heartRect = Global.gameMenu.heart.GetComponent<RectTransform>();
        Vector2 vector = new Vector2(imgRect.position.x + 16, imgRect.position.y);
        heartRect.position = vector;
        option.ToggleImageActive(false);

        if (option.GetText() != "(Nothing.)")
        {
            Item item = Global.items.Find(x => x.Name == option.GetText());
            statsText["AttackStat"].text = $"Attack: {selectedChar.BaseStats.Attack + item.Attack}";
            statsText["DefenceStat"].text = $"Defence: {selectedChar.BaseStats.Defense + item.Defence}";
        }
        else
        {
            statsText["AttackStat"].text = $"Attack: {selectedChar.BaseStats.Attack}";
            statsText["DefenceStat"].text = $"Defence: {selectedChar.BaseStats.Defense}";
        }
        scrollable.selectedItem = option.value;
        selectedArmorSlot = gameObject.name;
    }

    public void ArmorSlotDeselect(BaseEventData baseEvent)
    {
        GameObject gameObject = baseEvent.selectedObject;
        IconTextOption option = gameObject.GetComponent<IconTextOption>();
        option.ToggleImageActive(true);
    }

    public void ArmorSlotClick(BaseEventData baseEvent)
    {
        GameObject gameObject = baseEvent.selectedObject;
        IconTextOption option = gameObject.GetComponent<IconTextOption>();
        EquipSlotOption equipSlot = equipped[selectedEquipSlot];

        string statName = equipSlot.gameObject.name;
        string itemName = option.GetText();

        equipSlot.UpdateElement(option.GetText());
        switch (statName)
        {
            default:
                break;
            case "weapon":
                selectedChar.Equipment.Weapon = itemName;
                break;
            case "armor1":
                selectedChar.Equipment.Armor1 = itemName;
                break;
            case "armor2":
                selectedChar.Equipment.Armor2 = itemName;
                break;
        }
        selectedChar.EquipItem(itemName);
        EquipCharRefresh(selectedChar.Name);
        Level2();
    }

    //Extra methods
    public void HeartPos()
    {
        RectTransform rect = Global.gameMenu.heart.GetComponent<RectTransform>();
        Vector3 vector = new Vector3(charIconPos[childrenCount - 1], 106);
        rect.transform.localPosition = vector;
    }
}
