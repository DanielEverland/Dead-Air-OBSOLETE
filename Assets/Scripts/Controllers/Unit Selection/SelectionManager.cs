using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour {

    [SerializeField]
    private RectTransform image;

    public event System.Action OnSelectionUpdated;
    public IEnumerable<Entity> SelectedEntities { get { return selectedEntities.Keys; } }

    private Dictionary<Entity, SelectionHandler> selectedEntities = new Dictionary<Entity, SelectionHandler>();
    private Vector2 downPosition;

    private bool isDirty = false;

	private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            downPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            RightClick(Camera.main.ScreenToWorldPoint(Input.mousePosition).ToCellPosition());
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
        
        PollEvents();
    }
    private void HandleInput(Rect rect)
    {
        List<Entity> entitiesSelectedThisFrame = MapData.EntityQuadtree.Query(rect);
        List<Entity> newEntities = new List<Entity>(entitiesSelectedThisFrame.Except(selectedEntities.Keys));
        List<Entity> entitiesToRemove = new List<Entity>(selectedEntities.Keys.Except(entitiesSelectedThisFrame));
        
        foreach (Entity entity in entitiesToRemove)
        {
            RemoveEntity(entity);
        }

        foreach (Entity entity in newEntities)
        {
            AddEntity(entity);
        }
    }
    private void RightClick(Vector2 position)
    {
        List<WorkableEntity> unitsToMove = new List<WorkableEntity>(selectedEntities.Keys.Where(x => x is WorkableEntity).Select(x => x as WorkableEntity));
        
        if (unitsToMove.Count == 0)
            return;

        Queue<Vector2> unitPositions = new Queue<Vector2>(UnitFormation.GetFormation<SquareFormation>(position, unitsToMove.Count));

        for (int i = 0; i < unitsToMove.Count; i++)
        {
            unitsToMove[i].AssignWork(new MovePointWork(unitPositions.Dequeue()));
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

        SelectionUpdated();
    }
    private void RemoveEntity(Entity entity)
    {
        selectedEntities[entity].Remove();

        selectedEntities.Remove(entity);

        SelectionUpdated();
    }
    private void Clear()
    {
        foreach (SelectionHandler handler in selectedEntities.Values)
        {
            handler.Remove();
        }

        selectedEntities.Clear();

        SelectionUpdated();
    }
    private void SelectionUpdated()
    {
        isDirty = true;
    }
    private void PollEvents()
    {
        if (isDirty)
            RaiseEvent(OnSelectionUpdated);

        isDirty = false;
    }
    private void RaiseEvent(System.Action action)
    {
        if (action != null)
            action.Invoke();
    }

    private class SelectionHandler
    {
        private SelectionHandler() { }

        public SelectionHandler(Entity entity)
        {
            uiMarker = PrefabPool.GetObject<SelectionIndicator>(PREFAB_NAME);
            uiMarker.transform.SetParent(Canvas2D.Instance.transform);
            uiMarker.Select(entity);

            this.entity = entity;
        }

        private const string PREFAB_NAME = "SelectionMarker";

        private readonly SelectionIndicator uiMarker;
        private readonly Entity entity;
        
        public void Remove()
        {
            uiMarker.Clear();
            PrefabPool.ReturnObject(uiMarker.gameObject);
        }
    }
}
