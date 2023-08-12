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

    private Character character;
    private GameObject menuTop;
    private MenuTop menuTopScript;

    // Start is called before the first frame update
    void Start()
    {
        //this.heart = ui.transform.Find("Heart").gameObject;
        this.pages["Items"] = ui.transform.Find("page-Items").gameObject;
        this.pages["Equipment"] = ui.transform.Find("page-Equipment").gameObject;
        this.pages["Stats"] = ui.transform.Find("page-Stats").gameObject;
        this.pages["Settings"] = ui.transform.Find("page-Settings").gameObject;

        string json = Resources.Load<TextAsset>("Json/chara").text;
        this.character = JsonConvert.DeserializeObject<List<Character>>(json).Find(x => x.Name == "Kris");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Setup(GameObject topObj)
    {
        this.menuTop = topObj;
        this.menuTopScript = topObj.GetComponent<MenuTop>();

        this.pages["Items"] = ui.transform.Find("page-Items").gameObject;
        this.pages["Equipment"] = ui.transform.Find("page-Equipment").gameObject;
        this.pages["Stats"] = ui.transform.Find("page-Stats").gameObject;
        this.pages["Settings"] = ui.transform.Find("page-Settings").gameObject;

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

        string json = Resources.Load<TextAsset>("Json/chara").text;
        this.character = JsonConvert.DeserializeObject<List<Character>>(json).Find(x => x.Name == "Kris");
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
        Vector2 vector2 = new Vector2(optionRect.position.x - (optionRect.rect.width / 2) - 10, optionRect.position.y);
        rect.position = vector2;
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
        for (int i = 0; i < character.Inventory.Length; i++)
        {
            string itemName = character.Inventory[i];
            GameObject newBtn = Instantiate(optionPrefab, itemsContainer.transform);
            newBtn.GetComponentInChildren<Text>().text = itemName;
            RectTransform rect = newBtn.GetComponent<RectTransform>();
            EventTrigger eventTrigger = newBtn.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            Button button = newBtn.GetComponent<Button>();

            entry.eventID = EventTriggerType.Select;
            entry.callback = new EventTrigger.TriggerEvent();
            UnityEngine.Events.UnityAction<BaseEventData> call = new UnityEngine.Events.UnityAction<BaseEventData>(ItemHover);
            entry.callback.AddListener(call);
            eventTrigger.triggers.Add(entry);

            //rect.localPosition = new Vector3(36 + rect.rect.width, 10 + i * 5);
        }
    }

    public void OnSelectItemsAction()
    {
        this.level++;
        GameObject top = pageObj.transform.Find("TopOptions").gameObject;
        GameObject itemsContainer = pageObj.transform.Find("ItemsContainer").gameObject;
        top.GetComponent<CanvasGroup>().interactable = false;
        itemsContainer.GetComponent<CanvasGroup>().interactable = true;

        Button firstButton = itemsContainer.GetComponentInChildren<Button>();
        firstButton.Select();
    }

    public void ItemsReturnTop()
    {
        this.level--;
        GameObject top = pageObj.transform.Find("TopOptions").gameObject;
        GameObject itemsContainer = pageObj.transform.Find("ItemsContainer").gameObject;
        top.GetComponent<CanvasGroup>().interactable = true;
        itemsContainer.GetComponent<CanvasGroup>().interactable = false;

        Button firstButton = top.GetComponentInChildren<Button>();
        firstButton.Select();

        menuTopScript.HideItemDesc();
    }

    public void ItemHover(BaseEventData baseEvent)
    {
        BaseButtonHover(baseEvent);

        GameObject button = baseEvent.selectedObject;
        string itemID = button.GetComponentInChildren<Text>().text;
        string itemJSON = Resources.Load<TextAsset>("Json/items").text;
        Item item = JsonConvert.DeserializeObject<List<Item>>(itemJSON).Find(x => x.Name == itemID);

        GameObject container = menuTop.transform.Find("ItemDescContainer").gameObject;
        container.GetComponentInChildren<Text>().text = item.Description;
        menuTopScript.ShowItemDesc();
    }
}
