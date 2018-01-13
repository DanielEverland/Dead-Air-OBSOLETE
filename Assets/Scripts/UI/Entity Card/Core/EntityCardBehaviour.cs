using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class EntityCardBehaviour : MonoBehaviour, IEntityCardBehaviour {

    [SerializeField]
    private EntityCard.DataTypes _dataType;
    [SerializeField]
    private bool _disableIfEmpty = true;
    [SerializeField]
    private OnReceivedDataEvent OnReceviedData;

    public EntityCard.DataTypes DataType { get { return _dataType; } }

    protected abstract void InitializeData(EntityCard.Data data);

    public void Initialize(EntityCard.Data data)
    {
        if (data.HasData(DataType))
        {
            Enable();

            InitializeData(data);
        }
        else if(_disableIfEmpty)
        {
            Disable();
        }

        if (OnReceviedData != null)
            OnReceviedData.Invoke(data);
    }
    public void Enable()
    {
        gameObject.SetActive(true);
    }
    public void Disable()
    {
        gameObject.SetActive(false);
    }
    [System.Serializable]
    private class OnReceivedDataEvent : UnityEvent<EntityCard.Data> { }
}
