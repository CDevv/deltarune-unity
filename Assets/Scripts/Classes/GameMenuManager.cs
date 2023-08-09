using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenuManager
{
    public GameObject UI;
    public Dictionary<string, GameObject> pages = new Dictionary<string, GameObject>();

    public string currentPage = "Items";

    public GameMenuManager(GameObject gameObject)
    {
        this.UI = gameObject;
        this.pages["Items"] = gameObject.transform.Find("page-Items").gameObject;
    }

    public void ChangePage(string name)
    {
        GameObject prevPage = this.pages[currentPage];
        prevPage.SetActive(false);
        this.currentPage = name;
        GameObject newPage = this.pages[currentPage];
        newPage.SetActive(true);
    }
}
