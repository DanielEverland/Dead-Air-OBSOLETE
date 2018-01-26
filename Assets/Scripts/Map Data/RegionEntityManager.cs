using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RegionEntityManager {

    private static Dictionary<Entity, Region> _entityRegionLookup = new Dictionary<Entity, Region>();

    public static void Add(Entity entity)
    {
        if (_entityRegionLookup.ContainsKey(entity))
        {
            Poll(entity);
            return;
        }

        Region region = RegionManager.GetRegion(entity.CellPosition);

        if (region == null)
            throw new System.NullReferenceException("Cannot find region for entity");

        entity.SetRegion(region);
        region.AddEntity(entity);
        _entityRegionLookup.Add(entity, region);
    }
    /// <summary>
    /// Moves entity to a different region if required
    /// </summary>
    public static void Poll(Entity entity)
    {
        if(!_entityRegionLookup.ContainsKey(entity))
        {
            Add(entity);
            return;
        }

        Region region = _entityRegionLookup[entity];
        
        if(!region.ContainsCell(entity.CellPosition))
        {
            entity.SetRegion(null);
            region.RemoveEntity(entity);
            _entityRegionLookup.Remove(entity);

            Add(entity);
        }        
    }
    public static void Remove(Entity entity)
    {
        if (_entityRegionLookup.ContainsKey(entity))
        {
            Region region = _entityRegionLookup[entity];

            entity.SetRegion(null);
            region.RemoveEntity(entity);
            _entityRegionLookup.Remove(entity);
        }
    }
}
