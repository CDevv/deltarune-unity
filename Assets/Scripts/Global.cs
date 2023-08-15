using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class Global : MonoBehaviour
{
    public static Character mainChar;
    public static List<Character> characterList;
    public static List<Item> items;

    public GameObject objTop;
    public GameObject objBottom;
    public GameObject objMenu;

    public static MenuTop menuTop;
    public static MenuBottom menuBottom;
    public static GameMenu gameMenu;
    // Start is called before the first frame update
    void Start()
    {
        menuTop = objTop.GetComponent<MenuTop>();
        menuBottom = objBottom.GetComponent<MenuBottom>();
        gameMenu = objMenu.GetComponent<GameMenu>();

        gameMenu.Setup();

        string json = Resources.Load<TextAsset>("Json/chara").text;
        characterList = JsonConvert.DeserializeObject<List<Character>>(json);
        mainChar = characterList.Find(x => x.Name == "Kris");

        string itemsString = Resources.Load<TextAsset>("Json/items").text;
        items = JsonConvert.DeserializeObject<List<Item>>(itemsString);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
