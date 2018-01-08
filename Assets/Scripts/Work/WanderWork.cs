using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderWork : Work {

    public WanderWork(Vector2 anchor)
    {
        this.anchor = anchor;
    }

    private readonly Vector2 anchor;

    private IWork currentWork;

    private float WANDER_RADIUS = 5;

    public override void Update(WorkableEntity caller)
    {
        if(currentWork == null)
        {
            DoWalk();
        }

        if (currentWork.IsDone(caller))
        {
            if(currentWork is MovePointWork)
            {
                DoWait();        
            }
            else
            {
                DoWalk();
            }
        }

        currentWork.Update(caller);
    }
    private void DoWalk()
    {
        currentWork = new MovePointWork(MovePointWork.GetRandomPosition(anchor, WANDER_RADIUS));
    }
    private void DoWait()
    {
        currentWork = new Wait();
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
