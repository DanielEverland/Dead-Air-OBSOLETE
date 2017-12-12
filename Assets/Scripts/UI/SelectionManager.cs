using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour {

    [SerializeField]
    private RectTransform image;

    private Vector2 downPosition;

	private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            downPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            Vector2 min = Vector2.Min(downPosition, mouseWorldPos);
            Vector2 max = Vector2.Max(downPosition, mouseWorldPos);

            Vector3 size = max - min;

            Rect worldRect = new Rect(min, size);

            HandleInput(worldRect);
            RenderUI(worldRect);
        }
        else
        {
            RenderUI(Rect.zero);
        }
    }
    private void HandleInput(Rect rect)
    {
        Debug.Log(MapData.EntityQuadtree.Query(rect).Count);
    }
    private void RenderUI(Rect rect)
    {
        Vector2 screenPointCenter = Camera.main.WorldToScreenPoint(rect.center);
        Vector2 minScreen = Camera.main.WorldToScreenPoint(rect.min);
        Vector2 maxScreen = Camera.main.WorldToScreenPoint(rect.max);

        image.transform.position = screenPointCenter;
        image.sizeDelta = maxScreen - minScreen;
    }
}
