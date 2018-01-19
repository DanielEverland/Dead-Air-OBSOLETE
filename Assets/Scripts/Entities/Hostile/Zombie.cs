using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : WorkableEntity
{
    public override string Name { get { return "Zombie"; } }
    public override string PrefabName { get { return "Zombie"; } }
    public override WorkManager WorkManager { get { return WorkManager.ZombieWorkManager; } }

    protected override EntityPriorityLevel PriorityLevel { get { return EntityPriorityLevel.Enemy; } }
    
    /// <summary>
    /// Time between receiving new work and communicating it to other zombies
    /// </summary>
    private float COMMUNICATION_DELAY = 0.3f;

    /// <summary>
    /// Range in meters that zombies can communicate with each other
    /// </summary>
    private float COMMUNICATION_RANGE = 5;
    
    protected override void OnSightEnter(Entity entity)
    {
        if(entity is Zombie)
        {
            CommunicateWithZombie((Zombie)entity);
        }
        else if(entity is Colonist)
        {
            EngageColonist(entity as Colonist);
        }
    }
    private void EngageColonist(Colonist colonist)
    {
        if (!(CurrentWork is MoveDirectionWork))
            return;

        AssignWork(new FollowWork(colonist));

        Game.GetVisibleEntitiesWithinRange<Zombie>(Position, COMMUNICATION_RANGE).ForEach(x => x.EngageColonist(colonist));
    }
    private void CommunicateWithZombie(Zombie zombie)
    {
        if (!HasWork)
            return;

        if(CurrentWork is IHordeAbleWork)
        {
            IHordeAbleWork hordeAbleWork = CurrentWork as IHordeAbleWork;

            if (!hordeAbleWork.Compare(zombie.CurrentWork))
            {
                zombie.AssignWork(CurrentWork);
            }
        }
    }
}
