using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionManager {

    public static IEnumerable<ISeeingEntity> ActiveVisibleEntities { get { return activeVisibleEntities; } }

    private static Dictionary<ISeeingEntity, HashSet<Entity>> lastVisionMatrix = new Dictionary<ISeeingEntity, HashSet<Entity>>();
    private static List<ISeeingEntity> activeVisibleEntities = new List<ISeeingEntity>();

    public static void Update()
    {
        Dictionary<ISeeingEntity, HashSet<Entity>> newVisionMatrix = new Dictionary<ISeeingEntity, HashSet<Entity>>();

        foreach (ISeeingEntity entity in activeVisibleEntities)
        {
            List<Entity> visibleEntities = Game.GetVisibleEntitiesWithinRange<Entity>(entity.Position, entity.SightRange);
            
            if(visibleEntities.Count > 0)
                newVisionMatrix.Add(entity, new HashSet<Entity>());

            if (lastVisionMatrix.ContainsKey(entity))
            {
                for (int i = 0; i < visibleEntities.Count; i++)
                {
                    //Other entity was visible last frame
                    if (lastVisionMatrix[entity].Contains(visibleEntities[i]))
                    {
                        entity.SightStay(visibleEntities[i]);
                    }
                    else
                    {
                        entity.SightEnter(visibleEntities[i]);
                    }

                    newVisionMatrix[entity].Add(visibleEntities[i]);
                }

                foreach (Entity oldVisibleEntity in lastVisionMatrix[entity])
                {
                    if (!newVisionMatrix[entity].Contains(oldVisibleEntity))
                        entity.SightLeave(oldVisibleEntity);
                }
            }
        }

        lastVisionMatrix = newVisionMatrix;
    }
	public static void AddVisibleEntity(ISeeingEntity entity)
    {
        activeVisibleEntities.Add(entity);
    }
    public static void RemoveVisibleEntity(ISeeingEntity entity)
    {
        activeVisibleEntities.Remove(entity);
    }
}
