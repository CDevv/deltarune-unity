using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollableContext : MonoBehaviour
{
    VerticalLayoutGroup content;
    IconTextOption[] buttons;
    GameObject side;
    RectTransform bar;

    int maxItems;
    List<string> items = new List<string>();

    int selectedItem = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetupElement()
    {
        content = transform.GetComponentInChildren<VerticalLayoutGroup>();
        buttons = content.transform.GetComponentsInChildren<IconTextOption>();
        side = transform.Find("Side").gameObject;
        bar = side.GetComponentInChildren<RectTransform>();

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].Setup();
        }
    }

    public void Refresh(int max, List<string> strings)
    {
        selectedItem = 0;
        maxItems = max;
        items = strings;
        for (int i = 0; i < buttons.Length; i++)
        {
            if (items[i] == "")
            {
                buttons[i].DefaultHiddenImg(true);
                buttons[i].ToggleColor(false);
                buttons[i].SetText("(Nothing.)");
            }
            else
            {
                buttons[i].ToggleColor(true);
                buttons[i].DefaultHiddenImg(false);
                buttons[i].SetText(items[i]);
            }
            
        }

        Rect barRect = bar.rect;
        barRect.height = side.GetComponent<RectTransform>().rect.height / maxItems;
        bar.sizeDelta = new Vector2(6, barRect.height);
    }

    public IconTextOption GetFirst()
    {
        return buttons[0];
    }
}
