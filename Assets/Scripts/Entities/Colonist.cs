using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colonist : WorkableEntity {

    public override string PrefabName { get { return "Colonist"; } }

    protected override WorkManager WorkManager { get { return WorkManager.PlayerWorkManager; } }

    private const float WANDER_RADIUS = 5;

    protected Vector2? wanderAnchor;

    protected new virtual void Update()
    {
        base.Update();
        
        if(CurrentWork == null)
        {
            if (!wanderAnchor.HasValue)
            {
                wanderAnchor = transform.position;
            }

            Wander();
        }
    }
    private void Wander()
    {
        AssignWork(new MoveWork(MoveWork.GetRandomPosition(wanderAnchor.Value, WANDER_RADIUS)));
    }
    protected override void OnReceivedWorkFromManager()
    {
        wanderAnchor = null;
    }
}
