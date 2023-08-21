using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ItemsPage : MonoBehaviour
{
    CanvasGroup topOptions;
    CanvasGroup itemsContainer;

    public Item selectedItem;
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
        CanvasGroup[] objects = transform.GetComponentsInChildren<CanvasGroup>();
        topOptions = objects[0];
        itemsContainer = objects[1];
    }

    public void Refresh()
    {
        //Clear
        foreach (Transform item in itemsContainer.transform)
        {
            Destroy(item.gameObject);
        }
        //Populate
        for (int i = 0; i < Global.mainChar.Inventory.Length; i++)
        {
            string itemName = Global.mainChar.Inventory[i];
            GameObject newBtn = Instantiate(Global.gameMenu.optionPrefab, itemsContainer.transform);
            newBtn.GetComponentInChildren<Text>().text = itemName;
            RectTransform rect = newBtn.GetComponent<RectTransform>();
            Button button = newBtn.GetComponent<Button>();

            UnityAction<BaseEventData> callSelect = new UnityAction<BaseEventData>(ItemHover);
            UnityAction<BaseEventData> callClick = new UnityAction<BaseEventData>(ItemClick);
            Global.gameMenu.AddEventToButton(newBtn, callSelect, EventTriggerType.Select);
            Global.gameMenu.AddEventToButton(newBtn, callClick, EventTriggerType.Submit);
        }
    }

    //Level
    public void OnPageOpen()
    {
        Global.gameMenu.ToggleHeartAnim(false);
        Level1();
    }

    public void Level1()
    {
        Global.gameMenu.level = 1;

        topOptions.interactable = true;
        itemsContainer.interactable = false;

        Button firstButton = topOptions.gameObject.GetComponentInChildren<Button>();
        firstButton.Select();
        Global.menuTop.HideItemDesc();
    }

    public void Level2()
    {
        Global.gameMenu.level = 2;

        topOptions.interactable = false;
        itemsContainer.interactable = true;

        Button firstButton = itemsContainer.gameObject.GetComponentInChildren<Button>();
        firstButton.Select();
        Global.menuTop.ShowItemDesc();
    }

    //Button events
    public void ItemHover(BaseEventData baseEvent)
    {
        Global.gameMenu.BaseButtonHover(baseEvent);

        GameObject button = baseEvent.selectedObject;
        string itemID = button.GetComponentInChildren<Text>().text;
        Item item = Global.items.Find(x => x.Name == itemID);

        Global.menuTop.SetItemDesc(item.Description);
        Global.menuTop.ShowItemDesc();

        selectedItem = item;
    }

    public void ItemClick(BaseEventData baseEvent)
    {
        Global.gameMenu.heart.SetActive(false);
        Global.menuBottom.ToggleButtons(true);
        GameObject firstButton = Global.menuBottom.GetFirst();
        firstButton.GetComponent<Button>().Select();
    }
}
