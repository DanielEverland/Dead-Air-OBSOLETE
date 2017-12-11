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

    protected override void Update()
    {
        if(currentWork == null)
        {
            DoWalk();

            currentWork.SetOwner(Owner);
        }

        if (currentWork.IsDone())
        {
            if(currentWork is MoveWork)
            {
                DoWait();        
            }
            else
            {
                DoWalk();
            }

            currentWork.SetOwner(Owner);
        }

        currentWork.Poll();
    }
    private void DoWalk()
    {
        currentWork = new MoveWork(MoveWork.GetRandomPosition(anchor, WANDER_RADIUS));
    }
    private void DoWait()
    {
        currentWork = new Wait();
    }
    public override bool IsDone()
    {
        return Owner.WorkManager.HasWork;
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

        public override bool IsDone()
        {
            return currentWaitTime >= waitTime;
        }
        protected override void Update()
        {
            currentWaitTime += Time.deltaTime;
        }
    }
}
