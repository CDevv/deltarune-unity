/* 
(Attached to MenuTop)
The logic for the top Menu, which has buttons to open certain menus.
*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuTop : MonoBehaviour
{
    public GameObject menuUI;
    [HideInInspector]
    public static bool isOpen = false;
    public static bool enableMenu = true;
    Animator anim;
    private Component[] buttons;
    public bool langIsJapanese;
    public Image description;
    List<Sprite> menuDesc = new List<Sprite>();

    GameMenuManager menuManager;

    void Start()
    {
        anim = gameObject.GetComponent<Animator>();

        menuManager = new GameMenuManager(menuUI);

        for (int i = 0; i < 5; i++)
        {
            if (langIsJapanese)
                menuDesc.Add(Resources.Load<Sprite>("Sprites/Ui/ja_darkmenudesc" + i.ToString()));
            else
                menuDesc.Add(Resources.Load<Sprite>("Sprites/Ui/darkmenudesc" + i.ToString()));
        }

    }
    void Update()
    {
        if (Input.GetButtonDown("Menu") && enableMenu == true)
        {
            if (menuManager.level == 0)
            {
                isOpen = !isOpen;

                anim.SetBool("IsOpen", isOpen);

                buttons = GetComponentsInChildren<Button>();

                foreach (Button button in buttons)
                {
                    button.interactable = isOpen;
                }
                if (isOpen)
                {
                    Button firstButton = GetComponentInChildren<Button>();

                    firstButton.Select();
                    firstButton.OnSelect(null);
                }
            }


        }

        if (Input.GetButtonDown("ReturnMenu"))
        {
            if (menuManager.level != 0)
            {
                menuManager.level -= 1;
                menuUI.SetActive(false);

                foreach (Button button in buttons)
                {
                    button.interactable = true;
                    if (button.gameObject.name == menuManager.currentPage)
                    {
                        button.Select();
                    }
                }
            }
        }
    }
    public void HoverItems()
    {
        description.sprite = menuDesc[0];
    }
    public void SelectItems()
    {
        Debug.Log("Selected Items!");
        MenuPage("Items");
    }

    public void HoverEquipment()
    {
        description.sprite = menuDesc[1];
    }
    public void SelectEquipment()
    {
        Debug.Log("Selected Equipment!");
        MenuPage("Equipment");
    }

    public void HoverStats()
    {
        description.sprite = menuDesc[3];
    }
    public void SelectStats()
    {
        Debug.Log("Selected Stats!");
        MenuPage("Stats");
    }

    public void HoverSettings()
    {
        description.sprite = menuDesc[4];
    }
    public void SelectSettings()
    {
        Debug.Log("Selected Settings!");
        MenuPage("Settings");
    }

    //more methods
    private void MenuPage(string name)
    {
        menuUI.SetActive(true);
        menuManager.ChangePage(name);
        menuManager.level = 1;

        foreach (Button button in buttons)
        {
            button.interactable = false;
        }
    }
}
