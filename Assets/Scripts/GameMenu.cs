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

        GameObject top = this.pages["Items"].transform.Find("TopOptions").gameObject;
        GameObject itemsContainer = this.pages["Items"].transform.Find("ItemsContainer").gameObject;
        top.GetComponent<CanvasGroup>().interactable = true;
        itemsContainer.GetComponent<CanvasGroup>().interactable = false;

        foreach (Transform item in itemsContainer.transform)
        {
            Destroy(item);
        }

        string json = Resources.Load<TextAsset>("Json/chara").text;
        this.character = JsonConvert.DeserializeObject<List<Character>>(json).Find(x => x.Name == "Kris");
    }

    public void Refresh()
    {
        GameObject top = this.pages["Items"].transform.Find("TopOptions").gameObject;
        GameObject itemsContainer = this.pages["Items"].transform.Find("ItemsContainer").gameObject;
        top.GetComponent<CanvasGroup>().interactable = true;
        itemsContainer.GetComponent<CanvasGroup>().interactable = false;

        foreach (Transform item in itemsContainer.transform)
        {
            Destroy(item.gameObject);
        }
        //this.character = JsonConvert.DeserializeObject<List<Character>>(json).Find(x => x.Name == "Kris");
    }

    public void ButtonHover(GameObject button)
    {
        //Debug.Log("Selected button");
        RectTransform rect = heart.GetComponent<RectTransform>();
        RectTransform optionRect = button.GetComponent<RectTransform>();
        Vector2 vector2 = new Vector2(optionRect.position.x - (optionRect.rect.width / 2) - 10, optionRect.position.y);
        rect.position = vector2;
    }

    public void BaseButtonHover(BaseEventData baseEvent)
    {
        GameObject button = baseEvent.selectedObject;
        RectTransform rect = heart.GetComponent<RectTransform>();
        RectTransform optionRect = button.GetComponent<RectTransform>();
        Vector2 vector2 = new Vector2(optionRect.position.x - (optionRect.rect.width / 2) - 16, optionRect.position.y - 3);
        rect.position = vector2;
    }

    public void AddEventToButton(GameObject gameObject, UnityEngine.Events.UnityAction<BaseEventData> call, EventTriggerType type)
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

        //Set up page
        switch (name)
        {
            default:
                break;
            case "Items":
                PageItems();
                break;
            case "Equipment":
                EquipmentPage();
                break;
        }

        //Select first button
        heart.SetActive(true);
        Button firstButton = pageObj.GetComponentInChildren<Button>();
        firstButton.Select();      
    }
    //Items Page
    public void PageItems()
    {
        GameObject itemsContainer = pageObj.transform.Find("ItemsContainer").gameObject;
        //Debug.Log(string.Join(", ", character.Inventory));
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
        GameObject divChars = pageObj.transform.Find("div-Char").gameObject;

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
        Text text = divChars.GetComponentInChildren<Text>();
        RectTransform rect = heart.GetComponent<RectTransform>();
        RectTransform optionRect = button.GetComponent<RectTransform>();
        Vector3 vector = new Vector3(optionRect.position.x, optionRect.position.y + 30);
        rect.transform.position = vector;

        Animator animator = heart.GetComponent<Animator>();
        animator.SetBool("Selecting", true);

        rect.sizeDelta = new Vector2(32, 16);

        text.text = button.name;
    }
}
