using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EquipSlotOption : MonoBehaviour
{
    public Image imgPrimary;
    public Image imgSecondary;

    Button button;
    Text text;

    Color disabledColor = new Color(0.7843137f, 0.7843137f, 0.7843137f);
    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        text = GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetupElement()
    {
        button = GetComponent<Button>();
        text = GetComponentInChildren<Text>();
    }

    public void UpdateElement(string itemName)
    {
        if (itemName == "")
        {
            text.text = "(Nothing.)";
            ColorBlock colorBlock = button.colors;
            colorBlock.normalColor = disabledColor;
            colorBlock.disabledColor = disabledColor;
            button.colors = colorBlock;
            imgPrimary.gameObject.SetActive(false);
        }
        else
        {
            text.text = itemName;
            ColorBlock colorBlock = button.colors;
            colorBlock.normalColor = Color.white;
            colorBlock.disabledColor = Color.white;
            button.colors = colorBlock;
            imgPrimary.gameObject.SetActive(true);
        }
    }

    public void SetText(string s)
    {
        text.text = s;
    }

    public string GetText()
    {
        return text.text;
    }
}
