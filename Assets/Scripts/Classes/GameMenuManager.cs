using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMenuManager
{
    public GameObject UI;
    public Dictionary<string, GameObject> pages = new Dictionary<string, GameObject>();

    public string currentPage = "Items";
    public int level = 0;
    public GameObject heart;
    public GameObject pageObj;

    public GameMenuManager(GameObject gameObject)
    {
        this.UI = gameObject;
        this.heart = gameObject.transform.Find("Heart").gameObject;
        this.pages["Items"] = gameObject.transform.Find("page-Items").gameObject;
        this.pages["Equipment"] = gameObject.transform.Find("page-Equipment").gameObject;
        this.pages["Stats"] = gameObject.transform.Find("page-Stats").gameObject;
        this.pages["Settings"] = gameObject.transform.Find("page-Settings").gameObject;
    }

    public void ChangePage(string name)
    {
        GameObject prevPage = this.pages[currentPage];
        prevPage.SetActive(false);
        this.currentPage = name;
        GameObject newPage = this.pages[currentPage];
        newPage.SetActive(true);

        this.pageObj = newPage;
        PageItems();
    }

    public void PageItems()
    {
        Button firstButton = pageObj.GetComponentInChildren<Button>();
        firstButton.Select();
    }
}
