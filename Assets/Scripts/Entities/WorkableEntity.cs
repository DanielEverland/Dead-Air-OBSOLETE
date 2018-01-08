using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WorkableEntity : MovableEntity
{
    public IWork CurrentWork { get { return currentWork; } }

    public abstract WorkManager WorkManager { get; }

    private IWork currentWork;

    protected new virtual void Update()
    {
        base.Update();

        PollWork();
    }
    private void PollWork()
    {
        if(currentWork != null)
        {
            currentWork.Update(this);
        }
        else if(WorkManager.HasWork)
        {
            AssignWork(WorkManager.GetWork());

            OnReceivedWorkFromManager();
        }
    }
    public void StopWorking()
    {
        currentWork = null;

        OnFinishedWork();
    }
    public void AssignWork(IWork work)
    {
        StopWorking();

        currentWork = work;
        
        OnStartedWork();
    }
    protected virtual void OnStartedWork() { }
    protected virtual void OnFinishedWork() { }
    protected virtual void OnReceivedWorkFromManager() { }
}
