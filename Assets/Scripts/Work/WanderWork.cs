using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderWork : Work {

    public WanderWork(Vector2 anchor)
    {
        this.anchor = anchor;
    }

    private readonly Vector2 anchor;
    
    private float WANDER_RADIUS = 5;

    public override void Update(WorkableEntity caller)
    {
        if(caller.CurrentWork == null)
        {
            DoWalk(caller);
        }

        if (caller.CurrentWork.IsDone(caller))
        {
            if(caller.CurrentWork is MovePointWork)
            {
                DoWait(caller);        
            }
            else
            {
                DoWalk(caller);
            }
        }
    }
    private void DoWalk(WorkableEntity caller)
    {
        caller.AssignWork(new MovePointWork(MovePointWork.GetRandomPosition(anchor, WANDER_RADIUS)));
    }
    private void DoWait(WorkableEntity caller)
    {
        caller.AssignWork(new Wait());
    }
    public override bool IsDone(WorkableEntity caller)
    {
        return caller.WorkManager.HasWork;
    }

    private class Wait : Work
    {
        public Wait()
        {
            waitTime = Random.Range(MIN_WAIT_TIME, MAX_WAIT_TIME);
        }

        private const float MIN_WAIT_TIME = 1;
        private const float MAX_WAIT_TIME = 3;

        private readonly float waitTime;

        private float currentWaitTime;

        public override bool IsDone(WorkableEntity caller)
        {
            return currentWaitTime >= waitTime;
        }
        public override void Update(WorkableEntity caller)
        {
            currentWaitTime += Time.deltaTime;
        }
    }
}
