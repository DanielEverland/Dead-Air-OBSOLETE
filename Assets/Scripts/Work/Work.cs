using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Work : IWork {

    public WorkableEntity Owner { get { return owner; } }

    private WorkableEntity owner;

    public void SetOwner(WorkableEntity owner)
    {
        this.owner = owner;
    }
    public void Poll()
    {
        if (owner == null)
        {
            Debug.LogError("NO OWNER ASSIGNED");
            return;
        }            

        Update();

        if (IsDone())
        {
            owner.StopWorking();
        }
    }
    protected virtual void Update() { }
    public abstract bool IsDone();
}
