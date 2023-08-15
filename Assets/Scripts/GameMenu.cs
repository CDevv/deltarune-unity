using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using UnityEngine.EventSystems;

public class GameMenu : MonoBehaviour
{
    public GameObject ui;
    public GameObject optionPrefab;
    public Dictionary<string, GameObject> pages = new();

    public string currentPage = "Items";
    public int level = 0;
    public GameObject heart;
    public GameObject pageObj;

    public Character character;
    private MenuTop menuTop;
    private MenuBottom menuBottom;

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
        heart.SetActive(true);
        Button firstButton = pageObj.GetComponentInChildren<Button>();
        firstButton.Select();
        PageItems();
     
    }
    //Items Page
    public void PageItems()
    {
        GameObject itemsContainer = pageObj.transform.Find("ItemsContainer").gameObject;
        Debug.Log(string.Join(", ", character.Inventory));
        for (int i = 0; i < Global.mainChar.Inventory.Length; i++)
        {
            string itemName = Global.mainChar.Inventory[i];
            GameObject newBtn = Instantiate(optionPrefab, itemsContainer.transform);
            newBtn.GetComponentInChildren<Text>().text = itemName;
            RectTransform rect = newBtn.GetComponent<RectTransform>();
            Button button = newBtn.GetComponent<Button>();
            
            UnityEngine.Events.UnityAction<BaseEventData> callSelect = new UnityEngine.Events.UnityAction<BaseEventData>(ItemHover);
            UnityEngine.Events.UnityAction<BaseEventData> callClick = new UnityEngine.Events.UnityAction<BaseEventData>(ItemClick);
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
}
