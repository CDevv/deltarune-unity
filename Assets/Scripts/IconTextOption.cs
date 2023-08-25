using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconTextOption : MonoBehaviour
{
    Button button;
    public Text text;
    public Image img;

    Color disabledColor = new Color(0.7843137f, 0.7843137f, 0.7843137f);

    bool imgHidden = false;
    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Setup()
    {
        button = GetComponent<Button>();
    }

    public void SetText(string s)
    {
        text.text = s;
    }

    public string GetText()
    {
        return text.text;
    }

    public void ToggleImageActive(bool value)
    {
        if (!imgHidden)
        {
            img.gameObject.SetActive(value);
        }
    }

    public void SelectBtn()
    {
        button.Select();
    }

    public void ToggleColor(bool value)
    {
        ColorBlock colorBlock = button.colors;
        if (value)
        {
            colorBlock.normalColor = Color.white;
            colorBlock.disabledColor = Color.white;
        }
        else
        {
            colorBlock.normalColor = disabledColor;
            colorBlock.disabledColor = disabledColor;
        }
        button.colors = colorBlock;
    }

    public void DefaultHiddenImg(bool value)
    {
        if (value)
        {
            imgHidden = true;
            img.gameObject.SetActive(false);
        }
        else
        {
            imgHidden = false;
            img.gameObject.SetActive(true);
        }
    }
}
