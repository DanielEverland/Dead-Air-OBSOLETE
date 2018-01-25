using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RegionConnectionManager {

    private static Dictionary<int, HashSet<Region>> _connections = new Dictionary<int, HashSet<Region>>();

    public static void Remove(Region region)
    {
        foreach (Region.Connection connection in region.Connections)
        {
            if(_connections.ContainsKey(connection))
            {
                _connections[connection].Remove(region);

                if (_connections[connection].Count == 0)
                    _connections.Remove(connection);
            }                
        }
    }
    public static void Add(Region region)
    {
        foreach (Region.Connection connection in region.Connections)
        {
            if (!_connections.ContainsKey(connection))
                _connections.Add(connection, new HashSet<Region>());

            if(!_connections[connection].Contains(region))
                _connections[connection].Add(region);
        }
    }
    public static IEnumerable<Region> GetRegions(Region.Connection connection)
    {
        if (!_connections.ContainsKey(connection))
            return default(IEnumerable<Region>);

        return _connections[connection];
    }
    public static IEnumerable<Region> GetRegions(int hash)
    {
        if (!_connections.ContainsKey(hash))
            return default(IEnumerable<Region>);

        return _connections[hash];
    }
}
