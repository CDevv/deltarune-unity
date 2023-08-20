using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class GameMenu : MonoBehaviour
{
    public GameObject ui;
    public GameObject optionPrefab;
    public GameObject charPrefab;
    public Dictionary<string, GameObject> pages = new();
    public GameObject heart;
    [HideInInspector]
    public string currentPage = "Items";
    public int level = 0;
    [HideInInspector]
    public GameObject pageObj;
    public Character character;

    public Item selectedItem;

    // Start is called before the first frame update
    void Start()
    {
        //this.heart = ui.transform.Find("Heart").gameObject;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Setup()
    {
        this.pages["Items"] = transform.Find("page-Items").gameObject;
        this.pages["Equipment"] = transform.Find("page-Equipment").gameObject;
        this.pages["Stats"] = transform.Find("page-Stats").gameObject;
        this.pages["Settings"] = transform.Find("page-Settings").gameObject;

        PageItems();
        EquipmentPage();

        string json = Resources.Load<TextAsset>("Json/chara").text;
        this.character = JsonConvert.DeserializeObject<List<Character>>(json).Find(x => x.Name == "Kris");
    }

    public void Refresh()
    {
        PageItems();
        EquipmentPage();
    }

    public void ToggleHeartAnim(bool value)
    {
        RectTransform rect = heart.GetComponent<RectTransform>();
        Animator animator = heart.GetComponent<Animator>();
        animator.SetBool("Selecting", value);
        if (value)
        {
            rect.sizeDelta = new Vector2(32, 16);
        }
        else
        {
            rect.sizeDelta = new Vector2(16, 16);
        }
    }

    public void BaseButtonHover(BaseEventData baseEvent)
    {
        GameObject button = baseEvent.selectedObject;
        RectTransform rect = heart.GetComponent<RectTransform>();
        RectTransform optionRect = button.GetComponent<RectTransform>();
        Vector2 vector2 = new Vector2(optionRect.position.x - (optionRect.rect.width / 2) - 16, optionRect.position.y - 3);
        rect.position = vector2;
    }

    public void AddEventToButton(GameObject gameObject, UnityAction<BaseEventData> call, EventTriggerType type)
    {
        EventTrigger eventTrigger = gameObject.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();

        entry.eventID = type;
        entry.callback = new EventTrigger.TriggerEvent();
        entry.callback.AddListener(call);
        eventTrigger.triggers.Add(entry);
    }

    public void ChangePage(string name)
    {
        this.pages[name] = ui.transform.Find("page-" + name).gameObject;
        GameObject prevPage = this.pages[currentPage];
        prevPage.SetActive(false);
        this.currentPage = name;
        GameObject newPage = this.pages[currentPage];
        newPage.SetActive(true);

        this.pageObj = newPage;

        //Set up page and select first button
        heart.SetActive(true);
        switch (name)
        {
            default:
                break;
            case "Items":
                ToggleHeartAnim(false);
                Button firstButton = newPage.GetComponentInChildren<Button>();
                firstButton.Select();
                break;
            case "Equipment":
                ToggleHeartAnim(true);
                GameObject gameObject = pageObj.transform.Find("div-Char").gameObject.transform.Find("CharsContainer").gameObject;
                GameObject button = gameObject.transform.Find("Kris").gameObject;
                button.GetComponent<EventTrigger>().OnSelect(new BaseEventData(EventSystem.current));
                //EquipCharSelect(button);
                break;
        }             
    }
    //Items Page
    public void PageItems()
    {
        GameObject pg = pages["Items"];
        GameObject itemsContainer = pg.transform.Find("ItemsContainer").gameObject;
        //Debug.Log(string.Join(", ", character.Inventory));
        //Clear
        foreach (Transform item in itemsContainer.transform)
        {
            Destroy(item.gameObject);
        }
        //Populate
        for (int i = 0; i < Global.mainChar.Inventory.Length; i++)
        {
            string itemName = Global.mainChar.Inventory[i];
            GameObject newBtn = Instantiate(optionPrefab, itemsContainer.transform);
            newBtn.GetComponentInChildren<Text>().text = itemName;
            RectTransform rect = newBtn.GetComponent<RectTransform>();
            Button button = newBtn.GetComponent<Button>();
            
            UnityAction<BaseEventData> callSelect = new UnityAction<BaseEventData>(ItemHover);
            UnityAction<BaseEventData> callClick = new UnityAction<BaseEventData>(ItemClick);
            AddEventToButton(newBtn, callSelect, EventTriggerType.Select);
            AddEventToButton(newBtn, callClick, EventTriggerType.Submit);
            //rect.localPosition = new Vector3(36 + rect.rect.width, 10 + i * 5);
        }
    }

    public void OnSelectItemsAction()
    {
        this.level = 2;
        GameObject top = pageObj.transform.Find("TopOptions").gameObject;
        GameObject itemsContainer = pageObj.transform.Find("ItemsContainer").gameObject;
        top.GetComponent<CanvasGroup>().interactable = false;
        itemsContainer.GetComponent<CanvasGroup>().interactable = true;

        Button firstButton = itemsContainer.GetComponentInChildren<Button>();
        firstButton.Select();
    }

    public void ItemsReturnTop()
    {
        this.level = 1;
        GameObject top = pageObj.transform.Find("TopOptions").gameObject;
        GameObject itemsContainer = pageObj.transform.Find("ItemsContainer").gameObject;
        top.GetComponent<CanvasGroup>().interactable = true;
        itemsContainer.GetComponent<CanvasGroup>().interactable = false;

        Button firstButton = top.GetComponentInChildren<Button>();
        firstButton.Select();

        Global.menuTop.HideItemDesc();
    }

    public void ItemHover(BaseEventData baseEvent)
    {
        BaseButtonHover(baseEvent);

        GameObject button = baseEvent.selectedObject;
        string itemID = button.GetComponentInChildren<Text>().text;
        Item item = Global.items.Find(x => x.Name == itemID);

        Global.menuTop.SetItemDesc(item.Description);
        Global.menuTop.ShowItemDesc();

        selectedItem = item;
    }

    public void ItemClick(BaseEventData baseEvent)
    {
        heart.SetActive(false);
        Global.menuBottom.ToggleButtons(true);
        GameObject firstButton = Global.menuBottom.GetFirst();
        firstButton.GetComponent<Button>().Select();
    }

    //Equipment page
    public void EquipmentPage()
    {
        GameObject pg = pages["Equipment"];
        GameObject divChars = pg.transform.Find("div-Char").gameObject;
        GameObject charsContainer = divChars.transform.Find("CharsContainer").gameObject;
        //Clear
        Button[] components = charsContainer.GetComponentsInChildren<Button>();
        foreach (Button item in components)
        {
            Destroy(item.gameObject);
        }
        //Populate
        for (int i = 0; i < Global.characterList.Count; i++)
        {
            Character character = Global.characterList[i];
            Sprite sprite = Resources.Load<Sprite>("Sprites/Ui/equipchar" + (i + 1).ToString());
            GameObject newBtn = Instantiate(charPrefab, charsContainer.transform);
            newBtn.name = character.Name;
            newBtn.GetComponentInChildren<Image>().sprite = sprite;

            UnityAction<BaseEventData> callSelect = new UnityAction<BaseEventData>(EquipCharSelect);
            AddEventToButton(newBtn, callSelect, EventTriggerType.Select);
        }
    }

    public void EquipCharSelect(BaseEventData baseEvent)
    {
        GameObject divChars = pageObj.transform.Find("div-Char").gameObject;
        GameObject button = baseEvent.selectedObject;
        button.GetComponent<Button>().Select();
        Text text = divChars.GetComponentInChildren<Text>();
        RectTransform rect = heart.GetComponent<RectTransform>();
        RectTransform optionRect = button.GetComponent<RectTransform>();
        Vector3 vector = new Vector3(optionRect.position.x, optionRect.position.y + 30);
        rect.transform.position = vector;

        //Animator animator = heart.GetComponent<Animator>();
        //animator.SetBool("Selecting", true);

        //rect.sizeDelta = new Vector2(32, 16);

        text.text = button.name;

        EquipCharRefresh(button.name);
    }

    public void EquipCharSelect(GameObject gameObject)
    {
        GameObject divChars = pageObj.transform.Find("div-Char").gameObject;
        GameObject button = gameObject;
        Text text = divChars.GetComponentInChildren<Text>();
        RectTransform rect = heart.GetComponent<RectTransform>();
        RectTransform optionRect = button.GetComponent<RectTransform>();
        Vector3 vector = new Vector3(optionRect.position.x, optionRect.position.y + 30);
        rect.transform.position = vector;

        text.text = button.name;

        EquipCharRefresh(button.name);
    }

    public void EquipCharRefresh(string name)
    {
        Character character = Global.characterList.Find(x => x.Name == name);

        GameObject divStats = pageObj.transform.Find("div-Stats").gameObject;
        GameObject divEquip = pageObj.transform.Find("div-Equipped").gameObject;

        Text textAtk = divStats.transform.Find("AttackStat").gameObject.GetComponentInChildren<Text>();
        Text textDef = divStats.transform.Find("DefenceStat").gameObject.GetComponentInChildren<Text>();
        Text textMagic = divStats.transform.Find("MagicStat").gameObject.GetComponentInChildren<Text>();

        GameObject textWeapon = divEquip.transform.Find("weapon").gameObject;
        GameObject textArmor1 = divEquip.transform.Find("armor1").gameObject;
        GameObject textArmor2 = divEquip.transform.Find("armor2").gameObject;

        textAtk.text = $"Attack: {character.Stats.Attack}";
        textDef.text = $"Defence: {character.Stats.Defense}";
        textMagic.text = $"Magic: {character.Stats.Magic}";

        EquippedItemUpdate(textWeapon, character.Equipment.Weapon);
        EquippedItemUpdate(textArmor1, character.Equipment.Armor1);
        EquippedItemUpdate(textArmor2, character.Equipment.Armor2);
    }

    public void EquippedItemUpdate(GameObject gameObject, string itemName)
    {
        Text text = gameObject.GetComponentInChildren<Text>();
        Button button = gameObject.GetComponent<Button>();
        Image itemIcon = gameObject.transform.Find("Image").GetComponent<Image>();

        Color disabledColor = new Color((float)0.7843137, (float)0.7843137, (float)0.7843137);

        if (itemName == "")
        {
            ColorBlock colorBlock = button.colors;
            colorBlock.normalColor = disabledColor;
            colorBlock.disabledColor = disabledColor;
            button.colors = colorBlock;
            text.text = "(Nothing.)";

            itemIcon.gameObject.SetActive(false);
        }
        else
        {
            ColorBlock colorBlock = button.colors;
            colorBlock.normalColor = Color.white;
            colorBlock.disabledColor = Color.white;
            button.colors = colorBlock;
            text.text = itemName;

            itemIcon.gameObject.SetActive(true);
        }
    }
}
