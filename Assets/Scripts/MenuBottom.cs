/*
(Attached to MenuBottom)
Displays the characters on the bottom menu by reading the json, putting them in a list and checking if they are "inParty"
For now, it desializes the Json each frame the menu is up, which takes a bit of performance.
*/
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class MenuBottom : MonoBehaviour
{
    Animator anim;
    private List<Character> characterList;
    public GameObject characterMenu;
    public GameMenu gameMenu;
    List<Transform> childrensTransforms;

    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        // I need to get rid of animators and make my own probably
        childrensTransforms = new List<Transform>();

        // Read the json and put Characters in a list
        string json = Resources.Load<TextAsset>("Json/chara").text;
        characterList = JsonConvert.DeserializeObject<List<Character>>(json);
    }

    void Update()
    {
        
    }

    public void UpdateMenu()
    {
        string json = Resources.Load<TextAsset>("Json/chara").text;
        characterList = JsonConvert.DeserializeObject<List<Character>>(json);

        anim.SetBool("IsOpen", MenuTop.isOpen);

        if (MenuTop.isOpen)
        {
            foreach (Character chara in characterList)
            {
                string menuName = chara.Name + "_menu";
                Transform menuTransform = gameObject.transform.Find(menuName);

                if (chara.InParty) // If the character is in party
                {
                    if (menuTransform == null) // And is not being currently displayed, Instantiate a new characterMenu
                    {
                        GameObject newMenu = Instantiate(characterMenu, gameObject.transform);
                        newMenu.name = menuName;
                        MenuCharaInfo menuCharaInfo = newMenu.gameObject.GetComponent<MenuCharaInfo>();
                        menuCharaInfo.character = chara;

                        UnityAction<BaseEventData> callSelect = new UnityAction<BaseEventData>(OnCharSelect);
                        UnityAction<BaseEventData> callDeselect = new UnityAction<BaseEventData>(OnCharDeselect);
                        UnityAction<BaseEventData> callClick = new UnityAction<BaseEventData>(OnCharClick);
                        gameMenu.AddEventToButton(newMenu, callSelect, EventTriggerType.Select);
                        gameMenu.AddEventToButton(newMenu, callDeselect, EventTriggerType.Deselect);
                        gameMenu.AddEventToButton(newMenu, callClick, EventTriggerType.Submit);
                    }
                    else
                    { // Else, just update the infos.
                        MenuCharaInfo menuCharaInfo = menuTransform.gameObject.GetComponent<MenuCharaInfo>();
                        menuCharaInfo.character = chara;
                    }


                }
                else if (menuTransform != null)
                { // If not in party, and the characterMenu exists
                    Destroy(menuTransform.gameObject);
                }
            }
        }
    }

    public void OnTransformChildrenChanged() // Update the characterMenu's positions when their number has changed.
    {
        childrensTransforms.Clear();
        foreach (Transform child in transform)
        {
            childrensTransforms.Add(child);
        }

        switch (gameObject.transform.childCount)
        {
            case 1:
                childrensTransforms[0].transform.localPosition = new Vector3 (0, 32, 0);
                break;

            case 2:
                childrensTransforms[0].transform.localPosition = new Vector3 (-107, 32, 0);
                childrensTransforms[1].transform.localPosition = new Vector3 (107, 32, 0);
                break;

            case 3:
                childrensTransforms[0].transform.localPosition = new Vector3 (-214, 32, 0);
                childrensTransforms[1].transform.localPosition = new Vector3 (0, 32, 0);
                childrensTransforms[2].transform.localPosition = new Vector3 (214, 32, 0);
                break;
        }
    }

    //my methods
    public void ToggleButtons(bool value)
    {
        Button[] buttons = transform.gameObject.GetComponentsInChildren<Button>();
        foreach (Button btn in buttons)
        {
            btn.interactable = value;
        }
    }

    public GameObject GetFirst()
    {
        Button button = transform.gameObject.GetComponentInChildren<Button>();
        return button.gameObject;
    }

    public void OnCharSelect(BaseEventData baseEvent)
    {
        GameObject gameObject = baseEvent.selectedObject;
        Character character = gameObject.GetComponent<MenuCharaInfo>().character;
        Image image = gameObject.GetComponent<Image>();
        Color color = new Color(character.Color[0], character.Color[1], character.Color[2]);
        image.color = color;
    }

    public void OnCharDeselect(BaseEventData baseEvent)
    {
        GameObject gameObject = baseEvent.selectedObject;
        Image image = gameObject.GetComponent<Image>();
        Color color = new Color((float)0.2, (float)0.1254902, (float)0.2);
        image.color = color;
    }

    public void OnCharClick(BaseEventData baseEvent)
    {
        GameObject gameObject = baseEvent.selectedObject;
        Character character = gameObject.GetComponent<MenuCharaInfo>().character;
        Debug.Log("Used " + gameMenu.selectedItem.Name);

        character.AddHP(gameMenu.selectedItem.Effect);
        gameMenu.character.RemoveItem(gameMenu.selectedItem.Name);

        //SaveCharacters();

        UpdateMenu();
        gameMenu.Refresh();
        gameMenu.PageItems();

        gameMenu.OnSelectItemsAction();
        gameMenu.level--;

        ToggleButtons(false);
    }

    public void SaveCharacters()
    {
        //@"..\Resources\Json\chara.json"
        Debug.Log(Application.dataPath + "\\Resources\\Json\\chara.json");
        File.WriteAllText(Application.dataPath + "\\Resources\\Json\\chara.json", JsonConvert.SerializeObject(characterList));
    }
}
