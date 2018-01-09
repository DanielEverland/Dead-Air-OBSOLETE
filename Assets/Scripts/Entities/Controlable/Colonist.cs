using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colonist : WorkableEntity {

    public override string Name { get { return "Colonist"; } }
    public override string PrefabName { get { return "Colonist"; } }

    public override WorkManager WorkManager { get { return WorkManager.PlayerWorkManager; } }

    protected override EntityPriorityLevel PriorityLevel { get { return EntityPriorityLevel.Colonist; } }

    protected new virtual void Update()
    {
        base.Update();
        
        if(CurrentWork == null)
        {
            Wander();
        }
    }
    private void Wander()
    {
        AssignWork(new WanderWork(transform.position));
    }
}
