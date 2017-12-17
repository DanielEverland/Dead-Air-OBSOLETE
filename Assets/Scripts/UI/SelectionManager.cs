using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour {

    [SerializeField]
    private RectTransform image;

    private Dictionary<Entity, SelectionHandler> selectedEntities = new Dictionary<Entity, SelectionHandler>();
    private Vector2 downPosition;

	private void Update()
    {
        CallSelectionHandlers();

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
    private void CallSelectionHandlers()
    {
        foreach (SelectionHandler handler in selectedEntities.Values)
        {
            handler.Update();
        }
    }
    private void HandleInput(Rect rect)
    {
        List<Entity> entitiesSelectedThisFrame = MapData.EntityQuadtree.Query(rect);
        List<Entity> newEntities = new List<Entity>(entitiesSelectedThisFrame.Except(selectedEntities.Keys));
        List<Entity> entitiesToRemove = new List<Entity>(selectedEntities.Keys.Except(entitiesSelectedThisFrame));

        Debug.Log(entitiesSelectedThisFrame.Count);

        foreach (Entity entity in entitiesToRemove)
        {
            //RemoveEntity(entity);
        }

        foreach (Entity entity in newEntities)
        {
            AddEntity(entity);
        }
    }
    private void RenderUI(Rect rect)
    {
        Vector2 screenPointCenter = Camera.main.WorldToScreenPoint(rect.center);
        Vector2 minScreen = Camera.main.WorldToScreenPoint(rect.min);
        Vector2 maxScreen = Camera.main.WorldToScreenPoint(rect.max);

        image.transform.position = screenPointCenter;
        image.sizeDelta = maxScreen - minScreen;
    }
    private void AddEntity(Entity entity)
    {
        if (selectedEntities.ContainsKey(entity))
            return;

        selectedEntities.Add(entity, new SelectionHandler(entity));
    }
    private void RemoveEntity(Entity entity)
    {
        selectedEntities[entity].Remove();

        selectedEntities.Remove(entity);
    }

    private class SelectionHandler
    {
        private SelectionHandler() { }

        public SelectionHandler(Entity entity)
        {
            uiMarker = PrefabPool.GetObject(PREFAB_NAME).GetComponent<RectTransform>();
            uiMarker.transform.SetParent(Canvas2D.Instance.transform);

            this.entity = entity;
        }

        private const string PREFAB_NAME = "SelectionMarker";

        private readonly RectTransform uiMarker;
        private readonly Entity entity;

        public void Update()
        {
            uiMarker.transform.position = Camera.main.WorldToScreenPoint(entity.transform.position);
        }
        public void Remove()
        {
            PrefabPool.ReturnObject(uiMarker.gameObject);
        }
    }
}
