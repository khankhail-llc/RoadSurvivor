using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UIDebugger : MonoBehaviour
{
    void Update()
    {
        // Detect left mouse click
        if (Input.GetMouseButtonDown(0))
        {
            // Check if EventSystem exists
            if (EventSystem.current == null)
            {
                Debug.LogError("[UIDebugger] No EventSystem found in scene!");
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
                Debug.Log($"<b>[UIDebugger] Click detected! Hit {results.Count} UI objects:</b>");
                foreach (RaycastResult result in results)
                {
                    Debug.Log($"   ➥ <b>{result.gameObject.name}</b> (Depth: {result.depth}, Layer: {result.gameObject.layer})");
                }
            }
            else
            {
                Debug.Log("[UIDebugger] Clicked, but hit NO UI elements.");
            }
        }
    }
}
