using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FixButtonRaycasts : MonoBehaviour
{
    void Awake()
    {
        FixAll();
    }

    void Start()
    {
        // Run again in Start just in case
        FixAll();
    }

    public void FixAll()
    {
        Button[] buttons = FindObjectsOfType<Button>(true);
        int count = 0;

        foreach (var btn in buttons)
        {
            // 1. Fix Legacy Text
            Text[] texts = btn.GetComponentsInChildren<Text>(true);
            foreach (var t in texts)
            {
                if (t.raycastTarget)
                {
                    t.raycastTarget = false;
                    count++;
                }
            }

            // 2. Fix TextMeshPro
            TextMeshProUGUI[] tmps = btn.GetComponentsInChildren<TextMeshProUGUI>(true);
            foreach (var tmp in tmps)
            {
                if (tmp.raycastTarget)
                {
                    tmp.raycastTarget = false;
                    count++;
                }
            }
        }

        if (count > 0)
        {
            Debug.Log($"<b>[UI FIXER]</b> fixed {count} text objects blocking buttons!");
        }
    }
}
