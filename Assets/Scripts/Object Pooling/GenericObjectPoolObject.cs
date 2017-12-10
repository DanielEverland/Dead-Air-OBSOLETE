using System.Linq;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectPoolReference.asset", menuName = "Object Pool Reference", order = Utility.CONTEXT_MENU_ORDER)]
public class GenericObjectPoolObject : ScriptableObject {
        
    public int Count { get { return references.Count; } }
    public IList<ObjectReference> References { get { return references; } }

    private Dictionary<string, Object> QuickLookup
    {
        get
        {
            if(quickLookup == null)
            {
                quickLookup = new Dictionary<string, Object>(references.ToDictionary(x => x.Key, y => y.Object));
            }

            return quickLookup;
        }
    }

    private Dictionary<string, Object> quickLookup;

    [SerializeField]
    private List<ObjectReference> references = new List<ObjectReference>();
        
    public ObjectReference GetReference(int index)
    {
        return references[index];
    }
    public void SetReference(int index, ObjectReference reference)
    {
        references[index] = reference;
    }    
    public T GetObject<T>(string key) where T : Object
    {
        return (T)QuickLookup[key];
    }
    public Object GetObject(string key)
    {
        return QuickLookup[key];
    }

    [System.Serializable]
    public struct ObjectReference
    {
        public string Key;
        public Object Object;
        public int Instances;
    }
}
