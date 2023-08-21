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
using UnityEngine.TextCore.Text;

public class MenuBottom : MonoBehaviour
{
    Animator anim;
    private List<Character> characterList;
    public GameObject characterMenu;
    public GameObject menuUI;
    private GameMenu gameMenu;
    List<Transform> childrensTransforms;

    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        // I need to get rid of animators and make my own probably
        childrensTransforms = new List<Transform>();
    }

    void Update()
    {
        
    }

    public void UpdateMenu()
    {

        anim.SetBool("IsOpen", MenuTop.isOpen);

        if (MenuTop.isOpen)
        {
            foreach (Character chara in Global.characterList)
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

                        menuCharaInfo.UpdateElement();

                        UnityAction<BaseEventData> callSelect = new UnityAction<BaseEventData>(OnCharSelect);
                        UnityAction<BaseEventData> callDeselect = new UnityAction<BaseEventData>(OnCharDeselect);
                        UnityAction<BaseEventData> callClick = new UnityAction<BaseEventData>(OnCharClick);

                        Global.gameMenu.AddEventToButton(newMenu, callSelect, EventTriggerType.Select);
                        Global.gameMenu.AddEventToButton(newMenu, callDeselect, EventTriggerType.Deselect);
                        Global.gameMenu.AddEventToButton(newMenu, callClick, EventTriggerType.Submit);
                    }
                    else
                    { // Else, just update the infos.
                        MenuCharaInfo menuCharaInfo = menuTransform.gameObject.GetComponent<MenuCharaInfo>();
                        menuCharaInfo.character = chara;
                        menuCharaInfo.UpdateElement();
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
        MenuCharaInfo menuCharaInfo = gameObject.GetComponent<MenuCharaInfo>();
        Character character = gameObject.GetComponent<MenuCharaInfo>().character;
        Image image = gameObject.GetComponent<Image>();
        Color color = new Color(character.Color[0], character.Color[1], character.Color[2]);
        Sprite sprite = Resources.Load<Sprite>("Sprites/Ui/head" + character.Name + "7");
        image.color = color;
        menuCharaInfo.playerImg.sprite = sprite;
    }

    public void OnCharDeselect(BaseEventData baseEvent)
    {
        GameObject gameObject = baseEvent.selectedObject;
        MenuCharaInfo menuCharaInfo = gameObject.GetComponent<MenuCharaInfo>();
        Character character = menuCharaInfo.character;
        Image image = gameObject.GetComponent<Image>();
        Color color = new Color((float)0.2, (float)0.1254902, (float)0.2);
        Sprite sprite = Resources.Load<Sprite>("Sprites/Ui/head" + character.Name + "0");
        image.color = color;
        menuCharaInfo.playerImg.sprite = sprite;
    }

    public void OnCharClick(BaseEventData baseEvent)
    {
        GameObject gameObject = baseEvent.selectedObject;
        Character character = gameObject.GetComponent<MenuCharaInfo>().character;
        character = Global.characterList.Find(x => x.Name == character.Name);
        Debug.Log("Used " + Global.gameMenu.itemsPage.selectedItem.Name);

        character.AddHP(Global.gameMenu.itemsPage.selectedItem.Effect);
        Global.mainChar.RemoveItem(Global.gameMenu.itemsPage.selectedItem.Name);

        //SaveCharacters();
        Character newChar = character;
        Global.characterList.Remove(character);
        Global.characterList.Add(newChar);

        UpdateMenu();
        Global.gameMenu.itemsPage.Refresh();

        ToggleButtons(false);

        Global.gameMenu.heart.SetActive(true);
        Global.gameMenu.itemsPage.Level1();
    }

    public void SaveCharacters()
    {
        //@"..\Resources\Json\chara.json"
        Debug.Log(Application.dataPath + "\\Resources\\Json\\chara.json");
        File.WriteAllText(Application.dataPath + "\\Resources\\Json\\chara.json", JsonConvert.SerializeObject(characterList));
    }
}
