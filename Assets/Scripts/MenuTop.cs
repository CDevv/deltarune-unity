﻿/* 
(Attached to MenuTop)
The logic for the top Menu, which has buttons to open certain menus.
*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Luminosity.IO;

public class MenuTop : MonoBehaviour
{
    public GameObject menuUI;
    public GameObject menuBottom;
    [HideInInspector]
    public static bool isOpen = false;
    public static bool enableMenu = true;
    Animator anim;
    private Component[] buttons;
    public bool langIsJapanese;
    public Image description;
    List<Sprite> menuDesc = new List<Sprite>();
    public GameObject itemDesc;


    void Start()
    {
        anim = gameObject.GetComponent<Animator>();

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
        if (InputManager.GetKeyDown(Global.config.keyMenu) && enableMenu == true)
        {
            if (Global.gameMenu.level == 0)
            {
                Global.gameMenu.ToggleHeartAnim(false);

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

                Global.menuBottom.UpdateMenu();
            }


        }

        if (InputManager.GetButtonDown("ReturnMenu"))
        {
            if (Global.gameMenu.level == 1)
            {
                Global.gameMenu.level -= 1;
                menuUI.SetActive(false);

                foreach (Button button in buttons)
                {
                    button.interactable = true;
                    if (button.gameObject.name == Global.gameMenu.currentPage)
                    {
                        button.Select();
                    }
                }

                
            }
            else if (Global.gameMenu.level == 2)
            {
                switch (Global.gameMenu.currentPage)
                {
                    default:
                        break;
                    case "Items":
                        Global.gameMenu.itemsPage.Level1();
                        break;
                    case "Equipment":
                        Global.gameMenu.ToggleHeartAnim(true);
                        Global.gameMenu.equipmentPage.Level1();
                        break;
                }
            }
            else if (Global.gameMenu.level == 3)
            {
                switch (Global.gameMenu.currentPage)
                {
                    default:
                        break;
                    case "Equipment":
                        Global.gameMenu.equipmentPage.Level2();
                        break;
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
        Global.gameMenu.Refresh();
        Global.gameMenu.ChangePage(name);
        Global.gameMenu.level = 1;

        foreach (Button button in buttons)
        {
            button.interactable = false;
        }
    }

    public void ShowItemDesc()
    {
        foreach (Transform item in transform)
        {
            item.gameObject.SetActive(false);
        }
        itemDesc.SetActive(true);
    }

    public void HideItemDesc()
    {
        foreach (Transform item in transform)
        {
            item.gameObject.SetActive(true);
        }
        itemDesc.SetActive(false);
    }

    public void SetItemDesc(string desc)
    {
        itemDesc.GetComponentInChildren<Text>().text = desc;
    }

    public void ReturnToTop()
    {
        Global.gameMenu.level = 0;
        menuUI.SetActive(false);

        foreach (Button button in buttons)
        {
            button.interactable = true;
            if (button.gameObject.name == Global.gameMenu.currentPage)
            {
                button.Select();
            }
        }
    }
}
