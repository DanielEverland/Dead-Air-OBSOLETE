using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDirectionWork : Work, IHordeAbleWork
{
    public MoveDirectionWork(Vector2 direction)
    {
        this.direction = direction.normalized;
    }

    private readonly Vector2 direction;

    public override void Update(WorkableEntity caller)
    {
        caller.SetTargetPosition((Vector2)caller.transform.position + direction);
    }
    public override bool IsDone(WorkableEntity caller)
    {
        return false;
    }
    public bool Compare(IWork other)
    {
        if(other is MoveDirectionWork)
        {
            MoveDirectionWork otherDirection = other as MoveDirectionWork;

            return otherDirection.direction == this.direction;
        }

        return false;
    }
}
