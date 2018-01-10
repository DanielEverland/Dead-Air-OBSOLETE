using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityCardManager : MonoBehaviour {

    [SerializeField]
    private SelectionManager selectionManager;
    [SerializeField]
    private EntityCard entityCardPrefab;

    private Dictionary<System.Type, List<System.Type>> typesToLoad = new Dictionary<System.Type, List<System.Type>>();
    private EntityCard card;

	private void Start()
    {
        card = Instantiate(entityCardPrefab);
        card.transform.SetParent(Canvas2D.Instance.transform, false);
        card.gameObject.SetActive(false);

        selectionManager.OnSelectionUpdated += OnSelectionUpdated;
    }
    private void OnSelectionUpdated(IEnumerable<Entity> selectedEntities)
    {
        if (selectedEntities.Count() == 0)
        {
            card.gameObject.SetActive(false);
        }
        else
        {
            InitializeData(selectedEntities);

            card.gameObject.SetActive(true);
        }
    }
    private void InitializeData(IEnumerable<Entity> selectedEntities)
    {
        EntityCard.Data data = new EntityCard.Data(selectedEntities);

        foreach (Entity entity in selectedEntities)
        {
            LoadObject(data, entity);
        }

        card.Initialize(data);
    }
    private void LoadObject(EntityCard.Data data, object obj)
    {
        System.Type objType = obj.GetType();

        PollType(objType);

        foreach (System.Type type in typesToLoad[objType])
        {
            Load(data, obj, type);
        }
    }
    private void PollType(System.Type type)
    {
        if (!typesToLoad.ContainsKey(type))
        {
            typesToLoad.Add(type, type.GetDerivedTypesOrdered());
            typesToLoad[type].Add(type);
        }
    }
    private void Load(EntityCard.Data data, object obj, System.Type type)
    {
        if (dataParsers.ContainsKey(type))
        {
            dataParsers[type].Invoke(data, obj);
        }
    }

    //------------IMPLEMENTATION NOTES------------//
    /* To implement a parser, simply compare the
     * data the object provides, with what is
     * stored inside the EntityCard.Data. If there
     * is a conflict, simply add a generic value.
     * 
     * For instance, if we're implementing a parser
     * that handles names, we obviously must ensure
     * that the name of all the given objects are
     * the same. Therefore such a parser would
     * simply have to tell if the name inside the
     * EntityData.Data is the same as the object
     * and if not, then it should set the data
     * to be "Various", or something along
     * those lines, i.e. something generic
     */

    private Dictionary<System.Type, Action<EntityCard.Data, object>> dataParsers = new Dictionary<Type, Action<EntityCard.Data, object>>()
    {
        { typeof(object), ObjectParser },
        { typeof(Entity), EntityParser },
    };

    private static void ObjectParser(EntityCard.Data data, object obj)
    {
        data.EntityCount++;
    }
    private static void EntityParser(EntityCard.Data data, object obj)
    {
        Entity entity = obj as Entity;

        data.Name = (data.Name == entity.Name || !data.HasData(EntityCard.DataTypes.Name)) ? entity.Name : "Various";
    }
}
