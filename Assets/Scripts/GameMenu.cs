using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
    public GameObject ui;
    public Dictionary<string, GameObject> pages = new();

    public string currentPage = "Items";
    public int level = 0;
    public GameObject heart;
    public GameObject pageObj;
    // Start is called before the first frame update
    void Start()
    {
        //this.heart = ui.transform.Find("Heart").gameObject;
        this.pages["Items"] = ui.transform.Find("page-Items").gameObject;
        this.pages["Equipment"] = ui.transform.Find("page-Equipment").gameObject;
        this.pages["Stats"] = ui.transform.Find("page-Stats").gameObject;
        this.pages["Settings"] = ui.transform.Find("page-Settings").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ButtonHover(GameObject button)
    {
        Debug.Log("Selected button");
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
        PageItems();
    }

    public void PageItems()
    {
        heart.SetActive(true);
        Button firstButton = pageObj.GetComponentInChildren<Button>();
        firstButton.Select();
    }
}
