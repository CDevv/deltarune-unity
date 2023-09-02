using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.EventSystems;

public class Global : MonoBehaviour
{
    public static Character mainChar;
    public static List<Character> characterList;
    public static List<Item> items;
    public static GameConfig config;

    public CustomInput customInput;
    public GameObject objMusic;
    public GameObject objTop;
    public GameObject objBottom;
    public GameObject objMenu;

    public static AudioSource bgMusic;
    public static MenuTop menuTop;
    public static MenuBottom menuBottom;
    public static GameMenu gameMenu;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Cor());

        string json = Resources.Load<TextAsset>("Json/config").text;
        config = JsonConvert.DeserializeObject<GameConfig>(json);

        json = Resources.Load<TextAsset>("Json/chara").text;
        characterList = JsonConvert.DeserializeObject<List<Character>>(json);
        mainChar = characterList.Find(x => x.Name == "Kris");

        string itemsString = Resources.Load<TextAsset>("Json/items").text;
        items = JsonConvert.DeserializeObject<List<Item>>(itemsString);

        bgMusic = objMusic.GetComponent<AudioSource>();

        menuTop = objTop.GetComponent<MenuTop>();
        menuBottom = objBottom.GetComponent<MenuBottom>();
        gameMenu = objMenu.GetComponent<GameMenu>();

        gameMenu.Setup();
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator Cor()
    {
        yield return null;
        //EventSystem.current.currentInputModule.inputOverride = customInput;
    }
}
