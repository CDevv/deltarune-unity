using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Linq;

public class StatsPage : MonoBehaviour
{
    CanvasGroup divChars;
    CanvasGroup divStats;

    HorizontalLayoutGroup charsContainer;
    Dictionary<string, Text> charText = new();
    Dictionary<string, Text> statsText = new Dictionary<string, Text>();

    GameObject charIcon;
    int childrenCount = 0;
    float[] charIconPos = { -140.75f, -170.75f, -200.75f };

    Character selectedChar;
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

        charsContainer = divChars.GetComponentInChildren<HorizontalLayoutGroup>();
        Dictionary<string, Text> charsDict = divChars.transform.GetComponentsInChildren<Text>().ToDictionary(x => x.transform.gameObject.name);
        charText = charsDict;

        Dictionary<string, Text> statsDict = divStats.transform.GetComponentsInChildren<Text>().ToDictionary(x => x.transform.parent.gameObject.name);
        statsText = statsDict;

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
        childrenCount = buttons.Count;
        charIcon = buttons[0];
    }

    //Levels
    public void OnPageOpen()
    {
        Global.gameMenu.ToggleHeartAnim(true);
        Level1();
        HeartPos();
    }

    public void Level1()
    {
        Global.gameMenu.level = 1;
        charIcon.GetComponentInChildren<Button>().Select();
    }

    //Button events
    public void EquipCharSelect(BaseEventData baseEvent)
    {
        GameObject button = baseEvent.selectedObject;
        RectTransform rect = Global.gameMenu.heart.GetComponent<RectTransform>();
        RectTransform optionRect = button.GetComponent<RectTransform>();

        Vector3 vector = new Vector3(optionRect.position.x, optionRect.position.y + 30);
        rect.transform.position = vector;

        //EquipCharRefresh(button.name);
        Character character = Global.characterList.Find(x => x.Name == button.name);

        charText["CurrentChar"].text = character.Name;
        charText["CharInfo"].text = character.Description;

        statsText["AttackStat"].text = $"Attack: {character.Stats.Attack}";
        statsText["DefenceStat"].text = $"Defence: {character.Stats.Defense}";
        statsText["MagicStat"].text = $"Magic: {character.Stats.Magic}";

        selectedChar = character;
    }

    //Extra methods
    public void HeartPos()
    {
        RectTransform rect = Global.gameMenu.heart.GetComponent<RectTransform>();
        Vector3 vector = new Vector3(charIconPos[childrenCount - 1], 106);
        rect.transform.localPosition = vector;
    }
}
