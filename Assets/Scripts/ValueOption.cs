using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ValueOption : MonoBehaviour
{
    public int value;
    public string strValue;
    public string type;

    Button button;
    public Text title;
    public Text valueText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Setup()
    {
        button = GetComponent<Button>();
    }

    public void SetType(string s)
    {
        this.type = s;
    }

    public void SetValue(int value)
    {
        this.value = value;
        switch (type)
        {
            default:
                break;
            case "num":
                valueText.text = value.ToString();
                break;
            case "percent":
                valueText.text = value.ToString() + "%";
                break;
        }
    }

    public void SetString(string s)
    {
        this.strValue = s;
        if (type == "str")
        {
            valueText.text = s;
        }
        else if (type == "bool")
        {
            if (s == "True")
            {
                valueText.text = "ON";
            }
            else
            {
                valueText.text = "OFF";
            }
        }
    }

    public void SelectBtn()
    {
        button.Select();
    }
}
