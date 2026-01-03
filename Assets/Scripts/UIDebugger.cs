using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIDebugger : MonoBehaviour
{
    void Update()
    {
        // Detect left mouse click
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current == null)
            {
                Debug.LogError("[UIDebugger] No EventSystem found!");
                return;
            }

            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            if (results.Count > 0)
            {
                Debug.Log($"<b>[UIDebugger] Hit {results.Count} UI objects:</b>");
                foreach (RaycastResult result in results)
                {
                    GameObject go = result.gameObject;
                    string info = $"   ➥ <b>{go.name}</b> (Depth: {result.depth}, Layer: {go.layer})";
                    
                    // Check for Button
                    Button btn = go.GetComponent<Button>();
                    if (btn == null) btn = go.GetComponentInParent<Button>(); // Check parent too
                    
                    if (btn != null)
                    {
                        info += $" <color=green>[BUTTON FOUND]</color> Interactable: {btn.interactable}";
                        if (!btn.interactable) info += " <color=red>(DISABLED)</color>";
                    }
                    else
                    {
                        info += " [No Button]";
                    }
                    
                    Debug.Log(info);
                }
            }
            else
            {
                Debug.Log("[UIDebugger] Clicked but hit nothing.");
            }
        }
    }
}
