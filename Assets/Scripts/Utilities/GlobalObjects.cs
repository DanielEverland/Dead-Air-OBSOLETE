using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalObjects : ScriptableObject {

    private static GlobalObjects Instance
    {
        get
        {
            if (instance == null)
                instance = Resources.Load<GlobalObjects>("Global Objects");

            return instance;
        }
    }
    private static Dictionary<string, Object> QuickLookup
    {
        get
        {
            if (quickLookup == null)
                quickLookup = new Dictionary<string, Object>(Instance.references.ToDictionary(x => x.Key, y => y.Object));

            return quickLookup;
        }
    }

    private static GlobalObjects instance;
    private static Dictionary<string, Object> quickLookup;


    public int Count { get { return references.Count; } }
    public IList<ObjectReference> References { get { return references; } }
    
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
    public static Object GetObject(string key)
    {
        return QuickLookup[key];
    }

    [System.Serializable]
	public struct ObjectReference
    {
        public string Key;
        public Object Object;
    }
}

