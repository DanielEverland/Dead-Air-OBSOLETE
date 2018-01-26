using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour {

    [SerializeField]
    private ushort _maxHealth = 100;

    public abstract string Name { get; }

    public float Health
    {
        get
        {
            return Mathf.Clamp(MaxHealth - _healthOffset, 0, float.MaxValue);
        }
        set
        {
            _healthOffset = Mathf.Clamp(MaxHealth - value, 0, MaxHealth);

            if(_healthOffset == MaxHealth)
            {
                Die();
            }
        }
    }

    public Vector2 CellPosition { get { return transform.position.ToCellPosition(); } }
    public int MaxHealth { get { return (int)_maxHealth; } }
    public byte Priority { get { return (byte)PriorityLevel; } }
    public Rect Rect { get { return new Rect(transform.position - (Vector3)Size / 2, Size); } }
    
    public virtual Vector2 Size { get { return Vector2.one; } }

    protected abstract EntityPriorityLevel PriorityLevel { get; }

    private Vector2 _oldPosition;
    private Dictionary<string, object> _data;
    private float _healthOffset;
    private bool _isDead;

    protected virtual void Awake()
    {

    }
    public void PollRegion()
    {
        RegionEntityManager.Poll(this);
    }
    public static T CreateEntity<T>() where T : Entity
    {
        return CreateEntity<T>(typeof(T).Name);
    }
    public static T CreateEntity<T>(string key) where T : Entity
    {
        GameObject obj = EntityObjectPool.GetObject(key);

        T entityReference = obj.GetComponent<T>();

        if (entityReference == null)
        {
            throw new NullReferenceException("Object " + obj.name + " doesn't have a component of type " + typeof(T).Name);
        }

        entityReference.CreateReferences();

        return entityReference;
    }	

    protected virtual void CreateReferences() { }
    
    public void Serialize()
    {
        throw new NotImplementedException("Implement data serialization here");
    }
    public void Deserialize()
    {
        throw new NotImplementedException("Implement data deserialization here");
    }
    public bool DataExists(string key)
    {
        return _data.ContainsKey(key);
    }
    public void SetData(string key, object obj)
    {
        if (_data.ContainsKey(key))
        {
            _data[key] = obj;
        }
        else
        {
            _data.Add(key, obj);
        }
    }
    public object GetData(string key)
    {
        return _data[key];
    }
    private void Die()
    {
        if (_isDead)
            return;

        _isDead = true;

        OnDead();
    }
    protected virtual void OnDead()
    {
        GameObject.Destroy(gameObject);
    }
    protected virtual void OnDestroy() {  }

    public abstract string PrefabName { get; }
}
