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

    public int selectedItem = 0;
    int rangeStart, rangeEnd = 0;
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

        rangeStart = 0;
        rangeEnd = buttons.Length - 1;
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

            buttons[i].value = i;
        }

        Rect barRect = bar.rect;
        barRect.height = side.GetComponent<RectTransform>().rect.height / maxItems;
        bar.sizeDelta = new Vector2(6, barRect.height);
    }

    public IconTextOption GetFirst()
    {
        return buttons[0];
    }

    public void ScrollUp()
    {
        if (selectedItem > 0)
        {
            selectedItem--;
            rangeEnd--; rangeStart--;
            RefreshButtons(rangeStart, rangeEnd);
        }
    }

    public void ScrollDown()
    {
        if (selectedItem < maxItems - 1)
        {
            selectedItem++;
            rangeEnd++; rangeStart++;
            RefreshButtons(rangeStart, rangeEnd);
        }
    }

    private void RefreshButtons(int start, int end)
    {
        Debug.Log("start: " + start);
        for (int i = start; i <= end; i++)
        {
            int j = Mathf.Abs((end - i) - 4);
            Debug.Log("j: " + j);
            if (items[i] == "")
            {
                buttons[j].DefaultHiddenImg(true);
                buttons[j].ToggleColor(false);
                buttons[j].SetText("(Nothing.)");
            }
            else
            {
                buttons[j].ToggleColor(true);
                buttons[j].DefaultHiddenImg(false);
                buttons[j].SetText(items[i]);
            }

            buttons[j].value = i;
        }
    }
}
