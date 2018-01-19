/// <summary>
/// Object container used for data collections like quadtrees and spatialhashes.
/// </summary>
/// <typeparam name="T1">Object type</typeparam>
/// <typeparam name="T2">Key type</typeparam>
public class DataEntry<T1, T2>
{
    private DataEntry() { }
    public DataEntry(T1 obj, T2 key)
    {
        this.obj = obj;
        this.key = key;
    }

    public T1 Object { get { return obj; } }
    public T2 Key { get { return key; } }

    private readonly T1 obj;
    private readonly T2 key;
}