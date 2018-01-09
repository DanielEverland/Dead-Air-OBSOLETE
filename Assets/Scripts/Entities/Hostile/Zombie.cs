using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : WorkableEntity
{
    public override string PrefabName { get { return "Zombie"; } }
    public override WorkManager WorkManager { get { return WorkManager.ZombieWorkManager; } }
    
    /// <summary>
    /// Time between receiving new work and communicating it to other zombies
    /// </summary>
    private float COMMUNICATION_DELAY = 0.3f;

    /// <summary>
    /// Range in meters that zombies can communicate with each other
    /// </summary>
    private float COMMUNICATION_RANGE = 5;
    
    protected override void OnStartedWork()
    {
        if(CurrentWork is IHordeAbleWork)
            ActionManager.DelayedCallback(COMMUNICATION_DELAY, CommunicateNewWork, x => { return x == CurrentWork; }, CurrentWork);
    }
    private void CommunicateNewWork()
    {
        List<Zombie> zombiesWithinRange = Game.GetEntitiesWithinRange<Zombie>(transform.position, COMMUNICATION_RANGE);

        for (int i = 0; i < zombiesWithinRange.Count; i++)
        {
            zombiesWithinRange[i].ReceiveCommunicatedWork(CurrentWork);
        }
    }
    private void ReceiveCommunicatedWork(IWork work)
    {
        if (!(work is IHordeAbleWork))
            return;

        IHordeAbleWork hordableWork = work as IHordeAbleWork;

        if(!hordableWork.Compare(CurrentWork))
        {
            AssignWork(work);
        }
    }
}
