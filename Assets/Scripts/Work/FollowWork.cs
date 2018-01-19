using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowWork : Work, IHordeAbleWork
{
    public FollowWork(Entity entity)
    {
        Target = entity;
    }

    public Entity Target { get; set; }

    public override void Update(WorkableEntity caller)
    {
        caller.SetTargetPosition(Target.transform.position);
    }
    public bool Compare(IWork other)
    {
        if(other is FollowWork)
        {
            FollowWork otherFollow = other as FollowWork;

            return otherFollow.Target == this.Target;
        }

        return false;
    }

    public override bool IsDone(WorkableEntity caller)
    {
        return false;
    }
}
