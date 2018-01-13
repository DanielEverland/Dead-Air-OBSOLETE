using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityCard : MonoBehaviour {

    private List<IEntityCardBehaviour> elements;

    private void Awake()
    {
        elements = new List<IEntityCardBehaviour>(GetComponentsInChildren<IEntityCardBehaviour>());
    }
    public void Initialize(Data data)
    {
        foreach (IEntityCardBehaviour behaviour in elements)
        {
            behaviour.Initialize(data);
        }
    }

    public enum DataTypes
    {
        None = 0,

        Name = 2,
        EntityCount = 4,
        MaxCurrentHealth = 8,
        MinCurrentHealth = 16,
        MaxMaxHealth = 32,
        MinMaxHealth = 64,
    }
    public class Data
    {
        private Data() { }

        public Data(IEnumerable<Entity> selectedEntities)
        {
            this.selectedEntities = new List<Entity>(selectedEntities);

            //Initialize default values if needed
        }

        public IEnumerable<Entity> SelectedEntities { get { return selectedEntities; } }
        private readonly List<Entity> selectedEntities;

        private Dictionary<DataTypes, object> data = new Dictionary<DataTypes, object>();

        public int MaxMaxHealth
        {
            get
            {
                return GetData<int>(DataTypes.MaxMaxHealth);
            }
            set
            {
                SetData(DataTypes.MaxMaxHealth, value);
            }
        }
        public int MinMaxHealth
        {
            get
            {
                return GetData<int>(DataTypes.MinMaxHealth);
            }
            set
            {
                SetData(DataTypes.MinMaxHealth, value);
            }
        }
        public int MaxCurrentHealth
        {
            get
            {
                return GetData<int>(DataTypes.MaxCurrentHealth);
            }
            set
            {
                SetData(DataTypes.MaxCurrentHealth, value);
            }
        }
        public int MinCurrentHealth
        {
            get
            {
                return GetData<int>(DataTypes.MinCurrentHealth);
            }
            set
            {
                SetData(DataTypes.MinCurrentHealth, value);
            }
        }
        public string Name
        {
            get
            {
                return GetData<string>(DataTypes.Name);
            }
            set
            {
                SetData(DataTypes.Name, value);
            }
        }

        public int EntityCount
        {
            get
            {
                return GetData<int>(DataTypes.EntityCount);
            }
            set
            {
                SetData(DataTypes.EntityCount, value);
            }
        }

        public bool CanCast<T>(DataTypes dataType)
        {
            object result = null;

            try
            {
                result = System.Convert.ChangeType(GetData<object>(dataType), typeof(T));
            }
            catch
            {

            }

            return result != null;
        }
        public bool HasData(DataTypes dataType)
        {
            return data.ContainsKey(dataType);
        }
        public void SetData(DataTypes dataType, object value)
        {
            if (data.ContainsKey(dataType))
            {
                data[dataType] = value;
            }
            else
            {
                data.Add(dataType, value);
            }
        }
        public T GetData<T>(DataTypes dataType)
        {
            if (data.ContainsKey(dataType))
            {
                try
                {
                    return (T)System.Convert.ChangeType(data[dataType], typeof(T));
                }
                catch(System.InvalidCastException)
                {
                    throw new System.InvalidCastException("Tried to cast data from " + dataType + " however, it cannot be cast as " + typeof(T) + "\n" + data[dataType].ToString());
                }
            }

            return default(T);
        }
    }
}
