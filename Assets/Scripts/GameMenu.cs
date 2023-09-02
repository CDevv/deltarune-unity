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
    public GameObject heart;
    public Dictionary<string, GameObject> pages = new();
    public ItemsPage itemsPage;
    public EquipmentPage equipmentPage;
    public StatsPage statsPage;
    public SettingsPage settingsPage;
    [HideInInspector]
    public string currentPage = "Items";
    public int level = 0;
    [HideInInspector]
    public GameObject pageObj;
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
        //itemsPage = transform.GetComponentInChildren<ItemsPage>();
        //equipmentPage = transform.GetComponentInChildren<EquipmentPage>();

        pages["Items"] = itemsPage.gameObject;
        pages["Equipment"] = equipmentPage.gameObject;
        pages["Stats"] = statsPage.gameObject;
        pages["Settings"] = settingsPage.gameObject;
        //this.pages["Stats"] = transform.Find("page-Stats").gameObject;
        //this.pages["Settings"] = transform.Find("page-Settings").gameObject;

        itemsPage.Setup();
        equipmentPage.Setup();
        statsPage.Setup();
        settingsPage.Setup();
        Refresh();
    }

    public void Refresh()
    {
        itemsPage.Refresh();
        equipmentPage.Refresh();
    }

    public void ToggleHeartAnim(bool value)
    {
        RectTransform rect = heart.GetComponent<RectTransform>();
        Animator animator = heart.GetComponent<Animator>();
        animator.SetBool("Selecting", value);
    }

    public void BaseButtonHover(BaseEventData baseEvent)
    {
        GameObject button = baseEvent.selectedObject;
        RectTransform rect = heart.GetComponent<RectTransform>();
        RectTransform optionRect = button.GetComponent<RectTransform>();
        Vector2 vector2 = new Vector2(optionRect.position.x - (optionRect.rect.width / 2) - 20, optionRect.position.y);
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
        GameObject prevPage = this.pages[currentPage].gameObject;
        prevPage.SetActive(false);
        this.currentPage = name;
        GameObject newPage = this.pages[currentPage].gameObject;
        newPage.SetActive(true);

        this.pageObj = newPage;

        //Set up page and select first button
        heart.SetActive(true);
        switch (name)
        {
            default:
                break;
            case "Items":
                itemsPage.OnPageOpen();
                break;
            case "Equipment":
                equipmentPage.OnPageOpen();
                break;
            case "Stats":
                statsPage.OnPageOpen();
                break;
            case "Settings":
                settingsPage.OnPageOpen();
                break;
        }            
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
